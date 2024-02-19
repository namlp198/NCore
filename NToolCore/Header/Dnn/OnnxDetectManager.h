#pragma once
#include "OnnxManager.h"
//#include "windows.h"

using namespace OrtCppBase;

class OrtCppBase::OnnxDetectManager : public OrtCppBase::OnnxManager
{
private:
#pragma region Variables
	const int64_t BATCHSIZE = 1;
	const std::string INSTANCENAME{ "crack_2.4_inference_yolov8" };
	bool m_bUsingGPU;
	float m_fConfThreshold;
	float m_fIoUThreshold;
	std::string m_strLabelFilePath;
	std::wstring m_wstrOnnxFilePath;
	std::vector<std::string> m_vecLabels;
	Ort::SessionOptions m_sessionOptions;
	Ort::Session m_session{ nullptr };
	Ort::Env m_env;

	std::vector<int> m_classIds;
	std::vector<float> m_scores;
	std::vector<cv::Rect> m_boxes;
	double m_dOverallTime;

#pragma endregion
#pragma region Functions
	std::vector<std::string> readLabels(std::string& labelFilepath);
	cv::Rect2f               scaleCoords(const cv::Size& imageShape, cv::Rect2f coords, const cv::Size& imageOriginalShape, bool p_Clip = false);
	void                     getBestClassInfo(const cv::Mat& p_Mat, const int& numClasses, float& bestConf, int& bestClassId);
	std::vector<Detection>   postprocessing(const cv::Size& resizedImageShape, const cv::Size& originalImageShape, std::vector<Ort::Value>& outputTensors, const float& confThreshold, const float& iouThreshold);
	void                     letterbox(const cv::Mat& image, cv::Mat& outImage, const cv::Size& newShape = cv::Size(640, 640), const cv::Scalar& color = cv::Scalar(114, 114, 114), bool auto_ = true, bool scaleFill = false, bool scaleUp = true, int stride = 32);
	int                      calculate_product(const std::vector<int64_t>& v);
	std::string              getExePath();
#pragma endregion
public:
	OnnxDetectManager();
	~OnnxDetectManager();
	void setSessionOption() override;
	void Inference(cv::Mat* pMat) override;

#pragma region Setter, Getter
	void setLabelFielPath(std::string labelFilePath);
	std::string getLabelFilePath();
	void setOnnxFilePath(std::wstring onnxFilePath);
	std::wstring getOnnxFilePath();
	void setUsingGPU(bool b);
	bool getUsingGPU();
	void setConfThreshold(float conf);
	float getConfThreshold();
	void setIoUThreshold(float iou);
	float getIoUThreshold();
	double getDetectionResults(std::vector<int>& classIds, std::vector<float>& scores, std::vector<cv::Rect>& boxes);
#pragma endregion

#pragma region Test
	std::string test() override;
#pragma endregion
};

