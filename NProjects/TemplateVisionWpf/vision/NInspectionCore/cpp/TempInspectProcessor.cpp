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
	// Load Sys Config
	LoadSystemConfig();

	// Load Recipe
	LoadRecipe();

	if (m_pTempInspCore != NULL)
	{
		delete m_pTempInspCore, m_pTempInspCore = NULL;
	}
	m_pTempInspCore = new CTempInspectCore;

	// Test Run
	//m_pTempInspCore->StartInspect(0);

	if (m_pHikCamera != NULL)
	{
		m_pHikCamera->Destroy();
		delete m_pHikCamera, m_pHikCamera = NULL;
	}
	m_pHikCamera = new CTempInspectHikCam(this);
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

void CTempInspectProcessor::LoadSystemConfig()
{
	char buf[1024] = {};
	GetCurrentDirectoryA(1024, buf);
	std::string sysConfigPath = std::string(buf) + "\\config";
	m_pTempInspSysConfig = new CTempInspectSystemConfig((CString)sysConfigPath.c_str());
	m_pTempInspSysConfig->Initialize();
}

void CTempInspectProcessor::LoadRecipe()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pTempInspRecipe[i] = new CTempInspectRecipe;

		CString jobPath = m_pTempInspSysConfig->GetJobPath() + "\\" + m_pTempInspSysConfig->GetJobName() + ".xml";
		m_pTempInspRecipe[i]->SetJobPath(jobPath);
		m_pTempInspRecipe[i]->SetJobName(m_pTempInspSysConfig->GetJobName());

		m_pTempInspRecipe[i]->LoadRecipe(i);
	}
}

BOOL CTempInspectProcessor::TestRun()
{
	if (m_pTempInspCore == NULL)
		return FALSE;

	m_pTempInspCore->RunningThread(0);
	
	return TRUE;
}
