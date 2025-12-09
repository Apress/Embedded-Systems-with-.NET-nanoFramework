// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class PhControllerConfig : ControllerConfigBase
    {
        // default value is 7.2 pH
        public float TargetPh { get; set; } = 7.2f;
        public float ChemicalStrength { get; set; } = 1.0f; // mg/L per mL
    }
}
