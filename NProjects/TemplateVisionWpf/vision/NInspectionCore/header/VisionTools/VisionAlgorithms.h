#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "VisionDefine.h"
#include "SharedMemoryBuffer.h"

class AFX_EXT_CLASS CVisionAlgorithms
{
public:
	CVisionAlgorithms();
	~CVisionAlgorithms();

public:
	BOOL Run(emAlgorithms algorithm);

public:
	// Getter
	CParamCntPxlAlgorithm            GetParamCntPxlAlgorithm() { return m_paramCntPxl; }
	CParamCalAreaAlgorithm           GetParamCalAreaAlgorithm() { return m_paramCalArea; }
	CAlgorithmsCountPixelResult      GetCntPxlRes() { return m_cntPxlRes; }
	CAlgorithmsCalculateAreaResult   GetCalAreaRes() { return m_calAreaRes; }
	emAlgorithms                     GetAlgorithm() { return m_emAlgorithm; }
	LPBYTE                           GetImageBuffer();
	LPBYTE                           GetResultImageBuffer();

	// Get data after that trained
	LPBYTE                           GetResultROIBuffer_Train();
	BOOL                             GetResultCntPxl_Train(CAlgorithmsCountPixelResult* pCntPxlTrainRes);
	BOOL                             GetResultCalArea_Train(CAlgorithmsCalculateAreaResult* pCalAreaTrainRes);

	// Setter
	void              SetParamCntPxlAlgorithm(CParamCntPxlAlgorithm paramCntPxl) { m_paramCntPxl = paramCntPxl; }
	void              SetParamCalAreaAlgorithm(CParamCalAreaAlgorithm paramCalArea) { m_paramCalArea = paramCalArea; }
	void              SetLocResult(CLocatorToolResult locaRes) { m_locResult = locaRes; }
	void              SetAlgorithm(emAlgorithms algorithm) { m_emAlgorithm = algorithm; }
	BOOL              SetImageBuffer(LPBYTE pImgBuff);
	void              SetCntPxlRes(CAlgorithmsCountPixelResult cntPxlRes) { m_cntPxlRes = cntPxlRes; }
	void              SetCalAreaRes(CAlgorithmsCalculateAreaResult calAreaRes) { m_calAreaRes = calAreaRes; }

public:
	// all func will use to when runtime
	BOOL             NVision_CountPixelAlgorithm();
	BOOL             NVision_CalculateAreaAlgorithm();

	// all func for train data in the step create recipe to camera
	BOOL             NVision_CountPixelAlgorithm_TRAIN(CParamCntPxlAlgorithm* pParamTrainCntPxl);
	BOOL             NVision_CalculateAreaAlgorithm_TRAIN(CParamCalAreaAlgorithm* pParamTrainCalArea);

private:

	// define algorithm gonna run when inspection start.
	emAlgorithms                         m_emAlgorithm;

	
	// parameter
	CParamCntPxlAlgorithm                m_paramCntPxl;
	CParamCalAreaAlgorithm               m_paramCalArea;
	
	// Result
	CAlgorithmsCountPixelResult          m_cntPxlRes;
	CAlgorithmsCalculateAreaResult       m_calAreaRes;

	// result of locator tool when run done.
	CLocatorToolResult                   m_locResult; // this variable only can set, not get

	CSharedMemoryBuffer*                 m_pImageBuffer;
	cv::Mat                              m_resultImageBuffer;

	cv::Mat                              m_resultROIBuffer; // ROI
};