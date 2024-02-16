#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <FindLineTool.h>
#include "SharedMemoryBuffer.h"

#define MAX_BUFF 1
#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024
#define FRAME_COUNT 1

class AFX_EXT_CLASS ImageProcessor
{
public:
	ImageProcessor();
	~ImageProcessor();
	void                              FindLineWithHoughLine_Offline(std::string pathImg, cv::Rect rectROI);
	virtual LPBYTE                    GetBufferImage(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer(int nBuff, CString strFilePath);
	BOOL                              CreateBuffer();
	BOOL                              ClearBufferImage(int nBuff);
	BOOL							  Initialize();

private:
	// Image Buffer
	CSharedMemoryBuffer*                     m_pImageBuffer[MAX_BUFF];
};