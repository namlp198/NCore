#pragma once

#include "BasedConfig.h"

class CIniManager : public CBasedConfig
{
public:
	CIniManager();
	virtual ~CIniManager();

	BOOL	Initialize(HKEY hKey, CString strKey, CString strFilename);

	//////////////////////////////////////////////////////////////////////////
	BOOL	SetItemValue(CString strName, int& nValue);
	BOOL	SetItemValue(CString strName, unsigned short &usValue);
	BOOL	SetItemValue(CString strName, double& dValue);
	BOOL	SetItemValue(CString strName, CString& strValue);

	BOOL	SetItemValue(int nIndex, CString strName, int& nValue);
	BOOL	SetItemValue(int nIndex, CString strName, unsigned short &usValue);
	BOOL	SetItemValue(int nIndex, CString strName, double& dValue);
	BOOL	SetItemValue(int nIndex, CString strName, CString& strValue);
	//////////////////////////////////////////////////////////////////////////
	
	//////////////////////////////////////////////////////////////////////////
	int		GetItemValue(CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(CString strName, double& dValue, double dDefault = 0);
	
	int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault = _T(""));
	int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault = 0);
	int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault = 0);
	int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault = 0);
	//////////////////////////////////////////////////////////////////////////

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
	CString m_strApp;
	CString m_strFileName;

	BOOL CheckHeader();
	BOOL CheckFileName();

	int		StringToInteger(LPCTSTR strValue);
	double	StringToDouble(LPCTSTR strValue);
	long	StringToLong(LPCTSTR strValue);


	int		WriteDataStringA(LPCSTR App, LPCSTR  Key, LPCSTR Data, LPCSTR filename);
	int		ReadDataStringA(LPCSTR App, LPCSTR  Key, LPCSTR Default, char* Data, DWORD size, LPCSTR filename);
	
	int		WriteDataStringW(LPCWSTR App, LPCWSTR  Key, LPCWSTR Data, LPCWSTR filename);
	int		ReadDataStringW(LPCWSTR lpAppName, LPCWSTR  lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName);

#ifdef UNICODE
#define WriteDataString  WriteDataStringW
#define ReadDataString  ReadDataStringW
#else
#define WriteDataString  WriteDataStringA
#define ReadDataString  ReadDataStringA
#endif // !UNICODE
};