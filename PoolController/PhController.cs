// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using UnitsNet;

namespace PoolController
{
    public class PhController
    {
        private const float SafetyPhMax = 9.0f;

        private readonly PhSensor _sensor;
        private readonly DosagePump _pump;
        private readonly float _poolVolume;
        private Thread _controlThread;
        private bool _running;
        private float _integral;
        private float _lastPhDelta;

        public PhControllerConfig Config { get; set; }
        public float PhValue { get; set; }

        public PhController(
            PhSensor sensor,
            DosagePump pump,
            float poolVolume,
            PhControllerConfig config)
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
                // read the pH value from the sensor
                PhValue = _sensor.ReadPh();

                if (CheckSafetyValue())
                {
                    // compute the difference from target pH
                    float phDelta = Config.TargetPh - PhValue;

                    // Simple PID control
                    _integral += phDelta;
                    float derivative = phDelta - _lastPhDelta;
                    float output = Config.Kp * phDelta + Config.Ki * _integral + Config.Kd * derivative;

                    // Calculate required chemical dose (in mL or mg, depending on ChemicalStrength units)
                    float requiredDose = output * _poolVolume / Config.ChemicalStrength;

                    // Map requiredDose to pump current (mA) as needed for your system
                    double pumpCurrent = _pump.PumpMinCurrent.Milliamperes
                        + (requiredDose * (_pump.PumpMaxCurrent.Milliamperes - _pump.PumpMinCurrent.Milliamperes));

                    // Clamp to valid range (from the pump's configuration)
                    pumpCurrent = Math.Clamp(pumpCurrent, _pump.PumpMinCurrent.Milliamperes, _pump.PumpMaxCurrent.Milliamperes);

                    _pump.Current = ElectricCurrent.FromMilliamperes(pumpCurrent);

                    // store the last delta for derivative calculation
                    _lastPhDelta = phDelta;
                }

                // sleep untill the next cycle
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private bool CheckSafetyValue()
        {
            if (PhValue > SafetyPhMax)
            {
                Debug.WriteLine($"Safety check failed: pH value {PhValue} is out of range (0-{SafetyPhMax})");

                // force the pump to stop
                _pump.Current = ElectricCurrent.FromMilliamperes(_pump.PumpMinCurrent.Value);

                // send error message to Azure
                Program.AzureClient.SendMessage($"{{\"safety_alert\":\"pH out of range: {PhValue}\"}}");

                // flag abnormal condition
                return false;
            }

            // all good, pH is within safe range
            return true;
        }
    }
}
