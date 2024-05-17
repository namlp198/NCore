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

        public const int FRAME_WIDTH = 2448;
        public const int FRAME_HEIGHT = 2048;
    }
}
