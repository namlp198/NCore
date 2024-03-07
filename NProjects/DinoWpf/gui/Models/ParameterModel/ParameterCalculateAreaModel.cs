﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models.ParameterModel
{
    public class ParameterCalculateAreaModel : ModelBase, IParameter
    {
        private int[] _roi = new int[4];
        private int _threshold;
        private int[] _area = new int[2];

        public int[] ROI { get =>  _roi; set { SetProperty(ref _roi, value); } }
        public int Threshold { get => _threshold; set { SetProperty(ref _threshold, value); } }
        public int[] Area { get => _area; set { SetProperty(ref _area, value); } }
    }
}