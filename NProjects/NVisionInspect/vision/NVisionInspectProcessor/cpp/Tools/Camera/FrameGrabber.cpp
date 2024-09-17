#include "pch.h"
#include "FrameGrabber.h"


CFrameGrabber* FrameGrabber_New(int nInterfaceType, int nBayerType, int nCamIndex, PVOID pPtr)
{
	return NULL;
}

CFrameGrabber::CFrameGrabber(int nIndex, IFrameGrabber2Parent* pFG2P) : m_pIFG2P(pFG2P)
{
	m_nIndex = nIndex;
	m_GrabberStatus.Reset();
	m_pFrameCriticalSection = new CCriticalSection();
}

CFrameGrabber::~CFrameGrabber(void)
{
	if (m_pFrameCriticalSection)
	{
		delete m_pFrameCriticalSection;
	}
	m_pFrameCriticalSection = NULL;

	m_GrabberStatus.Reset();
}

int CFrameGrabber::SendTrigger( int nTriggerCount/*=1*/ )
{
	return 1;
}

int CFrameGrabber::IFG2P_FrameGrabbed(int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (m_pIFG2P == NULL) return 0;
	return m_pIFG2P->IFG2P_FrameGrabbed(m_nIndex, nFrameIndex, pBuffer, dwBufferSize);
}

int CFrameGrabber::IFG2P_GetFrameBuffer(int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (m_pIFG2P == NULL) return 0;
	return m_pIFG2P->IFG2P_GetFrameBuffer(m_nIndex, nFrameIndex, pBuffer, dwBufferSize);
}

int CFrameGrabber::IFG2P_SendLogMessage(const TCHAR* lpstrFormat, ...)
{
	if (m_pIFG2P == NULL) return 0;
	
	va_list list;
	TCHAR strText[2000] = { 0 };

	va_start(list, lpstrFormat);
	_vstprintf_s(strText, lpstrFormat, list);
	va_end(list);

	m_pIFG2P->DisplayMessage(strText);

	return 1;
}

