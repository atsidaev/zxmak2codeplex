﻿using System;
using System.Text;
using ZXMAK2.Engine.Cpu.Processor;


namespace ZXMAK2.Engine.Cpu.Tools
{
    public class TimingTool
    {
        private readonly Z80CPU _cpu;
        private readonly Func<ushort, byte> _memRead;


        public TimingTool(Z80CPU cpu, Func<ushort, byte> reader)
        {
            _cpu = cpu;
            _memRead = reader;
        }

        public string GetTimingString(int addr)
        {
            var t = GetTiming(addr);
            return t.HasValue ? string.Format("{0}T", t) : "N/A";
        }

        public int? GetTiming(int addr)
        {
            if (_memRead == null || _cpu == null)
            {
                return null;
            }
            var tableNumber = 0;
            var perfTime = 0;
            var offset = 0;
            var opCode = -1;
            while (true)
            {
                opCode = _memRead((ushort)(addr + offset));
                var res = Tables[tableNumber][opCode];
                if (res < 0xF0)
                {
                    return res + perfTime;
                }
                tableNumber = res - 0xF0;
                if (tableNumber >= 5)
                {
                    break;
                }
                perfTime += 4;
                offset++;
                if (tableNumber == 4)
                {
                    offset++;
                }
                if (perfTime > 1500000)
                {
                    LogAgent.Warn("TimingTool.GetTiming: {0}T reached!", perfTime); 
                    return null;
                }
            }
            switch (opCode)
            {
                case 0x10:
                    return DjnzTime(perfTime, 0x08, 0x0D);

                case 0xB1:
                case 0xB2:
                case 0xB9:
                case 0xBA:
                    return DjnzTime(perfTime, 0x0C, 0x11);

                case 0x20: return FlagTime(perfTime, 0x07, 0x0C, 0x40);
                case 0x28: return FlagTime(perfTime, 0x0C, 0x07, 0x40);
                case 0x30: return FlagTime(perfTime, 0x07, 0x0C, 0x01);
                case 0x38: return FlagTime(perfTime, 0x0C, 0x07, 0x01);
                case 0xC0: return FlagTime(perfTime, 0x05, 0x0B, 0x40);
                case 0xC8: return FlagTime(perfTime, 0x0B, 0x05, 0x40);
                case 0xD0: return FlagTime(perfTime, 0x05, 0x0B, 0x01);
                case 0xD8: return FlagTime(perfTime, 0x0B, 0x05, 0x01);
                case 0xE0: return FlagTime(perfTime, 0x05, 0x0B, 0x04);
                case 0xE8: return FlagTime(perfTime, 0x0B, 0x05, 0x04);
                case 0xF0: return FlagTime(perfTime, 0x05, 0x0B, 0x80);
                case 0xF8: return FlagTime(perfTime, 0x0B, 0x05, 0x80);
                case 0xC4: return FlagTime(perfTime, 0x0A, 0x11, 0x40);
                case 0xCC: return FlagTime(perfTime, 0x11, 0x0A, 0x40);
                case 0xD4: return FlagTime(perfTime, 0x0A, 0x11, 0x01);
                case 0xDC: return FlagTime(perfTime, 0x11, 0x0A, 0x01);
                case 0xE4: return FlagTime(perfTime, 0x0A, 0x11, 0x04);
                case 0xEC: return FlagTime(perfTime, 0x11, 0x0A, 0x04);
                case 0xF4: return FlagTime(perfTime, 0x0A, 0x11, 0x80);
                case 0xFC: return FlagTime(perfTime, 0x11, 0x0A, 0x80);

                case 0xB0:
                case 0xB8:
                    return RepTime(perfTime, 0x0C, 0x11);

                case 0xB3:
                case 0xBB:
                    return RepFlagTime(perfTime, 0x0C, 0x0C, 0x11);
            }
            LogAgent.Error(
                "TimingTool.GetTiming: unexpected opCode #{0:2X}", 
                opCode);
            return null;
        }


        #region Private

        private int DjnzTime(int timeBase, int timeEnd, int timeNoEnd)
        {
            var ifTime = _cpu.regs.B == 0x01 ? timeEnd : timeNoEnd;
            return timeBase + ifTime;
        }

