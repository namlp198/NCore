using NVisionInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models.FakeCam.Setting
{
    [DisplayName("Fake Cam Settings")]
    public class CNVisionInspect_FakeCameraSetting_PropertyGrid
    {
        [PropertyOrder(1)]
        [DisplayName("Channels")]
        public int Channels { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Frame Width")]
        public int FrameWidth { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Frame Height")]
        public int FrameHeight { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Frame Depth")]
        public int FrameDepth { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Max Frame Count")]
        public int MaxFrameCount { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Camera Name")]
        public string CameraName { get; set; }

        [PropertyOrder(7)]
        [DisplayName("FullImagePath")]
        public string FullImagePath { get; set; }

        [PropertyOrder(8)]
        [DisplayName("DefectImagePath")]
        public string DefectImagePath { get; set; }

        [PropertyOrder(9)]
        [DisplayName("TemplateImagePath")]
        public string TemplateImagePath { get; set; }

        [PropertyOrder(10)]
        [DisplayName("ROIsPath")]
        public string ROIsPath { get; set; }
    }
}
