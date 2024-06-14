#include "pch.h"
#include "SealingInspectResult_TopCam.h"

CSealingInspectResult_TopCam::CSealingInspectResult_TopCam(void)
{
	for (int i = 0; i < MAX_MEASURE_DIST_HOUGHCIRCLE_TOPCAM; i++)
	{
		m_nArrPosNG_TopCam[i] = 0;
	}
}

CSealingInspectResult_TopCam::~CSealingInspectResult_TopCam(void)
{
}
