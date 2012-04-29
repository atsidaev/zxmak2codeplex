﻿using System;
using System.Collections.Generic;
using System.Text;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Engine.Devices
{
    public class AyMouseDevice : IBusDevice, IMouseDevice
    {
        #region IBusDevice Members

        public string Name { get { return "AY Mouse"; } }
        public string Description { get { return "AY Mouse based on V.M.G. extension"; } }
        public BusCategory Category { get { return BusCategory.Mouse; } }
		private int m_busOrder = 0;
		public int BusOrder { get { return m_busOrder; } set { m_busOrder = value; } }

        public void BusInit(IBusManager bmgr)
        {
            m_ay8910 = bmgr.FindDevice(typeof(AY8910)) as AY8910;
        }

        public void BusConnect()
        {
            if (m_ay8910 != null)
                m_ay8910.UpdateIRA += new AyUpdatePortDelegate(ay_UpdateIRA);	// AY mouse handler
        }

        public void BusDisconnect()
        {
            if (m_ay8910 != null)
                m_ay8910.UpdateIRA -= new AyUpdatePortDelegate(ay_UpdateIRA);	// AY mouse handler
        }

        #endregion

        #region IMouseDevice Members

		public IMouseState MouseState
		{
			get { return m_mouseState; }
			set { m_mouseState = value; }
		}

        #endregion

        private AY8910 m_ay8910;
		private IMouseState m_mouseState = null;
        private int _lastAyMouseX = 0;
        private int _lastAyMouseY = 0;

        private void ay_UpdateIRA(AY8910 sender, AyPortState state)
        {
            //
            // Emulation AY-Mouse (V.M.G. schema)
            //
			int x = 0;
			int y = 0;
			int b = 0;
			if (MouseState != null)
			{
				x = MouseState.X;
				y = MouseState.Y;
				b = MouseState.Buttons;
			}
			
			int pcDelta;
            if ((state.OutState & 0x40) != 0) // selected V counter
                pcDelta = _lastAyMouseY - y / 4;
            else							  // selected H counter
                pcDelta = x / 4 - _lastAyMouseX;
            // make signed 4 bit integer...
            pcDelta = pcDelta + 8;

            // prevent overflow (this feature not present in original schema)...
            if (pcDelta < 0) pcDelta = 0;
            if (pcDelta > 15) pcDelta = 15;

            // buttons 0 and 1...
            state.InState = (byte)((pcDelta & 0x0F) | ((b & 3) ^ 3) << 4 | (state.OutState & 0xC0));

            if (state.DirOut && ((state.OutState ^ state.OldOutState) & 0x40) != 0 && (state.OutState & 0x40) == 0)
            {
                // reset H and V counters
                _lastAyMouseX = x / 4;
                _lastAyMouseY = y / 4;
            }
        }
    }
}
