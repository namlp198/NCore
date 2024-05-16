#include "pch.h"
#include "IOControls_Simulation.h"

CIOControls_Simulation::CIOControls_Simulation(int nIndex /*= 0*/, IIOControls2Parent* pIIC2P /*= NULL*/, DWORD dwPeriod /*= 100*/) : CIOControls(nIndex, pIIC2P, dwPeriod)
{

}

CIOControls_Simulation::~CIOControls_Simulation()
{
}

int CIOControls_Simulation::Connect(const CIOControlsParam& param)
{
	m_curStatus.Reset();
	CIOControlsParam *pParam = dynamic_cast<CIOControlsParam*>(&m_curStatus);
	*pParam = param;
	m_prevStatus = m_curStatus;

	m_curStatus.SetStatus_Connected(ModuleConnect_Connected);

	return 1;
}

int CIOControls_Simulation::Disconnect()
{
	return 1;
}

int CIOControls_Simulation::Send_SignalValue(int nAddrIdx, const SignalValue& dwValue)
{
	return 1;
}

int CIOControls_Simulation::Receive_SignalValue(int nAddrIdx, SignalValue& dwValue)
{
	return 1;
}

int CIOControls_Simulation::Send_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return 1;
}

int CIOControls_Simulation::Receive_DataValue(DWORD dwAddress, BYTE* pData, DWORD dwSize, TCHAR cDevice /*= L'D'*/, TCHAR cDeviceSub /*= L'*'*/)
{
	return 1;
}
