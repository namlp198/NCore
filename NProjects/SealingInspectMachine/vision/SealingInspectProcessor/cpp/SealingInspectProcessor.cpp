#include "pch.h"
#include "SealingInspectProcessor.h"

CSealingInspectProcessor::CSealingInspectProcessor()
{
	m_csSysSettingsPath = GetCurrentPathApp() + _T("Settings\\SystemSettings.config");
	m_csLightSettingPath = GetCurrentPathApp() + _T("Settings\\LightSettings.setting");

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++) {
		if (m_pImageBuffer_Side[i] != NULL)
			delete m_pImageBuffer_Side[i], m_pImageBuffer_Side[i] = NULL;
	}
	for (int i = 0; i < MAX_IMAGE_BUFFER_TOPCAM; i++) {
		if (m_pImageBuffer_Top[i] != NULL)
			delete m_pImageBuffer_Top[i], m_pImageBuffer_Top[i] = NULL;
	}

	m_pSealingInspRecipe = NULL;
	m_pTcpSocket = NULL;
}

CSealingInspectProcessor::~CSealingInspectProcessor()
{
	Destroy();
}

BOOL CSealingInspectProcessor::Initialize()
{
	if (m_pLogView != NULL)
	{
		CString strSaveFilePath;
		strSaveFilePath.Format(_T("%s"), "");

		if (strSaveFilePath.IsEmpty() || strSaveFilePath.GetLength() == 0)
			strSaveFilePath = _T("D:\\SealingInspect_Image");

		CString strLogPath;
		strLogPath.Format(_T("%s\\Log"), strSaveFilePath);
		CString strLogName = _T("ImageInspection");
		m_pLogView->SetLogPath(strLogPath, strLogName, 1000);
	}

	SystemMessage(_T("*********** Start Vision Processor ***********"));

	// 1. Create Image Buffer SIDE
	if (CreateBuffer_SIDE() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}
	// 2. Create Image Buffer TOP
	if (CreateBuffer_TOP() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// 3. Create Result Buffer SIDE
	if (CreateResultBuffer_SIDE() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}
	// 4. Create Result Buffer TOP
	if (CreateResultBuffer_TOP() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// 3. Load System Setting
	if (m_pSealingInspSystemSetting != NULL)
		delete m_pSealingInspSystemSetting, m_pSealingInspSystemSetting = NULL;
	m_pSealingInspSystemSetting = new CSealingInspectSystemSetting;
	LoadSystemSetting(m_pSealingInspSystemSetting);
	//MakeDirectory();

	// 4. Load Recipe
	if (m_pSealingInspRecipe != NULL)
		delete m_pSealingInspRecipe, m_pSealingInspRecipe = NULL;
	m_pSealingInspRecipe = new CSealingInspectRecipe;
	//LoadRecipe(m_pSealingInspRecipe);

	// 5. Inspect Result Data
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspResult[i] != NULL)
			delete m_pSealingInspResult[i], m_pSealingInspResult[i] = NULL;
		m_pSealingInspResult[i] = new CSealingInspectResult;
	}

	// 6. Status Inspect
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspStatus[i] != NULL)
			delete m_pSealingInspStatus[i], m_pSealingInspStatus[i] = NULL;
		m_pSealingInspStatus[i] = new CSealingInspectStatus;
	}

	// simulation IO
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspect_Simulation_IO[i] != NULL)
			delete m_pSealingInspect_Simulation_IO[i], m_pSealingInspect_Simulation_IO[i] = NULL;
		m_pSealingInspect_Simulation_IO[i] = new CSealingInspect_Simulation_IO;
	}

	// TCP Socket
	if (m_pTcpSocket != NULL)
		delete m_pTcpSocket, m_pTcpSocket = NULL;
	m_pTcpSocket = new CTCPSocket;
	m_pTcpSocket->SetRecvMessageCallback(TestTcpSocketCallback, this);


	if (m_pSealingInspSystemSetting->m_bSimulation == FALSE)
	{
		// 6. Hik Cam
		if (m_pSealingInspHikCam != NULL)
			delete m_pSealingInspHikCam, m_pSealingInspHikCam = NULL;
		m_pSealingInspHikCam = new CSealingInspectHikCam(this);
#ifndef TEST_NO_CAMERA
		m_pSealingInspHikCam->Initialize();
#endif
	}

	// 7. Inspect Core
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspCore[i] != NULL)
			delete m_pSealingInspCore[i], m_pSealingInspCore[i] = NULL;
		m_pSealingInspCore[i] = new CSealingInspectCore(this);
	}

	return TRUE;
}

BOOL CSealingInspectProcessor::Destroy()
{
	for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
		delete m_pImageBuffer_Side[i], m_pImageBuffer_Side[i] = NULL;
		delete m_pResultBuffer_Side[i], m_pResultBuffer_Side[i] = NULL;
	}
	for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
		delete m_pImageBuffer_Top[i], m_pImageBuffer_Top[i] = NULL;
		delete m_pResultBuffer_Top[i], m_pResultBuffer_Top[i] = NULL;
	}

	if (m_pSealingInspHikCam != NULL)
		delete m_pSealingInspHikCam, m_pSealingInspHikCam = NULL;

	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspCore[i] != NULL)
			delete m_pSealingInspCore[i], m_pSealingInspCore[i] = NULL;
	}

	if (m_pSealingInspSystemSetting != NULL)
		delete m_pSealingInspSystemSetting, m_pSealingInspSystemSetting = NULL;

	if (m_pSealingInspRecipe != NULL)
		delete m_pSealingInspRecipe, m_pSealingInspRecipe = NULL;

	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspResult[i] != NULL)
			delete m_pSealingInspResult[i], m_pSealingInspResult[i] = NULL;
	}

	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspect_Simulation_IO[i] != NULL)
			delete m_pSealingInspect_Simulation_IO[i], m_pSealingInspect_Simulation_IO[i] = NULL;
		m_pSealingInspect_Simulation_IO[i] = new CSealingInspect_Simulation_IO;
	}

	return TRUE;
}

CString CSealingInspectProcessor::GetCurrentPathApp()
{
	TCHAR buff[MAX_PATH];
	memset(buff, 0, MAX_PATH);
	::GetModuleFileName(NULL, buff, sizeof(buff));
	CString csFolder = buff;
	csFolder = csFolder.Left(csFolder.ReverseFind(_T('\\')) + 1);

	return csFolder;
}

