using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Trainer01
{
    public class Memoria
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, int lpAddress, UInt32 dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);
        static IntPtr _handle;
        static string _procNome = "";
        public static uint PROCESS_VM_READ = 0x0010;
        public static uint PROCESS_VM_WRITE = 0x0020;
        public static uint PROCESS_VM_OPERATION = 0x0008;
        public static void Iniciar(string ProcName)
        {
            _procNome = ProcName;
            if(CheckProcesso())
            {
                _handle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, Process.GetProcessesByName(ProcName)[0].Id);
            }
        }
        public static bool CheckProcesso(string pName)
        {
            if(Process.GetProcessesByName(pName).Length > 0) return true;
            else return false;
        }
        public static bool CheckProcesso()
        {
            if(Process.GetProcessesByName(_procNome).Length > 0) return true;
            else return false;
        }
        public static bool CheckModule(string pName, string pModule)
        {
            if(!CheckProcesso(pName)) return false;
            foreach(ProcessModule m in PegarModulos(pName))
            {
                if(m.ModuleName.Contains(pModule)) return true;
            }
            return false;
        }
        public static bool CheckModule(string pModule)
        {
            if(!CheckProcesso()) return false;
            foreach(ProcessModule m in PegarModulos(_procNome))
            {
                if(m.ModuleName == pModule) return true;
            }
            return false;
        }
        public static List<ProcessModule> PegarModulos(string pName)
        {
            if(!CheckProcesso(pName)) return null;
            List<ProcessModule> Lista = new List<ProcessModule>();
            foreach(ProcessModule m in Process.GetProcessesByName(pName)[0].Modules)
            {

                Lista.Add(m);
            }
            return Lista;
        }
        public static List<ProcessModule> PegarModulos()
        {
            if(!CheckProcesso()) return null;
            List<ProcessModule> Lista = new List<ProcessModule>();
            foreach(ProcessModule m in Process.GetProcessesByName(_procNome)[0].Modules)
            {

                Lista.Add(m);
            }
            return Lista;
        }
        public static int PegarBase(string pName, string pModule)
        {
            if(CheckProcesso(pName) && CheckModule(pName, pModule))
            {
                foreach(ProcessModule m in PegarModulos(pName))
                {
                    if(m.ModuleName.Contains(pModule))
                    {
                        return m.BaseAddress.ToInt32();
                    }
                }
                return 0x00;
            }
            else
            {
                return 0x00;
            }
        }
        private static byte[] LerMemoria(int Addr, int Len = 4)
        {
            byte[] buffer = new byte[Len];
            ReadProcessMemory(_handle, Addr, buffer, Len, 0);
            return buffer;
        }

        private static bool EscreverMemoria(int Addr, byte[] Value)
        {
            return WriteProcessMemory(_handle, Addr, Value, Value.Length, 0);
        }

        public static T Ler<T>(int Endereco, int Tamanho) where T : struct
        {
            byte[] buffer = LerMemoria(Endereco, Tamanho);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }

        public static void Escrever<T>(T Value, int Endereco)
        {
            Byte[] Buffer = new Byte[Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(Value, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), Buffer, 0, Buffer.Length);
            handle.Free();
            uint oldProtect;
            VirtualProtectEx(_handle, Endereco, (uint)Buffer.Length, 0x0004, out oldProtect);
            EscreverMemoria(Endereco, Buffer);
        }
    }
}
