#include "pch.h"
#include "VisionAlgorithms.h"

CVisionAlgorithms::CVisionAlgorithms()
{
	m_pImageBuffer = NULL;
}

CVisionAlgorithms::~CVisionAlgorithms()
{
}

BOOL CVisionAlgorithms::Run(emAlgorithms algorithm)
{
	switch (algorithm)
	{
	case emCountPixel:
		return NVision_CountPixelAlgorithm();
		break;
	case emCalculateArea:
		return NVision_CalculateAreaAlgorithm();
		break;
	case emCalculateCoordinate:
		break;
	case emCountBlob:
		break;
	case emFindLine:
		break;
	case emFindCircle:
		break;
	case emOCR:
		break;
	default:
		break;
	}
}

LPBYTE CVisionAlgorithms::GetImageBuffer()
{
	if (m_pImageBuffer == NULL)
		return nullptr;

	return m_pImageBuffer->GetBufferImage(0);
}

LPBYTE CVisionAlgorithms::GetResultImageBuffer()
{
	if (m_resultImageBuffer.empty())
		return NULL;

	return (LPBYTE)m_resultImageBuffer.data;
}

LPBYTE CVisionAlgorithms::GetResultROIBuffer_Train()
{
	if (m_resultROIBuffer.empty())
		return nullptr;

	return (LPBYTE)m_resultROIBuffer.data;
}

BOOL CVisionAlgorithms::GetResultCntPxl_Train(CAlgorithmsCountPixelResult* pCntPxlTrainRes)
{
	pCntPxlTrainRes->m_nNumberOfPixel = m_cntPxlRes.m_nNumberOfPixel;
	pCntPxlTrainRes->m_bResult = m_cntPxlRes.m_bResult;

	return TRUE;
}

BOOL CVisionAlgorithms::GetResultCalArea_Train(CAlgorithmsCalculateAreaResult* pCalAreaTrainRes)
{
	pCalAreaTrainRes->m_dArea = m_calAreaRes.m_dArea;
	pCalAreaTrainRes->m_bResult = m_calAreaRes.m_bResult;

	return TRUE;
}

BOOL CVisionAlgorithms::SetImageBuffer(LPBYTE pImgBuff)
{
	if (pImgBuff == NULL)
		return FALSE;

	return m_pImageBuffer->SetFrameImage(0, pImgBuff);
}


BOOL CVisionAlgorithms::NVision_CountPixelAlgorithm()
{
	
	//char cText[1024] = {};
	//sprintf_s(cText, "This is Count Pixel tool, %s: %s", "angle rotation: ", std::to_string(std::get<4>(param.m_tupROI)));
	//// code here
	//AfxMessageBox((CString)cText);

	// when the algorithm handle is done then assign the result received for the CAlgorithmsCountPixelResult object

	return TRUE;
}

BOOL CVisionAlgorithms::NVision_CalculateAreaAlgorithm()
{
	
	//char cText[1024] = {};
	//sprintf_s(cText, "This is Cal Area tool, %s: %s", "area: ", std::to_string(param.m_arrArea[1]));
	//// code here
	//AfxMessageBox((CString)cText);

	// when the algorithm handle is done then assign the result received for the CAlgorithmsCountPixelResult object

	return TRUE;
}

BOOL CVisionAlgorithms::NVision_CountPixelAlgorithm_TRAIN(CParamCntPxlAlgorithm* pParamTrainCntPxl)
{
	CString csData;
	csData.Format(_T("ROI_X:%d, ROI_Y:%d, ROI_Width:%d, ROI_Height:%d, ROI_Angle:%.3f"),
		pParamTrainCntPxl->m_nROIX, pParamTrainCntPxl->m_nROIY, pParamTrainCntPxl->m_nROIWidth, pParamTrainCntPxl->m_nROIHeight, pParamTrainCntPxl->m_dROIAngleRotate);

	AfxMessageBox(csData);

	return TRUE;
}

BOOL CVisionAlgorithms::NVision_CalculateAreaAlgorithm_TRAIN(CParamCalAreaAlgorithm* pParamTrainCalArea)
{
	CString csData;
	csData.Format(_T("ROI_X:%d, ROI_Y:%d, ROI_Width:%d, ROI_Height:%d, ROI_Angle:%.3f"),
		pParamTrainCalArea->m_nROIX, pParamTrainCalArea->m_nROIY, pParamTrainCalArea->m_nROIWidth, pParamTrainCalArea->m_nROIHeight, pParamTrainCalArea->m_dROIAngleRotate);

	AfxMessageBox(csData);

	return TRUE;
}
