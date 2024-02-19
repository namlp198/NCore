#include "pch.h"
#include "IOControls_PLC_MxComponent.h"
#include "MxComponent.h"

CIOControls_PLC_MxComponent::CIOControls_PLC_MxComponent(int nIndex /*= 0*/, IIOControls2Parent* pIIC2P /*= NULL*/, DWORD dwPeriod /*= 100*/) : CIOControls(nIndex, pIIC2P, dwPeriod)
{
	m_nStationNum = -1;

	m_nReceiveDataAddrCount = 0;
	m_nSendDataAddrCount = 0;
	for (int nIdx = 0; nIdx < MAX_SIGNAL_ADDRESS_COUNT; nIdx++)
	{
		m_pReceiveData[nIdx] = nullptr;
		m_pSendData[nIdx] = nullptr;
		m_pReceiveDataAddr[nIdx].Reset();
		m_pSendDataAddr[nIdx].Reset();
	}

	// new
	m_pMxComponent = new CMxComponent();
}

CIOControls_PLC_MxComponent::~CIOControls_PLC_MxComponent(void)
{
	Disconnect();

	// delete 
	if (m_pMxComponent)
	{
		delete m_pMxComponent;
	}
	m_pMxComponent = nullptr;

	// delete 
	for (int nIdx = 0; nIdx < MAX_SIGNAL_ADDRESS_COUNT; nIdx++)
	{
		if (m_pReceiveData[nIdx]) delete[]  m_pReceiveData[nIdx];
		m_pReceiveData[nIdx] = nullptr;

		if (m_pSendData[nIdx]) delete[]  m_pSendData[nIdx];
		m_pSendData[nIdx] = nullptr;

		m_pReceiveDataAddr[nIdx].Reset();
		m_pSendDataAddr[nIdx].Reset();
	}
	m_nReceiveDataAddrCount = 0;
	m_nSendDataAddrCount = 0;

	m_nStationNum = -1;
}

int CIOControls_PLC_MxComponent::Connect(int nStationNo, const CIOControlsParam& param)
{
	m_curStatus.Reset();
	CIOControlsParam* pParam = dynamic_cast<CIOControlsParam*>(&m_curStatus);
	*pParam = param;

	m_nStationNum = nStationNo;

	if (FALSE == m_pMxComponent->Open(m_nStationNum))
	{
		return 0;
	}

	// set address
	m_nReceiveDataAddrCount = m_curStatus.GetParam_AddressReceiveCount();
	m_nSendDataAddrCount = m_curStatus.GetParam_AddressSendCount();

	for (int nIdx = 0; nIdx < m_nReceiveDataAddrCount; nIdx++)
	{
		m_pReceiveDataAddr[nIdx] = *m_curStatus.GetParam_AddressReceive(nIdx);
		m_pReceiveData[nIdx] = new BYTE[m_pReceiveDataAddr[nIdx].dwSize + 1];
		memset(m_pReceiveData[nIdx], 0, sizeof(BYTE)*(m_pReceiveDataAddr[nIdx].dwSize + 1));
	}

	for (int nIdx = 0; nIdx < m_nReceiveDataAddrCount; nIdx++)
	{
		m_pSendDataAddr[nIdx] = *m_curStatus.GetParam_AddressSend(nIdx);
		m_pSendData[nIdx] = new BYTE[m_pSendDataAddr[nIdx].dwSize + 1];
		memset(m_pSendData[nIdx], 0, sizeof(BYTE)*(m_pSendDataAddr[nIdx].dwSize + 1));
	}
	
	m_curStatus.SetStatus_Connected(ModuleConnect_Connected);

	return 1;
}

int CIOControls_PLC_MxComponent::Disconnect()
{
	CSingleLock localLock(&m_csIOControls);
	localLock.Lock();

	if (1 != GetConnected())
	{
		return 0;
	}

	if (m_pMxComponent)
	{
		m_pMxComponent->Close();
	}
	
	m_curStatus.SetStatus_Connected(ModuleConnect_NotConnect);

	return 1;
}

int CIOControls_PLC_MxComponent::GetConnected()
{
	if (m_pMxComponent)
	{
		return m_pMxComponent->IsOpen();
	}

	return 0;
}

int CIOControls_PLC_MxComponent::Send_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	CSingleLock localLock(&m_csIOControls);
	localLock.Lock();

	if (1 != GetConnected())
	{
		return 0;
	}
	   	
	LONG lRet = -1;
	CString strDevice = L"";
	if (cDeviceSub == L'*')
		strDevice.Format(L"%c%d", cDevice, dwAddress);
	else
		strDevice.Format(L"%c%c%d", cDevice, cDeviceSub, dwAddress);

	LONG nLength = dwSize;
	if (nLength % 2) nLength += 1;
	nLength = nLength / 2;

	try
	{
		short sValue = 100;
		BSTR szDeviceAddr = strDevice.AllocSysString();
		HRESULT hr = m_pMxComponent->WriteDeviceBlock2(szDeviceAddr, nLength, (SHORT*)pData, &lRet);
		if (SUCCEEDED(hr))
		{
			TRACE(_T("success"));
		}
	}
	catch (COleDispatchException *Exception)
	{
		TRACE(_T("OLE Dispatch Exception Error! - Write\n"));
		Exception->Delete();
		return FALSE;
	}

	return lRet == 0 ? 1 : 0;
}

