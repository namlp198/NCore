#include "pch.h"
#include "TimeCalculator.h"

CTimeCalculator::CTimeCalculator()
{
	m_swFreq.LowPart = m_swFreq.HighPart = 0;
	m_swStart = m_swFreq;
	m_swEnd = m_swFreq;
	m_fTimeforDuration = 0;

	QueryPerformanceFrequency(&m_swFreq);
	//	m_swFreq.QuadPart = (LONGLONG)(2.41f*1024*1024*1024);
}

CTimeCalculator::~CTimeCalculator()
{
}

void CTimeCalculator::Start(void)
{
	QueryPerformanceCounter(&m_swStart);
}

void CTimeCalculator::End(void)
{
	QueryPerformanceCounter(&m_swEnd);
	m_fTimeforDuration = (m_swEnd.QuadPart - m_swStart.QuadPart) / (float)m_swFreq.QuadPart;
}