using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;

namespace NVisionInspectGUI.Models
{
    public class RecipeList_MapToDataGrid_Model : ModelBase
    {
        private int m_nIndex;
        private string m_sRecipeName;
        private bool m_bSelectState;

        public int Index
        {
            get => m_nIndex;
            set
            {
                if (SetProperty(ref m_nIndex, value)) { }
            }
        }

        public string RecipeName
        {
            get => m_sRecipeName;
            set
            {
                if (SetProperty(ref m_sRecipeName, value)) { }
            }
        }

        public bool SelectState
        {
            get => m_bSelectState;
            set
            {
                if (SetProperty(ref m_bSelectState, value)) { }
            }
        }
    }
}