        private int FlagTime(int timeBase, int timeOn, int timeOff, int flagMask)
        {
            var ifTime = (_cpu.regs.F & flagMask) != 0 ? timeOn : timeOff;
            return timeBase + ifTime;
        }

        private int RepTime(int perfTime, int timeEnd, int timeNoEnd)
        {
            var ifTime = _cpu.regs.BC == 0x0001 ? timeEnd : timeNoEnd;
            return perfTime + ifTime;
        }

        private int RepFlagTime(int timeEnd, int timeOn, int timeOff, int perfTime)
        {
            if (_cpu.regs.BC == 0x0001)
            {
                return perfTime + timeEnd;
            }
            var data = _memRead(_cpu.regs.HL);
            var ifTime = _cpu.regs.F == data ? timeOn : timeOff;
            return perfTime + ifTime;
        }

        #endregion Private


        #region Dictionary Tables

        private static int[] TaktsDirect = {
        0x04,0x0A,0x07,0x06,0x04,0x04,0x07,0x04, //;00h-07h
        0x04,0x0B,0x07,0x06,0x04,0x04,0x07,0x04, //;08h-0Fh
        0xFF,0x0A,0x07,0x06,0x04,0x04,0x07,0x04, //;10h-17h
        0x0C,0x0B,0x07,0x06,0x04,0x04,0x07,0x04, //;18h-1Fh
        0xFF,0x0A,0x10,0x06,0x04,0x04,0x07,0x04, //;20h-27h
        0xFF,0x0B,0x10,0x06,0x04,0x04,0x07,0x04, //;28h-2Fh
        0xFF,0x0A,0x0D,0x06,0x0B,0x0B,0x0A,0x04, //;30h-37h
        0xFF,0x0B,0x0D,0x06,0x04,0x04,0x07,0x04, //;38h-3Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;40h-47h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;48h-4Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;50h-57h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;58h-5Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;60h-67h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;68h-6Fh
        0x07,0x07,0x07,0x07,0x07,0x07,0x04,0x07, //;70h-77h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;78h-7Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;80h-87h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;88h-8Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;90h-97h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;98h-9Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;A0h-A7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;A8h-AFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;B0h-B7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x07,0x04, //;B8h-BFh
        0xFF,0x0A,0x0A,0x0A,0xFF,0x0B,0x07,0x0B, //;C0h-C7h
        0xFF,0x0A,0x0A,0xF1,0xFF,0x11,0x07,0x0B, //;C8h-CFh
        0xFF,0x0A,0x0A,0x0B,0xFF,0x0B,0x07,0x0B, //;D0h-D7h
        0xFF,0x04,0x0A,0x0B,0xFF,0xF3,0x07,0x0B, //;D8h-DFh
        0xFF,0x0A,0x0A,0x13,0xFF,0x0B,0x07,0x0B, //;E0h-E7h
        0xFF,0x04,0x0A,0x04,0xFF,0xF2,0x07,0x0B, //;E8h-EFh
        0xFF,0x0A,0x0A,0x04,0xFF,0x0B,0x07,0x0B, //;F0h-F7h
        0xFF,0x06,0x0A,0x04,0xFF,0xF3,0x07,0x0B  //;F8h-FFh
        };

        private static int[] TaktsCB = {                  
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;00h-07h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;08h-0Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;10h-17h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;18h-1Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;20h-27h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;28h-2Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;30h-37h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;38h-3Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;40h-47h
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;48h-4Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;50h-57h
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;58h-5Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;60h-67h
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;68h-6Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;70h-77h
        0x04,0x04,0x04,0x04,0x04,0x04,0x08,0x04, // ;78h-7Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;80h-87h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;88h-8Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;90h-97h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;98h-9Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;A0h-A7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;A8h-AFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;B0h-B7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;B8h-BFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;C0h-C7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;C8h-CFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;D0h-D7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;D8h-DFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;E0h-E7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;E8h-EFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04, // ;F0h-F7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0B,0x04  // ;F8h-FFh
        };             


