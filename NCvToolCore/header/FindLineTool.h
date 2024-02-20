#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

class FindLineTool
{
public:
	FindLineTool();
	~FindLineTool();
	void FindLineWithHoughLine(cv::Mat* pMatSrc, int top, int left, int width, int height, std::vector<cv::Point2f>& vPoints);
};