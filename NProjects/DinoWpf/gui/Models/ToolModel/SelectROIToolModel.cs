using DinoWpf.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DinoWpf.Models.ToolModel
{
    public class SelectROIToolModel : ModelBase, ITool
    {
        private int _id;
        private string _name;
        private string _type;
        private Algorithms _algorithm;
        private bool _rotations;
        private double _angleRotate;
        private int _priority;
        private IParameter _parameter;

        public int Id { get => _id; set { SetProperty(ref _id, value); } }
        public string Name { get => _name; set { SetProperty(ref _name, value); } }
        public string Type { get => _type; set { SetProperty(ref _type, value); } }
        public Algorithms Algorithm { get => _algorithm; set { SetProperty(ref _algorithm, value); } }
        public bool Rotations { get => _rotations; set { SetProperty(ref _rotations, value); } }
        public double AngleRotate { get => _angleRotate; set { SetProperty(ref _angleRotate, value); } }
        public int Priority { get => _priority; set { SetProperty(ref _priority, value); } }
        public IParameter Parameter { get => _parameter; set { SetProperty(ref _parameter, value); } }

        public void Dispose() { }
    }
}
