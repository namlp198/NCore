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
        [DisplayName("Inspect Camera Count")]
        public int InspectCameraCount { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Use Simulation ?")]
        public bool Simulation { get; set; }

        [PropertyOrder(3)]
        [DisplayName("By Pass ?")]
        public bool ByPass { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Test Mode ?")]
        public bool TestMode { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Recipe Name")]
        public string RecipeName { get; set; }
    }
}
