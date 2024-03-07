using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models
{
    public class JobModel : ModelBase
    {
        private int _id;
        private string _name;
        private int _numberOfCamera;
        private List<CameraModel> _cameras = new List<CameraModel>();

        public int Id { get => _id; set { SetProperty(ref _id, value); } }
        public string Name { get => _name; set { SetProperty(ref _name, value); } }
        public int NumberOfCamera { get => _numberOfCamera; set { SetProperty(ref _numberOfCamera, value); } }
        public List<CameraModel> Cameras { get => _cameras; set { SetProperty(ref _cameras, value); } }
    }
}
