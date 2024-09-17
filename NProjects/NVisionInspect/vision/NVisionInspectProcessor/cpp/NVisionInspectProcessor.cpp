#include "pch.h"
#include "NVisionInspectProcessor.h"

CNVisionInspectProcessor::CNVisionInspectProcessor()
{
	m_pCallbackInsCompleteFunc = NULL;

	m_pCallbackLogFunc = NULL;

	m_pCallbackAlarmFunc = NULL;

	m_pCallbackLocatorTrainCompleteFunc = NULL;


	m_csSysSettingsPath = GetCurrentPathApp() + _T("VisionSettings\\Settings\\SystemSettings.config");

	m_pLogView = new CLogView();

#ifndef _DEBUG
	m_pLogView->Create(CLogView::IDD);
	m_pLogView->ShowWindow(SW_SHOW);
#endif // !_DEBUG
}

CNVisionInspectProcessor::~CNVisionInspectProcessor()
{
	Destroy();
}

BOOL CNVisionInspectProcessor::Initialize()
{
	// 1. Load System Setting
	if (m_pNVisionInspectSystemSetting != NULL)
		delete m_pNVisionInspectSystemSetting, m_pNVisionInspectSystemSetting = NULL;
	m_pNVisionInspectSystemSetting = new CNVisionInspectSystemSetting;
	LoadSystemSettings(m_pNVisionInspectSystemSetting);

	// 2. Camera Setting
	for (int i = 0; i < m_pNVisionInspectSystemSetting->m_nNumberOfInspectionCamera; i++) {
		if (m_pNVisionInspectCameraSetting[i] != NULL)
			delete m_pNVisionInspectCameraSetting[i], m_pNVisionInspectCameraSetting[i] = NULL;
		m_pNVisionInspectCameraSetting[i] = new CNVisionInspectCameraSetting;

		// set camera setting path
		int nCamIdx = i + 1;
		m_csCamSettingPath.Format(_T("%sVisionSettings\\Settings\\%s%d%s.%s"), GetCurrentPathApp(), _T("Camera"), nCamIdx, _T("Settings"), _T("config"));

		LoadCameraSettings(m_pNVisionInspectCameraSetting[i]);
	}

	// 3. Create Result Buffer and simulator buffer

	if (CreateResultBuffer() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}
	if (CreateSimulatorBuffer() == FALSE)
	{
		SystemMessage(_T("Create Memory Simulator Failed!"));
		return FALSE;
	}

	// 4. Recipe
	if (m_pNVisionInspectRecipe != NULL)
		delete m_pNVisionInspectRecipe, m_pNVisionInspectRecipe = NULL;
	m_pNVisionInspectRecipe = new CNVisionInspectRecipe;

	// 5. Result

	if (m_pNVisionInspectResult != NULL)
		delete m_pNVisionInspectResult, m_pNVisionInspectResult = NULL;
	m_pNVisionInspectResult = new CNVisionInspectResult;


	// 6. Status
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pNVisionInspectStatus[i] != NULL)
			delete m_pNVisionInspectStatus[i], m_pNVisionInspectStatus[i] = NULL;
		m_pNVisionInspectStatus[i] = new CNVisionInspectStatus;
	}

	// 8. Process Core
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pNVisionInspectCore[i] != NULL)
			delete m_pNVisionInspectCore[i], m_pNVisionInspectCore[i] = NULL;
		m_pNVisionInspectCore[i] = new CNVisionInspectCore(this);
	}

	// 7. Camera
	if (m_pNVisionInspectSystemSetting->m_bSimulation == FALSE)
	{
		if (m_vecCameras.at(0) > 0) // if number of Hikcam greater more 0
		{
			if (m_pNVisionInspectHikCam != NULL)
				m_pNVisionInspectHikCam->Destroy();
			m_pNVisionInspectHikCam = new CNVisionInspect_HikCam(this);
#ifndef TEST_NO_CAMERA
			m_pNVisionInspectHikCam->Initialize();
			m_pNVisionInspectHikCam->RegisterReceivedImageCallback(ReceivedImageCallback, this);
#endif
		}

		if (m_vecCameras.at(1) > 0) // if number of BaslerCam greater more 0
		{
			if (m_pNVisionInspectBaslerCam != NULL)
				m_pNVisionInspectBaslerCam->Destroy();
			m_pNVisionInspectBaslerCam = new CNVisionInspect_BaslerCam(this);
			m_pNVisionInspectBaslerCam->Initialize();
			m_pNVisionInspectBaslerCam->RegisterReceivedImageCallback(ReceivedImageCallback, this);
		}
	}

	return TRUE;
}

BOOL CNVisionInspectProcessor::Destroy()
{
	if (m_pNVisionInspectHikCam != NULL)
		delete m_pNVisionInspectHikCam, m_pNVisionInspectHikCam = NULL;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_pResultBuffer[i] != NULL)
		{
			m_pResultBuffer[i]->DeleteSharedMemory();
			delete m_pResultBuffer[i];
			m_pResultBuffer[i] = NULL;
		}

		if (m_pSimulatorBuffer[i] != NULL)
		{
			m_pSimulatorBuffer[i]->DeleteSharedMemory();
			delete m_pSimulatorBuffer[i];
			m_pSimulatorBuffer[i] = NULL;
		}
	}

	if (m_pNVisionInspectSystemSetting != NULL)
		delete m_pNVisionInspectSystemSetting, m_pNVisionInspectSystemSetting = NULL;

	if (m_pNVisionInspectRecipe != NULL)
		delete m_pNVisionInspectRecipe, m_pNVisionInspectRecipe = NULL;

	if (m_pNVisionInspectResult != NULL)
		delete m_pNVisionInspectResult, m_pNVisionInspectResult = NULL;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {

		if (m_pNVisionInspectStatus[i] != NULL)
			delete m_pNVisionInspectStatus[i], m_pNVisionInspectStatus[i] = NULL;

		if (m_pNVisionInspectCore[i] != NULL)
			delete m_pNVisionInspectCore[i], m_pNVisionInspectCore[i] = NULL;

		if (m_pNVisionInspectCameraSetting[i] != NULL)
			delete m_pNVisionInspectCameraSetting[i], m_pNVisionInspectCameraSetting[i] = NULL;
	}

	return TRUE;
}

CString CNVisionInspectProcessor::GetCurrentPathApp()
{
	TCHAR buff[MAX_PATH];
	memset(buff, 0, MAX_PATH);
	::GetModuleFileName(NULL, buff, sizeof(buff));
	CString csFolder = buff;
	csFolder = csFolder.Left(csFolder.ReverseFind(_T('\\')) + 1);

	return csFolder;
}

