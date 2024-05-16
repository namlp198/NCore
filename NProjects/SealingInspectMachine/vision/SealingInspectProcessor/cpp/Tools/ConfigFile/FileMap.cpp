// FileManager.cpp: implementation of the CFileMap class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "Filemap.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFileMap::CFileMap()
{
	m_bRewrite = FALSE;
	m_strLogFileName = _T("");
	m_bLogMode = FALSE;
	m_nItemCount = 0;
	m_ItemList.clear();
	InitializeCriticalSection(&m_cs);
}

CFileMap::~CFileMap()
{
	m_ItemList.clear();

	DeleteCriticalSection(&m_cs);
}

// CFileMap::CFileMap(char* szFileName)
// {
// 	InitializeCriticalSection(&m_cs);
// 	Initialize(NULL, NULL, szFileName);
// 	m_strLogFileName = _T("");
// 	m_bLogMode = FALSE;
// }

BOOL CFileMap::RemoveAllItem()
{
	m_ItemList.clear();
	
	return TRUE;
}

/*!
 * \brief
 * 객체 초기화 함수(파일 로드 -> 메모리 적재)
 * 
 * \param hKey
 * 사용되지 않는다.
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
 * STL Map를 이용한 파일 입출력으로 파일을 열어서 모든 데이터를 CObList로 관리한다.\n
 * 처음 데이터의 로드는 느리지만 로드된 데이터에 자주 엑세스 하는 경우 유용
 */
BOOL CFileMap::Initialize(HKEY hKey, CString strKey, CString strFilename)
{
	m_strFileName.Empty();
	if (strFilename.IsEmpty())
		return FALSE;

	m_strFileName = strFilename;
	strFilename.Empty();

	m_nItemCount = 0;
	
	CFile FileI;
	if (!FileI.Open(m_strFileName, CFile::modeReadWrite))
		return FALSE;

	// 파일 읽기.
	CString strFile;
	CStringA strFileAnsi;
	DWORD  dwSize = (DWORD)FileI.GetLength();
	if (dwSize > 0)
	{
		char* pstrTemp = new char[dwSize + 1];
		ZeroMemory(pstrTemp, sizeof(char) * (dwSize + 1) );
		if (0 == FileI.Read(pstrTemp, dwSize))
		{
			FileI.Close();
			delete [] pstrTemp;
			strFile.Empty();
			strFileAnsi.Empty();
			return FALSE;
		}
		strFileAnsi = pstrTemp;
		strFile = static_cast<CString>(strFileAnsi);
		delete [] pstrTemp;
	}
	FileI.Close();
	// Line Parsing
	m_ItemList.clear();

	CString strLine;
	int nFindStart = 0;
	int nFindEnd = 0;

	CString strTitle;
	CString strData;
	dwSize = strFile.GetLength();
	while ((DWORD)nFindEnd < dwSize)
	{
		nFindEnd = strFile.Find(_T("\r\n"), nFindStart);
		if (nFindEnd == -1)					// 파일의 끝에 \r\n이 없는 경우임.
			nFindEnd = strFile.GetLength();

		strLine.Empty();
		strLine = strFile.Mid(nFindStart, nFindEnd - nFindStart);

		strTitle = _T("");
		strData = _T("");

		this->StringSeperate(strLine,&strTitle,&strData);
		m_ItemList.insert(std::pair<CString,CString>(strTitle,strData));
		nFindStart = nFindEnd + 2;
		m_nItemCount++;
	}

	strFile.Empty();
	strFileAnsi.Empty();
	strLine.Empty();
	strTitle.Empty();
	strData.Empty();

	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
// Item Read

int CFileMap::GetItemValue(CString strName, CString& strValue, CString strDefault)
{
	int nRet = 0;

	BOOL bFind = FALSE;

	m_Iter = m_ItemList.find(strName);
	if( m_Iter == m_ItemList.end())
	{
		strValue = strDefault;
		return -1;
	}

	strValue = m_Iter->second;
	return TRUE;

}

int CFileMap::GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = GetItemValue(strFullName, strValue, strDefault);

	strFullName.Empty();

	return bRet;
}

