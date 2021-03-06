﻿using System;
using System.Diagnostics;
using Telebot.BusinessLogic;
using Telebot.Extensions;
using Telebot.Models;

namespace Telebot.Commands
{
    public class CapAppCmd : CommandBase
    {
        private readonly CaptureLogic captureLogic;

        public CapAppCmd()
        {
            Pattern = "/capapp (\\d+)";
            Description = "Get a screenshot of the specified application (by pid).";
            captureLogic = Program.container.GetInstance<CaptureLogic>();
        }

        public override void Execute(object parameter, Action<CommandResult> callback)
        {
            var parameters = parameter as CommandParam;

            int pid = Convert.ToInt32(parameters.Groups[1].Value);

            var hWnd = Process.GetProcessById(pid).MainWindowHandle;

            var photo = captureLogic.CaptureWindow(hWnd);

            var cmdResult = new CommandResult
            {
                SendType = SendType.Photo,
                Stream = photo.ToStream()
            };

            callback(cmdResult);
        }
    }
}
