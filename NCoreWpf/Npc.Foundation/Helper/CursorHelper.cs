using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Npc.Foundation.Helper
{
    /// <summary>
    /// Create Cursor Class
    /// [ NCS-478 : Custom Mouse Cursor ]
    /// </summary>
    public class CursorHelper
    {
        // Singleton
        public static CursorHelper Instance
        {
            get { return Nested.instance; }
        }

        // Singleton
        private class Nested
        {
            internal static readonly CursorHelper instance = new CursorHelper();
        }

        /// <summary>
        /// Cursor Icon Info Struct
        /// </summary>
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo iconInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo iconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyIcon([In] IntPtr hIcon);

        /// <summary>
        /// Create Cursor
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="xHotSpot"></param>
        /// <param name="yHotSpot"></param>
        /// <returns></returns>
        public Cursor InternalCreateCursor(System.Drawing.Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            var iconhandle = bmp.GetHicon();

            IconInfo iconInfo = new IconInfo();
            GetIconInfo(iconhandle, ref iconInfo);
            iconInfo.xHotspot = xHotSpot;
            iconInfo.yHotspot = yHotSpot;
            iconInfo.fIcon = false;

            IntPtr ptr = CreateIconIndirect(ref iconInfo);
            SafeFileHandle handle = new SafeFileHandle(ptr);

            DestroyIcon(iconhandle);

            return CursorInteropHelper.Create(handle);
        }

        /// <summary>
        /// Element 를 Image 로 만들어서 Cursor 생성
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xHotSpot"></param>
        /// <param name="yHotSpot"></param>
        /// <returns></returns>
        public Cursor CreateCursor(UIElement element, int xHotSpot, int yHotSpot)
        {
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)element.DesiredSize.Width, (int)element.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(element);
            rtb.Freeze();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms);

            ms.Close();
            ms.Dispose();

            Cursor cur = this.InternalCreateCursor(bmp, xHotSpot, yHotSpot);

            bmp.Dispose();

            return cur;
        }

    }

    public class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyIcon([In] IntPtr hIcon);

        private SafeFileHandle() : base(true) { }

        public SafeFileHandle(IntPtr hIcon) : base(true)
        {
            this.SetHandle(hIcon);
        }

        protected override bool ReleaseHandle()
        {
            return DestroyIcon(this.handle);
        }
    }
}
