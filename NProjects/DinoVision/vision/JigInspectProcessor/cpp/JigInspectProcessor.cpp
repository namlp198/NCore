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

	CString csIsShowDetail = pRoot->first_node("IsShowDetail")->value();
	sysConfig.m_bShowDetail = csIsShowDetail.Compare(_T("true")) == 0 ? TRUE : FALSE;

	CString csIsSaveImage = pRoot->first_node("IsSaveImage")->value();
	sysConfig.m_bSaveImage = csIsSaveImage.Compare(_T("true")) == 0 ? TRUE : FALSE;

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
	ZeroMemory(camRecipe.m_sRecipeName, sizeof(camRecipe.m_sRecipeName));
	wsprintf(camRecipe.m_sRecipeName, _T("%s"), (TCHAR*)(LPCTSTR)csName);//1

	CString csAlgorithm = pRoot->first_node("Algorithm")->value();
	ZeroMemory(camRecipe.m_sAlgorithm, sizeof(camRecipe.m_sAlgorithm));
	wsprintf(camRecipe.m_sAlgorithm, _T("%s"), (TCHAR*)(LPCTSTR)csAlgorithm);//2

	camRecipe.m_nRectX = std::atoi(pRoot->first_node("RectX")->value());//3
	camRecipe.m_nRectY = std::atoi(pRoot->first_node("RectY")->value());//4
	camRecipe.m_nRectWidth = std::atoi(pRoot->first_node("RectWidth")->value());//5
	camRecipe.m_nRectHeight = std::atoi(pRoot->first_node("RectHeight")->value());//6
	camRecipe.m_dMatchingRate = std::stod(pRoot->first_node("MatchingRate")->value());//7
	camRecipe.m_nCenterX = std::atoi(pRoot->first_node("CenterX")->value());//8
	camRecipe.m_nCenterY = std::atoi(pRoot->first_node("CenterY")->value());//9

	CString csImageTemplate = pRoot->first_node("ImageTemplate")->value();
	ZeroMemory(camRecipe.m_sImageTemplate, sizeof(camRecipe.m_sImageTemplate));
	wsprintf(camRecipe.m_sImageTemplate, _T("%s"), (TCHAR*)(LPCTSTR)csImageTemplate);//10

	camRecipe.m_nOffsetROI0_X = std::atoi(pRoot->first_node("Offset_ROI0_X")->value());//11
	camRecipe.m_nOffsetROI0_Y = std::atoi(pRoot->first_node("Offset_ROI0_Y")->value());//12
	camRecipe.m_nOffsetROI1_X = std::atoi(pRoot->first_node("Offset_ROI1_X")->value());//13
	camRecipe.m_nOffsetROI1_Y = std::atoi(pRoot->first_node("Offset_ROI1_Y")->value());//14

	camRecipe.m_nROIWidth = std::atoi(pRoot->first_node("ROI_Width")->value());//15
	camRecipe.m_nROIHeight = std::atoi(pRoot->first_node("ROI_Height")->value());//16

	camRecipe.m_nNumberOfArray = std::atoi(pRoot->first_node("NumberOfArray")->value());//17
	camRecipe.m_nThresholdHeightMin = std::atoi(pRoot->first_node("ThresholdHeightMin")->value());//18
	camRecipe.m_nThresholdHeightMax = std::atoi(pRoot->first_node("ThresholdHeightMax")->value());//19
	camRecipe.m_nThresholdWidthMin = std::atoi(pRoot->first_node("ThresholdWidthMin")->value());//20
	camRecipe.m_nThresholdWidthMax = std::atoi(pRoot->first_node("ThresholdWidthMax")->value());//21

	camRecipe.m_nKSizeX_Open = std::atoi(pRoot->first_node("KSizeX_Open")->value());//22
	camRecipe.m_nKSizeY_Open = std::atoi(pRoot->first_node("KSizeY_Open")->value());//23
	camRecipe.m_nKSizeX_Close = std::atoi(pRoot->first_node("KSizeX_Close")->value());//24
	camRecipe.m_nKSizeY_Close = std::atoi(pRoot->first_node("KSizeY_Close")->value());//25
	camRecipe.m_nContourSizeMin = std::atoi(pRoot->first_node("ContourSizeMin")->value());//26
	camRecipe.m_nContourSizeMax = std::atoi(pRoot->first_node("ContourSizeMax")->value());//27
	camRecipe.m_nThresholdBinary = std::atoi(pRoot->first_node("ThresholdBinary")->value());//28

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
	*(m_pJigInspSysConfig) = *(pSysConfig);

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

	CString csIsShowDetail = sysConfig.m_bShowDetail == TRUE ? _T("true") : _T("false");
	const char* sIsShowDetail = W2A(csIsShowDetail);
	pRoot->first_node("IsShowDetail")->value(sIsShowDetail);

	CString csIsSaveImage = sysConfig.m_bSaveImage == TRUE ? _T("true") : _T("false");
	const char* sIsSaveImage = W2A(csIsSaveImage);
	pRoot->first_node("IsSaveImage")->value(sIsSaveImage);

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
	camConfig = *(pCamConfig);
	*(m_pJigInspCamConfig[nCamIdx]) = *(pCamConfig);

	// Convert path
	USES_CONVERSION;
	char cCamConfigPath[1024] = {};
	sprintf_s(cCamConfigPath, "%s", W2A(m_csCamConfigPath[nCamIdx]));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(cCamConfigPath, std::ios::in | std::ios::out);
	/* "Read file into vector<char>"  See linked thread above*/
	std::vector<char> buffer((std::istreambuf_iterator<char>(fs)), std::istreambuf_iterator<char>());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = xmlDoc.first_node("Configs");

	// write data

	const char* sCameraName = W2A(camConfig.m_sName);
	pRoot->first_node("CameraName")->value(sCameraName);

	const char* sInterfaceType = W2A(camConfig.m_sInterfaceType);
	pRoot->first_node("InterfaceType")->value(sInterfaceType);

	const char* sSensorType = W2A(camConfig.m_sSensorType);
	pRoot->first_node("SensorType")->value(sSensorType);

	char sChannels[10];
	sprintf_s(sChannels, "%d", camConfig.m_nChannels);
	pRoot->first_node("Channels")->value(sChannels);

	const char* sManufacturer = W2A(camConfig.m_sManufacturer);
	pRoot->first_node("Manufacturer")->value(sManufacturer);

	char sFrameWidth[10];
	sprintf_s(sFrameWidth, "%d", camConfig.m_nFrameWidth);
	pRoot->first_node("FrameWidth")->value(sFrameWidth);

	char sFrameHeight[10];
	sprintf_s(sFrameHeight, "%d", camConfig.m_nFrameHeight);
	pRoot->first_node("FrameHeight")->value(sFrameHeight);

	const char* sSerialNumber = W2A(camConfig.m_sSerialNumber);
	pRoot->first_node("SerialNumber")->value(sSerialNumber);

	const char* sImageSavePath = W2A(camConfig.m_sImageSavePath);
	pRoot->first_node("ImageSavePath")->value(sImageSavePath);

	const char* sImageTemplatePath = W2A(camConfig.m_sImageTemplatePath);
	pRoot->first_node("ImageTemplatePath")->value(sImageTemplatePath);

	const char* sRecipeName = W2A(camConfig.m_sRecipeName);
	pRoot->first_node("RecipeName")->value(sRecipeName);

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	std::ofstream file;
	file.open(cCamConfigPath);
	file << data;
	file.close();

	return TRUE;
}

