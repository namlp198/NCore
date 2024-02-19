#include "pch.h"
#include "IOControls.h"

CIOControls::CIOControls( int nIndex/*=0*/, IIOControls2Parent* pIIC2P/*=NULL*/, DWORD dwPeriod/*=100*/ ) : m_pIIC2P(pIIC2P), CTimerThreadPool(dwPeriod)
{
	m_prevStatus.Reset();
	m_curStatus.Reset();
	m_listSendSignal.clear();
}

CIOControls::~CIOControls(void)
{
	StopThread();

	m_prevStatus.Reset();
	m_curStatus.Reset();
	m_listSendSignal.clear();
}

BOOL CIOControls::StartThread()
{
	return CreateTimerThread(this);
}

void CIOControls::StopThread()
{
	CTimerThreadPool::StopThread();
}

void CIOControls::TimerThreadProcess( PVOID pParameter )
{
	//printf("Timer IO controller\n");
	if (m_pIIC2P==NULL) return;

	if (GetConnected()!=1) return;

	// receive signal
	for (int nAddrIdx = 0; nAddrIdx < m_curStatus.GetParam_AddressReceiveCount(); nAddrIdx++)
	{
		if (Receive_SignalValue(nAddrIdx, *m_curStatus.GetStatus_RecevieSignal(nAddrIdx)))
		{
			Make_ReceiveSignal(nAddrIdx, *m_curStatus.GetStatus_RecevieSignal(nAddrIdx));

			Process_ReceiveSignal(nAddrIdx, *m_curStatus.GetStatus_RecevieSignal(nAddrIdx));
		}
	}

	// send signal
	for (int nAddrIdx = 0; nAddrIdx < m_curStatus.GetParam_AddressSendCount(); nAddrIdx++)
	{
		if (Process_SendSignal(nAddrIdx, *m_curStatus.GetStatus_SendSignal(nAddrIdx)))
		{
			Send_SignalValue(nAddrIdx, *m_curStatus.GetStatus_SendSignal(nAddrIdx));
		}
	}
}

void CIOControls::Process_ReceiveSignal(int nAddrIdx, const SignalValue& dwCurValue)
{
	if (dwCurValue == *m_prevStatus.GetStatus_RecevieSignal(nAddrIdx))
	{
		return;
	}

	// 처음 접속시 변경되는 io는 무시함. (값은 저장되나, 해당신호 전달은 하지않음)
	if (m_curStatus.GetParam_InitSignal() == TRUE)
	{
		m_prevStatus = m_curStatus;
		m_curStatus.SetParam_InitSignal(FALSE);
		return;
	}

	SignalValue dwBitValue = 1;
	BOOL bCurValue = FALSE;
	BOOL bPrevValue = FALSE;
	for (int nSignalIdx = 0; nSignalIdx < MAX_SIGNAL_BIT_COUNT; nSignalIdx++)
	{
		// compare current and prev
		bCurValue = BOOL(dwBitValue & dwCurValue);
		bPrevValue = BOOL(dwBitValue & *m_prevStatus.GetStatus_RecevieSignal(nAddrIdx));

		// change check
		if (bCurValue != bPrevValue)
		{
			if (bCurValue)
				m_pIIC2P->IIC2P_ReceiveSignal_ActiveHigh(nAddrIdx, nSignalIdx, 1);
			else
				m_pIIC2P->IIC2P_ReceiveSignal_ActiveLow(nAddrIdx, nSignalIdx, 0);
		}

		// bit shift
		dwBitValue = dwBitValue << 1;
	}

	// save prev 
	m_prevStatus.SetStatus_RecevieSignal(nAddrIdx, dwCurValue);
}

int CIOControls::Process_SendSignal(int nAddrIdx, SignalValue& dwValue)
{
	CSingleLock localLock(&m_csSendSignal);
	localLock.Lock();

	if (m_listSendSignal.size() < 1)
	{
		return 0;
	}

	SignalValue nBitValue;
	ListSendSignalIt it = m_listSendSignal.begin();

	while (it != m_listSendSignal.end())
	{
		// addr check
		if (nAddrIdx != it->nAddrIndex)
		{
			++it;
			continue;
		}

		// set bit value
		nBitValue = 1LL << it->nSignalIndex;

		// not off signal
		if (it->bNotOff)
		{
			if (it->nValue == 1) // set on signal
			{
				dwValue |= nBitValue;
			}
			else // set off signal
			{
				dwValue &= (~nBitValue);
			}

			// delete list
			it = m_listSendSignal.erase(it);
			continue;
		}

		// not start?
		if (it->dwStartTime == 0)
		{
			// set on signal
			dwValue |= nBitValue;

			// set start time
			it->dwStartTime = GetTickCount64();
		}
		else // started?
		{
			// on time over?
			if ((GetTickCount64() - it->dwStartTime) >= it->dwOnTime)
			{
				// set off signal
				dwValue &= (~nBitValue);

				// delete list
				it = m_listSendSignal.erase(it);
				continue;
			}
		}

		++it;
	}

	return 1;
}

