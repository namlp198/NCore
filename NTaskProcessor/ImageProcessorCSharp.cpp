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

BYTE* GetBaslerCamBufferImage(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CInspectionBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return NULL;
	return pBaslerCam->GetBufferImage(nCamIdx);
}

bool LiveBaslerCam(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	BOOL bRetValue = pBaslerCam->LiveCamera(nCamIdx);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool StartGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionBaslerCam_New* pBaslerCam_New = pProcessor->GetBaslerCamControl_New();
	if (pBaslerCam_New == NULL)
		return false;

	int retVal = pBaslerCam_New->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionBaslerCam_New* pBaslerCam_New = pProcessor->GetBaslerCamControl_New();
	if (pBaslerCam_New == NULL)
		return false;

	int retVal = pBaslerCam_New->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionBaslerCam_New* pBaslerCam_New = pProcessor->GetBaslerCamControl_New();
	if (pBaslerCam_New == NULL)
		return false;

	int retVal = pBaslerCam_New->SingleGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StartGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionUsbCam* pUsbCam = pProcessor->GetUsbCamControl();
	if (pUsbCam == NULL)
		return false;

	int retVal = pUsbCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionUsbCam* pUsbCam = pProcessor->GetUsbCamControl();
	if (pUsbCam == NULL)
		return false;

	int retVal = pUsbCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CInspectionUsbCam* pUsbCam = pProcessor->GetUsbCamControl();
	if (pUsbCam == NULL)
		return false;

	int retVal = pUsbCam->SingleGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

void ShowLogView(ImageProcessor* pProcessor, int bShow)
{
	if (pProcessor == NULL)
		return;

	BOOL bShowWnd = (bShow == 0) ? FALSE : TRUE;

	pProcessor->ShowLogView(bShowWnd);
}

BYTE* GetBaslerCamBufferImage_New(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CInspectionBaslerCam_New* pBaslerCam_New = pProcessor->GetBaslerCamControl_New();
	if (pBaslerCam_New == NULL)
		return NULL;
	return pBaslerCam_New->GetBufferImage(nCamIdx);
}

BYTE* GetUsbCamBufferImage(ImageProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	CInspectionUsbCam* pUsbCam = pProcessor->GetUsbCamControl();
	if (pUsbCam == NULL)
		return NULL;

	return pUsbCam->GetBufferImage(nCamIdx);
}


