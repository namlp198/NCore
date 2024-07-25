// HMRegistery1.cpp: implementation of the CRegistery class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "Registery.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CRegistery::CRegistery()
{
	m_hKey		= NULL;
	m_strKey	= _T("");
}

CRegistery::~CRegistery()
{

}
/*!
 * \brief
 * 객체 초기화 함수(파일 로드 -> 메모리 적재)
 * 
 * \param hKey
 * 메인 키
 * 
 * \param strKey
 * 서브 키
 * 
 * \param strFilename
 * 사용하지 않음
 * 
 * \returns
 * 성공 : TRUE \n
 * 실패 : FALSE
 * 
 * 
 */
BOOL	CRegistery::Initialize(HKEY hKey, CString strKey, CString strFilename)
{ 
	m_hKey = hKey;	
	m_strKey = strKey; 

	return TRUE; 
}

//////////////////////////////////////////////////////////////////////////
// 
BOOL CRegistery::SetItemValue(CString strName, CString& strValue)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;

	CStringA strValueA = (CStringA)strValue;
	char str[255] = {0, };
	sprintf_s(str,255, "%s", strValueA);

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
	{
		strValueA.Empty();
		return FALSE;
	}

	if (RegSetValueEx(key, (LPCTSTR)strName, 0, REG_SZ, (LPBYTE)str, DWORD(strlen(str) + 1) ) != ERROR_SUCCESS)
	{
		strValueA.Empty();
		return FALSE;
	}

	RegCloseKey(key);

	strValueA.Empty();
	return TRUE;
}

BOOL CRegistery::SetItemValue(CString strName, double& dData)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	char str[255] = {0, };
	sprintf_s(str,255, "%10.10f", dData);

	if (RegSetValueEx(key, (LPCTSTR)strName, 0, REG_SZ, (LPBYTE)str, DWORD(strlen(str) + 1)) != ERROR_SUCCESS)
		return FALSE;

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::SetItemValue(CString strName, int& nData)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;
	UINT unData;

	unData = static_cast<UINT>(nData);

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegSetValueEx(key, (LPCTSTR)strName, 0, REG_DWORD, (LPBYTE)&unData, sizeof(UINT)) != ERROR_SUCCESS)
	{
		return FALSE;
	}

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::SetItemValue(CString strName, unsigned short &snData)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;
	UINT unData;

	unData = static_cast<UINT>(snData);

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegSetValueEx(key, (LPCTSTR)strName, 0, REG_DWORD, (LPBYTE)&unData, sizeof(UINT)) != ERROR_SUCCESS)
	{
		return FALSE;
	}

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::SetItemValue(int nIdx, CString strName, CString& strValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = SetItemValue(strFullName, strValue);
	strFullName.Empty();
	return bRet;
}

BOOL CRegistery::SetItemValue(int nIdx, CString strName, double& dData)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = SetItemValue(strFullName, dData);
	strFullName.Empty();
	return bRet;
}

BOOL CRegistery::SetItemValue(int nIdx, CString strName, int& nData)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = SetItemValue(strFullName, nData);
	strFullName.Empty();
	return bRet;
}

BOOL CRegistery::SetItemValue(int nIdx, CString strName, unsigned short &usData)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = SetItemValue(strFullName, usData);
	strFullName.Empty();
	return bRet;
}



int CRegistery::GetItemValue(CString strName, CString& strValue, CString strDefault)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	char strTemp[1024] = {0, };
	DWORD nSize = 1023;
	HKEY key;
	DWORD dwDisp;
	DWORD Size;

	strValue = strDefault;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = nSize;

	if (RegQueryValueEx(key, (LPCTSTR)strName, 0, NULL, (LPBYTE)strTemp, &Size) != ERROR_SUCCESS)
		return FALSE;


	RegCloseKey(key);

	strValue = strTemp;
		
	return TRUE;
}

int CRegistery::GetItemValue(CString strName, double& dValue, double dDefault)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;
	DWORD Size;

	dValue = dDefault;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = 255;
	char str[255] = {0, };
	if (RegQueryValueEx(key, (LPCTSTR)strName, 0, NULL, (LPBYTE)str, &Size) != ERROR_SUCCESS)
		return FALSE;

	RegCloseKey(key);

	char* end;
	dValue = strtod(str, &end);

	return TRUE;
}

