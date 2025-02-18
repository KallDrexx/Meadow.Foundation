﻿using Meadow.Foundation.Sensors.Base;
using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow.Foundation.Sensors.Light
{
    /// <summary>
    /// Represents a Temt6000 light sensor
    /// </summary>
    public class Temt6000 : AnalogSamplingBase
    {
        /// <summary>
        /// Creates a new Temt6000 object
        /// </summary>
        /// <param name="pin">The analog pin</param>
        /// <param name="sampleCount">The sample count</param>
        /// <param name="sampleInterval">The sample interval</param>
        /// <param name="voltage">The peak voltage</param>
        public Temt6000(IPin pin, int sampleCount = 5, TimeSpan? sampleInterval = null, Voltage? voltage = null)
            : this(pin.CreateAnalogInputPort(sampleCount, sampleInterval ?? TimeSpan.FromMilliseconds(40), voltage ?? new Voltage(3.3)))
        { }

        /// <summary>
        /// Creates a new Temt6000 driver
        /// </summary>
        /// <param name="port"></param>
        public Temt6000(IAnalogInputPort port) : base(port) 
        { }
    }
}