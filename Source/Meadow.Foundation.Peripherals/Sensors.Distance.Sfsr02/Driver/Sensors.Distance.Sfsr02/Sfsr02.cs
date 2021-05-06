﻿using System;
using System.Threading;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Units;

namespace Meadow.Foundation.Sensors.Distance
{
    /// <summary>
    /// Sfsr02 Distance Sensor
    /// </summary>
    public class Sfsr02:
        FilterableChangeObservable<CompositeChangeResult<Length>, Length>, 
        IRangeFinder
    {

		/// <summary>
        /// Raised when an received a rebound trigger signal
        /// </summary>
        public event EventHandler<CompositeChangeResult<Length>> DistanceUpdated;
        public event EventHandler<CompositeChangeResult<Length>> Updated;

        /// <summary>
        /// Returns current distance
        /// </summary>
        public Length Distance { get; private set; } = 0;

        /// <summary>
        /// Minimum valid distance in cm
        /// </summary>
        public double MinimumDistance => 2;

        /// <summary>
        /// Maximum valid distance in cm
        /// </summary>
        public double MaximumDistance => 450;


        /// <summary>
        /// Trigger/Echo Pin
        /// </summary>
        protected IBiDirectionalPort triggerEchoPort;

        protected long tickStart;

        /// <summary>
        /// Create a new SFSR02 object with an IO Device
        /// </summary>
        /// <param name="triggerEchoPin"></param>
        /// <param name="device"></param>
        public Sfsr02(IBiDirectionalController device, IPin triggerEchoPin) :
            this(device.CreateBiDirectionalPort(triggerEchoPin, false))
        { }

        /// <summary>
        /// Create a new SFSR02 object 
        /// </summary>
        /// <param name="triggerEchoPort"></param>
        public Sfsr02(IBiDirectionalPort triggerEchoPort)
        {
            this.triggerEchoPort = triggerEchoPort;

            this.triggerEchoPort.Changed += OnEchoPortChanged;
        }

        /// <summary>
        /// Sends a trigger signal
        /// </summary>
        public void MeasureDistance()
        {
            triggerEchoPort.Direction = PortDirectionType.Output;
            triggerEchoPort.State = false;
            Thread.Sleep(1); //smallest amount of time we can wait

            Distance = -1;

            // Raise trigger port to high for 20 micro-seconds
            triggerEchoPort.State = true;
            Thread.Sleep(1); //smallest amount of time we can wait

            // Start Clock
            tickStart = DateTime.Now.Ticks;
            // Trigger device to measure distance via sonic pulse
            triggerEchoPort.State = false;
        
            triggerEchoPort.Direction = PortDirectionType.Input;
        }

        private void OnEchoPortChanged(object sender, DigitalInputPortEventArgs e)
        {
            if (e.Value == true)
            {
                tickStart = DateTime.Now.Ticks;
                return;
            }

            // Calculate Difference
            var elapsed = DateTime.Now.Ticks - tickStart;

            // Return elapsed ticks
            // x10 for ticks to micro sec
            // divide by 58 for cm (assume speed of sound is 340m/s)
            var curDis = elapsed / 580;

            var oldDistance = Distance;
            Distance = new Length(curDis, Length.UnitType.Centimeters); 

            //debug - remove 
            Console.WriteLine($"{elapsed}, {curDis}, {Distance}, {DateTime.Now.Ticks}");

            //restore this before publishing to hide false results 
            //    if (CurrentDistance < MinimumDistance || CurrentDistance > MaximumDistance)
            //       CurrentDistance = -1;

            var result = new CompositeChangeResult<Length>(oldDistance, Distance);

            Updated?.Invoke(this, result);
            DistanceUpdated?.Invoke(this, result);
        }
    }
}