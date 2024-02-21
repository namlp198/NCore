#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <FindLineTool.h>
#include "SharedMemoryBuffer.h"
#include "InspectResult.h"
#include "InspectCore.h"
#include "InspectionHikCam.h"

#define MAX_BUFF 1
#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024
#define FRAME_COUNT 1

class AFX_EXT_CLASS ImageProcessor : public IInspectCoreToParent
{
public:
	ImageProcessor();
	~ImageProcessor();
	BOOL                              FindLineWithHoughLine_Simul(int top, int left, int width, int height, int nBuff);
	virtual LPBYTE                    GetBufferImage(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer(int nBuff, CString strFilePath);
	BOOL                              CreateBuffer();
	BOOL                              ClearBufferImage(int nBuff);
	BOOL							  Initialize();
	BOOL                              Destroy();

	BOOL                              GetInspectData(InspectResult* pInspectData);
	InspectionHikCam*                 GetHikCamControl() { return m_pInspHikCam; }

private:
	// Image Buffer
	CSharedMemoryBuffer*                     m_pImageBuffer[MAX_BUFF];
	std::vector<cv::Point2f>                 vPoints;

	InspectCore*                             m_pInspectCore;
	InspectionHikCam*                        m_pInspHikCam;
	
	//cv::Mat* pMat;
};