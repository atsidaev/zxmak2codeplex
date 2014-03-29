﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZXMAK2.Interfaces
{
    public interface IHost : IDisposable
    {
        IHostVideo Video { get; }
        IHostSound Sound { get; }
        IHostKeyboard Keyboard { get; }
        IHostMouse Mouse { get; }
        IHostJoystick Joystick { get; }
    }
}
