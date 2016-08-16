using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KangerToolKit
{

    class Program
    {
        [DllImport("Kernel32.dll", EntryPoint ="CreateFileMappping")]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, 
                                                       UInt32 lpAttributes,
                                                       UInt32 flProtect,
                                                       UInt32 dwMaximumSizeHigh,
                                                       UInt32 dwMaximumSizeLow,
                                                       string lpName
        );

        [DllImport("Kernel32.dll", EntryPoint = "OpenFileMapping")]
        private static extern IntPtr OpenFileMapping(
            UInt32 dwDesiredAccess,
            int bInheritHandle,
            string lpName
        );

        const int FILE_MAP_ALL_ACCESS = 0x0002;
        const int FILE_MAP_WRITE = 0x0002;

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile")]
        private static extern IntPtr MapViewOfFile(
         IntPtr hFileMappingObject,
         UInt32 dwDesiredAccess,
         UInt32 dwFileOffsetHight,//DWORD dwFileOffsetHigh,
         UInt32 dwFileOffsetLow,//DWORD dwFileOffsetLow,
         UInt32 dwNumberOfBytesToMap//SIZE_T dwNumberOfBytesToMap
         );

        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("Kernel32.dll", EntryPoint = "CloseHandle")]
        private static extern int CloseHandle(IntPtr hObject);

        private static Semaphore m_Write;  //可写的信号
        private static Semaphore m_Read;  //可读的信号
        private static IntPtr handle;     //文件句柄
        private static IntPtr addr;       //共享内存地址
        static uint mapLength = 1024;     //共享内存长


        static void Main(string[] args)
        {
            try
            {
                m_Write = Semaphore.OpenExisting("WriteMap");
                m_Read = Semaphore.OpenExisting("ReadMap");
                handle = OpenFileMapping(FILE_MAP_WRITE, 0, "shareMemory");
                addr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);

                m_Write.WaitOne();
                byte[] sendStr = Encoding.Default.GetBytes("Hello MODA" + '\0');
                //如果要是超长的话，应另外处理，最好是分配足够的内存
                if (sendStr.Length < mapLength)
                    Copy(sendStr, addr);

                m_Read.Release();


            }
            catch (WaitHandleCannotBeOpenedException)
            {
                
                return;
            }
        }

        static unsafe void Copy(byte[] byteSrc, IntPtr dst)
        {
            fixed (byte* pSrc = byteSrc)
            {
                byte* pDst = (byte*)dst;
                byte* psrc = pSrc;
                for (int i = 0; i < byteSrc.Length; i++)
                {
                    *pDst = *psrc;
                    pDst++;
                    psrc++;
                }
            }
        }
    }
}
