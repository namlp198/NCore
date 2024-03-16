#pragma once

#include "TempInspectRecipe.h"
#include "LocatorTool.h"
#include "SelectROITool.h"
#include "TempInspectDefine.h"

class AFX_EXT_CLASS CTempInspectCore
{
public:
	CTempInspectCore();
	~CTempInspectCore();

public:
	void LoadRecipe();

private:
	CTempInspectRecipe*                   m_TempInspRecipe[MAX_CAMERA_INSP_COUNT];
};