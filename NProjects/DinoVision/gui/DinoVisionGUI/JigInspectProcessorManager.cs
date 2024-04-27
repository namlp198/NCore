using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    public class JigInspectProcessorManager
    {
        public JigInspectProcessorDll JigInspProcessorDll = new JigInspectProcessorDll();
        public CJigInspectResults JigInspResults = new CJigInspectResults();
        public CJigInspectSystemConfig SystemConfigs = new CJigInspectSystemConfig();
        public CJigInspectCameraConfig[] CameraConfigs = new CJigInspectCameraConfig[ConstDefine.MAX_CAMERA_INSP_COUNT];
        public CJigInspectRecipe[] RecipeConfigs = new CJigInspectRecipe[ConstDefine.MAX_CAMERA_INSP_COUNT];
        
        public void Initialize()
        {
            JigInspProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            JigInspProcessorDll.Initialize();
        }
        public void Destroy()
        {
            JigInspProcessorDll.DeleteJigInspectProcessor();
        }
    }
}
