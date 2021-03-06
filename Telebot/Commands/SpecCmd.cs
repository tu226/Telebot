﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Telebot.Models;

namespace Telebot.Commands
{
    public class SpecCmd : CommandBase
    {
        public SpecCmd()
        {
            Pattern = "/spec";
            Description = "Get full hardware information.";
        }

        public override void Execute(object parameter, Action<CommandResult> callback)
        {
            Process.Start("SpecInfo.exe").WaitForExit();

            string filePath = Path.Combine(Application.StartupPath, "info.txt");

            var fileStream = new FileStream(filePath, FileMode.Open);

            var cmdResult = new CommandResult
            {
                SendType = SendType.Document,
                Stream = fileStream
            };

            callback(cmdResult);
        }
    }
}
