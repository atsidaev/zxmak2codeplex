﻿using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ZXMAK2.Interfaces;
using System.Windows.Forms;

namespace ZXMAK2.Controls.Debugger
{

    #region Debugger enums/structs, ...
    // enum BreakPointConditionType
    // e.g.: 1.) memoryVsValue = left is memory reference, right is number(#9C40, %1100, 6755, ...)
    //       2.) valueVsRegister = left is value, right is register value
    //
    public enum BreakPointConditionType { memoryVsValue, valueVsRegister, registryVsValue, registryMemoryReferenceVsValue };

    //Information about extended breakpoint
    public struct breakpointInfo
    {
        public BreakPointConditionType accessType;

        //condition in string, e.g.: "pc", "(#9C40)"
        public string leftCondition;
        public string rightCondition;

        //value of condition(if relevant), valid for whole values or memory access
        public ushort leftValue;
        public ushort rightValue;

        //condition type
        public string conditionTypeSign; // "!=", "==", "<", ...

        //is active
        public bool isOn;
    }
    #endregion

    public partial class FormCpu : Form
    {
        public static string[] Regs16Bit = new string[] { "AF", "BC", "DE", "HL", "IX", "IY", "SP", "IR", "PC", "AF'", "BC'", "DE'", "HL'" };
        public static char[]   Regs8Bit  = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'H', 'L' };

        public enum CommandType { memoryOrRegistryManipulation, breakpointManipulation, gotoAdress, removeBreakpoint, Unidentified }; // E.g.: ld = memoryOrRegistryManipulation
        public enum BreakPointAccessType { memoryAccess, memoryWrite, memoryChange, registryValue, All, Undefined };

        public enum CharType { Number = 0, Letter, Other };

        public static string DbgKeywordLD = "ld"; // memory/registers modification(=ld as in assembler)
        public static string DbgKeywordBREAK = "br"; // set breakpoint
        public static string DbgKeywordDissassemble = "ds"; // dasmPanel - goto adress(disassembly panel), (=disassembly)
        public static string DbgRemoveBreakpoint = "del"; // remove breakpoint, e.g.: del 1 - will delete breakpoint nr. 1

        static char[]  debugDelimitersOther = new char[] { '(', '=', ')', '!' };

