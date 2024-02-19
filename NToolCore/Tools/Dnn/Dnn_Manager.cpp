#include "pch.h"
#include "Dnn_Manager.h"

CDnn_Manager::CDnn_Manager()
{
	m_dModelScale = 1 / 255.0;
	m_szModelSize = cv::Size(608, 608);

	m_fConfidence_Threshold = 0.25;
	m_fNonMaximumSupression_Threshold = 0.4;

	m_pDetectModel = NULL;
	m_bDnnReady = FALSE;
	m_nYoloVersion = 4;

	m_onnxDetectManager = NULL;

	m_pCriticalSection = NULL;
	m_pCriticalSection = new CCriticalSection();
}

CDnn_Manager::~CDnn_Manager()
{
	if (m_pDetectModel != NULL)
		delete m_pDetectModel, m_pDetectModel = NULL;

	if (m_onnxDetectManager != NULL)
		delete m_onnxDetectManager, m_onnxDetectManager = NULL;

	if (m_pCriticalSection)
	{
		delete m_pCriticalSection;
	}
	m_pCriticalSection = NULL;
	m_bDnnReady = FALSE;
}

BOOL CDnn_Manager::Initialize(CString strCfgFilePath, CString strWeightFilePath, BOOL bUseGPU)
{
	if ((strCfgFilePath.Right(3).CompareNoCase(_T("cfg")) != 0 || strWeightFilePath.Right(7).CompareNoCase(_T("weights")) != 0) && strWeightFilePath.Right(4).CompareNoCase(_T("onnx")) != 0)
	{
		m_bDnnReady = FALSE;
		return FALSE;
	}
	CFileFind finder;
	BOOL bCfgExist = finder.FindFile(strCfgFilePath);
	BOOL bWeightExist = finder.FindFile(strWeightFilePath);

	if ((strWeightFilePath.Right(7).CompareNoCase(_T("weights")) != 0 && (bCfgExist == FALSE || bWeightExist == FALSE) ) ||
		(strWeightFilePath.Right(4).CompareNoCase(_T("onnx")) != 0 && bWeightExist == FALSE))
	{
		CString strPath = _T("Deep Learning Config and Weight files do not exist!: ") + strCfgFilePath + _T("||") + strWeightFilePath;
		AfxMessageBox(strPath);
		m_bDnnReady = FALSE;
		return FALSE;
	}
	
	// Initialize
	m_bUseGPU = bUseGPU;

	// File Path
	m_strCfgFilePath = strCfgFilePath;

	if (m_strCfgFilePath.IsEmpty())
	{
		//AfxMessageBox(_T("Can not empty the .cfg file"));
		m_bDnnReady = FALSE;
		return FALSE;
	}

	m_strWeightsFilePath = strWeightFilePath;

	if (m_strWeightsFilePath.IsEmpty())
	{
		//AfxMessageBox(_T("Can not empty the .weights file"));
		m_bDnnReady = FALSE;
		return FALSE;
	}

	// 1. Load Class Types

	USES_CONVERSION;
	char cCfgFilePath[1024] = {};
	sprintf_s(cCfgFilePath, "%s", W2A(m_strCfgFilePath));
	char cWeightsFilePath[1024] = {};
	sprintf_s(cWeightsFilePath, "%s", W2A(m_strWeightsFilePath));


	if (m_strWeightsFilePath.Right(4).CompareNoCase(_T("onnx")) == 0)
	{
		m_nYoloVersion = 7;

		int index = strWeightFilePath.Find(_T("v8"));
		if (index != -1)
			m_nYoloVersion = 8;

		if (m_nYoloVersion == 7)
		{
			// load onnx model using dnn (for v7)
			m_net = cv::dnn::readNetFromONNX(cWeightsFilePath);
		}
		else
		{
			// use onnxManager for v8
			// Setting
			std::wstring wsWeightFilePath(strWeightFilePath);
			
			m_onnxDetectManager = new OnnxDetectManager();
			m_onnxDetectManager->setOnnxFilePath(wsWeightFilePath);
			m_onnxDetectManager->setConfThreshold(m_fConfidence_Threshold);
			m_onnxDetectManager->setIoUThreshold(m_fNonMaximumSupression_Threshold);
		}
		
		//
		m_bONNXmodel = TRUE;
	}
	else
	{
		// Load v4 weight model
		m_net = cv::dnn::readNetFromDarknet(cCfgFilePath, cWeightsFilePath);
		m_bONNXmodel = FALSE;
	}

	if(m_bUseGPU == TRUE)
	{
		if (m_nYoloVersion == 8)
		{
			// use v8
			m_onnxDetectManager->setUsingGPU(true);
			m_onnxDetectManager->setSessionOption();
		}
		else
		{
			// use v7 / v4
			m_net.setPreferableTarget(cv::dnn::DNN_TARGET_CUDA);
			m_net.setPreferableBackend(cv::dnn::DNN_BACKEND_CUDA);
		}
	}
	else
	{
		if (m_nYoloVersion == 8)
		{
			// use v8
			m_onnxDetectManager->setUsingGPU(false);
			m_onnxDetectManager->setSessionOption();
		}
	}

	if (m_pDetectModel != NULL)
		delete m_pDetectModel, m_pDetectModel = NULL;

	m_pDetectModel = new cv::dnn::DetectionModel(m_net);
	m_pDetectModel->setInputParams(m_dModelScale, m_szModelSize, cv::Scalar(), true);

	m_bDnnReady = TRUE;
	return TRUE;
}

