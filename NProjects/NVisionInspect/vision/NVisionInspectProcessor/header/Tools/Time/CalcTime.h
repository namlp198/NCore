#pragma once
#include <chrono>
#include <iostream>
#include <string>

using namespace std::chrono;
using namespace std;

class CalcTime
{
public:
	CalcTime();
	virtual ~CalcTime();
public:
	void Start(void);
	void End(void);
	const float GetDurationSecond(void) const { return m_fTimeDuration / 1000.f; }
	const float GetDurationMilliSecond(void) const { return m_fTimeDuration; }
	const float GetDuarationMicroSecond(void) const { return m_fMicroTimeDuration; }
private:
	float m_fTimeDuration;
	float m_fMicroTimeDuration;
	steady_clock::time_point m_startTime, m_endTime;
};

