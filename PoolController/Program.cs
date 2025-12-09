// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using nanoFramework.Azure.Devices.Client;
using nanoFramework.Json;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;

namespace PoolController
{
    public class Program
    {
        // Azure IoT Hub connection settings - Replace with your own values
        private const string IotBrokerAddress = "YOUR_IOT_HUB_NAME.azure-devices.net";
        private const string DeviceID = "YOUR_DEVICE_ID";
        private const string SasKey = "YOUR_DEVICE_SAS_KEY";

        public static Stats stats = new Stats();
        public static DeviceClient AzureClient;
        public static LoggerModule Logger;
        private static AdcController adcController;
        private static GpioController gpioController;
        private static PhSensor _phSensor;
        private static ChlorineSensor _chlorineSensor;
        private static TurbiditySensor _turbiditySensor;
        private static DosagePumpController PumpController;
        public static PhController PhController0;
        public static ChlorineController ChlorineController0;
        public static TurbidityController TurbidityController0;

        public static void Main()
        {
            gpioController = new GpioController();

            // Initialize logger
            Logger = new LoggerModule("I:\\pool_controller.log", true);
            Logger.LogBoot();

            LcdManager.Init();

            KeypadController.Init(gpioController);

            LoadConfigurations();

            InitializeControllers();

            // Initialize Azure IoT Hub client
            AzureClient = new DeviceClient(IotBrokerAddress,
                                           DeviceID,
                                           SasKey,
                                           nanoFramework.M2Mqtt.Messages.MqttQoSLevel.AtLeastOnce);

            AzureClient.AddMethodCallback(UpdateConfiguration);

            // Start all controllers
            PhController0.Start();
            ChlorineController0.Start();
            TurbidityController0.Start();

            Logger.LogSystemEvent("All controllers started");

            while (true)
            {
                // Read current sensor values from controllers
                float ph = PhController0.PhValue;
                float chlorine = ChlorineController0.ChlorineValue;
                float turbidity = TurbidityController0.TurbidityValue;

                // Update statistics
                stats.AddPh(ph);
                stats.AddChlorine(chlorine);
                stats.AddTurbidity(turbidity);

                // Simple local decision logic
                if (ph < GeneralConfiguration.PhLowThreshold
                    || ph > GeneralConfiguration.PhHighThreshold)
                {
                    Log($"pH out of range: {ph}");
                    TriggerPhDosingPump();
                }

                if (chlorine < GeneralConfiguration.ChLowThreshold)
                {
                    Log($"Chlorine low: {chlorine}");
                    TriggerChlorinePump();
                }

                if (turbidity > GeneralConfiguration.TurbidityThreshold)
                {
                    Log($"Turbidity high: {turbidity}");
                    TriggerTurbidityAlert();
                }

                // Log readings for traceability
                Log($"ph={ph}, chlorine={chlorine}, turbidity={turbidity}");

                // Send daily summary to Azure IoT Hub (simplified: send on each iteration)
                // In a real implementation, you'd want to send this once per day
                var summary = new DailySummary
                {
                    Timestamp = DateTime.UtcNow,
                    PhMin = stats.PhMin,
                    PhMax = stats.PhMax,
                    ChlorineMin = stats.ChlorineMin,
                    ChlorineMax = stats.ChlorineMax,
                    TurbidityMin = stats.TurbidityMin,
                    TurbidityMax = stats.TurbidityMax
                };

                try
                {
                    AzureClient.SendMessage(JsonConvert.SerializeObject(summary));
                }
                catch (Exception ex)
                {
                    Log($"Error sending telemetry to Azure: {ex.Message}");
                }

                Thread.Sleep(GeneralConfiguration.SampleRate * 1000); // Convert seconds to milliseconds
            }
        }

