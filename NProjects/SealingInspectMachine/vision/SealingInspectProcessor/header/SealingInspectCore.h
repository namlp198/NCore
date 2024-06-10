#pragma once

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include "opencv2/video/background_segm.hpp"
#include <opencv2/bgsegm.hpp>

#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectSystemSetting.h"
#include "SealingInspectResult.h"
#include "SealingInspectHikCam.h"
#include "SealingInspect_Simulation_IO.h"
#include "WorkThreadArray.h"
#include <algorithm>
#include <vector>
#include <numeric>
#include <iostream>
#include <tbb/tbb.h>

#define TEST_INSPECT_CAVITY_1
//#undef TEST_INSPECT_CAVITY_1
#define TEST_INSPECT_CAVITY_2
//#undef TEST_INSPECT_CAVITY_2

interface ISealingInspectCoreToParent
{
	virtual void							InspectCavity1Complete(BOOL bSetting) = 0;
	virtual void							InspectCavity2Complete(BOOL bSetting) = 0;
	virtual void							InspectTopCam1Complete(BOOL bSetting) = 0;
	virtual void							InspectTopCam2Complete(BOOL bSetting) = 0;
	virtual void                            GrabFrameSideCam1Complete(BOOL bSetting) = 0;
	virtual void                            GrabFrameSideCam2Complete(BOOL bSetting) = 0;
	virtual CSealingInspectRecipe*          GetRecipe() = 0;
	virtual CSealingInspectSystemSetting*   GetSystemSetting() = 0;
	virtual CSealingInspectHikCam*          GetHikCamControl() = 0;
	virtual LPBYTE                          GetBufferImage_SIDE(int nBuff, int nFrame) = 0;
	virtual LPBYTE                          GetBufferImage_TOP(int nBuff, int nFrame) = 0;
	virtual BOOL                            SetResultBuffer_SIDE(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual BOOL                            SetResultBuffer_TOP(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual CSealingInspectResult*          GetSealingInspectResultControl(int nResIdx) = 0;
	virtual CSealingInspect_Simulation_IO*  GetSealingInspectSimulationIO(int nCoreIdx) = 0;
	virtual BOOL                            GetProcessStatus(int nCoreIdx) = 0;
	virtual void                            SetProcessStatus(int nCoreIdx, BOOL bProcessStatus) = 0;
	virtual BOOL                            GetGrabFrameSideCam(int nCoreIdx) = 0;
	virtual void                            SetGrabFrameSideCam(int nCoreIdx, BOOL bGrab) = 0;
};

class AFX_EXT_CLASS CTempInspectCoreThreadData : public CWorkThreadData
{
public:
	CTempInspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	CTempInspectCoreThreadData(PVOID pPtr, emInspectCavity nSetInsp, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nSetInspect = nSetInsp;
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~CTempInspectCoreThreadData() { Reset(); }
	void Reset()
	{
		m_nSetInspect = emUNKNOWN;
		m_nThreadIdx = -1;
	}

public:
	emInspectCavity			        m_nSetInspect;		// set Cam
	UINT							m_nThreadIdx;
};

class AFX_EXT_CLASS CSealingInspectCore : public IWorkThreadArray2Parent
{
public:
	CSealingInspectCore(ISealingInspectCoreToParent* pInterface);
	~CSealingInspectCore();

public:
	void CreateInspectThread(int nThreadCount, emInspectCavity nInspCavity);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread_INSPECT_CAVITY1(int nThreadIndex);
	void RunningThread_INSPECT_CAVITY2(int nThreadIndex);
	void StopThread();

	void ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat);
	void ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat);
	void ProcessFrame_SideCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, int nFrameIdx, cv::Mat& mat);

	void TestInspectCavity1(int nCoreIdx);
	void TestInspectCavity2(int nCoreIdx);

	void Inspect_TopCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame);
	void Inspect_SideCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame);