BOOL CNVisionInspectProcessor::InspectStart(int nThreadCount, int nCamCount)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	if (nCamCount < 1)
		return FALSE;

	for (int nCamIdx = 0; nCamIdx < nCamCount; nCamIdx++)
	{
		m_pNVisionInspectStatus[nCamIdx]->SetStreaming(FALSE);
		m_pNVisionInspectStatus[nCamIdx]->SetInspectRunning(TRUE);

		m_pNVisionInspectHikCam->SetTriggerMode(nCamIdx, 1);
		m_pNVisionInspectHikCam->SetTriggerSource(nCamIdx, 1);
		//m_pReadCodeBaslerCam->SetExposureTime(0, 35.0);

		m_pNVisionInspectHikCam->StartGrab(nCamIdx);
	}

	return TRUE;
}

BOOL CNVisionInspectProcessor::InspectStop(int nCamCount)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	if (nCamCount < 1)
		return FALSE;

	for (int nCamIdx = 0; nCamIdx < nCamCount; nCamIdx++)
	{
		m_pNVisionInspectStatus[nCamIdx]->SetStreaming(TRUE);
		m_pNVisionInspectStatus[nCamIdx]->SetInspectRunning(FALSE);

		m_pNVisionInspectHikCam->StopGrab(0);

		m_pNVisionInspectHikCam->SetTriggerMode(0, 0);
		m_pNVisionInspectHikCam->SetTriggerSource(0, 0);
		//m_pReadCodeBaslerCam->SetExposureTime(0, m_pReadCodeCameraSetting[0]->m_nExposureTime);
	}

	return TRUE;
}

BOOL CNVisionInspectProcessor::Inspect_Reality(emCameraBrand camBrand, int nCamIdx, LPBYTE pBuff)
{
	if (pBuff == NULL)
		return FALSE;

	int nCoreIdx = 0;
	m_pNVisionInspectCore[nCoreIdx]->Inspect_Reality(camBrand, nCamIdx, pBuff);

	return TRUE;
}

BOOL CNVisionInspectProcessor::Inspect_Simulator(emCameraBrand camBrand, int nCamIdx)
{
	if (nCamIdx < 0)
		return FALSE;

	int nCoreIdx = nCamIdx;
	int nFrame = 0;
	m_pNVisionInspectCore[nCoreIdx]->Inspect_Simulation(camBrand, nCamIdx, nCoreIdx, nFrame);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SetTriggerMode(int nCamIdx, int nMode)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	m_pNVisionInspectHikCam->SetTriggerMode(nCamIdx, nMode);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SetTriggerSource(int nCamIdx, int nSource)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	m_pNVisionInspectHikCam->SetTriggerSource(nCamIdx, nSource);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SetExposureTime(int nCamIdx, double dExpTime)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	m_pNVisionInspectHikCam->SetExposureTime(nCamIdx, dExpTime);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SetAnalogGain(int nCamIdx, double dGain)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	m_pNVisionInspectHikCam->SetExposureTime(nCamIdx, dGain);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SaveImage(int nCamIdx)
{
	if (m_pNVisionInspectHikCam == NULL)
		return FALSE;

	USES_CONVERSION;
	char chSavePath[1000] = {};
	sprintf_s(chSavePath, "%s", W2A(m_pNVisionInspectCameraSetting[nCamIdx]->m_sFullImagePath));
	std::string sSavePath(chSavePath);

	cv::Mat matSave(m_pNVisionInspectCameraSetting[nCamIdx]->m_nFrameHeight, m_pNVisionInspectCameraSetting[nCamIdx]->m_nFrameWidth, CV_8UC1, m_pNVisionInspectHikCam->GetBufferImage(nCamIdx));
	cv::cvtColor(matSave, matSave, cv::COLOR_GRAY2BGR);

	uint64_t ms = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
	std::string sFileName = "Image_" + std::to_string(ms) + ".png";

	sSavePath = sSavePath + sFileName;

	cv::imwrite(sSavePath, matSave);

	return TRUE;
}

void CNVisionInspectProcessor::ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nFrameIdx, emCameraBrand camBrand, LPVOID param)
{
	CNVisionInspectProcessor* pThis = (CNVisionInspectProcessor*)param;
	pThis->ReceivedImageCallbackEx(nGrabberIdx, nFrameIdx, camBrand, pBuffer);
}

void CNVisionInspectProcessor::ReceivedImageCallbackEx(int nGrabberIdx, int nFrameIdx, emCameraBrand camBrand, LPVOID pBuffer)
{
	if ((LPBYTE)pBuffer == NULL)
		return;

	int nCamIdx = nGrabberIdx;
	int nCoreIdx = nGrabberIdx;

	m_pNVisionInspectCore[nCoreIdx]->Inspect_Reality(camBrand, nCamIdx, (LPBYTE)pBuffer);
}