        private static int[] TaktsED = {
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;00h-07h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;08h-0Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;10h-17h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;18h-1Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;20h-27h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;28h-2Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;30h-37h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;38h-3Fh
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x05, // ;40h-47h
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x05, // ;48h-4Fh
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x05, // ;50h-57h
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x05, // ;58h-5Fh
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x0E, // ;60h-67h
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x0E, // ;68h-6Fh
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x04, // ;70h-77h
        0x08,0x08,0x0B,0x10,0x04,0x0A,0x04,0x04, // ;78h-7Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;80h-87h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;88h-8Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;90h-97h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;98h-9Fh
        0x0C,0x0C,0x0C,0x0C,0x04,0x04,0x04,0x04, // ;A0h-A7h
        0x0C,0x0C,0x0C,0x0C,0x04,0x04,0x04,0x04, // ;A8h-AFh
        0xFF,0xFF,0xFF,0xFF,0x04,0x04,0x04,0x04, // ;B0h-B7h
        0xFF,0xFF,0xFF,0xFF,0x04,0x04,0x04,0x04, // ;B8h-BFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;C0h-C7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;C8h-CFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;D0h-D7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;D8h-DFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;E0h-E7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;E8h-EFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04, // ;F0h-F7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04  // ;F8h-FFh
        };             


        private static int[] TaktsDDFD = {
        0x04,0x0A,0x07,0x06,0x04,0x04,0x07,0x04, // ;00h-07h
        0x04,0x0B,0x07,0x06,0x04,0x04,0x07,0x04, // ;08h-0Fh
        0xFF,0x0A,0x07,0x06,0x04,0x04,0x07,0x04, // ;10h-17h
        0x0C,0x0B,0x07,0x06,0x04,0x04,0x07,0x04, // ;18h-1Fh
        0xFF,0x0A,0x10,0x06,0x04,0x04,0x07,0x04, // ;20h-27h
        0xFF,0x0B,0x10,0x06,0x04,0x04,0x07,0x04, // ;28h-2Fh
        0xFF,0x0A,0x0D,0x06,0x13,0x13,0x0F,0x04, // ;30h-37h
        0xFF,0x0B,0x0D,0x06,0x04,0x04,0x0F,0x04, // ;38h-3Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;40h-47h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;48h-4Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;50h-57h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;58h-5Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;60h-67h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;68h-6Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x04,0x0F, // ;70h-77h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;78h-7Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;80h-87h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;88h-8Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;90h-97h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;98h-9Fh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;A0h-A7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;A8h-AFh
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;B0h-B7h
        0x04,0x04,0x04,0x04,0x04,0x04,0x0F,0x04, // ;B8h-BFh
        0xFF,0x0A,0x0A,0x0A,0xFF,0x0B,0x0F,0x0B, // ;C0h-C7h
        0xFF,0x0A,0x0A,0xF4,0xFF,0x11,0x0F,0x0B, // ;C8h-CFh
        0xFF,0x0A,0x0A,0x0B,0xFF,0x0B,0x0F,0x0B, // ;D0h-D7h
        0xFF,0x04,0x0A,0x0B,0xFF,0xF3,0x0F,0x0B, // ;D8h-DFh
        0xFF,0x0A,0x0A,0x13,0xFF,0x0B,0x0F,0x0B, // ;E0h-E7h
        0xFF,0x04,0x0A,0x04,0xFF,0xF2,0x0F,0x0B, // ;E8h-EFh
        0xFF,0x0A,0x0A,0x04,0xFF,0x0B,0x0F,0x0B, // ;F0h-F7h
        0xFF,0x06,0x0A,0x04,0xFF,0xF3,0x0F,0x0B  // ;F8h-FFh
        };            

        private static int[] TaktsDDCBFDCB = {
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;00h-07h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;08h-0Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;10h-17h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;18h-1Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;20h-27h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;28h-2Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;30h-37h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;38h-3Fh
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;40h-47h
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;48h-4Fh
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;50h-57h
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;58h-5Fh
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;60h-67h
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;68h-6Fh
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;70h-77h
        0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C,0x0C, // ;78h-7Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;80h-87h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;88h-8Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;90h-97h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;98h-9Fh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;A0h-A7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;A8h-AFh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;B0h-B7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;B8h-BFh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;C0h-C7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;C8h-CFh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;D0h-D7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;D8h-DFh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;E0h-E7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;E8h-EFh
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F, // ;F0h-F7h
        0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F  // ;F8h-FFh
        };

        private static int[][] Tables = new int[5][] { 
            TaktsDirect, TaktsCB, TaktsED, TaktsDDFD, TaktsDDCBFDCB 
        };

        #endregion
    }
}
