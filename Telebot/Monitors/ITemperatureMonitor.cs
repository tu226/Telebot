﻿using System;
using Telebot.Models;

namespace Telebot.Monitors
{
    public interface ITemperatureMonitor
    {
        event EventHandler<IHardwareInfo> TemperatureChanged;
        bool IsActive { get; }
        void Start();
        void Stop();
    }
}
