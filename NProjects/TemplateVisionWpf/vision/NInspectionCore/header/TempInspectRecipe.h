#pragma once

#include "interface_vision.h"
#include "TempInspectDefine.h"
#include "TempInspectConfig.h"
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

typedef std::queue<ITools> QueueTools;
typedef std::queue<CLocatorTool> QueueLocTools;
typedef std::queue<CSelectROITool> QueueSelROITools;

class AFX_EXT_CLASS CTempInspectRecipe
{
public:
	CTempInspectRecipe();
	~CTempInspectRecipe();

public:
	BOOL LoadRecipe(int nCamIdx);

public:
	// Getter

	QueueLocTools*                      GetQueueLocTools() { return m_queueLoc; }
	QueueSelROITools*                   GetQueueSelROITools() { return m_queueSelROI; }
	CameraInfo*                         GetCameraInfos() { return m_pCameraInfos; }
	CString                             GetCameraIdParent() { return m_csCameraIdParent; }

	// Setter

protected:
	void ReadCameraInfo(XMLElement* xmlCam, int nId);
	void ReadRecipeInfo(XMLElement* xmlRecipe);
	void ReadLocTool(XMLElement* xmlLoc);
	void ReadSelROITool(XMLElement* xmlSelROI, char* algorithm);

private:

	// info recipe
	int                              m_nId;
	CString                          m_csName;
	CString                          m_csCameraIdParent;
									 
	// config path					 
	CString                          m_csConfigPath;
	CString                          m_csJobPath;
	CString                          m_csJobName;
									 
	// queue the tools				 
	QueueLocTools*                      m_queueLoc;
	QueueSelROITools*                   m_queueSelROI;

	// xml
	XMLFile*                         m_pXmlFile;
	XMLDocument_2*                   m_pXmlDoc;

	// camera info
	CameraInfo*                      m_pCameraInfos;

	// CTempInspectConfig
	CTempInspectConfig*              m_pTempInspConfig;
};