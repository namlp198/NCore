#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "interface_vision.h"

class AFX_EXT_CLASS CLocatorTool
{
public:
	CLocatorTool();
	~CLocatorTool();

public:
	// getter
	CLocatorToolResult     GetLocaToolRes() { return m_LocaResult; }
	CParameterLocator      GetParamLoca() { return m_ParamLoca; }

	// setter
	void                    SetLocaToolRes(CLocatorToolResult locaToolRes) { m_LocaResult = locaToolRes; }
	void                    SetParamLoca(CParameterLocator paramLoca) { m_ParamLoca = paramLoca; }
public:
	void                    Run();

protected:
	void                    NVision_FindLocator();

private:
	CParameterLocator               m_ParamLoca;
	CLocatorToolResult              m_LocaResult;
};