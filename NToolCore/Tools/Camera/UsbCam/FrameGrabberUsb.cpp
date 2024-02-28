#include "pch.h"
#include "FrameGrabberUsb.h"

int CFrameGrabberUsb::IFGU2P_FrameGrabbed(int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (m_pIFGU2P == NULL) return 0;
	return m_pIFGU2P->IFGU2P_FrameGrabbedUsb(m_nIdCam, nFrameIndex, pBuffer, dwBufferSize);
}


CFrameGrabberUsb::CFrameGrabberUsb(int nIdCam, IFrameGrabberUsb2Parent* pIFGU2P)
{
	m_nIdCam = nIdCam;
	m_GrabberUsbStatus.Reset();
	m_pFrameCriticalSection = new CCriticalSection();
}

CFrameGrabberUsb::~CFrameGrabberUsb(void)
{
	if (m_pFrameCriticalSection)
	{
		delete m_pFrameCriticalSection;
	}
	m_pFrameCriticalSection = NULL;

	m_GrabberUsbStatus.Reset();
}
