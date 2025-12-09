// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace PoolController
{
    public class LoggerModule
    {
        private StreamWriter _logStream;
        private readonly bool _enabled;

        public LoggerModule(string logFilePath, bool enabled)
        {
            _enabled = enabled;

            if (_enabled)
            {
                var fileStream = new FileStream(
                    logFilePath,
                    FileMode.Append,
                    FileAccess.Write);

                _logStream = new StreamWriter(fileStream);
            }
        }

        public void Log(string message)
        {
            if (!_enabled)
            {
                return;
            }

            string logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} {message}";

            _logStream.WriteLine(logLine);
            _logStream.Flush();
        }

        public void LogBoot() => Log("System boot");
        public void LogPumpOn(string pump) => Log($"{pump} pump ON");
        public void LogPumpOff(string pump) => Log($"{pump} pump OFF");
        public void LogThresholdCrossed(string sensor, float value) => Log($"{sensor} threshold crossed: {value}");
        public void LogSystemEvent(string message) => Log($"{message}");

        public void Close()
        {
            if (_logStream != null)
            {
                _logStream.Flush();
                _logStream.Dispose();
            }
        }
    }
}
