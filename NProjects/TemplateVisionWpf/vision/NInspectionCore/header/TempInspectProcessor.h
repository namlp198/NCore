#pragma once
#include "TempInspectHikCam.h"
#include "TempInspectCore.h"
#include "TempInspectSystemConfig.h"

class AFX_EXT_CLASS CTempInspectProcessor : public ITempInspectHikCamToParent
{
public:
	CTempInspectProcessor();
	~CTempInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	void LoadSystemConfig();
	void LoadRecipe();

public:
	CTempInspectRecipe* GetRecipe(int nIdx) { return m_pTempInspRecipe[nIdx]; }
	CTempInspectSystemConfig* GetSystemConfig() { return m_pTempInspSysConfig; }

public:
	BOOL TestRun();

public:
	CTempInspectHikCam* GetHikCamControl() { return m_pHikCamera; }

private:

	// Hik cam
	CTempInspectHikCam* m_pHikCamera;

	// Inspect Core
	CTempInspectCore* m_pTempInspCore;

	// config
	CTempInspectSystemConfig* m_pTempInspSysConfig;

	// recipe
	CTempInspectRecipe* m_pTempInspRecipe[MAX_CAMERA_INSP_COUNT];
};