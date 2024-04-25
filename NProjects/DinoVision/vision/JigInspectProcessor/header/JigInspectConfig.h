#pragma once
#include "JigInspectDefine.h"

class AFX_EXT_CLASS CJigInspectSystemConfig
{
public:
	CJigInspectSystemConfig(void);
	~CJigInspectSystemConfig(void);

public:
	TCHAR            m_sRecipePath[MAX_STRING_SIZE];
	TCHAR            m_sModel[MAX_STRING_SIZE];
	TCHAR            m_sCOMPort[MAX_STRING_SIZE];
	BOOL             m_bUsePCControl;
};

class AFX_EXT_CLASS CJigInspectCameraConfig
{
public:
	CJigInspectCameraConfig(void);
	~CJigInspectCameraConfig(void);

public:
	TCHAR             m_sName[MAX_STRING_SIZE];
	TCHAR             m_sInterfaceType[MAX_STRING_SIZE];
	TCHAR             m_sSensorType[MAX_STRING_SIZE];
	int               m_nChannels;
	TCHAR             m_sManufacturer[MAX_STRING_SIZE];
	int               m_nFrameWidth;
	int               m_nFrameHeight;
	TCHAR             m_sSerialNumber[MAX_STRING_SIZE];
	TCHAR             m_sImageSavePath[MAX_STRING_SIZE];
	TCHAR             m_sImageTemplatePath[MAX_STRING_SIZE];
	TCHAR             m_sRecipeName[MAX_STRING_SIZE];
};