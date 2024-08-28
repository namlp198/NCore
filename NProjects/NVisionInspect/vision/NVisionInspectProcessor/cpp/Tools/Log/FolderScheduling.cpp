#include "pch.h"
#include "FolderScheduling.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFolderScheduling::CFolderScheduling()
{
	m_strMainPath = _T("");

	m_bTargetFolder = FALSE;
	m_bTargetFile = FALSE;

	m_pPathInfo = NULL;

	m_nRemainItemCount = 0;
	m_nSubItemCount = 0;
}

CFolderScheduling::CFolderScheduling(CString strPath)
{
	m_strMainPath = strPath;
	
	m_bTargetFolder = FALSE;
	m_bTargetFile = FALSE;

	m_pPathInfo = NULL;

	m_nRemainItemCount = 0;
	m_nSubItemCount = 0;
}

CFolderScheduling::CFolderScheduling(CString strPath, int nTargetType)
{
	m_strMainPath = strPath;
	
	if (nTargetType | TARGETTYPE_FOLDER)
		m_bTargetFolder = TRUE;
	else
		m_bTargetFolder = FALSE;

	if (nTargetType | TARGETTYPE_FILE)
		m_bTargetFile = TRUE;
	else
		m_bTargetFile = FALSE;
	
	m_pPathInfo = NULL;

	m_nRemainItemCount = 0;
	m_nSubItemCount = 0;
}

CFolderScheduling::~CFolderScheduling()
{
	if (m_pPathInfo)
	{
		delete [] m_pPathInfo;
		m_pPathInfo = NULL;
	}
}

void CFolderScheduling::ResetSchedule()
{
	m_strMainPath.Empty();
	
	if (m_pPathInfo)
	{
		delete [] m_pPathInfo;
		m_pPathInfo = NULL;
	}
	m_nSubItemCount = 0;

	m_bTargetFolder = FALSE;
	m_bTargetFile = FALSE;
	m_nRemainItemCount = 0;
}

BOOL CFolderScheduling::SetRemainDelete(CString strPath, int nTargetType, int nRemainItemCount)
{
	m_strMainPath = strPath;

	if (nTargetType & TARGETTYPE_FOLDER)
		m_bTargetFolder = TRUE;
	else
		m_bTargetFolder = FALSE;

	if (nTargetType & TARGETTYPE_FILE)
		m_bTargetFile = TRUE;
	else
		m_bTargetFile = FALSE;

	m_nRemainItemCount = nRemainItemCount;
	
	return TRUE;
}

