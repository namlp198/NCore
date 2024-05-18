using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Commons
{
    public class Defines
    {
        public static string StartupProgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        public const int MAX_TOP_CAM = 2;
        public const int MAX_SIDE_CAM = 2;
        public const int MAX_IMAGE_BUFFER_TOP = 4;
        public const int MAX_IMAGE_BUFFER_SIDE = 8;
        public const int FRAME_WIDTH_SIDE_CAM = 2448;
        public const int FRAME_HEIGHT_SIDE_CAM = 2048;
        public const int FRAME_WIDTH_TOP_CAM = 2448;
        public const int FRAME_HEIGHT_TOP_CAM = 2048;

    }
}
