#pragma once

#define SHLM	HKEY_LOCAL_MACHINE
#define SHCU	HKEY_CURRENT_USER
#define SHCR	HKEY_CLASSES_ROOT

#include "BasedConfig.h"

class CRegistery : public CBasedConfig
{
public:
	CRegistery();
	virtual ~CRegistery();

	// Setting 값 적용 함수.
	BOOL	Initialize(HKEY hKey = HKEY_CURRENT_USER, CString strKey = _T(""), CString strFilename = _T(""));

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
	HKEY		m_hKey;
	CString		m_strKey;
	
/*	BOOL	DeleteKey(HKEY hKey, LPCTSTR lpKey);
	
	//////////////////////////////////////////////////////////////////////////
	// 임의 사용 가능 함수.
	BOOL	WriteString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, LPCTSTR lpData);
	BOOL	WriteDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, double dData);
	BOOL	WriteInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nData);
	BOOL	ReadString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, LPTSTR lpRet, DWORD nSize);
	BOOL	ReadDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, double dDefault, double& dValue);
	BOOL	ReadInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nDefault, int& nValue);
	
	BOOL	WriteString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, LPCTSTR lpData);
	BOOL	WriteDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, double dData);
	BOOL	WriteInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, UINT nData);
	BOOL	ReadString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, LPTSTR lpRet, DWORD nSize);
	BOOL	ReadDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, double dDefault, double& dValue);
	BOOL	ReadInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, int nDefault, int& nValue);
	
	BOOL	DeleteValue(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValueName);
	BOOL	DeleteValue(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValueName, int nIdx);
*/
	//////////////////////////////////////////////////////////////////////////
};