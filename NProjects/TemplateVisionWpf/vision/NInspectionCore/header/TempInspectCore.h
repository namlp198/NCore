#pragma once

#include "TempInspectRecipe.h"
#include "TempInspectDefine.h"

class AFX_EXT_CLASS CTempInspectCore
{
public:
	CTempInspectCore();
	~CTempInspectCore();

public:
	void LoadRecipe(int nCamIdx);

public:
	void Running(int nCamIdx);

private:
	CTempInspectRecipe*                   m_TempInspRecipe[MAX_CAMERA_INSP_COUNT];
};