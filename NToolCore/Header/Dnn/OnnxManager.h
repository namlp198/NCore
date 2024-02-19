#pragma once
#include <chrono>
#include <cmath>
#include <exception>
#include <fstream>
#include <iostream>
#include <limits>
#include <numeric>
#include <string>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/dnn.hpp>

#include <onnxruntime_cxx_api.h>

namespace OrtCppBase
{
	class OnnxManager;
	class OnnxDetectManager;
	class OnnxSegmentManager;
	class OnnxPoseManager;
	class OnnxClassifyManager;

	struct Detection
	{
		cv::Rect box;
		float conf{};
		int classId{};
	};
}
class OrtCppBase::OnnxManager
{
private:
	// pass
protected:
	OnnxManager() {

	}
public:
	virtual ~OnnxManager() {

	}
	virtual void setSessionOption() = 0;
	virtual void Inference(cv::Mat* pMat) = 0;
	virtual std::string test() = 0;
};