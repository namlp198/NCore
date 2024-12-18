﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Commons
{
    public enum emInspectCavity
    {
        emUNKNOWN = 0, emInspectCavity_Cavity1, emInspectCavity_Cavity2
    }
    public enum eTriggerMode
    {
        TriggerMode_Internal = 0,
        TriggerMode_External
    }
    public enum eTriggerSource
    {
        TriggerSource_Software = 0,
        TriggerSource_Hardware
    }
    public enum eUserLevel
    {
        UserLevel_Operator,
        UserLevel_Admin,
        UserLevel_SuperAdmin
    }
    public enum eLoginStatus
    {
        LoginStatus_Success,
        LoginStatus_Failed
    }
    public enum emMachineMode { MachineMode_Auto, MachineMode_Manual}
    public class Defines
    {
        public static string StartupProgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        public const int MAX_TOPCAM_COUNT = 2;
        public const int MAX_SIDECAM_COUNT = 2;
        public const int MAX_IMAGE_BUFFER_TOPCAM = 2;
        public const int MAX_IMAGE_BUFFER_SIDECAM = 10;
        public const int FRAME_WIDTH_SIDECAM = 2448;
        public const int FRAME_HEIGHT_SIDECAM = 2048;
        public const int FRAME_WIDTH_TOPCAM = 4024;
        public const int FRAME_HEIGHT_TOPCAM = 3036;
        public const int NUMBER_OF_SET_INSPECT = 2;

        public const int MAX_STRING_SIZE = 256;
        public const int NUMBER_OF_LIGHT_CONTROLLER = 2;
        public const int ROI_PARAMETER_COUNT = 4;
        public const int MAX_PLC_COUNT = 2;
        public const int MAX_MEASURE_DIST_HOUGHCIRCLE_TOPCAM = 126;
    }
}