BOOL CSealingInspectProcessor::LoadSystemSetting(CSealingInspectSystemSetting* pSystemSetting)
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

	CSealingInspectSystemSetting sysSettings;

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

	// start read
	CString csIPPLC1 = (CString)pRoot->first_node("IPPLC1")->value();
	ZeroMemory(sysSettings.m_sIPPLC1, sizeof(sysSettings.m_sIPPLC1));
	wsprintf(sysSettings.m_sIPPLC1, _T("%s"), (TCHAR*)(LPCTSTR)csIPPLC1);

	CString csIPPLC2 = (CString)pRoot->first_node("IPPLC2")->value();
	ZeroMemory(sysSettings.m_sIPPLC2, sizeof(sysSettings.m_sIPPLC2));
	wsprintf(sysSettings.m_sIPPLC2, _T("%s"), (TCHAR*)(LPCTSTR)csIPPLC2);

	CString csPortPLC1 = (CString)pRoot->first_node("PortPLC1")->value();
	ZeroMemory(sysSettings.m_sPortPLC1, sizeof(sysSettings.m_sPortPLC1));
	wsprintf(sysSettings.m_sPortPLC1, _T("%s"), (TCHAR*)(LPCTSTR)csPortPLC1);

	CString csPortPLC2 = (CString)pRoot->first_node("PortPLC2")->value();
	ZeroMemory(sysSettings.m_sPortPLC2, sizeof(sysSettings.m_sPortPLC2));
	wsprintf(sysSettings.m_sPortPLC2, _T("%s"), (TCHAR*)(LPCTSTR)csPortPLC2);

	CString csIPLightController1 = (CString)pRoot->first_node("IPLightController1")->value();
	ZeroMemory(sysSettings.m_sIPLightController1, sizeof(sysSettings.m_sIPLightController1));
	wsprintf(sysSettings.m_sIPLightController1, _T("%s"), (TCHAR*)(LPCTSTR)csIPLightController1);

	CString csIPLightController2 = (CString)pRoot->first_node("IPLightController2")->value();
	ZeroMemory(sysSettings.m_sIPLightController2, sizeof(sysSettings.m_sIPLightController2));
	wsprintf(sysSettings.m_sIPLightController2, _T("%s"), (TCHAR*)(LPCTSTR)csIPLightController2);

	CString csPortLightController1 = (CString)pRoot->first_node("PortLightController1")->value();
	ZeroMemory(sysSettings.m_sPortLightController1, sizeof(sysSettings.m_sPortLightController1));
	wsprintf(sysSettings.m_sPortLightController1, _T("%s"), (TCHAR*)(LPCTSTR)csPortLightController1);

	CString csPortLightController2 = (CString)pRoot->first_node("PortLightController2")->value();
	ZeroMemory(sysSettings.m_sPortLightController2, sizeof(sysSettings.m_sPortLightController2));
	wsprintf(sysSettings.m_sPortLightController2, _T("%s"), (TCHAR*)(LPCTSTR)csPortLightController2);

	CString csSaveFullImage = (CString)pRoot->first_node("SaveFullImage")->value();
	sysSettings.m_bSaveFullImage = csSaveFullImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csSaveDefectImage = (CString)pRoot->first_node("SaveDefectImage")->value();
	sysSettings.m_bSaveDefectImage = csSaveDefectImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csShowDetailImage = (CString)pRoot->first_node("ShowDetailImage")->value();
	sysSettings.m_bShowDetailImage = csShowDetailImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csSimulation = (CString)pRoot->first_node("Simulation")->value();
	sysSettings.m_bSimulation = csSimulation.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csByPass = (CString)pRoot->first_node("ByPass")->value();
	sysSettings.m_bByPass = csByPass.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csFullImagePath = (CString)pRoot->first_node("FullImagePath")->value();
	ZeroMemory(sysSettings.m_sFullImagePath, sizeof(sysSettings.m_sFullImagePath));
	wsprintf(sysSettings.m_sFullImagePath, _T("%s"), (TCHAR*)(LPCTSTR)csFullImagePath);

	CString csDefectImagePath = (CString)pRoot->first_node("DefectImagePath")->value();
	ZeroMemory(sysSettings.m_sDefectImagePath, sizeof(sysSettings.m_sDefectImagePath));
	wsprintf(sysSettings.m_sDefectImagePath, _T("%s"), (TCHAR*)(LPCTSTR)csDefectImagePath);

	CString csModelName = (CString)pRoot->first_node("ModelName")->value();
	ZeroMemory(sysSettings.m_sModelName, sizeof(sysSettings.m_sModelName));
	wsprintf(sysSettings.m_sModelName, _T("%s"), (TCHAR*)(LPCTSTR)csModelName);

	CString csModelList = (CString)pRoot->first_node("ModelList")->value();
	ZeroMemory(sysSettings.m_sModelList, sizeof(sysSettings.m_sModelList));
	wsprintf(sysSettings.m_sModelList, _T("%s"), (TCHAR*)(LPCTSTR)csModelList);

	sysSettings.m_nGoToPos1Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos1Time_Cavity1")->value());
	sysSettings.m_nGoToPos2Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos2Time_Cavity1")->value());
	sysSettings.m_nGoToPos3Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos3Time_Cavity1")->value());
	sysSettings.m_nGoToPos4Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos4Time_Cavity1")->value());
	sysSettings.m_nGoToPos5Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos5Time_Cavity1")->value());
	sysSettings.m_nGoToPos6Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos6Time_Cavity1")->value());
	sysSettings.m_nGoToPos7Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos7Time_Cavity1")->value());
	sysSettings.m_nGoToPos8Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos8Time_Cavity1")->value());
	sysSettings.m_nGoToPos9Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos9Time_Cavity1")->value());
	sysSettings.m_nGoToPos10Time_Cavity1 = std::atoi(pRoot->first_node("GoToPos10Time_Cavity1")->value());

	sysSettings.m_nOffsetTime_Pos1_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos1_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos2_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos2_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos3_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos3_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos4_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos4_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos5_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos5_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos6_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos6_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos7_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos7_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos8_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos8_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos9_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos9_Cavity1")->value());
	sysSettings.m_nOffsetTime_Pos10_Cavity1 = std::atoi(pRoot->first_node("OffsetTime_Pos10_Cavity1")->value());

	// set recipe path
	m_csRecipePath.Format(_T("%sRecipe\\%s.%s"), GetCurrentPathApp(), sysSettings.m_sModelName, _T("cfg"));

	*(pSystemSetting) = sysSettings;

	LoadLightSetting(pSystemSetting);

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CSealingInspectProcessor::LoadLightSetting(CSealingInspectSystemSetting* pSystemSetting)
{
	if (m_csLightSettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Light Setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csLightSettingPath);
	if (m_csLightSettingPath.Right(7).CompareNoCase(_T("setting")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Light Setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CSealingInspectLightSetting lightSettings[NUMBER_OF_LIGHT_CONTROLLER];

	// convert path
	USES_CONVERSION;
	char chLightSettingPath[1024] = {};
	sprintf_s(chLightSettingPath, "%s", W2A(m_csLightSettingPath));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(chLightSettingPath, error);
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

	XMLElement* pRoot = NULL;
	for (int i = 0; i < NUMBER_OF_LIGHT_CONTROLLER; i++) {
		if (i == 0) {
			pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "LightController1", error);
			if (!pRoot)
			{
				AfxMessageBox((CString)(error.c_str()));
				::DisposeXMLFile(m_pXmlFile);
				::DisposeXMLObject(m_pXmlDoc);
				return FALSE;
			}
		}
		else if (i == 1) {
			pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "LightController2", error);
			if (!pRoot)
			{
				AfxMessageBox((CString)(error.c_str()));
				::DisposeXMLFile(m_pXmlFile);
				::DisposeXMLObject(m_pXmlDoc);
				return FALSE;
			}
		}

		// start read
		CString csCH1 = (CString)pRoot->first_node("CH1")->value();
		ZeroMemory(lightSettings[i].m_sCH1, sizeof(lightSettings[i].m_sCH1));
		wsprintf(lightSettings[i].m_sCH1, _T("%s"), (TCHAR*)(LPCTSTR)csCH1);

		CString csCH2 = (CString)pRoot->first_node("CH2")->value();
		ZeroMemory(lightSettings[i].m_sCH2, sizeof(lightSettings[i].m_sCH2));
		wsprintf(lightSettings[i].m_sCH2, _T("%s"), (TCHAR*)(LPCTSTR)csCH2);

		CString csCH3 = (CString)pRoot->first_node("CH3")->value();
		ZeroMemory(lightSettings[i].m_sCH3, sizeof(lightSettings[i].m_sCH3));
		wsprintf(lightSettings[i].m_sCH3, _T("%s"), (TCHAR*)(LPCTSTR)csCH3);

		CString csCH4 = (CString)pRoot->first_node("CH4")->value();
		ZeroMemory(lightSettings[i].m_sCH4, sizeof(lightSettings[i].m_sCH4));
		wsprintf(lightSettings[i].m_sCH4, _T("%s"), (TCHAR*)(LPCTSTR)csCH4);

		CString csCH5 = (CString)pRoot->first_node("CH5")->value();
		ZeroMemory(lightSettings[i].m_sCH5, sizeof(lightSettings[i].m_sCH5));
		wsprintf(lightSettings[i].m_sCH5, _T("%s"), (TCHAR*)(LPCTSTR)csCH5);

		CString csCH6 = (CString)pRoot->first_node("CH6")->value();
		ZeroMemory(lightSettings[i].m_sCH6, sizeof(lightSettings[i].m_sCH6));
		wsprintf(lightSettings[i].m_sCH6, _T("%s"), (TCHAR*)(LPCTSTR)csCH6);

		pSystemSetting->m_LightSettings[i] = lightSettings[i];
	}

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CSealingInspectProcessor::LoadRecipe(CSealingInspectRecipe* pRecipe)
{
	if (pRecipe == NULL)
		return FALSE;

	CConfig recipeFile;

	BOOL bNoFile = FALSE;

	if (recipeFile.SetRegiConfig(NULL, NULL, (TCHAR*)(LPCTSTR)m_csRecipePath, FileMap_Mode) == FALSE)
	{
		CFile pFile;
		pFile.Open(m_csRecipePath, CFile::modeCreate);
		pFile.Close();

		bNoFile = TRUE;
	}

	// Params..
	CString strParameterKey = _T("");

	CString strValue = _T("");
	double dValue = 0.0;
	int nValue = 0;

	CSealingInspectRecipe readRecipe;

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++)
	{
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_TOPCAM"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_TOPCAM"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dRatioUmPxl, 0);

		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_REFER"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_INNER_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusInner_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_INNER_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusInner_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_OUTER_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusOuter_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_OUTER_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusOuter_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_bUseAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_MINENCLOSING_CIRCLE_ALGORITHMS_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nThresholdBinaryMinEnclosing, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_HOUGHCIRCLE_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_RADIUS_DIFFERENCE_MIN_HOUGHCIRCLE_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceRadiusDiffMin, 0);
		recipeFile.GetItemValue(i + 1, _T("DELTA_RADIUS_OUTER_INNER_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDeltaRadiusOuterInner, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_HORIZONTAL_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIHeight_Hor, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_HORIZONTAL_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIWidth_Hor, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_VERTICAL_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIHeight_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_VERTICAL_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIWidth_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_12H_OFFSET_X_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI12H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_12H_OFFSET_Y_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI12H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_3H_OFFSET_X_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI3H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_3H_OFFSET_Y_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI3H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_6H_OFFSET_X_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI6H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_6H_OFFSET_Y_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI6H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_9H_OFFSET_X_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI9H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_9H_OFFSET_Y_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI9H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_MINENCLOSING_CIRCLE_TOPCAM_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_MINENCLOSING_CIRCLE_TOPCAM_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("INCREMENT_ANGLE_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dIncrementAngle, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dThresholdCanny1_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dThresholdCanny2_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDelayTimeGrab, 0);
		recipeFile.GetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("HOUGHCIRLCLE_PARAM1_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHoughCircleParam1, 0);
		recipeFile.GetItemValue(i + 1, _T("HOUGHCIRLCLE_PARAM2_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHoughCircleParam2, 0);

		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_TOPCAM_FRAME1"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nVMax, 0);
	}

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++)
	{
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_TOPCAM"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_TOPCAM"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dRatioUmPxl, 0);

		recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROIWidth, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROIHeight, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_1H_OFFSET_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI1H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_1H_OFFSET_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI1H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_5H_OFFSET_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI5H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_5H_OFFSET_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI5H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_7H_OFFSET_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI7H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_7H_OFFSET_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI7H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_11H_OFFSET_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI11H_OffsetX, 0);
		recipeFile.GetItemValue(i + 1, _T("ROI_11H_OFFSET_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI11H_OffsetY, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_REFER"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDelayTimeGrab, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_BLOB_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeFindBlob_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_BLOB_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeFindBlob_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_BLOB_WHITE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobWhite, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_MAX_FIND_BLOB_WHITE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_BLOB_BLACK_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobBlack, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_MAX_FIND_BLOB_BLACK_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("MAX_BLOB_COUNT_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nBlobCount_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("BLOB_AREA_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dBlobArea_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("BLOB_AREA_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dBlobArea_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_CHECK_SURFACE_ALGORITHMS_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nUseCheckSurface, 0);

		recipeFile.GetItemValue(i + 1, _T("SELECT_METHOD_FIND_CIRCLE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSelectMethodFindCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_CIRCLE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_FindCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_CIRCLE_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeMin, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_CIRCLE_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeMax, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadiusMin, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadiusMax, 0);
		recipeFile.GetItemValue(i + 1, _T("MINDIST_HOUGHCIRCLE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nMinDist_HoughCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("PARAM1_HOUGHCIRCLE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nParam1_HoughCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("PARAM2_HOUGHCIRCLE_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nParam2_HoughCircle, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_1_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshold1_Canny, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_2_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshold2_Canny, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_1H_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_1H_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_5H_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_5H_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_7H_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_7H_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_11H_X_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_11H_Y_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y, 0);

		recipeFile.GetItemValue(i + 1, _T("WIDTH_ROI_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nWidth_ROIFindSealingOverflow, 0);
		recipeFile.GetItemValue(i + 1, _T("HEIGHT_ROI_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHeight_ROIFindSealingOverflow, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_1H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_1H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_1H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_1H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Ver, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_5H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_5H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_5H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_5H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Ver, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_7H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_7H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_7H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_7H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Ver, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_11H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_11H_HOZ_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_11H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Ver, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_11H_VER_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Ver, 0);

		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_FindSealingOverflow, 0);
		recipeFile.GetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_SEALING_OVERFLOW_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSize_FindSealingOverflow_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("AREA_CONTOUR_FIND_SEALING_OVERFLOW_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dAreaContour_FindSealingOverflow_Max, 0);

		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_MEASURE_WIDTH_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_MeasureWidth, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_TOPCAM_FRAME2"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nVMax, 0);
	}

	for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
	{
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_PIXEL_UM_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dRatioPxlUm, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dRatioUmPxl, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dRatioUmPxl, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dRatioUmPxl, 0);
		recipeFile.GetItemValue(i + 1, _T("RATIO_UM_PIXEL_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dRatioUmPxl, 0);

		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_REFER"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_REFER"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_REFER"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_REFER"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer, 0);

		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Max, 0);

		for (int k = 0; k < ROI_PARAMETER_COUNT; k++)
		{
			switch (k)
			{
			case 0:
				recipeFile.GetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k], 0);
				break;
			case 1:
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k], 0);
				break;
			case 2:
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k], 0);
				break;
			case 3:
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k], 0);
				recipeFile.GetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k], 0);
				break;
			}
		}

		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDelayTimeGrab, 0);
		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDelayTimeGrab, 0);
		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDelayTimeGrab, 0);
		recipeFile.GetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDelayTimeGrab, 0);

		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndX, 0);

		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndSearchRangeX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndSearchRangeX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndSearchRangeX, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndSearchRangeX, 0);

		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndXThresholdGray, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndXThresholdGray, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndXThresholdGray, 0);
		recipeFile.GetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndXThresholdGray, 0);

		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dThresholdCanny1_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dThresholdCanny1_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dThresholdCanny1_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dThresholdCanny1_MakeROI, 0);

		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dThresholdCanny2_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dThresholdCanny2_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dThresholdCanny2_MakeROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dThresholdCanny2_MakeROI, 0);

		recipeFile.GetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bUseAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_bUseAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_bUseAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_bUseAdvancedAlgorithms, 0);

		recipeFile.GetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms, 0);

		recipeFile.GetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms, 0);
		recipeFile.GetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nOffetY_Top, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nOffetY_Top, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nOffetY_Top, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nOffetY_Top, 0);

		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nOffetY_Bottom, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nOffetY_Bottom, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nOffetY_Bottom, 0);
		recipeFile.GetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nOffetY_Bottom, 0);

		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nThresholdBinaryFindROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nThresholdBinaryFindROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nThresholdBinaryFindROI, 0);
		recipeFile.GetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nThresholdBinaryFindROI, 0);

		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nVMax, 0);

		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nVMax, 0);

		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nVMax, 0);

		recipeFile.GetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nHMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nHMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nSMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nSMax, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nVMin, 0);
		recipeFile.GetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nVMax, 0);

		recipeFile.GetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME1"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bJustJudgeByMinBoundingRect, 0);
		recipeFile.GetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME2"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_bJustJudgeByMinBoundingRect, 0);
		recipeFile.GetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME3"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_bJustJudgeByMinBoundingRect, 0);
		recipeFile.GetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME4"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_bJustJudgeByMinBoundingRect, 0);

		recipeFile.GetItemValue(i + 1, _T("USE_HARDWARE_TRIGGER_MODE_SIDECAM"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bUseHardwareTrigger, 0);
	}

	*(pRecipe) = readRecipe;

	return TRUE;
}

