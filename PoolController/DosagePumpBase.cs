// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using UnitsNet;

namespace PoolController
{
    public class DosagePumpBase
    {
        public ElectricCurrent PumpMaxCurrent { get; set; } = ElectricCurrent.FromMilliamperes(20.0);
        public ElectricCurrent PumpMinCurrent { get; set; } = ElectricCurrent.FromMilliamperes(4.0);
    }
}
