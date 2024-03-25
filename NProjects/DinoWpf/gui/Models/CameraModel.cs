using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Models
{
    public class CameraModel : ModelBase
    {
        #region variables
        private int _id = -1;
        private string _name = string.Empty;
        private string _interfaceType = string.Empty;
        private string _sensorType = string.Empty;
        private int _channels = 1;
        private string _manufacturer = string.Empty;
        private int _frameWidth = 0;
        private int _frameHeight = 0;
        private string _serialNumber = string.Empty;
        private RecipeModel _recipe = new RecipeModel();
        #endregion
        public int Id { get => _id; set { SetProperty(ref _id, value); } }
        public string Name { get => _name; set { SetProperty(ref _name, value); } }
        public string InterfaceType { get => _interfaceType; set { SetProperty(ref _interfaceType, value); } }
        public string SensorType { get => _sensorType; set { SetProperty(ref _sensorType, value); } }
        public int Channels { get => _channels; set { SetProperty(ref _channels, value); } }
        public string Manufacturer { get => _manufacturer; set { SetProperty(ref _manufacturer, value); } }
        public int FrameWidth { get => _frameWidth; set { SetProperty(ref _frameWidth, value); } }
        public int FrameHeight { get => _frameHeight; set { SetProperty(ref _frameHeight, value); } }
        public string SerialNumber { get => _serialNumber; set { SetProperty(ref _serialNumber, value); } }
        public RecipeModel Recipe { get => _recipe; set { SetProperty(ref _recipe, value); } }
    }
}
