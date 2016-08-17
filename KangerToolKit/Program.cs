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
        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping")]
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
         UInt32 dwFileOffsetHight,
         UInt32 dwFileOffsetLow,
         UInt32 dwNumberOfBytesToMap
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
                m_Write = new Semaphore(1,1,"WriteMap");
                m_Read = new Semaphore(0,1,"ReadMap");
                IntPtr hfile = new IntPtr(-1);
                handle = CreateFileMapping(hfile, 0, 0x04, 0, mapLength, "shareMemory");
                addr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);

                m_Write.WaitOne();
                byte[] sendStr = Encoding.Unicode.GetBytes("还是不会izheyt12342@#￥：：“”《》<>{}[]" + '\0');
                //如果要是超长的话，应另外处理，最好是分配足够的内存
                if (sendStr.Length < mapLength)
                    Copy(sendStr, addr);
                m_Read.Release();

                Console.WriteLine("write sccess");
                Console.ReadKey();
                

                UnmapViewOfFile(addr);
                CloseHandle(handle);
            }
            catch (WaitHandleCannotBeOpenedException)
            {

                Console.ReadKey();
            }

            Console.ReadKey();
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
