#pragma once

#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core.hpp>
#include <opencv2/opencv.hpp>
#include <opencv2/core/cuda.hpp>
#include "OnnxManagerCreator.h"

// DLL
#ifdef _DEBUG
#pragma comment(lib, "opencv_core460d.lib")
#pragma comment(lib, "opencv_calib3d460d.lib")
#pragma comment(lib, "opencv_dnn460d.lib")
#pragma comment(lib, "opencv_highgui460d.lib")
#pragma comment(lib, "opencv_imgcodecs460d.lib")
#pragma comment(lib, "opencv_imgproc460d.lib")
#pragma comment(lib, "opencv_videoio460d.lib")
#pragma comment(lib, "opencv_dnn_objdetect460d.lib")

#pragma comment(lib, "onnxruntime.lib")
#pragma comment(lib, "onnxruntime_providers_cuda.lib")
#pragma comment(lib, "onnxruntime_providers_shared.lib")
#pragma comment(lib, "onnxruntime_providers_tensorrt.lib")

#else
#pragma comment(lib, "opencv_core460.lib")
#pragma comment(lib, "opencv_calib3d460.lib")
#pragma comment(lib, "opencv_dnn460.lib")
#pragma comment(lib, "opencv_highgui460.lib")
#pragma comment(lib, "opencv_imgcodecs460.lib")
#pragma comment(lib, "opencv_imgproc460.lib")
#pragma comment(lib, "opencv_videoio460.lib")
#pragma comment(lib, "opencv_dnn_objdetect460.lib")

#pragma comment(lib, "onnxruntime.lib")
#pragma comment(lib, "onnxruntime_providers_cuda.lib")
#pragma comment(lib, "onnxruntime_providers_shared.lib")
#pragma comment(lib, "onnxruntime_providers_tensorrt.lib")
#endif

struct stDectionResult
{
	int		m_nClass_Idx;			// this class idx later can be changed to match with class count in the setting
	int		m_nDetectedClass_Idx;	// this class idx is the one that model detected, and will not change
	float	m_fScore;
	CRect	m_rtArea_pxl;
};

class CDnn_Manager
{
public:
	CDnn_Manager();
	~CDnn_Manager();

public:
	BOOL Initialize(CString strCfgFilePath, CString strWeightFilePath, BOOL bUseGPU);
	double DetectionImage(cv::Mat* pImage, std::vector<stDectionResult>* vecResult, int nDnnClassesCount);
	void DrawResult(cv::Mat* pGrayInputImage, cv::Mat* pColorInputImage, std::vector<stDectionResult>& vecResult);

private:
	
	// Return processing (inference) time for that image
	double DetectionImage_v4(cv::Mat* pImage, std::vector<stDectionResult>* vecResult);
	double DetectionImage_v7(cv::Mat* pImage, std::vector<stDectionResult>* vecResult, int nDnnClassesCount);
	double DetectionImage_v8(cv::Mat* pImage, std::vector<stDectionResult>* vecResult);

public:
	BOOL						m_bONNXmodel;
	int							m_nYoloVersion;
	cv::dnn::Net				m_net;
	OnnxDetectManager*			m_onnxDetectManager;

	CCriticalSection			*m_pCriticalSection;

	BOOL						m_bUseGPU;
	BOOL						m_bDnnReady;
	CString						m_strCfgFilePath;
	CString						m_strCocoFilePath;
	CString						m_strWeightsFilePath;

	double						m_dModelScale;
	cv::Size					m_szModelSize;
	cv::dnn::DetectionModel*	m_pDetectModel;

	std::vector<CString>		m_vecClasses;
	float						m_fConfidence_Threshold;			// ∞À√‚ Confidence
	float						m_fNonMaximumSupression_Threshold;
};

