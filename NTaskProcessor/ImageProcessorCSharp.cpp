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
		return;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer(nBuff, strPath);

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


