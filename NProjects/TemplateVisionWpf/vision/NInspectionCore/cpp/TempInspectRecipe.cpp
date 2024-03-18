#include "pch.h"
#include "TempInspectRecipe.h"

CTempInspectRecipe::CTempInspectRecipe()
{
	m_pQueueTools = new std::queue<ITools*>;

	m_pCameraInfos = new CameraInfo;

}

CTempInspectRecipe::~CTempInspectRecipe()
{
	if (m_pQueueTools != NULL)
		delete m_pQueueTools, m_pQueueTools = NULL;

	if (m_pCameraInfos != NULL)
		delete m_pCameraInfos, m_pCameraInfos = NULL;

	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CTempInspectRecipe::LoadRecipe(CString csRecipePath, int nCamIdx)
{
	m_csRecipePath = csRecipePath;
	if (m_csRecipePath.IsEmpty())
	{
		AfxMessageBox(_T("Recipe Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csRecipePath);
	if (m_csRecipePath.Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Recipe no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	USES_CONVERSION;
	char cRecipePath[1024] = {};
	sprintf_s(cRecipePath, "%s", W2A(m_csRecipePath));

	std::string error;

	m_pXmlFile = ::OpenXMLFile(cRecipePath, error);
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
		auto attribId = xmlCam->first_attribute("id");
		char* cid = attribId->value();
		int nid = atoi(cid);

#pragma region Read and save camera infos 

		if (nCamIdx == nid)
		{
			m_pCameraInfos->m_nId = nid;
			// name
			m_pCameraInfos->m_csName = xmlCam->first_attribute("name")->value();
			// interfaceType
			m_pCameraInfos->m_csInterfaceType = xmlCam->first_attribute("interfaceType")->value();
			// sensorType
			m_pCameraInfos->m_csSensorType = xmlCam->first_attribute("sensorType")->value();
			// manufacturer
			m_pCameraInfos->m_csManufacturer = xmlCam->first_attribute("manufacturer")->value();
			// frame width
			m_pCameraInfos->m_nFrameWidth = atoi(xmlCam->first_attribute("frameWidth")->value());
			// frame height
			m_pCameraInfos->m_nFrameHeight = atoi(xmlCam->first_attribute("frameHeight")->value());
			// serialNumber
			m_pCameraInfos->m_csSerialNumber = xmlCam->first_attribute("serialNumber")->value();

#pragma endregion

#pragma region Read Recipe
			XMLElement* xmlRecipe = xmlCam->first_node("Recipe");

			/* read and save recipe info */ 
			// id
			m_nId = atoi(xmlRecipe->first_attribute("id")->value());
			// name
			m_csName = xmlRecipe->first_attribute("name")->value();
			// cameraId parent
			m_csCameraIdParent = xmlRecipe->first_attribute("cameraIdParent")->value();

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

				CLocatorTool* locTool;
				CParameterLocator* paramLocTool;
				/* get parameter locator tool */

				// 1. id
				paramLocTool->m_csId = xmlLoc->first_attribute("id")->value();
				// 2. name
				paramLocTool->m_csName = xmlLoc->first_attribute("name")->value();
				// 3. priority
				paramLocTool->m_nPriority = atoi(xmlLoc->first_attribute("priority")->value());
				// 4. has children
				char* cHasChild = xmlLoc->first_attribute("hasChildren")->value();
				BOOL hasChild = strcmp(cHasChild, "True") == 0 ? TRUE : FALSE;
				paramLocTool->m_bHasChildren = hasChild;
				// 5. children
				paramLocTool->m_csChildren = xmlLoc->first_attribute("children")->value();

				// 6. RectangleInSide
				XMLElement* xmlRectInSide = xmlLoc->first_node("RectangleInSide");
				paramLocTool->m_RectangleInSide[0] = atoi(xmlRectInSide->first_attribute("x")->value()); // x
				paramLocTool->m_RectangleInSide[1] = atoi(xmlRectInSide->first_attribute("y")->value()); // y
				paramLocTool->m_RectangleInSide[2] = atoi(xmlRectInSide->first_attribute("width")->value()); // width
				paramLocTool->m_RectangleInSide[3] = atoi(xmlRectInSide->first_attribute("height")->value()); // height

				// 7. RectangleOutSide
				XMLElement* xmlRectOutSide = xmlLoc->first_node("RectangleOutSide");
				paramLocTool->m_RectangleOutSide[0] = atoi(xmlRectOutSide->first_attribute("x")->value()); // x
				paramLocTool->m_RectangleOutSide[1] = atoi(xmlRectOutSide->first_attribute("y")->value()); // y
				paramLocTool->m_RectangleOutSide[2] = atoi(xmlRectOutSide->first_attribute("width")->value()); // width
				paramLocTool->m_RectangleOutSide[3] = atoi(xmlRectOutSide->first_attribute("height")->value()); // height

				// 8. Data train
				XMLElement* xmlDataTrain = xmlLoc->first_node("DataTrain");
				paramLocTool->m_DataTrain[0] = atoi(xmlDataTrain->first_attribute("x")->value());
				paramLocTool->m_DataTrain[1] = atoi(xmlDataTrain->first_attribute("y")->value());

				// add parameter to locator tool
				locTool->SetParamLoca(paramLocTool);

				// add tool to queue tools
				m_pQueueTools->push(locTool);

				delete locTool;
				free(locTool);
				delete paramLocTool;
				free(paramLocTool);
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
