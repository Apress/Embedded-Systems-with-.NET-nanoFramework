// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System.Device.Adc;

namespace PoolController
{
    public class ChlorineSensor
    {
        private readonly AdcChannel _adc;

        private readonly float _gain;
        private readonly float _shuntResistor;

        public ChlorineSensor(AdcController adcController, int adcChannel)
        {
            _adc = adcController.OpenChannel(adcChannel);
        }

        public float ReadChlorineLevel(int samples = 10)
        {
            float total = 0f;

            return 0;
        }
    }
}
