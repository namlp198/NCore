#include "pch.h"
#include "ReadCodeProcessor.h"

CReadCodeProcessor::CReadCodeProcessor()
{
	m_csSysSettingsPath = GetCurrentPathApp() + _T("Settings\\SystemSettings.config");
	m_csCam1SettingPath = GetCurrentPathApp() + _T("Settings\\Camera1Settings.config");
}

CReadCodeProcessor::~CReadCodeProcessor()
{
	Destroy();
}

BOOL CReadCodeProcessor::Initialize()
{
	// 1. Load System Setting
	if (m_pReadCodeSystemSetting != NULL)
		delete m_pReadCodeSystemSetting, m_pReadCodeSystemSetting = NULL;
	m_pReadCodeSystemSetting = new CReadCodeSystemSetting;
	LoadSystemSettings(m_pReadCodeSystemSetting);

	// 2. Camera Setting
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pReadCodeCameraSetting[i] != NULL)
			delete m_pReadCodeCameraSetting[i], m_pReadCodeCameraSetting[i] = NULL;
		m_pReadCodeCameraSetting[i] = new CReadCodeCameraSetting;
		LoadCameraSettings(m_pReadCodeCameraSetting[i]);
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
	if (m_pReadCodeRecipe != NULL)
		delete m_pReadCodeRecipe, m_pReadCodeRecipe = NULL;
	m_pReadCodeRecipe = new CReadCodeRecipe;

	// 5. Result
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pReadCodeResult[i] != NULL)
			delete m_pReadCodeResult[i], m_pReadCodeResult[i] = NULL;
		m_pReadCodeResult[i] = new CReadCodeResult;
	}

	// 6. Status
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pReadCodeStatus[i] != NULL)
			delete m_pReadCodeStatus[i], m_pReadCodeStatus[i] = NULL;
		m_pReadCodeStatus[i] = new CReadCodeStatus;
	}

	// 8. Process Core
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pReadCodeCore[i] != NULL)
			delete m_pReadCodeCore[i], m_pReadCodeCore[i] = NULL;
		m_pReadCodeCore[i] = new CReadCodeCore(this);
	}

	// 7. Camera
	if (m_pReadCodeSystemSetting->m_bSimulation == FALSE)
	{

		if (m_pReadCodeBaslerCam != NULL)
			m_pReadCodeBaslerCam->Destroy();
		m_pReadCodeBaslerCam = new CReadCodeBaslerCam(this);
#ifndef TEST_NO_CAMERA
		m_pReadCodeBaslerCam->Initialize();
		//m_pReadCodeBaslerCam->RegisterReceivedImageCallback(ReceivedImageCallback, this);
#endif
	}

	return TRUE;
}

BOOL CReadCodeProcessor::Destroy()
{
	if (m_pReadCodeBaslerCam != NULL)
		delete m_pReadCodeBaslerCam, m_pReadCodeBaslerCam = NULL;

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

	if (m_pReadCodeSystemSetting != NULL)
		delete m_pReadCodeSystemSetting, m_pReadCodeSystemSetting = NULL;

	if (m_pReadCodeRecipe != NULL)
		delete m_pReadCodeRecipe, m_pReadCodeRecipe = NULL;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		if (m_pReadCodeResult[i] != NULL)
			delete m_pReadCodeResult[i], m_pReadCodeResult[i] = NULL;

		if (m_pReadCodeStatus[i] != NULL)
			delete m_pReadCodeStatus[i], m_pReadCodeStatus[i] = NULL;

		if (m_pReadCodeCore[i] != NULL)
			delete m_pReadCodeCore[i], m_pReadCodeCore[i] = NULL;

		if (m_pReadCodeCameraSetting[i] != NULL)
			delete m_pReadCodeCameraSetting[i], m_pReadCodeCameraSetting[i] = NULL;
	}

	return TRUE;
}

CString CReadCodeProcessor::GetCurrentPathApp()
{
	TCHAR buff[MAX_PATH];
	memset(buff, 0, MAX_PATH);
	::GetModuleFileName(NULL, buff, sizeof(buff));
	CString csFolder = buff;
	csFolder = csFolder.Left(csFolder.ReverseFind(_T('\\')) + 1);

	return csFolder;
}

