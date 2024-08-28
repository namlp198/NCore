#pragma once
#include "NVisionInspectDefine.h"

class AFX_EXT_CLASS CNVisionInspectCameraSetting
{
public:
	CNVisionInspectCameraSetting(void);
	~CNVisionInspectCameraSetting(void);

public:
	int m_nFrameWidth;
	int m_nFrameHeight;
	int m_nChannel;
	int m_nTriggerMode;
	int m_nTriggerSource;
	double m_nExposureTime;
	double m_nAnalogGain;
	TCHAR m_sSerialNumber[MAX_STRING_SIZE];
};