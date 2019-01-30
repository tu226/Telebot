﻿using System;
using Telebot.Contracts;
using Telebot.Models;

namespace Telebot.Commands
{
    public class MonitorOffCmd : ICommand
    {
        public string Name => "/monitor off";

        public string Description => "Turn off monitoring of temperature.";

        public event EventHandler<CommandResult> Completed;

        private readonly ISettings settings;
        private readonly ITemperatureMonitor tempMon;

        public MonitorOffCmd()
        {
            settings = Program.container.GetInstance<ISettings>();
            tempMon = Program.container.GetInstance<ITemperatureMonitor>();
        }

        public void Execute(object parameter)
        {
            var cmdInfo = parameter as CommandInfo;

            tempMon.Stop();
            settings.MonitorEnabled = false;

            var info = new CommandResult
            {
                Message = cmdInfo.Message,
                Text = "Temperature monitor is turned off.",
                SendType = SendType.Text
            };

            Completed?.Invoke(this, info);
        }

        public override string ToString()
        {
            return $"*{Name}* - {Description}";
        }
    }
}
