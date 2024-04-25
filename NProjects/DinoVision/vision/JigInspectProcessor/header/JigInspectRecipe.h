#pragma once
#include "JigInspectDefine.h"

class AFX_EXT_CLASS CJigInspectRecipe
{
public:
	CJigInspectRecipe(void);
	~CJigInspectRecipe(void);

public:
	TCHAR m_sName[MAX_STRING_SIZE];
	TCHAR m_sAlgorithm[MAX_STRING_SIZE];
};