BOOL CSealingInspectProcessor::SaveSystemSetting(CSealingInspectSystemSetting* pSystemSetting)
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

	CSealingInspectSystemSetting sysSetting;
	sysSetting = *(pSystemSetting);
	*(m_pSealingInspSystemSetting) = *(pSystemSetting);

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
	const char* sIPPLC1 = W2A(sysSetting.m_sIPPLC1);
	pRoot->first_node("IPPLC1")->value(sIPPLC1);

	const char* sIPPLC2 = W2A(sysSetting.m_sIPPLC2);
	pRoot->first_node("IPPLC2")->value(sIPPLC2);

	const char* sPortPLC1 = W2A(sysSetting.m_sPortPLC1);
	pRoot->first_node("PortPLC1")->value(sPortPLC1);

	const char* sPortPLC2 = W2A(sysSetting.m_sPortPLC2);
	pRoot->first_node("PortPLC2")->value(sPortPLC2);

	const char* sIPLightController1 = W2A(sysSetting.m_sIPLightController1);
	pRoot->first_node("IPLightController1")->value(sIPLightController1);

	const char* sIPLightController2 = W2A(sysSetting.m_sIPLightController2);
	pRoot->first_node("IPLightController2")->value(sIPLightController2);

	const char* sPortLightController1 = W2A(sysSetting.m_sPortLightController1);
	pRoot->first_node("PortLightController1")->value(sPortLightController1);

	const char* sPortLightController2 = W2A(sysSetting.m_sPortLightController2);
	pRoot->first_node("PortLightController2")->value(sPortLightController2);

	CString csSaveFullImage = sysSetting.m_bSaveFullImage == TRUE ? _T("true") : _T("false");
	const char* sSaveFullImage = W2A(csSaveFullImage);
	pRoot->first_node("SaveFullImage")->value(sSaveFullImage);

	CString csSaveDefectImage = sysSetting.m_bSaveDefectImage == TRUE ? _T("true") : _T("false");
	const char* sSaveDefectImage = W2A(csSaveDefectImage);
	pRoot->first_node("SaveDefectImage")->value(sSaveDefectImage);

	CString csShowDetailImage = sysSetting.m_bShowDetailImage == TRUE ? _T("true") : _T("false");
	const char* sShowDetailImage = W2A(csShowDetailImage);
	pRoot->first_node("ShowDetailImage")->value(sShowDetailImage);

	CString csSimulation = sysSetting.m_bSimulation == TRUE ? _T("true") : _T("false");
	const char* sSimulation = W2A(csSimulation);
	pRoot->first_node("Simulation")->value(sSimulation);

	CString csByPass = sysSetting.m_bByPass == TRUE ? _T("true") : _T("false");
	const char* sByPass = W2A(csByPass);
	pRoot->first_node("ByPass")->value(sByPass);

	const char* sFullImagePath = W2A(sysSetting.m_sFullImagePath);
	pRoot->first_node("FullImagePath")->value(sFullImagePath);

	const char* sDefectImagePath = W2A(sysSetting.m_sDefectImagePath);
	pRoot->first_node("DefectImagePath")->value(sDefectImagePath);

	const char* sModelName = W2A(sysSetting.m_sModelName);
	pRoot->first_node("ModelName")->value(sModelName);

	const char* sModelList = W2A(sysSetting.m_sModelList);
	pRoot->first_node("ModelList")->value(sModelList);

	// Go to time
	char sGoToPos1Time_Cavity1[10];
	sprintf_s(sGoToPos1Time_Cavity1, "%d", sysSetting.m_nGoToPos1Time_Cavity1);
	pRoot->first_node("GoToPos1Time_Cavity1")->value(sGoToPos1Time_Cavity1);

	char sGoToPos2Time_Cavity1[10];
	sprintf_s(sGoToPos2Time_Cavity1, "%d", sysSetting.m_nGoToPos2Time_Cavity1);
	pRoot->first_node("GoToPos2Time_Cavity1")->value(sGoToPos2Time_Cavity1);

	char sGoToPos3Time_Cavity1[10];
	sprintf_s(sGoToPos3Time_Cavity1, "%d", sysSetting.m_nGoToPos3Time_Cavity1);
	pRoot->first_node("GoToPos3Time_Cavity1")->value(sGoToPos3Time_Cavity1);

	char sGoToPos4Time_Cavity1[10];
	sprintf_s(sGoToPos4Time_Cavity1, "%d", sysSetting.m_nGoToPos4Time_Cavity1);
	pRoot->first_node("GoToPos4Time_Cavity1")->value(sGoToPos4Time_Cavity1);

	char sGoToPos5Time_Cavity1[10];
	sprintf_s(sGoToPos5Time_Cavity1, "%d", sysSetting.m_nGoToPos5Time_Cavity1);
	pRoot->first_node("GoToPos5Time_Cavity1")->value(sGoToPos5Time_Cavity1);

	char sGoToPos6Time_Cavity1[10];
	sprintf_s(sGoToPos6Time_Cavity1, "%d", sysSetting.m_nGoToPos6Time_Cavity1);
	pRoot->first_node("GoToPos6Time_Cavity1")->value(sGoToPos6Time_Cavity1);

	char sGoToPos7Time_Cavity1[10];
	sprintf_s(sGoToPos7Time_Cavity1, "%d", sysSetting.m_nGoToPos7Time_Cavity1);
	pRoot->first_node("GoToPos7Time_Cavity1")->value(sGoToPos7Time_Cavity1);

	char sGoToPos8Time_Cavity1[10];
	sprintf_s(sGoToPos8Time_Cavity1, "%d", sysSetting.m_nGoToPos8Time_Cavity1);
	pRoot->first_node("GoToPos8Time_Cavity1")->value(sGoToPos8Time_Cavity1);

	char sGoToPos9Time_Cavity1[10];
	sprintf_s(sGoToPos9Time_Cavity1, "%d", sysSetting.m_nGoToPos9Time_Cavity1);
	pRoot->first_node("GoToPos9Time_Cavity1")->value(sGoToPos9Time_Cavity1);

	char sGoToPos10Time_Cavity1[10];
	sprintf_s(sGoToPos10Time_Cavity1, "%d", sysSetting.m_nGoToPos10Time_Cavity1);
	pRoot->first_node("GoToPos10Time_Cavity1")->value(sGoToPos10Time_Cavity1);

	// Offset time
	char sOffsetTime_Pos1_Cavity1[10];
	sprintf_s(sOffsetTime_Pos1_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos1_Cavity1);
	pRoot->first_node("OffsetTime_Pos1_Cavity1")->value(sOffsetTime_Pos1_Cavity1);

	char sOffsetTime_Pos2_Cavity1[10];
	sprintf_s(sOffsetTime_Pos2_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos2_Cavity1);
	pRoot->first_node("OffsetTime_Pos2_Cavity1")->value(sOffsetTime_Pos2_Cavity1);

	char sOffsetTime_Pos3_Cavity1[10];
	sprintf_s(sOffsetTime_Pos3_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos3_Cavity1);
	pRoot->first_node("OffsetTime_Pos3_Cavity1")->value(sOffsetTime_Pos3_Cavity1);

	char sOffsetTime_Pos4_Cavity1[10];
	sprintf_s(sOffsetTime_Pos4_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos4_Cavity1);
	pRoot->first_node("OffsetTime_Pos4_Cavity1")->value(sOffsetTime_Pos4_Cavity1);

	char sOffsetTime_Pos5_Cavity1[10];
	sprintf_s(sOffsetTime_Pos5_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos5_Cavity1);
	pRoot->first_node("OffsetTime_Pos5_Cavity1")->value(sOffsetTime_Pos5_Cavity1);

	char sOffsetTime_Pos6_Cavity1[10];
	sprintf_s(sOffsetTime_Pos6_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos6_Cavity1);
	pRoot->first_node("OffsetTime_Pos6_Cavity1")->value(sOffsetTime_Pos6_Cavity1);

	char sOffsetTime_Pos7_Cavity1[10];
	sprintf_s(sOffsetTime_Pos7_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos7_Cavity1);
	pRoot->first_node("OffsetTime_Pos7_Cavity1")->value(sOffsetTime_Pos7_Cavity1);

	char sOffsetTime_Pos8_Cavity1[10];
	sprintf_s(sOffsetTime_Pos8_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos8_Cavity1);
	pRoot->first_node("OffsetTime_Pos8_Cavity1")->value(sOffsetTime_Pos8_Cavity1);

	char sOffsetTime_Pos9_Cavity1[10];
	sprintf_s(sOffsetTime_Pos9_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos9_Cavity1);
	pRoot->first_node("OffsetTime_Pos9_Cavity1")->value(sOffsetTime_Pos9_Cavity1);

	char sOffsetTime_Pos10_Cavity1[10];
	sprintf_s(sOffsetTime_Pos10_Cavity1, "%d", sysSetting.m_nOffsetTime_Pos10_Cavity1);
	pRoot->first_node("OffsetTime_Pos10_Cavity1")->value(sOffsetTime_Pos10_Cavity1);
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

