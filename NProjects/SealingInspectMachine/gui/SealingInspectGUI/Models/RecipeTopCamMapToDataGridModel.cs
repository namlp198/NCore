using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    public class RecipeTopCamMapToDataGridModel : ModelBase
    {
        private int m_nIdx;
        private string m_sParamName;
        private string m_sTopCam1Value;
        private string m_sTopCam2Value;

        public int Index
        {
            get => m_nIdx;
            set
            {
                if(SetProperty(ref m_nIdx, value)) { }
            }
        }
        public string ParamName
        {
            get => m_sParamName;
            set
            {
                if (SetProperty(ref m_sParamName, value)) { }
            }
        }
        public string TopCam1Value
        {
            get => m_sTopCam1Value;
            set
            {
                if(SetProperty(ref m_sTopCam1Value, value)) { }
            }
        }
        public string TopCam2Value
        {
            get => m_sTopCam2Value;
            set
            {
                if(SetProperty(ref m_sTopCam2Value, value)) { }
            }
        }
    }
}
