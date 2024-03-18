#pragma once
#include "TempInspectHikCam.h"
#include "TempInspectCore.h"

class AFX_EXT_CLASS CTempInspectProcessor
{
public:
	CTempInspectProcessor();
	~CTempInspectProcessor();

private:

	CTempInspectHikCam* m_HikCamera;
};