// interface function
double CDnn_Manager::DetectionImage(cv::Mat* pImage, std::vector<stDectionResult>* vecResult, int nDnnClassesCount)
{
	if (m_bONNXmodel)
	{
		if (m_nYoloVersion == 7)
			return DetectionImage_v7(pImage, vecResult, nDnnClassesCount);
		else
			return DetectionImage_v8(pImage, vecResult);
	}
	else
		return DetectionImage_v4(pImage, vecResult);
}

double CDnn_Manager::DetectionImage_v4(cv::Mat* pImage, std::vector<stDectionResult>* vecResult)
{
	if (m_bDnnReady == FALSE)
		return FALSE;

	if (vecResult == NULL)
		return FALSE;

	vecResult->clear();

	if (pImage == NULL)
		return FALSE;

	if (m_pDetectModel == NULL)
		return FALSE;

	if (m_net.empty())
		return FALSE;
	/*
	if (m_vecClasses.size() < 1)
		return FALSE;
	*/

	std::vector<int> classIds;
	std::vector<float> scores;
	std::vector<cv::Rect> boxes;

	CSingleLock localLock(m_pCriticalSection);
	localLock.Lock();

	cv::Mat pColorImg;
	cv::cvtColor(*(pImage), pColorImg, cv::COLOR_GRAY2BGR);

	m_pDetectModel->detect(pColorImg, classIds, scores, boxes, m_fConfidence_Threshold, m_fNonMaximumSupression_Threshold);

	std::vector<double> layersTimes;
	double freq = cv::getTickFrequency() / 1000;
	double dProcessingTime = m_net.getPerfProfile(layersTimes) / freq;

	vecResult->resize(classIds.size());

	for (int i = 0; i < classIds.size(); i++)
	{
		(*vecResult)[i].m_nClass_Idx = classIds[i];
		(*vecResult)[i].m_fScore = scores[i];
		(*vecResult)[i].m_rtArea_pxl.left = boxes[i].x;
		(*vecResult)[i].m_rtArea_pxl.top = boxes[i].y;
		(*vecResult)[i].m_rtArea_pxl.right = boxes[i].x + boxes[i].width;
		(*vecResult)[i].m_rtArea_pxl.bottom = boxes[i].y + boxes[i].height;

		// if the detected defect is water (class 1), and the height/width ratio is too large, then should change it to chip defect
		if (classIds[i] == 1) // water
		{
			float fHeightWidthRatio = (float)boxes[i].height / (float)boxes[i].height;
			if (fHeightWidthRatio > 1.5)
				(*vecResult)[i].m_nClass_Idx = 2; // change to Chip
		}

		(*vecResult)[i].m_nDetectedClass_Idx = (*vecResult)[i].m_nClass_Idx;
	}

	localLock.Unlock();

	return dProcessingTime;
}

