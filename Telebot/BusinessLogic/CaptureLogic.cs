﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Telebot.Contracts;

namespace Telebot.BusinessLogic
{
    public class CaptureLogic
    {
        private readonly ILogger logger;

        public CaptureLogic()
        {
            logger = Program.container.GetInstance<ILogger>();
        }

        public Bitmap CaptureDesktop()
        {
            int width = SystemInformation.VirtualScreen.Width;
            int height = SystemInformation.VirtualScreen.Height;
            int left = SystemInformation.VirtualScreen.Left;
            int top = SystemInformation.VirtualScreen.Top;

            Bitmap result = new Bitmap(width, height);

            try
            {
                using (Graphics gObj = Graphics.FromImage(result))
                {
                    gObj.CopyFromScreen(left, top, 0, 0, result.Size);
                }
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
            }

            return result;
        }

        public Bitmap CaptureControl(Control control)
        {
            Bitmap result = new Bitmap(control.Width, control.Height);

            control.Invoke((MethodInvoker)delegate
            {
                control.DrawToBitmap(result, new Rectangle(0, 0, control.Width, control.Height));
            });

            return result;
        }
    }
}
