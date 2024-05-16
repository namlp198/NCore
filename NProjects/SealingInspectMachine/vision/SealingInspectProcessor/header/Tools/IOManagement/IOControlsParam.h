#pragma once

#define MAX_SIGNAL_ADDRESS_COUNT	10
#define MAX_SIGNAL_BIT_COUNT		32
typedef ULONGLONG SignalValue;

enum IOControls_Connect_Status { ModuleConnect_NotConnect = 0, ModuleConnect_Connected = 1 };
enum IOControls_ModuleType { IOControls_Module_Simulation = 0, IOControls_Module_MVSol, IOControls_Module_MxComponent, IOControls_Module_PcControl, IOControls_Module_McProtocol, IOControls_Module_Fastech, IOControls_Module_Ajinextek, IOControls_Module_CCLink, IOControls_ModuleCount };
enum IOControls_SignalType { IOControls_SignalBit = 0, IOControls_SignalByte, IOControls_SignalWord, IOControls_SignalCount };
enum IOControls_DeviceType { IOControls_DeviceNone = 0, IOControls_DeviceUse, IOControls_DeviceCount };

interface IIOControls2Parent
{
	virtual void	DisplayMessage(TCHAR *str, ...) = 0;
	virtual void	IIC2P_ReceiveSignal_ActiveHigh(int nAddrIndex, int nSignalIndex, int nSignalValue) = 0;
	virtual void	IIC2P_ReceiveSignal_ActiveLow(int nAddrIndex, int nSignalIndex, int nSignalValue) = 0;
};

struct SMemoryAddress
{
	SMemoryAddress()
	{
		Reset();
	}

	void Reset()
	{
		cDevice = L'D';
		cDeviceSub = L'*';
		dwAddress = 0;
		dwSize = 0;
	}

	TCHAR	cDevice;
	TCHAR	cDeviceSub;
	DWORD	dwAddress;	// start address
	DWORD	dwSize;		// size of byte
};

class AFX_EXT_CLASS CIOControlsParam
{
public:
	CIOControlsParam(void) { Reset(); }
	virtual ~CIOControlsParam(void)	{ Reset(); }
	void Reset()
	{
		m_bInitSignal = FALSE;
		m_nSignalType = IOControls_SignalBit;
		m_dwMemorySize = 1000;

		m_nAddressReceiveCount = 1;
		m_nAddressSendCount = 1;

		for (int nIdx = 0; nIdx < MAX_SIGNAL_ADDRESS_COUNT; nIdx++)
		{
			m_pAddressReceive[nIdx].Reset();
			m_pAddressSend[nIdx].Reset();
		}
	}

	// getter
	BOOL GetParam_InitSignal() const { return m_bInitSignal; }
	int GetParam_SignalType() const { return m_nSignalType; }
	DWORD GetParam_MemorySize() const { return m_dwMemorySize; }

	int GetParam_AddressReceiveCount() const { return m_nAddressReceiveCount;  }
	int GetParam_AddressSendCount() const { return m_nAddressSendCount; }

	const SMemoryAddress* GetParam_AddressReceive(int nIdx) const { return &m_pAddressReceive[nIdx]; }
	const SMemoryAddress* GetParam_AddressSend(int nIdx) const { return &m_pAddressSend[nIdx]; }
	SMemoryAddress* GetParam_AddressReceive(int nIdx) { return &m_pAddressReceive[nIdx]; }
	SMemoryAddress* GetParam_AddressSend(int nIdx) { return &m_pAddressSend[nIdx]; }

	// setter
	void SetParam_InitSignal(BOOL bInitSignal) { m_bInitSignal = bInitSignal; }
	void SetParam_SignalType(int nType) { m_nSignalType = nType; }
	void SetParam_MemorySize(DWORD dwSize) { m_dwMemorySize = dwSize; }

	void SetParam_AddressReceiveCount(int nCount) { m_nAddressReceiveCount = min(nCount, MAX_SIGNAL_ADDRESS_COUNT); }
	void SetParam_AddressSendCount(int nCount) { m_nAddressSendCount = min(nCount, MAX_SIGNAL_ADDRESS_COUNT); }

	void SetParam_AddressReceive(int nIdx, const SMemoryAddress& addr) { m_pAddressReceive[nIdx] = addr; }
	void SetParam_AddressSend(int nIdx, const SMemoryAddress& addr) { m_pAddressSend[nIdx] = addr; }

protected:
	BOOL			m_bInitSignal;
	int				m_nSignalType;		// bit, byte, word
	DWORD			m_dwMemorySize;		// byte

	int				m_nAddressReceiveCount;
	SMemoryAddress	m_pAddressReceive[MAX_SIGNAL_ADDRESS_COUNT];		// read signal

	int				m_nAddressSendCount;
	SMemoryAddress	m_pAddressSend[MAX_SIGNAL_ADDRESS_COUNT];			// write signal
};

class CIOControlsStatus : public CIOControlsParam
{
public:
	CIOControlsStatus(void)	{ Reset(); }
	virtual ~CIOControlsStatus(void) { Reset(); }
	void Reset()
	{
		CIOControlsParam::Reset();

		for (int nIdx = 0; nIdx < MAX_SIGNAL_ADDRESS_COUNT; nIdx++)
		{
			m_nReceiveSignal[nIdx] = 0;
			m_nSendSignal[nIdx] = 0;
		}
	}

	// getter
	int			GetStatus_Connected() const { return m_nConnected; }
	const SignalValue*	GetStatus_SendSignal(int nIdx) const { return &m_nSendSignal[nIdx]; }
	const SignalValue*	GetStatus_RecevieSignal(int nIdx) const { return &m_nReceiveSignal[nIdx]; }

	SignalValue*	GetStatus_SendSignal(int nIdx) { return &m_nSendSignal[nIdx]; }
	SignalValue*	GetStatus_RecevieSignal(int nIdx) { return &m_nReceiveSignal[nIdx]; }

	// setter
	void		SetStatus_Connected(int nValue)  { m_nConnected = nValue; }
	void		SetStatus_SendSignal(int nIdx, SignalValue nValue)  { m_nSendSignal[nIdx] = nValue; }
	void		SetStatus_RecevieSignal(int nIdx, SignalValue nValue)  { m_nReceiveSignal[nIdx] = nValue; }

protected:
	int				m_nConnected;									// connect status
	SignalValue		m_nReceiveSignal[MAX_SIGNAL_ADDRESS_COUNT];		// receive signal
	SignalValue		m_nSendSignal[MAX_SIGNAL_ADDRESS_COUNT];		// send signal
};