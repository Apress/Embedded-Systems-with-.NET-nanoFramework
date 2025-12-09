// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using Iot.Device.Dac63004;
using nanoFramework.Azure.Devices.Shared;
using System;
using UnitsNet;

namespace PoolController
{
    public class DosagePump : DosagePumpBase
    {
        private ElectricCurrent _current;
        private readonly Dac63004 _dac;
        private readonly Channel _channel;
        private string _propertyName;

        public ElectricCurrent Current
        {
            get => _current;
            set => SetCurrent(value);
        }

        public string PropertyName
        {
            set => _propertyName = value;
        }

        internal DosagePump(Dac63004 dac, Channel channel)
        {
            _dac = dac ?? throw new ArgumentNullException();
            _channel = channel;
        }

        private void SetCurrent(ElectricCurrent current)
        {
            // validate range: 4-20 mA
            if (current.Milliamperes < PumpMinCurrent.Milliamperes
                || current.Milliamperes > PumpMaxCurrent.Milliamperes)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (_current.Value == current.Value)
            {
                // no change, nothing to do
                return;
            }
            // update current value
            _current = current;

            // convert to voltage absolute value (0-2.0 V) 
            // 4mA = 0.4 V, 20mA = 2.0 V
            double voltageValue = _current.Amperes * 100.0;

            // Convert to 12-bit value (0-4095)
            // max voltage is Vref x gain = 1.21 × 2.0 = 2.42 V
            int dacValue = (int)((voltageValue / 2.42) * 4095);

            _dac.SetChannelDataValue(_channel, dacValue);


            ReportToCloud();
        }

        private void ReportToCloud()
        {
            if (_propertyName is not null)
            {
                TwinCollection propertyToUpdate = new()
                {
                    { _propertyName, _current }
                };

                Program.AzureClient.UpdateReportedProperties(propertyToUpdate);
            }
        }
    }
}
