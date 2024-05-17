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

BYTE* GetBufferImage_Color(CSealingInspectProcessor* pProcessor, int nBuff, int nY)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_Color(nBuff, nY);
}

BYTE* GetBufferImage_Mono(CSealingInspectProcessor* pProcessor, int nBuff, int nY)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_Mono(nBuff, nY);
}

bool LoadImageBuffer_Color(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_Color(nBuff, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool LoadImageBuffer_Mono(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_Mono(nBuff, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ClearBufferImage(CSealingInspectProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}
