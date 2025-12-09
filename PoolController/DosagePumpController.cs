// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using Iot.Device.Dac63004;
using System;
using System.Device.I2c;

namespace PoolController
{
    public class DosagePumpController : IDisposable
    {
        private readonly Dac63004 _dac;
        private readonly I2cDevice _i2CDevice;
        public DosagePump[] Pumps { get; }

        public DosagePumpController(int i2cBusId)
        {
            I2cConnectionSettings settings = new(
                i2cBusId,
                Dac63004.DefaultI2cAddress);

            _i2CDevice = I2cDevice.Create(settings);
            _dac = new Dac63004(_i2CDevice);

            ConfigureDac();

            Pumps = new DosagePump[4];
            Pumps[0] = new DosagePump(_dac, Channel.Channel0);
            Pumps[1] = new DosagePump(_dac, Channel.Channel1);
            Pumps[2] = new DosagePump(_dac, Channel.Channel2);
            Pumps[3] = new DosagePump(_dac, Channel.Channel3);
        }

        private void ConfigureDac()
        {
            // all channels are set to voltage output mode 
            _dac.ConfigureChannelMode(Channel.Channel0, Mode.VoltageOutput);
            _dac.ConfigureChannelMode(Channel.Channel1, Mode.VoltageOutput);
            _dac.ConfigureChannelMode(Channel.Channel2, Mode.VoltageOutput);
            _dac.ConfigureChannelMode(Channel.Channel3, Mode.VoltageOutput);

            // gain for all channels is set to 2
            _dac.ConfigureChannelVoutGain(Channel.Channel0, VoutGain.Internal2x);
            _dac.ConfigureChannelVoutGain(Channel.Channel1, VoutGain.Internal2x);
            _dac.ConfigureChannelVoutGain(Channel.Channel2, VoutGain.Internal2x);
            _dac.ConfigureChannelVoutGain(Channel.Channel3, VoutGain.Internal2x);

            // Enable internal reference voltage
            _dac.InternalRefEnabled = true;
        }

        public void Dispose()
        {
            _dac?.Dispose();
            _i2CDevice?.Dispose();
        }
    }
}