BOOL CNVisionInspectProcessor::LoadSystemSettings(CNVisionInspectSystemSetting* pSystemSetting)
{
	if (m_csSysSettingsPath.IsEmpty())
	{
		AfxMessageBox(_T("System setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csSysSettingsPath);
	if (m_csSysSettingsPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("System setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CNVisionInspectSystemSetting sysSettings;

	// convert path
	USES_CONVERSION;
	char chSysSettingPath[1024] = {};
	sprintf_s(chSysSettingPath, "%s", W2A(m_csSysSettingsPath));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(chSysSettingPath, error);
	if (!m_pXmlFile)
	{
		AfxMessageBox((CString)(error.c_str()));
		return FALSE;
	}

	// 3. Create xml doc
	m_pXmlDoc = ::CreateXMLFromFile(m_pXmlFile, error);
	if (!m_pXmlDoc)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		return FALSE;
	}

	// 4. Find root: Configurations
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "SystemSettings", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// START READ SYSTEM SETTING

	sysSettings.m_nNumberOfInspectionCamera = std::stoi(pRoot->first_node("NumberOfInspectionCamera")->value());
	// The Camera Brand Count used in application
	sysSettings.m_nNumberOfBrand = std::stoi(pRoot->first_node("NumberOfBrand")->value());

	CString csSimulation = (CString)pRoot->first_node("Simulation")->value();
	sysSettings.m_bSimulation = csSimulation.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csByPass = (CString)pRoot->first_node("ByPass")->value();
	sysSettings.m_bByPass = csByPass.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csTestMode = (CString)pRoot->first_node("TestMode")->value();
	sysSettings.m_bTestMode = csTestMode.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csModelName = (CString)pRoot->first_node("ModelName")->value();
	ZeroMemory(sysSettings.m_sModelName, sizeof(sysSettings.m_sModelName));
	wsprintf(sysSettings.m_sModelName, _T("%s"), (TCHAR*)(LPCTSTR)csModelName);

	CString csModelList = (CString)pRoot->first_node("ModelList")->value();
	ZeroMemory(sysSettings.m_sModelList, sizeof(sysSettings.m_sModelList));
	wsprintf(sysSettings.m_sModelList, _T("%s"), (TCHAR*)(LPCTSTR)csModelList);

	// The required order of the string must be (don't change this order):
	// 1. Hik
	// 2. Basler
	// 3. ...
	// For example format of the string: Hik(1),Basler(1),..

	CString csCameras = (CString)pRoot->first_node("Cameras")->value();
	ZeroMemory(sysSettings.m_sCameras, sizeof(sysSettings.m_sCameras));
	wsprintf(sysSettings.m_sCameras, _T("%s"), (TCHAR*)(LPCTSTR)csCameras);

	CString strPosHik = (CString)csCameras.GetAt(csCameras.FindOneOf(_T("Hik")) + 4);
	CString strPosBasler = (CString)csCameras.GetAt(csCameras.FindOneOf(_T("Basler")) + 7);
	
	m_vecCameras.resize(sysSettings.m_nNumberOfBrand);
	m_vecCameras.at(0) = (_ttoi(strPosHik)); // Pos 0: number of Hik Cam
	m_vecCameras.at(1) = (_ttoi(strPosBasler)); // Pos 1: number of Basler Cam

	// set recipe path
	m_csRecipePath.Format(_T("%sVisionSettings\\Recipe\\%s.%s"), GetCurrentPathApp(), sysSettings.m_sModelName, _T("cfg"));

	*(pSystemSetting) = sysSettings;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CNVisionInspectProcessor::LoadRecipe(int nCamCount, CNVisionInspectRecipe* pRecipe)
{
	if (m_pNVisionInspectRecipe == NULL)
		return FALSE;

	if (nCamCount < 0)
		return FALSE;

	CNVisionInspectRecipe readRecipe;

	BOOL bNoFile = FALSE;
	// Params..
	CString strParameterKey = _T("");
	CString strValue = _T("");
	double dValue = 0.0;
	int nValue = 0;

	for (int nCamIdx = 0; nCamIdx < nCamCount; nCamIdx++)
	{
		// set recipe path
		m_csRecipePath.Format(_T("%sVisionSettings\\Recipe\\%s_%s%d.%s"), GetCurrentPathApp(), m_pNVisionInspectSystemSetting->m_sModelName, _T("Cam"), (nCamIdx + 1), _T("cfg"));

		switch (nCamIdx)
		{
		case 0:
		{
			CConfig recipeFile_Cam1;
			if (recipeFile_Cam1.SetRegiConfig(NULL, NULL, (TCHAR*)(LPCTSTR)m_csRecipePath, FileMap_Mode) == FALSE)
			{
				CFile pFile;
				pFile.Open(m_csRecipePath, CFile::modeCreate);
				pFile.Close();

				bNoFile = TRUE;
			}

			recipeFile_Cam1.GetItemValue(_T("MAX_CODE_COUNT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nMaxCodeCount, 1);
			recipeFile_Cam1.GetItemValue(_T("USE_READCODE"), readRecipe.m_NVisionInspRecipe_Cam1.m_bUseReadCode, 1);
			recipeFile_Cam1.GetItemValue(_T("USE_INKJET_CHARACTERS_INSPECT"), readRecipe.m_NVisionInspRecipe_Cam1.m_bUseInkjetCharactersInspect, 1);
			recipeFile_Cam1.GetItemValue(_T("USE_ROTATE_ROI"), readRecipe.m_NVisionInspRecipe_Cam1.m_bUseRotateROI, 0);
			recipeFile_Cam1.GetItemValue(_T("NUMBER_OF_ROI"), readRecipe.m_NVisionInspRecipe_Cam1.m_nNumberOfROI, 0);

			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_OUTER_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterX, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_OUTER_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterY, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_OUTER_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Width, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_OUTER_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Height, 0);

			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_INNER_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerX, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_INNER_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerY, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_INNER_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Width, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ROI_INNER_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Height, 0);

			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_COORDINATES_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_COORDINATES_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateAngleRotate, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_MATCHING_RATE"), readRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateMatchingRate, 0);
			recipeFile_Cam1.GetItemValue(_T("TEMPLATE_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam1.m_bTemplateShowGraphics, 0);

			recipeFile_Cam1.GetItemValue(_T("ROI1_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Width, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Height, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_AngleRotate, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseOffset, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseLocator, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI1ShowGraphics, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Max, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI1_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Max, 0);

			recipeFile_Cam1.GetItemValue(_T("ROI2_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Width, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Height, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_AngleRotate, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseOffset, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseLocator, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI2ShowGraphics, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Max, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI2_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Max, 0);

			recipeFile_Cam1.GetItemValue(_T("ROI3_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Width, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Height, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_X, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_Y, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_AngleRotate, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseOffset, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseLocator, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam1.m_bROI3ShowGraphics, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Max, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Min, 0);
			recipeFile_Cam1.GetItemValue(_T("ROI3_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Max, 0);
			break;
		}
		case 1:
		{
			CConfig recipeFile_Cam2;
			if (recipeFile_Cam2.SetRegiConfig(NULL, NULL, (TCHAR*)(LPCTSTR)m_csRecipePath, FileMap_Mode) == FALSE)
			{
				CFile pFile;
				pFile.Open(m_csRecipePath, CFile::modeCreate);
				pFile.Close();

				bNoFile = TRUE;
			}

			recipeFile_Cam2.GetItemValue(_T("MAX_CODE_COUNT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nMaxCodeCount, 1);
			recipeFile_Cam2.GetItemValue(_T("USE_READCODE"), readRecipe.m_NVisionInspRecipe_Cam2.m_bUseReadCode, 1);
			recipeFile_Cam2.GetItemValue(_T("USE_INKJET_CHARACTERS_INSPECT"), readRecipe.m_NVisionInspRecipe_Cam2.m_bUseInkjetCharactersInspect, 1);
			recipeFile_Cam2.GetItemValue(_T("USE_ROTATE_ROI"), readRecipe.m_NVisionInspRecipe_Cam2.m_bUseRotateROI, 0);
			recipeFile_Cam2.GetItemValue(_T("NUMBER_OF_ROI"), readRecipe.m_NVisionInspRecipe_Cam2.m_nNumberOfROI, 0);

			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_OUTER_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_OuterX, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_OUTER_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_OuterY, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_OUTER_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Outer_Width, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_OUTER_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Outer_Height, 0);

			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_INNER_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_InnerX, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_INNER_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_InnerY, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_INNER_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Inner_Width, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ROI_INNER_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Inner_Height, 0);

			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_COORDINATES_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateCoordinatesX, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_COORDINATES_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateCoordinatesY, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam2.m_dTemplateAngleRotate, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_MATCHING_RATE"), readRecipe.m_NVisionInspRecipe_Cam2.m_dTemplateMatchingRate, 0);
			recipeFile_Cam2.GetItemValue(_T("TEMPLATE_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam2.m_bTemplateShowGraphics, 0);

			recipeFile_Cam2.GetItemValue(_T("ROI1_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Width, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Height, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Offset_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Offset_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_AngleRotate, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI1UseOffset, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI1UseLocator, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI1ShowGraphics, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_GrayThreshold_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_GrayThreshold_Max, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_PixelCount_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI1_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_PixelCount_Max, 0);

			recipeFile_Cam2.GetItemValue(_T("ROI2_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Width, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Height, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Offset_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Offset_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_AngleRotate, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI2UseOffset, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI2UseLocator, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI2ShowGraphics, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_GrayThreshold_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_GrayThreshold_Max, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_PixelCount_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI2_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_PixelCount_Max, 0);
						  
			recipeFile_Cam2.GetItemValue(_T("ROI3_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_WIDTH"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Width, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_HEIGHT"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Height, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_OFFSET_X"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Offset_X, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_OFFSET_Y"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Offset_Y, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_ANGLE_ROTATE"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_AngleRotate, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_USE_OFFSET"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI3UseOffset, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_USE_LOCATOR"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI3UseLocator, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_SHOW_GRAPHICS"), readRecipe.m_NVisionInspRecipe_Cam2.m_bROI3ShowGraphics, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_GRAY_THRESHOLD_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_GrayThreshold_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_GRAY_THRESHOLD_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_GrayThreshold_Max, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_PIXEL_COUNT_MIN"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_PixelCount_Min, 0);
			recipeFile_Cam2.GetItemValue(_T("ROI3_PIXEL_COUNT_MAX"), readRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_PixelCount_Max, 0);
			break;
		}
		case 2:
		{
			break;
		}
		case 3:
		{
			break;
		}
		case 4:
		{
			break;
		}
		case 5:
		{
			break;
		}
		case 6:
		{
			break;
		}
		case 7:
		{
			break;
		}
		}
	}

	*(m_pNVisionInspectRecipe) = readRecipe;
	*(pRecipe) = *(m_pNVisionInspectRecipe);

	return TRUE;
}

BOOL CNVisionInspectProcessor::LoadCameraSettings(CNVisionInspectCameraSetting* pCameraSetting)
{
	if (m_csCamSettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Camera setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csCamSettingPath);
	if (m_csCamSettingPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Camera setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CNVisionInspectCameraSetting camSettings;

	// convert path
	USES_CONVERSION;
	char chCamSettingPath[1024] = {};
	sprintf_s(chCamSettingPath, "%s", W2A(m_csCamSettingPath));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(chCamSettingPath, error);
	if (!m_pXmlFile)
	{
		AfxMessageBox((CString)(error.c_str()));
		return FALSE;
	}

	// 3. Create xml doc
	m_pXmlDoc = ::CreateXMLFromFile(m_pXmlFile, error);
	if (!m_pXmlDoc)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		return FALSE;
	}

	// 4. Find root: Configurations
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "Camera1Settings", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// start read

	CString csIsSaveFullImage = (CString)pRoot->first_node("IsSaveFullImage")->value();
	camSettings.m_bSaveFullImage = csIsSaveFullImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csIsSaveDefectImage = (CString)pRoot->first_node("IsSaveDefectImage")->value();
	camSettings.m_bSaveDefectImage = csIsSaveDefectImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csShowGraphics = (CString)pRoot->first_node("ShowGraphics")->value();
	camSettings.m_bShowGraphics = csShowGraphics.Compare(_T("true")) == 0 ? TRUE : FALSE;

	camSettings.m_nChannels = std::stoi(pRoot->first_node("Channels")->value());
	camSettings.m_nFrameWidth = std::stoi(pRoot->first_node("FrameWidth")->value());
	camSettings.m_nFrameHeight = std::stoi(pRoot->first_node("FrameHeight")->value());
	camSettings.m_nFrameDepth = std::stoi(pRoot->first_node("FrameDepth")->value());
	camSettings.m_nMaxFrameCount = std::stoi(pRoot->first_node("MaxFrameCount")->value());

	CString csCameraName = (CString)pRoot->first_node("CameraName")->value();
	ZeroMemory(camSettings.m_sCameraName, sizeof(camSettings.m_sCameraName));
	wsprintf(camSettings.m_sCameraName, _T("%s"), (TCHAR*)(LPCTSTR)csCameraName);

	CString csInterfaceType = (CString)pRoot->first_node("InterfaceType")->value();
	ZeroMemory(camSettings.m_sInterfaceType, sizeof(camSettings.m_sInterfaceType));
	wsprintf(camSettings.m_sInterfaceType, _T("%s"), (TCHAR*)(LPCTSTR)csInterfaceType);

	CString csSensorType = (CString)pRoot->first_node("SensorType")->value();
	ZeroMemory(camSettings.m_sSensorType, sizeof(camSettings.m_sSensorType));
	wsprintf(camSettings.m_sSensorType, _T("%s"), (TCHAR*)(LPCTSTR)csSensorType);

	CString csManufacturer = (CString)pRoot->first_node("Manufacturer")->value();
	ZeroMemory(camSettings.m_sManufacturer, sizeof(camSettings.m_sManufacturer));
	wsprintf(camSettings.m_sManufacturer, _T("%s"), (TCHAR*)(LPCTSTR)csManufacturer);

	CString csSerialNumber = (CString)pRoot->first_node("SerialNumber")->value();
	ZeroMemory(camSettings.m_sSerialNumber, sizeof(camSettings.m_sSerialNumber));
	wsprintf(camSettings.m_sSerialNumber, _T("%s"), (TCHAR*)(LPCTSTR)csSerialNumber);

	CString csModel = (CString)pRoot->first_node("Model")->value();
	ZeroMemory(camSettings.m_sModel, sizeof(camSettings.m_sModel));
	wsprintf(camSettings.m_sModel, _T("%s"), (TCHAR*)(LPCTSTR)csModel);

	CString csFullImagePath = (CString)pRoot->first_node("FullImagePath")->value();
	ZeroMemory(camSettings.m_sFullImagePath, sizeof(camSettings.m_sFullImagePath));
	wsprintf(camSettings.m_sFullImagePath, _T("%s"), (TCHAR*)(LPCTSTR)csFullImagePath);

	CString csDefectImagePath = (CString)pRoot->first_node("DefectImagePath")->value();
	ZeroMemory(camSettings.m_sDefectImagePath, sizeof(camSettings.m_sDefectImagePath));
	wsprintf(camSettings.m_sDefectImagePath, _T("%s"), (TCHAR*)(LPCTSTR)csDefectImagePath);

	CString csTemplateImagePath = (CString)pRoot->first_node("TemplateImagePath")->value();
	ZeroMemory(camSettings.m_sTemplateImagePath, sizeof(camSettings.m_sTemplateImagePath));
	wsprintf(camSettings.m_sTemplateImagePath, _T("%s"), (TCHAR*)(LPCTSTR)csTemplateImagePath);

	CString csROIsPath = (CString)pRoot->first_node("ROIsPath")->value();
	ZeroMemory(camSettings.m_sROIsPath, sizeof(camSettings.m_sROIsPath));
	wsprintf(camSettings.m_sROIsPath, _T("%s"), (TCHAR*)(LPCTSTR)csROIsPath);

	*(pCameraSetting) = camSettings;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SaveSystemSetting(CNVisionInspectSystemSetting* pSystemSetting)
{
	if (m_csSysSettingsPath.IsEmpty())
	{
		AfxMessageBox(_T("System setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csSysSettingsPath);
	if (m_csSysSettingsPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("System setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CNVisionInspectSystemSetting sysSetting;
	sysSetting = *(pSystemSetting);
	*(m_pNVisionInspectSystemSetting) = *(pSystemSetting);

	// convert path
	USES_CONVERSION;
	char chSysSettingPath[1024] = {};
	sprintf_s(chSysSettingPath, "%s", W2A(m_csSysSettingsPath));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(chSysSettingPath, std::ios::in | std::ios::out);
	std::string inputXml;
	std::string line;
	while (std::getline(fs, line))
	{
		inputXml += line;
	}
	std::vector<char> buffer(inputXml.begin(), inputXml.end());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = xmlDoc.first_node("SystemSettings");

	// Write data

#pragma region Write data 

	std::string strNumberOfInspCamera = std::to_string(sysSetting.m_nNumberOfInspectionCamera);
	pRoot->first_node("NumberOfInspectionCamera")->value(strNumberOfInspCamera.c_str());

	CString csSimulation = sysSetting.m_bSimulation == TRUE ? _T("true") : _T("false");
	const char* sSimulation = W2A(csSimulation);
	pRoot->first_node("Simulation")->value(sSimulation);

	CString csByPass = sysSetting.m_bByPass == TRUE ? _T("true") : _T("false");
	const char* sByPass = W2A(csByPass);
	pRoot->first_node("ByPass")->value(sByPass);

	CString csTestMode = sysSetting.m_bTestMode == TRUE ? _T("true") : _T("false");
	const char* sTestMode = W2A(csTestMode);
	pRoot->first_node("TestMode")->value(sTestMode);

	const char* sModelName = W2A(sysSetting.m_sModelName);
	pRoot->first_node("ModelName")->value(sModelName);

	const char* sModelList = W2A(sysSetting.m_sModelList);
	pRoot->first_node("ModelList")->value(sModelList);

#pragma endregion

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	std::ofstream file;
	file.open(chSysSettingPath);
	file << data;
	file.close();

	return TRUE;
}

BOOL CNVisionInspectProcessor::SaveRecipe(int nCamIdx, CNVisionInspectRecipe* pRecipe)
{
	if (pRecipe == NULL)
		return FALSE;

	if (nCamIdx < 0)
		return FALSE;

	nCamIdx += 1; // because the cam index start from 0 , the name file start from 1.

	// set recipe path
	m_csRecipePath.Format(_T("%sVisionSettings\\Recipe\\%s_%s%d.%s"), GetCurrentPathApp(), m_pNVisionInspectSystemSetting->m_sModelName, _T("Cam"), nCamIdx, _T("cfg"));

	BOOL bNoFile = FALSE;
	// Params..
	CString strParameterKey;

	char	strChar[1024] = {};
	CString strValue = _T("");
	int		nValue = 0;

	switch (nCamIdx)
	{
	case 1:
	{
		CConfig recipeFile_Cam1;

		if (recipeFile_Cam1.SetRegiConfig(NULL, NULL, (TCHAR*)(LPCTSTR)m_csRecipePath, FileMap_Mode) == FALSE)
		{
			CFile pFile;
			pFile.Open(m_csRecipePath, CFile::modeCreate);
			pFile.Close();

			bNoFile = TRUE;
		}

		USES_CONVERSION;

		recipeFile_Cam1.SetItemValue(_T("MAX_CODE_COUNT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nMaxCodeCount);
		recipeFile_Cam1.SetItemValue(_T("USE_READCODE"), pRecipe->m_NVisionInspRecipe_Cam1.m_bUseReadCode);
		recipeFile_Cam1.SetItemValue(_T("USE_INKJET_CHARACTERS_INSPECT"), pRecipe->m_NVisionInspRecipe_Cam1.m_bUseInkjetCharactersInspect);
		recipeFile_Cam1.SetItemValue(_T("USE_ROTATE_ROI"), pRecipe->m_NVisionInspRecipe_Cam1.m_bUseRotateROI);
		recipeFile_Cam1.SetItemValue(_T("NUMBER_OF_ROI"), pRecipe->m_NVisionInspRecipe_Cam1.m_nNumberOfROI);

		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_OUTER_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterX);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_OUTER_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterY);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_OUTER_WIDTH"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Width);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_OUTER_HEIGHT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Height);

		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_INNER_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerX);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_INNER_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerY);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_INNER_WIDTH"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Width);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ROI_INNER_HEIGHT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Height);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_SHOW_GRAPHICS"), pRecipe->m_NVisionInspRecipe_Cam1.m_bTemplateShowGraphics);

		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_COORDINATES_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_COORDINATES_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_ANGLE_ROTATE"), pRecipe->m_NVisionInspRecipe_Cam1.m_dTemplateAngleRotate);
		recipeFile_Cam1.SetItemValue(_T("TEMPLATE_MATCHING_RATE"), pRecipe->m_NVisionInspRecipe_Cam1.m_dTemplateMatchingRate);

		recipeFile_Cam1.SetItemValue(_T("ROI1_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_X);
		recipeFile_Cam1.SetItemValue(_T("ROI1_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI1_WIDTH"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Width);
		recipeFile_Cam1.SetItemValue(_T("ROI1_HEIGHT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Height);
		recipeFile_Cam1.SetItemValue(_T("ROI1_OFFSET_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Offset_X);
		recipeFile_Cam1.SetItemValue(_T("ROI1_OFFSET_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Offset_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI1_ANGLE_ROTATE"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_AngleRotate);
		recipeFile_Cam1.SetItemValue(_T("ROI1_GRAY_THRESHOLD_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI1_GRAY_THRESHOLD_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI1_PIXEL_COUNT_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI1_PIXEL_COUNT_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI1_USE_OFFSET"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI1UseOffset);
		recipeFile_Cam1.SetItemValue(_T("ROI1_USE_LOCATOR"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI1UseLocator);
		recipeFile_Cam1.SetItemValue(_T("ROI1_SHOW_GRAPHICS"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI1ShowGraphics);

		recipeFile_Cam1.SetItemValue(_T("ROI2_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_X);
		recipeFile_Cam1.SetItemValue(_T("ROI2_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI2_WIDTH"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Width);
		recipeFile_Cam1.SetItemValue(_T("ROI2_HEIGHT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Height);
		recipeFile_Cam1.SetItemValue(_T("ROI2_OFFSET_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Offset_X);
		recipeFile_Cam1.SetItemValue(_T("ROI2_OFFSET_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Offset_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI2_ANGLE_ROTATE"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_AngleRotate);
		recipeFile_Cam1.SetItemValue(_T("ROI2_GRAY_THRESHOLD_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI2_GRAY_THRESHOLD_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI2_PIXEL_COUNT_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI2_PIXEL_COUNT_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI2_USE_OFFSET"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI2UseOffset);
		recipeFile_Cam1.SetItemValue(_T("ROI2_USE_LOCATOR"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI2UseLocator);
		recipeFile_Cam1.SetItemValue(_T("ROI2_SHOW_GRAPHICS"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI2ShowGraphics);

		recipeFile_Cam1.SetItemValue(_T("ROI3_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_X);
		recipeFile_Cam1.SetItemValue(_T("ROI3_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI3_WIDTH"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Width);
		recipeFile_Cam1.SetItemValue(_T("ROI3_HEIGHT"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Height);
		recipeFile_Cam1.SetItemValue(_T("ROI3_OFFSET_X"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Offset_X);
		recipeFile_Cam1.SetItemValue(_T("ROI3_OFFSET_Y"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Offset_Y);
		recipeFile_Cam1.SetItemValue(_T("ROI3_ANGLE_ROTATE"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_AngleRotate);
		recipeFile_Cam1.SetItemValue(_T("ROI3_GRAY_THRESHOLD_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI3_GRAY_THRESHOLD_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI3_PIXEL_COUNT_MIN"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Min);
		recipeFile_Cam1.SetItemValue(_T("ROI3_PIXEL_COUNT_MAX"), pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Max);
		recipeFile_Cam1.SetItemValue(_T("ROI3_USE_OFFSET"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI3UseOffset);
		recipeFile_Cam1.SetItemValue(_T("ROI3_USE_LOCATOR"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI3UseLocator);
		recipeFile_Cam1.SetItemValue(_T("ROI3_SHOW_GRAPHICS"), pRecipe->m_NVisionInspRecipe_Cam1.m_bROI3ShowGraphics);

		recipeFile_Cam1.WriteToFile();

		break;
	}
	case 2:
	{
		break;
	}
	case 3:
	{
		break;
	}
	case 4:
	{
		break;
	}
	case 5:
	{
		break;
	}
	case 6:
	{
		break;
	}
	case 7:
	{
		break;
	}
	case 8:
	{
		break;
	}
	}

	*(m_pNVisionInspectRecipe) = *pRecipe;
}

BOOL CNVisionInspectProcessor::SaveCameraSettings(int nCamIdx, CNVisionInspectCameraSetting* pCameraSetting)
{
	// set camera setting path
	m_csCamSettingPath.Format(_T("%sVisionSettings\\Settings\\%s%d%s.%s"), GetCurrentPathApp(), _T("Camera"), nCamIdx, _T("Settings"), _T("config"));

	if (m_csCamSettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Camera setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csCamSettingPath);
	if (m_csCamSettingPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Camera setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CNVisionInspectCameraSetting camSetting;
	camSetting = *(pCameraSetting);
	*(m_pNVisionInspectCameraSetting[nCamIdx]) = *(pCameraSetting);

	// convert path
	USES_CONVERSION;
	char chCamSettingPath[1024] = {};
	sprintf_s(chCamSettingPath, "%s", W2A(m_csCamSettingPath));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(chCamSettingPath, std::ios::in | std::ios::out);
	std::string inputXml;
	std::string line;
	while (std::getline(fs, line))
	{
		inputXml += line;
	}
	std::vector<char> buffer(inputXml.begin(), inputXml.end());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = xmlDoc.first_node("Camera1Settings");

	// Write data

#pragma region Write data 

	CString csIsSaveFullImage = camSetting.m_bSaveFullImage == TRUE ? _T("true") : _T("false");
	const char* sIsSaveFullImage = W2A(csIsSaveFullImage);
	pRoot->first_node("IsSaveFullImage")->value(sIsSaveFullImage);

	CString csIsSaveDefectImage = camSetting.m_bSaveDefectImage == TRUE ? _T("true") : _T("false");
	const char* sIsSaveDefectImage = W2A(csIsSaveDefectImage);
	pRoot->first_node("IsSaveDefectImage")->value(sIsSaveDefectImage);

	CString csShowGraphics = camSetting.m_bShowGraphics == TRUE ? _T("true") : _T("false");
	const char* sShowGraphics = W2A(csShowGraphics);
	pRoot->first_node("ShowGraphics")->value(sShowGraphics);

	CString csChannels;
	csChannels.Format(_T("%d"), camSetting.m_nChannels);
	const char* sChannels = W2A(csChannels);
	pRoot->first_node("Channels")->value(sChannels);

	CString csFrameWidth;
	csFrameWidth.Format(_T("%d"), camSetting.m_nFrameWidth);
	const char* sFrameWidth = W2A(csFrameWidth);
	pRoot->first_node("FrameWidth")->value(sFrameWidth);

	CString csFrameHeight;
	csFrameHeight.Format(_T("%d"), camSetting.m_nFrameHeight);
	const char* sFrameHeight = W2A(csFrameHeight);
	pRoot->first_node("FrameHeight")->value(sFrameHeight);

	CString csFrameDepth;
	csFrameDepth.Format(_T("%d"), camSetting.m_nFrameDepth);
	const char* sFrameDepth = W2A(csFrameDepth);
	pRoot->first_node("FrameDepth")->value(sFrameDepth);

	CString csMaxFrameCount;
	csMaxFrameCount.Format(_T("%d"), camSetting.m_nMaxFrameCount);
	const char* sMaxFrameCount = W2A(csMaxFrameCount);
	pRoot->first_node("MaxFrameCount")->value(sMaxFrameCount);

	const char* sCameraName = W2A(camSetting.m_sCameraName);
	pRoot->first_node("CameraName")->value(sCameraName);

	const char* sInterfaceType = W2A(camSetting.m_sInterfaceType);
	pRoot->first_node("InterfaceType")->value(sInterfaceType);

	const char* sSensorType = W2A(camSetting.m_sSensorType);
	pRoot->first_node("SensorType")->value(sSensorType);

	const char* sManufacturer = W2A(camSetting.m_sManufacturer);
	pRoot->first_node("Manufacturer")->value(sManufacturer);

	const char* sSerialNumber = W2A(camSetting.m_sSerialNumber);
	pRoot->first_node("SerialNumber")->value(sSerialNumber);

	const char* sModel = W2A(camSetting.m_sModel);
	pRoot->first_node("Model")->value(sModel);

	const char* sFullImagePath = W2A(camSetting.m_sFullImagePath);
	pRoot->first_node("FullImagePath")->value(sFullImagePath);

	const char* sDefectImagePath = W2A(camSetting.m_sDefectImagePath);
	pRoot->first_node("DefectImagePath")->value(sDefectImagePath);

	const char* sTemplateImagePath = W2A(camSetting.m_sTemplateImagePath);
	pRoot->first_node("TemplateImagePath")->value(sTemplateImagePath);

	const char* sROIsPath = W2A(camSetting.m_sROIsPath);
	pRoot->first_node("ROIsPath")->value(sROIsPath);

#pragma endregion

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	std::ofstream file;
	file.open(chCamSettingPath);
	file << data;
	file.close();

	return TRUE;
}

void CNVisionInspectProcessor::RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}

void CNVisionInspectProcessor::RegCallbackLogFunc(CallbackLogFunc* pFunc)
{
	m_pCallbackLogFunc = pFunc;
}

void CNVisionInspectProcessor::RegCallbackAlarmFunc(CallbackAlarmFunc* pFunc)
{
	m_pCallbackAlarmFunc = pFunc;
}

void CNVisionInspectProcessor::RegCallbackLocatorTrainCompleteFunc(CallbackLocatorTrainComplete* pFunc)
{
	m_pCallbackLocatorTrainCompleteFunc = pFunc;
}

void CNVisionInspectProcessor::InspectComplete(int nCamIdx, BOOL bSetting)
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	(m_pCallbackInsCompleteFunc)(nCamIdx, bSetting);
}

void CNVisionInspectProcessor::LocatorTrainComplete(int nCamIdx)
{
	SaveRecipe(nCamIdx, m_pNVisionInspectRecipe);

	if (m_pCallbackLocatorTrainCompleteFunc == NULL)
		return;

	(m_pCallbackLocatorTrainCompleteFunc)(nCamIdx);
}

void CNVisionInspectProcessor::LogMessage(char* strMessage)
{
	if (strMessage == NULL)
		return;

	CString strLogMessage = (CString)strMessage;

	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strLogMessage);
}

void CNVisionInspectProcessor::LogMessage(CString strMessage)
{
	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strMessage);
}

void CNVisionInspectProcessor::ShowLogView(BOOL bShow)
{
	if (m_pLogView == NULL)
		return;

#ifndef _DEBUG
	m_pLogView->ShowMode(bShow);
#endif // !_DEBUG
}

LPBYTE CNVisionInspectProcessor::GetResultBuffer(int nBuff, int nFrame)
{
	if (m_pResultBuffer[nBuff] == NULL)
		return NULL;

	return m_pResultBuffer[nBuff]->GetFrameImage(nFrame);
}

BOOL CNVisionInspectProcessor::SetResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pResultBuffer[nBuff] == NULL)
		return FALSE;

	return m_pResultBuffer[nBuff]->SetFrameImage(nFrame, buff);
}

BOOL CNVisionInspectProcessor::GetInspectionResult(int nCoreIdx, CNVisionInspectResult* pNVisionInspRes)
{
	CSingleLock localLock(&m_csInspResult);
	localLock.Lock();
	*(pNVisionInspRes) = *(m_pNVisionInspectResult);

	localLock.Unlock();
	return TRUE;
}

LPBYTE CNVisionInspectProcessor::GetImageBufferHikCam(int nCamIdx)
{
	if (m_pNVisionInspectHikCam == NULL)
		return NULL;

	LPBYTE pImageBuff = m_pNVisionInspectHikCam->GetBufferImage(nCamIdx);

	if (m_pNVisionInspectCameraSetting[nCamIdx]->m_nChannels == 1)
	{
		int nWidth = m_pNVisionInspectCameraSetting[nCamIdx]->m_nFrameWidth;
		int nHeight = m_pNVisionInspectCameraSetting[nCamIdx]->m_nFrameHeight;
		cv::Mat matGray(nHeight, nWidth, CV_8UC1, pImageBuff);

		cv::cvtColor(matGray, m_matBGR, cv::COLOR_GRAY2BGR);

		return m_matBGR.data;
	}

	return pImageBuff;
}

BOOL CNVisionInspectProcessor::LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath)
{
	if (m_pSimulatorBuffer[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pSimulatorBuffer[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pSimulatorBuffer[nBuff]->GetFrameHeight();
	int nFrameCount = m_pSimulatorBuffer[nBuff]->GetFrameCount();
	int nFrameSize = m_pSimulatorBuffer[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_COLOR);

	if (pOpenImage.empty())
		return FALSE;

	/*if (pOpenImage.type() != CV_8UC1)
		return FALSE;

	LPBYTE pBuffer = m_pImageBuffer[nBuff]->GetSharedBuffer();

	int nCopyHeight = (nFrameHeight * nFrameCount < pOpenImage.rows) ? nFrameHeight * nFrameCount : pOpenImage.rows;
	int nCopyWidth = (nFrameWidth < pOpenImage.cols) ? nFrameWidth : pOpenImage.cols;

	ZeroMemory(pBuffer, nFrameSize * nFrameCount);

	for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer + (i * nFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);*/

	m_pSimulatorBuffer[nBuff]->SetFrameImage(nFrame, pOpenImage.data);

	return TRUE;
}

BOOL CNVisionInspectProcessor::LocatorTool_Train(int nCamIdx)
{
	LPBYTE pBuffer = GetImageBufferHikCam(nCamIdx);

	if (pBuffer == NULL)
		return FALSE;

	int nCoreIdx = nCamIdx;

	m_pNVisionInspectCore[nCoreIdx]->LocatorTool_Train(nCamIdx, pBuffer);

	return TRUE;
}

BOOL CNVisionInspectProcessor::LocatorToolSimulator_Train(int nSimuBuff, int nFrame)
{
	LPBYTE pBuffer = GetSimulatorBuffer(nSimuBuff, nFrame);

	if (pBuffer == NULL)
		return FALSE;

	int nCoreIdx = nSimuBuff;
	int nCamIdx = nSimuBuff;
	m_pNVisionInspectCore[nCoreIdx]->LocatorTool_Train(nCamIdx, pBuffer);

	return TRUE;
}

BOOL CNVisionInspectProcessor::SelectROI(int nCamIdx, int nROIIdx, int nFrom)
{
	int nSimuBuff = nCamIdx;
	int nCoreIdx = nCamIdx;
	int nFrame = 0;
	LPBYTE pBuffer = NULL;

	switch (nFrom)
	{
	case 0:
		pBuffer = GetSimulatorBuffer(nSimuBuff, nFrame);
		break;
	case 1:
		pBuffer = GetImageBufferHikCam(nCamIdx);
		break;
	}

	if (pBuffer == NULL)
		return FALSE;

	m_pNVisionInspectCore[nCoreIdx]->MakeROI(nCamIdx, nROIIdx, pBuffer);

	return TRUE;
}

LPBYTE CNVisionInspectProcessor::GetSimulatorBuffer(int nBuff, int nFrame)
{
	if (m_pSimulatorBuffer[nBuff] == NULL)
		return NULL;

	return m_pSimulatorBuffer[nBuff]->GetFrameImage(nFrame);
}

void CNVisionInspectProcessor::AlarmMessage(CString strAlarmMessage)
{
	if (m_pCallbackAlarmFunc == NULL)
		return;

	USES_CONVERSION;
	char strMsgBuffer[1024] = {};
	sprintf_s(strMsgBuffer, "%s", W2A(strAlarmMessage));

	(m_pCallbackAlarmFunc)(strMsgBuffer);
}

void CNVisionInspectProcessor::SystemMessage(const TCHAR* lpstrFormat, ...)
{
	va_list list;
	TCHAR strText[2048] = { 0 };

	va_start(list, lpstrFormat);
	_vstprintf_s(strText, lpstrFormat, list);
	va_end(list);

	CString strValue = _T("");
	strValue.Format(_T("%s"), strText);

	SystemMessage(strValue);
}

void CNVisionInspectProcessor::SystemMessage(CString strMessage)
{
	if (m_pCallbackLogFunc == NULL)
	{
		USES_CONVERSION;
		char strMsgBuffer[1024] = {};
		sprintf_s(strMsgBuffer, "%s", W2A(strMessage));

		LogMessage(strMsgBuffer);

		printf("%s\r\n", strMsgBuffer);
	}
	else
	{
		USES_CONVERSION;
		char strMsgBuffer[1024] = {};
		sprintf_s(strMsgBuffer, "%s", W2A(strMessage));

		m_pCallbackLogFunc(strMsgBuffer);

		printf("%s\r\n", strMsgBuffer);
	}
}

BOOL CNVisionInspectProcessor::CreateResultBuffer()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameSize = 0;
	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < m_pNVisionInspectSystemSetting->m_nNumberOfInspectionCamera; i++)
	{
		DWORD dwFrameWidth = (DWORD)m_pNVisionInspectCameraSetting[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pNVisionInspectCameraSetting[i]->m_nFrameHeight;
		DWORD dwFrameCount = 0;

		dwFrameSize = dwFrameWidth * dwFrameHeight * (DWORD)NUMBER_OF_CHANNEL_BGR;

		if (m_pResultBuffer[i] != NULL)
		{
			m_pResultBuffer[i]->DeleteSharedMemory();
			delete m_pResultBuffer[i];
			m_pResultBuffer[i] = NULL;
		}

		m_pResultBuffer[i] = new CSharedMemoryBuffer;

		dwFrameCount = (DWORD)MAX_IMAGE_BUFFER;

		dwTotalFrameCount += dwFrameCount;

		m_pResultBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pResultBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pResultBuffer[i]->SetFrameCount(dwFrameCount);
		m_pResultBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size_Side = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory_Side;
		strMemory_Side.Format(_T("%s_%d"), "ResultBuffer", i);

		bRetValue = m_pResultBuffer[i]->CreateSharedMemory(strMemory_Side, dw64Size_Side);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

BOOL CNVisionInspectProcessor::CreateSimulatorBuffer()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameCount = 0;
	DWORD dwFrameSize = 0;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < m_pNVisionInspectSystemSetting->m_nNumberOfInspectionCamera; i++)
	{
		DWORD dwFrameWidth = (DWORD)m_pNVisionInspectCameraSetting[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pNVisionInspectCameraSetting[i]->m_nFrameHeight;
		dwFrameSize = dwFrameWidth * dwFrameHeight * (DWORD)NUMBER_OF_CHANNEL_BGR;

		if (m_pSimulatorBuffer[i] != NULL)
		{
			m_pSimulatorBuffer[i]->DeleteSharedMemory();
			delete m_pSimulatorBuffer[i];
			m_pSimulatorBuffer[i] = NULL;
		}

		m_pSimulatorBuffer[i] = new CSharedMemoryBuffer;

		dwFrameCount = (DWORD)MAX_IMAGE_BUFFER;

		dwTotalFrameCount += dwFrameCount;

		m_pSimulatorBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pSimulatorBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pSimulatorBuffer[i]->SetFrameCount(dwFrameCount);
		m_pSimulatorBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size_Side = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory_Side;
		strMemory_Side.Format(_T("%s_%d"), "ResultBuffer", i);

		bRetValue = m_pSimulatorBuffer[i]->CreateSharedMemory(strMemory_Side, dw64Size_Side);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}