BOOL CSealingInspectProcessor::SaveLightSetting(CSealingInspectSystemSetting* pSystemSetting, int nLightIdx)
{
	if (m_csLightSettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Light Setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csLightSettingPath);
	if (m_csLightSettingPath.Right(7).CompareNoCase(_T("setting")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Light Setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CSealingInspectSystemSetting sysSetting;
	sysSetting = *(pSystemSetting);
	*(m_pSealingInspSystemSetting) = *(pSystemSetting);

	// convert path
	USES_CONVERSION;
	char chLightSettingPath[1024] = {};
	sprintf_s(chLightSettingPath, "%s", W2A(m_csLightSettingPath));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(chLightSettingPath, std::ios::in | std::ios::out);
	std::string inputXml;
	std::string line;
	while (std::getline(fs, line))
	{
		inputXml += line;
	}
	std::vector<char> buffer(inputXml.begin(), inputXml.end());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = NULL;
	if (nLightIdx == 0) {
		pRoot = xmlDoc.first_node("LightController1");
	}
	else {
		pRoot = xmlDoc.first_node("LightController2");
	}

	// Write data
	const char* sCH1 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH1);
	pRoot->first_node("CH1")->value(sCH1);

	const char* sCH2 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH2);
	pRoot->first_node("CH2")->value(sCH2);

	const char* sCH3 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH3);
	pRoot->first_node("CH3")->value(sCH3);

	const char* sCH4 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH4);
	pRoot->first_node("CH4")->value(sCH4);

	const char* sCH5 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH5);
	pRoot->first_node("CH5")->value(sCH5);

	const char* sCH6 = W2A(sysSetting.m_LightSettings[nLightIdx].m_sCH6);
	pRoot->first_node("CH6")->value(sCH6);

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	std::ofstream file;
	file.open(chLightSettingPath);
	file << data;
	file.close();

	return TRUE;
}

