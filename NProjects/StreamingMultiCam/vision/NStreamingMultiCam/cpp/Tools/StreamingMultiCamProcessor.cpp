#include "pch.h"
#include "StreamingMultiCamProcessor.h"

CStreamingMultiCamProcessor::CStreamingMultiCamProcessor()
{
	for (int i = 0; i < MAX_BUFF_HIK; i++)
	{
		m_pImageBufferHik[i] = NULL;
	}

	for (int i = 0; i < MAX_BUFF_iRAYPLE; i++)
	{
		m_pImageBufferiRayple[i] = NULL;
	}
}

CStreamingMultiCamProcessor::~CStreamingMultiCamProcessor()
{
	Destroy();
}

BOOL CStreamingMultiCamProcessor::Initialize()
{
	// Inspection Hik Cam
	if (m_pInspHikCam != NULL)
	{
		m_pInspHikCam->Destroy();
		delete m_pInspHikCam, m_pInspHikCam = NULL;
	}
	m_pInspHikCam = new CInspectionHikCam;
	m_pInspHikCam->Initialize();

	// Inspection iRayple Cam
	if (m_pInspiRaypleCam != NULL)
	{
		m_pInspiRaypleCam->Destroy();
		delete m_pInspiRaypleCam, m_pInspiRaypleCam = NULL;
	}
	m_pInspiRaypleCam = new CInspectioniRaypleCam;
	m_pInspiRaypleCam->Initialize();

	return TRUE;
}

BOOL CStreamingMultiCamProcessor::Destroy()
{
	if (m_pInspHikCam != NULL)
	{
		m_pInspHikCam->Destroy();
		delete m_pInspHikCam;
		m_pInspHikCam = NULL;
	}

	if (m_pInspiRaypleCam != NULL)
	{
		m_pInspiRaypleCam->Destroy();
		delete m_pInspiRaypleCam;
		m_pInspiRaypleCam = NULL;
	}

	return TRUE;
}
