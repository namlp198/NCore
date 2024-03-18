#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "interface_vision.h"

class AFX_EXT_CLASS CLocatorTool : public ITools
{
public:
	CLocatorTool();
	~CLocatorTool();

public:
	// getter
	CLocatorToolResult*     GetLocaToolRes() { return m_pLocaResult; }
	CParameterLocator*      GetParamLoca() { return m_pParamLoca; }

	// setter
	void                    SetLocaToolRes(CLocatorToolResult* locaToolRes) { m_pLocaResult = locaToolRes; }
	void                    SetParamLoca(CParameterLocator* paramLoca) { m_pParamLoca = paramLoca; }

protected:
	void                    NVision_FindLocator();

private:
	CParameterLocator*               m_pParamLoca;
	CLocatorToolResult*              m_pLocaResult;
};