using NCore.Wpf.BufferViewerSimple;
using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Manager.Class;
using SealingInspectGUI.Models;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public enum ECameraList
    {
        [Description("Top Cam 1")]
        TopCam1,
        [Description("Top Cam 2")]
        TopCam2,
        [Description("Side Cam 1")]
        SideCam1,
        [Description("Side Cam 2")]
        SideCam2
    }
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView _settingView;

        private bool _bStreamming = false;
        private bool m_bUseSoftwareTrigger = false;
        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string[] m_arrFrameOfTOP = new string[Defines.MAX_IMAGE_BUFFER_TOPCAM] { "1", "2" };
        private string[] m_arrFrameOfSIDE = new string[Defines.MAX_IMAGE_BUFFER_SIDECAM] { "1", "2", "3", "4" };

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_frameList = new List<string>();
        private string m_strCameraSelected = string.Empty;
        private string m_strFrameSelected = string.Empty;
        private ECameraList m_cameraSelected = new ECameraList();
        private EInspectResult m_inspectResult = new EInspectResult();
        private int m_nBuffIdx = 0;
        private int m_nFrame = 0;

        private List<SettingsMapToDataGridModel> m_systemSettingsModels = new List<SettingsMapToDataGridModel>();
        private List<SettingsMapToDataGridModel> m_lightSettingModel1 = new List<SettingsMapToDataGridModel>();
        private List<SettingsMapToDataGridModel> m_lightSettingModel2 = new List<SettingsMapToDataGridModel>();

        // Top cam
        private List<RecipeTopCamMapToDataGridModel> m_recipeFrame1_TopCam = new List<RecipeTopCamMapToDataGridModel>();
        private List<RecipeTopCamMapToDataGridModel> m_recipeFrame2_TopCam = new List<RecipeTopCamMapToDataGridModel>();

        // Side Cam
        private List<RecipeSideCamMapToDataGridModel> m_recipeFrame1_SideCam = new List<RecipeSideCamMapToDataGridModel>();
        private List<RecipeSideCamMapToDataGridModel> m_recipeFrame2_SideCam = new List<RecipeSideCamMapToDataGridModel>();
        private List<RecipeSideCamMapToDataGridModel> m_recipeFrame3_SideCam = new List<RecipeSideCamMapToDataGridModel>();
        private List<RecipeSideCamMapToDataGridModel> m_recipeFrame4_SideCam = new List<RecipeSideCamMapToDataGridModel>();

        public CameraStreamingController m_cameraStreamingController = null;

        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            _settingView = settingView;

            CameraList = GetEnumDescriptionToListString();

            _settingView.buffVSSettings.CameraIndex = 99;

            _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Color;
            _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_HEIGHT_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

            this.LoadImageCmd = new LoadImageCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SoftwareTriggerHikCamCmd = new SoftwareTriggerHikCamCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.InspectSimulationCmd = new InspectSimulationCmd();

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);

            m_cameraStreamingController = new CameraStreamingController(_settingView.buffVSSettings.FrameWidth,
                                                                        _settingView.buffVSSettings.FrameHeight,
                                                                        _settingView.buffVSSettings,
                                                                        _settingView.buffVSSettings.ModeView);
        }

        private async void SimulationThread_UpdateUI()
        {
            if (CameraSelected == ECameraList.TopCam1 ||
                CameraSelected == ECameraList.TopCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_TOP(m_nBuffIdx, m_nFrame - 1);

            else if (CameraSelected == ECameraList.SideCam1 ||
                CameraSelected == ECameraList.SideCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_SIDE(m_nBuffIdx, m_nFrame - 1);

            await _settingView.buffVSSettings.UpdateImage();
        }
        #endregion

        #region Properties
        public UcSettingView SettingView { get { return _settingView; } }

        public List<string> CameraList
        {
            get => m_cameraLst;
            set
            {
                if (SetProperty(ref m_cameraLst, value))
                {

                }
            }
        }
        public List<string> FrameList
        {
            get => m_frameList;
            set
            {
                if (SetProperty(ref m_frameList, value))
                {

                }
            }
        }

        public string StrCameraSelected
        {
            get => m_strCameraSelected;
            set
            {
                if (SetProperty(ref m_strCameraSelected, value))
                {
                    m_cameraStreamingController.StopSingleGrab(CameraType.Hik);

                    if (string.Compare("Top Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam1;
                        _settingView.buffVSSettings.CameraIndex = 0; // 0: Index of Top Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam1;
                        _settingView.buffVSSettings.CameraIndex = 2; // 2: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }
                    else if (string.Compare("Top Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam2;
                        _settingView.buffVSSettings.CameraIndex = 1; // 1: Index of Top Cam 2
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam2;
                        _settingView.buffVSSettings.CameraIndex = 3; // 3: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }

                    UpdateParamSoftwareTrigger();
                }
            }
        }
        public string StrFrameSelected
        {
            get => m_strFrameSelected;
            set
            {
                if (SetProperty(ref m_strFrameSelected, value))
                {
                    int.TryParse(m_strFrameSelected, out m_nFrame);
                }
            }
        }
        public int Frame
        {
            get => m_nFrame;
            set
            {
                if (SetProperty(ref m_nFrame, value))
                {

                }
            }
        }
        public ECameraList CameraSelected
        {
            get => m_cameraSelected;
            set
            {
                if (SetProperty(ref m_cameraSelected, value))
                {
                    switch (m_cameraSelected)
                    {
                        case ECameraList.TopCam1:
                        case ECameraList.SideCam1:
                            m_nBuffIdx = 0;
                            break;
                        case ECameraList.TopCam2:
                        case ECameraList.SideCam2:
                            m_nBuffIdx = 1;
                            break;
                    }
                }
            }
        }
        public int BuffIdx
        {
            get => m_nBuffIdx;
            set
            {
                if (SetProperty(ref m_nBuffIdx, value))
                {

                }
            }
        }
        public EInspectResult InspectResult
        {
            get => m_inspectResult;
            set
            {
                if (!SetProperty(ref m_inspectResult, value))
                {

                }
            }
        }

        public string DisplayImagePath
        {
            get => _displayImagePath;
            set
            {
                if (SetProperty(ref _displayImagePath, value))
                {

                }
            }
        }
        public bool IsStreamming
        {
            get => _bStreamming;
            set
            {
                if (SetProperty(ref _bStreamming, value))
                {
                    if (_bStreamming)
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/btn_stop_all_50.png";
                        _settingView.cbbCameraList.IsEnabled = false;
                    }
                    else
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                        _settingView.cbbCameraList.IsEnabled = true;
                    }
                }
            }
        }
        public bool UseSoftwareTrigger
        {
            get => m_bUseSoftwareTrigger;
            set
            {
                if (SetProperty(ref m_bUseSoftwareTrigger, value))
                {
                    UpdateParamSoftwareTrigger();
                }
            }
        }
        public List<SettingsMapToDataGridModel> SystemSettingsModels
        {
            get => m_systemSettingsModels;
            set
            {
                if (SetProperty(ref m_systemSettingsModels, value))
                {

                }
            }
        }
        public List<SettingsMapToDataGridModel> LightSettingModel1
        {
            get => m_lightSettingModel1;
            set
            {
                if (SetProperty(ref m_lightSettingModel1, value))
                {

                }
            }
        }
        public List<SettingsMapToDataGridModel> LightSettingModel2
        {
            get => m_lightSettingModel2;
            set
            {
                if (SetProperty(ref m_lightSettingModel2, value))
                {

                }
            }
        }
        public List<RecipeTopCamMapToDataGridModel> RecipeFrame1_TopCam
        {
            get => m_recipeFrame1_TopCam;
            set
            {
                if (SetProperty(ref m_recipeFrame1_TopCam, value)) { }
            }
        }
        public List<RecipeTopCamMapToDataGridModel> RecipeFrame2_TopCam
        {
            get => m_recipeFrame2_TopCam;
            set
            {
                if (SetProperty(ref m_recipeFrame2_TopCam, value)) { }
            }
        }
        public List<RecipeSideCamMapToDataGridModel> RecipeFrame1_SideCam
        {
            get => m_recipeFrame1_SideCam;
            set
            {
                if (SetProperty(ref m_recipeFrame1_SideCam, value)) { }
            }
        }
        public List<RecipeSideCamMapToDataGridModel> RecipeFrame2_SideCam
        {
            get => m_recipeFrame2_SideCam;
            set
            {
                if (SetProperty(ref m_recipeFrame2_SideCam, value)) { }
            }
        }
        public List<RecipeSideCamMapToDataGridModel> RecipeFrame3_SideCam
        {
            get => m_recipeFrame3_SideCam;
            set
            {
                if (SetProperty(ref m_recipeFrame3_SideCam, value)) { }
            }
        }
        public List<RecipeSideCamMapToDataGridModel> RecipeFrame4_SideCam
        {
            get => m_recipeFrame4_SideCam;
            set
            {
                if (SetProperty(ref m_recipeFrame4_SideCam, value)) { }
            }
        }

        #endregion

        #region Methods
        private void UpdateParamSoftwareTrigger()
        {
            if (m_bUseSoftwareTrigger)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_External);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerSource.TriggerSource_Software);
            }
            else
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_Internal);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerSource.TriggerSource_Hardware);
            }
        }
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
        private List<string> GetEnumDescriptionToListString()
        {
            List<string> modeTestString = new List<string>();
            List<ECameraList> modeTests = Enum.GetValues(typeof(ECameraList))
                                           .Cast<ECameraList>()
                                           .ToList();

            foreach (var item in modeTests)
            {
                string s = GetEnumDescription(item);
                //if (s.Equals("Null"))
                //    continue;
                modeTestString.Add(s);
            }

            return modeTestString;
        }

        public void LoadSystemSettings()
        {
            List<SettingsMapToDataGridModel> sysSettingLst = new List<SettingsMapToDataGridModel>();
            int nPropertyCount = typeof(CSealingInspectSystemSetting).GetFields().Count();
            string value = string.Empty;
            // Don't care to CSealingInspectLightSetting: nPropertyCount - 1
            for (int i = 0; i < nPropertyCount - 1; i++)
            {
                SettingsMapToDataGridModel sysSetting = new SettingsMapToDataGridModel();
                sysSetting.Index = i + 1;
                sysSetting.Params = GetParamNameAndValue_SystemSetting(i, ref value);
                sysSetting.Value = value;
                sysSettingLst.Add(sysSetting);
            }
            SystemSettingsModels = sysSettingLst;

            LoadLightSettings();
        }
        private void LoadLightSettings()
        {
            int nPropertyCount = typeof(CSealingInspectLightSetting).GetFields().Count();
            string value = string.Empty;
            for (int lightSettingIdx = 0; lightSettingIdx < Defines.NUMBER_OF_LIGHT_CONTROLLER; lightSettingIdx++)
            {
                List<SettingsMapToDataGridModel> lightSettingLst = new List<SettingsMapToDataGridModel>();
                for (int i = 1; i <= nPropertyCount; i++)
                {
                    SettingsMapToDataGridModel lightSetting = new SettingsMapToDataGridModel();
                    lightSetting.Index = i;
                    lightSetting.Params = GetParamNameAndValue_LightSetting(i, ref value, lightSettingIdx);
                    lightSetting.Value = value;
                    lightSettingLst.Add(lightSetting);
                }
                if (lightSettingIdx == 0)
                    LightSettingModel1 = lightSettingLst;
                else if (lightSettingIdx == 1)
                    LightSettingModel2 = lightSettingLst;
            }
        }
        public void LoadRecipe()
        {
            LoadRecipeTopCam();
            LoadRecipeSideCam();
        }
        private void LoadRecipeTopCam()
        {
            LoadRecipeFrame1_TopCam();
            LoadRecipeFrame2_TopCam();
        }
        private void LoadRecipeSideCam()
        {
            List<RecipeSideCamMapToDataGridModel> lstRecipeFrame1 = new List<RecipeSideCamMapToDataGridModel>();
            List<RecipeSideCamMapToDataGridModel> lstRecipeFrame2 = new List<RecipeSideCamMapToDataGridModel>();
            List<RecipeSideCamMapToDataGridModel> lstRecipeFrame3 = new List<RecipeSideCamMapToDataGridModel>();
            List<RecipeSideCamMapToDataGridModel> lstRecipeFrame4 = new List<RecipeSideCamMapToDataGridModel>();

            int nPropertyCount_1 = typeof(CRecipe_SideCam_Frame1).GetFields().Count();
            int nPropertyCount_2 = typeof(CRecipe_SideCam_Frame2).GetFields().Count();
            int nPropertyCount_3 = typeof(CRecipe_SideCam_Frame3).GetFields().Count();
            int nPropertyCount_4 = typeof(CRecipe_SideCam_Frame4).GetFields().Count();
            int nSideCam1 = 0;
            int nSideCam2 = 1;

            for (int nFrameIdx = 1; nFrameIdx <= Defines.MAX_IMAGE_BUFFER_SIDECAM; nFrameIdx++)
            {
                switch (nFrameIdx)
                {
                    // frame 1
                    case 1:
                        for (int i = 0; i < nPropertyCount_1; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min + "";
                                    break;
                                case 1:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max + "";
                                    break;
                            }
                            lstRecipeFrame1.Add(recipe);
                        }
                        RecipeFrame1_SideCam = lstRecipeFrame1;
                        break;

                    // frame 2
                    case 2:
                        for (int i = 0; i < nPropertyCount_2; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min + "";
                                    break;
                                case 1:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max + "";
                                    break;
                            }
                            lstRecipeFrame2.Add(recipe);
                        }
                        RecipeFrame2_SideCam = lstRecipeFrame2;
                        break;

                    // frame 3
                    case 3:
                        for (int i = 0; i < nPropertyCount_3; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nDistanceMeasurementTolerance_Min + "";
                                    break;
                                case 1:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nDistanceMeasurementTolerance_Max + "";
                                    break;
                            }
                            lstRecipeFrame3.Add(recipe);
                        }
                        RecipeFrame3_SideCam = lstRecipeFrame3;
                        break;

                    // frame 4
                    case 4:
                        for (int i = 0; i < nPropertyCount_4; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nDistanceMeasurementTolerance_Min + "";
                                    break;
                                case 1:
                                    recipe.Index = i + 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nDistanceMeasurementTolerance_Max + "";
                                    break;
                            }
                            lstRecipeFrame4.Add(recipe);
                        }
                        RecipeFrame4_SideCam = lstRecipeFrame4;
                        break;
                }
            }
        }
        private void LoadRecipeFrame1_TopCam()
        {
            List<RecipeTopCamMapToDataGridModel> lstRecipeFrame1 = new List<RecipeTopCamMapToDataGridModel>();

            int nPropertyCount = typeof(CRecipe_TopCam_Frame1).GetFields().Count();
            int nTopCam1 = 0;
            int nTopCam2 = 1;

            for (int i = 0; i < nPropertyCount; i++)
            {
                RecipeTopCamMapToDataGridModel recipe = new RecipeTopCamMapToDataGridModel();
                switch (i)
                {
                    case 0:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary Enclosing Circle";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdBinaryMinEnclosing + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdBinaryMinEnclosing + "";
                        break;
                    case 1:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary Canny";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle + "";
                        break;
                    case 2:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Radius Difference Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDistanceRadiusDiffMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDistanceRadiusDiffMin + "";
                        break;
                    case 3:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                        break;
                    case 4:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                        break;
                    case 5:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Inner Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Min + "";
                        break;
                    case 6:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Inner Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Max + "";
                        break;
                    case 7:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Outer Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Min + "";
                        break;
                    case 8:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Outer Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Max + "";
                        break;
                    case 9:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Delta Radius Outer-Inner";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDeltaRadiusOuterInner + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDeltaRadiusOuterInner + "";
                        break;
                    case 10:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Width (Horizontal)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Hor + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Hor + "";
                        break;
                    case 11:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Height (Horizontal)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Hor + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Hor + "";
                        break;
                    case 12:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Width (Vertical)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Ver + "";
                        break;
                    case 13:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Height (Vertical)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Ver + "";
                        break;

                    case 14:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 12H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetX + "";
                        break;
                    case 15:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 12H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetY + "";
                        break;
                    case 16:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 3H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetX + "";
                        break;
                    case 17:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 3H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetY + "";
                        break;
                    case 18:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 6H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetX + "";
                        break;
                    case 19:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 6H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetY + "";
                        break;
                    case 20:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 9H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetX + "";
                        break;
                    case 21:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 9H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetY + "";
                        break;
                    case 22:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                        break;
                    case 23:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min + "";
                        break;
                    case 24:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max + "";
                        break;
                    case 25:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Increment Angle (rad)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dIncrementAngle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dIncrementAngle + "";
                        break;
                    case 26:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold canny 1 make ROI";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdCanny1_MakeROI + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdCanny1_MakeROI + "";
                        break;
                    case 27:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold canny 2 make ROI";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdCanny2_MakeROI + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdCanny2_MakeROI + "";
                        break;
                    case 28:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Delay Time Grab Image";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDelayTimeGrab + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDelayTimeGrab + "";
                        break;
                }
                lstRecipeFrame1.Add(recipe);
            }
            RecipeFrame1_TopCam = lstRecipeFrame1;

        }
        private void LoadRecipeFrame2_TopCam()
        {
            List<RecipeTopCamMapToDataGridModel> lstRecipeFrame2 = new List<RecipeTopCamMapToDataGridModel>();

            int nPropertyCount = typeof(CRecipe_TopCam_Frame2).GetFields().Count();
            int nTopCam1 = 0;
            int nTopCam2 = 1;

            for (int i = 0; i < nPropertyCount; i++)
            {
                RecipeTopCamMapToDataGridModel recipe = new RecipeTopCamMapToDataGridModel();
                switch (i)
                {
                    case 0:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min + "";
                        break;
                    case 1:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max + "";
                        break;
                }
                lstRecipeFrame2.Add(recipe);
            }
            RecipeFrame2_TopCam = lstRecipeFrame2;

        }
        private string GetParamNameAndValue_SystemSetting(int idx, ref string value)
        {
            switch (idx)
            {
                case 0:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC1;
                    return "IP PLC1";
                case 1:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC2;
                    return "IP PLC2";
                case 2:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC1;
                    return "PORT PLC1";
                case 3:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC2;
                    return "PORT PLC2";
                case 4:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController1;
                    return "IP Light Controller 1";
                case 5:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController2;
                    return "IP Light Controller 2";
                case 6:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController1;
                    return "PORT Light Controller 1";
                case 7:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController2;
                    return "PORT Light Controller 2";
                case 8:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSaveFullImage + "";
                    return "Save Full Image (0: No, 1: Yes)";
                case 9:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSaveDefectImage + "";
                    return "Save Defect Image (0: No, 1: Yes)";

                case 10:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bShowDetailImage + "";
                    return "Show Detail Image (0: No, 1: Yes)";

                case 11:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSimulation + "";
                    return "Simulation (0: No, 1: Yes)";

                case 12:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bByPass + "";
                    return "By Pass (0: No, 1: Yes)";

                case 13:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sFullImagePath;
                    return "Full Image Path";

                case 14:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sDefectImagePath;
                    return "Defect Image Path";

                case 15:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelName;
                    return "Model Name";
            }
            return "";
        }
        private string GetParamNameAndValue_LightSetting(int idx, ref string value, int lightSettingIdx)
        {
            switch (idx)
            {
                case 1:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH1;
                    return "CH1";
                case 2:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH2;
                    return "CH2";
                case 3:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH3;
                    return "CH3";
                case 4:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH4;
                    return "CH4";
                case 5:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH5;
                    return "CH5";
                case 6:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH6;
                    return "CH6";
            }
            return value;
        }
        public void SetValue_SystemSetting()
        {
            for (int i = 0; i < SystemSettingsModels.Count; i++)
            {
                string value = SystemSettingsModels[i].Value;
                switch (i)
                {
                    case 0:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC1 = value;
                        break;
                    case 1:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC2 = value;
                        break;
                    case 2:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC1 = value;
                        break;
                    case 3:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC2 = value;
                        break;
                    case 4:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController1 = value;
                        break;
                    case 5:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController2 = value;
                        break;
                    case 6:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController1 = value;
                        break;
                    case 7:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController2 = value;
                        break;
                    case 8:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSaveFullImage);
                        break;
                    case 9:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSaveDefectImage);
                        break;
                    case 10:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bShowDetailImage);
                        break;
                    case 11:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSimulation);
                        break;
                    case 12:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bByPass);
                        break;
                    case 13:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sFullImagePath = value;
                        break;
                    case 14:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sDefectImagePath = value;
                        break;
                    case 15:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelName = value;
                        break;
                }
            }
        }
        public void SetValue_LightSetting(int lightIdx)
        {
            if (lightIdx == 0)
            {
                for (int i = 0; i < LightSettingModel1.Count; i++)
                {
                    string value = LightSettingModel1[i].Value;
                    switch (i)
                    {
                        case 0:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH1 = value;
                            break;
                        case 1:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH2 = value;
                            break;
                        case 2:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH3 = value;
                            break;
                        case 3:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH4 = value;
                            break;
                        case 4:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH5 = value;
                            break;
                        case 5:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH6 = value;
                            break;
                    }
                }
            }
            else if (lightIdx == 1)
            {
                for (int i = 0; i < LightSettingModel2.Count; i++)
                {
                    string value = LightSettingModel2[i].Value;
                    switch (i)
                    {
                        case 0:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH1 = value;
                            break;
                        case 1:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH2 = value;
                            break;
                        case 2:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH3 = value;
                            break;
                        case 3:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH4 = value;
                            break;
                        case 4:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH5 = value;
                            break;
                        case 5:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH6 = value;
                            break;
                    }
                }
            }
        }

        private void InspectionComplete(emInspectCavity eInspCav, int bSetting)
        {
            if(bSetting == 0) 
                return;

            int nCoreIdx = 0;
            int nStatus = 0;
            int nFrameIdx = m_nFrame - 1;

            if (eInspCav == emInspectCavity.emInspectCavity_Cavity1)
                nCoreIdx = 0;
            else if (eInspCav == emInspectCavity.emInspectCavity_Cavity2)
                nCoreIdx = 1;

            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

            switch (m_cameraSelected)
            {
                case ECameraList.TopCam1:
                case ECameraList.TopCam2:
                    if (m_nFrame == 1)
                        nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                  m_sealingInspResult_TopCam.m_bStatusFrame1;
                    else if (m_nFrame == 2)
                        nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                  m_sealingInspResult_TopCam.m_bStatusFrame2;
                    
                    UpdateResultView(SettingView.buffVSSettings, nStatus, m_nBuffIdx, nFrameIdx, "TOP");
                    break;
                case ECameraList.SideCam1:
                case ECameraList.SideCam2:
                    switch (m_nFrame)
                    {
                        case 1:
                            nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                 m_sealingInspResult_SideCam.m_bStatusFrame1;
                            break;
                        case 2:
                            nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                 m_sealingInspResult_SideCam.m_bStatusFrame2;
                            break;
                        case 3:
                            nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                 m_sealingInspResult_SideCam.m_bStatusFrame3;
                            break;
                        case 4:
                            nStatus = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                                 m_sealingInspResult_SideCam.m_bStatusFrame4;
                            break;
                    }
                    UpdateResultView(SettingView.buffVSSettings, nStatus, m_nBuffIdx, nFrameIdx, "SIDE");
                    break;
            }
        }
        private async void UpdateResultView(BufferViewerSimple bufferSimple, int nStatus, int nBuff, int nFrame, string s)
        {
            if (string.Compare(s.ToUpper(), "TOP") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetResultBuffer_TOP(nBuff, nFrame);
            else if (string.Compare(s.ToUpper(), "SIDE") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetResultBuffer_SIDE(nBuff, nFrame);
            await bufferSimple.UpdateImage();

            if (nStatus == 1) bufferSimple.InspectResult = EInspectResult.InspectResult_OK;
            else if (nStatus == 0) bufferSimple.InspectResult = EInspectResult.InspectResult_NG;
            else bufferSimple.InspectResult = EInspectResult.InspectResult_UNKNOWN;
        }
        #endregion

        #region Command
        public ICommand LoadImageCmd { get; }
        public ICommand ContinuousGrabCmd { get; }
        public ICommand SoftwareTriggerHikCamCmd { get; }
        public ICommand SaveSettingCmd { get; }
        public ICommand SaveRecipeCmd { get; }
        public ICommand InspectSimulationCmd { get; }
        #endregion
    }
}
