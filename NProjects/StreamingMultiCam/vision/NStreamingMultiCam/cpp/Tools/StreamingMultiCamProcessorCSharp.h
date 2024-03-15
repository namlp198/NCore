#pragma once

#include "StreamingMultiCamProcessor.h"

extern "C"
{
	__declspec(dllexport) CStreamingMultiCamProcessor*      CreateStreamingMultiCamProcessor();

	__declspec(dllexport) void                              DeleteStreamingMultiCamProcessor(CStreamingMultiCamProcessor* pProcessor);

	__declspec(dllexport) bool                              Initialize(CStreamingMultiCamProcessor* pProcessor);

	__declspec(dllexport) bool                              StartGrabHikCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                              StopGrabHikCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                             GetHikCamBufferImage(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                              StartGrabiRaypleCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                              StopGrabiRaypleCam(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                             GetiRaypleCamBufferImage(CStreamingMultiCamProcessor* pProcessor, int nCamIdx);
};
