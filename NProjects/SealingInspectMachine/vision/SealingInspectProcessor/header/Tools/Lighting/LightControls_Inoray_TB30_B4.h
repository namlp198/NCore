#pragma once

#include "SerialPort.h"
#include "LightControls.h"

class AFX_EXT_CLASS CLightControls_Inoray_TB30_B4 : public CLightControls, public ISerialPort2Parent
{
public:
	CLightControls_Inoray_TB30_B4(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls_Inoray_TB30_B4(void);

public:
	virtual BOOL Connect(const CLightControlsParam& param);
	virtual void Disconnect();

	// setter
	virtual BOOL SetLightLevel(int nValue, int nChannel = 0);
	virtual BOOL SetLightLevel(double dValue, int nChannel = 0);
	virtual BOOL SetLightStatus(int nValue, int nChannel = 0);
	virtual BOOL SetLightStatusAll(int nValue);
	virtual BOOL SetLightLevelAll(int nValue);

public:
	virtual void ISP2P_ReceiveData(LPSTR lpszData, long nLength);

protected:
	BOOL SendData(CHAR* pData, DWORD nSize);

protected:
	CSerialPort* m_pSerialPort;
};