int CFileMap::GetItemValue(CString strName, int& nValue, int nDefault)
{
	CString strValue;

	if (GetItemValue(strName, strValue) == -1)
	{
		nValue = nDefault;

		strValue.Empty();
		return -1;
	}

	nValue = _tstoi((LPTSTR)(LPCTSTR)strValue);

	strValue.Empty();
	return TRUE;
}

int CFileMap::GetItemValue(int nIdx, CString strName, int& nValue, int nDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	if (GetItemValue(strFullName, strValue) == -1)
	{
		nValue = nDefault;

		strFullName.Empty();
		strValue.Empty();
		return -1;
	}

	nValue = _tstoi((LPTSTR)(LPCTSTR)strValue);

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

int CFileMap::GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault)
{
	CString strValue;
	if (GetItemValue(strName, strValue) == -1)
	{
		nValue = sDefault;

		strValue.Empty();
		return -1;
	}

	int nVal = _tstoi((LPTSTR)(LPCTSTR)strValue);
	nValue = static_cast<unsigned short>(nVal);

	strValue.Empty();
	return TRUE;
}

int CFileMap::GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	if (GetItemValue(strFullName, strValue) == -1)
	{
		nValue = sDefault;

		strFullName.Empty();
		strValue.Empty();
		return -1;
	}

	int nVal = _tstoi((LPTSTR)(LPCTSTR)strValue);
	nValue = static_cast<unsigned short>(nVal);

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

int CFileMap::GetItemValue(CString strName, double &dValue, double dDefault)
{
	CString strValue;
	if (GetItemValue(strName, strValue) == -1)
	{
		dValue = dDefault;

		strValue.Empty();
		return -1;
	}

	TCHAR*	end ;
	dValue = _tcstod((LPCTSTR)strValue, &end) ;


	strValue.Empty();
	return TRUE;
}

