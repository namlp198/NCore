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

bool FindLineWithHoughLine_Simul(ImageProcessor* pProcessor, cv::Mat* mat, cv::Rect rectROI)
{
	if (pProcessor == NULL)
		return false;
	BOOL bRetValue = pProcessor->FindLineWithHoughLine_Simul(mat, rectROI);
	if (bRetValue == FALSE) return false;
	else                    return true;
}