int CRegistery::GetItemValue(CString strName, int& nValue, int nDefault)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;
	UINT Result;
	DWORD Size;

	nValue = nDefault;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = sizeof(LONG);

	if (RegQueryValueEx(key, (LPCTSTR)strName, 0, NULL, (LPBYTE)&Result, &Size) != ERROR_SUCCESS)
		return FALSE;

	nValue = Result;

	RegCloseKey(key);

	return TRUE;
}

int CRegistery::GetItemValue(CString strName, unsigned short& nValue, unsigned short nDefault)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;
	UINT Result;
	DWORD Size;

	nValue = nDefault;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = sizeof(LONG);

	if (RegQueryValueEx(key, (LPCTSTR)strName, 0, NULL, (LPBYTE)&Result, &Size) != ERROR_SUCCESS)
		return FALSE;

	nValue = Result;

	RegCloseKey(key);

	return TRUE;
}

int CRegistery::GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = GetItemValue(strFullName, strValue, strDefault);
	strFullName.Empty();
	return bRet;
}

int CRegistery::GetItemValue(int nIdx, CString strName, double& dValue, double dDefault)
{
	double dResult;
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL dRet = GetItemValue(strFullName, dResult, dDefault);

	dValue = dResult;
	strFullName.Empty();
	return dRet;
}

int CRegistery::GetItemValue(int nIdx, CString strName, int& nValue, int nDefault)
{
	int nResult;
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = GetItemValue(strFullName, nResult, nDefault);

	nValue =  nResult;
	strFullName.Empty();
	return bRet;
}

int CRegistery::GetItemValue(int nIdx, CString strName, unsigned short& nValue, unsigned short nDefault)
{
	int nResult;
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = GetItemValue(strFullName, nResult, nDefault);
	
	nValue =  nResult;
	strFullName.Empty();
	return bRet;
}
//////////////////////////////////////////////////////////////////////////
// 지우기
BOOL CRegistery::RemoveAllItem()
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	if (!RegDeleteKey(m_hKey, (LPCTSTR)m_strKey) != ERROR_SUCCESS)
		return FALSE;

	return TRUE;
}

BOOL CRegistery::RemoveItem(CString strNameName)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;

	if (RegCreateKeyEx(m_hKey, (LPCTSTR)m_strKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ | KEY_SET_VALUE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegDeleteValue(key, (LPCTSTR)strNameName) != ERROR_SUCCESS)
		return FALSE;

	return TRUE;
}

BOOL CRegistery::RemoveItem(int nIdx, CString strName)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = RemoveItem(strFullName);
	strFullName.Empty();
	return bRet;
}

BOOL CRegistery::WriteToFile()
{
	return FALSE;
}

BOOL CRegistery::GetRewriteMode()
{
	return FALSE;
}

void CRegistery::SetLogMode( BOOL bMode )
{
	return;
}
void CRegistery::SetRewriteMode( BOOL bRewrite )
{
	return;
}

BOOL CRegistery::GetLogMode()
{
	return FALSE;	
}

void CRegistery::SetLogFilePath( CString strLogFilePath )
{
	return;
}

CString CRegistery::GetLogFilePath()
{
	return _T("");	
}




















