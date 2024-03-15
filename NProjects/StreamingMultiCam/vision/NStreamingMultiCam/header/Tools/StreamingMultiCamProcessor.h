#pragma once
#include "InspectionHikCam.h"
#include "InspectioniRaypleCam.h"

#define MAX_BUFF_HIK 4
#define MAX_BUFF_iRAYPLE 8

class AFX_EXT_CLASS CStreamingMultiCamProcessor
{
public:

	CStreamingMultiCamProcessor();
	~CStreamingMultiCamProcessor();

public:

	CInspectionHikCam*                GetHikCamControl() { return m_pInspHikCam; }
	CInspectioniRaypleCam*            GetiRaypleCamControl() { return m_pInspiRaypleCam; }

	BOOL							  Initialize();
	BOOL                              Destroy();

private:

	CInspectionHikCam*                m_pInspHikCam;

	CInspectioniRaypleCam*            m_pInspiRaypleCam;

	// Image Buffer
	CSharedMemoryBuffer*              m_pImageBufferHik[MAX_BUFF_HIK];

	CSharedMemoryBuffer*              m_pImageBufferiRayple[MAX_BUFF_iRAYPLE];
};