BOOL CFolderScheduling::CommitSchedule()
{
	// 남김 수만큼 남기고 나머지를 지운다.
	try
	{
		// 항목을 가져와서...
		SearchSubItem(m_strMainPath, FALSE);

		// 지운다.
		if (m_nRemainItemCount <= m_nSubItemCount)
		{
			for (int i = m_nRemainItemCount; i < m_nSubItemCount; i++)
			{
				if (m_pPathInfo[i].s_nItemType == TARGETTYPE_FILE)
					CFile::Remove(m_pPathInfo[i].s_strItemPath);
				else if (m_pPathInfo[i].s_nItemType == TARGETTYPE_FOLDER)
					DeleteFolder(m_pPathInfo[i].s_strItemPath);
				m_pPathInfo[i].Reset();
			}
		}
	}
	catch (...)
	{
		return FALSE;
	}

	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
// Utility Function
BOOL CFolderScheduling::SearchSubItem(const CString &strMainPath, BOOL bIsAscending)
{
	// 기존 리스트 삭제.
	if (m_pPathInfo)
	{
		delete [] m_pPathInfo;
		m_pPathInfo = NULL;
	}

	if (strMainPath.IsEmpty())
		return FALSE;

	CString strPath;
	strPath = strMainPath;
	if(strPath.Right(1) != "\\") 
		strPath += "\\";
	
	CString strFindPath = strPath + _T("*.*");
	CString strDirName;
	CFileFind filefind;
	BOOL bContinue;	
	if(!(bContinue = filefind.FindFile(strFindPath)))
		return FALSE;

	///////////////////////////////////////
	//동적 할당하기 위해 총 하위 디렉토리 갯수를 먼저 알아낸다.
	////////////////////////////////////////
	// 초기화
	m_nSubItemCount = 0;

	while (bContinue)
	{
		bContinue = filefind.FindNextFile();
		if (filefind.IsDots())
			continue;

		//디렉토리이면
		if(m_bTargetFolder && filefind.IsDirectory())
			m_nSubItemCount++;
		if (m_bTargetFile && !filefind.IsDirectory())
			m_nSubItemCount++;
	}

	//////////////////////////////////////////////////////////////////////////
	// 버퍼를 생성하고 집어넣는다.
	// 20160512 yjm 예외처리(m_nSubItemCount 가 0 일 경우 존재)
	if(!(bContinue = filefind.FindFile(strFindPath)) || m_nSubItemCount == 0)
		return FALSE;
	
	m_pPathInfo = new SSubPathInfo[m_nSubItemCount];
	int nCount = 0;
	CTime TMFileCreate;

	while (bContinue)
	{
		bContinue = filefind.FindNextFile();
		if (filefind.IsDots())
			continue;

		//디렉토리이면
		if(m_bTargetFolder && filefind.IsDirectory())
		{
			strDirName = filefind.GetFileName();
			if (filefind.GetLastWriteTime(TMFileCreate))
			{
				m_pPathInfo[nCount].s_strItemPath = strPath + strDirName;
				m_pPathInfo[nCount].s_strItemName = strDirName;
				m_pPathInfo[nCount].s_nCreateDate = (long)TMFileCreate.GetTime();
				m_pPathInfo[nCount].s_nItemType = TARGETTYPE_FOLDER;
				nCount++;
			}
		}
		if (m_bTargetFile && !filefind.IsDirectory())
		{
			strDirName = filefind.GetFileName();
			if (filefind.GetLastWriteTime(TMFileCreate))
			{
				m_pPathInfo[nCount].s_strItemPath = strPath + strDirName;
				m_pPathInfo[nCount].s_strItemName = strDirName;
				m_pPathInfo[nCount].s_nCreateDate = (long)TMFileCreate.GetTime();
				m_pPathInfo[nCount].s_nItemType = TARGETTYPE_FILE;
				nCount++;
			}
		}
	}

	if(!SortSubItem(bIsAscending))		//Sub폴더를 날짜에 맞게 정렬
		return FALSE;

	filefind.Close();

	return TRUE;
}

BOOL CFolderScheduling::SortSubItem(BOOL bIsAscending)
{
	int i,j;

	// 앞쪽이 더 최근 날짜로
	SSubPathInfo SubPathInfo;
	for(i = 0; i < m_nSubItemCount; i++)
	{
		for(j = 0; j < m_nSubItemCount - i - 1 ; j++)
		{
			// 오름차순
			if (bIsAscending && m_pPathInfo[j].s_nCreateDate > m_pPathInfo[j + 1].s_nCreateDate)
			{
				SubPathInfo = m_pPathInfo[j + 1];
				m_pPathInfo[j + 1] = m_pPathInfo[j];
				m_pPathInfo[j] = SubPathInfo;
			}
			// 내림차순
			if (!bIsAscending && m_pPathInfo[j].s_nCreateDate < m_pPathInfo[j + 1].s_nCreateDate)
			{
				SubPathInfo = m_pPathInfo[j + 1];
				m_pPathInfo[j + 1] = m_pPathInfo[j];
				m_pPathInfo[j] = SubPathInfo;
			}
		}
	}

	return TRUE;
}

BOOL CFolderScheduling::DeleteFolder(const CString &strFolder)
{
	SHFILEOPSTRUCT FileOp = {0};
	TCHAR szTemp[MAX_PATH];

	wcscpy_s(szTemp, MAX_PATH, strFolder);
	szTemp[strFolder.GetLength() + 1] = NULL; // NULL문자가 두개 들어가야 한다.

	FileOp.hwnd = NULL;
	FileOp.wFunc = FO_DELETE;
	FileOp.pFrom = NULL;
	FileOp.pTo = NULL;
	FileOp.fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI; // 확인메시지가 안뜨도록 설정
	FileOp.fAnyOperationsAborted = false;
	FileOp.hNameMappings = NULL;
	FileOp.lpszProgressTitle = NULL;
	FileOp.pFrom = szTemp;

	SHFileOperation(&FileOp);

	return true;
}
