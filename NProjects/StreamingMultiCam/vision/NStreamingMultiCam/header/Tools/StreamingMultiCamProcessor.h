#pragma once
#include "InspectionHikCam.h"

#define MAX_BUFF_HIK 4

class AFX_EXT_CLASS CStreamingMultiCamProcessor
{
public:

	CStreamingMultiCamProcessor();
	~CStreamingMultiCamProcessor();

public:

	CInspectionHikCam*                GetHikCamControl() { return m_pInspHikCam; }

	BOOL							  Initialize();
	BOOL                              Destroy();

private:

	CInspectionHikCam*                m_pInspHikCam;

	// Image Buffer
	CSharedMemoryBuffer*              m_pImageBufferHik[MAX_BUFF_HIK];
};