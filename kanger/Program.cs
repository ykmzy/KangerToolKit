using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kanger
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
        uint mapLength = 1024;

        static void Main(string[] args)
        {
            m_Write = Semaphore.OpenExisting("WriteMap");
            m_Read = Semaphore.OpenExisting("ReadMap");
            handle = OpenFileMapping(0x0002, 0, "shareMemory");

            m_Read.WaitOne();
            IntPtr strPtr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
            string str = Marshal.PtrToStringAuto(strPtr);
            Console.WriteLine(str);
            Console.ReadLine();
            m_Write.Release();
        }
    }
}
