#pragma once

#include "SerialPort.h"
#include "LightControls.h"

class AFX_EXT_CLASS CLightControls_ONUV_OLC_304A : public CLightControls, public ISerialPort2Parent
{
public:
	CLightControls_ONUV_OLC_304A(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls_ONUV_OLC_304A(void);

	virtual BOOL Connect(const CLightControlsParam& param);
	virtual void Disconnect();

	// setter
	virtual BOOL SetLightLevel(int nValue, int nChannel = 0);
	virtual BOOL SetLightLevel(double dValue, int nChannel = 0);
	virtual BOOL SetLightStatus(int nValue, int nChannel = 0);
	virtual BOOL SetLightStatusAll(int nValue);
	virtual BOOL SetLightLevelAll(int nValue);

	// getter
	virtual BOOL GetConnected();

public:
	virtual void ISP2P_ReceiveData(LPSTR lpszData, long nLength);

protected:
	BOOL SendData(CHAR* pData, DWORD nSize);

protected:
	CSerialPort* m_pSerialPort;
};