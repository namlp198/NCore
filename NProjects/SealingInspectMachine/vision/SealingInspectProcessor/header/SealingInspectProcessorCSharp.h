#pragma once

#include "SealingInspectProcessor.h"

extern "C"
{
	__declspec(dllexport) CSealingInspectProcessor* CreateSealingInspectProcessor();

	__declspec(dllexport) void                      DeleteSealingInspectProcessor(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      Initialize(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) BYTE*                     GetBufferImage_Color(CSealingInspectProcessor* pProcessor, int nBuff, int nY);

	__declspec(dllexport) BYTE*                     GetBufferImage_Mono(CSealingInspectProcessor* pProcessor, int nBuff, int nY);
											        
	__declspec(dllexport) bool                      LoadImageBuffer_Color(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool                      LoadImageBuffer_Mono(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);
											        
	__declspec(dllexport) bool                      ClearBufferImage(CSealingInspectProcessor* pProcessor, int nBuff);
};