﻿using System.Diagnostics;
using System.Windows.Forms;
using Telebot.Helpers;

namespace Telebot.BusinessLogic
{
    public class PowerLogic
    {
        public void ShutdownWorkstation()
        {
            Process.Start
            (
                new ProcessStartInfo("shutdown", "/s /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            );
        }

        public void RestartWorkstation()
        {
            Process.Start
            (
                new ProcessStartInfo("shutdown", "/r /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            );
        }

        public void SleepWorkstation()
        {
            Application.SetSuspendState(PowerState.Suspend, false, false);
        }

        public void LockWorkstation()
        {
            User32Helper.LockWorkStation();
        }
    }
}
