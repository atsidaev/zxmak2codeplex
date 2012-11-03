﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Devices.Ula;

namespace ZXMAK2.Engine.Devices.Memory
{
	public class MemoryPentagon1024 : MemoryBase
	{
		#region IBusDevice

		public override string Name { get { return "Pentagon 1024K"; } }
		public override string Description { get { return "Pentagon 1024K Memory Module"; } }

		public override void BusInit(IBusManager bmgr)
		{
			base.BusInit(bmgr);
			bmgr.SubscribeWRIO(0xC002, 0x4000, writePort7FFD);
			bmgr.SubscribeWRIO(0xF008, 0xE000, writePortEFF7);
			bmgr.SubscribeRDMEM_M1(0xC000, 0x4000, BusReadMemRam);
			bmgr.SubscribeRDMEM_M1(0xC000, 0x8000, BusReadMemRam);
			bmgr.SubscribeRDMEM_M1(0xC000, 0xC000, BusReadMemRam);
			bmgr.SubscribeNMIACK(busNmi);
			bmgr.SubscribeRESET(busReset);
		}

		#endregion

		#region MemoryBase

		public override byte[][] RamPages { get { return m_ramPages; } }

		public override bool IsMap48 { get { return m_lock; } }

		protected override void UpdateMapping()
		{
			bool extMode = (CMR1 & 0x04) == 0;			// D2 - 0=extended memory mode; 1=lock 128K mode
			bool norom = extMode && (CMR1 & 0x10) != 0;	// D3 - ram0 at 0000...3FFF

			m_lock = !extMode && (CMR0 & 0x20) != 0;
			int ramPage = CMR0 & 7;
			int romPage = (CMR0 & 0x10) >> 4;
			int videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;

			if (DOSEN)      // trdos or 48/128
				romPage = 2;
			if (SYSEN)
				romPage = 3;

			if (extMode)
			{
				int sega = ((CMR0 & 0xC0) >> 6) | ((CMR0 & 0x20) >> 3);	//PENT1024: D5,D7,D6,D2,D1,D0
				ramPage |= sega << 3;
			}

			m_ula.SetPageMapping(videoPage, norom ? 0 : -1, 5, 2, ramPage);
			MapRead0000 = norom ? RamPages[5] : RomPages[romPage];
			MapRead4000 = RamPages[5];
			MapRead8000 = RamPages[2];
			MapReadC000 = RamPages[ramPage];

			MapWrite0000 = norom ? MapRead0000 : m_trashPage;
			MapWrite4000 = MapRead4000;
			MapWrite8000 = MapRead8000;
			MapWriteC000 = MapReadC000;

			Map48[0] = romPage;
			Map48[1] = 5;
			Map48[2] = 2;
			Map48[3] = ramPage;
		}

		protected override void LoadRom()
		{
			base.LoadRom();
			LoadRomPack("Pentagon1024");
		}

		protected virtual void BusReadMemRam(ushort addr, ref byte value)
		{
			if (SYSEN)
				SYSEN = false;
		}

		#endregion

		private byte[][] m_ramPages = new byte[64][];
		private byte[] m_trashPage = new byte[0x4000];
		private bool m_lock = false;


		public MemoryPentagon1024()
		{
			for (int i = 0; i < m_ramPages.Length; i++)
				m_ramPages[i] = new byte[0x4000];
		}

		#region Bus Handlers

		private void writePort7FFD(ushort addr, byte value, ref bool iorqge)
		{
			if (!m_lock)
				CMR0 = value;
		}

		private void writePortEFF7(ushort addr, byte value, ref bool iorqge)
		{
			CMR1 = value;
		}

		private void busNmi()
		{
			// check DOSEN to avoid conflict with BDI
			if (!DOSEN)
			{
				// enable shadow rom
				SYSEN = true;
			}
		}

		private void busReset()
		{
			SYSEN = true;
			CMR0 = 0;
			CMR1 = 0;
		}

		#endregion
	}
}