﻿namespace ZXMAK2.Controls
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuMain = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuFileOpen = new System.Windows.Forms.MenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.MenuItem();
            this.menuFileSplitter = new System.Windows.Forms.MenuItem();
            this.menuFileExit = new System.Windows.Forms.MenuItem();
            this.menuView = new System.Windows.Forms.MenuItem();
            this.menuViewSize = new System.Windows.Forms.MenuItem();
            this.menuViewSizeX1 = new System.Windows.Forms.MenuItem();
            this.menuViewSizeX2 = new System.Windows.Forms.MenuItem();
            this.menuViewSizeX3 = new System.Windows.Forms.MenuItem();
            this.menuViewSizeX4 = new System.Windows.Forms.MenuItem();
            this.menuViewWindowed = new System.Windows.Forms.MenuItem();
            this.menuViewFullscreen = new System.Windows.Forms.MenuItem();
            this.menuViewSeparator1 = new System.Windows.Forms.MenuItem();
            this.menuViewSmoothing = new System.Windows.Forms.MenuItem();
            this.menuViewNoFlic = new System.Windows.Forms.MenuItem();
            this.menuViewScaleMode = new System.Windows.Forms.MenuItem();
            this.menuViewScaleModeStretch = new System.Windows.Forms.MenuItem();
            this.menuViewScaleModeKeepProportion = new System.Windows.Forms.MenuItem();
            this.menuViewScaleModeFixedPixelSize = new System.Windows.Forms.MenuItem();
            this.menuViewVBlankSync = new System.Windows.Forms.MenuItem();
            this.menuViewDisplayIcon = new System.Windows.Forms.MenuItem();
            this.menuViewDebugInfo = new System.Windows.Forms.MenuItem();
            this.menuVm = new System.Windows.Forms.MenuItem();
            this.menuVmPause = new System.Windows.Forms.MenuItem();
            this.menuVmMaximumSpeed = new System.Windows.Forms.MenuItem();
            this.menuVmSeparator2 = new System.Windows.Forms.MenuItem();
            this.menuVmReset = new System.Windows.Forms.MenuItem();
            this.menuVmNmi = new System.Windows.Forms.MenuItem();
            this.menuVmSeparator1 = new System.Windows.Forms.MenuItem();
            this.menuVmSettings = new System.Windows.Forms.MenuItem();
            this.menuTools = new System.Windows.Forms.MenuItem();
            this.menuHelp = new System.Windows.Forms.MenuItem();
            this.menuHelpViewHelp = new System.Windows.Forms.MenuItem();
            this.menuHelpKeyboard = new System.Windows.Forms.MenuItem();
            this.menuHelpSeparator1 = new System.Windows.Forms.MenuItem();
            this.menuHelpAbout = new System.Windows.Forms.MenuItem();
            this.renderVideo = new ZXMAK2.Controls.RenderVideo();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuView,
            this.menuVm,
            this.menuTools,
            this.menuHelp});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileOpen,
            this.menuFileSaveAs,
            this.menuFileSplitter,
            this.menuFileExit});
            this.menuFile.Text = "File";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Index = 0;
            this.menuFileOpen.Text = "Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Index = 1;
            this.menuFileSaveAs.Text = "Save as...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            // 
            // menuFileSplitter
            // 
            this.menuFileSplitter.Index = 2;
            this.menuFileSplitter.Text = "-";
            // 
            // menuFileExit
            // 
            this.menuFileExit.Index = 3;
            this.menuFileExit.Text = "Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuView
            // 
            this.menuView.Index = 1;
            this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuViewSize,
            this.menuViewWindowed,
            this.menuViewFullscreen,
            this.menuViewSeparator1,
            this.menuViewSmoothing,
            this.menuViewNoFlic,
            this.menuViewScaleMode,
            this.menuViewVBlankSync,
            this.menuViewDisplayIcon,
            this.menuViewDebugInfo});
            this.menuView.Text = "View";
            this.menuView.Popup += new System.EventHandler(this.menuView_Popup);
            // 
            // menuViewSize
            // 
            this.menuViewSize.Index = 0;
            this.menuViewSize.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuViewSizeX1,
            this.menuViewSizeX2,
            this.menuViewSizeX3,
            this.menuViewSizeX4});
            this.menuViewSize.Text = "Size";
            // 
            // menuViewSizeX1
            // 
            this.menuViewSizeX1.Index = 0;
            this.menuViewSizeX1.Text = "100%";
            this.menuViewSizeX1.Click += new System.EventHandler(this.menuViewX_Click);
            // 
            // menuViewSizeX2
            // 
            this.menuViewSizeX2.Index = 1;
            this.menuViewSizeX2.Text = "200%";
            this.menuViewSizeX2.Click += new System.EventHandler(this.menuViewX_Click);
            // 
            // menuViewSizeX3
            // 
            this.menuViewSizeX3.Index = 2;
            this.menuViewSizeX3.Text = "300%";
            this.menuViewSizeX3.Click += new System.EventHandler(this.menuViewX_Click);
            // 
            // menuViewSizeX4
            // 
            this.menuViewSizeX4.Index = 3;
            this.menuViewSizeX4.Text = "400%";
            this.menuViewSizeX4.Click += new System.EventHandler(this.menuViewX_Click);
            // 
            // menuViewWindowed
            // 
            this.menuViewWindowed.Index = 1;
            this.menuViewWindowed.Text = "Windowed";
            this.menuViewWindowed.Click += new System.EventHandler(this.menuViewWindowed_Click);
            // 
            // menuViewFullscreen
            // 
            this.menuViewFullscreen.Index = 2;
            this.menuViewFullscreen.Text = "Fullscreen   (Alt+Enter)";
            this.menuViewFullscreen.Click += new System.EventHandler(this.menuViewFullscreen_Click);
            // 
            // menuViewSeparator1
            // 
            this.menuViewSeparator1.Index = 3;
            this.menuViewSeparator1.Text = "-";
            // 
            // menuViewSmoothing
            // 
            this.menuViewSmoothing.Index = 4;
            this.menuViewSmoothing.Text = "Smoothing";
            this.menuViewSmoothing.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewNoFlic
            // 
            this.menuViewNoFlic.Index = 5;
            this.menuViewNoFlic.Text = "No Flic";
            this.menuViewNoFlic.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewScaleMode
            // 
            this.menuViewScaleMode.Index = 6;
            this.menuViewScaleMode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuViewScaleModeStretch,
            this.menuViewScaleModeKeepProportion,
            this.menuViewScaleModeFixedPixelSize});
            this.menuViewScaleMode.Text = "Scale";
            // 
            // menuViewScaleModeStretch
            // 
            this.menuViewScaleModeStretch.Index = 0;
            this.menuViewScaleModeStretch.Text = "Stretch";
            this.menuViewScaleModeStretch.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewScaleModeKeepProportion
            // 
            this.menuViewScaleModeKeepProportion.Index = 1;
            this.menuViewScaleModeKeepProportion.Text = "Keep Proportion";
            this.menuViewScaleModeKeepProportion.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewScaleModeFixedPixelSize
            // 
            this.menuViewScaleModeFixedPixelSize.Index = 2;
            this.menuViewScaleModeFixedPixelSize.Text = "Fixed Pixel Size";
            this.menuViewScaleModeFixedPixelSize.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewVBlankSync
            // 
            this.menuViewVBlankSync.Index = 7;
            this.menuViewVBlankSync.Text = "VBlank Sync";
            this.menuViewVBlankSync.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewDisplayIcon
            // 
            this.menuViewDisplayIcon.Index = 8;
            this.menuViewDisplayIcon.Text = "Display Icons";
            this.menuViewDisplayIcon.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuViewDebugInfo
            // 
            this.menuViewDebugInfo.Index = 9;
            this.menuViewDebugInfo.Text = "Debug Info";
            this.menuViewDebugInfo.Click += new System.EventHandler(this.menuViewRender_Click);
            // 
            // menuVm
            // 
            this.menuVm.Index = 2;
            this.menuVm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuVmPause,
            this.menuVmMaximumSpeed,
            this.menuVmSeparator2,
            this.menuVmReset,
            this.menuVmNmi,
            this.menuVmSeparator1,
            this.menuVmSettings});
            this.menuVm.Text = "VM";
            this.menuVm.Popup += new System.EventHandler(this.menuVm_Popup);
            // 
            // menuVmPause
            // 
            this.menuVmPause.Index = 0;
            this.menuVmPause.Text = "Pause";
            this.menuVmPause.Click += new System.EventHandler(this.menuVmPause_Click);
            // 
            // menuVmMaximumSpeed
            // 
            this.menuVmMaximumSpeed.Index = 1;
            this.menuVmMaximumSpeed.Text = "Maximum Speed";
            this.menuVmMaximumSpeed.Click += new System.EventHandler(this.menuVmMaximumSpeed_Click);
            // 
            // menuVmSeparator2
            // 
            this.menuVmSeparator2.Index = 2;
            this.menuVmSeparator2.Text = "-";
            // 
            // menuVmReset
            // 
            this.menuVmReset.Index = 3;
            this.menuVmReset.Text = "RESET    (Alt+Ctrl+Insert)";
            this.menuVmReset.Click += new System.EventHandler(this.menuVmReset_Click);
            // 
            // menuVmNmi
            // 
            this.menuVmNmi.Index = 4;
            this.menuVmNmi.Text = "NMI";
            this.menuVmNmi.Click += new System.EventHandler(this.menuVmNmi_Click);
            // 
            // menuVmSeparator1
            // 
            this.menuVmSeparator1.Index = 5;
            this.menuVmSeparator1.Text = "-";
            // 
            // menuVmSettings
            // 
            this.menuVmSettings.Index = 6;
            this.menuVmSettings.Text = "Settings";
            this.menuVmSettings.Click += new System.EventHandler(this.menuVmOptions_Click);
            // 
            // menuTools
            // 
            this.menuTools.Index = 3;
            this.menuTools.Text = "Tools";
            // 
            // menuHelp
            // 
            this.menuHelp.Index = 4;
            this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuHelpViewHelp,
            this.menuHelpKeyboard,
            this.menuHelpSeparator1,
            this.menuHelpAbout});
            this.menuHelp.Text = "Help";
            // 
            // menuHelpViewHelp
            // 
            this.menuHelpViewHelp.Index = 0;
            this.menuHelpViewHelp.Text = "View Help";
            this.menuHelpViewHelp.Click += new System.EventHandler(this.menuHelpViewHelp_Click);
            // 
            // menuHelpKeyboard
            // 
            this.menuHelpKeyboard.Index = 1;
            this.menuHelpKeyboard.Text = "Keyboard Help";
            this.menuHelpKeyboard.Click += new System.EventHandler(this.menuHelpKeyboard_Click);
            // 
            // menuHelpSeparator1
            // 
            this.menuHelpSeparator1.Index = 2;
            this.menuHelpSeparator1.Text = "-";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Index = 3;
            this.menuHelpAbout.Text = "About";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // renderVideo
            // 
            this.renderVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderVideo.Location = new System.Drawing.Point(0, 0);
            this.renderVideo.Name = "renderVideo";
            this.renderVideo.Size = new System.Drawing.Size(640, 421);
            this.renderVideo.TabIndex = 0;
            this.renderVideo.Text = "renderVideo";
            this.renderVideo.DeviceReset += new System.EventHandler(this.renderVideo_DeviceReset);
            this.renderVideo.SizeChanged += new System.EventHandler(this.renderVideo_SizeChanged);
            this.renderVideo.DoubleClick += new System.EventHandler(this.renderVideo_DoubleClick);
            this.renderVideo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderVideo_MouseMove);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 421);
            this.Controls.Add(this.renderVideo);
            this.KeyPreview = true;
            this.Menu = this.menuMain;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZXMAK2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormMain_DragEnter);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.RenderVideo renderVideo;
        private System.Windows.Forms.MainMenu menuMain;
        private System.Windows.Forms.MenuItem menuFile;
        private System.Windows.Forms.MenuItem menuFileOpen;
        private System.Windows.Forms.MenuItem menuFileSaveAs;
        private System.Windows.Forms.MenuItem menuFileSplitter;
        private System.Windows.Forms.MenuItem menuFileExit;
        private System.Windows.Forms.MenuItem menuView;
        private System.Windows.Forms.MenuItem menuViewFullscreen;
        private System.Windows.Forms.MenuItem menuViewWindowed;
        private System.Windows.Forms.MenuItem menuViewSize;
        private System.Windows.Forms.MenuItem menuViewSizeX1;
        private System.Windows.Forms.MenuItem menuViewSizeX2;
        private System.Windows.Forms.MenuItem menuViewSizeX3;
        private System.Windows.Forms.MenuItem menuViewSizeX4;
        private System.Windows.Forms.MenuItem menuViewSeparator1;
        private System.Windows.Forms.MenuItem menuViewSmoothing;
        private System.Windows.Forms.MenuItem menuViewScaleMode;
        private System.Windows.Forms.MenuItem menuViewVBlankSync;
        private System.Windows.Forms.MenuItem menuViewDisplayIcon;
        private System.Windows.Forms.MenuItem menuViewDebugInfo;
        private System.Windows.Forms.MenuItem menuViewNoFlic;
        private System.Windows.Forms.MenuItem menuVm;
        private System.Windows.Forms.MenuItem menuVmReset;
        private System.Windows.Forms.MenuItem menuVmNmi;
        private System.Windows.Forms.MenuItem menuVmSeparator1;
        private System.Windows.Forms.MenuItem menuVmSettings;
        private System.Windows.Forms.MenuItem menuVmPause;
        private System.Windows.Forms.MenuItem menuVmSeparator2;
		private System.Windows.Forms.MenuItem menuVmMaximumSpeed;
        private System.Windows.Forms.MenuItem menuTools;
        private System.Windows.Forms.MenuItem menuHelp;
        private System.Windows.Forms.MenuItem menuHelpViewHelp;
        private System.Windows.Forms.MenuItem menuHelpKeyboard;
        private System.Windows.Forms.MenuItem menuHelpSeparator1;
        private System.Windows.Forms.MenuItem menuHelpAbout;
        private System.Windows.Forms.MenuItem menuViewScaleModeStretch;
        private System.Windows.Forms.MenuItem menuViewScaleModeKeepProportion;
        private System.Windows.Forms.MenuItem menuViewScaleModeFixedPixelSize;
    }
}

