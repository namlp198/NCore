#pragma once

#include "Config.h"

class CFileVersionManager
{
public:
	CFileVersionManager(void);
	virtual ~CFileVersionManager(void);

	BOOL	LoadData(CConfig* pConfig);
	BOOL	SaveData(CConfig* pConfig);

	CString			GetVersion();
	int				GetSoftRev();
private:
	CString			m_strVersion;
	int				m_nSoftRev;
};

