﻿using System;
using System.Collections.Generic;
using Telebot.Models;
using Telebot.Monitors;

namespace Telebot.Commands
{
    public class TempMonCmd : CommandBase
    {
        private readonly Dictionary<string, Action> actions;

        public TempMonCmd()
        {
            Pattern = "/tempmon (on|off)";
            Description = "Turn on or off the temperature monitor.";

            actions = new Dictionary<string, Action>()
            {
                { "on", PermanentTempMonitor.Instance.Start },
                { "off", PermanentTempMonitor.Instance.Stop }
            };
        }

        public override void Execute(object parameter, Action<CommandResult> callback)
        {
            var parameters = parameter as CommandParam;

            string state = parameters.Groups[1].Value;

            var result = new CommandResult
            {
                SendType = SendType.Text,
                Text = $"Successfully turned {state} the temperature monitor."
            };

            callback(result);

            actions[state].Invoke();
        }
    }
}
