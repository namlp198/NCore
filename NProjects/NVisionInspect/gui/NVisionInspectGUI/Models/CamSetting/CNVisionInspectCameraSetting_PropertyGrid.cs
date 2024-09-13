using NVisionInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models
{
    [DisplayName("Cam Settings")]
    public class CNVisionInspectCameraSetting_PropertyGrid
    {
        [PropertyOrder(1)]
        [DisplayName("Save Full Image?")]
        public bool IsSaveFullImage { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Save Defect Image?")]
        public bool IsSaveDefectImage { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Show Graphics?")]
        public bool IsShowGraphics { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Channels")]
        public int Channels { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Frame Width")]
        public int FrameWidth { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Frame Height")]
        public int FrameHeight { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Frame Depth")]
        public int FrameDepth { get; set; }

        [PropertyOrder(8)]
        [DisplayName("Max Frame Count")]
        public int MaxFrameCount { get; set; }

        [PropertyOrder(9)]
        [DisplayName("Camera Name")]
        public string CameraName { get; set; }

        [PropertyOrder(10)]
        [DisplayName("Interface Type")]
        public string InterfaceType { get; set; }

        [PropertyOrder(11)]
        [DisplayName("Sensor Type")]
        public string SensorType { get; set; }

        [PropertyOrder(12)]
        [DisplayName("Manufacturer")]
        public string Manufacturer { get; set; }

        [PropertyOrder(13)]
        [DisplayName("SerialNumber")]
        public string SerialNumber { get; set; }

        [PropertyOrder(14)]
        [DisplayName("Model")]
        public string Model { get; set; }

        [PropertyOrder(15)]
        [DisplayName("FullImagePath")]
        public string FullImagePath { get; set; }

        [PropertyOrder(16)]
        [DisplayName("DefectImagePath")]
        public string DefectImagePath { get; set; }

        [PropertyOrder(17)]
        [DisplayName("TemplateImagePath")]
        public string TemplateImagePath { get; set; }

        [PropertyOrder(18)]
        [DisplayName("ROIsPath")]
        public string ROIsPath { get; set; }
    }
}
