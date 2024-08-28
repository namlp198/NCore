#include "pch.h"
#include "ReadCodeStatus.h"

CReadCodeStatus::CReadCodeStatus(void)
{
	m_bSimulation = FALSE;
	m_bStreaming = FALSE;
	m_bInspectRunning = FALSE;
}

CReadCodeStatus::~CReadCodeStatus(void)
{
}
