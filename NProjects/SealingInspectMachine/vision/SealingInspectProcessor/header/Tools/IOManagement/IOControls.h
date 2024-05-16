#pragma once

#include "TimerThreadPool.h"
#include "IOControlsParam.h"
#include <list>

struct SSendSignal
{
	SSendSignal()
	{
		nAddrIndex = 0;
		nSignalIndex = 0;
		nValue = FALSE;
		dwWaitTime = 0;
		dwOnTime = 1000;
		bNotOff = FALSE;
		dwStartTime = 0;
	}

	int		nAddrIndex;
	int		nSignalIndex;
	int		nValue;
	DWORD	dwWaitTime;
	DWORD	dwOnTime;
	BOOL	bNotOff;
	DWORD64	dwStartTime;
};

typedef std::list<SSendSignal>						ListSendSignal;
typedef std::list<SSendSignal>::iterator			ListSendSignalIt;
typedef std::list<SSendSignal>::const_iterator		constListSendSignalIt;

class AFX_EXT_CLASS CIOControls : public CTimerThreadPool
{
public:
	CIOControls(int nIndex=0, IIOControls2Parent* pIIC2P=NULL, DWORD dwPeriod=50);
	virtual ~CIOControls(void);
	virtual BOOL StartThread();
	virtual void StopThread();

public: // pure virtual function
	// virtual int		Connect(const CIOControlsParam& param) = 0;			// connect
	virtual int		Connect(int nStationNo, const CIOControlsParam& param) = 0;							// connect
	virtual int		Disconnect() = 0;										// disconnect

	virtual int		Send_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*') = 0;	// send data
	virtual int		Receive_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*') = 0; // receive data

protected:
	virtual int		Send_SignalValue(int nAddrIdx, const SignalValue& dwValue) = 0;	// send signal
	virtual int		Receive_SignalValue(int nAddrIdx, SignalValue& dwValue) = 0;		// receive signal

public: // virtual function
	// getter
	virtual BOOL	GetConnected();
	virtual const	CIOControlsStatus* GetStatus() const;
	virtual int		RecvSignal(int nAddrIdx, int nSignalIdx, int nValue, DWORD dwWaitTime = 0, DWORD dwOnTime = 500, BOOL bNotOff = FALSE);	// receive signal
	virtual int		SendSignal(int nAddrIdx, int nSignalIdx, int nValue, DWORD dwWaitTime = 0, DWORD dwOnTime = 500, BOOL bNotOff = FALSE);	// send signal

	virtual int		GetSendSignalValue(int nAddrIdx, SignalValue& dwValue) const;
	virtual int		GetReceiveSignalValue(int nAddrIdx, SignalValue& dwValue) const;
	virtual int		GetReceiveSignalStatus(int nAddrIdx, int nIndex) const;

	virtual int		Send_AsciiValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');	// send data
	virtual int		Receive_AsciiValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*'); // receive data
	virtual int		Send_ShortValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');		// send data
	virtual int		Receive_ShortValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');		// receive data
	virtual int		Send_IntValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*');	// send data
	virtual int		Receive_IntValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice = L'D', TCHAR cDeviceSub = L'*'); // receive data

protected: 
	virtual void	TimerThreadProcess(PVOID pParameter);
	virtual void	Process_ReceiveSignal(int nAddrIdx, const SignalValue& dwValue);
	virtual int		Process_SendSignal(int nAddrIdx, SignalValue& dwValue);
	virtual int		Make_ReceiveSignal(int nAddrIdx, SignalValue& dwValue);

protected:
	IIOControls2Parent*		m_pIIC2P;
	CIOControlsStatus		m_prevStatus;
	CIOControlsStatus		m_curStatus;
	CCriticalSection		m_csIOControls;

private:
	CCriticalSection		m_csSendSignal;
	ListSendSignal			m_listSendSignal;
	CCriticalSection		m_csRecvSignal;
	ListSendSignal			m_listRecvSignal;
};

