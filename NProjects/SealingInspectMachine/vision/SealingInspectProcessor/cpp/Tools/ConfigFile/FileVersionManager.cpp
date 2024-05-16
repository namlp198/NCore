#include "pch.h"
#include "FileVersionManager.h"


CFileVersionManager::CFileVersionManager(void)
{
	m_strVersion = _T("");
	m_nSoftRev = 0;
}


CFileVersionManager::~CFileVersionManager(void)
{
	m_strVersion.Empty();
}


BOOL CFileVersionManager::LoadData( CConfig* pConfig )
{
	ASSERT(pConfig);

	// Version Update ////////////////////////////////////////////////////////
	pConfig->GetItemValue(_T("VERSION"),m_strVersion);
	//////////////////////////////////////////////////////////////////////////

	// SOFTREV
	pConfig->GetItemValue(_T("SOFTREV"), m_nSoftRev);

	return TRUE;
}

BOOL CFileVersionManager::SaveData( CConfig* pConfig )
{
	ASSERT(pConfig);

	// Version Update ////////////////////////////////////////////////////////
	CString strVersion = _T("");
	CTime cuTime = CTime::GetCurrentTime();
	strVersion.Format(_T("%04d%02d%02d%02d%02d%02d"),cuTime.GetYear(),cuTime.GetMonth(),cuTime.GetDay(),cuTime.GetHour(),cuTime.GetMinute(),cuTime.GetSecond());
	pConfig->SetItemValue(_T("VERSION"),strVersion);
	strVersion.Empty();
	//////////////////////////////////////////////////////////////////////////

	// SOFTREV
	m_nSoftRev++;
	if(m_nSoftRev >= INT_MAX)
	{
		m_nSoftRev = 0;
	}
	pConfig->SetItemValue(_T("SOFTREV"), m_nSoftRev);
	return TRUE;
}

CString CFileVersionManager::GetVersion()
{
	return m_strVersion;
}

int CFileVersionManager::GetSoftRev()
{
	return m_nSoftRev;
}
