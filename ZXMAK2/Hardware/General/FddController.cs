﻿using System;
using System.Xml;
using ZXMAK2.Interfaces;
using ZXMAK2.Entities;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Hardware.IC;
using ZXMAK2.MVP.Interfaces;
using ZXMAK2.Controls.Debugger;
using ZXMAK2.Dependency;
using ZXMAK2.MVP;

namespace ZXMAK2.Hardware.General
{
    public class FddController : BusDeviceBase, IBetaDiskDevice
    {
        #region Fields

        private bool m_sandbox = false;
        private IconDescriptor m_iconRd = new IconDescriptor("FDDRD", Utils.GetIconStream("Fdd.png"));
        private IconDescriptor m_iconWr = new IconDescriptor("FDDWR", Utils.GetIconStream("FddWr.png"));
        protected CpuUnit m_cpu;
        protected IMemoryDevice m_memory;
        protected Wd1793 m_wd = new Wd1793();

        private IViewHolder m_viewHolder;

        #endregion


        public FddController()
        {
            CreateViewHolder();
        }

        
        #region IBusDevice

        public override string Name { get { return "FDD WD1793"; } }
        public override string Description { get { return "FDD controller WD1793\r\nBDI-ports compatible\r\nPorts active when DOSEN=1 or SYSEN=1"; } }
        public override BusDeviceCategory Category { get { return BusDeviceCategory.Disk; } }

        public override void BusInit(IBusManager bmgr)
        {
            m_sandbox = bmgr.IsSandbox;
            m_cpu = bmgr.CPU;
            m_memory = bmgr.FindDevice<IMemoryDevice>();

            bmgr.RegisterIcon(m_iconRd);
            bmgr.RegisterIcon(m_iconWr);
            bmgr.SubscribeBeginFrame(BusBeginFrame);
            bmgr.SubscribeEndFrame(BusEndFrame);
            
            OnSubscribeIo(bmgr);

            foreach (var fs in m_wd.FDD[0].SerializeManager.GetSerializers())
            {
                bmgr.AddSerializer(fs);
            }
            if (m_viewHolder != null)
            {
                bmgr.AddCommandUi(m_viewHolder.CommandOpen);
            }
        }

        public override void BusConnect()
        {
            if (!m_sandbox)
            {
                foreach (var di in m_wd.FDD)
                {
                    di.Connect();
                }
            }
        }

        public override void BusDisconnect()
        {
            if (!m_sandbox)
            {
                foreach (var di in m_wd.FDD)
                {
                    di.Disconnect();
                }
            }
            if (m_viewHolder != null)
            {
                m_viewHolder.Close();
            }
        }

        protected override void OnConfigLoad(XmlNode itemNode)
        {
            base.OnConfigLoad(itemNode);
            NoDelay = Utils.GetXmlAttributeAsBool(itemNode, "noDelay", false);
            LogIo = Utils.GetXmlAttributeAsBool(itemNode, "logIo", false);
            for (var i = 0; i < m_wd.FDD.Length; i++)
            {
                var inserted = false;
                var readOnly = true;
                var fileName = string.Empty;
                var node = itemNode.SelectSingleNode(string.Format("Drive[@index='{0}']", i));
                if (node != null)
                {
                    inserted = Utils.GetXmlAttributeAsBool(node, "inserted", inserted);
                    readOnly = Utils.GetXmlAttributeAsBool(node, "readOnly", readOnly);
                    fileName = Utils.GetXmlAttributeAsString(node, "fileName", fileName);
                }
                // will be opened on Connect
                m_wd.FDD[i].FileName = fileName;
                m_wd.FDD[i].IsWP = readOnly;
                m_wd.FDD[i].Present = inserted;
            }
        }

        protected override void OnConfigSave(XmlNode itemNode)
        {
            base.OnConfigSave(itemNode);
            Utils.SetXmlAttribute(itemNode, "noDelay", NoDelay);
            Utils.SetXmlAttribute(itemNode, "logIo", LogIo);
            for (var i = 0; i < m_wd.FDD.Length; i++)
            {
                if (m_wd.FDD[i].Present)
                {
                    XmlNode xn = itemNode.AppendChild(itemNode.OwnerDocument.CreateElement("Drive"));
                    Utils.SetXmlAttribute(xn, "index", i);
                    Utils.SetXmlAttribute(xn, "inserted", m_wd.FDD[i].Present);
                    Utils.SetXmlAttribute(xn, "readOnly", m_wd.FDD[i].IsWP);
                    if (!string.IsNullOrEmpty(m_wd.FDD[i].FileName))
                    {
                        Utils.SetXmlAttribute(xn, "fileName", m_wd.FDD[i].FileName);
                    }
                }
            }
        }