double CDnn_Manager::DetectionImage_v7(cv::Mat* pImage, std::vector<stDectionResult>* vecResult, int nDnnClassesCount)
{
	if (m_bDnnReady == FALSE)
		return FALSE;

	if (vecResult == NULL)
		return FALSE;

	vecResult->clear();

	if (pImage == NULL)
		return FALSE;

	if (m_pDetectModel == NULL)
		return FALSE;

	if (m_net.empty())
		return FALSE;

	CSingleLock localLock(m_pCriticalSection);
	localLock.Lock();

	cv::Mat blob;
	cv::Size modelShape(640.0, 640.0);

	// Calculate taks time detect
	cv::Mat pColorImg;
	cv::cvtColor(*(pImage), pColorImg, cv::COLOR_GRAY2BGR);
	cv::dnn::blobFromImage(pColorImg, blob, 1.0 / 255.0, modelShape, cv::Scalar(), true, false);
	m_net.setInput(blob);

	std::vector<cv::Mat> outputs;
	m_net.forward(outputs, m_net.getUnconnectedOutLayersNames());

	float x_factor = (float)pImage->rows / (float)modelShape.width;
	float y_factor = (float)pImage->cols / (float)modelShape.height;

	float* data = (float*)outputs[0].data;

	// Number of Classes + 5
	const int dimensions = outputs[0].size[2];

	// Natural number of the YOLO detections (Usually 25200 @640x640)
	const int rows = outputs[0].size[1];

	std::vector<int> classIds;
	std::vector<float> scores;
	std::vector<cv::Rect> boxes;

	for (int i = 0; i < rows; ++i)
	{
		float confidence = data[4];

		if (confidence >= m_fConfidence_Threshold)
		{
			float* classes_scores = data + 5;
			cv::Mat mat_scores(1, nDnnClassesCount, CV_32FC1, classes_scores);
			cv::Point class_id;
			double max_class_score;

			minMaxLoc(mat_scores, 0, &max_class_score, 0, &class_id);

			if (max_class_score > m_fConfidence_Threshold)
			{
				scores.push_back(confidence);
				classIds.push_back(class_id.x);

				float x = data[0];
				float y = data[1];
				float w = data[2];
				float h = data[3];

				int left = int((x - 0.5 * w) * x_factor);
				int top = int((y - 0.5 * h) * y_factor);

				int width = int(w * x_factor);
				int height = int(h * y_factor);

				boxes.push_back(cv::Rect(left, top, width, height));
			}
		}

		data += dimensions;
	}

	std::vector<int> nms_result;
	cv::dnn::NMSBoxes(boxes, scores, m_fConfidence_Threshold, m_fNonMaximumSupression_Threshold, nms_result);

	// output
	vecResult->resize(nms_result.size());

	for (int i = 0; i < nms_result.size(); i++)
	{
		int idx = nms_result[i];

		(*vecResult)[i].m_nClass_Idx = classIds[idx];
		(*vecResult)[i].m_fScore = scores[idx];
		(*vecResult)[i].m_rtArea_pxl.left = boxes[idx].x;
		(*vecResult)[i].m_rtArea_pxl.top = boxes[idx].y;
		(*vecResult)[i].m_rtArea_pxl.right = boxes[idx].x + boxes[idx].width;
		(*vecResult)[i].m_rtArea_pxl.bottom = boxes[idx].y + boxes[idx].height;
		(*vecResult)[i].m_nDetectedClass_Idx = (*vecResult)[i].m_nClass_Idx;
	}
	//
	std::vector<double> layersTimes;
	double freq = cv::getTickFrequency() / 1000;
	double dProcessingTime = m_net.getPerfProfile(layersTimes) / freq;

	localLock.Unlock();

	return dProcessingTime;
}

