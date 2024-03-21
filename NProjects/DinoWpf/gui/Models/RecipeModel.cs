using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models
{
    public class RecipeModel : ModelBase
    {
        private int _id;
        private string _name;
        private int _cameraIdParent;
        private List<ITool> _toolList;

        public int Id { get => _id; set { SetProperty(ref _id, value); } }
        public string Name { get => _name; set { SetProperty(ref _name, value); } }
        public int CameraIdParent { get => _cameraIdParent; set { SetProperty(ref _cameraIdParent, value); } }
        public List<ITool> ToolList { get => _toolList; set { SetProperty(ref _toolList, value); } }
    }
}
