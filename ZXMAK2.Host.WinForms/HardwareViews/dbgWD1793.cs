using System;
using System.Windows.Forms;

using ZXMAK2.Hardware.Circuits.Fdd;
using ZXMAK2.Presentation.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Views;


namespace ZXMAK2.Host.WinForms.HardwareViews
{
    public partial class dbgWD1793 : FormView, IFddDebugView
    {
        private Wd1793 _wd1793;

        public dbgWD1793(Wd1793 debugTarget)
        {
            _wd1793 = debugTarget;
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (_wd1793 != null)
                label1.Text = _wd1793.DumpState();
            else
                label1.Text = "Beta Disk interface not found";
        }
    }
}