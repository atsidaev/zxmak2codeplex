﻿using System;
using ZXMAK2.Hardware.General;

namespace ZXMAK2.Hardware.Quorum
{
    public class BeeperQuorum : BeeperDevice
    {
        #region IBusDevice

        public override string Name { get { return "Quorum Beeper"; } }
        public override string Description { get { return "Simple Quorum Beeper"; } }

        #endregion

        #region BeeperDevice

        protected override void WritePortFE(ushort addr, byte value, ref bool iorqge)
        {
            if ((addr & 0x99)==(0xFE&0x99))
                base.WritePortFE(addr, value, ref iorqge);
        }

        #endregion
    }
}