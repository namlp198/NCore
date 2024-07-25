// IniManager.cpp: implementation of the CIniManager class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "IniManager.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIniManager::CIniManager()
{
	m_strApp.Empty();
	m_strFileName.Empty();
}

CIniManager::~CIniManager()
{

}

int CIniManager::ReadDataStringA(LPCSTR App, LPCSTR  Key, LPCSTR Default, char* Data, DWORD size, LPCSTR filename)
{
	DWORD MAXSIZE = 255;
	DWORD ReturnSize;
	char szGetStringValue[255] = {0, };

//	ClearBuffer(szGetStringValue, MAXSIZE);
//	ClearBuffer(Data, size);


	ReturnSize = GetPrivateProfileStringA(App, Key, Default, szGetStringValue, size, filename);
	
	if((ReturnSize > MAXSIZE) ||  (ReturnSize <= 0))  return 1;
	
	memcpy(Data, szGetStringValue, size);
	
	return 0;
}

int CIniManager::WriteDataStringA(LPCSTR App, LPCSTR  Key, LPCSTR Data, LPCSTR filename)
{
	LPCSTR iniFile = filename;
	
	if(WritePrivateProfileStringA(App,Key,Data,iniFile) == 0)
		return 1;
	
	return 0;
}


int CIniManager::ReadDataStringW(LPCWSTR lpAppName, LPCWSTR  lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName)
{
	DWORD ReturnSize;
	WCHAR  wchGetStringValue[255] = {0, };
	
	//	ClearBuffer(lpGetStringValue, MAXSIZE);
	//	ClearBuffer(lpReturnedString, nSize);
	
	//GetPrivateProfileStringW(
	//	__in_opt LPCWSTR lpAppName,
	//	__in_opt LPCWSTR lpKeyName,
	//	__in_opt LPCWSTR lpDefault,
	//	__out_ecount_part_opt(nSize, return + 1) LPWSTR lpReturnedString,
	//	__in     DWORD nSize,
	//	__in_opt LPCWSTR lpFileName
	//	);
	
	ReturnSize = GetPrivateProfileStringW(lpAppName, lpKeyName, lpDefault, wchGetStringValue, nSize, lpFileName);
	
	if((ReturnSize > nSize) ||  (ReturnSize <= 0))  return 1;
	
	memcpy(lpReturnedString, wchGetStringValue, nSize*sizeof(WCHAR));
	
	return 0;
}

int CIniManager::WriteDataStringW(LPCWSTR lpAppName, LPCWSTR  lpKeyName, LPCWSTR lpString, LPCWSTR lpFileName)
{
	//WritePrivateProfileStringW(
	//	__in_opt LPCWSTR lpAppName,
	//	__in_opt LPCWSTR lpKeyName,
	//	__in_opt LPCWSTR lpString,
	//	__in_opt LPCWSTR lpFileName
	//	);
	
	if(WritePrivateProfileStringW(lpAppName, lpKeyName, lpString, lpFileName) == 0)
		return 1;
	
	return 0;
}

int CIniManager::StringToInteger(LPCTSTR strValue)
{
	return _ttoi(strValue);
}

double CIniManager::StringToDouble(LPCTSTR strValue)
{
	return _tstof(strValue);
}

long CIniManager::StringToLong(LPCTSTR strValue)
{
	return _ttol(strValue);
}

int CIniManager::GetItemValue( CString strName, int& nValue, int nDefault )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault;
	
	strDefault.Format(_T("%d"), nDefault);
	
	ReadDataString(m_strApp, strName, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	nValue = StringToInteger(wchTmpBuf);

	strDefault.Empty();
	return TRUE;
}

int CIniManager::GetItemValue( CString strName, unsigned short& usValue, unsigned short usDefault )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault;
	
	strDefault.Format(_T("%d"), usDefault);
	
	ReadDataString(m_strApp, strName, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	usValue = static_cast<unsigned short>(StringToInteger(wchTmpBuf));

	strDefault.Empty();
	return TRUE;
}

int CIniManager::GetItemValue( CString strName, double& dValue, double dDefault )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault;
	
	strDefault.Format(_T("%d"), dDefault);
	
	ReadDataString(m_strApp, strName, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	dValue = StringToDouble(wchTmpBuf);

	strDefault.Empty();
	return TRUE;
}

int CIniManager::GetItemValue( CString strName, CString& strValue, CString strDefault )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };
	
	ReadDataString(m_strApp, strName, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	strValue = wchTmpBuf;

	return TRUE;
}

int CIniManager::GetItemValue( int nIndex, CString strName, int& nValue, int nDefault /*= 0*/ )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault, strKeyEx;
	
	strDefault.Format(_T("%d"), nDefault);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	ReadDataString(m_strApp, strKeyEx, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	nValue = StringToInteger(wchTmpBuf);


	strDefault.Empty();
	strKeyEx.Empty();

	return TRUE;
}

int CIniManager::GetItemValue( int nIndex, CString strName, unsigned short& usValue, unsigned short usDefault /*= 0*/ )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault, strKeyEx;
	
	strDefault.Format(_T("%d"), usDefault);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	ReadDataString(m_strApp, strKeyEx, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	usValue = StringToInteger(wchTmpBuf);

	strDefault.Empty();
	strKeyEx.Empty();
	return TRUE;
}