int CIOControls_PLC_MxComponent::Receive_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	CSingleLock localLock(&m_csIOControls);
	localLock.Lock();

	if (1 != GetConnected())
	{
		return 0;
	}

	if (dwSize % 2)
	{
		return 0;
	}

	LONG lRet = -1;
	CString strDevice = L"";
	if (cDeviceSub == L'*')
		strDevice.Format(L"%c%d", cDevice, dwAddress);
	else
		strDevice.Format(L"%c%c%d", cDevice, cDeviceSub, dwAddress);

	LONG nLength = dwSize/2;

	try
	{
		BSTR szDeviceAddr = strDevice.AllocSysString();
		m_pMxComponent->ReadDeviceBlock2(szDeviceAddr, nLength, (SHORT*)pData, &lRet);
	}
	catch (COleDispatchException *Exception)
	{
		TRACE(_T("OLE Dispatch Exception Error! - READ\n"));
		Exception->Delete();
		return 0;
	}

	return lRet == 0 ? 1 : 0;
}

int CIOControls_PLC_MxComponent::Send_SignalValue(int nAddrIdx, const SignalValue& dwValue)
{
	SMemoryAddress *pSendDataAddr = &(m_pSendDataAddr[nAddrIdx]);
	BYTE *pSendData = m_pSendData[nAddrIdx];

	// make send data 
	switch (m_curStatus.GetParam_SignalType())
	{
	case IOControls_SignalBit: // bit -> bit
		memcpy(pSendData, &dwValue, sizeof(BYTE)*pSendDataAddr->dwSize);
		break;

	case IOControls_SignalByte: // bit -> byte
	{
		SignalValue Value;

		// byte parsing
		for (DWORD i = 0; i < pSendDataAddr->dwSize; i++)
		{
			Value = dwValue & (1i64 << i);
			Value = Value >> i;

			memcpy(pSendData + i, &Value, sizeof(BYTE));
		}
		break;
	}

	case IOControls_SignalWord: // bit -> word
	{
		SignalValue Value;

		// word parsing
		for (DWORD i = 0; i < pSendDataAddr->dwSize; i = i + 2)
		{
			Value = dwValue & (1i64 << (i / 2));
			Value = Value >> (i / 2);

			memcpy(pSendData + i, &Value, sizeof(WORD));
		}
		break;
	}

	default:
		return 0;
		break;
	}

	// write data
	return Send_DataValue(pSendDataAddr->dwAddress, pSendData, pSendDataAddr->dwSize, pSendDataAddr->cDevice, pSendDataAddr->cDeviceSub);
}

int CIOControls_PLC_MxComponent::Receive_SignalValue(int nAddrIdx, SignalValue& dwValue)
{
	SMemoryAddress *pReceiveDataAddr = &(m_pReceiveDataAddr[nAddrIdx]);
	BYTE *pReceiveData = m_pReceiveData[nAddrIdx];

	// read data
	if (0 == Receive_DataValue(pReceiveDataAddr->dwAddress, pReceiveData, pReceiveDataAddr->dwSize, pReceiveDataAddr->cDevice, pReceiveDataAddr->cDeviceSub))
	{
		return 0;
	}

	// signal type
	switch (m_curStatus.GetParam_SignalType())
	{
	case IOControls_SignalBit: // bit -> bit
		memcpy(&dwValue, pReceiveData, sizeof(BYTE)*pReceiveDataAddr->dwSize);
		break;

	case IOControls_SignalByte: // byte -> bit
	{
		dwValue = 0;
		BYTE Value;

		// byte parsing
		for (DWORD i = 0; i < pReceiveDataAddr->dwSize; i++)
		{
			memcpy(&Value, pReceiveData + i, sizeof(BYTE));

			if (Value > 0)
				dwValue += (1i64 << i);
		}

		break;
	}

	case IOControls_SignalWord: // word -> bit
	{
		dwValue = 0;
		WORD Value;

		// word parsing
		for (DWORD i = 0; i < pReceiveDataAddr->dwSize; i = i + 2)
		{
			memcpy(&Value, pReceiveData + i, sizeof(WORD));

			if (Value > 0)
				dwValue += (1i64 << (i / 2));
		}
		break;
	}

	default:
		return 0;
		break;
	}

	return 1;
}

