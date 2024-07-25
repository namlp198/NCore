// HMConfigFile.cpp: implementation of the CFileList class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "FileList.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFileList::CFileList()
{
	m_bRewrite = FALSE;
	m_strLogFileName = _T("");
	m_bLogMode = FALSE;
	m_nItemCount = 0;
	InitializeCriticalSection(&m_cs);
}

CFileList::~CFileList()
{
	RemoveAllItem();

	DeleteCriticalSection(&m_cs);
}
/*
CFileList::CFileList(char* szFileName)
{
	InitializeCriticalSection(&m_cs);
	Initialize(NULL, NULL, szFileName);
	m_strLogFileName = _T("");
	m_bLogMode = FALSE;
}
*/
BOOL CFileList::RemoveAllItem()
{
	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem)
			delete pItem;
	}

	m_ItemList.RemoveAll();

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
 * CObList를 이용한 파일 입출력으로 파일을 열어서 모든 데이터를 CObList로 관리한다.
 * 
 */
BOOL CFileList::Initialize(HKEY hKey, CString strKey, CString strFilename)
{
	m_strFileName.Empty();
	if (strFilename.IsEmpty())
		return FALSE;

	m_strFileName = strFilename;
	strFilename.Empty();

	m_nItemCount = 0;

	CFile FileI;
	if (!FileI.Open((LPCTSTR)m_strFileName, CFile::modeReadWrite))
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
		FileI.Close();
		strFileAnsi = pstrTemp;
		strFile = static_cast<CString>(strFileAnsi);
		delete [] pstrTemp;
	}

	// Line Parsing
	RemoveAllItem();

	CString strLine;
	int nFindStart = 0;
	int nFindEnd = 0;
	dwSize = strFile.GetLength();
	while ((DWORD)nFindEnd < dwSize)
	{
		nFindEnd = strFile.Find(_T("\r\n"), nFindStart);
		if (nFindEnd == -1)					// 파일의 끝에 \r\n이 없는 경우임.
			nFindEnd = strFile.GetLength();

		strLine.Empty();
		strLine = strFile.Mid(nFindStart, nFindEnd - nFindStart);

		FileListItem* pItem = new FileListItem;
		pItem->SetItem(strLine);
		m_ItemList.AddTail(pItem);
		nFindStart = nFindEnd + 2;
		m_nItemCount++;
	}

	strFile.Empty();
	strFileAnsi.Empty();
	strLine.Empty();

	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
// Item Read

int CFileList::GetItemValue(CString strName, CString& strValue, CString strDefault)
{
	int nRet = 0;

	BOOL bFind = FALSE;
	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem && strName == pItem->s_strName)
		{
			strValue = pItem->s_strValue;
			bFind = TRUE;
			break;
		}
		nRet++;
	}

	if (!bFind)
	{
		strValue = strDefault;
		nRet = -1;
	}

	return nRet;
}

int CFileList::GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = GetItemValue(strFullName, strValue, strDefault);

	strFullName.Empty();

	return bRet;
}

int CFileList::GetItemValue(CString strName, int& nValue, int nDefault)
{
	CString strValue;
	if (GetItemValue(strName, strValue) == -1)
	{
		nValue = nDefault;
		return -1;
	}

	nValue = _tstoi((LPTSTR)(LPCTSTR)strValue);

	return TRUE;
}

int CFileList::GetItemValue(int nIdx, CString strName, int& nValue, int nDefault)
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

int CFileList::GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault)
{
	CString strValue;
	if (GetItemValue(strName, strValue) == -1)
	{
		nValue = sDefault;
		return -1;
	}

	int nVal = _tstoi((LPTSTR)(LPCTSTR)strValue);
	nValue = static_cast<unsigned short>(nVal);

	return TRUE;
}

int CFileList::GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault)
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

int CFileList::GetItemValue(CString strName, double &dValue, double dDefault)
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

int CFileList::GetItemValue(int nIdx, CString strName, double &dValue, double dDefault)
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
BOOL CFileList::SetItemValue(CString strName, CString& strValue)
{
	if (strName.IsEmpty() || strValue.IsEmpty())
		return FALSE;

	EnterCriticalSection(&m_cs);

	WriteLog(strName,strValue);

	// Modify Value
	BOOL bFind = FALSE;
	CString strLine;
	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem && strName == pItem->s_strName)
		{
			pItem->s_strValue = strValue;
			bFind = TRUE;
			break;
		}
	}

	// Add New Value
	if (!bFind)
	{
		pItem = new FileListItem;
		pItem->SetItem(strName, strValue);

		m_ItemList.AddTail(pItem);
		m_nItemCount++;
	}

	strLine.Empty();


	LeaveCriticalSection(&m_cs);


	if (m_bRewrite)
		return WriteToFile();

	return TRUE;
}