/* 쓸 필요가 있을까? eugene
BOOL CRegistery::ReadInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nDefault, int& nValue)
{
	HKEY key;
	DWORD dwDisp;
	UINT Result;
	DWORD Size;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = sizeof(LONG);

	if (RegQueryValueEx(key, lpValue, 0, NULL, (LPBYTE)&Result, &Size) != ERROR_SUCCESS)
		Result = nDefault;
		nValue = Result;

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::ReadDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, double dDefault, double& dValue)
{
	HKEY key;
	DWORD dwDisp;
	DWORD Size;
	dValue = dDefault;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = 255;

	char lpRet[255] = {0, };
	if (RegQueryValueEx(key, lpValue, 0, NULL, (LPBYTE)lpRet, &Size) != ERROR_SUCCESS)
	{
		return FALSE;
	}
	RegCloseKey(key);

	char* end;
	dValue = strtod(lpRet, &end);

	return TRUE;
}

BOOL CRegistery::ReadString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, LPTSTR lpRet, DWORD nSize)
{
	HKEY key;
	DWORD dwDisp;
	DWORD Size;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	Size = nSize;

	if (RegQueryValueEx(key, lpValue, 0, NULL, (LPBYTE)lpRet, &Size) != ERROR_SUCCESS)
	{
//		strcpy(lpRet, lpDefault);
		return FALSE;
	}

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::WriteInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nData)
{
	HKEY key;
	DWORD dwDisp;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegSetValueEx(key, lpValue, 0, REG_DWORD, (LPBYTE)&nData, sizeof(UINT)) != ERROR_SUCCESS)
	{
		return FALSE;
	}

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::WriteDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, double dData)
{
	HKEY key;
	DWORD dwDisp;

	char str[255] = {0, };
	sprintf(str, "%10.10f", dData);
	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegSetValueEx(key, lpValue, 0, REG_SZ, (LPBYTE)str, strlen(str) + 1) != ERROR_SUCCESS)
		return FALSE;

	RegCloseKey(key);

	return TRUE;
}

BOOL CRegistery::WriteString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, LPCTSTR lpData)
{
	HKEY key;
	DWORD dwDisp;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegSetValueEx(key, lpValue, 0, REG_SZ, (LPBYTE)lpData, strlen(lpData) + 1) != ERROR_SUCCESS)
		return FALSE;

	RegCloseKey(key);

	return TRUE;
}


BOOL CRegistery::DeleteKey(HKEY hKey, LPCTSTR lpKey)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	if (!RegDeleteKey(hKey, lpKey) != ERROR_SUCCESS)
		return FALSE;

	return TRUE;
}

BOOL CRegistery::DeleteValue(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValueName)
{
	if (m_hKey == NULL || m_strKey.IsEmpty())
		return FALSE;

	HKEY key;
	DWORD dwDisp;

	if (RegCreateKeyEx(hKey, lpKey, 0, NULL, REG_OPTION_NON_VOLATILE, KEY_READ | KEY_SET_VALUE, NULL, &key, &dwDisp) != ERROR_SUCCESS)
		return FALSE;

	if (RegDeleteValue(key, lpValueName) != ERROR_SUCCESS)
		return FALSE;

	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
// Index로 작업하기.
BOOL CRegistery::WriteString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, LPCTSTR lpData)
{
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	BOOL bRet = WriteString(hKey, lpKey, lpName, lpData);
	delete [] lpName;

	return bRet;
}

BOOL CRegistery::WriteDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, double dData)
{
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	BOOL bRet = WriteDouble(hKey, lpKey, lpName, dData);
	delete [] lpName;

	return bRet;
}

BOOL CRegistery::WriteInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, UINT nData)
{
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	BOOL bRet = WriteInt(hKey, lpKey, lpName, nData);
	delete [] lpName;

	return bRet;
}

BOOL CRegistery::ReadString(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, LPTSTR lpRet, DWORD nSize)
{
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	BOOL bRet = ReadString(hKey, lpKey, lpName, lpRet, nSize);
	delete [] lpName;

	return bRet;
}

BOOL CRegistery::ReadDouble(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, double dDefault, double& dValue)
{
	double dResult;	
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	BOOL dRet = ReadDouble(hKey, lpKey, lpName, dDefault, dResult);
	dValue = dResult;
	
	delete [] lpName;

	return dRet;
}

BOOL CRegistery::ReadInt(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValue, int nIdx, int nDefault, int& nValue)
{
	int nResult;
	char* lpName = new char[strlen(lpValue) + 10];
	ZeroMemory(lpName, strlen(lpValue) + 10);
	sprintf(lpName, "%s_%d", lpValue, nIdx);

	ReadInt(hKey, lpKey, lpName, nDefault, nResult);
	nValue = nResult;
	delete [] lpName;
	
	return TRUE;
}

BOOL CRegistery::DeleteValue(HKEY hKey, LPCTSTR lpKey, LPCTSTR lpValueName, int nIdx)
{
	char* lpName = new char[strlen(lpValueName) + 10];
	ZeroMemory(lpName, strlen(lpValueName) + 10);
	sprintf(lpName, "%s_%d", lpValueName, nIdx);

	BOOL bRet = DeleteValue(hKey, lpKey, lpName);

	delete [] lpName;

	return bRet;
}

*/