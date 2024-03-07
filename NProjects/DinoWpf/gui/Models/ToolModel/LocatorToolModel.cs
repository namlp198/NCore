﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models.ToolModel
{
    public class LocatorToolModel : ModelBase
    {
        private int _id;
        private string _name;
        private int _priority;
        private bool _hasChildren;
        private List<SelectROIToolModel> _children;
        private int[] _rectangleInSide = new int[4];
        private int[] _rectangleOutSide = new int[4];


        public int Id { get => _id; set { SetProperty(ref _id, value); } }
        public string Name { get => _name; set { SetProperty(ref _name, value); } }
        public int Priority { get => _priority; set { SetProperty(ref _priority, value); } }
        public bool HasChildren {  get => _hasChildren; set {  SetProperty(ref _hasChildren, value); } }
        public List<SelectROIToolModel> Children { get => _children; set { SetProperty(ref _children, value); } }
        public int[] RectangleInSide { get => _rectangleInSide; set { SetProperty(ref _rectangleInSide, value); } }
        public int[] RectangleOutSide { get => _rectangleOutSide; set { SetProperty(ref _rectangleOutSide, value); } }
        public void Dispose() { }
    }
}