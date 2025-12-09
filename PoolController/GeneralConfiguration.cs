// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

namespace PoolController
{
    public class GeneralConfiguration
    {
        public static int SampleRate { get; set; }
        public static float PhLowThreshold { get; set; }
        public static float PhHighThreshold { get; set; }
        public static float ChLowThreshold { get; set; }
        public static float TurbidityThreshold { get; set; }
        public static float PoolVolume { get; set; }

        public static void Update(BaseConfiguration newConfig)
        {
            SampleRate = newConfig.SampleRate;
            PhLowThreshold = newConfig.PhLowThreshold;
            PhHighThreshold = newConfig.PhHighThreshold;
            ChLowThreshold = newConfig.ChLowThreshold;
            TurbidityThreshold = newConfig.TurbidityThreshold;
            PoolVolume = newConfig.PoolVolume;
        }

        public static BaseConfiguration ToBaseConfiguration()
        {
            return new BaseConfiguration
            {
                SampleRate = SampleRate,
                PhLowThreshold = PhLowThreshold,
                PhHighThreshold = PhHighThreshold,
                ChLowThreshold = ChLowThreshold,
                TurbidityThreshold = TurbidityThreshold,
                PoolVolume = PoolVolume
            };
        }
    }

}
