#pragma once

#include<map>

#include "BasedConfig.h"

class CFileMap : public CBasedConfig 
{
public:
	CFileMap();
//	CFileMap(char* szFileName);
	virtual ~CFileMap();


	BOOL	Initialize(HKEY hKey, CString strKey, CString strFilename);
	
	//////////////////////////////////////////////////////////////////////////
	BOOL	SetItemValue(CString strName, CString& strValue);
	BOOL	SetItemValue(CString strName, int& nValue);
	BOOL	SetItemValue(CString strName, unsigned short &nValue);
	BOOL	SetItemValue(CString strName, double& dValue);
	
	BOOL	SetItemValue(int nIdx, CString strName, CString& strValue);
	BOOL	SetItemValue(int nIdx, CString strName, int& nValue);
	BOOL	SetItemValue(int nIdx, CString strName, unsigned short &nValue);
	BOOL	SetItemValue(int nIdx, CString strName, double& dValue);
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	int		GetItemValue(CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(CString strName, double& dValue, double dDefault = 0.0);
	
	int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault = 0.0);
	
	//////////////////////////////////////////////////////////////////////////
	
	BOOL	RemoveAllItem();
	BOOL	RemoveItem(CString strName);
	BOOL	RemoveItem(int nIdx, CString strName);
	
	// OhByungGil  Modify -> LogFile Write
	void	SetLogMode(BOOL bMode)					{	m_bLogMode = bMode;	}
	BOOL	GetLogMode()							{	return m_bLogMode;	}
	void	SetLogFilePath(CString strLogFilePath)	{	m_strLogFileName = strLogFilePath;}
	CString GetLogFilePath()						{	return m_strLogFileName;	}
	
	BOOL	WriteToFile();
	
	void	SetRewriteMode(BOOL bRewrite)	{ m_bRewrite = bRewrite; }
	BOOL	GetRewriteMode()				{ return m_bRewrite; }

protected:
	void	WriteLog( CString strName, CString strValue );
	void	WriteLog( CString strName, int& nValue );
	void	WriteLog( CString strName, unsigned short &nValue );
	void	WriteLog( CString strName, double& dValue );

	BOOL	WriteToLogFile(CString& strContents);
	void	StringSeperate(CString& strLine, CString* pStrTitle, CString* pStrData);
private:
	CString				m_strFileName;
	int					m_nItemCount;
	BOOL				m_bRewrite;
	
	CString				m_strLogFileName;	// OhByungGil 100531
	BOOL				m_bLogMode;
	
	CRITICAL_SECTION	m_cs;


	std::map<CString,CString>					m_ItemList;
	std::map<CString,CString>::iterator			m_Iter;
};
