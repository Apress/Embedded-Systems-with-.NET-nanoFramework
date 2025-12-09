// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Device.Adc;

namespace PoolController
{
    public class TurbiditySensor
    {
        private readonly AdcChannel _adc;
        private readonly float _referenceVoltage;
        private readonly int _adcResolution;

        // Example calibration values (customize as needed)
        private const float _turbidityMin = 0.0f; // NTU at max voltage
        private const float _turbidityMax = 5.0f; // NTU at min voltage

        public TurbiditySensor(
            AdcController adcController,
            int adcChannel,
            float referenceVoltage = 3.3f,
            int adcResolution = 4095)
        {
            _adc = adcController.OpenChannel(adcChannel);
            _referenceVoltage = referenceVoltage;
            _adcResolution = adcResolution;
        }

        public float ReadTurbidity(int samples = 10)
        {
            float total = 0f;
            for (int i = 0; i < samples; i++)
            {
                total += _adc.ReadValue();
            }
            float averageRaw = total / samples;

            // Convert ADC reading to voltage
            float voltage = (averageRaw / _adcResolution) * _referenceVoltage;

            // Example linear mapping: higher voltage = lower turbidity (NTU)
            // Adjust calibration as needed for your sensor
            float turbidity = _turbidityMax - ((voltage / _referenceVoltage) * (_turbidityMax - _turbidityMin));
            turbidity = Math.Max(_turbidityMin, Math.Min(_turbidityMax, turbidity));

            return turbidity;
        }
    }
}
