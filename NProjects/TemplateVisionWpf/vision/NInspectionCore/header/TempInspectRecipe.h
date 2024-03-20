#pragma once

#include "interface_vision.h"
#include "TempInspectDefine.h"
#include "TempInspectSystemConfig.h"
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
	CString                             GetJobPath() { return m_csJobPath; }
	CString                             GetJobName() { return m_csJobName; }

	// Setter
	void                                SetJobPath(CString jobPath) { m_csJobPath = jobPath; }
	void                                SetJobName(CString jobName) { m_csJobName = jobName; }

protected:
	void ReadCameraInfo(XMLElement* xmlCam, int nId);
	void ReadRecipeInfo(XMLElement* xmlRecipe);
	void ReadLocTool(XMLElement* xmlLoc);
	void ReadSelROITool(XMLElement* xmlSelROI, char* algorithm);

private:

	// info recipe
	int                                 m_nId;
	CString                             m_csName;
	CString                             m_csCameraIdParent;
									    
	CString                             m_csJobPath;
	CString                             m_csJobName;
									 
	// queue the tools				 
	QueueLocTools*                      m_queueLoc;
	QueueSelROITools*                   m_queueSelROI;

	// camera info
	CameraInfo*                         m_pCameraInfos;
};