double CDnn_Manager::DetectionImage_v8(cv::Mat* pImage, std::vector<stDectionResult>* vecResult)
{
	if (m_bDnnReady == FALSE)
		return FALSE;

	if (vecResult == NULL)
		return FALSE;

	vecResult->clear();

	if (pImage == NULL)
		return FALSE;

	if (m_onnxDetectManager == NULL)
		return FALSE;

	CSingleLock localLock(m_pCriticalSection);
	localLock.Lock();

	// Calculate taks time detect
	cv::Mat pColorImg;
	cv::cvtColor(*(pImage), pColorImg, cv::COLOR_GRAY2BGR);
	
	m_onnxDetectManager->Inference(&pColorImg);

	std::vector<int> classIds;
	std::vector<float> scores;
	std::vector<cv::Rect> boxes;

	double dProcessingTime = m_onnxDetectManager->getDetectionResults(classIds, scores, boxes);

	vecResult->resize(classIds.size());

	for (int idx = 0; idx < classIds.size(); idx++)
	{
		(*vecResult)[idx].m_nClass_Idx = classIds[idx];
		(*vecResult)[idx].m_fScore = scores[idx];
		(*vecResult)[idx].m_rtArea_pxl.left = boxes[idx].x;
		(*vecResult)[idx].m_rtArea_pxl.top = boxes[idx].y;
		(*vecResult)[idx].m_rtArea_pxl.right = boxes[idx].x + boxes[idx].width;
		(*vecResult)[idx].m_rtArea_pxl.bottom = boxes[idx].y + boxes[idx].height;
		(*vecResult)[idx].m_nDetectedClass_Idx = (*vecResult)[idx].m_nClass_Idx;
	}


	localLock.Unlock();

	return dProcessingTime;
}


void CDnn_Manager::DrawResult(cv::Mat* pGrayInputImage, cv::Mat* pColorInputImage, std::vector<stDectionResult> &vecResult)
{
	cv::cvtColor(*pGrayInputImage, *pColorInputImage, cv::COLOR_GRAY2BGR);
	std::vector<std::string> classes;
	classes.push_back("artifact");
	classes.push_back("water");
	classes.push_back("chip");
	classes.push_back("crack");

	for (int i = 0; i < vecResult.size(); i++) {

		char text[100];
		snprintf(text, sizeof(text), "%s: %.2f", classes[vecResult[i].m_nClass_Idx].c_str(), vecResult[i].m_fScore);
		cv::Rect boxes;
		boxes.x = vecResult[i].m_rtArea_pxl.left;
		boxes.y = vecResult[i].m_rtArea_pxl.top;
		boxes.width = vecResult[i].m_rtArea_pxl.Width();
		boxes.height = vecResult[i].m_rtArea_pxl.Height();

		if (vecResult[i].m_nClass_Idx == 0)
		{
			cv::rectangle(*pColorInputImage, boxes, cv::Scalar(0, 255, 0), 1);
			cv::putText(*pColorInputImage, text, cv::Point(boxes.x - 10, boxes.y - 5), cv::FONT_HERSHEY_PLAIN, 1,
				cv::Scalar(0, 255, 0), 1);
		}
		else if (vecResult[i].m_nClass_Idx == 1)
		{
			cv::rectangle(*pColorInputImage, boxes, cv::Scalar(0, 255, 255), 1);
			cv::putText(*pColorInputImage, text, cv::Point(boxes.x - 10, boxes.y - 5), cv::FONT_HERSHEY_PLAIN, 1,
				cv::Scalar(0, 255, 0), 1);
		}
		else
		{
			cv::rectangle(*pColorInputImage, boxes, cv::Scalar(0, 0, 255), 1);
			cv::putText(*pColorInputImage, text, cv::Point(boxes.x - 10, boxes.y - 5), cv::FONT_HERSHEY_PLAIN, 1,
				cv::Scalar(0, 0, 255), 1);
		}
	}
}