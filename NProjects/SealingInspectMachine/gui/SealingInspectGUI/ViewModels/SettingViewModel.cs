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
        private string m_strImageSavePath = string.Empty;
        private string[] m_arrFrameOfTOP = new string[Defines.MAX_IMAGE_BUFFER_TOPCAM] { "1", "2" };
        private string[] m_arrFrameOfSIDE = new string[Defines.MAX_IMAGE_BUFFER_SIDECAM] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_frameList = new List<string>();
        private string m_strCameraSelected = string.Empty;
        private string m_strFrameSelected = string.Empty;
        private ECameraList m_cameraSelected = new ECameraList();
        private EInspectResult m_inspectResult = new EInspectResult();
        private int m_nBuffIdx = 0;
        private int m_nFrame = 0;
        private int m_nCoreIdx = 0;

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
            _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);

            this.LoadImageCmd = new LoadImageCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SoftwareTriggerHikCamCmd = new SoftwareTriggerHikCamCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.InspectSimulationCmd = new InspectSimulationCmd();
            this.SaveImageCmd = new SaveImageCmd();

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionCavity1Complete += new InterfaceManager.InspectionCavity1Complete_Handler(InspectionCavity1Complete);
            InterfaceManager.InspectionCavity2Complete += new InterfaceManager.InspectionCavity2Complete_Handler(InspectionCavity2Complete);

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
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;

                        m_strImageSavePath = "D:\\Sealing_Folder\\Cavity1\\ImageSaved\\Cav1_TopCam1_";
                    }
                    else if (string.Compare("Side Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam1;
                        _settingView.buffVSSettings.CameraIndex = 2; // 2: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDECAM, Defines.FRAME_HEIGHT_SIDECAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;

                        m_strImageSavePath = "D:\\Sealing_Folder\\Cavity1\\ImageSaved\\Cav1_SideCam1_";
                    }
                    else if (string.Compare("Top Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam2;
                        _settingView.buffVSSettings.CameraIndex = 1; // 1: Index of Top Cam 2
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;

                        m_strImageSavePath = "D:\\Sealing_Folder\\Cavity2\\ImageSaved\\Cav2_TopCam2_";
                    }
                    else if (string.Compare("Side Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam2;
                        _settingView.buffVSSettings.CameraIndex = 3; // 3: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDECAM, Defines.FRAME_HEIGHT_SIDECAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;

                        m_strImageSavePath = "D:\\Sealing_Folder\\Cavity2\\ImageSaved\\Cav2_SideCam2_";
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
                    switch(CameraSelected)
                    {
                        case ECameraList.TopCam1:
                            m_strImageSavePath = "D:\\Sealing_Folder\\Cavity1\\ImageSaved\\Cav1_TopCam1_";
                            break;
                        case ECameraList.SideCam1:
                            m_strImageSavePath = "D:\\Sealing_Folder\\Cavity1\\ImageSaved\\Cav1_SideCam1_";
                            break;
                        case ECameraList.TopCam2:
                            m_strImageSavePath = "D:\\Sealing_Folder\\Cavity2\\ImageSaved\\Cav2_TopCam2_";
                            break;
                        case ECameraList.SideCam2:
                            m_strImageSavePath = "D:\\Sealing_Folder\\Cavity2\\ImageSaved\\Cav2_SideCam2_";
                            break;
                    }

                    ImageSavePath = string.Format("{0}{1}{2}{3}", m_strImageSavePath, "Frame", m_nFrame, "_");

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
                            m_nCoreIdx = 0;
                            break;
                        case ECameraList.TopCam2:
                        case ECameraList.SideCam2:
                            m_nBuffIdx = 1;
                            m_nCoreIdx = 1;
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
        public int CoreIdx
        {
            get => m_nCoreIdx;
            set
            {
                if (!SetProperty(ref m_nCoreIdx, value))
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

        public string ImageSavePath
        {
            get => m_strImageSavePath;
            set
            {
                if (SetProperty(ref m_strImageSavePath, value))
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
                        int k1 = 0;
                        for (int i = 0; i < nPropertyCount_1; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                case 1:
                                    LoadROI_SideCam(nFrameIdx, i, ref k1, ref lstRecipeFrame1);
                                    break;
                                case 2:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 3:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 4:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 5:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Delay Time Grab Image";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nDelayTimeGrab + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nDelayTimeGrab + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 6:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Find Start/End Line X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nFindStartEndX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nFindStartEndX + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 7:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Find Start/End Line Search Range X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nFindStartEndSearchRangeX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nFindStartEndSearchRangeX + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 8:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Find Start/End Line Threshold Gray";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nFindStartEndXThresholdGray + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nFindStartEndXThresholdGray + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 9:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Threshold Canny 1 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dThresholdCanny1_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dThresholdCanny1_MakeROI + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 10:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Threshold Canny 2 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dThresholdCanny2_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dThresholdCanny2_MakeROI + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 11:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 12:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Number of Distance NG Max Count";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 13:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Use Find ROI Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 14:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Offset Y Top (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nOffetY_Top + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nOffetY_Top + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 15:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Offset Y Bottom (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nOffetY_Bottom + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nOffetY_Bottom + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 16:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Threshold Binary (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nThresholdBinaryFindROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nThresholdBinaryFindROI + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 17:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "H Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nHMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nHMin + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 18:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "H Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nHMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nHMax + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 19:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "S Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nSMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nSMin + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 20:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "S Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nSMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nSMax + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 21:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "V Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nVMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nVMin + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 22:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "V Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nVMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nVMax + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 23:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Just Judge By Min Bouding Rect";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_bJustJudgeByMinBoundingRect + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_bJustJudgeByMinBoundingRect + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 24:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Use Hardware Trigger (1: Yes, 0: No)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_bUseHardwareTrigger + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_bUseHardwareTrigger + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 25:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Ratio Pixel Um";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dRatioPxlUm + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dRatioPxlUm + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                                case 26:
                                    recipe.Index = i + k1 - 1;
                                    recipe.ParamName = "Ratio Um Pixel";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_dRatioUmPxl + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_dRatioUmPxl + "";
                                    lstRecipeFrame1.Add(recipe);
                                    break;
                            }
                        }
                        RecipeFrame1_SideCam = lstRecipeFrame1;
                        break;

                    // frame 2
                    case 2:
                        int k2 = 0;
                        for (int i = 0; i < nPropertyCount_2; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                case 1:
                                    LoadROI_SideCam(nFrameIdx, i, ref k2, ref lstRecipeFrame2);
                                    break;
                                case 2:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 3:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 4:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 5:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Delay Time Grab Image";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nDelayTimeGrab + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nDelayTimeGrab + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 6:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Find Start/End Line X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nFindStartEndX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nFindStartEndX + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 7:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Find Start/End Line Search Range X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nFindStartEndSearchRangeX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nFindStartEndSearchRangeX + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 8:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Find Start/End Line Threshold Gray";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nFindStartEndXThresholdGray + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nFindStartEndXThresholdGray + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 9:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Threshold Canny 1 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_dThresholdCanny1_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_dThresholdCanny1_MakeROI + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 10:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Threshold Canny 2 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_dThresholdCanny2_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_dThresholdCanny2_MakeROI + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 11:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_bUseAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_bUseAdvancedAlgorithms + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 12:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Number of Distance NG Max Count";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 13:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Use Find ROI Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 14:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Offset Y Top (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nOffetY_Top + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nOffetY_Top + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 15:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Offset Y Bottom (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nOffetY_Bottom + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nOffetY_Bottom + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 16:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Threshold Binary (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nThresholdBinaryFindROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nThresholdBinaryFindROI + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 17:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "H Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nHMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nHMin + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 18:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "H Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nHMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nHMax + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 19:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "S Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nSMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nSMin + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 20:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "S Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nSMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nSMax + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 21:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "V Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nVMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nVMin + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 22:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "V Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nVMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nVMax + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                                case 23:
                                    recipe.Index = i + k2 - 1;
                                    recipe.ParamName = "Just Judge By Min Bouding Rect";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_bJustJudgeByMinBoundingRect + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_bJustJudgeByMinBoundingRect + "";
                                    lstRecipeFrame2.Add(recipe);
                                    break;
                            }
                        }
                        RecipeFrame2_SideCam = lstRecipeFrame2;
                        break;

                    // frame 3
                    case 3:
                        int k3 = 0;
                        for (int i = 0; i < nPropertyCount_3; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                case 1:
                                    LoadROI_SideCam(nFrameIdx, i, ref k3, ref lstRecipeFrame3);
                                    break;
                                case 2:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 3:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_dDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_dDistanceMeasurementTolerance_Min + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 4:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_dDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_dDistanceMeasurementTolerance_Max + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 5:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Delay Time Grab Image";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nDelayTimeGrab + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nDelayTimeGrab + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 6:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Find Start/End Line X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nFindStartEndX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nFindStartEndX + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 7:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Find Start/End Line Search Range X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nFindStartEndSearchRangeX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nFindStartEndSearchRangeX + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 8:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Find Start/End Line Threshold Gray";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nFindStartEndXThresholdGray + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nFindStartEndXThresholdGray + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 9:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Threshold Canny 1 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_dThresholdCanny1_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_dThresholdCanny1_MakeROI + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 10:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Threshold Canny 2 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_dThresholdCanny2_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_dThresholdCanny2_MakeROI + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 11:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_bUseAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_bUseAdvancedAlgorithms + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 12:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Number of Distance NG Max Count";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 13:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Use Find ROI Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 14:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Offset Y Top (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nOffetY_Top + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nOffetY_Top + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 15:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Offset Y Bottom (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nOffetY_Bottom + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nOffetY_Bottom + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 16:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Threshold Binary (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nThresholdBinaryFindROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nThresholdBinaryFindROI + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 17:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "H Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nHMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nHMin + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 18:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "H Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nHMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nHMax + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 19:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "S Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nSMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nSMin + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 20:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "S Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nSMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nSMax + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 21:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "V Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nVMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nVMin + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 22:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "V Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nVMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nVMax + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                                case 23:
                                    recipe.Index = i + k3 - 1;
                                    recipe.ParamName = "Just Judge By Min Bouding Rect";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_bJustJudgeByMinBoundingRect + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_bJustJudgeByMinBoundingRect + "";
                                    lstRecipeFrame3.Add(recipe);
                                    break;
                            }
                        }
                        RecipeFrame3_SideCam = lstRecipeFrame3;
                        break;

                    // frame 4
                    case 4:
                        int k4 = 0;
                        for (int i = 0; i < nPropertyCount_4; i++)
                        {
                            RecipeSideCamMapToDataGridModel recipe = new RecipeSideCamMapToDataGridModel();
                            switch (i)
                            {
                                case 0:
                                case 1:
                                    LoadROI_SideCam(nFrameIdx, i, ref k4, ref lstRecipeFrame4);
                                    break;
                                case 2:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 3:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_dDistanceMeasurementTolerance_Min + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_dDistanceMeasurementTolerance_Min + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 4:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_dDistanceMeasurementTolerance_Max + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_dDistanceMeasurementTolerance_Max + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 5:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Delay Time Grab Image";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nDelayTimeGrab + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nDelayTimeGrab + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 6:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Find Start/End Line X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nFindStartEndX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nFindStartEndX + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 7:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Find Start/End Line Search Range X";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nFindStartEndSearchRangeX + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nFindStartEndSearchRangeX + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 8:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Find Start/End Line Threshold Gray";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nFindStartEndXThresholdGray + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nFindStartEndXThresholdGray + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 9:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Threshold Canny 1 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_dThresholdCanny1_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_dThresholdCanny1_MakeROI + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 10:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Threshold Canny 2 Make ROI";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_dThresholdCanny2_MakeROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_dThresholdCanny2_MakeROI + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 11:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_bUseAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_bUseAdvancedAlgorithms + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 12:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Number of Distance NG Max Count";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 13:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Use Find ROI Advanced Algorithms (0: No, 1: Yes)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 14:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Offset Y Top (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nOffetY_Top + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nOffetY_Top + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 15:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Offset Y Bottom (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nOffetY_Bottom + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nOffetY_Bottom + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 16:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Threshold Binary (Find ROI Advanced)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nThresholdBinaryFindROI + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nThresholdBinaryFindROI + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 17:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "H Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nHMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nHMin + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 18:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "H Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nHMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nHMax + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 19:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "S Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nSMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nSMin + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 20:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "S Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nSMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nSMax + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 21:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "V Min (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nVMin + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nVMin + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 22:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "V Max (HSV)";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nVMax + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nVMax + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                                case 23:
                                    recipe.Index = i + k4 - 1;
                                    recipe.ParamName = "Just Judge By Min Bouding Rect";
                                    recipe.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_bJustJudgeByMinBoundingRect + "";
                                    recipe.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_bJustJudgeByMinBoundingRect + "";
                                    lstRecipeFrame4.Add(recipe);
                                    break;
                            }
                        }
                        RecipeFrame4_SideCam = lstRecipeFrame4;
                        break;
                }
            }
        }
        private void LoadROI_SideCam(int nFrame, int nPropertyIdx, ref int k, ref List<RecipeSideCamMapToDataGridModel> lstSideCamMapToDataGrid)
        {
            int nSideCam1 = 0;
            int nSideCam2 = 1;

            switch (nFrame)
            {
                // Frame 1
                case 1:
                    if (nPropertyIdx == 0)
                    {
                        for (k = 0; k < Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe1 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 0:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI X Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 1:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Y Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                            m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 2:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Width Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 3:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Height Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;

                            }
                        }
                    }
                    else if (nPropertyIdx == 1)
                    {
                        for (k = Defines.ROI_PARAMETER_COUNT; k < 2 * Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe2 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 4:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI X Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 5:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Y Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 6:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Width Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 7:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Height Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame1.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;

                            }
                        }
                        break;
                    }
                    break;

                // Fram 2
                case 2:
                    if (nPropertyIdx == 0)
                    {
                        for (k = 0; k < Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe1 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 0:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI X Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 1:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Y Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                            m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 2:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Width Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 3:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Height Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;

                            }
                        }
                    }
                    else if (nPropertyIdx == 1)
                    {
                        for (k = Defines.ROI_PARAMETER_COUNT; k < 2 * Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe2 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 4:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI X Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 5:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Y Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 6:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Width Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 7:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Height Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame2.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;

                            }
                        }
                        break;
                    }
                    break;

                // Frame 3
                case 3:
                    if (nPropertyIdx == 0)
                    {
                        for (k = 0; k < Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe1 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 0:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI X Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 1:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Y Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                            m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 2:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Width Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 3:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Height Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;

                            }
                        }
                    }
                    else if (nPropertyIdx == 1)
                    {
                        for (k = Defines.ROI_PARAMETER_COUNT; k < 2 * Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe2 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 4:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI X Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 5:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Y Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 6:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Width Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 7:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Height Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame3.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;

                            }
                        }
                        break;
                    }
                    break;

                // Frame 4
                case 4:
                    if (nPropertyIdx == 0)
                    {
                        for (k = 0; k < Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe1 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 0:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI X Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 1:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Y Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                            m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 2:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Width Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;
                                case 3:
                                    recipe1.Index = nPropertyIdx + k + 1;
                                    recipe1.ParamName = "ROI Height Top";
                                    recipe1.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Top[k] + "";
                                    recipe1.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Top[k] + "";
                                    lstSideCamMapToDataGrid.Add(recipe1);
                                    break;

                            }
                        }
                    }
                    else if (nPropertyIdx == 1)
                    {
                        for (k = Defines.ROI_PARAMETER_COUNT; k < 2 * Defines.ROI_PARAMETER_COUNT; k++)
                        {
                            RecipeSideCamMapToDataGridModel recipe2 = new RecipeSideCamMapToDataGridModel();
                            switch (k)
                            {
                                case 4:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI X Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 5:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Y Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 6:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Width Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;
                                case 7:
                                    recipe2.Index = nPropertyIdx + k;
                                    recipe2.ParamName = "ROI Height Bottom";
                                    recipe2.SideCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam1].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    recipe2.SideCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                           m_sealingInspRecipe_SideCam[nSideCam2].m_recipeFrame4.m_nROI_Bottom[k - Defines.ROI_PARAMETER_COUNT] + "";
                                    lstSideCamMapToDataGrid.Add(recipe2);
                                    break;

                            }
                        }
                        break;
                    }
                    break;
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
                        recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer + "";
                        break;
                    case 4:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min + "";
                        break;
                    case 5:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max + "";
                        break;
                    case 6:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Inner Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Min + "";
                        break;
                    case 7:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Inner Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Max + "";
                        break;
                    case 8:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Outer Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Min + "";
                        break;
                    case 9:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Outer Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Max + "";
                        break;
                    case 10:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Delta Radius Outer-Inner";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDeltaRadiusOuterInner + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDeltaRadiusOuterInner + "";
                        break;
                    case 11:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Width (Horizontal)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Hor + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Hor + "";
                        break;
                    case 12:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Height (Horizontal)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Hor + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Hor + "";
                        break;
                    case 13:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Width (Vertical)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Ver + "";
                        break;
                    case 14:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Make ROI Height (Vertical)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Ver + "";
                        break;

                    case 15:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 12H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetX + "";
                        break;
                    case 16:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 12H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetY + "";
                        break;
                    case 17:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 3H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetX + "";
                        break;
                    case 18:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 3H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetY + "";
                        break;
                    case 19:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 6H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetX + "";
                        break;
                    case 20:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 6H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetY + "";
                        break;
                    case 21:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 9H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetX + "";
                        break;
                    case 22:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI 9H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetY + "";
                        break;
                    case 23:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Use Advanced Algorithms (0: No, 1: Yes)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_bUseAdvancedAlgorithms + "";
                        break;
                    case 24:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min + "";
                        break;
                    case 25:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max + "";
                        break;
                    case 26:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Increment Angle (rad)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dIncrementAngle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dIncrementAngle + "";
                        break;
                    case 27:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold canny 1 make ROI";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dThresholdCanny1_MakeROI + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dThresholdCanny1_MakeROI + "";
                        break;
                    case 28:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold canny 2 make ROI";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dThresholdCanny2_MakeROI + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dThresholdCanny2_MakeROI + "";
                        break;
                    case 29:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Delay Time Grab Image";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDelayTimeGrab + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDelayTimeGrab + "";
                        break;
                    case 30:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Number of Distance NG Max Count";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms + "";
                        break;
                    case 31:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Hough Circle Param 1";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHoughCircleParam1 + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHoughCircleParam1 + "";
                        break;
                    case 32:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Hough Circle Param 2";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHoughCircleParam2 + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHoughCircleParam2 + "";
                        break;
                    case 33:
                        recipe.Index = i + 1;
                        recipe.ParamName = "H Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHMin + "";
                        break;
                    case 34:
                        recipe.Index = i + 1;
                        recipe.ParamName = "H Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHMax + "";
                        break;
                    case 35:
                        recipe.Index = i + 1;
                        recipe.ParamName = "S Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nSMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nSMin + "";
                        break;
                    case 36:
                        recipe.Index = i + 1;
                        recipe.ParamName = "S Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nSMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nSMax + "";
                        break;
                    case 37:
                        recipe.Index = i + 1;
                        recipe.ParamName = "V Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nVMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nVMin + "";
                        break;
                    case 38:
                        recipe.Index = i + 1;
                        recipe.ParamName = "V Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nVMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nVMax + "";
                        break;
                    case 39:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Ratio Pixel_Um";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dRatioPxlUm + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dRatioPxlUm + "";
                        break;
                    case 40:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Ratio Um_Pixel";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dRatioUmPxl + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dRatioUmPxl + "";
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
                        recipe.ParamName = "ROI Width";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROIWidth + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROIWidth + "";
                        break;
                    case 1:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI Height";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROIHeight + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROIHeight + "";
                        break;
                    case 2:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI1H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI1H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI1H_OffsetX + "";
                        break;
                    case 3:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI1H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI1H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI1H_OffsetY + "";
                        break;
                    case 4:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI5H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI5H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI5H_OffsetX + "";
                        break;
                    case 5:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI5H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI5H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI5H_OffsetY + "";
                        break;
                    case 6:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI7H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI7H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI7H_OffsetX + "";
                        break;
                    case 7:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI7H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI7H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI7H_OffsetY + "";
                        break;
                    case 8:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI11H Offset X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI11H_OffsetX + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI11H_OffsetX + "";
                        break;
                    case 9:
                        recipe.Index = i + 1;
                        recipe.ParamName = "ROI11H Offset Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI11H_OffsetY + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI11H_OffsetY + "";
                        break;
                    case 10:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Refer (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer + "";
                        break;
                    case 11:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Min (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min + "";
                        break;
                    case 12:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Distance Measurement Tolerance Max (mm)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max + "";
                        break;
                    case 13:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Delay Time Grab Image";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDelayTimeGrab + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDelayTimeGrab + "";
                        break;
                    case 14:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary + "";
                        break;
                    case 15:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Find Blob Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeFindBlob_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeFindBlob_Min + "";
                        break;
                    case 16:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Find Blob Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeFindBlob_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeFindBlob_Max + "";
                        break;
                    case 17:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Find Blob White";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobWhite + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobWhite + "";
                        break;
                    case 18:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Find Blob White Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max + "";
                        break;
                    case 19:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Find Blob Black";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobBlack + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobBlack + "";
                        break;
                    case 20:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Find Blob Black Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max + "";
                        break;
                    case 21:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Blob Count Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nBlobCount_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nBlobCount_Max + "";
                        break;
                    case 22:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Blob Area Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dBlobArea_Min + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dBlobArea_Min + "";
                        break;
                    case 23:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Blob Area Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dBlobArea_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dBlobArea_Max + "";
                        break;
                    case 24:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Use Surface Inspection (0: No, 1: Yes)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nUseCheckSurface + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nUseCheckSurface + "";
                        break;
                    case 25:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Select Method Find Circle";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nSelectMethodFindCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nSelectMethodFindCircle + "";
                        break;
                    case 26:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary Find Circle";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary_FindCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary_FindCircle + "";
                        break;
                    case 27:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Find Circle Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeMin + "";
                        break;
                    case 28:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Find Circle Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeMax + "";
                        break;
                    case 29:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Min";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadiusMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadiusMin + "";
                        break;
                    case 30:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Radius Max";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadiusMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadiusMax + "";
                        break;
                    case 31:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Min Dist Radius (Hough Circle)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nMinDist_HoughCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nMinDist_HoughCircle + "";
                        break;
                    case 32:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Param 1 (Hough Circle)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nParam1_HoughCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nParam1_HoughCircle + "";
                        break;
                    case 33:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Param 2 (Hough Circle)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nParam2_HoughCircle + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nParam2_HoughCircle + "";
                        break;
                    case 34:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold 1 Canny";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshold1_Canny + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshold1_Canny + "";
                        break;
                    case 35:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold 2 Canny";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshold2_Canny + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshold2_Canny + "";
                        break;
                    case 36:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 1H X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X + "";
                        break;
                    case 37:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 1H Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y + "";
                        break;
                    case 38:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 5H X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X + "";
                        break;
                    case 39:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 5H Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y + "";
                        break;
                    case 40:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 7H X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X + "";
                        break;
                    case 41:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 7H Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y + "";
                        break;
                    case 42:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 11H X";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X + "";
                        break;
                    case 43:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset ROI Find Measure Point 11H Y";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y + "";
                        break;
                    case 44:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Width ROI Find Sealing Overflow";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nWidth_ROIFindSealingOverflow + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nWidth_ROIFindSealingOverflow + "";
                        break;
                    case 45:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Height ROI Find Sealing Overflow";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nHeight_ROIFindSealingOverflow + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nHeight_ROIFindSealingOverflow + "";
                        break;
                    case 46:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 1H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Hoz + "";
                        break;
                    case 47:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 1H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz + "";
                        break;
                    case 48:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 1H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Ver + "";
                        break;
                    case 49:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 1H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Ver + "";
                        break;
                    case 50:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 5H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Hoz + "";
                        break;
                    case 51:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 5H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz + "";
                        break;
                    case 52:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 5H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Ver + "";
                        break;
                    case 53:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 5H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Ver + "";
                        break;
                    case 54:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 7H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Hoz + "";
                        break;
                    case 55:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 7H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz + "";
                        break;
                    case 56:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 7H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Ver + "";
                        break;
                    case 57:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 7H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Ver + "";
                        break;
                    case 58:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 11H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Hoz + "";
                        break;
                    case 59:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 11H Hoz";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz + "";
                        break;
                    case 60:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset X ROI Find Sealing Overflow 11H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Ver + "";
                        break;
                    case 61:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Offset Y ROI Find Sealing Overflow 11H Ver";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Ver + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Ver + "";
                        break;
                    case 62:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary Find Sealing Overflow";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary_FindSealingOverflow + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary_FindSealingOverflow + "";
                        break;
                    case 63:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Contour Size Max Find Sealing Overflow";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSize_FindSealingOverflow_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSize_FindSealingOverflow_Max + "";
                        break;
                    case 64:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Area Contour Max Find Sealing Overflow";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dAreaContour_FindSealingOverflow_Max + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dAreaContour_FindSealingOverflow_Max + "";
                        break;
                    case 65:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Threshold Binary Measure Width";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary_MeasureWidth + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary_MeasureWidth + "";
                        break;
                    case 66:
                        recipe.Index = i + 1;
                        recipe.ParamName = "H Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nHMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nHMin + "";
                        break;
                    case 67:
                        recipe.Index = i + 1;
                        recipe.ParamName = "H Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nHMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nHMax + "";
                        break;
                    case 68:
                        recipe.Index = i + 1;
                        recipe.ParamName = "S Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nSMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nSMin + "";
                        break;
                    case 69:
                        recipe.Index = i + 1;
                        recipe.ParamName = "S Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nSMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nSMax + "";
                        break;
                    case 70:
                        recipe.Index = i + 1;
                        recipe.ParamName = "V Min (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nVMin + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nVMin + "";
                        break;
                    case 71:
                        recipe.Index = i + 1;
                        recipe.ParamName = "V Max (HSV)";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nVMax + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nVMax + "";
                        break;
                    case 72:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Ratio Pixel Um";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dRatioPxlUm + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dRatioPxlUm + "";
                        break;
                    case 73:
                        recipe.Index = i + 1;
                        recipe.ParamName = "Ratio Um Pixel";
                        recipe.TopCam1Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                              m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dRatioUmPxl + "";
                        recipe.TopCam2Value = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                             m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dRatioUmPxl + "";
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

                case 16:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelList;
                    return "Model List";

                case 17:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos1Time_Cavity1 + "";
                    return "Go to Pos 1 Time (ms)";
                case 18:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos2Time_Cavity1 + "";
                    return "Go to Pos 2 Time (ms)";
                case 19:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos3Time_Cavity1 + "";
                    return "Go to Pos 3 Time (ms)";
                case 20:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos4Time_Cavity1 + "";
                    return "Go to Pos 4 Time (ms)";
                case 21:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos5Time_Cavity1 + "";
                    return "Go to Pos 5 Time (ms)";
                case 22:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos6Time_Cavity1 + "";
                    return "Go to Pos 6 Time (ms)";
                case 23:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos7Time_Cavity1 + "";
                    return "Go to Pos 7 Time (ms)";
                case 24:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos8Time_Cavity1 + "";
                    return "Go to Pos 8 Time (ms)";
                case 25:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos9Time_Cavity1 + "";
                    return "Go to Pos 9 Time (ms)";
                case 26:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos10Time_Cavity1 + "";
                    return "Go to Pos 10 Time (ms)";

                case 27:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos1_Cavity1 + "";
                    return "Offset Time Pos 1 (ms)";
                case 28:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos2_Cavity1 + "";
                    return "Offset Time Pos 2 (ms)";
                case 29:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos3_Cavity1 + "";
                    return "Offset Time Pos 3 (ms)";
                case 30:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos4_Cavity1 + "";
                    return "Offset Time Pos 4 (ms)";
                case 31:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos5_Cavity1 + "";
                    return "Offset Time Pos 5 (ms)";
                case 32:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos6_Cavity1 + "";
                    return "Offset Time Pos 6 (ms)";
                case 33:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos7_Cavity1 + "";
                    return "Offset Time Pos 7 (ms)";
                case 34:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos8_Cavity1 + "";
                    return "Offset Time Pos 8 (ms)";
                case 35:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos9_Cavity1 + "";
                    return "Offset Time Pos 9 (ms)";
                case 36:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos10_Cavity1 + "";
                    return "Offset Time Pos 10 (ms)";

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

                    case 16:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelList = value;
                        break;

                    case 17:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos1Time_Cavity1 = int.Parse(value);
                        break;
                    case 18:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos2Time_Cavity1 = int.Parse(value);
                        break;
                    case 19:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos3Time_Cavity1 = int.Parse(value);
                        break;
                    case 20:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos4Time_Cavity1 = int.Parse(value);
                        break;
                    case 21:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos5Time_Cavity1 = int.Parse(value);
                        break;
                    case 22:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos6Time_Cavity1 = int.Parse(value);
                        break;
                    case 23:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos7Time_Cavity1 = int.Parse(value);
                        break;
                    case 24:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos8Time_Cavity1 = int.Parse(value);
                        break;
                    case 25:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos9Time_Cavity1 = int.Parse(value);
                        break;
                    case 26:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nGoToPos10Time_Cavity1 = int.Parse(value);
                        break;

                    case 27:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos1_Cavity1 = int.Parse(value);
                        break;
                    case 28:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos2_Cavity1 = int.Parse(value);
                        break;
                    case 29:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos3_Cavity1 = int.Parse(value);
                        break;
                    case 30:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos4_Cavity1 = int.Parse(value);
                        break;
                    case 31:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos5_Cavity1 = int.Parse(value);
                        break;
                    case 32:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos6_Cavity1 = int.Parse(value);
                        break;
                    case 33:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos7_Cavity1 = int.Parse(value);
                        break;
                    case 34:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos8_Cavity1 = int.Parse(value);
                        break;
                    case 35:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos9_Cavity1 = int.Parse(value);
                        break;
                    case 36:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_nOffsetTime_Pos10_Cavity1 = int.Parse(value);
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

        private void InspectionCavity1Complete(int bSetting)
        {
            if (bSetting == 0)
                return;

            int nCoreIdx = 0;
            int nStatus = 0;
            int nFrameIdx = m_nFrame - 1;

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
        private void InspectionCavity2Complete(int bSetting)
        {
            if (bSetting == 0)
                return;

            int nCoreIdx = 1;
            int nStatus = 0;
            int nFrameIdx = m_nFrame - 1;

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
        public ICommand SaveImageCmd { get; }
        #endregion
    }
}