        private static void LoadConfigurations()
        {
            BaseConfiguration baseConfiguration = StorageManager.TryLoadGeneralConfiguration();

            if (baseConfiguration == null)
            {
                Log("No general configuration found, using defaults.");

                // If no configuration exists, create a new one with default values
                baseConfiguration = new BaseConfiguration();

                // Update the general configuration with default values
                GeneralConfiguration.Update(baseConfiguration);

                // Save the default configuration to storage
                StorageManager.SaveGeneralConfiguration(GeneralConfiguration.ToBaseConfiguration());
            }
            else
            {
                Log("General configuration loaded.");
                GeneralConfiguration.Update(baseConfiguration);
            }

            // Load pH controller configuration
            PhControllerConfig phControllerConfig = StorageManager.TryLoadPhControllerConfig();
            if (phControllerConfig == null)
            {
                Log("No pH controller configuration found, using defaults.");
                phControllerConfig = new PhControllerConfig();

                StorageManager.SavePhControllerConfig(phControllerConfig);
            }
            else
            {
                Log("pH controller configuration loaded.");
            }

            // Load chlorine controller configuration
            ChlorineControllerConfig chlorineControllerConfig = StorageManager.TryLoadChlorineControllerConfig();
            if (chlorineControllerConfig == null)
            {
                Log("No chlorine controller configuration found, using defaults.");

                chlorineControllerConfig = new ChlorineControllerConfig();
                StorageManager.SaveChlorineControllerConfig(chlorineControllerConfig);
            }
            else
            {
                Log("Chlorine controller configuration loaded.");
            }

            // Load turbidity controller configuration
            TurbidityControllerConfig turbidityControllerConfig = StorageManager.TryLoadTurbidityControllerConfig();
            if (turbidityControllerConfig == null)
            {
                Log("No turbidity controller configuration found, using defaults.");
                turbidityControllerConfig = new TurbidityControllerConfig();
                StorageManager.SaveTurbidityControllerConfig(turbidityControllerConfig);
            }
            else
            {
                Log("Turbidity controller configuration loaded.");
            }
        }

        private static void InitializeControllers()
        {
            // Initialize ADC controller
            adcController = new AdcController();

            // Create sensors on ADC channels 0, 1, 2 
            _phSensor = new PhSensor(adcController, 0);
            _chlorineSensor = new ChlorineSensor(adcController, 1);
            _turbiditySensor = new TurbiditySensor(adcController, 2);

            // create pumps controller
            PumpController = new DosagePumpController(1);

            // Create controllers using the sensors and their configs
            // pH controller
            PhController0 = new PhController(_phSensor,
                                             PumpController.Pumps[0],
                                             GeneralConfiguration.PoolVolume,
                                             StorageManager.TryLoadPhControllerConfig());

            // Chlorine controller
            ChlorineController0 = new ChlorineController(_chlorineSensor,
                                                         PumpController.Pumps[1],
                                                         GeneralConfiguration.PoolVolume,
                                                         StorageManager.TryLoadChlorineControllerConfig());

            // Turbidity controller
            TurbidityController0 = new TurbidityController(_turbiditySensor,
                                                           StorageManager.TryLoadTurbidityControllerConfig());
        }

        private static string UpdateConfiguration(int rid, string payload)
        {
            try
            {
                BaseConfiguration newConfig = (BaseConfiguration)JsonConvert.DeserializeObject(payload, typeof(BaseConfiguration));

                if (newConfig == null)
                {
                    return $"{{\"result\": \"Failed to parse new configuration\"}}";
                }
                else
                {
                    GeneralConfiguration.Update(newConfig);
                }

                return "{{\"result\": \"OK\"}}";
            }
            catch (Exception ex)
            {
                Log("Error parsing a configuration update: " + ex.Message);

                return $"{{\"result\": \"{ex.Message}\"}}";
            }
        }

        private static void TriggerTurbidityAlert()
        {
            // Log turbidity alert
            Logger.LogSystemEvent($"ALERT: High turbidity detected - {TurbidityController0.TurbidityValue} NTU");

            // Display alert on LCD
            LcdManager.ShowMessage("HIGH TURBIDITY!", $"{TurbidityController0.TurbidityValue:F2} NTU", TimeSpan.FromSeconds(5));

            // In a real system, you might want to:
            // - Send a notification
            // - Trigger an alarm
            // - Activate filtration system
        }

        private static void TriggerChlorinePump()
        {
            // The ChlorineController is already managing the pump automatically
            // This method can be used for manual override or additional logging
            Logger.LogSystemEvent($"Chlorine dosing triggered - Current: {ChlorineController0.ChlorineValue} mg/L");

            // The controller's PID loop will handle the actual dosing
            // No manual intervention needed as the controller is running
        }

        private static void Log(string v)
        {
            if (Logger != null)
            {
                Logger.Log(v);
            }
        }

        private static void TriggerPhDosingPump()
        {
            // The PhController is already managing the pump automatically
            // This method can be used for manual override or additional logging
            Logger.LogSystemEvent($"pH dosing triggered - Current: {PhController0.PhValue} pH");

            // The controller's PID loop will handle the actual dosing
            // No manual intervention needed as the controller is running
        }
    }
}
