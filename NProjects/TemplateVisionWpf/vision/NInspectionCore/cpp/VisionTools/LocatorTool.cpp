#include "pch.h"
#include "LocatorTool.h"

CLocatorTool::CLocatorTool()
{
	m_pImageBuffer = NULL;
}

CLocatorTool::~CLocatorTool()
{
}

LPBYTE CLocatorTool::GetImageBuffer()
{
	if (m_pImageBuffer == NULL)
		return nullptr;

	return m_pImageBuffer->GetBufferImage(0);
}

LPBYTE CLocatorTool::GetTemplateImageBuffer()
{
	if (m_imageTemplate.empty())
		return nullptr;

	return (LPBYTE)m_imageTemplate.data;
}

LPBYTE CLocatorTool::GetResultImageBuffer()
{
	if (m_resultImageBuffer.empty())
		return nullptr;

	return (LPBYTE)m_resultImageBuffer.data;
}

BOOL CLocatorTool::GetDataTrained_TemplateMatching(CLocatorToolResult* pDataTrained)
{
	pDataTrained->m_nX = m_locResult.m_nX;
	pDataTrained->m_nY = m_locResult.m_nY;
	pDataTrained->m_bResult = m_locResult.m_bResult;
	pDataTrained->m_nDelta_x = m_locResult.m_nDelta_x;
	pDataTrained->m_nDelta_y = m_locResult.m_nDelta_y;
	pDataTrained->m_dDif_Angle = m_locResult.m_dDif_Angle;

	return TRUE;
}

BOOL CLocatorTool::SetImageBuffer(LPBYTE pBuff)
{
	if (pBuff == NULL)
		return FALSE;

	m_pImageBuffer->SetFrameImage(0, pBuff);
}

BOOL CLocatorTool::Run()
{
	return NVision_FindLocator_TemplateMatching();
}

BOOL CLocatorTool::Initialize(CameraInfo pCamInfo)
{
	int width = pCamInfo.m_nFrameWidth;
	int height = pCamInfo.m_nFrameHeight;
	
	USES_CONVERSION;
	char cSensorType[10] = {};
	sprintf_s(cSensorType, "%s", W2A(pCamInfo.m_csSensorType));


	int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
	int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

	// Buffer
	if (m_pImageBuffer != NULL)
	{
		m_pImageBuffer->DeleteSharedMemory();
		delete m_pImageBuffer;
		m_pImageBuffer = NULL;
	}

	DWORD dwFrameWidth = (DWORD)pCamInfo.m_nFrameWidth;
	DWORD dwFrameHeight = (DWORD)pCamInfo.m_nFrameHeight;
	DWORD dwFrameCount = MAX_FRAME_COUNT;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nFrameChannels;

	m_pImageBuffer = new CSharedMemoryBuffer;
	m_pImageBuffer->SetFrameWidth(dwFrameWidth);
	m_pImageBuffer->SetFrameHeight(dwFrameHeight);
	m_pImageBuffer->SetFrameCount(dwFrameCount);
	m_pImageBuffer->SetFrameSize(dwFrameSize);

	DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

	CString strMemory;
	strMemory.Format(_T("%s_%d"), "Buffer_LocTool", 0);
	m_pImageBuffer->CreateSharedMemory(strMemory, dw64Size);

	return TRUE;
}

BOOL CLocatorTool::SaveImageTemplate(cv::Mat* pSaveImage, CString strFileTitle)
{
	if (pSaveImage == NULL)
		return FALSE;



	return TRUE;
}

BOOL CLocatorTool::NVision_FindLocator_TemplateMatching()
{
	// implement algorithm at here

	// when the algorithm handle is done then assign the result received for the CLocatorToolResult object

	return TRUE;
}

BOOL CLocatorTool::NVision_FindLocator_TemplateMatching_TRAIN(int nCamIdx, CRectForTrainLocTool* paramTrainLoc)
{
	CString csData;
	csData.Format(_T("rectInside_X:%d, rectInside_Y:%d, rectInsie_Width:%d, rectInsie_Height:%d", 
		paramTrainLoc->m_nRectIn_X, paramTrainLoc->m_nRectIn_Y, paramTrainLoc->m_nRectIn_Width, paramTrainLoc->m_nRectIn_Height));

	AfxMessageBox(csData);

	return TRUE;
}
