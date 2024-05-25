#pragma once
#include "SealingInspectDefine.h"

class AFX_EXT_CLASS CSealingInspectLightSetting
{
public:
	CSealingInspectLightSetting(void);
	~CSealingInspectLightSetting(void);

public:
	TCHAR          m_sCH1[MAX_STRING_SIZE];
	TCHAR          m_sCH2[MAX_STRING_SIZE];
	TCHAR          m_sCH3[MAX_STRING_SIZE];
	TCHAR          m_sCH4[MAX_STRING_SIZE];
	TCHAR          m_sCH5[MAX_STRING_SIZE];
	TCHAR          m_sCH6[MAX_STRING_SIZE];
};

class AFX_EXT_CLASS CSealingInspectSystemSetting
{
public:
	CSealingInspectSystemSetting(void);
	~CSealingInspectSystemSetting(void);
	
public:
	TCHAR               m_sIPPLC1[MAX_STRING_SIZE];
	TCHAR               m_sIPPLC2[MAX_STRING_SIZE];
	TCHAR               m_sPortPLC1[MAX_STRING_SIZE];
	TCHAR               m_sPortPLC2[MAX_STRING_SIZE];
	TCHAR               m_sIPLightController1[MAX_STRING_SIZE];
	TCHAR               m_sIPLightController2[MAX_STRING_SIZE];
	TCHAR               m_sPortLightController1[MAX_STRING_SIZE];
	TCHAR               m_sPortLightController2[MAX_STRING_SIZE];
	BOOL                m_bSaveFullImage;
	BOOL                m_bSaveDefectImage;
	BOOL                m_bShowDetailImage;
	BOOL                m_bSimulation;
	BOOL                m_bByPass;
	TCHAR               m_sFullImagePath[MAX_STRING_SIZE];
	TCHAR               m_sDefectImagePath[MAX_STRING_SIZE];
	TCHAR               m_sModelName[MAX_STRING_SIZE];

	CSealingInspectLightSetting m_LightSettings[NUMBER_OF_LIGHT_CONTROLLER];
};