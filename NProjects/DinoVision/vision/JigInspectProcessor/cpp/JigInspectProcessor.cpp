#include "pch.h"
#include "JigInspectProcessor.h"

CJigInspectProcessor::CJigInspectProcessor()
{
	m_csSysConfigPath = GetCurrentApp() + _T("Configurations\\Configurations.xml");
	SetCamConfigPath();

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pImageBuffer[i] = NULL;
	}
}

CJigInspectProcessor::~CJigInspectProcessor()
{
	Destroy();
}

BOOL CJigInspectProcessor::Initialize()
{
	if (m_pJigInspSysConfig != NULL)
		delete m_pJigInspSysConfig, m_pJigInspSysConfig == NULL;
	m_pJigInspSysConfig = new CJigInspectSystemConfig;
	LoadSysConfigurations(m_pJigInspSysConfig);

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspCamConfig[i] != NULL)
			delete m_pJigInspCamConfig[i], m_pJigInspCamConfig[i] == NULL;
		m_pJigInspCamConfig[i] = new CJigInspectCameraConfig;

		// Load Cam Config
		if (LoadCamConfigurations(i, m_pJigInspCamConfig[i]) == FALSE)
			return FALSE;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspRecipe[i] != NULL)
			delete m_pJigInspRecipe[i], m_pJigInspRecipe[i] == NULL;
		m_pJigInspRecipe[i] = new CJigInspectRecipe;

		// Load Recipe
		if (LoadRecipe(i, m_pJigInspRecipe[i]) == FALSE)
			return FALSE;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspResutls[i] != NULL)
			delete m_pJigInspResutls[i], m_pJigInspResutls[i] == NULL;
		m_pJigInspResutls[i] = new CJigInspectResults;
	}

	// Create Image Buffer..
	if (CreateBuffer() == FALSE)
		return FALSE;

	if (m_pInspDinoCam != NULL)
	{
		delete m_pInspDinoCam, m_pInspDinoCam = NULL;
	}
	m_pInspDinoCam = new CJigInspectDinoCam(this);
	m_pInspDinoCam->Initialize();
}

BOOL CJigInspectProcessor::Destroy()
{
	if (m_pInspDinoCam != NULL)
		delete m_pInspDinoCam, m_pInspDinoCam = NULL;

	if (m_pJigInspSysConfig != NULL)
		delete m_pJigInspSysConfig, m_pJigInspSysConfig == NULL;

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspCamConfig[i] != NULL)
			delete m_pJigInspCamConfig[i], m_pJigInspCamConfig[i] == NULL;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspRecipe[i] != NULL)
			delete m_pJigInspRecipe[i], m_pJigInspRecipe[i] == NULL;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspResutls[i] != NULL)
			delete m_pJigInspResutls[i], m_pJigInspResutls[i] == NULL;
	}

	return TRUE;
}

