#pragma once
#include "JigInspectDefine.h"
#include "CLocatorTool_TemplateMatching_Result.h"

class AFX_EXT_CLASS CJigInspectResults
{
public:
	CJigInspectResults(void);
	~CJigInspectResults(void);

public:
	BOOL m_bInspectCompleted;
	BOOL m_bResultOKNG;
	CLocatorTool_TemplateMatching_Result m_TemplateMatchingResult;
};
