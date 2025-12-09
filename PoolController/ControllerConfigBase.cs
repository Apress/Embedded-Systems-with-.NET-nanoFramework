// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class ControllerConfigBase
    {
        public float Kd { get; set; } = 0.05f;
        public float Ki { get; set; } = 0.1f;
        public float Kp { get; set; } = 2.0f;
    }
}