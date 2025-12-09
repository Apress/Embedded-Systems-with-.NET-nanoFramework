// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class BaseConfiguration
    {
        public int SampleRate { get; set; } = 60; // Default sample rate in seconds
        public float PhLowThreshold { get; set; } = 6.8f; // Default low pH threshold
        public float PhHighThreshold { get; set; } = 7.8f; // Default high pH threshold
        public float ChLowThreshold { get; set; } = 1.0f; // Default low chlorine threshold (mg/L)
        public float TurbidityThreshold { get; set; } = 1.0f; // Default turbidity threshold (NTU)
        public float PoolVolume { get; set; } = 8.0f; // Default pool volume in cubic meters
    }
}
