#pragma once
#include "OnnxManager.h"
#include "OnnxDetectManager.h"
#include "OnnxSegmentManager.h"
#include "OnnxClassifyManager.h"
#include "OnnxPoseManager.h"

#include <iostream>

using namespace OrtCppBase;

class OnnxManagerCreator
{
public:
	virtual ~OnnxManagerCreator() {

	}
	virtual OnnxManager* FactoryMethod() const = 0;

	virtual void setSessionOption() {
		OnnxManager* onnxManager = this->FactoryMethod();
		onnxManager->setSessionOption();
		delete onnxManager;
	}
	virtual void Inference(cv::Mat* pMat) {
		OnnxManager* onnxManager = this->FactoryMethod();
		onnxManager->Inference(pMat);
		delete onnxManager;
	}
	virtual void test() {
		OnnxManager* onnxManager = this->FactoryMethod();
		std::cout << "Hello, This is: " + onnxManager->test() << std::endl;
		delete onnxManager;
	}
};

class OnnxDetectManagerCreator : public OnnxManagerCreator
{
public:
	OnnxDetectManager* FactoryMethod() const override {
		return new OnnxDetectManager();
	}
};

class OnnxSegmentManagerCreator : public OnnxManagerCreator
{
public:
	OnnxSegmentManager* FactoryMethod() const override {
		return new OnnxSegmentManager();
	}
};

class OnnxPoseManagerCreator : public OnnxManagerCreator
{
public:
	OnnxPoseManager* FactoryMethod() const override {
		return new OnnxPoseManager();
	}
};

class OnnxClassifyManagerCreator : public OnnxManagerCreator
{
public:
	OnnxClassifyManager* FactoryMethod() const override {
		return new OnnxClassifyManager();
	}
};

