#define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamingMultiCam
{
    internal class StreamingMultiCamProcessorDll
    {
        IntPtr m_StreamingMultiCamProcessor;
        public StreamingMultiCamProcessorDll()
        {
            m_StreamingMultiCamProcessor = CreateStreamingMultiCamProcessor();
        }

        /// <summary>
        /// Create a pointer the image processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateStreamingMultiCamProcessor();

        /// <summary>
        /// Create the image buffer
        /// </summary>
        /// <param name="streamingProcessor"></param>
        /// <returns></returns>
#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr streamingProcessor);
        public bool Initialize() { return Initialize(m_StreamingMultiCamProcessor); }

        /// <summary>
        /// Delete pointer the image processor
        /// </summary>
        /// <param name="streamingProcessor"></param>
#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteStreamingMultiCamProcessor(IntPtr streamingProcessor);
        public void DeleteStreamingMultiCamProcessor()
        {
            DeleteStreamingMultiCamProcessor(m_StreamingMultiCamProcessor);
        }


        #region Method's Hik Camera

#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StartGrabHikCam(IntPtr streamingProcessor, int nCamIdx);
        public bool StartGrabHikCam(int nCamIdx) { return StartGrabHikCam(m_StreamingMultiCamProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabHikCam(IntPtr streamingProcessor, int nCamIdx);
        public bool StopGrabHikCam(int nCamIdx) { return StopGrabHikCam(m_StreamingMultiCamProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetHikCamBufferImage(IntPtr streamingProcessor, int nCamIdx);
        public IntPtr GetHikCamBufferImage(int nCamIdx) { return GetHikCamBufferImage(m_StreamingMultiCamProcessor, nCamIdx); }
        #endregion


        #region Method's iRayple Camera

#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StartGrabiRaypleCam(IntPtr streamingProcessor, int nCamIdx);
        public bool StartGrabiRaypleCam(int nCamIdx) { return StartGrabiRaypleCam(m_StreamingMultiCamProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabiRaypleCam(IntPtr streamingProcessor, int nCamIdx);
        public bool StopGrabiRaypleCam(int nCamIdx) { return StopGrabiRaypleCam(m_StreamingMultiCamProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NStreamingMultiVision_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NStreamingMultiVision_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetiRaypleCamBufferImage(IntPtr streamingProcessor, int nCamIdx);
        public IntPtr GetiRaypleCamBufferImage(int nCamIdx) { return GetiRaypleCamBufferImage(m_StreamingMultiCamProcessor, nCamIdx); }
        #endregion


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
