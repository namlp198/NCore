#pragma once

#include "IOControls.h"

class AFX_EXT_CLASS CIOControls_Simulation : public CIOControls
{
public:
	CIOControls_Simulation(int nIndex = 0, IIOControls2Parent* pIIC2P = NULL, DWORD dwPeriod = 100);
	virtual ~CIOControls_Simulation();

public: // pure virtual function
	virtual int		Connect(const CIOControlsParam& param);			// connect
	virtual int		Disconnect();									// disconnect

protected:
	virtual int		Send_SignalValue(int nAddrIdx, const SignalValue& dwValue);	// send signal
	virtual int		Receive_SignalValue(int nAddrIdx, SignalValue& dwValue);		// receive signal

public: // virtual function
	virtual int		Send_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');	// send data
	virtual int		Receive_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*'); // receive data
};

