// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;

namespace PoolController
{
    public class DailySummary
    {
        public DateTime Timestamp { get; set; }
        public float PhMin { get; set; }
        public float PhMax { get; set; }
        public float ChlorineMin { get; set; }
        public float ChlorineMax { get; set; }
        public float TurbidityMin { get; set; }
        public float TurbidityMax { get; set; }
        public float TemperatureMin { get; set; }
        public float TemperatureMax { get; set; }
    }
}
