#pragma once
#include "ImageProcessor.h"
#include "InspectResult.h"

extern "C"
{
	__declspec(dllexport) ImageProcessor*    CreateImageProcessor();

	__declspec(dllexport) void               DeleteImageProcessor(ImageProcessor* pProcessor);

	__declspec(dllexport) BYTE*              GetBufferImage(ImageProcessor* pProcessor, int nBuff, int nY);

	__declspec(dllexport) bool               LoadImageBuffer(ImageProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool               ClearBufferImage(ImageProcessor* pProcessor, int nBuff);

	__declspec(dllexport) bool               Initialize(ImageProcessor* pProcessor);

	__declspec(dllexport) bool               FindLineWithHoughLine_Simul(ImageProcessor* pProcessor, int top, int left, int width, int height, int nBuff);

	__declspec(dllexport) bool               GetInspectData(ImageProcessor* pProcessor, InspectResult* pInspectData);
};