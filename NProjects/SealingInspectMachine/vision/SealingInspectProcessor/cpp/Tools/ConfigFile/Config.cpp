// HMRegiConfig.cpp: implementation of the CConfig class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "Config.h"

//#ifdef _DEBUG
//#undef THIS_FILE
//static char THIS_FILE[]=__FILE__;
//#define new DEBUG_NEW
//#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CConfig::CConfig()
{
	m_pConfig = NULL;
	m_strFilePath = _T("");
}

CConfig::~CConfig()
{
	if(m_pConfig) {delete m_pConfig; m_pConfig = NULL; }
}

BOOL CConfig::SetLogWriteMode(BOOL bUse,CString strFilePath)
{
	if(strFilePath.IsEmpty() == TRUE)
		return FALSE;
	
	if(m_Classification & FileMap_Mode)
	{
		m_pConfig->SetLogMode(bUse);
		m_pConfig->SetLogFilePath(strFilePath);
	}
	else if(m_Classification & FileManager_Mode)
	{
		m_pConfig->SetLogMode(bUse);
		m_pConfig->SetLogFilePath(strFilePath);
	}

	strFilePath.Empty();
		
	return TRUE;
}

BOOL CConfig::SetRegiConfig(HKEY hKey, CString strKey, TCHAR* szFilename, Grouping Select)
{
	BOOL bRet = TRUE;

	if(m_pConfig) {delete m_pConfig; m_pConfig = NULL; }

	switch(Select)
	{
	case Registery_mode:
		m_pConfig = static_cast<CBasedConfig *>(new CRegistery);
		break;
	case FileMap_Mode:
		m_pConfig = static_cast<CBasedConfig *>(new CFileMap);
		break;
	case FileManager_Mode:
		m_pConfig = static_cast<CBasedConfig *>(new CFileList);
		break;
	case Ini_Mode:
		m_pConfig = static_cast<CBasedConfig *>(new CIniManager);
		break;
	}

	bRet = m_pConfig->Initialize(hKey, strKey, szFilename);
	m_Classification = Select;

	m_strFilePath = szFilename;


	strKey.Empty();
	return bRet;
}

BOOL CConfig::DeleteAllItem()
{
	if(m_pConfig == NULL)
		return FALSE;

	m_pConfig->RemoveAllItem();

	return TRUE;
}

BOOL CConfig::DeleteValue(CString strvalue)
{
	BOOL bRet = FALSE;
	
	bRet = m_pConfig->RemoveItem(strvalue);

	return bRet;
}

BOOL CConfig::DeleteValue(CString strvalue, int nIdx)
{
	BOOL bRet = FALSE;
	
	bRet = m_pConfig->RemoveItem(nIdx, strvalue);
	
	return bRet;

}

BOOL CConfig::SetItemValue(CString strName, CString strValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(strName, strValue);

	return b4;
}
//get 
BOOL CConfig::SetItemValue(CString strName, int nValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(strName, nValue);
	
	return b4;
}

BOOL CConfig::SetItemValue(CString strName, unsigned short sValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(strName, sValue);
	
	return b4;
}

BOOL CConfig::SetItemValue(CString strName, double dValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(strName, dValue);
	
	return b4;
}

BOOL CConfig::SetItemValue(int nIdx, CString strName, CString strValue)
{ 
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(nIdx, strName, strValue);
	
	return b4;

}

BOOL CConfig::SetItemValue(int nIdx, CString strName, int nValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(nIdx, strName, nValue);
	
	return b4;
}

BOOL CConfig::SetItemValue(int nIdx, CString strName, unsigned short sValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(nIdx, strName, sValue);
	
	return b4;
}

BOOL CConfig::SetItemValue(int nIdx, CString strName, double dValue)
{
	BOOL b4 = TRUE;
	
	b4 = m_pConfig->SetItemValue(nIdx, strName, dValue);
	
	return b4;
}

//////////////////////////////////////////////////////////////////////////
int	CConfig::GetItemValue(CString strName, CString& strValue, CString strDefault)
{
	int nRet = -1;
	char strTemp[1024] = {0, };

	nRet = m_pConfig->GetItemValue(strName, strValue, strDefault);
	
	return nRet;
}

int	CConfig::GetItemValue(CString strName, int& nValue, int nDefault)
{
	int nRet = -1;

	nRet = m_pConfig->GetItemValue(strName, nValue, nDefault);
		
	return nRet;
}

int	CConfig::GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault)
{
	int nRet = -1;

	nRet = m_pConfig->GetItemValue(strName, nValue, sDefault);

	return nRet;
}

int	CConfig::GetItemValue(CString strName, double& dValue, double dDefault)
{
	int nRet = -1;

	nRet = m_pConfig->GetItemValue(strName, dValue, dDefault);

	return nRet;
}

int	CConfig::GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault)
{
	int nRet = -1;
	
	nRet = m_pConfig->GetItemValue(nIdx, strName, strValue, strDefault);

	return nRet;
}

int	CConfig::GetItemValue(int nIdx, CString strName, int& nValue, int nDefault)
{
	int nRet = -1;

	nRet = m_pConfig->GetItemValue(nIdx, strName, nValue, nDefault);

	return nRet;
}

int	 CConfig::GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault)
{
	int nRet = -1;
	
	nRet = m_pConfig->GetItemValue(nIdx, strName, nValue, sDefault);

	return nRet;
}

int	 CConfig::GetItemValue(int nIdx, CString strName, double& dValue, double dDefault)
{
	int nRet = -1;

	nRet = m_pConfig->GetItemValue(nIdx, strName, dValue, dDefault);

	return nRet;
}

BOOL CConfig::WriteToFile()
{
	return m_pConfig->WriteToFile();
}

void CConfig::SetRewriteMode( BOOL bRewrite )
{	
	m_pConfig->SetRewriteMode(bRewrite);
}

BOOL CConfig::GetRewriteMode()
{
	return m_pConfig->GetRewriteMode();
}

CString CConfig::GetFilePath()
{
	return m_strFilePath;
}
