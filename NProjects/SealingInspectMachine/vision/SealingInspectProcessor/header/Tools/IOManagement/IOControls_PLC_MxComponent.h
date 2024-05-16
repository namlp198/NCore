#pragma once

#include "IOControls.h"

class CMxComponent;

class AFX_EXT_CLASS CIOControls_PLC_MxComponent : public CIOControls
{
public:
	CIOControls_PLC_MxComponent(int nIndex = 0, IIOControls2Parent* pIIC2P = NULL, DWORD dwPeriod = 100);
	virtual ~CIOControls_PLC_MxComponent(void);

public: // pure virtual function
	virtual int		Connect(int nStationNo, const CIOControlsParam& param);		// connect
	virtual int		Disconnect();												// disconnect

	// getter
	virtual int		GetConnected();												// get connection status
	virtual int		Send_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');	// send data
	virtual int		Receive_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*'); // receive data

protected: // virtual function
	virtual int		Send_SignalValue(int nAddrIdx, const SignalValue& dwValue);	// send signal
	virtual int		Receive_SignalValue(int nAddrIdx, SignalValue& dwValue);	// receive signal

private:
	long				m_nStationNum;
	CMxComponent*		m_pMxComponent;

	int					m_nReceiveDataAddrCount;
	int					m_nSendDataAddrCount;
	BYTE*				m_pReceiveData[MAX_SIGNAL_ADDRESS_COUNT];
	BYTE*				m_pSendData[MAX_SIGNAL_ADDRESS_COUNT];
	SMemoryAddress		m_pReceiveDataAddr[MAX_SIGNAL_ADDRESS_COUNT];
	SMemoryAddress		m_pSendDataAddr[MAX_SIGNAL_ADDRESS_COUNT];
};

