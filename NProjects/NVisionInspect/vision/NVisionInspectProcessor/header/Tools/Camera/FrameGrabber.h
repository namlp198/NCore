#pragma once

#include <afxmt.h>
#include "FrameGrabberParam.h"

class AFX_EXT_CLASS CFrameGrabber
{
public:
	CFrameGrabber(int nIndex=0, IFrameGrabber2Parent* pIFG2P=NULL);
	virtual ~CFrameGrabber(void);

public: // pure virtual functions
	// connect disconnect
	virtual int		Connect(const CFrameGrabberParam& grabberParam) = 0;
	virtual int		Disconnect() = 0;
	
	// grab command
	virtual int		StartGrab() = 0;
	virtual int		StopGrab() = 0;

public: // virtual functions
	// trigger command
	virtual int		SendTrigger(int nTriggerCount=1);
	
	// status getter
	virtual int		GetConnected()					{ return m_GrabberStatus.GetStatus_Connected(); }
	virtual int		GetGrabbing()					{ if (!GetConnected()) return 0; return m_GrabberStatus.GetStatus_Grabbing(); }
	virtual int		GetCurFrameIndex()				{ if (!GetConnected()) return -1; return m_GrabberStatus.GetStatus_CurFrameIndex(); }
	virtual int		GetCurFrameCount()				{ if (!GetConnected()) return -1; return m_GrabberStatus.GetStatus_CurFrameCount(); }
	virtual	int		GetTriggerMode(int& nMode)		{ if (!GetConnected()) return -1; nMode = m_GrabberStatus.GetStatus_TriggerMode();		return 1; }
	virtual	int		GetBinningMode(int& nMode)		{ if (!GetConnected()) return -1; nMode = m_GrabberStatus.GetStatus_BinningMode();		return 1; }
	virtual	int		GetTriggerSource(int& nSource)	{ if (!GetConnected()) return -1; nSource = m_GrabberStatus.GetStatus_TriggerSource();	return 1; }
	virtual	int		GetScanDirection(int& nDir)		{ if (!GetConnected()) return -1; nDir = m_GrabberStatus.GetStatus_ScanDirection();		return 1; }
	virtual int		GetExposureTime(double& dTime)	{ if (!GetConnected()) return -1; dTime = m_GrabberStatus.GetStatus_ExposureTime();		return 1; }
	virtual int		GetFrameRate(double& dRate)		{ if (!GetConnected()) return -1; dRate = m_GrabberStatus.GetStatus_FrameRate();		return 1; }
	virtual int		GetAnalogGain(double& dGain)	{ if (!GetConnected()) return -1; dGain = m_GrabberStatus.GetStatus_AnalogGain();		return 1; }

	// status setter
	virtual	int		SetTriggerMode(int nMode)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_TriggerMode(nMode);		return 1; }
	virtual	int		SetBinningMode(int nMode)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_BinningMode(nMode);		return 1; }
	virtual int		SetTriggerSource(int nSource)	{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_TriggerSource(nSource);	return 1; }
	virtual int		SetScanDirection(int nDir)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_ScanDirection(nDir);		return 1; }
	virtual int		SetExposureTime(double dTime)	{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_ExposureTime(dTime);		return 1; }
	virtual int		SetFrameRate(double dRate)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_FrameRate(dRate);			return 1; }
	virtual int		SetAnalogGain(double dGain)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_AnalogGain(dGain);			return 1; }
	virtual int		SetInspectFrame(BOOL bIns)		{ if (!GetConnected() || GetGrabbing()) return -1; m_GrabberStatus.SetStatus_InspectFrame(bIns);		return 1; }

protected: // virtual functions
	// status setter
	virtual void	SetConnected(int nConnected)			{ m_GrabberStatus.SetStatus_Connected(nConnected); }
	virtual void	SetGrabbing(int nGrabbing)				{ m_GrabberStatus.SetStatus_Grabbing(nGrabbing); }
	virtual void	SetCurFrameIndex(int nFrameIndex)		{ m_GrabberStatus.SetStatus_CurFrameIndex(nFrameIndex); }
	virtual void	SetCurFrameCount(int nFrameCount)		{ m_GrabberStatus.SetStatus_CurFrameCount(nFrameCount); }
			
public: // common functions
	const CFrameGrabberStatus*	GetStatus()	const 			{ return &m_GrabberStatus; }
	
protected:
	int IFG2P_FrameGrabbed(int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	int IFG2P_GetFrameBuffer(int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);
	int IFG2P_SendLogMessage(const TCHAR* lpstrFormat, ...);
	
protected:
	int						m_nIndex;

	CFrameGrabberStatus		m_GrabberStatus;
	CCriticalSection*		m_pFrameCriticalSection;

protected:
	IFrameGrabber2Parent*	m_pIFG2P;
};

typedef CFrameGrabber* (*FRAMEGRABBER)(int, int, int, PVOID pPtr);
extern "C" __declspec(dllexport) CFrameGrabber* FrameGrabber_New(int nInterfaceType, int nBayerType, int nCamIndex, PVOID pPtr);
