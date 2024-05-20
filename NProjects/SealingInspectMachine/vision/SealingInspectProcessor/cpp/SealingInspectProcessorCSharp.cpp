#include "pch.h"
#include "SealingInspectProcessorCSharp.h"

CSealingInspectProcessor* CreateSealingInspectProcessor()
{
	return new CSealingInspectProcessor();
}

void DeleteSealingInspectProcessor(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

#pragma region Offline simulation
BYTE* GetBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nY)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_SIDE(nBuff, nY);
}
BYTE* GetBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nY)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_TOP(nBuff, nY);
}

bool LoadImageBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_SIDE(nBuff, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}
bool LoadImageBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_TOP(nBuff, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool LoadAllImageBuffer(CSealingInspectProcessor* pProcessor, char* strDirPath, char* strImageType)
{
	if (pProcessor == NULL)
		return false;

	CString strDir = (CString)strDirPath;
	CString strImgType = (CString)strImageType;
	BOOL bRetValue = pProcessor->LoadAllImageBuffer(strDir, strImgType);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ClearBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage_SIDE(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}
bool ClearBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage_TOP(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ContinuousGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SingleGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetBufferImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return NULL;

	return pHikCam->GetBufferImage(nCamIdx);
}

bool SetTriggerModeHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nMode)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerMode(nCamIdx, nMode);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SetTriggerSourceHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nSource)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerSource(nCamIdx, nSource);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

#pragma endregion

void RegCallbackLogFunc(CSealingInspectProcessor* pProcessor, CallbackLogFunc* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLogFunc(pFunc);
}

void RegCallBackInspectCompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectComplete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInscompleteFunc(pFunc);
}