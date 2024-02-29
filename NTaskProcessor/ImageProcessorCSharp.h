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

	__declspec(dllexport) BYTE*              GetHikCamBufferImage(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*              GetBaslerCamBufferImage(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*              GetBaslerCamBufferImage_New(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*              GetUsbCamBufferImage(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               LiveBaslerCam(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               StartGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               StopGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               SingleGrabBaslerCam_New(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               StartGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               StopGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool               SingleGrabUsbCam(ImageProcessor* pProcessor, int nCamIdx);

	// Show log view
	__declspec(dllexport) void				 ShowLogView(ImageProcessor* pProcessor, int bShow);
};