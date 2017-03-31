using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

class GetSysicon
{
    /// <summary>
    /// 获得指定文件图标句柄
    /// </summary>
    /// <param name="path">指定的文件名</param>
    /// <param name="fileattribute">文件属性</param>
    /// <param name="?">返回的文件信息</param>
    /// <param name="SizeFileInfo">sfinfo的比特值</param>
    /// <param name="flag">指明要返回的文件信息标识符</param>
    /// <returns>文件的图标句柄</returns>
    [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
    public static extern IntPtr SHGetFileInfo(string path, uint fileattribute, ref SHFILEINFO sfinfo
          , uint SizeFileInfo, uint flag);

    /// <summary>
    /// 图标结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]//自定义类型
    public struct SHFILEINFO
    {
        public IntPtr hIcon;//文件的图标句柄
        public IntPtr iIcon;//图标的系统索引号
        public uint aattributes;//文件的属性值
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]//MarshalAs只是如何在托管代码和非托管代码间传送数据
        public string displayname;//文件的显示名
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string typename;//文件的类型
    }

    /// <summary>
    /// 清除图标
    /// </summary>
    /// <param name="hIcon">图标句柄</param>
    /// <returns>返回非零表示成功，零表示失败</returns>
    [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
    public static extern int DestroyIcon(IntPtr hIcon);

    /// <summary>
    /// 获取指定路径的系统图标
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="imglist">Imagelist对象保存图标</param>
    /// <param name="flag">表示是否为获取文件夹图标，true为获取文件夹图标，false为获取文件图标</param>
    public static void GetSysIcon(string path, ImageList imglist, bool flag)
    {
        SHFILEINFO shfi = new SHFILEINFO();
        if (flag)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Name == "RECYCLER" || dir.Name == "RECYCLED" || dir.Name == "Recycled" ||
                dir.Name == "System Volume Information")//windows系统文件夹
            { }
            else
            {
                SHGetFileInfo(path, (uint)0x80, ref shfi,
                    (uint)Marshal.SizeOf(shfi),//Marshal.SizeOf返回对象的非托管大小
                       (uint)(0x100 | 0x400));//取得icon和typename
                imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                DestroyIcon(shfi.hIcon);
            }
        }
        else
        {
            FileInfo file = new FileInfo(path);
            string exten = Path.GetExtension(path).Substring(1, Path.GetExtension(path).Length - 1).ToLower();//获取路径文件的扩展名，并去掉.
            if (exten == "sys" || exten == "ini" || exten == "bin" || exten == "log" ||
                exten == "com" || exten == "bat" || exten == "db")
            { }
            else
            {
                SHGetFileInfo(path, (uint)0x80, ref shfi,
                    (uint)Marshal.SizeOf(shfi), (uint)(0x100 | 0x400));
                imglist.Images.Add(file.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                DestroyIcon(shfi.hIcon);
            }
        }
    }
}

