#pragma once

#include "VisionManager.h"

class AFX_EXT_CLASS CVisionAlgorithms
{
public:
	CVisionAlgorithms();
	~CVisionAlgorithms();

public:
	void Run();

public:
	// Getter
	CLocatorToolResult               GetLocaResult() { return m_locaResult; }
	CVisionParameterManager*         GetVsParamManager() { return m_vsParamManeger; }
	CVisionResultManager*            GetVsResultManager() { return m_vsResultManager; }
	emAlgorithms                     GetAlgorithm() { return m_emAlgorithm; }
	LPBYTE                           GetImageBuffer() { return m_pImageBuffer; }

	// Setter
	void              SetLocaResult(CLocatorToolResult locaRes) { m_locaResult = locaRes; }
	void              SetVsParamManager(CVisionParameterManager* pVsParam) { m_vsParamManeger = pVsParam; }
	void              SetVsResultManager(CVisionResultManager* pVsResult) { m_vsResultManager = pVsResult; }
	void              SetAlgorithm(emAlgorithms algorithm) { m_emAlgorithm = algorithm; }
	void              SetImageBuffer(LPBYTE pImgBuff) { m_pImageBuffer = pImgBuff; }

private:
	// all algorithms will declare at here
	void             NVision_CountPixelAlgorithm();
	void             NVision_CalculateAreaAlgorithm();

private:

	// define algorithm gonna run when inspection start.
	emAlgorithms                         m_emAlgorithm;

	// manager parameter and result of vision
	CVisionParameterManager*             m_vsParamManeger;
	CVisionResultManager*                m_vsResultManager;

	// result of locator tool when run done.
	CLocatorToolResult                   m_locaResult;

	LPBYTE                               m_pImageBuffer;
};