// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System.Device.Gpio;

namespace PoolController
{
    public class TurbidityControllerConfig
    {
        public float Threshold { get; set; } = 1.0f; // Example threshold (NTU)
        public GpioPin FilterGpioPin { get; set; }
    }
}
