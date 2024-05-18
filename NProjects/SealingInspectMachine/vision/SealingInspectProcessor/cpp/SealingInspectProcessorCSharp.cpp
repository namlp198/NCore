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
#pragma endregion
