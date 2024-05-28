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

	// 1. Create Image Buffer Color..
	if (CreateBuffer_SIDE() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// 2. Create Image Buffer Mono..
	if (CreateBuffer_TOP() == FALSE)
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

	// simulation IO
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspect_Simulation_IO[i] != NULL)
			delete m_pSealingInspect_Simulation_IO[i], m_pSealingInspect_Simulation_IO[i] = NULL;
		m_pSealingInspect_Simulation_IO[i] = new CSealingInspect_Simulation_IO;
	}

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
	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++) {
		delete m_pImageBuffer_Side[i], m_pImageBuffer_Side[i] = NULL;
	}
	for (int i = 0; i < MAX_IMAGE_BUFFER_TOPCAM; i++) {
		delete m_pImageBuffer_Top[i], m_pImageBuffer_Top[i] = NULL;
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

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME2_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadius_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadius_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_FRAME2_MIN"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadius_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("RADIUS_FRAME2_MAX"), readRecipe.m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadius_Max, 0);
	}
	for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
	{
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDistanceMeasurementTolerance_Max, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MIN"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDistanceMeasurementTolerance_Min, 0);
		recipeFile.GetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MAX"), readRecipe.m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDistanceMeasurementTolerance_Max, 0);
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
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadius_Min);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame1.m_nRadius_Max);
			}
		}
		else if (nFrameIdx == 2) {
			for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_TOPCAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadius_Min);
				recipeFile.SetItemValue(i + 1, _T("RADIUS_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_TopCam[i].m_recipeFrame2.m_nRadius_Max);
			}
		}
	}

	else if (sPosCam.CompareNoCase(_T("SIDECAM")) == 0) {
		switch (nFrameIdx)
		{
		case 1:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME1_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
			}
			break;
		case 2:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME2_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
			}
			break;
		case 3:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME3_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame3.m_nDistanceMeasurementTolerance_Max);
			}
			break;
		case 4:
			for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MIN"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDistanceMeasurementTolerance_Min);
				recipeFile.SetItemValue(i + 1, _T("DISTANCE_MEASUREMENT_TOLERANCE_SIDECAM_FRAME4_MAX"), pRecipe->m_sealingInspRecipe_SideCam[i].m_recipeFrame4.m_nDistanceMeasurementTolerance_Max);
			}
			break;
		}
	}


	*(m_pSealingInspRecipe) = *pRecipe;

	recipeFile.WriteToFile();
}

BOOL CSealingInspectProcessor::InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL bSimulator)
{
	// NUMBER_OF_SET_INSPECT = 2
	int nCoreIdx = 0;
	int nTopCamIdx = 0;
	int nSideCamIdx = 0;
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

void CSealingInspectProcessor::MakeDirectory()
{
	CString strFullImagePath = m_pSealingInspSystemSetting->m_sFullImagePath;
	CString strDefectImagePath = m_pSealingInspSystemSetting->m_sDefectImagePath;

	CString strSealingId = _T("Sealing");

	m_strFullImagePath.Format(_T("%s\\%04d_%02d_%02d\\%s_%02d%02d%02d\\"),strFullImagePath, m_timeLoadingTime.GetYear(), m_timeLoadingTime.GetMonth(), m_timeLoadingTime.GetDay(), strSealingId, m_timeLoadingTime.GetHour(), m_timeLoadingTime.GetMinute(), m_timeLoadingTime.GetSecond());
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

BOOL CSealingInspectProcessor::SetTopCamResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	return m_pImageBuffer_Top[nBuff]->SetFrameImage(nFrame, buff);
}

BOOL CSealingInspectProcessor::SetSideCamResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	return m_pImageBuffer_Side[nBuff]->SetFrameImage(nFrame, buff);
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

void CSealingInspectProcessor::RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}

BOOL CSealingInspectProcessor::SetSealingInspectSimulationIO(int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO)
{
	CSingleLock localLock(&m_csSimulation_IO[nCoreIdx]);
	localLock.Lock();

	*(m_pSealingInspect_Simulation_IO[nCoreIdx]) = *(sealingInspSimulationIO);

	localLock.Unlock();

	return TRUE;
}

void CSealingInspectProcessor::InspectComplete(emInspectCavity nSetInsp)
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	(m_pCallbackInsCompleteFunc)(nSetInsp);
}

BOOL CSealingInspectProcessor::GetInspectionResult(int nCoreIdx, CSealingInspectResult* pSealingInspRes)
{
	CSingleLock localLock(&m_csInspResult[nCoreIdx]);
	localLock.Lock();
	*(pSealingInspRes) = *(m_pSealingInspResult[nCoreIdx]);

	localLock.Unlock();
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