private:
	BOOL FindCircle_MinEnclosing(cv::Mat* matProcess, int nThresholdBinary, int nContourSizeMin, int nContourSizeMax,
		                        int nRadiusInnerMin, int nRadiusInnerMax,
		                        std::vector<std::vector<cv::Point>>& vecContours, 
		                        std::vector<cv::Vec4i>& vecHierarchy, cv::Point2f& center, double& dRadius);

	BOOL FindCircle_HoughCircle(cv::Mat* matProcess, std::vector<cv::Vec3f>& vecCircles, 
		                        std::vector<cv::Point2i>& vecPts, int nThresholdCanny, int minDist, int nParam1, int nParam2, 
		                        int nRadiusOuterMin, int nRadiusOuterMax, double dIncreAngle);

	BOOL FindDistanceAll_OuterToInner(std::vector<cv::Point2i>& vecPts,
		                              std::vector<cv::Point2i>& vecPtsIntersection, 
		                              std::vector<cv::Point2f>& vecIntsecPtsFound,
		                              cv::Point2f center, double radius,
		                              std::vector<double>& vecDistance);

	BOOL JudgementInspectDistanceMeasurement(std::vector<double>& vecDistance, std::vector<int>& vecPosNG, 
		                                     double nDistanceMin, double nDistanceMax);

	BOOL JudgementInspectDistanceMeasurement_AdvancedAlgorithms(std::vector<double>& vecDistance, std::vector<int>& vecPosNG, double nDistanceMin, double nDistanceMax, int nNumberOfDistNGMax);

	BOOL FindMeasurePointsAtPosMinMax(CRecipe_TopCam_Frame1* pRecipeTopCamFrame1, cv::Mat* pMatProcess, cv::Rect& rectROI, std::vector<cv::Point>& vecMeaPts, int nROIIdx);

	BOOL FindMeasurePointsAtPosDistMinMax_SideCam(CSealingInspectRecipe_SideCam* pRecipeSideCam, cv::Mat* pImageData, int nFrame, cv::Rect rectROI, std::vector<cv::Point>& vecMeaPts);

	BOOL FindMeasurePoints_SideCam(const CSealingInspectRecipe_SideCam pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect rectROI, std::vector<cv::Point>& vecMeaPts);

	BOOL CalculateDistancePointToLine(cv::Point measurePt, cv::Point2f p1, cv::Point2f p2, cv::Point2f& closesPt, float& fDistance);

	BOOL FindClosesPointAndDistancePointToLine(const std::vector<cv::Point>& vecMeasurePt,const std::vector<cv::Point2f>& vecPtLineTop, std::vector<cv::Point2f>& vecClosesPt, std::vector<double>& vecDist);

	void MakeROIAdvancedAlgorithms(CRecipe_TopCam_Frame1 recipeTopCamFrame1, std::vector<cv::Rect>& vecRectROI, cv::Point centerPt, double dRadius);

	void MakeROITopCamFrame2(const CRecipe_TopCam_Frame2* pRecipeTopCamFrame2, std::vector<cv::Rect>& vecRectROI, cv::Mat* pMatProcess, cv::Point centerPt, double dRadius);

	void MakeROIFindLine(CSealingInspectRecipe_SideCam& pRecipeSideCam, int nFrame, cv::Rect& rectROIFindLIne);

	void MakeROIFindPoints(CSealingInspectRecipe_SideCam& pRecipeSideCam, int nFrame, cv::Rect& rectROIFindPts);

	BOOL MakeROI_AdvancedAlgorithms(CSealingInspectRecipe_SideCam recipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROIFindLIne, cv::Rect& rectROIFindPts);

	BOOL MakeCannyEdgeImage(cv::Mat* pImageData, cv::Mat& pEdgeImageData, double dThreshold1, double dThreshold2, int nGaussianMask = 3);

	BOOL FindLagestElementsInVector(std::vector<int>& vecNum, int k, std::vector<int>& vecElementIndex);

	BOOL FindSmallestElementsInVector(std::vector<int>& vecNum, int k, std::vector<int>& vecElementIndex);

	BOOL FindLine_Top_Bottom_Average(CSealingInspectRecipe_SideCam& pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROI, std::vector<cv::Point2f>& vecPtsLine);

	BOOL FindLine_Bottom_Top_Average(CSealingInspectRecipe_SideCam* pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROI, cv::Mat& matROI, std::vector<cv::Point2f>& vecPtsLine);

	BOOL FindBlob_SealingSurface(CRecipe_TopCam_Frame2 pRecipeTopCamFrame2, cv::Mat* pMatProcess, cv::Point2f ptCenter, double dRadius, std::vector<std::vector<cv::Point>>& mContours);

	BOOL FindBlob(CRecipe_TopCam_Frame2 pRecipeTopCamFrame2, cv::Mat* pMatProcess, std::vector<std::vector<cv::Point>>& mContours);
private:

	double             CalculateDistancePointToCircle(cv::Point2i pt, cv::Point2f centerPt, double dRadius);

	cv::Point2i        CalculateIntersectionPointCoordinate(cv::Point2i pt, cv::Point2f centerPt, double dRadius, double dDist);

	cv::Point2f        FindIntersectionPoint_LineCircle(cv::Point2i pt, cv::Point2f centerPt, double dRadius);

	void               Draw_MinEnclosing(cv::Mat& matSrc, std::vector<std::vector<cv::Point>>& vecContours,
		                                 cv::Point2f vecCenters, float vecRadius);
	void               Draw_HoughCircle(cv::Mat& matSrc, std::vector<cv::Vec3f>& vecCircles, std::vector<cv::Point2i>& vecPts);

	void               DrawDistance(cv::Mat& mat, std::vector<cv::Point> vecPts, std::vector<cv::Point> vecIntsecPts);

	void               DrawPositionNG(cv::Mat& mat, std::vector<int>& vecPosNG, std::vector<cv::Point> vecPts);

	void               DrawROIFindLine(cv::Mat& mat, cv::Rect rectROI, std::vector<cv::Point2f> vecPtsLine);

	void               DrawROIFindPoints(cv::Mat& mat, cv::Rect rectROI, std::vector<cv::Point> vecMeasurePt, std::vector<cv::Point2f> vecClosesPt);

private:
	double             ConvertUmToPixel(double dGap, double dPixSize) { return (dGap * 1000.0) / dPixSize; }
	double             ConvertPixelToUm(double dPxl, double dPxlSize) { return (dPxl * dPxlSize) / 1000.0; }

public:

	// setter
	void        SetSimulatorMode(BOOL bSimu) { m_bSimulator = bSimu; }
	//void        SetProcessStatus(BOOL bProcessStatus) { m_bProcessStatus = bProcessStatus; }
	void        SetCoreIndex(UINT nCoreIdx) { m_nCoreIdx = nCoreIdx; }
	// getter
	BOOL        GetSimulatorMode() { return m_bSimulator; }
	//BOOL        GetProcessStatus() { return m_bProcessStatus; }
	UINT        GetCoreIndex() { return m_nCoreIdx; }

private:

	// interface
	ISealingInspectCoreToParent*        m_pInterface;

	UINT								m_nThreadCount;

	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	BOOL                                m_bSimulator;
	
	//BOOL                                m_bProcessStatus;

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	std::vector<BOOL>					m_vecProcessedFrame;

	CCriticalSection					m_csPostProcessing;

	UINT                                m_nCoreIdx;

	cv::Point2f                         m_ptCenter_Inner;
	cv::Point2f                         m_ptCenter_Outer;
	double                              m_dRadius_Inner;
	double                              m_dRadius_Outer;
};