﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using ZXMAK2.Dependency;
using ZXMAK2.Engine;
using ZXMAK2.Hardware.Adlers.Core;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Views.AssemblerView
{
    public class Compiler
    {
        public static int DoCompile(string i_compileOption, string i_sourceOrFileName, ref COMPILED_INFO o_compiled)
        {
            int retCode;

            if (bIsCompilerDllLoaded == false)
            {
                if(Compiler.LoadCompilerDll() == false)
                    return 1; //missing library
            }

            unsafe
            {
                COMPILED_INFO info = new COMPILED_INFO();
                retCode = compile(i_compileOption, i_sourceOrFileName, &info);

                o_compiled = info;
            }
            return retCode;
        }

        public static int GetVersion(out double i_version)
        {
            i_version = 0.0;
            if (bIsCompilerDllLoaded == false)
            {
                if (Compiler.LoadCompilerDll() == false)
                    return 1; //missing library
            }

            unsafe
            {
                COMPILED_INFO info = new COMPILED_INFO();
                int retCode = compile("--version", "", &info);
                if (retCode != 0)
                    return 1;

                string version = GetStringFromMemory(info.czCompiled+1);
                Double.TryParse(version, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out i_version);
                if (i_version > 0)
                    return 0;
                else
                    return 1;
            }
        }

        private static bool LoadCompilerDll()
        {
            if (pDllModule == IntPtr.Zero)
            {
                pDllModule = LoadLibrary(Path.Combine(Utils.GetAppFolder(), "Pasmo2.dll"));
            }
            if (pDllModule == IntPtr.Zero)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("Cannot load Pasmo2.dll...\n\nTrying to download it again. Press OK please.");

                File.Delete(Path.Combine(Utils.GetAppFolder(), "Pasmo2.dll"));

                TcpHelper client = new TcpHelper();
                client.Show();
            }
            else
            {
                bIsCompilerDllLoaded = true;

                unsafe
                {
                    //ToDo: check dll version
                    string dllVersion = String.Empty;
                    COMPILED_INFO temp = new COMPILED_INFO();

                    int retCode = compile("--version", "", &temp);
                    if (retCode != 0 || new IntPtr(temp.czCompiled) == IntPtr.Zero)
                    {
                        Locator.Resolve<IUserMessage>().Error("Pasmo2.dll has incorrect version...\n\nPlease delete the Pasmo2.dll file when application is closed.");

                        if (FreeLibrary(pDllModule))
                        {
                            bIsCompilerDllLoaded = false;
                            pDllModule = IntPtr.Zero;

                            //string user = System.IO.File.GetAccessControl("Pasmo2.dll").GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();

                            //File.Delete(Path.Combine(Utils.GetAppFolder(), "Pasmo2.dll"));
                        }

                        //TcpHelper client = new TcpHelper();
                        //client.Show();

                        return false;
                    }
                    else
                    {
                        dllVersion = GetStringFromMemory(temp.czCompiled);
                    }
                }
                return true; //OK
            }

            return false;
        }

        public static Dictionary<string, ushort> ParseSymbols(string i_symbols)
        {
            if (i_symbols == null || i_symbols == String.Empty)
                return null;

            Dictionary<string, ushort> out_ParsedSymbols = new Dictionary<string, ushort>();
            string[] lines = i_symbols.Split('\n');
            foreach (string line in lines)
            {
                string[] lineParsed = Regex.Split(line, @"\s+");
                if (lineParsed.Length == 3)
                {
                    ushort symbolAddr = ConvertRadix.ConvertNumberWithPrefix(lineParsed[2]);
                    out_ParsedSymbols.Add(lineParsed[0], symbolAddr);
                }
            }

            return out_ParsedSymbols;
        }

        static unsafe public string GetStringFromMemory(byte* i_pointer)
        {
            string retString = String.Empty;
            if (new IntPtr(i_pointer) == IntPtr.Zero)
                return retString;
            for (; ; )
            {
                byte* c = (byte*)i_pointer;
                if (*c == 0)
                    break;
                i_pointer++;

                retString += (char)*c;
            }

            return retString;
        }

        #region members
            [DllImport(@"Pasmo2.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "compile")]
            private unsafe static extern int compile( [MarshalAs(UnmanagedType.LPStr)] string compileArg,   //e.g. --bin, --tap; terminated by NULL(0); generate symbol table: --<mode> <input> <output> <symbol_table_filename>
                                                      [MarshalAs(UnmanagedType.LPStr)] string inAssembler,
                                                      [In, Out] COMPILED_INFO* out_Compiled
                                                    );
            [DllImport("kernel32.dll")]
            private static extern IntPtr LoadLibrary(string fileName);
            [DllImport("kernel32.dll", SetLastError = true)]
            internal extern static bool FreeLibrary(IntPtr hModule);
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool CloseHandle(IntPtr hObject);

            private static IntPtr pDllModule = IntPtr.Zero;
            private static bool bIsCompilerDllLoaded = false;
        #endregion members
    }

    public unsafe struct COMPILED_INFO
    {
        public void ResetValues()
        {
            czCompiled = null;
            iCompiledSize = -1;
            iErrFileLine = -1; czErrFileName = czErrMessage = null;
            arrSourceSymbols = null;
        }

        //compiled
        public byte* czCompiled;
        public int iCompiledSize; // REAL compiled size, without first 2 bytes(mem address where the code will be placed)

        //error info
        public int iErrFileLine;
        public byte* czErrFileName;
        public byte* czErrMessage;

        //source code symbols
        public byte* arrSourceSymbols;
    }
}
