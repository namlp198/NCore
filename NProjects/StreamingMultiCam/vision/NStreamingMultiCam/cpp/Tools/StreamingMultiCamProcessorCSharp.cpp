#include "pch.h"
#include "StreamingMultiCamProcessorCSharp.h"

CStreamingMultiCamProcessor* CreateStreamingMultiCamProcessor()
{
	return new CStreamingMultiCamProcessor();
}

void DeleteStreamingMultiCamProcessor(CStreamingMultiCamProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CStreamingMultiCamProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->Initialize();
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool StartGrabHikCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabHikCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetHikCamBufferImage(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CInspectionHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return NULL;

	return pHikCam->GetBufferImage(nCamIdx);
}

bool StartGrabiRaypleCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectioniRaypleCam* piRaypleCam = pProcessor->GetiRaypleCamControl();
	if (piRaypleCam == NULL)
		return false;

	int retVal = piRaypleCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabiRaypleCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectioniRaypleCam* piRaypleCam = pProcessor->GetiRaypleCamControl();
	if (piRaypleCam == NULL)
		return false;

	int retVal = piRaypleCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetiRaypleCamBufferImage(CStreamingMultiCamProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CInspectioniRaypleCam* piRaypleCam = pProcessor->GetiRaypleCamControl();
	if (piRaypleCam == NULL)
		return NULL;

	return piRaypleCam->GetBufferImage(nCamIdx);
}
