using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.Wpf.BufferViewerSimple.Model
{
    public class CountPixelModel
    {
        private int[] m_arrROICntPxl;
        private int m_nNumberOfPxl;
        private bool m_bResult;
        public int[] ROI_CountPixel {  get => m_arrROICntPxl; set => m_arrROICntPxl = value; }
        public int NumberOfPixel { get => m_nNumberOfPxl; set => m_nNumberOfPxl = value; }
        public bool Result { get => m_bResult; set => m_bResult = value; }
        public void Init()
        {
            ROI_CountPixel = new int[4];
            NumberOfPixel = 0;
            Result = false;
        }
    }
}
