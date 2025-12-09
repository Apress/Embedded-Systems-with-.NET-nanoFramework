// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Device.Gpio;
using System.Threading;

namespace PoolController
{
    public class TurbidityController
    {
        private readonly TurbiditySensor _sensor;
        private readonly TurbidityControllerConfig _config;
        private Thread _controlThread;
        private bool _running;

        public float TurbidityValue { get; private set; }

        public TurbidityController(
            TurbiditySensor sensor,
            TurbidityControllerConfig config)
        {
            _sensor = sensor;
            _config = config;
        }

        public void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            _controlThread = new Thread(ControlLoop);
            _controlThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _controlThread?.Join();
            _config.FilterGpioPin.Write(PinValue.Low); // Ensure filter is off
        }

        private void ControlLoop()
        {
            while (_running)
            {
                TurbidityValue = _sensor.ReadTurbidity();

                if (TurbidityValue > _config.Threshold)
                {
                    _config.FilterGpioPin.Write(PinValue.High); // Run filter
                }
                else
                {
                    _config.FilterGpioPin.Write(PinValue.Low); // Stop filter
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }
    }
}
