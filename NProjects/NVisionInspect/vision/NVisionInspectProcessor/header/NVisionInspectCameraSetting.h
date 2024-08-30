#pragma once
#include "NVisionInspectDefine.h"

class AFX_EXT_CLASS CNVisionInspectCameraSetting
{
public:
	CNVisionInspectCameraSetting(void);
	~CNVisionInspectCameraSetting(void);

public:
	BOOL              m_bSaveFullImage;
	BOOL              m_bSaveDefectImage;
	BOOL              m_bShowGraphics;
	int               m_nChannels;
	int               m_nFrameWidth;
	int               m_nFrameHeight;
	int               m_nFrameDepth;
	int               m_nMaxFrameCount;
	int               m_nNumberOfROI;
	TCHAR             m_sCameraName[MAX_STRING_SIZE];
	TCHAR             m_sInterfaceType[MAX_STRING_SIZE];
	TCHAR             m_sSensorType[MAX_STRING_SIZE];
	TCHAR             m_sManufacturer[MAX_STRING_SIZE];
	TCHAR             m_sSerialNumber[MAX_STRING_SIZE];
	TCHAR             m_sModel[MAX_STRING_SIZE];
	TCHAR             m_sFullImagePath[MAX_STRING_SIZE];
	TCHAR             m_sDefectImagePath[MAX_STRING_SIZE];
	TCHAR             m_sTemplateImagePath[MAX_STRING_SIZE];
};