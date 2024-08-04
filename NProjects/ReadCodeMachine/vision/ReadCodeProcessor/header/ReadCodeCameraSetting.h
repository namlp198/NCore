#pragma once
#include "ReadCodeDefine.h"

class AFX_EXT_CLASS CReadCodeCameraSetting
{
public:
	CReadCodeCameraSetting(void);
	~CReadCodeCameraSetting(void);

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