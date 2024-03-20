#include "pch.h"
#include "TempInspectProcessor.h"

CTempInspectProcessor::CTempInspectProcessor()
{
}

CTempInspectProcessor::~CTempInspectProcessor()
{
}

BOOL CTempInspectProcessor::Initialize()
{
	if (m_pTempInspCore != NULL)
	{
		delete m_pTempInspCore, m_pTempInspCore = NULL;
	}
	m_pTempInspCore = new CTempInspectCore;
	m_pTempInspCore->LoadRecipe();

	// Test Run
	//m_pTempInspCore->StartInspect(0);

	if (m_pHikCamera != NULL)
	{
		m_pHikCamera->Destroy();
		delete m_pHikCamera, m_pHikCamera = NULL;
	}
	m_pHikCamera = new CTempInspectHikCam;
	m_pHikCamera->SetTempInspCore(m_pTempInspCore);
	m_pHikCamera->Initialize();

	return TRUE;
}

BOOL CTempInspectProcessor::Destroy()
{
	if (m_pHikCamera != NULL)
	{
		m_pHikCamera->Destroy();
		delete m_pHikCamera, m_pHikCamera = NULL;
	}

	if (m_pTempInspCore != NULL)
	{
		delete m_pTempInspCore, m_pTempInspCore = NULL;
	}

	return TRUE;
}

BOOL CTempInspectProcessor::TestRun()
{
	if (m_pTempInspCore == NULL)
		return FALSE;

	m_pTempInspCore->RunningThread(0);
	
	return TRUE;
}