BOOL CReadCodeProcessor::InspectStart(int nThreadCount, BOOL bSimulator)
{
	if (bSimulator == TRUE)
	{
		int nCoreIdx = 0;
		int nFrame = 0;
		m_pReadCodeCore[nCoreIdx]->Inspect_Simulation(nCoreIdx, nFrame);

		return TRUE;
	}

	int nCamIdx = 0;
	m_pReadCodeStatus[nCamIdx]->SetStreaming(FALSE);
	m_pReadCodeStatus[nCamIdx]->SetInspectRunning(TRUE);

	m_pReadCodeBaslerCam->SetTriggerMode(0, 1);
	m_pReadCodeBaslerCam->SetTriggerSource(0, 1);
	//m_pReadCodeBaslerCam->SetExposureTime(0, 35.0);

	m_pReadCodeBaslerCam->StartGrab(0);

	return TRUE;
}

BOOL CReadCodeProcessor::InspectStop()
{
	int nCamIdx = 0;
	m_pReadCodeStatus[nCamIdx]->SetStreaming(TRUE);
	m_pReadCodeStatus[nCamIdx]->SetInspectRunning(FALSE);

	m_pReadCodeBaslerCam->StopGrab(0);

	m_pReadCodeBaslerCam->SetTriggerMode(0, 0);
	m_pReadCodeBaslerCam->SetTriggerSource(0, 0);
	//m_pReadCodeBaslerCam->SetExposureTime(0, m_pReadCodeCameraSetting[0]->m_nExposureTime);

	return TRUE;
}

BOOL CReadCodeProcessor::ProcessFrame(int nCoreIdx, LPBYTE pBuff)
{
	if (pBuff == NULL)
		return FALSE;

	m_pReadCodeCore[nCoreIdx]->Inspect_Real(nCoreIdx, pBuff);

	return TRUE;
}

//void CReadCodeProcessor::ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nFrameIdx, LPVOID param)
//{
//	CReadCodeProcessor* pThis = (CReadCodeProcessor*)param;
//	pThis->ReceivedImageCallbackEx(nGrabberIdx, nFrameIdx, pBuffer);
//}
//
//void CReadCodeProcessor::ReceivedImageCallbackEx(int nGrabberIdx, int nFrameIdx, LPVOID pBuffer)
//{
//	if ((LPBYTE)pBuffer == NULL)
//		return;
//
//	int nCoreIdx = nGrabberIdx;
//
//	m_pReadCodeCore[nCoreIdx]->Inspect_Real(nCoreIdx, (LPBYTE)pBuffer);
//}

BOOL CReadCodeProcessor::LoadSystemSettings(CReadCodeSystemSetting* pSystemSetting)
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

	CReadCodeSystemSetting sysSettings;

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

	CString csTestMode = (CString)pRoot->first_node("TestMode")->value();
	sysSettings.m_bTestMode = csTestMode.Compare(_T("true")) == 0 ? TRUE : FALSE;

	// set recipe path
	m_csRecipePath.Format(_T("%sRecipe\\%s.%s"), GetCurrentPathApp(), sysSettings.m_sModelName, _T("cfg"));

	*(pSystemSetting) = sysSettings;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CReadCodeProcessor::LoadRecipe(CReadCodeRecipe* pRecipe)
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

	CReadCodeRecipe readRecipe;

	recipeFile.GetItemValue(_T("MAX_CODE_COUNT"), readRecipe.m_nMaxCodeCount, 1);

	*(pRecipe) = readRecipe;

	return TRUE;
}

BOOL CReadCodeProcessor::LoadCameraSettings(CReadCodeCameraSetting* pCameraSetting)
{
	if (m_csCam1SettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Camera setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csCam1SettingPath);
	if (m_csCam1SettingPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Camera setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CReadCodeCameraSetting camSettings;

	// convert path
	USES_CONVERSION;
	char chCamSettingPath[1024] = {};
	sprintf_s(chCamSettingPath, "%s", W2A(m_csCam1SettingPath));

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

	camSettings.m_nFrameWidth = std::stoi(pRoot->first_node("FrameWidth")->value());
	camSettings.m_nFrameHeight = std::stoi(pRoot->first_node("FrameHeight")->value());
	camSettings.m_nChannel = std::stoi(pRoot->first_node("Channel")->value());
	camSettings.m_nTriggerMode = std::stoi(pRoot->first_node("TriggerMode")->value());
	camSettings.m_nTriggerSource = std::stoi(pRoot->first_node("TriggerSource")->value());
	camSettings.m_nExposureTime = std::atof(pRoot->first_node("ExposureTime")->value());
	camSettings.m_nAnalogGain = std::atof(pRoot->first_node("AnalogGain")->value());
	
	CString csSerialNumber = (CString)pRoot->first_node("SerialNumber")->value();
	ZeroMemory(camSettings.m_sSerialNumber, sizeof(camSettings.m_sSerialNumber));
	wsprintf(camSettings.m_sSerialNumber, _T("%s"), (TCHAR*)(LPCTSTR)csSerialNumber);
	*(pCameraSetting) = camSettings;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);

	return TRUE;
}

