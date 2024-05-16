#pragma once

#include "SerialPort.h"
#include "LightControls.h"
#include <vector>

class AFX_EXT_CLASS CLightControls_Dasol_Ch16 : public CLightControls, public ISerialPort2Parent
{
public:
	CLightControls_Dasol_Ch16(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls_Dasol_Ch16(void);

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

	// All ON/OFF
	BOOL SetLightAllOnOFF(bool bOn);
	BOOL SetLightLevel(std::vector<int> nValue);

public:
	virtual void ISP2P_ReceiveData(LPSTR lpszData, long nLength);

protected:
	BOOL SendData(CHAR* pData, DWORD nSize);

protected:
	CSerialPort* m_pSerialPort;
};