int CIOControls::Make_ReceiveSignal(int nAddrIdx, SignalValue& dwValue)
{
	CSingleLock startListLock(&m_csRecvSignal);
	startListLock.Lock();

	if (m_listRecvSignal.size() < 1)
	{
		return 0;
	}

	SignalValue nBitValue;
	ListSendSignalIt it = m_listRecvSignal.begin();

	while (it != m_listRecvSignal.end())
	{
		// addr check
		if (nAddrIdx != it->nAddrIndex)
		{
			++it;
			continue;
		}

		// set bit value
		nBitValue = 1LL << it->nSignalIndex;

		// not off signal
		if (it->bNotOff)
		{
			if (it->nValue == 1) // set on signal
			{
				dwValue |= nBitValue;
			}
			else // set off signal
			{
				dwValue &= (~nBitValue);
			}

			// delete list
			it = m_listRecvSignal.erase(it);
			continue;
		}

		// not start?
		if (it->dwStartTime == 0)
		{
			// set on signal
			dwValue |= nBitValue;

			// set start time
			it->dwStartTime = GetTickCount64();
		}
		else // started?
		{
			// on time over?
			if ((GetTickCount64() - it->dwStartTime) >= it->dwOnTime)
			{
				// set off signal
				dwValue &= (~nBitValue);

				// delete list
				it = m_listRecvSignal.erase(it);
				continue;
			}
			else // not over?
			{
				// set on signal
				dwValue |= nBitValue;
			}
		}

		++it;
	}

	return 1;
}

int CIOControls::GetConnected()
{
	return m_curStatus.GetStatus_Connected();
}

const CIOControlsStatus* CIOControls::GetStatus() const
{
	return &m_curStatus;
}

int CIOControls::RecvSignal(int nAddrIdx, int nSigIdx, int nValue, DWORD dwWaitTime /*= 0*/, DWORD dwOnTime /*= 500*/, BOOL bNotOff /*= FALSE*/)
{
	if (nAddrIdx < 0 || nAddrIdx >= MAX_SIGNAL_ADDRESS_COUNT) return 0;
	if (nSigIdx < 0 || nSigIdx >= MAX_SIGNAL_BIT_COUNT) return 0;
	if (1 != m_curStatus.GetStatus_Connected()) return 0;

	CSingleLock localLock(&m_csRecvSignal);
	localLock.Lock();

	SSendSignal newSignal;
	newSignal.nAddrIndex = nAddrIdx;
	newSignal.nSignalIndex = nSigIdx;
	newSignal.nValue = nValue;
	newSignal.dwWaitTime = dwWaitTime;
	newSignal.dwOnTime = dwOnTime;
	newSignal.bNotOff = bNotOff;

	m_listRecvSignal.push_back(newSignal);

	return 1;
}

int CIOControls::SendSignal(int nAddrIdx, int nSigIdx, int nValue, DWORD dwWaitTime /*= 0*/, DWORD dwOnTime /*= 500*/, BOOL bNotOff /*= FALSE*/)
{
	if (nAddrIdx < 0 || nAddrIdx >= MAX_SIGNAL_ADDRESS_COUNT) return 0;
	if (nSigIdx < 0 || nSigIdx >= MAX_SIGNAL_BIT_COUNT) return 0;
	if (1 != m_curStatus.GetStatus_Connected()) return 0;
	
	CSingleLock localLock(&m_csSendSignal);
	localLock.Lock();

	SSendSignal newSignal;
	newSignal.nAddrIndex = nAddrIdx;
	newSignal.nSignalIndex = nSigIdx;
	newSignal.nValue = nValue;
	newSignal.dwWaitTime = dwWaitTime;
	newSignal.dwOnTime = dwOnTime;
	newSignal.bNotOff = bNotOff;

	m_listSendSignal.push_back(newSignal);

	return 1;
}

int CIOControls::GetSendSignalValue(int nAddrIdx, SignalValue& dwValue ) const
{
	dwValue = *m_curStatus.GetStatus_SendSignal(nAddrIdx);
	return 1;
}

int CIOControls::GetReceiveSignalValue(int nAddrIdx, SignalValue& dwValue ) const
{
	dwValue = *m_curStatus.GetStatus_RecevieSignal(nAddrIdx);
	return 1;
}

int CIOControls::GetReceiveSignalStatus(int nAddrIdx, int nSignalIndex) const
{
	SignalValue nBitValue = 1LL << nSignalIndex;
	return ((*m_curStatus.GetStatus_RecevieSignal(nAddrIdx) & nBitValue) == 0) ? 0 : 1;
}

int CIOControls::Send_AsciiValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Send_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}

int CIOControls::Receive_AsciiValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Receive_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}

int CIOControls::Send_ShortValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Send_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}

int CIOControls::Receive_ShortValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Receive_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}

int CIOControls::Send_IntValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Send_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}

int CIOControls::Receive_IntValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return Receive_DataValue(dwAddress, pData, dwSize, cDevice, cDeviceSub);
}
