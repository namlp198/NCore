using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    public class TopCamResult_MapToDataGrid_Model : ModelBase
    {
        private int m_nIndex;
        private string m_sDist_mm;
        private string m_sDistRefer_mm;
        private string m_sToleranceMin_mm;
        private string m_sToleranceMax_mm;
        private string m_sColorSet = "DarkGreen";

        public int Index
        {
            get { return m_nIndex; }
            set { SetProperty(ref m_nIndex, value); }
        }
        public string Dist_mm
        {
            get { return m_sDist_mm; }
            set
            {
                SetProperty(ref m_sDist_mm, value);
            }
        }
        public string DistRefer_mm
        {
            get => m_sDistRefer_mm;
            set { SetProperty(ref m_sDistRefer_mm, value); }
        }
        public string ToleranceMin_mm
        {
            get => m_sToleranceMin_mm;
            set { SetProperty(ref m_sToleranceMin_mm, value); }
        }
        public string ToleranceMax_mm
        {
            get => m_sToleranceMax_mm;
            set { SetProperty(ref m_sToleranceMax_mm, value); }
        }
        public string ColorSet
        {
            get => m_sColorSet;
            set { SetProperty(ref m_sColorSet, value); }
        }
    }
}
