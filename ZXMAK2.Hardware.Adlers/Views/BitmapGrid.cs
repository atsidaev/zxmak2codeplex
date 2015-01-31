﻿using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.Adlers.Core;

namespace ZXMAK2.Hardware.Adlers.Views
{
    class BitmapGrid : Panel
    {
        private byte X_BIT_COUNT = 8, Y_BIT_COUNT = 8*3;           // Grid Height and Width
        private BitArray[] _gridBits = null;

        private Size _originalSize;

        //private bool _isInitialised = false;
        //private bool _needRepaint = true;

        private IDebuggable _spectrum = null;

        public BitmapGrid()
        {
        }

        public void Init()
        {
            _originalSize = this.Size;
        }
        public void Init(IDebuggable i_spectrum, byte i_gridWidth, byte i_gridHeight)
        {
            Init();

            _spectrum = i_spectrum;
            X_BIT_COUNT = i_gridWidth;
            Y_BIT_COUNT = i_gridHeight;

            //_isInitialised = true;
        }

        public int getGridBit(int XBit, int YBit)
        {
            return XBit + YBit * X_BIT_COUNT;
        }
        public void setBitmapBits(IDebuggable i_spectrum, ushort i_startAddress)
        {
            int gridBytes = X_BIT_COUNT / 8 * Y_BIT_COUNT;
            _gridBits = new BitArray[gridBytes];

            for( int counter = 0; counter < gridBytes; counter++ )
            {
                byte blockByte = i_spectrum.ReadMemory(i_startAddress++);
                _gridBits[counter] = GraphicsTools.getAttributePixels(blockByte, false);
            }
        }

        public void setGridWidth(byte i_newWidth )
        {
            X_BIT_COUNT = i_newWidth;
            //_needRepaint = true;
            //this.Draw(null);
        }
        public void setGridHeight(byte i_newHeight)
        {
            Y_BIT_COUNT = i_newHeight;
            //_needRepaint = true;
            //this.Draw(null);
        }

        public void Draw(PaintEventArgs e)
        {
            if (_gridBits == null)
                return;

            using (Graphics g = ( e == null ? this.CreateGraphics() : e.Graphics))
            {
                //g.Clear(Color.Black);

                int bitWidth = _originalSize.Width / X_BIT_COUNT;
                int bitHeight = _originalSize.Height / Y_BIT_COUNT;
                Rectangle rect = ClientRectangle;
                rect.Size = new Size(bitWidth, bitHeight);

                int arrBitmapCounter = 0;
                int startX = 2; // grid margin
                int startY = 2; // grid margin
                this.Width = bitWidth * X_BIT_COUNT + 8;
                this.Height = bitHeight * Y_BIT_COUNT + 8;
                for (int counterY = 0; counterY < Y_BIT_COUNT; counterY++)
                    for (int counter = 0; counter < X_BIT_COUNT; counter++)
                    {
                        Color bitColor = _gridBits[arrBitmapCounter / 8][arrBitmapCounter % 8] ? Color.White : Color.DarkGray;
                        arrBitmapCounter++;

                        rect.Location = new Point(startX + (counter * bitWidth), startY + (counterY * bitHeight));
                        using (Brush brush = new SolidBrush(bitColor))
                        {
                            g.FillRectangle(brush, rect);
                        }
                        using (Pen pen = new Pen(Color.Black))
                        {
                            g.DrawRectangle(pen, rect);
                        }
                    }
            }

            //_needRepaint = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.Draw(e);
        }

        public int getClickedPixel(MouseEventArgs e)
        {
            int pixelClicked = getGridBit(e.X / (this.Width / X_BIT_COUNT), e.Y / (this.Height / Y_BIT_COUNT));
            bool setValue = !_gridBits[pixelClicked / 8][pixelClicked % 8];

            _gridBits[pixelClicked / 8][pixelClicked % 8] = setValue;

            //_needRepaint = true;
            this.Draw(null);

            return pixelClicked;
        }
    }
}