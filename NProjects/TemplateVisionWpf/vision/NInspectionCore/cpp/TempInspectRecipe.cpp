#include "pch.h"
#include "TempInspectRecipe.h"

CTempInspectRecipe::CTempInspectRecipe()
{
	m_csConfigPath = _T("D:\\NCore\\NProjects\\TemplateVisionWpf\\bin\\Release\\Config");

	m_pTempInspConfig = new CTempInspectConfig(m_csConfigPath);
	m_pTempInspConfig->Initialize();

	m_csJobPath = m_pTempInspConfig->GetJobPath();
	m_csJobName = m_pTempInspConfig->GetJobName();
	m_csJobPath = m_csJobPath + "\\" + m_csJobName + ".xml";

	//m_pQueueTools = new QueueTools;
	m_queueLoc = new QueueLocTools;
	m_queueSelROI = new QueueSelROITools;
	m_pCameraInfos = new CameraInfo;
}

CTempInspectRecipe::~CTempInspectRecipe()
{
	/*if (m_pQueueTools != NULL)
		delete m_pQueueTools, m_pQueueTools = NULL;*/

	/*if (m_pCameraInfos != NULL)
		delete m_pCameraInfos, m_pCameraInfos = NULL;

	if (m_pTempInspConfig != NULL)
		delete m_pTempInspConfig, m_pTempInspConfig = NULL;*/

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CTempInspectRecipe::LoadRecipe(int nCamIdx)
{
	if (m_csJobPath.IsEmpty())
	{
		AfxMessageBox(_T("Recipe Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csJobPath);
	if (m_csJobPath.Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Recipe no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	USES_CONVERSION;
	char cJobPath[1024] = {};
	sprintf_s(cJobPath, "%s", W2A(m_csJobPath));

	std::string error;

	m_pXmlFile = ::OpenXMLFile(cJobPath, error);
	if (!m_pXmlFile)
	{
		AfxMessageBox((CString)(error.c_str()));
		return FALSE;
	}

	m_pXmlDoc = ::CreateXMLFromFile(m_pXmlFile, error);
	if (!m_pXmlDoc)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		return FALSE;
	}

	// Find root: Job
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "Job", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	// Start read the nodes: Camera
	for (XMLElement* xmlCam = pRoot->first_node("Camera"); xmlCam; xmlCam = xmlCam->next_sibling("Camera"))
	{
		if (!xmlCam)
		{
			AfxMessageBox((CString)(error.c_str()));
			::DisposeXMLFile(m_pXmlFile);
			::DisposeXMLObject(m_pXmlDoc);
			continue;
		}

		// read id cam from Job file and compare with camera index passed into: = save, != continute
		int nId = std::atoi(xmlCam->first_attribute("id")->value());

#pragma region Read and save camera infos 

		if (nCamIdx == nId)
		{
			ReadCameraInfo(xmlCam, nId);

#pragma endregion

#pragma region Read Recipe
			XMLElement* xmlRecipe = xmlCam->first_node("Recipe");

			/* read and save recipe info */
			ReadRecipeInfo(xmlRecipe);

			// add Locator Tool
			for (XMLElement* xmlLoc = xmlRecipe->first_node("LocatorTool"); xmlLoc; xmlLoc = xmlLoc->next_sibling("LocatorTool"))
			{
				if (!xmlLoc)
				{
					AfxMessageBox((CString)(error.c_str()));
					::DisposeXMLFile(m_pXmlFile);
					::DisposeXMLObject(m_pXmlDoc);
					continue;
				}

				ReadLocTool(xmlLoc);
			}

			for (XMLElement* xmlSelROI = xmlRecipe->first_node("SelectROITool"); xmlSelROI; xmlSelROI = xmlSelROI->next_sibling("SelectROITool"))
			{
				if (!xmlSelROI)
				{
					AfxMessageBox((CString)(error.c_str()));
					::DisposeXMLFile(m_pXmlFile);
					::DisposeXMLObject(m_pXmlDoc);
					continue;
				}

				ReadSelROITool(xmlSelROI, xmlSelROI->first_attribute("algorithm")->value());

			}
			// add Select ROI tool
#pragma endregion

			break;
		}
		else
		{
			continue;
		}
	}
}

void CTempInspectRecipe::ReadCameraInfo(XMLElement* xmlCam, int nId)
{
	m_pCameraInfos->m_nId = nId;
	// name
	m_pCameraInfos->m_csName = xmlCam->first_attribute("name")->value();
	// interfaceType
	m_pCameraInfos->m_csInterfaceType = xmlCam->first_attribute("interfaceType")->value();
	// sensorType
	m_pCameraInfos->m_csSensorType = xmlCam->first_attribute("sensorType")->value();
	// manufacturer
	m_pCameraInfos->m_csManufacturer = xmlCam->first_attribute("manufacturer")->value();
	// frame width
	m_pCameraInfos->m_nFrameWidth = std::stoi(xmlCam->first_attribute("frameWidth")->value());
	// frame height
	m_pCameraInfos->m_nFrameHeight = std::atoi(xmlCam->first_attribute("frameHeight")->value());
	// serialNumber
	m_pCameraInfos->m_csSerialNumber = xmlCam->first_attribute("serialNumber")->value();
}

void CTempInspectRecipe::ReadRecipeInfo(XMLElement* xmlRecipe)
{
	/* read and save recipe info */
	// 1. id
	m_nId = std::atoi(xmlRecipe->first_attribute("id")->value());
	// 2. name
	m_csName = xmlRecipe->first_attribute("name")->value();
	// 3. cameraId parent
	m_csCameraIdParent = xmlRecipe->first_attribute("cameraIdParent")->value();
}

void CTempInspectRecipe::ReadLocTool(XMLElement* xmlLoc)
{
	CLocatorTool locTool;
	CParameterLocator paramLocTool;
	/* get parameter locator tool */

	// 1. id
	paramLocTool.m_csId = xmlLoc->first_attribute("id")->value();
	// 2. name
	paramLocTool.m_csName = xmlLoc->first_attribute("name")->value();
	// 3. priority
	paramLocTool.m_nPriority = std::atoi(xmlLoc->first_attribute("priority")->value());
	// 4. has children
	char* cHasChild = xmlLoc->first_attribute("hasChildren")->value();
	BOOL hasChild = strcmp(cHasChild, "True") == 0 ? TRUE : FALSE;
	paramLocTool.m_bHasChildren = hasChild;
	// 5. children
	paramLocTool.m_csChildren = xmlLoc->first_attribute("children")->value();

	// 6. RectangleInSide
	XMLElement* xmlRectInSide = xmlLoc->first_node("RectangleInSide");
	paramLocTool.m_RectangleInSide[0] = std::atoi(xmlRectInSide->first_attribute("x")->value()); // x
	paramLocTool.m_RectangleInSide[1] = std::atoi(xmlRectInSide->first_attribute("y")->value()); // y
	paramLocTool.m_RectangleInSide[2] = std::atoi(xmlRectInSide->first_attribute("width")->value()); // width
	paramLocTool.m_RectangleInSide[3] = std::atoi(xmlRectInSide->first_attribute("height")->value()); // height

	// 7. RectangleOutSide
	XMLElement* xmlRectOutSide = xmlLoc->first_node("RectangleOutSide");
	paramLocTool.m_RectangleOutSide[0] = std::atoi(xmlRectOutSide->first_attribute("x")->value()); // x
	paramLocTool.m_RectangleOutSide[1] = std::atoi(xmlRectOutSide->first_attribute("y")->value()); // y
	paramLocTool.m_RectangleOutSide[2] = std::atoi(xmlRectOutSide->first_attribute("width")->value()); // width
	paramLocTool.m_RectangleOutSide[3] = std::atoi(xmlRectOutSide->first_attribute("height")->value()); // height

	// 8. Data train
	XMLElement* xmlDataTrain = xmlLoc->first_node("DataTrain");
	paramLocTool.m_DataTrain[0] = std::atoi(xmlDataTrain->first_attribute("x")->value());
	paramLocTool.m_DataTrain[1] = std::atoi(xmlDataTrain->first_attribute("y")->value());

	// add parameter to locator tool
	locTool.SetParamLoca(paramLocTool);

	// add tool to queue tools
	m_queueLoc->push(locTool);
}

void CTempInspectRecipe::ReadSelROITool(XMLElement* xmlSelROI, char* cAlgorithm)
{
	emAlgorithms algorithm;

	CSelectROITool pSelROITool;
	CParameterSelectROI paramSelROI;

	CVisionParameterManager vsParamManager;
	CVisionResultManager vsResultManager;
	CVisionAlgorithms vsAlgorithm;

	if (strcmp(cAlgorithm, "CountPixel") == 0)
	{
		// read seletect ROI's infos
		algorithm = emCountPixel;
		paramSelROI.m_csId = xmlSelROI->first_attribute("id")->value();
		paramSelROI.m_csName = xmlSelROI->first_attribute("name")->value();
		paramSelROI.m_csType = xmlSelROI->first_attribute("type")->value();
		paramSelROI.m_emAlgorithm = algorithm;
		paramSelROI.m_bRotations = (strcmp(xmlSelROI->first_attribute("rotations")->value(), "True") == 0 ? TRUE : FALSE);
		paramSelROI.m_nPriority = std::atoi(xmlSelROI->first_attribute("priority")->value());
		pSelROITool.SetParamSelROI(paramSelROI);

		// create parameter and result of Count Pixel algorithm
		CParameterCountPixel paramCountPxl;

		// 1. ROI
		XMLElement* xmlROI = xmlSelROI->first_node("Parameters")->first_node("ROI");
		int x = std::atoi(xmlROI->first_attribute("x")->value());
		int y = std::atoi(xmlROI->first_attribute("y")->value());
		int width = std::atoi(xmlROI->first_attribute("width")->value());
		int height = std::atoi(xmlROI->first_attribute("height")->value());
		float angle = std::atof(xmlROI->first_attribute("angleRotate")->value());
		paramCountPxl.m_tupROI = std::make_tuple(x, y, width, height, angle);
		// 2. Threshold Gray
		XMLElement* xmlThresholdGray = xmlSelROI->first_node("Parameters")->first_node("ThresholdGray");
		paramCountPxl.m_arrThresholdGray[0] = std::atoi(xmlThresholdGray->first_attribute("min")->value());
		paramCountPxl.m_arrThresholdGray[1] = std::atoi(xmlThresholdGray->first_attribute("max")->value());
		// 3. NumberOfPixel
		XMLElement* xmlNumberOfPxl = xmlSelROI->first_node("Parameters")->first_node("NumberOfPixel");
		paramCountPxl.m_arrNumberOfPixel[0] = std::atoi(xmlNumberOfPxl->first_attribute("min")->value());
		paramCountPxl.m_arrNumberOfPixel[1] = std::atoi(xmlNumberOfPxl->first_attribute("max")->value());

		// add param and result into param manager
		vsParamManager.SetParamCntPxl(paramCountPxl);

		// add para manager into CVisionAlgorithms
		vsAlgorithm.SetVsParamManager(vsParamManager);
		vsAlgorithm.SetAlgorithm(algorithm);

		// add param manager and result manager into Select ROI tool
		pSelROITool.SetVsAlgorithms(vsAlgorithm);

		// add tool into queue tools
		m_queueSelROI->push(pSelROITool);

		// release resource
		//delete vsAlgorithm, vsAlgorithm = NULL;

		return;
	}
	else if (strcmp(cAlgorithm, "CalculateArea") == 0)
	{
		algorithm = emCalculateArea;
		// read seletect ROI's infos
		paramSelROI.m_csId = xmlSelROI->first_attribute("id")->value();
		paramSelROI.m_csName = xmlSelROI->first_attribute("name")->value();
		paramSelROI.m_csType = xmlSelROI->first_attribute("type")->value();
		paramSelROI.m_emAlgorithm = algorithm;
		paramSelROI.m_bRotations = (strcmp(xmlSelROI->first_attribute("rotations")->value(), "True") == 0 ? TRUE : FALSE);
		paramSelROI.m_nPriority = std::atoi(xmlSelROI->first_attribute("priority")->value());
		pSelROITool.SetParamSelROI(paramSelROI);

		// create parameter and result of Count Pixel algorithm
		CParameterCalculateArea paramCalArea;

		// 1. ROI
		XMLElement* xmlROI = xmlSelROI->first_node("Parameters")->first_node("ROI");
		int x = std::atoi(xmlROI->first_attribute("x")->value());
		int y = std::atoi(xmlROI->first_attribute("y")->value());
		int width = std::atoi(xmlROI->first_attribute("width")->value());
		int height = std::atoi(xmlROI->first_attribute("height")->value());
		float angle = std::atof(xmlROI->first_attribute("angleRotate")->value());
		paramCalArea.m_tupROI = std::make_tuple(x, y, width, height, angle);
		// 2. Threshold
		XMLElement* xmlThreshold = xmlSelROI->first_node("Parameters")->first_node("Threshold");
		paramCalArea.m_nThreshold = std::atoi(xmlThreshold->first_attribute("value")->value());
		// 3. Area
		XMLElement* xmlArea = xmlSelROI->first_node("Parameters")->first_node("Area");
		paramCalArea.m_arrArea[0] = std::atoi(xmlArea->first_attribute("min")->value());
		paramCalArea.m_arrArea[1] = std::atoi(xmlArea->first_attribute("max")->value());

		// add param and result into param manager
		vsParamManager.SetParamCalArea(paramCalArea);

		// add param manager into CVisionAlgorithms
		vsAlgorithm.SetVsParamManager(vsParamManager);
		vsAlgorithm.SetAlgorithm(algorithm);

		// add to Select ROI tool
		pSelROITool.SetVsAlgorithms(vsAlgorithm);

		// add too to queue tools
		m_queueSelROI->push(pSelROITool);

		// release resource
		//delete vsAlgorithm, vsAlgorithm = NULL;

		return;
	}
}
