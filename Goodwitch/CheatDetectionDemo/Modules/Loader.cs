using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using CheatDetectionDemo.CommonUtils;

namespace CheatDetectionDemo.Modules
{
    class Loader
    {
        private Process LoadedProcess;
        private IntPtr Kernel32PTR;
        private IntPtr LoadLibraryPTR;

        internal Loader(string ProcessName)
        {
            try { LoadedProcess = Process.GetProcessesByName(ProcessName).First(); }
            catch (Exception)
            {
                Console.WriteLine($"Could Not Find Process: {ProcessName}");
                Console.Read();
                Environment.Exit(0);
            }

            Kernel32PTR = NativeImport.LoadLibrary("kernel32");
            LoadLibraryPTR = NativeImport.GetProcAddress(Kernel32PTR, "LoadLibraryA");
        }

        internal void LoadAndCallMethod(string Library, string MethodName)
        {
            PerformValidationCheck(Library);

            var LoadedLibraryPTR = Load(Library);
            var LibraryToLoadCurrentProcPTR = NativeImport.LoadLibrary(Library);
            var PTRToMethod = NativeImport.GetProcAddress(LibraryToLoadCurrentProcPTR, MethodName);

            CallMethod(PTRToMethod, IntPtr.Zero);
        }

        private void PerformValidationCheck(string Library)
        {
            ValidateDotNetAssembly(Library);
            ValidateTargetPlatform(Library);
        }

        private void ValidateDotNetAssembly(string Library)
        {
            try { AssemblyName.GetAssemblyName(Library); }
            catch (Exception) { LogAndExit("Not A Valid .NET Assembly."); }
        }

        private void ValidateTargetPlatform(string Library)
        {
            if (!IsLoadedProcessX64() && !AssemblyName.GetAssemblyName(Library).ProcessorArchitecture.HasFlag(ProcessorArchitecture.X86)
             || IsLoadedProcessX64() && AssemblyName.GetAssemblyName(Library).ProcessorArchitecture.HasFlag(ProcessorArchitecture.X86))
            { LogAndExit("Platform Mismatch Between The Process And The Library."); }
        }

        private bool IsLoadedProcessX64()
        {
            if (!Environment.Is64BitOperatingSystem)
                return false;

            bool isWow64;
            if (!NativeImport.IsWow64Process(LoadedProcess.Handle, out isWow64)) isWow64 = false;

            return !isWow64;
        }

        private void LogAndExit(string LogMessage)
        {
            Console.WriteLine(LogMessage);
            Console.Read();
            Environment.Exit(0);
        }

        private IntPtr Load(string Library)
        {
            var LibraryBytes = Encoding.UTF8.GetBytes(Library);
            var PointerToAllocatedBytes = VirtualAllocEx(LibraryBytes.Length);

            WriteProcessMemory(PointerToAllocatedBytes, LibraryBytes);

            return (IntPtr)CallLoadLibraryA(PointerToAllocatedBytes);
        }

        private IntPtr VirtualAllocEx(int size)
        {
            return NativeImport.VirtualAllocEx(LoadedProcess.Handle, IntPtr.Zero, (IntPtr)size,
                Enums.AllocationType.Commit | Enums.AllocationType.Reserve,
                Enums.MemoryProtection.ExecuteReadWrite);
        }

        private bool WriteProcessMemory(IntPtr Pointer, byte[] BytesToWrite)
        {
            IntPtr nBytes;
            return NativeImport.WriteProcessMemory(LoadedProcess.Handle, Pointer, BytesToWrite,
                BytesToWrite.Length, out nBytes);
        }

        private int CallLoadLibraryA(IntPtr PointerToArg)
        {
            IntPtr hThreadId;
            var hThread = NativeImport.CreateRemoteThread(LoadedProcess.Handle, IntPtr.Zero, 0, LoadLibraryPTR, PointerToArg, 0, out hThreadId);
            NativeImport.WaitForSingleObject(hThread, unchecked((uint)-1));
            uint exitCode;
            NativeImport.GetExitCodeThread(hThread, out exitCode);
            return (int)exitCode;
        }

        private IntPtr CallMethod(IntPtr ptr, IntPtr arg)
        {
            IntPtr hThreadId;
            var hThread = NativeImport.CreateRemoteThread(LoadedProcess.Handle, IntPtr.Zero, 0,
                ptr, arg, 0, out hThreadId);
            return hThread;
        }
    }
}