        #endregion

        
        #region IBetaDiskInterface

        public bool DOSEN
        {
            get { return m_memory.DOSEN; }
            set { m_memory.DOSEN = value; }
        }

        public DiskImage[] FDD
        {
            get { return m_wd.FDD; }
        }

        public bool NoDelay
        {
            get { return m_wd.NoDelay; }
            set { m_wd.NoDelay = value; }
        }

        public bool LogIo { get; set; }

        #endregion


        #region IGuiExtension Members

        private void CreateViewHolder()
        {
            try
            {
                var resolver = Locator.Resolve<IViewResolver>();
                m_viewHolder = new ViewHolder<IFddDebugView>(
                    resolver, 
                    "WD1793", 
                    new Argument("debugTarget", m_wd));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion


        #region Private

        public virtual bool IsActive
        {
            get { return m_memory.DOSEN || m_memory.SYSEN; }
        }

        protected virtual void OnSubscribeIo(IBusManager bmgr)
        {
            //var mask = 0x83;
            //var mask = 0x87;  // original #83 conflicts with port #FB (covox)
            var mask = 0x97;    // #87 conflicts with port #CF (IDE ATM)
            bmgr.SubscribeWrIo(mask, 0x1F & mask, BusWriteFdc);
            bmgr.SubscribeRdIo(mask, 0x1F & mask, BusReadFdc);
            bmgr.SubscribeWrIo(mask, 0xFF & mask, BusWriteSys);
            bmgr.SubscribeRdIo(mask, 0xFF & mask, BusReadSys);
        }

        protected virtual void BusBeginFrame()
        {
            m_wd.LedRd = false;
            m_wd.LedWr = false;
        }

        protected virtual void BusEndFrame()
        {
            m_iconWr.Visible = m_wd.LedWr;
            m_iconRd.Visible = !m_wd.LedWr && m_wd.LedRd;
        }

        protected virtual void BusWriteFdc(ushort addr, byte value, ref bool iorqge)
        {
            if (!iorqge)
            {
                return;
            }
            if (IsActive)
            {
                iorqge = false;
                int fdcReg = (addr & 0x60) >> 5;
                LogIoWrite(m_cpu.Tact, (WD93REG)fdcReg, value);
                m_wd.Write(m_cpu.Tact, (WD93REG)fdcReg, value);
            }
        }

        protected virtual void BusReadFdc(ushort addr, ref byte value, ref bool iorqge)
        {
            if (!iorqge)
            {
                return;
            }
            if (IsActive)
            {
                iorqge = false;
                int fdcReg = (addr & 0x60) >> 5;
                value = m_wd.Read(m_cpu.Tact, (WD93REG)fdcReg);
                LogIoRead(m_cpu.Tact, (WD93REG)fdcReg, value);
            }
        }

        protected virtual void BusWriteSys(ushort addr, byte value, ref bool iorqge)
        {
            if (!iorqge)
            {
                return;
            }
            if (IsActive)
            {
                iorqge = false;
                LogIoWrite(m_cpu.Tact, WD93REG.SYS, value);
                m_wd.Write(m_cpu.Tact, WD93REG.SYS, value);
            }
        }

        protected virtual void BusReadSys(ushort addr, ref byte value, ref bool iorqge)
        {
            if (!iorqge)
            {
                return;
            }
            if (IsActive)
            {
                iorqge = false;
                value = m_wd.Read(m_cpu.Tact, WD93REG.SYS);
                LogIoRead(m_cpu.Tact, WD93REG.SYS, value);
            }
        }

        protected void LogIoWrite(long tact, WD93REG reg, byte value)
        {
            if (LogIo)
            {
                Logger.Debug(
                    "WD93 {0} <== #{1:X2} [PC=#{2:X4}, T={3}]",
                    reg,
                    value,
                    m_cpu.regs.PC,
                    tact);
            }
        }

        protected void LogIoRead(long tact, WD93REG reg, byte value)
        {
            if (LogIo)
            {
                Logger.Debug(
                    "WD93 {0} ==> #{1:X2} [PC=#{2:X4}, T={3}]",
                    reg,
                    value,
                    m_cpu.regs.PC,
                    tact);
            }
        }

        #endregion Private
    }
}
