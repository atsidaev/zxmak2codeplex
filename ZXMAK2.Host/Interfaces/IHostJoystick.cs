﻿using System;
using System.Collections.Generic;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostJoystick
    {
        void CaptureHostDevice(string hostId);
        void ReleaseHostDevice(string hostId);
        void Scan();
        IJoystickState GetState(string hostId);
        IKeyboardState KeyboardState { set; }
        bool IsKeyboardStateRequired { get; }
        IEnumerable<IHostDeviceInfo> GetAvailableJoysticks();
    }
}