BOOL CSealingInspectProcessor::SaveRecipe(CSealingInspectRecipe* pRecipe, CString sPosCam, int nFrameIdx)
{
	if (pRecipe == NULL)
		return FALSE;

	CConfig recipeFile;

	BOOL bNoFile = FALSE;

	if (recipeFile.SetRegiConfig(NULL, NULL, (TCHAR*)(LPCTSTR)m_csRecipePath, FileMap_Mode) == FALSE)
	{
		CFile pFile;
		pFile.Open(m_csRecipePath, CFile::modeCreate);
		pFile.Close();

		bNoFile = TRUE;
	}

	// Params..
	CString strParameterKey;

	char	strChar[1024] = {};
	CString strValue = _T("");
	int		nValue = 0;

	USES_CONVERSION;

	if (sPosCam.CompareNoCase(_T("TOPCAM")) == 0) {

		if (nFrameIdx == 1) {
			for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("RATIO_PIXEL_UM_TOPCAM"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dRatioPxlUm);
				recipeFile.SetItemValue(i + 1, _T("RATIO_UM_PIXEL_TOPCAM"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dRatioUmPxl);

				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_REFER"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_INNER_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusInner_Min);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_INNER_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusInner_Max);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_OUTER_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusOuter_Min);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_OUTER_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadiusOuter_Max);
				recipeFile.SetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_bUseAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_MINENCLOSING_CIRCLE_ALGORITHMS_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nThresholdBinaryMinEnclosing);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_HOUGHCIRCLE_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_RADIUS_DIFFERENCE_MIN_HOUGHCIRCLE_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceRadiusDiffMin);
				recipeFile.SetItemValue(i + 1, _T("DELTA_RADIUS_OUTER_INNER_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDeltaRadiusOuterInner);
				recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_HORIZONTAL_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIWidth_Hor);
				recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_HORIZONTAL_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIHeight_Hor);
				recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_VERTICAL_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIWidth_Ver);
				recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_VERTICAL_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROIHeight_Ver);
				recipeFile.SetItemValue(i + 1, _T("ROI_12H_OFFSET_X_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI12H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_12H_OFFSET_Y_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI12H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_3H_OFFSET_X_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI3H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_3H_OFFSET_Y_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI3H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_6H_OFFSET_X_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI6H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_6H_OFFSET_Y_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI6H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_9H_OFFSET_X_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI9H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_9H_OFFSET_Y_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nROI9H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_MINENCLOSING_CIRCLE_TOPCAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_MINENCLOSING_CIRCLE_TOPCAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max);
				recipeFile.SetItemValue(i + 1, _T("INCREMENT_ANGLE_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dIncrementAngle);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dThresholdCanny1_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_dThresholdCanny2_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDelayTimeGrab);
				recipeFile.SetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("HOUGHCIRLCLE_PARAM1_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHoughCircleParam1);
				recipeFile.SetItemValue(i + 1, _T("HOUGHCIRLCLE_PARAM2_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHoughCircleParam2);

				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_TOPCAM_FRAME1"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nVMax);

			}
		}
		else if (nFrameIdx == 2) {
			for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("RATIO_PIXEL_UM_TOPCAM"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dRatioPxlUm);
				recipeFile.SetItemValue(i + 1, _T("RATIO_UM_PIXEL_TOPCAM"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dRatioUmPxl);

				recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROIWidth);
				recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROIHeight);
				recipeFile.SetItemValue(i + 1, _T("ROI_1H_OFFSET_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI1H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_1H_OFFSET_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI1H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_5H_OFFSET_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI5H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_5H_OFFSET_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI5H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_7H_OFFSET_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI7H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_7H_OFFSET_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI7H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("ROI_11H_OFFSET_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI11H_OffsetX);
				recipeFile.SetItemValue(i + 1, _T("ROI_11H_OFFSET_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nROI11H_OffsetY);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_REFER"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDelayTimeGrab);

				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_BLOB_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeFindBlob_Min);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_BLOB_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeFindBlob_Max);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_BLOB_WHITE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobWhite);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_MAX_FIND_BLOB_WHITE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_BLOB_BLACK_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobBlack);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_MAX_FIND_BLOB_BLACK_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max);
				recipeFile.SetItemValue(i + 1, _T("MAX_BLOB_COUNT_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nBlobCount_Max);
				recipeFile.SetItemValue(i + 1, _T("BLOB_AREA_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dBlobArea_Min);
				recipeFile.SetItemValue(i + 1, _T("BLOB_AREA_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dBlobArea_Max);
				recipeFile.SetItemValue(i + 1, _T("USE_CHECK_SURFACE_ALGORITHMS_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nUseCheckSurface);

				recipeFile.SetItemValue(i + 1, _T("SELECT_METHOD_FIND_CIRCLE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSelectMethodFindCircle);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_CIRCLE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_FindCircle);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_CIRCLE_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeMin);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_CIRCLE_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSizeMax);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadiusMin);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadiusMax);
				recipeFile.SetItemValue(i + 1, _T("MINDIST_HOUGHCIRCLE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nMinDist_HoughCircle);
				recipeFile.SetItemValue(i + 1, _T("PARAM1_HOUGHCIRCLE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nParam1_HoughCircle);
				recipeFile.SetItemValue(i + 1, _T("PARAM2_HOUGHCIRCLE_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nParam2_HoughCircle);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_1_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshold1_Canny);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_CANNY_2_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThreshold2_Canny);

				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_1H_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_1H_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_5H_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_5H_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_7H_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_7H_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_11H_X_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_MEASURE_POINT_11H_Y_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y);

				recipeFile.SetItemValue(i + 1, _T("WIDTH_ROI_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nWidth_ROIFindSealingOverflow);
				recipeFile.SetItemValue(i + 1, _T("HEIGHT_ROI_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHeight_ROIFindSealingOverflow);

				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_1H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_1H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_1H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Ver);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_1H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Ver);

				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_5H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_5H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_5H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Ver);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_5H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Ver);

				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_7H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_7H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_7H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Ver);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_7H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Ver);

				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_11H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_11H_HOZ_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_X_11H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Ver);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_ROI_FIND_SEALING_OVERFLOW_Y_11H_VER_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Ver);

				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_SEALING_OVERFLOW_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_FindSealingOverflow);
				recipeFile.SetItemValue(i + 1, _T("CONTOUR_SIZE_FIND_SEALING_OVERFLOW_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nContourSize_FindSealingOverflow_Max);
				recipeFile.SetItemValue(i + 1, _T("AREA_CONTOUR_FIND_SEALING_OVERFLOW_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_dAreaContour_FindSealingOverflow_Max);

				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_MEASURE_WIDTH_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nThresholdBinary_MeasureWidth);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_TOPCAM_FRAME2"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nVMax);

			}
		}
	}

	else if (sPosCam.CompareNoCase(_T("SIDECAM")) == 0) {
		switch (nFrameIdx)
		{
		case 1:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
			{
				recipeFile.SetItemValue(i + 1, _T("RATIO_PIXEL_UM_SIDECAM"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dRatioPxlUm);
				recipeFile.SetItemValue(i + 1, _T("RATIO_UM_PIXEL_SIDECAM"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dRatioUmPxl);

				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_REFER"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDelayTimeGrab);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndSearchRangeX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nFindStartEndXThresholdGray);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dThresholdCanny1_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_dThresholdCanny2_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bUseAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nOffetY_Top);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nOffetY_Bottom);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nThresholdBinaryFindROI);

				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nVMax);

				recipeFile.SetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bJustJudgeByMinBoundingRect);
				recipeFile.SetItemValue(i + 1, _T("USE_HARDWARE_TRIGGER_MODE_SIDECAM"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_bUseHardwareTrigger);

				for (int k = 0; k < ROI_PARAMETER_COUNT; k++)
				{
					switch (k) {
					case 0:
						recipeFile.SetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k]);
						break;
					case 1:
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k]);
						break;
					case 2:
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k]);
						break;
					case 3:
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME1"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nROI_Bottom[k]);
						break;
					}
				}
			}
			break;
		case 2:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
			{
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_REFER"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDelayTimeGrab);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndSearchRangeX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nFindStartEndXThresholdGray);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dThresholdCanny1_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_dThresholdCanny2_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_bUseAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nOffetY_Top);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nOffetY_Bottom);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nThresholdBinaryFindROI);

				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nVMax);

				recipeFile.SetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_bJustJudgeByMinBoundingRect);

				for (int k = 0; k < ROI_PARAMETER_COUNT; k++)
				{
					switch (k) {
					case 0:
						recipeFile.SetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k]);
						break;
					case 1:
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k]);
						break;
					case 2:
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k]);
						break;
					case 3:
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME2"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nROI_Bottom[k]);
						break;
					}
				}
			}
			break;
		case 3:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
			{
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_REFER"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDelayTimeGrab);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndSearchRangeX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nFindStartEndXThresholdGray);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dThresholdCanny1_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_dThresholdCanny2_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_bUseAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nOffetY_Top);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nOffetY_Bottom);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nThresholdBinaryFindROI);

				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nVMax);

				recipeFile.SetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_bJustJudgeByMinBoundingRect);

				for (int k = 0; k < ROI_PARAMETER_COUNT; k++)
				{
					switch (k) {
					case 0:
						recipeFile.SetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k]);
						break;
					case 1:
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k]);
						break;
					case 2:
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k]);
						break;
					case 3:
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME3"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nROI_Bottom[k]);
						break;
					}
				}
			}
			break;
		case 4:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
			{
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_REFER"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("DELAY_TIME_GRAB_IMAGE_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDelayTimeGrab);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_X_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_LINE_SEARCH_RANGE_X_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndSearchRangeX);
				recipeFile.SetItemValue(i + 1, _T("FIND_START_END_THRESHOLD_GRAY_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nFindStartEndXThresholdGray);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_1_MAKEROI_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dThresholdCanny1_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_CANNY_2_MAKEROI_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_dThresholdCanny2_MakeROI);
				recipeFile.SetItemValue(i + 1, _T("USE_ADVANCED_ALGORITHMS_INSPECTION_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_bUseAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("NUMBER_OF_DISTANCE_NG_MAX_COUNT_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("USE_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_TOP_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nOffetY_Top);
				recipeFile.SetItemValue(i + 1, _T("OFFSET_Y_BOTTOM_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nOffetY_Bottom);
				recipeFile.SetItemValue(i + 1, _T("THRESHOLD_BINARY_FIND_ROI_ADVANCED_ALGORITHMS_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nThresholdBinaryFindROI);

				recipeFile.SetItemValue(i + 1, _T("HSV_H_MIN_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nHMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_H_MAX_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nHMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MIN_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nSMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_S_MAX_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nSMax);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MIN_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nVMin);
				recipeFile.SetItemValue(i + 1, _T("HSV_V_MAX_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nVMax);

				recipeFile.SetItemValue(i + 1, _T("JUST_JUDGE_BY_MIN_BOUNDING_RECT_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_bJustJudgeByMinBoundingRect);

				for (int k = 0; k < ROI_PARAMETER_COUNT; k++)
				{
					switch (k) {
					case 0:
						recipeFile.SetItemValue(i + 1, _T("ROI_X_TOP_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_X_BOTTOM_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k]);
						break;
					case 1:
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_TOP_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_Y_BOTTOM_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k]);
						break;
					case 2:
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_TOP_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_WIDTH_BOTTOM_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k]);
						break;
					case 3:
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_TOP_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Top[k]);
						recipeFile.SetItemValue(i + 1, _T("ROI_HEIGHT_BOTTOM_SIDECAM_FRAME4"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nROI_Bottom[k]);
						break;
					}
				}
			}
			break;
		}
	}

	*(m_pSealingInspRecipe) = *pRecipe;

	recipeFile.WriteToFile();
}

BOOL CSealingInspectProcessor::ReloadSystemSetting()
{
	if (m_pSealingInspSystemSetting == NULL)
		return FALSE;

	BOOL bRet = LoadSystemSetting(m_pSealingInspSystemSetting);
	return bRet;
}

BOOL CSealingInspectProcessor::ReloadRecipe()
{
	if (m_pSealingInspRecipe == NULL)
		return FALSE;

	BOOL bRet = LoadRecipe(m_pSealingInspRecipe);
	return bRet;
}

BOOL CSealingInspectProcessor::InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL bSimulator)
{
	// NUMBER_OF_SET_INSPECT = 2
	int nCoreIdx = 0;
	int nTopCamIdx = 0;
	int nSideCamIdx = 0;
	int nThreadIdx = 0;
	emInspectCavity nInspCav = emUNKNOWN;

	switch (nInspCavity)
	{
	case emInspectCavity_Cavity1:

		nTopCamIdx = 0;
		nSideCamIdx = 2;
		nCoreIdx = 0;
		nInspCav = emInspectCavity_Cavity1;
		break;
	case emInspectCavity_Cavity2:
		nTopCamIdx = 1;
		nSideCamIdx = 3;
		nCoreIdx = 1;
		nInspCav = emInspectCavity_Cavity2;
		break;
	}

	// start grabbing top cam 1 and side cam 1
	m_pSealingInspHikCam->StartGrab(nTopCamIdx);
	m_pSealingInspHikCam->StartGrab(nSideCamIdx);

	// create thread inspect cavity 1
	m_pSealingInspCore[nCoreIdx]->SetCoreIndex(nCoreIdx);
	m_pSealingInspCore[nCoreIdx]->SetSimulatorMode(bSimulator);

	m_pSealingInspCore[nCoreIdx]->CreateInspectThread(nThreadCount, nInspCav);

	/*if (m_pSealingInspCore[nCoreIdx]->GetWorkThreadArray(nThreadIdx) == NULL) {
		m_pSealingInspCore[nCoreIdx]->CreateInspectThread(nThreadCount, nInspCav);
	}
	else {
		m_pSealingInspCore[nCoreIdx]->StartThread(nThreadCount, nInspCav);
	}*/
	
	m_pSealingInspStatus[nCoreIdx]->m_bInspRunning = TRUE;

	return TRUE;
}

BOOL CSealingInspectProcessor::InspectStop(emInspectCavity nInspCavity)
{
	int nCoreIdx = 0;

	if (nInspCavity == emInspectCavity_Cavity1)
		nCoreIdx = 0;
	else if (nInspCavity == emInspectCavity_Cavity2)
		nCoreIdx = 1;
	else {
		nCoreIdx = -1;
		return FALSE;
	}
	m_pSealingInspCore[nCoreIdx]->StopThread();
	m_pSealingInspect_Simulation_IO[nCoreIdx]->m_bLOCK_PROCESS = TRUE;

	m_pSealingInspStatus[nCoreIdx]->m_bInspRunning = FALSE;
	m_pSealingInspCore[nCoreIdx]->DeleteInspectThread();

	// Stop Grabbing
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		m_pSealingInspHikCam->StopGrab(i);
	}
	return TRUE;
}

BOOL CSealingInspectProcessor::TestInspectCavity1()
{
	int nTopCamIdx = 0;
	int nSideCamIdx = 2;
	int nCoreIdx = 0;

	// start grabbing top cam 1 and side cam 1
	m_pSealingInspHikCam->StartGrab(nTopCamIdx);
	m_pSealingInspHikCam->StartGrab(nSideCamIdx);

	// create thread inspect cavity 1
	m_pSealingInspCore[nCoreIdx]->SetCoreIndex(nCoreIdx);
	m_pSealingInspCore[nCoreIdx]->TestInspectCavity1(nCoreIdx);

	return TRUE;
}

BOOL CSealingInspectProcessor::TestInspectCavity2()
{
	int nTopCamIdx = 1;
	int nSideCamIdx = 3;
	int nCoreIdx = 1;

	// start grabbing top cam 2 and side cam 2
	m_pSealingInspHikCam->StartGrab(nTopCamIdx);
	m_pSealingInspHikCam->StartGrab(nSideCamIdx);

	// create thread inspect cavity 1
	m_pSealingInspCore[nCoreIdx]->SetCoreIndex(nCoreIdx);
	m_pSealingInspCore[nCoreIdx]->TestInspectCavity2(nCoreIdx);

	return TRUE;
}

BOOL CSealingInspectProcessor::Inspect_TopCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame)
{
	if (m_pSealingInspCore[nCoreIdx] == NULL)
		return FALSE;

	m_pSealingInspCore[nCoreIdx]->Inspect_TopCam_Simulation(nCoreIdx, nCamIdx, nFrame);

	return TRUE;
}

BOOL CSealingInspectProcessor::Inspect_SideCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame)
{
	if (m_pSealingInspCore[nCoreIdx] == NULL)
		return FALSE;

	m_pSealingInspCore[nCoreIdx]->Inspect_SideCam_Simulation(nCoreIdx, nCamIdx, nFrame);

	return TRUE;
}

BOOL CSealingInspectProcessor::TestTCPSocket()
{
	char strIP[15] = { "192.168.0.2" };
	int nPort = 40001;

	char strSendTo[10] = { "@00F25582" };
	//sprintf_s(strSendTo, "%s", "@00F25582");

	CTCPSocket::CONNECTRESULT connResult = m_pTcpSocket->Connect(strIP, nPort);
	if (connResult == CTCPSocket::CONNECTRESULT_CONNECT_SUCCESS) {
		AfxMessageBox(_T("Connect success"));
	}
	else
	{
		AfxMessageBox(_T("connect fail"));
	}

	m_pTcpSocket->Send(strSendTo, 10);

	return TRUE;
}

void CSealingInspectProcessor::TestTcpSocketCallback(char* pMsg, int nMsglen, void* param)
{
	CSealingInspectProcessor* pThis = (CSealingInspectProcessor*)param;
	pThis->TestTcpSocketCallbackEx(pMsg, nMsglen);
}

void CSealingInspectProcessor::TestTcpSocketCallbackEx(char* pMsg, int nMsglen)
{
	AfxMessageBox((CString)pMsg);
}

#pragma region Offine Simulation
LPBYTE CSealingInspectProcessor::GetBufferImage_SIDE(int nBuff, int nFrame)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return NULL;

	return m_pImageBuffer_Side[nBuff]->GetFrameImage(nFrame);
}
LPBYTE CSealingInspectProcessor::GetBufferImage_TOP(int nBuff, int nFrame)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return NULL;

	return m_pImageBuffer_Top[nBuff]->GetFrameImage(nFrame);
}

BOOL CSealingInspectProcessor::LoadImageBuffer_SIDE(int nBuff, int nFrame, CString strFilePath)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer_Side[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer_Side[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer_Side[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBuffer_Side[nBuff]->GetFrameSize();

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

	m_pImageBuffer_Side[nBuff]->SetFrameImage(nFrame, pOpenImage.data);

	return TRUE;
}
BOOL CSealingInspectProcessor::LoadImageBuffer_TOP(int nBuff, int nFrame, CString strFilePath)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer_Top[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer_Top[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer_Top[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBuffer_Top[nBuff]->GetFrameSize();

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

	m_pImageBuffer_Top[nBuff]->SetFrameImage(nFrame, pOpenImage.data);

	return TRUE;
}

LPBYTE CSealingInspectProcessor::GetResultBuffer_SIDE(int nBuff, int nFrame)
{
	if (m_pResultBuffer_Side[nBuff] == NULL)
		return NULL;

	return m_pResultBuffer_Side[nBuff]->GetFrameImage(nFrame);
}

LPBYTE CSealingInspectProcessor::GetResultBuffer_TOP(int nBuff, int nFrame)
{
	if (m_pResultBuffer_Top[nBuff] == NULL)
		return NULL;

	return m_pResultBuffer_Top[nBuff]->GetFrameImage(nFrame);
}

LPBYTE CSealingInspectProcessor::GetBufferImageHikCam(int nCamIdx)
{
	if (m_pSealingInspHikCam == NULL)
		return NULL;


	LPBYTE pImageBuff = m_pSealingInspHikCam->GetBufferImage(nCamIdx);

	if (nCamIdx == 0 || nCamIdx == 1)
	{
		cv::Mat matBayer(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1, pImageBuff);

		cv::cvtColor(matBayer, m_matBGR, cv::COLOR_BayerBG2BGR);

		return m_matBGR.data;
	}

	return pImageBuff;
}

BOOL CSealingInspectProcessor::SaveImageHikCam(int nCamIdx, CString strImageSavePath)
{
	if (m_pSealingInspHikCam == NULL)
		return FALSE;

	USES_CONVERSION;
	char chSavePath[1000] = {};
	sprintf_s(chSavePath, "%s", W2A(strImageSavePath));

	if (nCamIdx == 0 || nCamIdx == 1)
	{
		cv::Mat matBayer(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1, m_pSealingInspHikCam->GetBufferImage(nCamIdx));
		cv::Mat matSave(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);
		cv::cvtColor(matBayer, matSave, cv::COLOR_BayerBG2BGR);

		cv::imwrite(chSavePath, matSave);
		return TRUE;
	}

	cv::Mat matSave(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, m_pSealingInspHikCam->GetBufferImage(nCamIdx));
	cv::imwrite(chSavePath, matSave);

	return TRUE;
}

BOOL CSealingInspectProcessor::LoadAllImageBuffer(CString strDirPath, CString strImageType)
{
	if (strDirPath.IsEmpty() == TRUE)
		return FALSE;

	for (int nSideIdx = 0; nSideIdx < MAX_SIDECAM_COUNT; nSideIdx++) {

		for (int nFrame = 0; nFrame < MAX_IMAGE_BUFFER_SIDECAM; nFrame++)
		{
			CString strExt = strImageType;
			strExt.MakeUpper();

			if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")))
				continue;

			CString strImagePath;

			strImagePath.Format(_T("%s\\%s%d_%s%d.%s"), strDirPath, _T("SideCam"), (nSideIdx + 1), _T("Frame"), (nFrame + 1), strImageType);

			LoadImageBuffer_SIDE(nSideIdx, nFrame, strImagePath);
		}
	}

	for (int nTopIdx = 0; nTopIdx < MAX_TOPCAM_COUNT; nTopIdx++) {

		for (int nFrame = 0; nFrame < MAX_IMAGE_BUFFER_TOPCAM; nFrame++)
		{
			CString strExt = strImageType;
			strExt.MakeUpper();

			if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")))
				continue;

			CString strImagePath;

			strImagePath.Format(_T("%s\\%s%d_%s%d.%s"), strDirPath, _T("TopCam"), (nTopIdx + 1), _T("Frame"), (nFrame + 1), strImageType);

			LoadImageBuffer_TOP(nTopIdx, nFrame, strImagePath);
		}
	}
}

BOOL CSealingInspectProcessor::CreateBuffer_SIDE()
{
	BOOL bRetValue_Side = FALSE;

	DWORD dwFrameWidth_Side = (DWORD)FRAME_WIDTH_SIDECAM;
	DWORD dwFrameHeight_Side = (DWORD)FRAME_HEIGHT_SIDECAM;
	DWORD dwFrameCount_Side = 0;
	DWORD dwFrameSize_Side = dwFrameWidth_Side * dwFrameHeight_Side * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
	{
		if (m_pImageBuffer_Side[i] != NULL)
		{
			m_pImageBuffer_Side[i]->DeleteSharedMemory();
			delete m_pImageBuffer_Side[i];
			m_pImageBuffer_Side[i] = NULL;
		}

		m_pImageBuffer_Side[i] = new CSharedMemoryBuffer;

		dwFrameCount_Side = (DWORD)MAX_IMAGE_BUFFER_SIDECAM;

		dwTotalFrameCount += dwFrameCount_Side;

		m_pImageBuffer_Side[i]->SetFrameWidth(dwFrameWidth_Side);
		m_pImageBuffer_Side[i]->SetFrameHeight(dwFrameHeight_Side);
		m_pImageBuffer_Side[i]->SetFrameCount(dwFrameCount_Side);
		m_pImageBuffer_Side[i]->SetFrameSize(dwFrameSize_Side);

		DWORD64 dw64Size_Side = (DWORD64)dwFrameCount_Side * dwFrameSize_Side;

		CString strMemory_Side;
		strMemory_Side.Format(_T("%s_%d"), "BufferOffline_Color_Side", i);

		bRetValue_Side = m_pImageBuffer_Side[i]->CreateSharedMemory(strMemory_Side, dw64Size_Side);

		if (bRetValue_Side == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Side * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}
BOOL CSealingInspectProcessor::CreateBuffer_TOP()
{
	BOOL bRetValue_Top = FALSE;

	DWORD dwFrameWidth_Top = (DWORD)FRAME_WIDTH_TOPCAM;
	DWORD dwFrameHeight_Top = (DWORD)FRAME_HEIGHT_TOPCAM;
	DWORD dwFrameCount_Top = 0;
	DWORD dwFrameSize_Top = dwFrameWidth_Top * dwFrameHeight_Top * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++)
	{
		if (m_pImageBuffer_Top[i] != NULL)
		{
			m_pImageBuffer_Top[i]->DeleteSharedMemory();
			delete m_pImageBuffer_Top[i];
			m_pImageBuffer_Top[i] = NULL;
		}

		m_pImageBuffer_Top[i] = new CSharedMemoryBuffer;

		dwFrameCount_Top = (DWORD)MAX_IMAGE_BUFFER_TOPCAM;

		dwTotalFrameCount += dwFrameCount_Top;

		m_pImageBuffer_Top[i]->SetFrameWidth(dwFrameWidth_Top);
		m_pImageBuffer_Top[i]->SetFrameHeight(dwFrameHeight_Top);
		m_pImageBuffer_Top[i]->SetFrameCount(dwFrameCount_Top);
		m_pImageBuffer_Top[i]->SetFrameSize(dwFrameSize_Top);

		DWORD64 dw64Size_Top = (DWORD64)dwFrameCount_Top * dwFrameSize_Top;

		CString strMemory_Top;
		strMemory_Top.Format(_T("%s_%d"), "BufferOffline_Color_Top", i);

		bRetValue_Top = m_pImageBuffer_Top[i]->CreateSharedMemory(strMemory_Top, dw64Size_Top);

		if (bRetValue_Top == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Top * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

BOOL CSealingInspectProcessor::CreateResultBuffer_SIDE()
{
	BOOL bRetValue_Side = FALSE;

	DWORD dwFrameWidth_Side = (DWORD)FRAME_WIDTH_SIDECAM;
	DWORD dwFrameHeight_Side = (DWORD)FRAME_HEIGHT_SIDECAM;
	DWORD dwFrameCount_Side = 0;
	DWORD dwFrameSize_Side = dwFrameWidth_Side * dwFrameHeight_Side * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
	{
		if (m_pResultBuffer_Side[i] != NULL)
		{
			m_pResultBuffer_Side[i]->DeleteSharedMemory();
			delete m_pResultBuffer_Side[i];
			m_pResultBuffer_Side[i] = NULL;
		}

		m_pResultBuffer_Side[i] = new CSharedMemoryBuffer;

		dwFrameCount_Side = (DWORD)MAX_IMAGE_BUFFER_SIDECAM;

		dwTotalFrameCount += dwFrameCount_Side;

		m_pResultBuffer_Side[i]->SetFrameWidth(dwFrameWidth_Side);
		m_pResultBuffer_Side[i]->SetFrameHeight(dwFrameHeight_Side);
		m_pResultBuffer_Side[i]->SetFrameCount(dwFrameCount_Side);
		m_pResultBuffer_Side[i]->SetFrameSize(dwFrameSize_Side);

		DWORD64 dw64Size_Side = (DWORD64)dwFrameCount_Side * dwFrameSize_Side;

		CString strMemory_Side;
		strMemory_Side.Format(_T("%s_%d"), "BufferOffline_Color_Side", i);

		bRetValue_Side = m_pResultBuffer_Side[i]->CreateSharedMemory(strMemory_Side, dw64Size_Side);

		if (bRetValue_Side == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Side * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

BOOL CSealingInspectProcessor::CreateResultBuffer_TOP()
{
	BOOL bRetValue_Top = FALSE;

	DWORD dwFrameWidth_Top = (DWORD)FRAME_WIDTH_TOPCAM;
	DWORD dwFrameHeight_Top = (DWORD)FRAME_HEIGHT_TOPCAM;
	DWORD dwFrameCount_Top = 0;
	DWORD dwFrameSize_Top = dwFrameWidth_Top * dwFrameHeight_Top * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++)
	{
		if (m_pResultBuffer_Top[i] != NULL)
		{
			m_pResultBuffer_Top[i]->DeleteSharedMemory();
			delete m_pResultBuffer_Top[i];
			m_pResultBuffer_Top[i] = NULL;
		}

		m_pResultBuffer_Top[i] = new CSharedMemoryBuffer;

		dwFrameCount_Top = (DWORD)MAX_IMAGE_BUFFER_TOPCAM;

		dwTotalFrameCount += dwFrameCount_Top;

		m_pResultBuffer_Top[i]->SetFrameWidth(dwFrameWidth_Top);
		m_pResultBuffer_Top[i]->SetFrameHeight(dwFrameHeight_Top);
		m_pResultBuffer_Top[i]->SetFrameCount(dwFrameCount_Top);
		m_pResultBuffer_Top[i]->SetFrameSize(dwFrameSize_Top);

		DWORD64 dw64Size_Top = (DWORD64)dwFrameCount_Top * dwFrameSize_Top;

		CString strMemory_Top;
		strMemory_Top.Format(_T("%s_%d"), "BufferOffline_Color_Top", i);

		bRetValue_Top = m_pResultBuffer_Top[i]->CreateSharedMemory(strMemory_Top, dw64Size_Top);

		if (bRetValue_Top == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Top * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

void CSealingInspectProcessor::MakeDirectory()
{
	CString strFullImagePath = m_pSealingInspSystemSetting->m_sFullImagePath;
	CString strDefectImagePath = m_pSealingInspSystemSetting->m_sDefectImagePath;

	CString strSealingId = _T("Sealing");

	m_strFullImagePath.Format(_T("%s\\%04d_%02d_%02d\\%s_%02d%02d%02d\\"), strFullImagePath, m_timeLoadingTime.GetYear(), m_timeLoadingTime.GetMonth(), m_timeLoadingTime.GetDay(), strSealingId, m_timeLoadingTime.GetHour(), m_timeLoadingTime.GetMinute(), m_timeLoadingTime.GetSecond());
	CheckDirectory(m_strFullImagePath);

	m_strDefectImagePath.Format(_T("%s\\%04d_%02d_%02d\\%s_%02d%02d%02d\\"), strDefectImagePath, m_timeLoadingTime.GetYear(), m_timeLoadingTime.GetMonth(), m_timeLoadingTime.GetDay(), strSealingId, m_timeLoadingTime.GetHour(), m_timeLoadingTime.GetMinute(), m_timeLoadingTime.GetSecond());
	CheckDirectory(m_strDefectImagePath);
}

BOOL CSealingInspectProcessor::CheckDirectory(const TCHAR szPathName[], BOOL bDelete)
{
	CFileFind finder;
	CString strTemp;
	CString strDir = szPathName;
	int nPos;

	BOOL bExist = finder.FindFile(szPathName);

	if (bDelete == TRUE)
	{
		DeleteFolder(szPathName);
		bExist = FALSE;
	}

	if (bExist == FALSE)
	{
		nPos = strDir.Find(_T("\\"));
		nPos = strDir.Find(_T("\\"), nPos + 1);
		while (nPos > 0)
		{
			strTemp = strDir.Mid(0, nPos);
			if (0 < strTemp.GetLength())
			{
				if (CString("\\") == strTemp.GetAt(strTemp.GetLength() - 1))
				{
					nPos = strDir.Find(_T("\\"), nPos + 1);
					continue;
				}
			}
			if (finder.FindFile(strTemp) == FALSE)
			{
				if (::CreateDirectory(strTemp, NULL) == FALSE)
				{
					strTemp.Format(_T("[%s]Folder Create Fail. "), szPathName);

					//LogMessage(strTemp, 2);
					return FALSE;
				}
			}
			nPos = strDir.Find(_T("\\"), nPos + 1);
		}
	}
	return TRUE;
}

BOOL CSealingInspectProcessor::DeleteFolder(const CString strFolder)
{
	SHFILEOPSTRUCT FileOp = { 0 };
	TCHAR szTemp[MAX_PATH];

	wcscpy_s(szTemp, MAX_PATH, strFolder);
	szTemp[strFolder.GetLength() + 1] = NULL;

	FileOp.hwnd = NULL;
	FileOp.wFunc = FO_DELETE;
	FileOp.pFrom = NULL;
	FileOp.pTo = NULL;
	FileOp.fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI;
	FileOp.fAnyOperationsAborted = false;
	FileOp.hNameMappings = NULL;
	FileOp.lpszProgressTitle = NULL;
	FileOp.pFrom = szTemp;

	SHFileOperation(&FileOp);

	return TRUE;
}

void CSealingInspectProcessor::SetCavityInfo(CString strLoadingTime)
{
}

BOOL CSealingInspectProcessor::ClearBufferImage_SIDE(int nBuff)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	BOOL nRet = FALSE;
	nRet = m_pImageBuffer_Side[nBuff]->ClearBufferImage();

	return nRet;
}
BOOL CSealingInspectProcessor::ClearBufferImage_TOP(int nBuff)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	BOOL nRet = FALSE;
	nRet = m_pImageBuffer_Top[nBuff]->ClearBufferImage();

	return nRet;
}

BOOL CSealingInspectProcessor::SetResultBuffer_TOP(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pResultBuffer_Top[nBuff] == NULL)
		return FALSE;

	return m_pResultBuffer_Top[nBuff]->SetFrameImage(nFrame, buff);
}

BOOL CSealingInspectProcessor::SetResultBuffer_SIDE(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pResultBuffer_Side[nBuff] == NULL)
		return FALSE;

	return m_pResultBuffer_Side[nBuff]->SetFrameImage(nFrame, buff);
}

#pragma endregion

void CSealingInspectProcessor::RegCallbackLogFunc(CallbackLogFunc* pFunc)
{
	m_pCallbackLogFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackAlarm(CallbackAlarm* pFunc)
{
	m_pCallbackAlarm = pFunc;
}

void CSealingInspectProcessor::RegCallbackInsCavity1completeFunc(CallbackInspectCavity1Complete* pFunc)
{
	m_pCallbackInsCavity1CompleteFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackInsCavity2completeFunc(CallbackInspectCavity2Complete* pFunc)
{
	m_pCallbackInsCavity2CompleteFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackInsTopCam1CompleteFunc(CallbackInspectTopCam1Complete* pFunc)
{
	m_pCallbackInsTopCam1CompleteFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackInsTopCam2CompleteFunc(CallbackInspectTopCam2Complete* pFunc)
{
	m_pCallbackInsTopCam2CompleteFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackGrabFrameSideCam1CompleteFunc(CallbackGrabFrameSideCam1Complete* pFunc)
{
	m_pCallbackGrabFrameSideCam1CompleteFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackGrabFrameSideCam2CompleteFunc(CallbackGrabFrameSideCam2Complete* pFunc)
{
	m_pCallbackGrabFrameSideCam2CompleteFunc = pFunc;
}

BOOL CSealingInspectProcessor::SetSealingInspectSimulationIO(int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO)
{
	if (m_pSealingInspCore[nCoreIdx] == NULL)
		return FALSE;

	CSingleLock localLock(&m_csSimulation_IO[nCoreIdx]);
	localLock.Lock();

	*(m_pSealingInspect_Simulation_IO[nCoreIdx]) = *(sealingInspSimulationIO);

	localLock.Unlock();

	return TRUE;
}

void CSealingInspectProcessor::SetProcessStatus1(BOOL bProcessStatus1)
{
	CSingleLock localLock(&m_csProcessStatus1);
	localLock.Lock();
	m_bProcessStatus1 = bProcessStatus1;
	localLock.Unlock();
	return;
}

void CSealingInspectProcessor::SetProcessStatus2(BOOL bProcessStatus2)
{
	CSingleLock localLock(&m_csProcessStatus2);
	localLock.Lock();
	m_bProcessStatus2 = bProcessStatus2;
	localLock.Unlock();
	return;
}

void CSealingInspectProcessor::SetGrabFrameSideCam(int nCoreIdx, BOOL bGrab)
{
	CSingleLock localLock(&m_csProcessStatus1);
	localLock.Lock();

	m_bGrabFrameSideCam[nCoreIdx] = bGrab;

	localLock.Unlock();
	return;
}

void CSealingInspectProcessor::InspectCavity1Complete(BOOL bSetting)
{
	if (m_pCallbackInsCavity1CompleteFunc == NULL)
		return;

	(m_pCallbackInsCavity1CompleteFunc)(bSetting);
}

void CSealingInspectProcessor::InspectCavity2Complete(BOOL bSetting)
{
	if (m_pCallbackInsCavity2CompleteFunc == NULL)
		return;

	(m_pCallbackInsCavity2CompleteFunc)(bSetting);
}

void CSealingInspectProcessor::InspectTopCam1Complete(BOOL bSetting)
{
	if (m_pCallbackInsTopCam1CompleteFunc == NULL)
		return;

	(m_pCallbackInsTopCam1CompleteFunc)(bSetting);
}

void CSealingInspectProcessor::InspectTopCam2Complete(BOOL bSetting)
{
	if (m_pCallbackInsTopCam2CompleteFunc == NULL)
		return;

	(m_pCallbackInsTopCam2CompleteFunc)(bSetting);
}

void CSealingInspectProcessor::GrabFrameSideCam1Complete(BOOL bSetting)
{
	if (m_pCallbackGrabFrameSideCam1CompleteFunc == NULL)
		return;

	(m_pCallbackGrabFrameSideCam1CompleteFunc)(bSetting);
}

void CSealingInspectProcessor::GrabFrameSideCam2Complete(BOOL bSetting)
{
	if (m_pCallbackGrabFrameSideCam2CompleteFunc == NULL)
		return;

	(m_pCallbackGrabFrameSideCam2CompleteFunc)(bSetting);
}

BOOL CSealingInspectProcessor::GetInspectionResult(int nCoreIdx, CSealingInspectResult* pSealingInspRes)
{
	CSingleLock localLock(&m_csInspResult[nCoreIdx]);
	localLock.Lock();
	*(pSealingInspRes) = *(m_pSealingInspResult[nCoreIdx]);

	localLock.Unlock();
	return TRUE;
}

BOOL CSealingInspectProcessor::GetInspectStatus(int nCavity, CSealingInspectStatus* pInspStatus)
{
	if (m_pSealingInspStatus[nCavity] == NULL)
		return FALSE;

	*pInspStatus = *m_pSealingInspStatus[nCavity];

	return TRUE;
}

void CSealingInspectProcessor::LogMessage(char* strMessage)
{
	if (strMessage == NULL)
		return;

	CString strLogMessage = (CString)strMessage;

	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strLogMessage);
}

void CSealingInspectProcessor::LogMessage(CString strMessage)
{
	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strMessage);
}

void CSealingInspectProcessor::ShowLogView(BOOL bShow)
{
	if (m_pLogView == NULL)
		return;

#ifndef _DEBUG
	m_pLogView->ShowMode(bShow);
#endif // !_DEBUG
}

void CSealingInspectProcessor::SystemMessage(CString strMessage)
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
