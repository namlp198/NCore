#include "pch.h"
#include "ImageProcessorCSharp.h"

ImageProcessor* CreateImageProcessor()
{
	return new ImageProcessor();
}

void DeleteImageProcessor(ImageProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

BYTE* GetBufferImage(ImageProcessor* pProcessor, int nBuff, int nY)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage(nBuff, nY);
}

bool LoadImageBuffer(ImageProcessor* pProcessor, int nBuff, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer(nBuff, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ClearBufferImage(ImageProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool Initialize(ImageProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->Initialize();
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool FindLineWithHoughLine_Simul(ImageProcessor* pProcessor, int top, int left, int width, int height, int nBuff)
{
	if (pProcessor == NULL)
		return false;
	BOOL bRetValue = pProcessor->FindLineWithHoughLine_Simul(top, left, width, height, nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool GetInspectData(ImageProcessor* pProcessor, InspectResult* pInspectData)
{
	if (pProcessor == NULL)
		return FALSE;
	if (pInspectData == NULL)
		return FALSE;

	ZeroMemory(pInspectData, sizeof(InspectResult));
	InspectResult inspectData;

	BOOL bRetValue = pProcessor->GetInspectData(&inspectData);
	CopyMemory(pInspectData, &inspectData, sizeof(InspectResult));
	if (bRetValue == FALSE) return false;
	else                    return true;
}

BYTE* GetHikCamBufferImage(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	InspectionHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return NULL;

	return pHikCam->GetBufferImage(nCamIdx);
}


