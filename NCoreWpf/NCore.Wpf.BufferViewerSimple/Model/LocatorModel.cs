using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.Wpf.BufferViewerSimple.Model
{
    public class LocatorModel
    {
        private int[] m_arrRectOuter;
        private int[] m_arrCenterPt;
        private bool m_bResult;
        public int[] RectOuter { get => m_arrRectOuter; set => m_arrRectOuter = value; }
        public int[] CenterPt { get => m_arrCenterPt; set => m_arrCenterPt = value; }
        public bool Result { get => m_bResult; set => m_bResult = value; }
        public void Init()
        {
            m_arrRectOuter = new int[4];
            m_arrCenterPt = new int[2];
            m_bResult = false;
        }
    }
}
