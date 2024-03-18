#pragma once

#include "interface_vision.h"
#include "TempInspectDefine.h"
#include "LocatorTool.h"
#include "SelectROITool.h"
#include <queue>
#include "rapidxml.hpp"
#include "rapidxml_utils.hpp"
#include "RapidXMLSTD.hpp"

struct CameraInfo
{
	int               m_nId;
	CString           m_csName;
	CString           m_csInterfaceType;
	CString           m_csSensorType;
	CString           m_csManufacturer;
	int               m_nFrameWidth;
	int               m_nFrameHeight;
	CString           m_csSerialNumber;
};

typedef std::queue<ITools*>* QueueToolsPtr;

class AFX_EXT_CLASS CTempInspectRecipe
{
public:
	CTempInspectRecipe();
	~CTempInspectRecipe();

public:
	BOOL LoadRecipe(CString csRecipePath, int nCamIdx);

public:
	// Getter
	QueueToolsPtr                      GetQueueTools() { return m_pQueueTools; }
	CameraInfo*                        GetCameraInfos() { return m_pCameraInfos; }
	CString                            GetCameraIdParent() { return m_csCameraIdParent; }

private:

	// info recipe
	int                              m_nId;
	CString                          m_csName;
	CString                          m_csCameraIdParent;
									 
	// recipe path					 
	CString                          m_csRecipePath;
									 
	// queue the tools				 
	QueueToolsPtr                    m_pQueueTools;

	// xml
	XMLFile*                         m_pXmlFile;
	XMLDocument_2*                   m_pXmlDoc;

	// camera info
	CameraInfo*                      m_pCameraInfos;
};