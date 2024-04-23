#pragma once

class AFX_EXT_CLASS CJigInspectConfig
{
public:
	CJigInspectConfig(void);
	~CJigInspectConfig(void);

public:
		CString           m_csName;
		CString           m_csInterfaceType;
		CString           m_csSensorType;
		int               m_nChannels;
		CString           m_csManufacturer;
		int               m_nFrameWidth;
		int               m_nFrameHeight;
		CString           m_csSerialNumber;
};