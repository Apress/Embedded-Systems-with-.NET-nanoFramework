// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Device.Adc;

namespace PoolController
{
    public class PhSensor
    {
        private const float _phMin = 0; // pH at 4mA
        private const float _phMax = 14; // pH at 20mA

        private readonly AdcChannel _adc;
        private readonly float _shuntResistor;
        private readonly float _gain; // INA196A gain

        public PhSensor(
            AdcController adcController,
            int adcChannel,
            float shuntResistor = 4.99f,
            float gain = 20f)
        {
            _adc = adcController.OpenChannel(adcChannel);
            _shuntResistor = shuntResistor;
            _gain = gain;
        }

        public float ReadPh(int samples = 10)
        {
            float total = 0f;

            for (int i = 0; i < samples; i++)
            {
                total += _adc.ReadValue();
            }

            float averageRaw = total / samples;

            // Convert ADC reading to voltage (assume 12-bit ADC, 3.3V ref)
            float voltage = (averageRaw / 4095f) * 3.3f;

            // Calculate current in mA: I = Vout / (R_shunt * Gain)
            float current_mA = voltage / (_shuntResistor * _gain) * 1000f;

            // Map 4–20mA to pH range
            float phValue = _phMin + ((current_mA - 4f) / 16f) * (_phMax - _phMin);
            phValue = Math.Max(_phMin, Math.Min(_phMax, phValue));

            // Return pH
            return phValue;
        }
    }
}
