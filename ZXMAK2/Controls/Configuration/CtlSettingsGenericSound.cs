﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ZXMAK2.Engine;
using ZXMAK2.Interfaces;
using ZXMAK2.Entities;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Controls.Configuration
{
    public partial class CtlSettingsGenericSound : ConfigScreenControl
    {
        private BusManager m_bmgr;
        private ISoundRenderer m_device;


        public CtlSettingsGenericSound()
        {
            InitializeComponent();
        }

        public void Init(BusManager bmgr, IHost host, ISoundRenderer device)
        {
            m_bmgr = bmgr;
            m_device = device;
            var busDevice = (BusDeviceBase)device;
            txtDevice.Text = busDevice.Name;
            txtDescription.Text = busDevice.Description.Replace("\n", Environment.NewLine);

            int value = m_device.Volume;
            if (value < 0)
                value = 0;
            if (value > 100)
                value = 100;
            trkVolume.Value = value;
        }

        public override void Apply()
        {
            m_device.Volume = trkVolume.Value;
        }
    }
}
