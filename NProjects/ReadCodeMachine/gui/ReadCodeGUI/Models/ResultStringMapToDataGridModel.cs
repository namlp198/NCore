using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;

namespace ReadCodeGUI.Models
{
    public class ResultStringMapToDataGridModel : ModelBase
    {
        private int m_nIdx;
        private string m_sParams;
        private string m_sValue;

        public int Index
        {
            get => m_nIdx;
            set
            {
                if (SetProperty(ref m_nIdx, value)) { }
            }
        }
        public string CodeName
        {
            get => m_sParams;
            set
            {
                if (SetProperty(ref m_sParams, value)) { }
            }
        }
        public string Code
        {
            get => m_sValue;
            set
            {
                if (!SetProperty(ref m_sValue, value)) { }
            }
        }
    }
}
