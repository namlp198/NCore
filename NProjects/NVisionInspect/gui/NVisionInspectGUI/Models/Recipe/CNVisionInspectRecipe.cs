using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe
    {
        public CNVisionInspectRecipe_Cam1 m_NVisionInspRecipe_Cam1;
        public CNVisionInspectRecipe_Cam2 m_NVisionInspRecipe_Cam2;
        public CNVisionInspectRecipe_Cam3 m_NVisionInspRecipe_Cam3;
        public CNVisionInspectRecipe_Cam4 m_NVisionInspRecipe_Cam4;
        public CNVisionInspectRecipe_Cam5 m_NVisionInspRecipe_Cam5;
        public CNVisionInspectRecipe_Cam6 m_NVisionInspRecipe_Cam6;
        public CNVisionInspectRecipe_Cam7 m_NVisionInspRecipe_Cam7;
        public CNVisionInspectRecipe_Cam8 m_NVisionInspRecipe_Cam8;
    }
}
