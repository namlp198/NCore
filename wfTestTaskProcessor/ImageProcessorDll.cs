using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wfTestTaskProcessor
{
    // Inspect Resutl
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct InspectResult
    {
        public int m_nX1;
        public int m_nY1;
        public int m_nX2;
        public int m_nY2;
    }
    internal class ImageProcessorDll
    {
        IntPtr m_ImageProcessor;
        public ImageProcessorDll()
        {
            m_ImageProcessor = CreateImageProcessor();
        }

        /// <summary>
        /// Create a pointer the image processor
        /// </summary>
        /// <returns></returns>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static IntPtr CreateImageProcessor();

        /// <summary>
        /// Create the image buffer
        /// </summary>
        /// <param name="imageProcessor"></param>
        /// <returns></returns>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool Initialize(IntPtr imageProcessor);
        public bool Initialize() { return Initialize(m_ImageProcessor); }

        /// <summary>
        /// Delete pointer the image processor
        /// </summary>
        /// <param name="imageProcessor"></param>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static void DeleteImageProcessor(IntPtr imageProcessor);
        public void DeleteImageProcessor()
        {
            DeleteImageProcessor(m_ImageProcessor);
        }

        /// <summary>
        /// Get the image buffer according index of that buffer
        /// </summary>
        /// <param name="imageProcessor"></param>
        /// <param name="nBuff"></param>
        /// <param name="nY"></param>
        /// <returns></returns>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static IntPtr GetBufferImage(IntPtr imageProcessor, int nBuff, int nY);
        public IntPtr GetBufferImage(int nBuff, int nY) { return  GetBufferImage(m_ImageProcessor, nBuff, nY); }

        /// <summary>
        /// Load the image into specified buffer
        /// </summary>
        /// <param name="imageProcessor"></param>
        /// <param name="nBuff"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool LoadImageBuffer(IntPtr imageProcessor, int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer(int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer(m_ImageProcessor, nBuff, filePath); }

        /// <summary>
        /// Clear the image buffer
        /// </summary>
        /// <param name="imageProcessor"></param>
        /// <param name="nBuff"></param>
        /// <returns></returns>
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool ClearBufferImage(IntPtr imageProcessor, int nBuff);
        public bool ClearBufferImage(int nBuff) { return ClearBufferImage(m_ImageProcessor, nBuff); }

       
        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool FindLineWithHoughLine_Simul(IntPtr imageProcessor, int top, int left, int width, int height, int nBuff);
        public bool FindLineWithHoughLine_Simul(int top, int left, int width, int height, int nBuff) { return FindLineWithHoughLine_Simul(m_ImageProcessor, top, left, width, height, nBuff); }


        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static IntPtr GetHikCamBufferImage(IntPtr imageProcessor, int nCamIdx);
        public IntPtr GetHikCamBufferImage(int nCamIdx) { return GetHikCamBufferImage(m_ImageProcessor, nCamIdx); }


        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static IntPtr GetBaslerCamBufferImage(IntPtr imageProcessor, int nCamIdx);
        public IntPtr GetBaslerCamBufferImage(int nCamIdx) { return GetBaslerCamBufferImage(m_ImageProcessor, nCamIdx); }


        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool LiveBaslerCam(IntPtr imageProcessor, int nCamIdx);
        public bool LiveBaslerCam(int nCamIdx) { return LiveBaslerCam(m_ImageProcessor, nCamIdx); }


        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static IntPtr GetBaslerCamBufferImage_New(IntPtr imageProcessor, int nCamIdx);
        public IntPtr GetBaslerCamBufferImage_New(int nCamIdx) { return GetBaslerCamBufferImage_New(m_ImageProcessor, nCamIdx); }


        [DllImport("NTaskProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
        extern private static bool GetInspectData(IntPtr imageProcessor, IntPtr inspectData);
        public bool GetInspectData(ref InspectResult inspectResult)
        {
            InspectResult pResult = new InspectResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pResult));

            Marshal.StructureToPtr(pResult, pPointer, false);
            bool bRetValue = GetInspectData(m_ImageProcessor, pPointer);
            inspectResult = (InspectResult)Marshal.PtrToStructure(pPointer, typeof(InspectResult));
            return bRetValue;
        }


        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

        public static void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }
    }
}
