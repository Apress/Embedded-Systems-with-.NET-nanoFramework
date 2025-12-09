// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class ChlorineControllerConfig : ControllerConfigBase
    {
        // Target chlorine level in mg/L (ppm), default 1.5 ppm
        public float TargetChlorine { get; set; } = 1.5f;

        // Chemical strength (mg/mL or as appropriate for your dosing chemical)
        public float ChemicalStrength { get; set; } = 1.0f;
    }
}