BOOL CJigInspectProcessor::LoadSysConfigurations(CJigInspectSystemConfig* pSysConfig)
{
	if (m_csSysConfigPath.IsEmpty())
	{
		AfxMessageBox(_T("Config Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csSysConfigPath);
	if (m_csSysConfigPath.Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Config file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CJigInspectSystemConfig sysConfig;

	// Convert path
	USES_CONVERSION;
	char cConfigPath[1024] = {};
	sprintf_s(cConfigPath, "%s", W2A(m_csSysConfigPath));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(cConfigPath, error);
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
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "Configurations", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// 5. start read
	CString csRecipePath = pRoot->first_node("RecipePath")->value();
	ZeroMemory(sysConfig.m_sRecipePath, sizeof(sysConfig.m_sRecipePath));
	wsprintf(sysConfig.m_sRecipePath, _T("%s"), (TCHAR*)(LPCTSTR)csRecipePath);

	CString csModel = pRoot->first_node("Model")->value();
	ZeroMemory(sysConfig.m_sModel, sizeof(sysConfig.m_sModel));
	wsprintf(sysConfig.m_sModel, _T("%s"), (TCHAR*)(LPCTSTR)csModel);

	CString csCOMPort = pRoot->first_node("COMPort")->value();
	ZeroMemory(sysConfig.m_sCOMPort, sizeof(sysConfig.m_sCOMPort));
	wsprintf(sysConfig.m_sCOMPort, _T("%s"), (TCHAR*)(LPCTSTR)csCOMPort);

	CString csUsePCControl = pRoot->first_node("UsePCControl")->value();
	sysConfig.m_bUsePCControl = csUsePCControl.Compare(_T("true")) == 0 ? TRUE : FALSE;

	*(pSysConfig) = sysConfig;

	return TRUE;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CJigInspectProcessor::LoadCamConfigurations(int nCamIdx, CJigInspectCameraConfig* pCamConfig)
{
	if (m_csCamConfigPath[nCamIdx].IsEmpty())
	{
		AfxMessageBox(_T("Config Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csCamConfigPath[nCamIdx]);
	if (m_csCamConfigPath[nCamIdx].Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Config file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CJigInspectCameraConfig camConfig;

	// Convert path
	USES_CONVERSION;
	char cCamConfigPath[1024] = {};
	sprintf_s(cCamConfigPath, "%s", W2A(m_csCamConfigPath[nCamIdx]));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(cCamConfigPath, error);
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
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "Configs", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// 5. start read
	CString csName = pRoot->first_node("CameraName")->value();
	ZeroMemory(camConfig.m_sName, sizeof(camConfig.m_sName));
	wsprintf(camConfig.m_sName, _T("%s"), (TCHAR*)(LPCTSTR)csName);

	CString csInterfaceType = pRoot->first_node("InterfaceType")->value();
	ZeroMemory(camConfig.m_sInterfaceType, sizeof(camConfig.m_sInterfaceType));
	wsprintf(camConfig.m_sInterfaceType, _T("%s"), (TCHAR*)(LPCTSTR)csInterfaceType);

	CString csSensorType = pRoot->first_node("SensorType")->value();
	ZeroMemory(camConfig.m_sSensorType, sizeof(camConfig.m_sSensorType));
	wsprintf(camConfig.m_sSensorType, _T("%s"), (TCHAR*)(LPCTSTR)csSensorType);

	camConfig.m_nChannels = std::atoi(pRoot->first_node("Channels")->value());

	CString csManufacturer = pRoot->first_node("Manufacturer")->value();
	ZeroMemory(camConfig.m_sManufacturer, sizeof(camConfig.m_sManufacturer));
	wsprintf(camConfig.m_sManufacturer, _T("%s"), (TCHAR*)(LPCTSTR)csManufacturer);
	
	camConfig.m_nFrameWidth = std::atoi(pRoot->first_node("FrameWidth")->value());
	camConfig.m_nFrameHeight = std::atoi(pRoot->first_node("FrameHeight")->value());

	CString csSerialNumber = pRoot->first_node("SerialNumber")->value();
	ZeroMemory(camConfig.m_sSerialNumber, sizeof(camConfig.m_sSerialNumber));
	wsprintf(camConfig.m_sSerialNumber, _T("%s"), (TCHAR*)(LPCTSTR)csSerialNumber);

	CString csImageSavePath = pRoot->first_node("ImageSavePath")->value();
	ZeroMemory(camConfig.m_sImageSavePath, sizeof(camConfig.m_sImageSavePath));
	wsprintf(camConfig.m_sImageSavePath, _T("%s"), (TCHAR*)(LPCTSTR)csImageSavePath);

	CString csImageTemplatePath = pRoot->first_node("ImageTemplatePath")->value();
	ZeroMemory(camConfig.m_sImageTemplatePath, sizeof(camConfig.m_sImageTemplatePath));
	wsprintf(camConfig.m_sImageTemplatePath, _T("%s"), (TCHAR*)(LPCTSTR)csImageTemplatePath);

	CString csRecipeName = pRoot->first_node("RecipeName")->value();
	ZeroMemory(camConfig.m_sRecipeName, sizeof(camConfig.m_sRecipeName));
	wsprintf(camConfig.m_sRecipeName, _T("%s"), (TCHAR*)(LPCTSTR)csRecipeName);

	*(pCamConfig) = camConfig;

	// Set Recipe Path at here
	SetRecipePath(nCamIdx, csRecipeName);

	return TRUE;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CJigInspectProcessor::LoadRecipe(int nCamIdx, CJigInspectRecipe* pRecipe)
{
	if (m_csRecipePath[nCamIdx].IsEmpty())
	{
		AfxMessageBox(_T("Config Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csRecipePath[nCamIdx]);
	if (m_csRecipePath[nCamIdx].Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Config file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CJigInspectRecipe camRecipe;

	// Convert path
	USES_CONVERSION;
	char cCamRecipePath[1024] = {};
	sprintf_s(cCamRecipePath, "%s", W2A(m_csRecipePath[nCamIdx]));

	// 1. init xml manager
	XMLFile* m_pXmlFile;
	XMLDocument_2* m_pXmlDoc;
	std::string error;

	// 2. Open file
	m_pXmlFile = ::OpenXMLFile(cCamRecipePath, error);
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
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "Recipe", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// 5. start read
	CString csName = pRoot->first_node("RecipeName")->value();
	ZeroMemory(camRecipe.m_sName, sizeof(camRecipe.m_sName));
	wsprintf(camRecipe.m_sName, _T("%s"), (TCHAR*)(LPCTSTR)csName);

	CString csAlgorithm = pRoot->first_node("Algorithm")->value();
	ZeroMemory(camRecipe.m_sAlgorithm, sizeof(camRecipe.m_sAlgorithm));
	wsprintf(camRecipe.m_sAlgorithm, _T("%s"), (TCHAR*)(LPCTSTR)csAlgorithm);

	*(pRecipe) = camRecipe;
	
	return TRUE;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CJigInspectProcessor::SaveSysConfigurations(CJigInspectSystemConfig* pSysConfig)
{
	if (m_csSysConfigPath.IsEmpty())
	{
		AfxMessageBox(_T("Config Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csSysConfigPath);
	if (m_csSysConfigPath.Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Config file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	CJigInspectSystemConfig sysConfig;
	sysConfig = *(pSysConfig);

	// Convert path
	USES_CONVERSION;
	char cConfigPath[1024] = {};
	sprintf_s(cConfigPath, "%s", W2A(m_csSysConfigPath));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(cConfigPath, std::ios::in | std::ios::out);
	/* "Read file into vector<char>"  See linked thread above*/
	std::vector<char> buffer((std::istreambuf_iterator<char>(fs)), std::istreambuf_iterator<char>());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = xmlDoc.first_node("Configurations");

	// write data
	
	const char* sRecipePath = W2A(sysConfig.m_sRecipePath);
	pRoot->first_node("RecipePath")->value(sRecipePath);

	const char* sModel = W2A(sysConfig.m_sModel);
	pRoot->first_node("Model")->value(sModel);

	const char* sCOMPort = W2A(sysConfig.m_sCOMPort);
	pRoot->first_node("COMPort")->value(sCOMPort);

	CString csUsePCControl = sysConfig.m_bUsePCControl == TRUE ? _T("true") : _T("false");
	const char* sUsePCControl = W2A(csUsePCControl);
	pRoot->first_node("UsePCControl")->value(sUsePCControl);

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	// Convert a TCHAR string to a LPCSTR
	//CT2CA pszConvertedAnsiString(m_csSysConfigPath);
	// construct a std::string using the LPCSTR input
	//std::string configPath(pszConvertedAnsiString);

	std::ofstream file;
	file.open(cConfigPath);
	file << data;
	file.close();

	return TRUE;
}

BOOL CJigInspectProcessor::SaveCamConfigurations(int nCamIdx, CJigInspectCameraConfig* pCamConfig)
{
	return 0;
}

BOOL CJigInspectProcessor::SaveRecipe(int nCamIdx, CJigInspectRecipe* pRecipe)
{
	return 0;
}

BOOL CJigInspectProcessor::CreateBuffer()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		USES_CONVERSION;
		char cSensorType[10] = {};
		sprintf_s(cSensorType, "%s", W2A(m_pJigInspCamConfig[i]->m_sSensorType));


		int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
		int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

		BOOL bRetValue = FALSE;

		DWORD dwFrameWidth = (DWORD)m_pJigInspCamConfig[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pJigInspCamConfig[i]->m_nFrameHeight;
		DWORD dwFrameCount = 0;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nFrameChannels;

		DWORD64 dwTotalFrameCount = 0;

		if (m_pImageBuffer[i] != NULL)
		{
			m_pImageBuffer[i]->DeleteSharedMemory();
			delete m_pImageBuffer[i];
			m_pImageBuffer[i] = NULL;
		}

		m_pImageBuffer[i] = new CSharedMemoryBuffer;

		dwFrameCount = MAX_FRAME_COUNT;

		dwTotalFrameCount += dwFrameCount;

		m_pImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), MAP_FILE_NAME_INS_BUFFER, i);

		bRetValue = m_pImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
		}

		CString strLogMessage;
		strLogMessage.Format(_T("Total Create Memory : %.2f GB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000000.0));
	}

	return TRUE;
}

BOOL CJigInspectProcessor::InspectStart(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pInspDinoCam == NULL)
		return FALSE;

	m_pInspDinoCam->InspectStart(nCamIdx);
}

LPBYTE CJigInspectProcessor::GetFrameImage(int nCamIdx, UINT nFrameIndex)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetFrameImage((DWORD)nFrameIndex);
}

LPBYTE CJigInspectProcessor::GetBufferImage(int nCamIdx, UINT nY)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetBufferImage(nY);
}

void CJigInspectProcessor::InspectComplete()
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	m_pCallbackInsCompleteFunc();
}

BOOL CJigInspectProcessor::GetInspectionResult(int nCamIdx, CJigInspectResults* pJigInspRes)
{
	pJigInspRes->m_bInspectCompleted = m_pJigInspResutls[nCamIdx]->m_bInspectCompleted;
	pJigInspRes->m_bResultOKNG = m_pJigInspResutls[nCamIdx]->m_bResultOKNG;

	return TRUE;
}

void CJigInspectProcessor::RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}

CString CJigInspectProcessor::GetCurrentApp()
{
	TCHAR buff[MAX_PATH];
	memset(buff, 0, MAX_PATH);
	::GetModuleFileName(NULL, buff, sizeof(buff));
	CString csFolder = buff;
	csFolder = csFolder.Left(csFolder.ReverseFind(_T('\\')) + 1);

	return csFolder;
}

BOOL CJigInspectProcessor::SetCamConfigPath()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		CString fileName;
		fileName.Format(_T("cam%d_config.xml"), i);

		m_csCamConfigPath[i] = GetCurrentApp() + _T("Configurations\\") + fileName;
	}

	return TRUE;
}

BOOL CJigInspectProcessor::SetRecipePath(int nCamIdx, CString recipeName)
{
	if (recipeName.Compare(_T("")) == 0)
		return FALSE;

	m_csRecipePath[nCamIdx] = GetCurrentApp() + _T("Recipe\\") + recipeName + _T(".xml");

	return TRUE;
}
