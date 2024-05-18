#pragma once

#include "SealingInspectProcessor.h"

extern "C"
{
	__declspec(dllexport) CSealingInspectProcessor* CreateSealingInspectProcessor();

	__declspec(dllexport) void                      DeleteSealingInspectProcessor(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      Initialize(CSealingInspectProcessor* pProcessor);

#pragma region Offline simulation
	__declspec(dllexport) BYTE*                     GetBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nY);
								                    
	__declspec(dllexport) BYTE*                     GetBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nY);
											        
	__declspec(dllexport) bool                      LoadImageBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool                      LoadImageBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool                      LoadAllImageBuffer(CSealingInspectProcessor* pProcessor, char* strDirPath, char* strImageType);
											        
	__declspec(dllexport) bool                      ClearBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff);

	__declspec(dllexport) bool                      ClearBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff);
#pragma endregion
};