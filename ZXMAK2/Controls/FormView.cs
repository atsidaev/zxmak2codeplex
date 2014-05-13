﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ZXMAK2.MVP.Interfaces;
using System.ComponentModel;

namespace ZXMAK2.Controls
{
    public class FormView : Form, IView
    {
        public void Show(IMainView parent)
        {
            if (!Visible)
            {
                Show(parent as IWin32Window);
            }
            else
            {
                Show();
                Activate();
            }
        }

        public event EventHandler ViewClosed;
        public event CancelEventHandler ViewClosing;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var handler = ViewClosing;
                if (handler != null)
                {
                    var arg = new CancelEventArgs(e.Cancel);
                    handler(this, arg);
                    e.Cancel = arg.Cancel;
                }
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            var handler = ViewClosed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            base.OnFormClosed(e);
        }
    }
}