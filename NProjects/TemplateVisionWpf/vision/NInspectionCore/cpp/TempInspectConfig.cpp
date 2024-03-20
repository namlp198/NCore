#include "pch.h"
#include "TempInspectConfig.h"

CTempInspectConfig::CTempInspectConfig(CString configPath) : m_csConfigPath(configPath)
{
	m_csConfigName = "SystemSettings_Template.xml";
	m_csConfigPath = m_csConfigPath + "\\" + m_csConfigName;
}

CTempInspectConfig::~CTempInspectConfig()
{
	::DisposeXMLFile(m_pXmlFile);
	::DisposeXMLObject(m_pXmlDoc);
}

BOOL CTempInspectConfig::Initialize()
{
	if (m_csConfigPath.IsEmpty())
	{
		AfxMessageBox(_T("Config Path cannot empty!"));
		return FALSE;
	}

	CFileFind finder;
	BOOL bRecipeExist = finder.FindFile(m_csConfigPath);
	if (m_csConfigPath.Right(3).CompareNoCase(_T("xml")) != 0 && bRecipeExist == FALSE)
	{
		CString msg = _T("Config file no exist, check again");
		AfxMessageBox(msg);
		return FALSE;
	}

	USES_CONVERSION;
	char cConfigPath[1024] = {};
	sprintf_s(cConfigPath, "%s", W2A(m_csConfigPath));

	std::string error;

	m_pXmlFile = ::OpenXMLFile(cConfigPath, error);
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

	// Find root: SystemSettings
	XMLElement* pRoot = ::FirstOrDefaultElement(m_pXmlDoc, "SystemSettings", error);
	if (!pRoot)
	{
		AfxMessageBox((CString)(error.c_str()));
		::DisposeXMLFile(m_pXmlFile);
		::DisposeXMLObject(m_pXmlDoc);
		return FALSE;
	}

	m_csJobPath = pRoot->first_node("JobSettings")->first_node("JobPath")->value();
	m_csJobName = pRoot->first_node("JobSettings")->first_node("JobName")->value();
	m_csSaveImageFolder = pRoot->first_node("SaveImageFolder")->value();
	m_csTemplateImage = pRoot->first_node("TemplateImage")->value();
}
