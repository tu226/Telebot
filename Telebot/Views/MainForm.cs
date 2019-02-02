﻿using System.Windows.Forms;
using BrightIdeasSoftware;
using Telebot.Events;
using Telebot.Views;

namespace Telebot
{
    public partial class MainForm : Form, IMainFormView
    {
        public ObjectListView ObjectListView { get { return objectListView1; } }

        public event MouseEventHandler TrayMouseClick
        {
            add { notifyIcon1.MouseClick += value; }
            remove { notifyIcon1.MouseClick -= value; }
        }

        public MainForm()
        {
            InitializeComponent();

            EventAggregator.Instance.Subscribe<UpdateNotifyIconVisible>(OnUpdateNotifyIconVisible);
            EventAggregator.Instance.Subscribe<ShowNotifyIconBalloon>(OnShowNotifyIconBalloon);
        }

        private void OnUpdateNotifyIconVisible(UpdateNotifyIconVisible obj)
        {
            notifyIcon1.Visible = obj.Visible;
        }

        private void OnShowNotifyIconBalloon(ShowNotifyIconBalloon obj)
        {
            notifyIcon1.ShowBalloonTip(1000, Text, obj.Text, ToolTipIcon.Info);
        }
    }
}
