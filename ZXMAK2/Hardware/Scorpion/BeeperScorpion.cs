﻿using System;
using ZXMAK2.Interfaces;
using ZXMAK2.Hardware.General;

namespace ZXMAK2.Hardware.Scorpion
{
	public class BeeperScorpion : BeeperDevice
	{
		#region IBusDevice

		public override string Name { get { return "Scorpion Beeper"; } }
		public override string Description { get { return "Simple Scorpion ZS Beeper"; } }

		public override void BusInit(IBusManager bmgr)
		{
			base.BusInit(bmgr);
			m_memory = (IMemoryDevice)bmgr.FindDevice(typeof(IMemoryDevice));
		}

		#endregion

		#region BeeperDevice

		protected override void WritePortFE(ushort addr, byte value, ref bool iorqge)
		{
			if (!m_memory.DOSEN && (addr & 0x23) == (0xFE & 0x23))
				base.WritePortFE(addr, value, ref iorqge);
		}

		#endregion

		private IMemoryDevice m_memory;
	}
}
