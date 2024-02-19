#pragma once
#include "OnnxManager.h"

using namespace OrtCppBase;

class OrtCppBase::OnnxClassifyManager : public OrtCppBase::OnnxManager
{
public:
	void setSessionOption() override {}
	void Inference(cv::Mat* pMat) override;
	std::string test() override;
};

