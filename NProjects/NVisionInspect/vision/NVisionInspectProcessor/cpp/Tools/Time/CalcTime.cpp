#include "pch.h"
#include "CalcTime.h"

CalcTime::CalcTime()
{
	m_fTimeDuration = 0.0f;
}

CalcTime::~CalcTime()
{
}

void CalcTime::Start(void)
{
	m_startTime = high_resolution_clock::now();
}

void CalcTime::End(void)
{
	m_endTime = high_resolution_clock::now();

	auto duration = duration_cast<milliseconds>(m_endTime - m_startTime);
	m_fTimeDuration = duration.count();
	auto microDuration = duration_cast<microseconds>(m_endTime - m_startTime);
	m_fMicroTimeDuration = microDuration.count();

}
