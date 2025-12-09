// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using UnitsNet;

namespace PoolController
{
    public class ChlorineController
    {
        private const float SafetyChlorineMax = 5.0f; // Adjust as needed

        private readonly ChlorineSensor _sensor;
        private readonly DosagePump _pump;
        private readonly float _poolVolume;
        private Thread _controlThread;
        private bool _running;
        private float _integral;
        private float _lastChlorineDelta;

        public ChlorineControllerConfig Config { get; set; }
        public float ChlorineValue { get; set; }

        public ChlorineController(
            ChlorineSensor sensor,
            DosagePump pump,
            float poolVolume,
            ChlorineControllerConfig config)
        {
            _sensor = sensor;
            _pump = pump;
            _poolVolume = poolVolume;
            Config = config;
        }

        public void Start()
        {
            if (_running)
            {
                // Already running
                return;
            }

            _running = true;
            _controlThread = new Thread(ControlLoop);
            _controlThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _controlThread.Join();
        }

        private void ControlLoop()
        {
            while (_running)
            {
                // read the chlorine value from the sensor
                ChlorineValue = _sensor.ReadChlorineLevel();

                if (CheckSafetyValue())
                {
                    // compute the difference from target chlorine
                    float chlorineDelta = Config.TargetChlorine - ChlorineValue;

                    // Simple PID control
                    _integral += chlorineDelta;
                    float derivative = chlorineDelta - _lastChlorineDelta;
                    float output = Config.Kp * chlorineDelta + Config.Ki * _integral + Config.Kd * derivative;

                    // Calculate required chemical dose (in mL or mg, depending on ChemicalStrength units)
                    float requiredDose = output * _poolVolume / Config.ChemicalStrength;

                    // Map requiredDose to pump current (mA) as needed for your system
                    double pumpCurrent = _pump.PumpMinCurrent.Milliamperes
                        + (requiredDose * (_pump.PumpMaxCurrent.Milliamperes - _pump.PumpMinCurrent.Milliamperes));

                    // Clamp to valid range (from the pump's configuration)
                    pumpCurrent = Math.Clamp(pumpCurrent, _pump.PumpMinCurrent.Milliamperes, _pump.PumpMaxCurrent.Milliamperes);

                    _pump.Current = ElectricCurrent.FromMilliamperes(pumpCurrent);

                    // store the last delta for derivative calculation
                    _lastChlorineDelta = chlorineDelta;
                }

                // sleep until the next cycle
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private bool CheckSafetyValue()
        {
            if (ChlorineValue > SafetyChlorineMax)
            {
                Debug.WriteLine($"Safety check failed: Chlorine value {ChlorineValue} is out of range (0-{SafetyChlorineMax})");

                // force the pump to stop
                _pump.Current = ElectricCurrent.FromMilliamperes(_pump.PumpMinCurrent.Value);

                // send error message to Azure
                Program.AzureClient.SendMessage($"{{\"safety_alert\":\"Chlorine out of range: {ChlorineValue}\"}}");

                // flag abnormal condition
                return false;
            }

            // all good, chlorine is within safe range
            return true;
        }
    }
}