BOOL CFileList::SetItemValue(int nIdx, CString strName, CString& strValue)
{
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);

	BOOL bRet = SetItemValue(strFullName, strValue);

	strFullName.Empty();

	return bRet;
}

BOOL CFileList::SetItemValue(CString strName, int& nValue)
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

BOOL CFileList::SetItemValue(int nIdx, CString strName, int& nValue)
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

BOOL CFileList::SetItemValue(CString strName, unsigned short &nValue)
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

BOOL CFileList::SetItemValue(int nIdx, CString strName, unsigned short &nValue)
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

BOOL CFileList::SetItemValue(CString strName, double& dValue)
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

BOOL CFileList::SetItemValue(int nIdx, CString strName, double& dValue)
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

BOOL CFileList::RemoveItem(CString strName)
{
	EnterCriticalSection(&m_cs);

	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem && strName == pItem->s_strName)
		{
			delete pItem;
			m_ItemList.GetPrev(pos); 
			m_ItemList.RemoveAt(pos);
		}
	}

	LeaveCriticalSection(&m_cs);

	return TRUE;
}

BOOL CFileList::RemoveItem(int nIdx, CString strName)
{
	EnterCriticalSection(&m_cs);
	
	CString strFullName;
	strFullName.Format(_T("%s_%d"), strName, nIdx);
	
	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem && strFullName == pItem->s_strName)
		{
			delete pItem;
			m_ItemList.GetPrev(pos); 
			m_ItemList.RemoveAt(pos);
		}
	}
	
	strFullName.Empty();
	LeaveCriticalSection(&m_cs);
	
	return TRUE;
}

BOOL CFileList::WriteToFile()
{
	CFile FileI;
	CString strTemp;

	if (!FileI.Open(m_strFileName, CFile::modeCreate | CFile::modeWrite))
	{
		if (!FileI.Open(m_strFileName, CFile::modeCreate | CFile::modeWrite))
		{
			return FALSE;
		}
	}

	EnterCriticalSection(&m_cs);

	// Write to File
	FileListItem* pItem;
	POSITION pos = m_ItemList.GetHeadPosition();
	CStringA strName = "";
	CStringA strData = "";
	while (pos)
	{
		pItem = static_cast<FileListItem*>(m_ItemList.GetNext(pos));
		if (pItem && !pItem->s_strName.IsEmpty() && !pItem->s_strValue.IsEmpty())
		{
			strName = static_cast<CStringA>(pItem->s_strName);
			strData = static_cast<CStringA>(pItem->s_strValue);
			FileI.Write(strName, strName.GetLength());
			if (!pItem->s_strValue.IsEmpty())
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


	strTemp.Empty();
	return TRUE;
}

void CFileList::WriteLog( CString strName, CString strValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;

	GetItemValue(strName,strTmp);
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
void CFileList::WriteLog( CString strName, int& nValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	int nTmpValue;
	GetItemValue(strName,nTmpValue);
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
void CFileList::WriteLog( CString strName, unsigned short &nValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	unsigned short nTmpValue;
	GetItemValue(strName,nTmpValue);
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
void CFileList::WriteLog( CString strName, double& dValue )
{
	if( m_bLogMode == FALSE )
		return;
	
	if( m_strLogFileName.IsEmpty() == TRUE )
		return;
	
	CString strTmp;
	double dTmpValue;
	GetItemValue(strName,dTmpValue);
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
BOOL CFileList::WriteToLogFile(CString& strContents)
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


void	CFileList::SetLogMode(BOOL bMode)					
{	
	m_bLogMode = bMode;	
}
BOOL	CFileList::GetLogMode()							
{	
	return m_bLogMode;	
}

void	CFileList::SetLogFilePath(CString strLogFilePath)	
{	
	m_strLogFileName = strLogFilePath;
}

CString CFileList::GetLogFilePath()						
{	
	return m_strLogFileName;	
}

void	CFileList::SetRewriteMode(BOOL bRewrite) 
{
	m_bRewrite = bRewrite;
}

BOOL	CFileList::GetRewriteMode() 
{ 
	return m_bRewrite; 
}
