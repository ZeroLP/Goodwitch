using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Utils;

namespace Goodwitch.Memory.Hooks
{
    internal unsafe class Detour
    {
        private byte[] m_OriginalBytes;

        internal IntPtr TargetAddress { get; }
        internal IntPtr HookAddress { get; }

        internal Detour(IntPtr target, IntPtr hook)
        {
            if (Environment.Is64BitProcess)
                throw new NotSupportedException("X64 not supported, TODO");

            TargetAddress = target;
            HookAddress = hook;

            m_OriginalBytes = new byte[5];
            fixed (byte* p = m_OriginalBytes)
            {
                ProtectionSafeMemoryCopy(new IntPtr(p), target, m_OriginalBytes.Length);
            }
        }

        internal void Install()
        {
            var jmp = CreateJMP(TargetAddress, HookAddress);
            fixed (byte* p = jmp)
            {
                ProtectionSafeMemoryCopy(TargetAddress, new IntPtr(p), jmp.Length);
            }
        }

        internal void Uninstall()
        {
            fixed (byte* p = m_OriginalBytes)
            {
                ProtectionSafeMemoryCopy(TargetAddress, new IntPtr(p), m_OriginalBytes.Length);
            }
        }

        internal int CallOriginal(Delegate Original,  IntPtr args)
        {
            Uninstall();
            var ret = Original.DynamicInvoke(args);
            this.Install();
            return (int)ret;
        }

        static void ProtectionSafeMemoryCopy(IntPtr dest, IntPtr source, int count)
        {
            // UIntPtr = size_t
            var bufferSize = new UIntPtr((uint)count);
            Enums.VirtualProtectionType oldProtection, temp;

            // unprotect memory to copy buffer
            if (!NativeImport.VirtualProtect(dest, bufferSize, Enums.VirtualProtectionType.ExecuteReadWrite, out oldProtection))
                throw new Exception("Failed to unprotect memory.");

            byte* pDest = (byte*)dest;
            byte* pSrc = (byte*)source;

            // copy buffer to address
            for (int i = 0; i < count; i++)
            {
                *(pDest + i) = *(pSrc + i);
            }

            // protect back
            if (!NativeImport.VirtualProtect(dest, bufferSize, oldProtection, out temp))
                throw new Exception("Failed to protect memory.");
        }

        static byte[] CreateJMP(IntPtr from, IntPtr to)
        {
            return CreateJMP(new IntPtr(to.ToInt32() - from.ToInt32() - 5));
        }

        static byte[] CreateJMP(IntPtr relAddr)
        {
            var list = new List<byte>();
            // get bytes of function address
            var funcAddr32 = BitConverter.GetBytes(relAddr.ToInt32());

            // jmp [relative addr] (http://ref.x86asm.net/coder32.html#xE9)
            list.Add(0xE9); // jmp
            list.AddRange(funcAddr32); // func addr

            return list.ToArray();
        }
    }
}
