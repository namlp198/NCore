#pragma once
#include "NVisionInspectDefine.h"

class AFX_EXT_CLASS CNVisionInspectResult
{
public:
	CNVisionInspectResult(void);
	~CNVisionInspectResult(void);

public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	TCHAR m_sResultString[MAX_STRING_SIZE_RESULT];
};