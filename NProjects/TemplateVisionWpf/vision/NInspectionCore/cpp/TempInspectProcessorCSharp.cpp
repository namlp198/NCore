#include "pch.h"
#include "TempInspectProcessorCSharp.h"

CTempInspectProcessor* CreateTempInspectProcessor()
{
    return new CTempInspectProcessor();
}

void DeleteTempInspectProcessor(CTempInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CTempInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStart(CTempInspectProcessor* pProcessor, int nThreadCount, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool ContinuousGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SingleGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetBufferImageHikCam(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return NULL;

	return pHikCam->GetBufferImage(nCamIdx);
}

bool SetTriggerModeHikCam(CTempInspectProcessor* pProcessor, int nCamIdx, int nMode)
{
	if (pProcessor == NULL)
		return false;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerMode(nCamIdx, nMode);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SetTriggerSourceHikCam(CTempInspectProcessor* pProcessor, int nCamIdx, int nSource)
{
	if (pProcessor == NULL)
		return false;

	CTempInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerSource(nCamIdx, nSource);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool TrainLocator_TemplateMatching(CTempInspectProcessor* pProcessor, int nCamIdx, CRectForTrainLocTool* rectForTrainLoc)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->TrainLocator_TemplateMatching(nCamIdx, rectForTrainLoc);
	if (bRet == FALSE) return false;
	else               return true;
}

BYTE* GetTemplateImage(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetTemplateImage(nCamIdx);
}

bool GetDataTrained_TemplateMatching(CTempInspectProcessor* pProcessor, int nCamIdx, CLocatorToolResult* dataTrained)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetDataTrained_TemplateMatching(nCamIdx, dataTrained);
	if (bRet == FALSE) return false;
	else               return true;
}

bool CountPixelAlgorithm_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CParamCntPxlAlgorithm* pParamCntPxlTrain)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->CountPixelAlgorithm_Train(nCamIdx, pParamCntPxlTrain);
	if (bRet == FALSE) return false;
	else               return true;
}

bool CalculateAreaAlgorithm_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CParamCalAreaAlgorithm* pParamTrainCalArea)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->CalculateAreaAlgorithm_Train(nCamIdx, pParamTrainCalArea);
	if (bRet == FALSE) return false;
	else               return true;
}

BYTE* GetResultROIBuffer_Train(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultROIBuffer_Train(nCamIdx);
}

BYTE* GetResultImageBuffer(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultImageBuffer(nCamIdx);
}

bool GetResultCntPxl_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CAlgorithmsCountPixelResult* pCntPxlTrainRes)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetResultCntPxl_Train(nCamIdx, pCntPxlTrainRes);
	if (bRet == FALSE) return false;
	else               return true;
}

bool GetResultCalArea_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CAlgorithmsCalculateAreaResult* pCalAreaTrainRes)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetResultCalArea_Train(nCamIdx, pCalAreaTrainRes);
	if (bRet == FALSE) return false;
	else               return true;
}

bool GetSumResult(CTempInspectProcessor* pProcessor, int nCamIdx, CSumResult* pSumRes)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetSumResult(nCamIdx, pSumRes);
	if (bRet == FALSE) return false;
	else               return true;
}


