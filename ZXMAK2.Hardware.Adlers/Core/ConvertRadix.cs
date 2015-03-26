﻿using System;
using System.Collections.Generic;


namespace ZXMAK2.Hardware.Adlers
{
    public static class ConvertRadix
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        public static ushort ParseUInt16(string text, int radix)
        {
            if (radix < 2 || radix > Alphabet.Length)
            {
                throw new ArgumentOutOfRangeException("radix");
            }
            text = text.Trim().ToUpperInvariant();

            var pos = 0;
            var result = 0;

            // Use lookup table to parse string
            while (pos < text.Length && !char.IsWhiteSpace(text[pos]))
            {
                var digit = text.Substring(pos, 1);
                var i = Alphabet.IndexOf(digit);
                if (i >= 0 && i < radix)
                {
                    result *= radix;
                    result += i;
                    pos++;
                    if (result > ushort.MaxValue)
                    {
                        // Overflow
                        throw new CommandParseException(
                            string.Format("Value out of range: {0}", text));
                    }
                }
                else
                {
                    // Invalid character encountered
                    throw new CommandParseException(
                        string.Format("Invalid character encountered: {0}", text));
                }
            }
            // Return true if any characters processed
            if (pos < 1)
            {
                throw new CommandParseException("Missing value");
            }
            return (ushort)result;
        }

        ////////////////////////////////////////////////////////////////////
        //
        // Method: convertNumberWithPrefix()
        //       - default number(without prefix) is hexadecimal
        //
        ////////////////////////////////////////////////////////////////////
        public static UInt16 ConvertNumberWithPrefix(string input) //Prefix: % - binary, # - hexadecimal
        {
            if (input == null || input.Trim() == String.Empty)
            {
                throw new CommandParseException("Empty or null value cannot be converted => error");
            }
            string inputTrimmed = input.Trim();

            // % - binary
            if (inputTrimmed[0] == '%')
            {
                var number = inputTrimmed.Substring(1, inputTrimmed.Length - 1);
                return ConvertRadix.ParseUInt16(number, 2);
            }
            // $ - decimal
            if (inputTrimmed[0] == '$')
            {
                var number = inputTrimmed.Substring(1, inputTrimmed.Length - 1);
                return ConvertRadix.ParseUInt16(number, 10);
            }

            // '#' or 'x' - hexadecimal
            if (inputTrimmed[0] == '#' || inputTrimmed[0] == 'x' )
            {
                var number = inputTrimmed.Substring(1, inputTrimmed.Length - 1);
                return ConvertRadix.ParseUInt16(number, 16);
            }
            // '0x' - hexadecimal
            if (inputTrimmed.Length >= 3)
            {
                if (inputTrimmed[0] == '0' && inputTrimmed[1] == 'x')
                {
                    var number = inputTrimmed.Substring(2, inputTrimmed.Length - 2);
                    return ConvertRadix.ParseUInt16(number, 16);

                }
            }
            return ConvertRadix.ParseUInt16(inputTrimmed, 16);
        }

        ////////////////////////////////////////////////////////////////////
        //
        // Method: convertASCIIStringToBytes()
        //
        ////////////////////////////////////////////////////////////////////
        public static byte[] convertASCIIStringToBytes(string input)
        {
            List<byte> arrOut = new List<byte>();
            foreach (char c in input)
            {
                if (c == '"')
                    continue;
                arrOut.Add((byte)c);
            }

            return arrOut.ToArray();
        }

        ////////////////////////////////////////////////////////////////////
        //
        // Method: GetBytesInStringBetweenCharacter()
        //
        // Returns byte[] of string between character, e.g. ld (C350), "Hello there" => returns byte[] { Hello there}
        //
        ////////////////////////////////////////////////////////////////////
        public static byte[] GetBytesInStringBetweenCharacter(string i_inputStr, char i_chrToParse, int i_startPos = -1)
        {
            bool hasStartChar = i_startPos != -1;

            List<byte> arrOut = new List<byte>();
            string strToParse;
            if (i_startPos != -1)
                strToParse = i_inputStr.Substring(i_startPos, i_inputStr.Length - i_startPos);
            else
                strToParse = i_inputStr;
            foreach (char c in strToParse)
            {
                if (c == '"')
                {
                    if (hasStartChar)
                        break;
                    hasStartChar = true;
                    continue;
                }
                if (hasStartChar)
                    arrOut.Add((byte)c);
            }
            return arrOut.ToArray();
        }

        ////////////////////////////////////////////////////////////////////
        //
        // Method: RemoveEndLinesAtEnd()
        //
        // removes all newlines from string
        ////////////////////////////////////////////////////////////////////
        public static string RemoveFormattingChars(ref string i_Str, bool i_bTrimStart)
        {
            char[] arrStr = i_Str.ToCharArray();
            string strTrimmed = string.Empty;
            foreach (char chr in arrStr)
                if (chr != '\n' && chr != '\r' && !(chr == ' ' && i_bTrimStart && strTrimmed == string.Empty))
                    strTrimmed += chr;

            i_Str = strTrimmed;
            return i_Str;
            /*i_Str = i_Str.Trim('\n', '\r');
            return i_Str;*/
        }

        // bits: 0-1-2-3-4-5-6-7-8 : 1-2-4-8-16-32-64-128
        public static void setBitInByteRightToLeft(ref byte manipulatinByte, byte bitToBeSet)
        {
            manipulatinByte ^= (byte)(1 << (7 - bitToBeSet));
        }

        // bits: 7-6-5-4-3-2-1-0 : 128-64-32-16-8-4-2-1
        public static void setBitInByte(ref byte manipulatinByte, byte bitToBeSet)
        {
            byte bitValue = Convert.ToByte(Math.Pow(2, bitToBeSet));

            if ((manipulatinByte & bitValue) == bitValue) // bit already set
                return;

            manipulatinByte += bitValue;

            // another solution:
            // number |= 1 << bitToBeSet;
        }

        public unsafe static byte[] PointerToManagedType(byte* i_arrayStart, int i_length)
        {
            if( i_length <= 0  )
                return null;
            else
            {
                byte[] arrOut = new byte[i_length];

                for (int counter = 0; counter < i_length; counter++)
                {
                    arrOut[counter] = i_arrayStart[counter];
                }
                return arrOut;
            }
        }
    }
}
