﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telebot.Models;
using Telebot.StatusCommands;

namespace Telebot.Commands
{
    public class StatusCmd : ICommand
    {
        public string Name => "/status";

        public string Description => "Receive workstation information.";

        public event EventHandler<CommandResult> Completed;

        private readonly IEnumerable<IStatusCommand> statusCommands;

        public StatusCmd()
        {
            statusCommands = Program.container.GetAllInstances<IStatusCommand>();
        }

        public void Execute(object parameter)
        {
            var cmdInfo = parameter as CommandInfo;

            var status = new StringBuilder();

            foreach(IStatusCommand cmd in statusCommands)
            {
                status.AppendLine(cmd.Execute());
            }

            var info = new CommandResult
            {
                Message = cmdInfo.Message,
                Text = status.ToString(),
                SendType = SendType.Text
            };

            Completed?.Invoke(this, info);
        }

        public void ExecuteAsync(object parameter)
        {
            Task.Run(() => Execute(parameter));
        }

        public override string ToString()
        {
            return $"*{Name}* - {Description}";
        }
    }
}
