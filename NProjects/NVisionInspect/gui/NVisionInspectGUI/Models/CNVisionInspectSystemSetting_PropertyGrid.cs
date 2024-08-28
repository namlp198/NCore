using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;
using NVisionInspectGUI.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models
{
    [DisplayName("System Settings")]
    public class CNVisionInspectSystemSetting_PropertyGrid : ModelBase
    {
        [PropertyOrder(1)]
        [DisplayName("Save Image?")]
        public bool m_bSaveFullImage {  get; set; }
        [PropertyOrder(2)]
        [DisplayName("Save Defect Image?")]
        public bool m_bSaveDefectImage { get; set; }
        [PropertyOrder(3)]
        [DisplayName("Show Detail Image?")]
        public bool m_bShowDetailImage { get; set; }
        [PropertyOrder(4)]
        [DisplayName("Use Simulation?")]
        public bool m_bSimulation { get; set; }
        [PropertyOrder(5)]
        [DisplayName("ByPass Mode?")]
        public bool m_bByPass { get; set; }
        [PropertyOrder(6)]
        [DisplayName("Full Image Path")]
        public string m_sFullImagePath { get; set; }
        [PropertyOrder(7)]
        [DisplayName("Defect Image Path")]
        public string m_sDefectImagePath { get; set; }
        [PropertyOrder(8)]
        [DisplayName("Template Image Path")]
        public string m_sTemplateImagePath { get; set; }
        [PropertyOrder(9)]
        [DisplayName("Model Name")]
        public string m_sModelName { get; set; }
        [PropertyOrder(10)]
        [DisplayName("Test Mode?")]
        public bool m_bTestMode { get; set; }
    }
}
