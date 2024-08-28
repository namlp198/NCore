using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Commons
{
    public enum eMachineMode { MachineMode_Auto, MachineMode_Manual }
    public class Defines
    {
        public static string StartupProgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        public static string DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "\\DbTest.db";
        public static string ReportFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "\\Report";

        //public const int FRAME_WIDTH = 1280;
        //public const int FRAME_HEIGHT = 1024;

        public const int FRAME_WIDTH = 2590;
        public const int FRAME_HEIGHT = 1942;

        public const int NUMBER_OF_SET_INSPECT = 1;
        public const int MAX_STRING_SIZE = 256;
        public const int MAX_STRING_SIZE_RESULT = 1000;
        public const int MAX_CAMERA_INSPECT_COUNT = 1;
    }
}
