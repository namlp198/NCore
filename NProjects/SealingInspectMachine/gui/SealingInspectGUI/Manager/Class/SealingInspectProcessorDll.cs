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


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_Color(IntPtr sealingInspProcessor, int nBuff, int nY);
        public IntPtr GetBufferImage_Color(int nBuff, int nY) { return GetBufferImage_Color(m_sealingInspectProcessor, nBuff, nY); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_Mono(IntPtr sealingInspProcessor, int nBuff, int nY);
        public IntPtr GetBufferImage_Mono(int nBuff, int nY) { return GetBufferImage_Mono(m_sealingInspectProcessor, nBuff, nY); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_Color(IntPtr sealingInspProcessor, int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_Color(int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_Color(m_sealingInspectProcessor, nBuff, filePath); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_Mono(IntPtr sealingInspProcessor, int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_Mono(int nBuff, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_Mono(m_sealingInspectProcessor, nBuff, filePath); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ClearBufferImage(IntPtr sealingInspProcessor, int nBuff);
        public bool ClearBufferImage(int nBuff) { return ClearBufferImage(m_sealingInspectProcessor, nBuff); }
    }
}
