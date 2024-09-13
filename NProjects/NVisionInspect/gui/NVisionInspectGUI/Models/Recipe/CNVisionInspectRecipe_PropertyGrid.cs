using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models.Recipe
{
    [DisplayName("Recipe Settings")]
    public class CNVisionInspectRecipe_PropertyGrid : ModelBase
    {
        private CNVisionInspectRecipe_Cam1_PropertyGrid m_NVIRCam1_PropGrid = new CNVisionInspectRecipe_Cam1_PropertyGrid();
        private CNVisionInspectRecipe_Cam2_PropertyGrid m_NVIRCam2_PropGrid = new CNVisionInspectRecipe_Cam2_PropertyGrid();
        private CNVisionInspectRecipe_Cam3_PropertyGrid m_NVIRCam3_PropGrid = new CNVisionInspectRecipe_Cam3_PropertyGrid();
        private CNVisionInspectRecipe_Cam4_PropertyGrid m_NVIRCam4_PropGrid = new CNVisionInspectRecipe_Cam4_PropertyGrid();
        private CNVisionInspectRecipe_Cam5_PropertyGrid m_NVIRCam5_PropGrid = new CNVisionInspectRecipe_Cam5_PropertyGrid();
        private CNVisionInspectRecipe_Cam6_PropertyGrid m_NVIRCam6_PropGrid = new CNVisionInspectRecipe_Cam6_PropertyGrid();
        private CNVisionInspectRecipe_Cam7_PropertyGrid m_NVIRCam7_PropGrid = new CNVisionInspectRecipe_Cam7_PropertyGrid();
        private CNVisionInspectRecipe_Cam8_PropertyGrid m_NVIRCam8_PropGrid = new CNVisionInspectRecipe_Cam8_PropertyGrid();

        public CNVisionInspectRecipe_Cam1_PropertyGrid RecipeCam1_PropertyGrid { get => m_NVIRCam1_PropGrid; set { if (SetProperty(ref m_NVIRCam1_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam2_PropertyGrid RecipeCam2_PropertyGrid { get => m_NVIRCam2_PropGrid; set { if (SetProperty(ref m_NVIRCam2_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam3_PropertyGrid RecipeCam3_PropertyGrid { get => m_NVIRCam3_PropGrid; set { if (SetProperty(ref m_NVIRCam3_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam4_PropertyGrid RecipeCam4_PropertyGrid { get => m_NVIRCam4_PropGrid; set { if (SetProperty(ref m_NVIRCam4_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam5_PropertyGrid RecipeCam5_PropertyGrid { get => m_NVIRCam5_PropGrid; set { if (SetProperty(ref m_NVIRCam5_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam6_PropertyGrid RecipeCam6_PropertyGrid { get => m_NVIRCam6_PropGrid; set { if (SetProperty(ref m_NVIRCam6_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam7_PropertyGrid RecipeCam7_PropertyGrid { get => m_NVIRCam7_PropGrid; set { if (SetProperty(ref m_NVIRCam7_PropGrid, value)) { } } }
        public CNVisionInspectRecipe_Cam8_PropertyGrid RecipeCam8_PropertyGrid { get => m_NVIRCam8_PropGrid; set { if (SetProperty(ref m_NVIRCam8_PropGrid, value)) { } } }
    }
}
