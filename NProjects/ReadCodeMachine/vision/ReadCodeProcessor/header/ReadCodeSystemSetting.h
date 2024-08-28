#pragma once
#include "ReadCodeDefine.h"

class AFX_EXT_CLASS CReadCodeSystemSetting
{
public:
	CReadCodeSystemSetting(void);
	~CReadCodeSystemSetting(void);

public:
	BOOL                m_bSaveFullImage;
	BOOL                m_bSaveDefectImage;
	BOOL                m_bShowDetailImage;
	BOOL                m_bSimulation;
	BOOL                m_bByPass;
	TCHAR               m_sFullImagePath[MAX_STRING_SIZE];
	TCHAR               m_sDefectImagePath[MAX_STRING_SIZE];
	TCHAR               m_sTemplateImagePath[MAX_STRING_SIZE];
	TCHAR               m_sModelName[MAX_STRING_SIZE];
	BOOL                m_bTestMode;
};