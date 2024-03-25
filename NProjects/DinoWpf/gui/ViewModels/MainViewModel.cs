using DinoWpf.Commons;
using DinoWpf.Models;
using DinoWpf.Models.ParameterModel;
using DinoWpf.Models.ToolModel;
using DinoWpf.Views;
using DinoWpf.Views.Uc;
using Kis.Toolkit;
using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace DinoWpf.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region SingleTon
        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;

            GetAllJobNames();
        }
        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        private List<string> _jobs = new List<string>();
        private string _sJobSelected = string.Empty;
        private bool _isJobSelected = false;
        private double m_dOpacity = 0.2d;
        private JobModel _jobSelected = new JobModel();
        private XmlManagement _xmlManagement = new XmlManagement();
        private int _cameraIdSelected = -1;
        #endregion

        public MainView MainView { get { return _mainView; } private set { } }

        #region Properties
        public List<string> JobList
        {
            get => _jobs;
            set
            {
                if (SetProperty(ref _jobs, value))
                {

                }
            }
        }
        public string JobSelectedItem
        {
            get => _sJobSelected;
            set
            {
                if (SetProperty(ref _sJobSelected, value))
                {
                    IsJobSelected = true;
                    LoadJob();
                }
            }
        }
        public bool IsJobSelected
        {
            get => _isJobSelected;
            set
            {
                if (SetProperty(ref _isJobSelected, value))
                {
                    if(IsJobSelected == true)
                    {
                        DOpacity = 1.0;
                    }
                }
            }
        }
        public double DOpacity
        {
            get => m_dOpacity;
            set
            {
                if (SetProperty(ref m_dOpacity, value))
                {
                }
            }
        }
        public JobModel JobSelected
        {
            get => _jobSelected;
            set
            {
                if (SetProperty(ref _jobSelected, value))
                {
                    switch (_jobSelected.NumberOfCamera)
                    {
                        case 0:
                            break;
                        case 1:
                            Uc1Viewer uc1Viewer = new Uc1Viewer();
                            MainView.contentViewer.Content = uc1Viewer;
                            uc1Viewer.ucView1.HasRecipe = JobSelected.Cameras[0].Recipe.Name != null;
                            break;
                        case 2:
                            Uc2Viewer uc2Viewer = new Uc2Viewer();
                            MainView.contentViewer.Content = uc2Viewer;
                            uc2Viewer.ucView1.HasRecipe = JobSelected.Cameras[0].Recipe.Name != null;
                            uc2Viewer.ucView2.HasRecipe = JobSelected.Cameras[1].Recipe.Name != null;
                            break;
                        case 3:
                            Uc3Viewer uc3Viewer = new Uc3Viewer();
                            MainView.contentViewer.Content = uc3Viewer;
                            uc3Viewer.ucView1.HasRecipe = JobSelected.Cameras[0].Recipe.Name != null;
                            uc3Viewer.ucView2.HasRecipe = JobSelected.Cameras[1].Recipe.Name != null;
                            uc3Viewer.ucView3.HasRecipe = JobSelected.Cameras[2].Recipe.Name != null;
                            break;
                        case 4:
                            Uc4Viewer uc4Viewer = new Uc4Viewer();
                            MainView.contentViewer.Content = uc4Viewer;
                            uc4Viewer.ucView1.HasRecipe = JobSelected.Cameras[0].Recipe.Name != null;
                            uc4Viewer.ucView2.HasRecipe = JobSelected.Cameras[1].Recipe.Name != null;
                            uc4Viewer.ucView3.HasRecipe = JobSelected.Cameras[2].Recipe.Name != null;
                            uc4Viewer.ucView4.HasRecipe = JobSelected.Cameras[3].Recipe.Name != null;
                            break;
                    }
                }
            }
        }
        public XmlManagement XmlManagementVM
        { get => _xmlManagement; private set { } }
        public int CameraIdSelected
        {
            get => _cameraIdSelected;
            set
            {
                if(SetProperty(ref _cameraIdSelected, value))
                {

                }
            }
        }
        #endregion

        #region Methods
        public void GetAllJobNames()
        {
            _jobs.Clear();

            List<string> jobs = new List<string>();
            DirectoryInfo di = new DirectoryInfo(CommonDefines.JobXmlPath);
            FileInfo[] fileInfos = di.GetFiles("*.xml");
            foreach (FileInfo fileInfo in fileInfos)
            {
                string jobName = fileInfo.Name;
                jobs.Add(jobName.Substring(0, jobName.Length - 4));
            }
            JobList = jobs;
        }
        public void LoadJob()
        {
            JobModel job = new JobModel();
            List<CameraModel> cameras = new List<CameraModel>();

            string jobPathName = CommonDefines.JobXmlPath + _sJobSelected + ".xml";
            _xmlManagement.Load(jobPathName);
            XmlNode nodeJob = _xmlManagement.SelectSingleNode("//Job");
            if (nodeJob != null)
            {
                job.Id = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeJob, "id"));
                job.Name = _xmlManagement.GetAttributeValueFromNode(nodeJob, "name");
                job.NumberOfCamera = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeJob, "numberOfCamera"));
                XmlNodeList nodeListCamera = nodeJob.ChildNodes;
                if (nodeListCamera.Count > 0)
                {
                    foreach (XmlNode nodeCamera in nodeListCamera)
                    {
                        CameraModel camera = new CameraModel();
                        camera.Id = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeCamera, "id"));
                        camera.Name = _xmlManagement.GetAttributeValueFromNode(nodeCamera, "name");
                        camera.InterfaceType = _xmlManagement.GetAttributeValueFromNode(nodeCamera, "interfaceType");
                        camera.SensorType = _xmlManagement.GetAttributeValueFromNode(nodeCamera, "sensorType");
                        camera.Channels = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeCamera, "channels"));
                        camera.Manufacturer = _xmlManagement.GetAttributeValueFromNode(nodeCamera, "manufacturer");
                        camera.FrameWidth = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeCamera, "frameWidth"));
                        camera.FrameHeight = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeCamera, "frameHeight"));
                        camera.SerialNumber = _xmlManagement.GetAttributeValueFromNode(nodeCamera, "serialNumber");

                        XmlNode nodeRecipe = nodeCamera.SelectSingleNode("//Recipe");
                        if (nodeRecipe != null)
                        {
                            RecipeModel recipe = new RecipeModel();
                            List<ITool> tools = new List<ITool>();

                            recipe.Id = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRecipe, "id"));
                            recipe.Name = _xmlManagement.GetAttributeValueFromNode(nodeRecipe, "name");
                            recipe.CameraIdParent = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRecipe, "cameraIdParent"));

                            XmlNodeList nodeListTool = nodeRecipe.ChildNodes;
                            foreach (XmlNode nodeTool in nodeListTool)
                            {
                                switch (nodeTool.Name)
                                {
                                    case "LocatorTool":
                                        LocatorToolModel locator = new LocatorToolModel();
                                        locator.Id = _xmlManagement.GetAttributeValueFromNode(nodeTool, "id");
                                        locator.Name = _xmlManagement.GetAttributeValueFromNode(nodeTool, "name");
                                        locator.Priority = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeTool, "priority"));
                                        locator.HasChildren = bool.Parse(_xmlManagement.GetAttributeValueFromNode(nodeTool, "hasChildren"));
                                        locator.Children = _xmlManagement.GetAttributeValueFromNode(nodeTool, "children");

                                        XmlNodeList nodeListRect = nodeTool.ChildNodes;
                                        foreach (XmlNode nodeRect in nodeListRect)
                                        {
                                            if (string.Compare(nodeRect.Name, "RectangleInSide") == 0)
                                            {
                                                locator.RectangleInSide[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "x"));
                                                locator.RectangleInSide[1] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "y"));
                                                locator.RectangleInSide[2] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "width"));
                                                locator.RectangleInSide[3] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "height"));
                                            }
                                            else if (string.Compare(nodeRect.Name, "RectangleOutSide") == 0)
                                            {
                                                locator.RectangleOutSide[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "x"));
                                                locator.RectangleOutSide[1] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "y"));
                                                locator.RectangleOutSide[2] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "width"));
                                                locator.RectangleOutSide[3] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "height"));
                                            }
                                            else if (string.Compare(nodeRect.Name, "DataTrain") == 0)
                                            {
                                                locator.DataTrain[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "x"));
                                                locator.DataTrain[1] = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeRect, "y"));
                                                
                                            }
                                        }
                                        tools.Add(locator);
                                        break;
                                    case "SelectROITool":
                                        SelectROIToolModel selectROITool = new SelectROIToolModel();
                                        Algorithms algoEnum;

                                        selectROITool.Id = _xmlManagement.GetAttributeValueFromNode(nodeTool, "id");
                                        selectROITool.Name = _xmlManagement.GetAttributeValueFromNode(nodeTool, "name");
                                        selectROITool.Type = _xmlManagement.GetAttributeValueFromNode(nodeTool, "type");

                                        Enum.TryParse(_xmlManagement.GetAttributeValueFromNode(nodeTool, "algorithm"), out algoEnum);
                                        selectROITool.Algorithm = algoEnum;

                                        selectROITool.Rotations = bool.Parse(_xmlManagement.GetAttributeValueFromNode(nodeTool, "rotations"));
                                        selectROITool.Priority = int.Parse(_xmlManagement.GetAttributeValueFromNode(nodeTool, "priority"));

                                        XmlNode nodeParam = nodeTool.SelectSingleNode("//Parameters");
                                        if (nodeParam != null)
                                        {
                                            XmlNodeList nodeListParam = nodeParam.ChildNodes;
                                            switch (selectROITool.Algorithm)
                                            {
                                                case Algorithms.None:
                                                    break;
                                                case Algorithms.CountPixel:
                                                    ParameterCountPixelModel paramCountPixel = new ParameterCountPixelModel();
                                                    foreach (XmlNode param in nodeListParam)
                                                    {
                                                        if (string.Compare(param.Name, "ROI") == 0)
                                                        {
                                                            int x = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "x"));
                                                            int y = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "y"));
                                                            int width = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "width"));
                                                            int height = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "height"));
                                                            double angle = double.Parse(_xmlManagement.GetAttributeValueFromNode(param, "angleRotate"));
                                                            paramCountPixel.ROI = new Tuple<int, int, int, int, double> ( x, y, width, height, angle );
                                                        }
                                                        else if (string.Compare(param.Name, "ThresholdGray") == 0)
                                                        {
                                                            paramCountPixel.ThresholdGray[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "min"));
                                                            paramCountPixel.ThresholdGray[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "max"));
                                                        }
                                                        else if (string.Compare(param.Name, "NumberOfPixel") == 0)
                                                        {
                                                            paramCountPixel.NumberOfPixel[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "min"));
                                                            paramCountPixel.NumberOfPixel[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "max"));
                                                        }
                                                    }
                                                    selectROITool.Parameter = paramCountPixel;
                                                    break;
                                                case Algorithms.CalculateArea:
                                                    ParameterCalculateAreaModel paramCalculateArea = new ParameterCalculateAreaModel();
                                                    foreach (XmlNode param in nodeListParam)
                                                    {
                                                        if (string.Compare(param.Name, "ROI") == 0)
                                                        {
                                                            int x = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "x"));
                                                            int y = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "y"));
                                                            int width = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "width"));
                                                            int height = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "height"));
                                                            double angle = double.Parse(_xmlManagement.GetAttributeValueFromNode(param, "angleRotate"));
                                                            paramCalculateArea.ROI = new Tuple<int, int, int, int, double>(x, y, width, height, angle);
                                                        }
                                                        else if (string.Compare(param.Name, "Threshold") == 0)
                                                        {
                                                            paramCalculateArea.Threshold = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "value"));
                                                        }
                                                        else if (string.Compare(param.Name, "Area") == 0)
                                                        {
                                                            paramCalculateArea.Area[0] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "min"));
                                                            paramCalculateArea.Area[1] = int.Parse(_xmlManagement.GetAttributeValueFromNode(param, "max"));
                                                        }
                                                    }
                                                    selectROITool.Parameter = paramCalculateArea;
                                                    break;
                                                case Algorithms.CalculateCoordinate:
                                                    break;
                                                case Algorithms.CountBlob:
                                                    break;
                                                case Algorithms.FindLine:
                                                    break;
                                                case Algorithms.FindCircle:
                                                    break;
                                                case Algorithms.OCR:
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        tools.Add(selectROITool);
                                        break;
                                }
                            }
                            recipe.ToolList = tools;

                            camera.Recipe = recipe;
                        }
                        cameras.Add(camera);
                    }
                }
                job.Cameras = cameras;

                JobSelected = job;
            }
        }
        #endregion
    }
}
