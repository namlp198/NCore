#pragma once
#include "NVisionInspectDefine.h"

class AFX_EXT_CLASS CNVisionInspectSystemSetting
{
public:
	CNVisionInspectSystemSetting(void);
	~CNVisionInspectSystemSetting(void);

public:
	int                 m_nInspectCameraCount;
	BOOL                m_bSimulation;
	BOOL                m_bByPass;
	BOOL                m_bTestMode;
	TCHAR               m_sRecipeName[MAX_STRING_SIZE];
};