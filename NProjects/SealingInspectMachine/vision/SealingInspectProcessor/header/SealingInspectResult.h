#pragma once
#include "SealingInspectResult_TopCam.h"
#include "SealingInspectResult_SideCam.h"

class AFX_EXT_CLASS CSealingInspectResult
{
public:
	CSealingInspectResult(void);
	~CSealingInspectResult(void);

public:
	CSealingInspectResult_TopCam m_sealingInspResult_TopCam;
	CSealingInspectResult_SideCam m_sealingInspResult_SideCam;

public:
	BOOL              m_bStatus;
	BOOL              m_bInspectComplete;
};