﻿using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Profi
{
    public class CovoxProfi : SoundDeviceBase
    {
        #region IBusDevice

        public override string Name { get { return "COVOX PROFI"; } }
        public override string Description { get { return "COVOX PROFI \r\n#3F - right channel\r\n#5F - left channel"; } }
        public override BusDeviceCategory Category { get { return BusDeviceCategory.Sound; } }

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            m_memory = bmgr.FindDevice<IMemoryDevice>();

            bmgr.SubscribeWrIo(0x00FF, 0x003F, WritePortR);
            bmgr.SubscribeWrIo(0x00FF, 0x005F, WritePortL);
        }

        #endregion

        private IMemoryDevice m_memory;
        private ushort m_left = 0;
        private ushort m_right = 0;
        private int m_mult = 0;


        private void WritePortL(ushort addr, byte value, ref bool iorqge)
        {
            if (m_memory.DOSEN)
                return;
            if (!iorqge)
                return;
            iorqge = false;
            
            m_left = (ushort)(value * m_mult);
            UpdateDac(m_left, m_right);
        }

        private void WritePortR(ushort addr, byte value, ref bool iorqge)
        {
            if (m_memory.DOSEN)
                return;
            if (!iorqge)
                return;
            iorqge = false;
            
            m_right = (ushort)(value * m_mult);
            UpdateDac(m_left, m_right);
        }

        protected override void OnVolumeChanged(int oldVolume, int newVolume)
        {
            m_mult = (0xFFFF * newVolume) / (100 * 0xFF);
        }
    }
}
