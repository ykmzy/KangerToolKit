/*
    功能： 共享内存的交互通信

    2016-8-17 18:37:28
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KangerCore.BaseCore
{
    /// <summary>
    /// 共享内存
    /// </summary>
    class KTKShareMemory
    {
        KTKShareMemory()
        {
            if(!Semaphore.TryOpenExisting(WRITESEMNAME, out writeSem))
                writeSem = new Semaphore(1, 1, WRITESEMNAME);
            if (!Semaphore.TryOpenExisting(READSEMNAME, out readSem))
                readSem = new Semaphore(1, 1, READSEMNAME);

            IntPtr handle = OpenFileMapping(0x0002, 0, SHAREMEMORYNAME);
            if (handle == IntPtr.Zero)
            {
                IntPtr hfile = new IntPtr(-1);
                smHandle = CreateFileMapping(hfile, 0, 0x04, 0, smLength, SHAREMEMORYNAME);
            }
            else
            {
                smHandle = handle;
            }
            smAddr = MapViewOfFile(smHandle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
        }

        public static KTKShareMemory Instance
        {
            get
            {
                if (instance == null)
                    instance = new KTKShareMemory();
                return instance;
            }
        }
        private static KTKShareMemory instance;



        Semaphore   writeSem;           //可写的信号
        Semaphore   readSem;            //可读的信号
        IntPtr      smHandle;           //文件句柄
        IntPtr      smAddr;             //共享内存地址
        uint        smLength = 1024;    //共享内存长

        const string WRITESEMNAME = "ktkWriteSem";
        const string READSEMNAME = "ktkReadSem";
        const string SHAREMEMORYNAME = "ktkShareMemory";
        const int FILE_MAP_ALL_ACCESS = 0x0002;
        const int FILE_MAP_WRITE = 0x0002;

        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping")]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, UInt32 lpAttributes, UInt32 flProtect, UInt32 dwMaximumSizeHigh, UInt32 dwMaximumSizeLow, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "OpenFileMapping")]
        private static extern IntPtr OpenFileMapping(UInt32 dwDesiredAccess, int bInheritHandle, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile")]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, UInt32 dwDesiredAccess, UInt32 dwFileOffsetHight, UInt32 dwFileOffsetLow, UInt32 dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("Kernel32.dll", EntryPoint = "CloseHandle")]
        private static extern int CloseHandle(IntPtr hObject);
    }
}
