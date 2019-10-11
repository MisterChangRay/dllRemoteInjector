using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace RemoteDllInjector
{
    class LoadLibrary {
        internal static class WinAPI
        {
            [Flags]
            public enum ProcessAccessFlags : uint {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            [Flags]
            public enum AllocationType {
                Commit = 0x1000,
                Reserve = 0x2000,
                Decommit = 0x4000,
                Release = 0x8000,
                Reset = 0x80000,
                Physical = 0x400000,
                TopDown = 0x100000,
                WriteWatch = 0x200000,
                LargePages = 0x20000000
            }

            [Flags]
            public enum MemoryProtection {
                Execute = 0x10,
                ExecuteRead = 0x20,
                ExecuteReadWrite = 0x40,
                ExecuteWriteCopy = 0x80,
                NoAccess = 0x01,
                ReadOnly = 0x02,
                ReadWrite = 0x04,
                WriteCopy = 0x08,
                GuardModifierflag = 0x100,
                NoCacheModifierflag = 0x200,
                WriteCombineModifierflag = 0x400
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(WinAPI.ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr str, uint nSize, out UIntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags,out  IntPtr o);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hHandle);

        }


        public static bool LoadNativeLibrary(int pid, string dllpath, out string returnMsg) {
            byte[] buffer = Encoding.Unicode.GetBytes(dllpath);
            var ptr = Marshal.StringToHGlobalAnsi(dllpath);

            UIntPtr BytesWritten = UIntPtr.Zero;

            IntPtr hProcess = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, false, pid);
            if (hProcess == IntPtr.Zero) {
                returnMsg = "Process cannot be opened!";
                return false;
            }

            IntPtr pLoadLibraryA = WinAPI.GetProcAddress(WinAPI.GetModuleHandle("Kernel32.dll"), "LoadLibraryA");
            if (pLoadLibraryA == IntPtr.Zero) {
                returnMsg = "LoadLibraryA API not found!";
                WinAPI.CloseHandle(hProcess);
                return false;
            }

            IntPtr BaseAddress = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(dllpath.Length + 1), (WinAPI.AllocationType.Commit | WinAPI.AllocationType.Reserve), WinAPI.MemoryProtection.ReadWrite);
            if (BaseAddress == IntPtr.Zero) {
                returnMsg = "Memory cannot be allocated!";
                WinAPI.CloseHandle(hProcess);
                return false;
            }

            if (WinAPI.WriteProcessMemory(hProcess, BaseAddress, ptr, (uint)buffer.Length, out BytesWritten)) {
                if (BytesWritten != UIntPtr.Zero) {
                    IntPtr res  ;
                    IntPtr hRemoteThead = WinAPI.CreateRemoteThread(hProcess, IntPtr.Zero, 0, pLoadLibraryA, BaseAddress, 0, out res);
                    if (hRemoteThead != IntPtr.Zero) {
                        returnMsg = "Dll Injected successfully!";
                        WinAPI.CloseHandle(hRemoteThead);
                        WinAPI.CloseHandle(hProcess);
                        return true;
                    } else {
                        returnMsg = "Thread cannot be created!";
                        WinAPI.CloseHandle(hProcess);
                        return false;
                    }
                } else {
                    returnMsg = "No byte was written!";
                    WinAPI.CloseHandle(hProcess);
                    return false;
                }
            } else {
                returnMsg = "Memory cannot be written!";
                WinAPI.CloseHandle(hProcess);
                return false;
            }
        }
    }
}