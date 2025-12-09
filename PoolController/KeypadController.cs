// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.Device.Gpio;
using System.Threading;

public static class KeypadController
{
    private const int BackPin = 4;
    private const int UpPin = 5;
    private const int DownPin = 6;
    private const int SelectPin = 7;
    private static readonly TimeSpan DebounceTime = TimeSpan.FromMilliseconds(250);

    private static GpioController _gpio;
    private static GpioPin _back;
    private static GpioPin _up;
    private static GpioPin _down;
    private static GpioPin _select;

    private static bool _backPressed;
    private static bool _upPressed;
    private static bool _downPressed;
    private static bool _selectPressed;

    public static readonly ManualResetEvent BackChanged = new ManualResetEvent(false);
    public static readonly ManualResetEvent UpChanged = new ManualResetEvent(false);
    public static readonly ManualResetEvent DownChanged = new ManualResetEvent(false);
    public static readonly ManualResetEvent SelectChanged = new ManualResetEvent(false);

    public static bool BackPressed => _backPressed;
    public static bool UpPressed => _upPressed;
    public static bool DownPressed => _downPressed;
    public static bool SelectPressed => _selectPressed;

    public static void Init(GpioController gpioController)
    {
        _gpio = gpioController;

        _back = _gpio.OpenPin(BackPin, PinMode.InputPullUp);
        _up = _gpio.OpenPin(UpPin, PinMode.InputPullUp);
        _down = _gpio.OpenPin(DownPin, PinMode.InputPullUp);
        _select = _gpio.OpenPin(SelectPin, PinMode.InputPullUp);

        _back.DebounceTimeout = DebounceTime;
        _up.DebounceTimeout = DebounceTime;
        _down.DebounceTimeout = DebounceTime;
        _select.DebounceTimeout = DebounceTime;

        _back.ValueChanged += BackHandler;
        _up.ValueChanged += UpHandler;
        _down.ValueChanged += DownHandler;
        _select.ValueChanged += SelectHandler;
    }

    private static void BackHandler(object sender, PinValueChangedEventArgs e)
    {
        _backPressed = e.ChangeType == PinEventTypes.Falling;
        BackChanged.Set();
    }

    private static void UpHandler(object sender, PinValueChangedEventArgs e)
    {
        _upPressed = e.ChangeType == PinEventTypes.Falling;
        UpChanged.Set();
    }

    private static void DownHandler(object sender, PinValueChangedEventArgs e)
    {
        _downPressed = e.ChangeType == PinEventTypes.Falling;
        DownChanged.Set();
    }

    private static void SelectHandler(object sender, PinValueChangedEventArgs e)
    {
        _selectPressed = e.ChangeType == PinEventTypes.Falling;
        SelectChanged.Set();
    }
}
