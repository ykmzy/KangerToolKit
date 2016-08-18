/*
    功能： 外部调用KTK工具包的接口所在

    2016-8-17 18:37:28
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace KangerCore
{
    /// <summary>
    /// KTK工具包接口
    /// </summary>
    interface IKangerToolKit
    {
        
    }
    /// <summary>
    /// KTK工具包
    /// </summary>
    public class KangerToolKit : IDisposable, IKangerToolKit
    {
        KangerToolKit()
        {
        }
        public void Dispose()
        {
        }


    }
}
