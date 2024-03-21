using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models.ParameterModel
{
    public class ParameterCountPixelModel : ModelBase, IParameter
    {
        private Tuple<int, int, int, int, double> _roi = new Tuple<int, int, int, int, double>(0, 0, 0, 0, 0.0d);
        private int[] _thresholdGray = new int[2];
        private int[] _numberOfPixel = new int[2];

        public Tuple<int, int, int, int, double> ROI { get => _roi; set { SetProperty(ref _roi, value); } }
        public int[] ThresholdGray { get => _thresholdGray; set { SetProperty(ref _thresholdGray, value); } }
        public int[] NumberOfPixel { get => _numberOfPixel; set { SetProperty(ref _numberOfPixel, value); } }
    }
}
