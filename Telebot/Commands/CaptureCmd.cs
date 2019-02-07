﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Telebot.BusinessLogic;
using Telebot.Extensions;
using Telebot.Models;
using System.Threading.Tasks;

namespace Telebot.Commands
{
    public class CaptureCmd : ICommand
    {
        public string Name => "/capture";

        public string Description => "Get a screenshot of the workstation.";

        public event EventHandler<CommandResult> Completed;

        private readonly CaptureLogic captureLogic;
        private readonly MainForm mainForm;

        public CaptureCmd()
        {
            captureLogic = Program.container.GetInstance<CaptureLogic>();
            mainForm = Program.container.GetInstance<MainForm>();
        }

        public void Execute(object parameter)
        {
            var cmdInfo = parameter as CommandInfo;

            var bitmaps = new List<Bitmap>
            {
                //captureLogic.CaptureControl(mainForm),
                captureLogic.CaptureDesktop()
            };

            foreach (Bitmap bitmap in bitmaps)
            {
                var info = new CommandResult
                {
                    Message = cmdInfo.Message,
                    Stream = bitmap.ToStream(),
                    SendType = SendType.Photo
                };

                Completed?.Invoke(this, info);
            }
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
