using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Commons
{
    public enum emMachineMode { MachineMode_Auto, MachineMode_Manual }
    public enum emCameraBrand
    {
        CameraBrand_Hik = 0,
        CameraBrand_Basler,
        CameraBrand_Jai,
        CameraBrand_IRayple
    }
    public enum emInspectTool
    {
        InspectTool_Locator = 0,
        InspectTool_CountPixel = 1,
        InspectTool_CountBlob = 2,
        InspectTool_Calib = 3,
        InspectTool_ColorSpace = 4,
        InspectTool_FindLine = 5,
        InspectTool_FindCircle = 6,
        InspectTool_PCA = 7,
        InspectTool_TrainOCR = 8,
        InspectTool_OCR = 9,
        InspectTool_TemplateMatchingRotate = 10,
        InspectTool_Decode = 11
    }
    public enum emRole
    {
        Role_Operator,
        Role_Engineer,
        Role_Admin,
        Role_SuperAdmin
    }
    public enum emLoginStatus
    {
        LoginStatus_Success,
        LoginStatus_Failed
    }
    public class Defines
    {
        public static string StartupProgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        public static string ReportFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "\\VisionSettings\\Report";

        public const int FRAME_WIDTH = 1280;
        public const int FRAME_HEIGHT = 1024;

        //public const int FRAME_WIDTH = 2590;
        //public const int FRAME_HEIGHT = 1942;

        public const int NUMBER_OF_SET_INSPECT = 1;
        public const int MAX_STRING_SIZE = 256;
        public const int MAX_STRING_SIZE_RESULT = 1000;
        public const int MAX_CAMERA_INSPECT_COUNT = 8;
    }
}
