#pragma once

#include "rapidxml.hpp"
#include "RapidXMLSTD.hpp"
#include "rapidxml_utils.hpp"

class AFX_EXT_CLASS CTempInspectSystemConfig
{
public:
	CTempInspectSystemConfig(CString configPath);
	virtual ~CTempInspectSystemConfig();

public:
	BOOL Initialize();

public:
	// Getter
	CString                       GetJobPath() { return m_csJobPath; }
	CString                       GetJobName() { return m_csJobName; }
	CString                       GetSaveImageFolder() { return m_csSaveImageFolder; }
	CString                       GetTemplateImage() { return m_csTemplateImage; }

	// Setter
	void                          SetJobPath(CString jobPath) { m_csJobPath = jobPath; }
	void                          SetJobName(CString jobName) { m_csJobName = jobName; }
	void                          SetSaveImageFolder(CString saveImgFolder) { m_csSaveImageFolder = saveImgFolder; }
	void                          SetTemplateImage(CString templateImg) { m_csTemplateImage = templateImg; }

private:
	CString                             m_csConfigPath;
	CString                             m_csConfigName;
			                            
	CString                             m_csJobPath;
	CString                             m_csJobName;
			                            
	CString                             m_csSaveImageFolder;
	CString                             m_csTemplateImage;
};