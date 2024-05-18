using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager.Class
{
    public class SealingInspectProcessorDll
    {
        IntPtr m_sealingInspectProcessor;
        public SealingInspectProcessorDll()
        {
            m_sealingInspectProcessor = CreateSealingInspectProcessor();
        }

        #region Init and delete
        /// <summary>
        /// Create a pointer the sealing inspect processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateSealingInspectProcessor();


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr sealingInspProcessor);
        public bool Initialize() { return Initialize(m_sealingInspectProcessor); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteSealingInspectProcessor(IntPtr sealingInspProcessor);
        public void DeleteSealingInspectProcessor()
        {
            DeleteSealingInspectProcessor(m_sealingInspectProcessor);
        }
        #endregion

        #region Offline Simulation
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_SIDE(IntPtr sealingInspProcessor, int nBuff, int nY);
        public IntPtr GetBufferImage_SIDE(int nBuff, int nY) { return GetBufferImage_SIDE(m_sealingInspectProcessor, nBuff, nY); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_TOP(IntPtr sealingInspProcessor, int nBuff, int nY);
        public IntPtr GetBufferImage_TOP(int nBuff, int nY) { return GetBufferImage_TOP(m_sealingInspectProcessor, nBuff, nY); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_SIDE(IntPtr sealingInspProcessor, int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_SIDE(int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_SIDE(m_sealingInspectProcessor, nBuff, filePath); }

#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_TOP(IntPtr sealingInspProcessor, int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_TOP(int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_TOP(m_sealingInspectProcessor, nBuff, filePath); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadAllImageBuffer(IntPtr sealingInspProcessor, [MarshalAs(UnmanagedType.LPStr)] string dirPath, [MarshalAs(UnmanagedType.LPStr)] string imageType);
        public bool LoadAllImageBuffer([MarshalAs(UnmanagedType.LPStr)] string dirPath, [MarshalAs(UnmanagedType.LPStr)] string imageType) { return LoadAllImageBuffer(m_sealingInspectProcessor, dirPath, imageType); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ClearBufferImage_SIDE(IntPtr sealingInspProcessor, int nBuff);
        public bool ClearBufferImage_SIDE(int nBuff) { return ClearBufferImage_SIDE(m_sealingInspectProcessor, nBuff); }

#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ClearBufferImage_TOP(IntPtr sealingInspProcessor, int nBuff);
        public bool ClearBufferImage_TOP(int nBuff) { return ClearBufferImage_TOP(m_sealingInspectProcessor, nBuff); }

        #endregion
    }
}