int CIniManager::GetItemValue( int nIndex, CString strName, double& dValue, double dDefault /*= 0*/ )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strDefault, strKeyEx;
	
	strDefault.Format(_T("%f"), dDefault);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	ReadDataString(m_strApp, strKeyEx, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	dValue = StringToDouble(wchTmpBuf);

	strDefault.Empty();
	strKeyEx.Empty();

	return TRUE;
}

int CIniManager::GetItemValue( int nIndex, CString strName, CString& strValue, CString strDefault /*= 0*/ )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	int		nMaxLength = 255;
	TCHAR	wchTmpBuf[255] = {0, };

	CString strKeyEx;
	
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	ReadDataString(m_strApp, strKeyEx, strDefault, wchTmpBuf, nMaxLength, m_strFileName);

	strValue = wchTmpBuf;

	strKeyEx.Empty();
	return TRUE;
}

BOOL CIniManager::SetItemValue( CString strName, int& nValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff;
	
	strTmpBuff.Format(_T("%d"), nValue);
	
	WriteDataString(m_strApp, strName,(LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();
	return TRUE;
}

BOOL CIniManager::SetItemValue( CString strName, unsigned short &usValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff;
	
	strTmpBuff.Format(_T("%d"), usValue);

	WriteDataString(m_strApp, strName, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();

	return TRUE;
}

BOOL CIniManager::SetItemValue( CString strName, double& dValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff;
	
	strTmpBuff.Format(_T("%f"), dValue);
	
	WriteDataString(m_strApp, strName, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();

	return TRUE;
}

BOOL CIniManager::SetItemValue( CString strName, CString& strValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff;
	
	strTmpBuff.Format(_T("%s"), strValue);
	
	WriteDataString(m_strApp, strName, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();

	return TRUE;
}

BOOL CIniManager::SetItemValue( int nIndex, CString strName, int& nValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff, strKeyEx;
	
	strTmpBuff.Format(_T("%d"), nValue);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	WriteDataString(m_strApp, strKeyEx,(LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();
	strKeyEx.Empty();

	return TRUE;
}

BOOL CIniManager::SetItemValue( int nIndex, CString strName, unsigned short &usValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff, strKeyEx;
	
	strTmpBuff.Format(_T("%d"), usValue);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	WriteDataString(m_strApp, strKeyEx, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();
	strKeyEx.Empty();
	return TRUE;
}

BOOL CIniManager::SetItemValue( int nIndex, CString strName, double& dValue )
{
	CString strTmpBuff, strKeyEx;
	
	strTmpBuff.Format(_T("%f"), dValue);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	WriteDataString(m_strApp, strKeyEx, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();
	strKeyEx.Empty();

	return TRUE;
}

BOOL CIniManager::SetItemValue( int nIndex, CString strName, CString& strValue )
{
	if(!CheckHeader() || !CheckFileName()) return FALSE;

	CString strTmpBuff, strKeyEx;
	
	strTmpBuff.Format(_T("%s"), strValue);
	strKeyEx.Format(_T("%s_%d"), strName, nIndex);
	
	WriteDataString(m_strApp, strKeyEx, (LPCTSTR)strTmpBuff, m_strFileName);

	strTmpBuff.Empty();
	strKeyEx.Empty();

	return TRUE;
}
/*!
 * \brief
 * 객체 초기화 함수(파일 로드 -> 메모리 적재)
 * 
 * \param hKey
 * 섹션 값으로 사용된다.
 * 
 * \param strKey
 * 사용되지 않는다.
 * 
 * \param strFilename
 * 파일 입출력 할 경우 파일 패스
 * 
 * \returns
 * 성공 : TRUE \n
 * 실패 : FALSE
 * 
 * 
 * INI 형식를 이용한 파일 입출력으로 파일을 열어서 모든 데이터를 CObList로 관리한다.\n
 */
BOOL CIniManager::Initialize(HKEY hKey, CString strKey, CString strFilename)
{
	m_strApp = strKey;	
	m_strFileName = strFilename;

	return TRUE;
}

BOOL CIniManager::CheckHeader()
{
	if(m_strApp.IsEmpty()) return FALSE;
	
	return TRUE;
}

BOOL CIniManager::CheckFileName()
{
	if(m_strFileName.IsEmpty()) return FALSE;
	
	return TRUE;
}


BOOL CIniManager::RemoveAllItem()
{
		return TRUE;
}

BOOL CIniManager::RemoveItem(CString strName)
{
	return TRUE;
}

BOOL CIniManager::RemoveItem(int nIdx, CString strName)
{
	return FALSE;
}

BOOL CIniManager::WriteToFile()
{
	return FALSE;
}

BOOL CIniManager::GetRewriteMode()
{
	return FALSE;
}

void CIniManager::SetLogMode( BOOL bMode )
{
	return;
}
void CIniManager::SetRewriteMode( BOOL bRewrite )
{
	return;
}

BOOL CIniManager::GetLogMode()
{
	return FALSE;	
}

void CIniManager::SetLogFilePath( CString strLogFilePath )
{
	return;
}

CString CIniManager::GetLogFilePath()
{
	return _T("");	
}