#pragma once
#include "TempInspectHikCam.h"
#include "TempInspectCore.h"

class AFX_EXT_CLASS CTempInspectProcessor
{
public:
	CTempInspectProcessor();
	~CTempInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();

public:
	BOOL TestRun();

public:
	CTempInspectHikCam* GetHikCamControl() { return m_pHikCamera; }

private:

	// Hik cam
	CTempInspectHikCam* m_pHikCamera;

	// Inspect Core
	CTempInspectCore* m_pTempInspCore;
};