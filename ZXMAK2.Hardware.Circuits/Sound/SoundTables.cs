﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXMAK2.Hardware.Circuits.Sound
{
    public static class SoundTables
    {
        public static uint[] AmplitudeAy8910 = new uint[32]
        {
            0x0000,0x0000,0x0340,0x0340,0x04C0,0x04C0,0x06F2,0x06F2,
            0x0A44,0x0A44,0x0F13,0x0F13,0x1510,0x1510,0x227E,0x227E,
            0x289F,0x289F,0x414E,0x414E,0x5B21,0x5B21,0x7258,0x7258,
            0x905E,0x905E,0xB550,0xB550,0xD7A0,0xD7A0,0xFFFF,0xFFFF,
        };

        public static uint[] AmplitudeYm2149 = new uint[32]
        {
            0x0000,0x0000,0x00EF,0x01D0,0x0290,0x032A,0x03EE,0x04D2,
            0x0611,0x0782,0x0912,0x0A36,0x0C31,0x0EB6,0x1130,0x13A0,
            0x1751,0x1BF5,0x20E2,0x2594,0x2CA1,0x357F,0x3E45,0x475E,
            0x5502,0x6620,0x7730,0x8844,0xA1D2,0xC102,0xE0A2,0xFFFF,
        };

        public static uint[] AmplitudeCustom = new uint[32]
        {
            0x0000,0x0000,0x00F8,0x01C2,0x029E,0x033A,0x03F2,0x04D7,
            0x0610,0x077F,0x090A,0x0A42,0x0C3B,0x0EC2,0x1137,0x13A7,
            0x1750,0x1BF9,0x20DF,0x2596,0x2C9D,0x3579,0x3E55,0x4768,
            0x54FF,0x6624,0x773B,0x883F,0xA1DA,0xC0FC,0xE094,0xFFFF,
        };

        public static uint[] PanAbc = new uint[6]    { 100,10,  66,66,   10,100,  };
        public static uint[] PanAcb = new uint[6]    { 100,10,  10,100,  66,66,   };
        public static uint[] PanBac = new uint[6]    { 66,66,   100,10,  10,100,  };
        public static uint[] PanBca = new uint[6]    { 10,100,  100,10,  66,66,   };
        public static uint[] PanCab = new uint[6]    { 66,66,   10,100,  100,10,  };
        public static uint[] PanCba = new uint[6]    { 10,100,  66,66,   100,10,  };
        public static uint[] PanMono = new uint[6]   { 100,100, 100,100, 100,100, };
        public static uint[] PanCustom = new uint[6] { 90,15,   60,60,   15,90,   };
    }
}