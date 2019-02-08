﻿using System;
using System.Collections.Generic;
using System.Timers;
using Telebot.HwProviders;
using Telebot.Managers;
using Telebot.Models;

namespace Telebot.Monitors
{
    public class SystemTempMonitor : ITemperatureMonitor
    {
        private const int Refresh_Interval = 10 * 1000;

        private readonly Timer workerTimer;
        private readonly IEnumerable<ITemperatureProvider> tempProviders;
        private readonly ISettings settings;
        public bool IsActive { get { return workerTimer.Enabled; } }

        public event EventHandler<IHardwareInfo> TemperatureChanged;

        public SystemTempMonitor()
        {
            tempProviders = Program.container.GetAllInstances<ITemperatureProvider>();
            settings = Program.container.GetInstance<ISettings>();

            workerTimer = new Timer(Refresh_Interval);
            workerTimer.Elapsed += WorkerTimer_Elapsed;

            if (settings.TempMonEnabled) Start();
        }

        private void WorkerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (ITemperatureProvider tempProvider in tempProviders)
            {
                var hwInfos = tempProvider.GetTemperature();

                foreach (IHardwareInfo hwInfo in hwInfos)
                {
                    TemperatureChanged?.Invoke(this, hwInfo);
                }
            }
        }

        public void Start()
        {
            workerTimer.Start();
        }

        public void Stop()
        {
            workerTimer.Stop();
        }
    }
}