        // Main method - returns string list with items entered in debug command line, e.g. : 
        //
        // 1. item: ld
        // 2. item: bc
        // 3. item: #4000
        //
        public static List<string> ParseCommand(string dbgCommand)
        {
            try
            {
                string[] delimiters = new string[] { ",", " " };
                string[] parts = dbgCommand.Split(delimiters,
                                 StringSplitOptions.RemoveEmptyEntries);

                return CorrectSplitDbgCommands(parts); // toto treba, lebo napr. moze vzniknut pc==#3455(v kope)
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<string> CorrectSplitDbgCommands(string[] i_splittedCommands)
        {
            char[] delimiters2 = new char[] { '=', '!' };
            List<string> dbgCommandsList = new List<string>();

            dbgCommandsList.Clear(); // output List

            for (int counter = 0; counter < i_splittedCommands.Length; counter++)
            {
                string actItem = i_splittedCommands[counter];

                bool hasLetters = false;
                bool hasDigits = false;
                bool hasOtherChars = false;

                HasDigitsAndLettersInString(actItem, ref hasLetters, ref hasDigits);
                HasOtherCharsInString(actItem, delimiters2, ref hasOtherChars);

                // treba opravit dany string ?...Priklad takeho chybneho: "pc==#0000"
                if (((hasLetters ? 1 : 0) + (hasDigits ? 1 : 0) + (hasOtherChars ? 1 : 0)) > 1)
                {
                    // treba opravovat - inicializacia
                    CharType prevCharType = getCharType(actItem[0]);

                    string actCommand = String.Empty;

                    for (byte counterCmdChars = 0; counterCmdChars < actItem.Length; counterCmdChars++)
                    {
                        char actCharInDbgCommand = actItem[counterCmdChars];

                        CharType actCharType = getCharType(actCharInDbgCommand);

                        if (actCharType != prevCharType)
                        {
                            dbgCommandsList.Add(actCommand);

                            actCommand = actCharInDbgCommand.ToString();
                            prevCharType = actCharType;
                            actCharType = getCharType(actCharInDbgCommand);

                        }
                        else
                        {
                            actCommand += actItem[counterCmdChars];
                        }
                    }

                    dbgCommandsList.Add(actCommand);

                }
                else
                    dbgCommandsList.Add(actItem);
            }

            return dbgCommandsList;
        }

        public static void HasDigitsAndLettersInString(string s, ref bool hasLetters, ref bool hasDigits)
        {
            bool parsingDigits = false;

            foreach (char c in s)
            {
                if (Char.IsLetter(c) && !parsingDigits) //parsingDigits - do not consider [A-Fa-f] as letter in case we`re parsing number
                {
                    hasLetters = true;
                    parsingDigits = false;
                    continue;
                }

                if (Char.IsDigit(c) || c == '%' || c == '#' || c == '(' || c == ')')  // % - binary number, # - hex number; '(' and ')' are also digits
                {
                    hasDigits = true;
                    parsingDigits = true;
                    continue;
                }
            }
        }

        public static void HasOtherCharsInString(string s, char[] searchingChars, ref bool hasOtherChars)
        {
            for (byte listCounter = 0; listCounter < searchingChars.Length; listCounter++)
            {
                if (s.IndexOf(searchingChars[listCounter]) >= 0)
                {
                    hasOtherChars = true;
                    return;
                }
            }
        }

        public static CharType getCharType(char inputChar)
        {
            if (Char.IsLetter(inputChar))
                return CharType.Letter;

            if (Char.IsDigit(inputChar) || inputChar == '%' || inputChar == '#') // % - binary number, # - hex number
                return CharType.Letter;

            foreach (char c in debugDelimitersOther)
            {
                if (c == inputChar)
                    return CharType.Other;
            }

            throw new Exception("Incorrect character found: " + inputChar);
        }

        //Method will resolve whether command entered is memory modification or breakpoint setting
        public static CommandType getDbgCommandType(List<string> command)
        {
            if (command[0].ToUpper() == DbgKeywordLD.ToString().ToUpper())
            {
                return CommandType.memoryOrRegistryManipulation;
            }

            if (command[0].ToUpper() == DbgKeywordBREAK.ToString().ToUpper())
            {
                return CommandType.breakpointManipulation;
            }

            if (command[0].ToUpper() == DbgKeywordDissassemble.ToString().ToUpper())
            {
                return CommandType.gotoAdress;
            }

            if (command[0].ToUpper() == DbgRemoveBreakpoint.ToString().ToUpper())
            {
                return CommandType.removeBreakpoint;
            }

            return CommandType.Unidentified;
        }

        ////////////////////////////////////////////////////////////////////
        //
        // Method: convertNumberWithPrefix()
        //
        public static UInt16 convertNumberWithPrefix(string input) //Prefix: % - binary, # - hexadecimal
        {
            try
            {
                // % - binary
                if (input[0] == '%')
                {
                    string number = input.Substring(1, input.Length - 1 );
                    return Convert.ToUInt16(number, 2);
                }

                // # - hexadecimal
                if (input[0] == '#')
                {
                    string number = input.Substring(1, input.Length - 1);
                    return Convert.ToUInt16(number, 16);
                }

                return Convert.ToUInt16(input); // maybe decimal number
            }
            catch (Exception)
            {
                throw new Exception("Incorrect number in convertNumberWithPrefix(), number=" + input.ToString());
            }
        }

        public static bool isRegistry(string input)
        {
            try
            {
                string registry = input.ToUpper().Trim();

                //not available in .NET Framework 2.0 - so must coding :-)
                /*if (Regs16Bit.ToArray().Contains<string>(registry))
                    return true;
                if (Regs8Bit.Contains<char>(Convert.ToChar(registry)))
                    return true;*/

                for (byte counter = 0; counter < Regs16Bit.Length; counter++)
                {
                    if (Regs16Bit[counter] == registry)
                        return true;
                }

                //now only low(8bit) registry are allowed, such as A, B, C, L, D, ...
                if (registry.Length > 1)
                    return false;

                for (byte counter = 0; counter < Regs8Bit.Length; counter++)
                {
                    if ( Regs8Bit[counter].ToString() == registry)
                        return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool isRegistryMemoryReference(string registryMemoryReference)
        {
            string registry = getRegistryFromReference(registryMemoryReference);
            if (isRegistry(registry))
                return true;

            return false;
        }

        public static string getRegistryFromReference( string registryMemoryRef )
        {
            if (registryMemoryRef.Length < 4 || !registryMemoryRef.StartsWith("(") || !registryMemoryRef.EndsWith(")")) // (PC), (DE), (hl), ...
                return String.Empty;

            return registryMemoryRef.Substring(1, registryMemoryRef.Length - 2);
        }

        public static bool isMemoryReference(string input)
        {
            if (input.StartsWith("(") && input.EndsWith(")"))
                return true;

            return false;
        }

        public static UInt16 getReferencedMemoryPointer(string input)
        {
            if (!isMemoryReference(input))
                throw new Exception("Incorrect memory reference: " + input);

            return convertNumberWithPrefix(input.Substring(1, input.Length - 2));
        }

        public static BreakPointAccessType getBreakpointType( List<string> breakpoint )
        {
            try
            {
                string left  = breakpoint[1];
                string right = breakpoint[3];

                if (isMemoryReference(left) || isMemoryReference(right))
                    return BreakPointAccessType.memoryChange;
            }
            catch(Exception)
            {
            }

            return BreakPointAccessType.Undefined;
        }

        public static ushort getRegistryValueByName( ZXMAK2.Engine.Z80.REGS regs, string i_registryName)
        {
            string registryName = i_registryName.ToUpper();

            switch (registryName)
            {
                case "PC":
                    return regs.PC;
                case "IR":
                    return regs.IR;
                case "SP":
                    return regs.SP;
                case "AF":
                    return regs.AF;
                case "A":
                    return (ushort)(regs.AF >> 8);
                case "HL":
                    return regs.HL;
                case "DE":
                    return regs.DE;
                case "BC":
                    return regs.BC;
                case "IX":
                    return regs.IX;
                case "IY":
                    return regs.IY;
                case "AF'":
                    return regs._AF;
                case "HL'":
                    return regs._HL;
                case "DE'":
                    return regs._DE;
                case "BC'":
                    return regs._BC;
                case "MW (Memptr Word)":
                    return regs.MW;
                default:
                    throw new Exception("Bad registry name: " + i_registryName);
            }
        }
    }
}