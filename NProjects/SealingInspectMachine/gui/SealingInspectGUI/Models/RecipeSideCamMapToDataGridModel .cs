using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    public class RecipeSideCamMapToDataGridModel : ModelBase
    {
        private int m_nIdx;
        private string m_sParamName;
        private string m_sSideCam1Value;
        private string m_sSideCam2Value;

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
        public string SideCam1Value
        {
            get => m_sSideCam1Value;
            set
            {
                if(SetProperty(ref m_sSideCam1Value, value)) { }
            }
        }
        public string SideCam2Value
        {
            get => m_sSideCam2Value;
            set
            {
                if(SetProperty(ref m_sSideCam2Value, value)) { }
            }
        }
    }
}