int CFileMap::GetItemValue(int nIdx, CString strName, double &dValue, double dDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	if (GetItemValue(strFullName, strValue) == -1)
	{
		dValue = dDefault;
		
		strFullName.Empty();
		strValue.Empty();
		return -1;
	}

	TCHAR*	end ;
	dValue = _tcstod((LPCTSTR)strValue, &end) ;

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
// Write Information
BOOL CFileMap::SetItemValue(CString strName, CString& strValue)
{
	if (strName.IsEmpty() || strValue.IsEmpty())
		return FALSE;

	EnterCriticalSection(&m_cs);

	WriteLog(strName, strValue);

	// Modify Value
	BOOL bFind = FALSE;

	
	m_Iter = m_ItemList.find(strName);
	if( m_Iter != m_ItemList.end())
	{
		m_Iter->second = strValue;
		bFind = FALSE;
	}	
	
	// Add New Value
	if (!bFind)
	{
		m_ItemList.insert(std::pair<CString,CString>(strName,strValue));
		m_nItemCount++;
	}

	LeaveCriticalSection(&m_cs);

	if (m_bRewrite)
		return WriteToFile();

	return TRUE;
}

BOOL CFileMap::SetItemValue(int nIdx, CString strName, CString& strValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet =  SetItemValue(strFullName, strValue);
	
	strFullName.Empty();

	return bRet;
}

BOOL CFileMap::SetItemValue(CString strName, int& nValue)
{
	CString strValue;
	strValue.Format(_T("%d"), nValue);

	if (!SetItemValue(strName, strValue))
	{
		strValue.Empty();
		return FALSE;
	}

	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::SetItemValue(int nIdx, CString strName, int& nValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	strValue.Format(_T("%d"), nValue);

	if (!SetItemValue(strFullName, strValue))
	{
		strFullName.Empty();
		strValue.Empty();
		return FALSE;
	}

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::SetItemValue(CString strName, unsigned short &nValue)
{
	CString strValue;
	strValue.Format(_T("%d"), nValue);

	if (!SetItemValue(strName, strValue))
	{
		strValue.Empty();
		return FALSE;
	}

	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::SetItemValue(int nIdx, CString strName, unsigned short &nValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	strValue.Format(_T("%d"), nValue);

	if (!SetItemValue(strFullName, strValue))
	{
		strFullName.Empty();
		strValue.Empty();
		return FALSE;
	}

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::SetItemValue(CString strName, double& dValue)
{
	CString strValue;
	strValue.Format(_T("%f"), dValue);

	if (!SetItemValue(strName, strValue))
	{
		strValue.Empty();
		return FALSE;
	}

	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::SetItemValue(int nIdx, CString strName, double& dValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	CString strValue;
	strValue.Format(_T("%f"), dValue);

	if (!SetItemValue(strFullName, strValue))
	{
		strFullName.Empty();
		strValue.Empty();
		return FALSE;
	}

	strFullName.Empty();
	strValue.Empty();
	return TRUE;
}

BOOL CFileMap::RemoveItem(CString strName)
{
	EnterCriticalSection(&m_cs);

	m_Iter = m_ItemList.find(strName);
	if( m_Iter != m_ItemList.end() )
	{
		m_Iter = m_ItemList.erase(m_Iter);
		m_nItemCount--;
	}

	LeaveCriticalSection(&m_cs);

	return TRUE;
}

BOOL CFileMap::RemoveItem(int nIdx, CString strName)
{
	EnterCriticalSection(&m_cs);
	
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	m_Iter = m_ItemList.find(strFullName);

	if( m_Iter != m_ItemList.end() )
	{
		m_Iter = m_ItemList.erase(m_Iter);
		m_nItemCount--;
	}
	
	strFullName.Empty();
	LeaveCriticalSection(&m_cs);
	
	return TRUE;
}

BOOL CFileMap::WriteToFile()
{
	CFile FileI;
	CString strTemp;

	if (!FileI.Open(m_strFileName, CFile::modeCreate| CFile::modeWrite))
	{
		if (!FileI.Open(m_strFileName, CFile::modeCreate | CFile::modeWrite))
			return FALSE;
	}

	DWORD dwTick = GetTickCount();

	EnterCriticalSection(&m_cs);

	// Write to File

	CStringA strName = "";
	CStringA strData = "";
	for(m_Iter = m_ItemList.begin(); m_Iter != m_ItemList.end(); ++m_Iter)
	{
		strName = static_cast<CStringA>(m_Iter->first);
		strData = static_cast<CStringA>(m_Iter->second);
		if (!strName.IsEmpty() && !strData.IsEmpty())
		{
			FileI.Write(strName, strName.GetLength());
			if (!strData.IsEmpty())
			{
				FileI.Write("=`", 2);
				FileI.Write(strData, strData.GetLength());
				FileI.Write("`", 1);
			}
			FileI.Write("\r\n", 2);
		}
	}
	FileI.Close();

	strName.Empty();
	strData.Empty();
	LeaveCriticalSection(&m_cs);

	TRACE("[CFileMap][WriteToFile] Tick[%d] \n",GetTickCount() - dwTick);

	strTemp.Empty();
	return TRUE;
}

void CFileMap::WriteLog( CString strName, CString strValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;

	GetItemValue(strName, strTmp);

	if( strTmp != strValue)
	{
		// Write Log
		CTime cuTime = CTime::GetCurrentTime();
		CString strContents;
		strContents.Format(_T("%d-%d-%d:%d:%d:%d : [%s] : [%s] -> [%s]"),cuTime.GetYear(),cuTime.GetMonth(),cuTime.GetDay(),cuTime.GetHour(),cuTime.GetMinute(),cuTime.GetSecond(), strName, strTmp, strValue);
		WriteToLogFile(strContents);
		strContents.Empty();
	}

	strTmp.Empty();
	return;
}
void CFileMap::WriteLog( CString strName, int& nValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	int nTmpValue;

	GetItemValue(strName, nTmpValue);

	if( nValue != nTmpValue)
	{
		// Write Log
		CTime cuTime = CTime::GetCurrentTime();
		CString strContents;
		strContents.Format(_T("%d-%d-%d:%d:%d:%d : [%s] : [%d] -> [%d]"),cuTime.GetYear(),cuTime.GetMonth(),cuTime.GetDay(),cuTime.GetHour(),cuTime.GetMinute(),cuTime.GetSecond(), strName, nTmpValue, nValue);
		WriteToLogFile(strContents);
		strContents.Empty();
	}	
	strTmp.Empty();
}
void CFileMap::WriteLog( CString strName, unsigned short &nValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	unsigned short nTmpValue;

	GetItemValue(strName, nTmpValue);

	if( nValue != nTmpValue)
	{
		// Write Log
		CTime cuTime = CTime::GetCurrentTime();
		CString strContents;
		strContents.Format(_T("%d-%d-%d:%d:%d:%d : [%s] : [%d] -> [%d]"),cuTime.GetYear(),cuTime.GetMonth(),cuTime.GetDay(),cuTime.GetHour(),cuTime.GetMinute(),cuTime.GetSecond(), strName, nTmpValue, nValue);
		WriteToLogFile(strContents);
		strContents.Empty();
	}	
	strTmp.Empty();
}
void CFileMap::WriteLog( CString strName, double& dValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	double dTmpValue;

	GetItemValue(strName, dTmpValue);

	if( dValue != dTmpValue)
	{
		// Write Log
		CTime cuTime = CTime::GetCurrentTime();
		CString strContents;
		strContents.Format(_T("%d-%d-%d:%d:%d:%d : [%s] : [%f] -> [%f]"),cuTime.GetYear(),cuTime.GetMonth(),cuTime.GetDay(),cuTime.GetHour(),cuTime.GetMinute(),cuTime.GetSecond(), strName, dTmpValue, dValue);
		WriteToLogFile(strContents);
		strContents.Empty();
	}
	strTmp.Empty();
}

// Log File에 쓰기.
BOOL CFileMap::WriteToLogFile(CString& strContents)
{
	if(m_strLogFileName.IsEmpty() == TRUE || m_bLogMode == FALSE)
		return FALSE;

	CFile File;
	if (!File.Open(m_strLogFileName, CFile::modeWrite))
	{
		if (!File.Open(m_strLogFileName, CFile::modeCreate | CFile::modeWrite))
		{
			return FALSE;
		}
	}

	CStringA strBufA = (CStringA)strContents;

	File.SeekToEnd();
	File.Write(strBufA, strBufA.GetLength());
	File.Write("\r\n", (UINT)strlen("\r\n"));
	File.Close();
	
	return TRUE;
}
void CFileMap::StringSeperate(CString& strLine, CString* pStrTitle, CString* pStrData)
{
	if (strLine.IsEmpty())
		return;

	int nTemp1 = 0, nTemp2 = 0, nTemp3 = 0;

	nTemp1 = strLine.Find('=', 0);
	nTemp2 = strLine.Find('`', 0);
	nTemp3 = strLine.Find('`', nTemp2 + 1);
	if (nTemp1 == -1 || nTemp2 == -1 || nTemp3 == -1)
	{
		*pStrTitle = strLine;
		
		pStrTitle->TrimRight(_T("\r"));
		pStrTitle->TrimRight(_T("\n"));
		
		return;
	}
	
	*pStrTitle = strLine.Left(nTemp1);
	pStrTitle->TrimLeft(_T(" "));
	pStrTitle->TrimRight(_T(" "));
	pStrTitle->TrimRight(_T("\r"));
	pStrTitle->TrimRight(_T("\n"));
	
	pStrData->Empty();
	*pStrData = strLine.Mid(nTemp2 + 1, nTemp3 - nTemp2 - 1);
	pStrData->TrimLeft(_T(" "));
	pStrData->TrimRight(_T(" "));
	pStrData->TrimRight(_T("\r"));
	pStrData->TrimRight(_T("\n"));
}