#pragma once
#include "ReadCodeDefine.h"

class AFX_EXT_CLASS CReadCodeResult
{
public:
	CReadCodeResult(void);
	~CReadCodeResult(void);

public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	TCHAR m_sResultString[MAX_STRING_SIZE_RESULT];
};