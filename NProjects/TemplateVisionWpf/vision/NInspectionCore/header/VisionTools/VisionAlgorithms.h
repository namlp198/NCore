#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"

class AFX_EXT_CLASS CVisionAlgorithms
{
public:
	CVisionAlgorithms(emAlgorithms algorithm, IParameters* pParam, IResults* pResult);
	~CVisionAlgorithms();

public:
	void Run();

public:
	// Getter
	IResults*                        GetResult() { return m_pResult; }
	IParameters*                     GetParam() { return m_pParam; }
	CLocatorToolResult*              GetLocaResult() { return m_pLocaResult; }

	// Setter
	void                             SetLocaResult(CLocatorToolResult* locaRes) { m_pLocaResult = locaRes; }
	void                             SetParam(IParameters* pParam) { m_pParam = pParam; }

private:
	// Src all algorithms
	void                             NVision_CountPixelAlgorithm();

private:

	emAlgorithms                        m_emAlgorithm;
	IParameters*                        m_pParam;
	IResults*                           m_pResult;

	CLocatorToolResult*                 m_pLocaResult;
};