BOOL CReadCodeProcessor::SaveSystemSetting(CReadCodeSystemSetting* pSystemSetting)
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

	CReadCodeSystemSetting sysSetting;
	sysSetting = *(pSystemSetting);
	*(m_pReadCodeSystemSetting) = *(pSystemSetting);

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

	CString csTestMode = sysSetting.m_bTestMode == TRUE ? _T("true") : _T("false");
	const char* sTestMode = W2A(csTestMode);
	pRoot->first_node("TestMode")->value(sTestMode);

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

BOOL CReadCodeProcessor::SaveRecipe(CReadCodeRecipe* pRecipe)
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

	recipeFile.SetItemValue(_T("MAX_CODE_COUNT"), pRecipe->m_nMaxCodeCount);

	*(m_pReadCodeRecipe) = *pRecipe;

	recipeFile.WriteToFile();
}

BOOL CReadCodeProcessor::SaveCameraSettings(CReadCodeCameraSetting* pCameraSetting, int nCamIdx)
{
	if (m_csCam1SettingPath.IsEmpty())
	{
		AfxMessageBox(_T("Camera setting Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csCam1SettingPath);
	if (m_csCam1SettingPath.Right(6).CompareNoCase(_T("config")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Camera setting file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CReadCodeCameraSetting camSetting;
	camSetting = *(pCameraSetting);
	*(m_pReadCodeCameraSetting[nCamIdx]) = *(pCameraSetting);

	// convert path
	USES_CONVERSION;
	char chCamSettingPath[1024] = {};
	sprintf_s(chCamSettingPath, "%s", W2A(m_csCam1SettingPath));

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

	CString csFrameWidth;
	csFrameWidth.Format(_T("%d"), camSetting.m_nFrameWidth);
	const char* sFrameWidth = W2A(csFrameWidth);
	pRoot->first_node("FrameWidth")->value(sFrameWidth);

	CString csFrameHeight;
	csFrameHeight.Format(_T("%d"), camSetting.m_nFrameHeight);
	const char* sFrameHeight = W2A(csFrameHeight);
	pRoot->first_node("FrameHeight")->value(sFrameHeight);

	CString csChannel;
	csChannel.Format(_T("%d"), camSetting.m_nChannel);
	const char* sChannel = W2A(csChannel);
	pRoot->first_node("Channel")->value(sChannel);

	CString csTriggerMode;
	csTriggerMode.Format(_T("%d"), camSetting.m_nTriggerMode);
	const char* sTriggerMode = W2A(csTriggerMode);
	pRoot->first_node("TriggerMode")->value(sTriggerMode);

	CString csTriggerSource;
	csTriggerSource.Format(_T("%d"), camSetting.m_nTriggerSource);
	const char* sTriggerSource = W2A(csTriggerSource);
	pRoot->first_node("TriggerSource")->value(sTriggerSource);

	CString csExposureTime;
	csExposureTime.Format(_T("%.1f"), camSetting.m_nExposureTime);
	const char* sExposureTime = W2A(csExposureTime);
	pRoot->first_node("ExposureTime")->value(sExposureTime);

	CString csAnalogGain;
	csAnalogGain.Format(_T("%.1f"), camSetting.m_nAnalogGain);
	const char* sAnalogGain = W2A(csAnalogGain);
	pRoot->first_node("AnalogGain")->value(sAnalogGain);

	const char* sSerialNumber = W2A(camSetting.m_sSerialNumber);
	pRoot->first_node("SerialNumber")->value(sSerialNumber);

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

BOOL CReadCodeProcessor::ReloadSystemSetting()
{
	if (m_pReadCodeSystemSetting == NULL)
		return FALSE;

	BOOL bRet = LoadSystemSettings(m_pReadCodeSystemSetting);
	return bRet;
}

BOOL CReadCodeProcessor::ReloadRecipe()
{
	if (m_pReadCodeRecipe == NULL)
		return FALSE;

	BOOL bRet = LoadRecipe(m_pReadCodeRecipe);
	return bRet;
}

void CReadCodeProcessor::RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}

void CReadCodeProcessor::RegCallbackLogFunc(CallbackLogFunc* pFunc)
{
	m_pCallbackLogFunc = pFunc;
}

void CReadCodeProcessor::RegCallbackAlarm(CallbackAlarm* pFunc)
{
	m_pCallbackAlarm = pFunc;
}

void CReadCodeProcessor::InspectComplete(BOOL bSetting)
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	(m_pCallbackInsCompleteFunc)(bSetting);
}

void CReadCodeProcessor::LogMessage(char* strMessage)
{
	if (strMessage == NULL)
		return;

	CString strLogMessage = (CString)strMessage;

	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strLogMessage);
}

void CReadCodeProcessor::LogMessage(CString strMessage)
{
	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strMessage);
}

void CReadCodeProcessor::ShowLogView(BOOL bShow)
{
	if (m_pLogView == NULL)
		return;

#ifndef _DEBUG
	m_pLogView->ShowMode(bShow);
#endif // !_DEBUG
}

LPBYTE CReadCodeProcessor::GetImageBufferBaslerCam(int nCamIdx)
{
	if (m_pReadCodeBaslerCam == NULL)
		return NULL;


	LPBYTE pImageBuff = m_pReadCodeBaslerCam->GetBufferImage(nCamIdx);


	cv::Mat mat(m_pReadCodeCameraSetting[0]->m_nFrameWidth, m_pReadCodeCameraSetting[0]->m_nFrameHeight, CV_8UC1, pImageBuff);

	cv::cvtColor(mat, m_matBGR, cv::COLOR_GRAY2BGR);

	/*char ch[200] = {};
	sprintf_s(ch, "%s%s", "D:\\entry\\NCore\\NProjects\\ReadCodeMachine\\bin\\SaveImages\\", "imageTest.bmp");
	cv::imwrite(ch, m_matBGR);*/
	
	return m_matBGR.data;

}


BOOL CReadCodeProcessor::LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath)
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

LPBYTE CReadCodeProcessor::GetSimulatorBuffer(int nBuff, int nFrame)
{
	if (m_pSimulatorBuffer[nBuff] == NULL)
		return NULL;

	return m_pSimulatorBuffer[nBuff]->GetFrameImage(nFrame);
}

void CReadCodeProcessor::SystemMessage(CString strMessage)
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

BOOL CReadCodeProcessor::CreateResultBuffer()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameSize = 0;
	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		DWORD dwFrameWidth = (DWORD)m_pReadCodeCameraSetting[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pReadCodeCameraSetting[i]->m_nFrameHeight;
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

BOOL CReadCodeProcessor::CreateSimulatorBuffer()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameCount = 0;
	DWORD dwFrameSize = 0;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		DWORD dwFrameWidth = (DWORD)m_pReadCodeCameraSetting[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pReadCodeCameraSetting[i]->m_nFrameHeight;
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

LPBYTE CReadCodeProcessor::GetResultBuffer(int nBuff, int nFrame)
{
	if (m_pResultBuffer[nBuff] == NULL)
		return NULL;

	return m_pResultBuffer[nBuff]->GetFrameImage(nFrame);
}

BOOL CReadCodeProcessor::SetResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pResultBuffer[nBuff] == NULL)
		return FALSE;

	return m_pResultBuffer[nBuff]->SetFrameImage(nFrame, buff);
}

BOOL CReadCodeProcessor::GetInspectionResult(int nCoreIdx, CReadCodeResult* pReadCodeInspRes)
{
	CSingleLock localLock(&m_csInspResult[nCoreIdx]);
	localLock.Lock();
	*(pReadCodeInspRes) = *(m_pReadCodeResult[nCoreIdx]);

	localLock.Unlock();
	return TRUE;
}
