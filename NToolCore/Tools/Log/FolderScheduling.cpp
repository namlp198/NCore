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
	// ���� ����ŭ ����� �������� �����.
	try
	{
		// �׸��� �����ͼ�...
		SearchSubItem(m_strMainPath, FALSE);

		// �����.
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
	// ���� ����Ʈ ����.
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
	//���� �Ҵ��ϱ� ���� �� ���� ���丮 ������ ���� �˾Ƴ���.
	////////////////////////////////////////
	// �ʱ�ȭ
	m_nSubItemCount = 0;

	while (bContinue)
	{
		bContinue = filefind.FindNextFile();
		if (filefind.IsDots())
			continue;

		//���丮�̸�
		if(m_bTargetFolder && filefind.IsDirectory())
			m_nSubItemCount++;
		if (m_bTargetFile && !filefind.IsDirectory())
			m_nSubItemCount++;
	}

	//////////////////////////////////////////////////////////////////////////
	// ���۸� �����ϰ� ����ִ´�.
	// 20160512 yjm ����ó��(m_nSubItemCount �� 0 �� ��� ����)
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

		//���丮�̸�
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

	if(!SortSubItem(bIsAscending))		//Sub������ ��¥�� �°� ����
		return FALSE;

	filefind.Close();

	return TRUE;
}

BOOL CFolderScheduling::SortSubItem(BOOL bIsAscending)
{
	int i,j;

	// ������ �� �ֱ� ��¥��
	SSubPathInfo SubPathInfo;
	for(i = 0; i < m_nSubItemCount; i++)
	{
		for(j = 0; j < m_nSubItemCount - i - 1 ; j++)
		{
			// ��������
			if (bIsAscending && m_pPathInfo[j].s_nCreateDate > m_pPathInfo[j + 1].s_nCreateDate)
			{
				SubPathInfo = m_pPathInfo[j + 1];
				m_pPathInfo[j + 1] = m_pPathInfo[j];
				m_pPathInfo[j] = SubPathInfo;
			}
			// ��������
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
	szTemp[strFolder.GetLength() + 1] = NULL; // NULL���ڰ� �ΰ� ���� �Ѵ�.

	FileOp.hwnd = NULL;
	FileOp.wFunc = FO_DELETE;
	FileOp.pFrom = NULL;
	FileOp.pTo = NULL;
	FileOp.fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI; // Ȯ�θ޽����� �ȶߵ��� ����
	FileOp.fAnyOperationsAborted = false;
	FileOp.hNameMappings = NULL;
	FileOp.lpszProgressTitle = NULL;
	FileOp.pFrom = szTemp;

	SHFileOperation(&FileOp);

	return true;
}
