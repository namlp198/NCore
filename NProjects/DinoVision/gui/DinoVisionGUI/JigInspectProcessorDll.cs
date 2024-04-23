using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    public class JigInspectProcessorDll
    {
        IntPtr _jigInspectProcessor;
        public JigInspectProcessorDll()
        {
            _jigInspectProcessor = CreateJigInspectProcessor();
        }


        /// <summary>
        /// Create a pointer the image processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateJigInspectProcessor();

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr tempInspProcessor);
        public bool Initialize() { return Initialize(_jigInspectProcessor); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr tempInspProcessor, int nThreadCount, int nCamIdx);
        public bool InspectStart(int nThreadCount, int nCamIdx) { return InspectStart(_jigInspectProcessor, nThreadCount, nCamIdx); }

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr tempInspProcessor, int nCamIdx);
        public bool InspectStop(int nCamIdx) { return InspectStop(_jigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SingleGrabDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool SingleGrabDinoCam(int nCamIdx) { return SingleGrabDinoCam(_jigInspectProcessor, nCamIdx); }

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool StopGrabDinoCam(int nCamIdx) { return StopGrabDinoCam(_jigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetBufferDinoCam(int nCamIdx) { return GetBufferDinoCam(_jigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteJigInspectProcessor(IntPtr tempInspProcessor);
        public void DeleteJigInspectProcessor()
        {
            DeleteJigInspectProcessor(_jigInspectProcessor);
        }
    }
}
