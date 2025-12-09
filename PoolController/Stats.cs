// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class Stats
    {
        private float _phMin = float.MaxValue;
        private float _phMax = float.MinValue;
        private float _phSum = 0;
        private int _phCount = 0;

        private float _chlorineMin = float.MaxValue;
        private float _chlorineMax = float.MinValue;
        private float _chlorineSum = 0;
        private int _chlorineCount = 0;

        private float _turbidityMin = float.MaxValue;
        private float _turbidityMax = float.MinValue;
        private float _turbiditySum = 0;
        private int _turbidityCount = 0;

        public float PhMin => _phCount > 0 ? _phMin : float.NaN;
        public float PhMax => _phCount > 0 ? _phMax : float.NaN;
        public float PhAverage => _phCount > 0 ? _phSum / _phCount : float.NaN;

        public float ChlorineMin => _chlorineCount > 0 ? _chlorineMin : float.NaN;
        public float ChlorineMax => _chlorineCount > 0 ? _chlorineMax : float.NaN;
        public float ChlorineAverage => _chlorineCount > 0 ? _chlorineSum / _chlorineCount : float.NaN;

        public float TurbidityMin => _turbidityCount > 0 ? _turbidityMin : float.NaN;
        public float TurbidityMax => _turbidityCount > 0 ? _turbidityMax : float.NaN;
        public float TurbidityAverage => _turbidityCount > 0 ? _turbiditySum / _turbidityCount : float.NaN;

        public void Reset()
        {
            _phMin = float.MaxValue;
            _phMax = float.MinValue;
            _phSum = 0;
            _phCount = 0;

            _chlorineMin = float.MaxValue;
            _chlorineMax = float.MinValue;
            _chlorineSum = 0;
            _chlorineCount = 0;

            _turbidityMin = float.MaxValue;
            _turbidityMax = float.MinValue;
            _turbiditySum = 0;
            _turbidityCount = 0;
        }

        public void AddPh(float value)
        {
            if (value < _phMin)
            {
                _phMin = value;
            }

            if (value > _phMax)
            {
                _phMax = value;
            }

            _phSum += value;
            _phCount++;
        }

        public void AddChlorine(float value)
        {
            if (value < _chlorineMin)
            {
                _chlorineMin = value;
            }

            if (value > _chlorineMax)
            {
                _chlorineMax = value;
            }

            _chlorineSum += value;
            _chlorineCount++;
        }

        public void AddTurbidity(float value)
        {
            if (value < _turbidityMin)
            {
                _turbidityMin = value;
            }

            if (value > _turbidityMax)
            {
                _turbidityMax = value;
            }

            _turbiditySum += value;
            _turbidityCount++;
        }
    }
}
