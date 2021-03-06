﻿using BrightIdeasSoftware;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telebot.Views
{
    public interface IMainFormView
    {
        event EventHandler Load;
        event FormClosedEventHandler FormClosed;
        event EventHandler Resize;
        event MouseEventHandler TrayMouseClick;

        void Hide();
        void Show();
        ObjectListView ObjectListView { get; }
        Rectangle Bounds { get; set; }
        FormWindowState WindowState { get; set; }
    }
}