BOOL CJigInspectProcessor::SaveRecipe(int nCamIdx, CJigInspectRecipe* pRecipe)
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

	CJigInspectRecipe recipe;
	recipe = *(pRecipe);
	*(m_pJigInspRecipe[nCamIdx]) = *(pRecipe);

	// Convert path
	USES_CONVERSION;
	char cRecipePath[1024] = {};
	sprintf_s(cRecipePath, "%s", W2A(m_csRecipePath[nCamIdx]));

	XMLDocument_2 xmlDoc;
	std::string error;

	std::ifstream fs(cRecipePath, std::ios::in | std::ios::out);
	std::string inputXml;
	std::string line;
	while (std::getline(fs, line))
	{
		inputXml += line;
	}
	std::vector<char> buffer(inputXml.begin(), inputXml.end());
	buffer.push_back('\0');
	xmlDoc.parse<rapidxml::parse_full | rapidxml::parse_no_data_nodes>(&buffer[0]);

	rapidxml::xml_node<>* pRoot = xmlDoc.first_node("Recipe");

#pragma region Write Data
	const char* sRecipeName = W2A(recipe.m_sRecipeName);
	pRoot->first_node("RecipeName")->value(sRecipeName);//1

	const char* sAlgorithm = W2A(recipe.m_sAlgorithm);
	pRoot->first_node("Algorithm")->value(sAlgorithm);//2

	char sRectX[10];
	sprintf_s(sRectX, "%d", recipe.m_nRectX);
	pRoot->first_node("RectX")->value(sRectX);//3

	char sRectY[10];
	sprintf_s(sRectY, "%d", recipe.m_nRectY);
	pRoot->first_node("RectY")->value(sRectY);//4

	char sRectWidth[10];
	sprintf_s(sRectWidth, "%d", recipe.m_nRectWidth);
	pRoot->first_node("RectWidth")->value(sRectWidth);//5

	char sRectHeight[10];
	sprintf_s(sRectHeight, "%d", recipe.m_nRectHeight);
	pRoot->first_node("RectHeight")->value(sRectHeight);//6

	char sMatchingRate[10];
	sprintf_s(sMatchingRate, "%.2f", recipe.m_dMatchingRate);
	pRoot->first_node("MatchingRate")->value(sMatchingRate);//7

	char sCenterX[10];
	sprintf_s(sCenterX, "%d", recipe.m_nCenterX);
	pRoot->first_node("CenterX")->value(sCenterX);//8

	char sCenterY[10];
	sprintf_s(sCenterY, "%d", recipe.m_nCenterY);
	pRoot->first_node("CenterY")->value(sCenterY);//9

	const char* sImageTemplate = W2A(recipe.m_sImageTemplate);
	pRoot->first_node("ImageTemplate")->value(sImageTemplate);//10

	char sOffset_ROI0_X[10];
	sprintf_s(sOffset_ROI0_X, "%d", recipe.m_nOffsetROI0_X);
	pRoot->first_node("Offset_ROI0_X")->value(sOffset_ROI0_X);//11

	char sOffset_ROI0_Y[10];
	sprintf_s(sOffset_ROI0_Y, "%d", recipe.m_nOffsetROI0_Y);
	pRoot->first_node("Offset_ROI0_Y")->value(sOffset_ROI0_Y);//12

	char sOffset_ROI1_X[10];
	sprintf_s(sOffset_ROI1_X, "%d", recipe.m_nOffsetROI1_X);
	pRoot->first_node("Offset_ROI1_X")->value(sOffset_ROI1_X);//13

	char sOffset_ROI1_Y[10];
	sprintf_s(sOffset_ROI1_Y, "%d", recipe.m_nOffsetROI1_Y);
	pRoot->first_node("Offset_ROI1_Y")->value(sOffset_ROI1_Y);//14

	char sROI_Width[10];
	sprintf_s(sROI_Width, "%d", recipe.m_nROIWidth);
	pRoot->first_node("ROI_Width")->value(sROI_Width);//15

	char sROI_Height[10];
	sprintf_s(sROI_Height, "%d", recipe.m_nROIHeight);
	pRoot->first_node("ROI_Height")->value(sROI_Height);//16

	char sNumberOfArray[10];
	sprintf_s(sNumberOfArray, "%d", recipe.m_nNumberOfArray);
	pRoot->first_node("NumberOfArray")->value(sNumberOfArray);//17

	char sThresholdHeightMin[10];
	sprintf_s(sThresholdHeightMin, "%d", recipe.m_nThresholdHeightMin);
	pRoot->first_node("ThresholdHeightMin")->value(sThresholdHeightMin);//18

	char sThresholdHeightMax[10];
	sprintf_s(sThresholdHeightMax, "%d", recipe.m_nThresholdHeightMax);
	pRoot->first_node("ThresholdHeightMax")->value(sThresholdHeightMax);//19

	char sThresholdWidthMin[10];
	sprintf_s(sThresholdWidthMin, "%d", recipe.m_nThresholdWidthMin);
	pRoot->first_node("ThresholdWidthMin")->value(sThresholdWidthMin);//20

	char sThresholdWidthMax[10];
	sprintf_s(sThresholdWidthMax, "%d", recipe.m_nThresholdWidthMax);
	pRoot->first_node("ThresholdWidthMax")->value(sThresholdWidthMax);//21

	char sKSizeX_Open[10];
	sprintf_s(sKSizeX_Open, "%d", recipe.m_nKSizeX_Open);
	pRoot->first_node("KSizeX_Open")->value(sKSizeX_Open);//22

	char sKSizeY_Open[10];
	sprintf_s(sKSizeY_Open, "%d", recipe.m_nKSizeY_Open);
	pRoot->first_node("KSizeY_Open")->value(sKSizeY_Open);//23

	char sKSizeX_Close[10];
	sprintf_s(sKSizeX_Close, "%d", recipe.m_nKSizeX_Close);
	pRoot->first_node("KSizeX_Close")->value(sKSizeX_Close);//24

	char sKSizeY_Close[10];
	sprintf_s(sKSizeY_Close, "%d", recipe.m_nKSizeY_Close);
	pRoot->first_node("KSizeY_Close")->value(sKSizeY_Close);//25

	char sContourSizeMin[10];
	sprintf_s(sContourSizeMin, "%d", recipe.m_nContourSizeMin);
	pRoot->first_node("ContourSizeMin")->value(sContourSizeMin);//26

	char sContourSizeMax[10];
	sprintf_s(sContourSizeMax, "%d", recipe.m_nContourSizeMax);
	pRoot->first_node("ContourSizeMax")->value(sContourSizeMax);//27

	char sThresholdBinary[10];
	sprintf_s(sThresholdBinary, "%d", recipe.m_nThresholdBinary);
	pRoot->first_node("ThresholdBinary")->value(sThresholdBinary);//28

