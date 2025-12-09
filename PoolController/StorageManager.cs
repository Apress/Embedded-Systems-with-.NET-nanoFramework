// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PoolController
{
    public static class StorageManager
    {
        private const string GeneralConfigFile = @"I:\sys\general_config.bin";
        private const string PhControllerConfigFile = @"I:\sys\ph_controller_config.bin";
        private const string ChlorineControllerConfigFile = @"I:\sys\chlorine_controller_config.bin";
        private const string TurbidityControllerConfigFile = @"I:\sys\turbidity_controller_config.bin";

        public static void SaveGeneralConfiguration(BaseConfiguration config)
        {
            byte[] encodedConfig = BinaryFormatter.Serialize(config);
            File.WriteAllBytes(GeneralConfigFile, encodedConfig);
        }

        public static BaseConfiguration TryLoadGeneralConfiguration()
        {
            if (!File.Exists(GeneralConfigFile))
            {
                return null;
            }

            byte[] encodedConfig = File.ReadAllBytes(GeneralConfigFile);

            return BinaryFormatter.Deserialize(encodedConfig) as BaseConfiguration;
        }

        public static void SavePhControllerConfig(PhControllerConfig config)
        {
            byte[] encodedConfig = BinaryFormatter.Serialize(config);
            File.WriteAllBytes(PhControllerConfigFile, encodedConfig);
        }

        public static PhControllerConfig TryLoadPhControllerConfig()
        {
            if (!File.Exists(PhControllerConfigFile))
            {
                return null;
            }

            byte[] encodedConfig = File.ReadAllBytes(PhControllerConfigFile);
            return BinaryFormatter.Deserialize(encodedConfig) as PhControllerConfig;
        }

        public static void SaveTurbidityControllerConfig(TurbidityControllerConfig config)
        {
            byte[] encodedConfig = BinaryFormatter.Serialize(config);
            File.WriteAllBytes(TurbidityControllerConfigFile, encodedConfig);
        }

        public static TurbidityControllerConfig TryLoadTurbidityControllerConfig()
        {
            if (!File.Exists(TurbidityControllerConfigFile))
            {
                return null;
            }

            byte[] encodedConfig = File.ReadAllBytes(TurbidityControllerConfigFile);
            return BinaryFormatter.Deserialize(encodedConfig) as TurbidityControllerConfig;
        }

        public static void SaveChlorineControllerConfig(ChlorineControllerConfig config)
        {
            byte[] encodedConfig = BinaryFormatter.Serialize(config);
            File.WriteAllBytes(ChlorineControllerConfigFile, encodedConfig);
        }

        public static ChlorineControllerConfig TryLoadChlorineControllerConfig()
        {
            if (!File.Exists(ChlorineControllerConfigFile))
            {
                return null;
            }

            byte[] encodedConfig = File.ReadAllBytes(ChlorineControllerConfigFile);
            return BinaryFormatter.Deserialize(encodedConfig) as ChlorineControllerConfig;
        }
    }
}
