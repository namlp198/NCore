#pragma once

#include <afxwin.h>

class AFX_EXT_CLASS CTimeCalculator
{
public:
	CTimeCalculator();
	virtual ~CTimeCalculator();

public:
	void Start(void);
	void End(void);
	const float GetDurationSecond(void) const { return m_fTimeforDuration; }
	const float GetDurationMilliSecond(void) const { return m_fTimeforDuration * 1000.f; }

public:
	LARGE_INTEGER GetStartInteger(void) const { return m_swStart; }
	LARGE_INTEGER GetEndInteger(void) const { return m_swEnd; }
	LARGE_INTEGER GetFrequency(void) const { return m_swFreq; }

protected:
	LARGE_INTEGER		m_swFreq, m_swStart, m_swEnd;
	float				m_fTimeforDuration;
};

