#pragma once
#include "TempInspectHikCam.h"
#include "TempInspectCore.h"
#include "TempInspectSystemConfig.h"
#include "TempInspectStatus.h"
#include "TempInspectRecipe.h"

class AFX_EXT_CLASS CTempInspectProcessor : public ITempInspectHikCamToParent,
	                                        public ITempInspectCoreToParent
{
public:
	CTempInspectProcessor();
	~CTempInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadSystemConfig();
	BOOL LoadRecipe();
	BOOL CreateBuffer();

public:
	BOOL InspectStart(int nThreadCount, int nCamIdx);
	BOOL InspectStop(int nCamIdx);

	static void ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx,int nNextFrameIdx, LPVOID param);
	void ReceivedImageCallbackEx(int nGrabberIdx, int nNextFrameIdx, LPVOID pBuffer);

public:
	virtual void							InspectComplete(int nCamIdx);
	virtual LPBYTE							GetFrameImage(int nCamIdx, UINT nFrameIndex);
	virtual LPBYTE							GetBufferImage(int nCamIdx, UINT nY);
	virtual CTempInspectRecipe*             GetRecipe(int nIdx) { return m_pTempInspRecipe[nIdx]; }
	virtual CTempInspectSystemConfig*       GetSystemConfig() { return m_pTempInspSysConfig; }
	virtual CTempInspectStatus*             GetTempInspectStatus(int nCamIdx) { return m_pTempInspStatus[nCamIdx]; }
	virtual int                             PopInspectWaitFrame(int nCamIdx);

public:
	CTempInspectHikCam*    GetHikCamControl() { return m_pHikCamera; }
	CLocatorTool*          GetLocToolControl() { return m_pLocToolTrain; }
	CVisionAlgorithms*     GetVsAlgorithmControl() { return m_pVsAlgorithm; }

public:
	// LocatorTool: train loc tool and after that get: data trained, template image
	BOOL                   TrainLocator_TemplateMatching(int nCamIdx, CRectForTrainLocTool* rectForTrainLoc); // step 1
	BYTE*                  GetTemplateImage();                                             // step 2
	BOOL                   GetDataTrained_TemplateMatching(CLocatorToolResult* dataTrained); // step 3

	// SelectROITool
	BOOL                   CountPixelAlgorithm_Train(CParamCntPxlAlgorithm* pParamCntPxlTrain);
	BOOL                   CalculateAreaAlgorithm_Train(CParamCalAreaAlgorithm* pParamTrainCalArea);
	BYTE*                  GetResultROIBuffer_Train();
	// pass a reference from the client have format is a structure for getting to data Count Pixel after then trained.
	BOOL                   GetResultCntPxl_Train(CAlgorithmsCountPixelResult* pCntPxlTrainRes);
	// pass a reference from the client have format is a structure for getting to data Area after then trained.
	BOOL                   GetResultCalArea_Train(CAlgorithmsCalculateAreaResult* pCalAreaTrainRes);

public:
	BOOL                   GetSumResult(int nCamIdx, CSumResult* pSumRes); // this func will to get to the result when the program runtime

private:

	// Hik cam
	CTempInspectHikCam*                 m_pHikCamera;

	// Inspect Core
	CTempInspectCore*                   m_pTempInspCore[MAX_CAMERA_INSP_COUNT];

	// config
	CTempInspectSystemConfig*           m_pTempInspSysConfig;

	// recipe
	CTempInspectRecipe*                 m_pTempInspRecipe[MAX_CAMERA_INSP_COUNT];
						                
	// status			                
	CTempInspectStatus*                 m_pTempInspStatus[MAX_CAMERA_INSP_COUNT];

	// Inspect Wait Frame List
	CCriticalSection					m_csInspectWaitList[MAX_CAMERA_INSP_COUNT];
	std::queue<int>						m_queueInspectWaitList[MAX_CAMERA_INSP_COUNT];

	// Image Buffer..
	CSharedMemoryBuffer*                m_pImageBuffer[MAX_CAMERA_INSP_COUNT];

	// Locator tool - this is object for train locator tool
	CLocatorTool*                       m_pLocToolTrain;
	// Vision Algorithm
	CVisionAlgorithms*                  m_pVsAlgorithm;
};