#pragma endregion

	// Convert the modified XML back to a string
	std::string data;
	rapidxml::print(std::back_inserter(data), xmlDoc);

	std::ofstream file;
	file.open(cRecipePath);
	file << data;
	file.close();

	return TRUE;
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

	pJigInspRes->m_TemplateMatchingResult.m_nLeft = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nLeft;
	pJigInspRes->m_TemplateMatchingResult.m_nTop = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nTop;
	pJigInspRes->m_TemplateMatchingResult.m_nWidth = m_pJigInspRecipe[nCamIdx]->m_nRectWidth;
	pJigInspRes->m_TemplateMatchingResult.m_nHeight = m_pJigInspRecipe[nCamIdx]->m_nRectHeight;
	pJigInspRes->m_TemplateMatchingResult.m_nCenterX = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nCenterX;
	pJigInspRes->m_TemplateMatchingResult.m_nCenterY = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nCenterY;
	pJigInspRes->m_TemplateMatchingResult.m_dMatchingRate = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_dMatchingRate;
	pJigInspRes->m_TemplateMatchingResult.m_nDelta_X = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nDelta_X;
	pJigInspRes->m_TemplateMatchingResult.m_nDelta_Y = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_nDelta_Y;
	pJigInspRes->m_TemplateMatchingResult.m_dDif_Angle = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_dDif_Angle;
	pJigInspRes->m_TemplateMatchingResult.m_bResult = m_pJigInspResutls[nCamIdx]->m_TemplateMatchingResult.m_bResult;

	return TRUE;
}

BOOL CJigInspectProcessor::GrabImageForLocatorTool(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pInspDinoCam == NULL)
		return FALSE;

	m_pInspDinoCam->GrabImageForLocatorTool(nCamIdx);
}

BOOL CJigInspectProcessor::LocatorTrain(int nCamIdx, CJigInspectRecipe* pRecipe)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pInspDinoCam == NULL)
		return FALSE;

	m_pInspDinoCam->LocatorTrain(nCamIdx, pRecipe);
	//SaveRecipe(nCamIdx, m_pJigInspRecipe[nCamIdx]);
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
