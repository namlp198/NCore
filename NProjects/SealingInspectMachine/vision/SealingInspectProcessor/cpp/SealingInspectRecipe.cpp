#include "pch.h"
#include "SealingInspectRecipe.h"

CSealingInspectRecipe::CSealingInspectRecipe(void)
{
	for (int i = 0; i < MAX_TOPCAM_COUNT; i++) {
		ZeroMemory(&m_sealingInspRecipe_TopCam[i], sizeof(CSealingInspectRecipe_TopCam));
	}
	for (int i = 0; i < MAX_SIDECAM_COUNT; i++) {
		ZeroMemory(&m_sealingInspRecipe_SideCam[i], sizeof(CSealingInspectRecipe_SideCam));
	}
}

CSealingInspectRecipe::~CSealingInspectRecipe(void)
{
}

CSealingInspectRecipe_TopCam::CSealingInspectRecipe_TopCam()
{
}

CSealingInspectRecipe_TopCam::~CSealingInspectRecipe_TopCam()
{
}

CSealingInspectRecipe_SideCam::CSealingInspectRecipe_SideCam()
{
}

CSealingInspectRecipe_SideCam::~CSealingInspectRecipe_SideCam()
{
}
