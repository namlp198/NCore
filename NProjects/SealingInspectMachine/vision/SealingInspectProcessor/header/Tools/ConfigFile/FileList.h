#pragma once

#include "BasedConfig.h"

class FileListItem : public CObject
{
public:
	CString s_strName;
	CString s_strValue;

	FileListItem()
	{
		Reset();
	}

	void Reset()
	{
		s_strName.Empty();
		s_strValue.Empty();
	}

	void SetItem(CString& strName, CString& strValue)
	{
		s_strName = strName;
		s_strValue = strValue;
	}

	void SetItem(CString& strItem)
	{
		if (strItem.IsEmpty())
			return;

		int nTemp1 = 0, nTemp2 = 0, nTemp3 = 0;

		nTemp1 = strItem.Find('=', 0);
		nTemp2 = strItem.Find('`', 0);
		nTemp3 = strItem.Find('`', nTemp2 + 1);
		if (nTemp1 == -1 || nTemp2 == -1 || nTemp3 == -1)
		{
			s_strName = strItem;

			s_strName.TrimRight(_T("\r"));
			s_strName.TrimRight(_T("\n"));

			return;
		}

		s_strName = strItem.Left(nTemp1);
		s_strName.TrimLeft(_T(" "));
		s_strName.TrimRight(_T(" "));
		s_strName.TrimRight(_T("\r"));
		s_strName.TrimRight(_T("\n"));

		s_strValue.Empty();
		s_strValue = strItem.Mid(nTemp2 + 1, nTemp3 - nTemp2 - 1);
		s_strValue.TrimLeft(_T(" "));
		s_strValue.TrimRight(_T(" "));
		s_strValue.TrimRight(_T("\r"));
		s_strValue.TrimRight(_T("\n"));
	}
};

class CFileList : public CBasedConfig 
{
public:
	CFileList();
//	CFileList(char* szFilename);	// 상대경로
	virtual ~CFileList();
	
	BOOL	Initialize(HKEY hKey, CString strKey, CString strFilename);
	
	//////////////////////////////////////////////////////////////////////////
	BOOL	SetItemValue(CString strName,  CString& strValue);
	BOOL	SetItemValue(CString strName, int& nValue);
	BOOL	SetItemValue(CString strName, unsigned short &sValue);
	BOOL	SetItemValue(CString strName, double& dValue);
	
	BOOL	SetItemValue(int nIdx, CString strName, CString& strValue);
	BOOL	SetItemValue(int nIdx, CString strName, int& nValue);
	BOOL	SetItemValue(int nIdx, CString strName, unsigned short &sValue);
	BOOL	SetItemValue(int nIdx, CString strName, double& dValue);
	
	//////////////////////////////////////////////////////////////////////////
	int		GetItemValue(CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(CString strName, double& dValue, double dDefault = 0);
	
	int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault = 0);
	
	BOOL	RemoveAllItem();
	BOOL	RemoveItem(CString strName);
	BOOL	RemoveItem(int nIdx, CString strName);

	
	BOOL	WriteToFile();

	void	SetRewriteMode(BOOL bRewrite);
	BOOL	GetRewriteMode();
	

	// OhByungGil  Modify -> LogFile Write
	void	SetLogMode(BOOL bMode);
	BOOL	GetLogMode();
	void	SetLogFilePath(CString strLogFilePath);
	CString GetLogFilePath();

protected:
	void	WriteLog( CString strName, CString strValue );
	void	WriteLog( CString strName, int& nValue );
	void	WriteLog( CString strName, unsigned short &nValue );
	void	WriteLog( CString strName, double& dValue );

	BOOL	WriteToLogFile(CString& strContents);

private:
	CString				m_strFileName;
	CObList				m_ItemList;
	int					m_nItemCount;
	BOOL				m_bRewrite;

	CString				m_strLogFileName;	// OhByungGil 100531
	BOOL				m_bLogMode;

	CRITICAL_SECTION	m_cs;
};