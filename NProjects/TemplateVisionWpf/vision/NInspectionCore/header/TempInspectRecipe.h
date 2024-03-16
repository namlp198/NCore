#pragma once
#include "LocatorTool.h"
#include "SelectROITool.h"

class AFX_EXT_CLASS CTempInspectRecipe
{
public:
	CTempInspectRecipe();
	~CTempInspectRecipe();

public:
	void LoadRecipe();

private:
	CLocatorTool*     m_LocTool;
	CSelectROITool*   m_SelROITool;
};