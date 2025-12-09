// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using nanoFramework.Runtime.Native;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace NFApp1
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            Debug.WriteLine($"Target name: {SystemInfo.TargetName}");

            Thread.Sleep(Timeout.Infinite);

            ManualResetEvent gpioEvent = new ManualResetEvent(false);

            GpioController gpio = new GpioController();
            GpioPin button = gpio.OpenPin(12, PinMode.InputPullUp);
            button.ValueChanged += (sender, args) =>
            {
                if( args.ChangeType != PinEventTypes.Rising)
                {
                    gpioEvent.Set();
                }
            };

            button.DebounceTimeout = TimeSpan.FromMilliseconds(200);





            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
