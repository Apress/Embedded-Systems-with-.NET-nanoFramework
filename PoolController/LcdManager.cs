// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using Iot.Device.CharacterLcd;
using System;
using System.Device.I2c;
using System.Threading;

namespace PoolController
{
    public static class LcdManager
    {
        private static Lcd1602 _lcd;
        private static Thread _updateThread;
        private static bool _running;
        private static bool _paused;
        private static string _appVersion = "v1.0.0";

        public static void Init()
        {
            if (_lcd != null)
            {
                // LCD already initialized
                return;
            }

            var i2cSettings = new I2cConnectionSettings(1, 0x27);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            _lcd = new Lcd1602(i2cDevice);

            _lcd.Clear();
            ShowBootMessage();

            _running = true;
            _updateThread = new Thread(IdleUpdateLcd);
            _updateThread.Start();
        }

        private static void ShowBootMessage()
        {
            _lcd.Clear();
            _lcd.SetCursorPosition(0, 0);
            _lcd.Write("nano pool ctrlr");
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write($"v {_appVersion}");

            Thread.Sleep(5_000);

            _lcd.Clear();
        }

        private static void IdleUpdateLcd()
        {
            while (_running)
            {
                if (!_paused)
                {
                    _lcd.Clear();

                    _lcd.SetCursorPosition(0, 0);
                    _lcd.Write($"pH:{Program.PhController0.PhValue:F1} Cl:{Program.ChlorineController0.ChlorineValue:F1}");

                    _lcd.SetCursorPosition(0, 1);
                    _lcd.Write($"Turb:{Program.TurbidityController0.TurbidityValue:F1}");
                }

                Thread.Sleep(1_000);
            }
        }

        public static void Pause() => _paused = true;
        public static void Resume() => _paused = false;

        public static void ShowMessage(
            string line1,
            string line2,
            TimeSpan duration)
        {
            Pause();

            _lcd.Clear();
            _lcd.SetCursorPosition(0, 0);
            _lcd.Write(line1);
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(line2);

            Thread.Sleep((int)(duration.TotalMilliseconds));

            Resume();
        }

        public static void Stop()
        {
            _running = false;

            _updateThread?.Join();

            _lcd?.Clear();
        }

        public static void UpdateScreesn(string line1, string line2)
        {
            if (_lcd is null)
            {
                return; // LCD not initialized
            }

            _lcd.Clear();
            _lcd.SetCursorPosition(0, 0);
            _lcd.Write(line1);
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(line2);
        }

        public static void UpdateLine1(string content)
        {
            if (_lcd is null)
            {
                return; // LCD not initialized
            }

            _lcd.SetCursorPosition(0, 0);
            _lcd.Write(content);
        }

        public static void UpdateLine2(string content)
        {
            if (_lcd is null)
            {
                return; // LCD not initialized
            }

            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(content);
        }

        public static void ClearLine1()
        {
            if (_lcd is null)
            {
                return; // LCD not initialized
            }

            _lcd.SetCursorPosition(0, 0);
            _lcd.Write(new string(' ', _lcd.Size.Width));
        }

        public static void ClearLine2()
        {
            if (_lcd is null)
            {
                return; // LCD not initialized
            }

            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(new string(' ', _lcd.Size.Width));
        }
    }
}
