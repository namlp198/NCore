#include "pch.h"
#include "FindLineTool.h"

FindLineTool::FindLineTool()
{

}

FindLineTool::~FindLineTool()
{
}

void FindLineTool::FindLineWithHoughLine(cv::Mat* pMatSrc, int top, int left, int width, int height, std::vector<cv::Point2f>& vPoints)
{
    cv::Mat matROI = cv::Mat(height, width, CV_8UC1);

    for (int i = 0; i < matROI.rows; i++)
        memcpy(&matROI.data[i * matROI.step1()], &pMatSrc->data[(i + top) * pMatSrc->step1() + left], matROI.cols);

	cv::Mat matBlur, matCanny;
	cv::GaussianBlur(matROI, matBlur, cv::Size(5, 5), 3.0);
	cv::Canny(matBlur, matCanny, 10.0, 30.0);

    std::vector<cv::Vec2f> lines;
    cv::HoughLines(matCanny, lines, 1, CV_PI / 180, 150, 0, 0);
    // draw line
    for (size_t i = 0; i < lines.size(); i++)
    {
        float rho = lines[i][0], theta = lines[i][1];
        cv::Point pt1, pt2;
        double a = cos(theta), b = sin(theta);
        double x0 = a * rho, y0 = b * rho;
        pt1.x = cvRound(x0 + 1000 * (-b));
        pt1.y = cvRound(y0 + 1000 * (a));
        pt2.x = cvRound(x0 - 1000 * (-b));
        pt2.y = cvRound(y0 - 1000 * (a));

        cv::Point2f p1(left, pt1.y + top);
        cv::Point2f p2(left + width, pt2.y + top);

        /*cv::line(*pMatSrc, p1, p2, cv::Scalar(0, 255, 0), 1, cv::LINE_AA);
        cv::circle(*pMatSrc, p1, 3, cv::Scalar(0, 0, 255), cv::FILLED, cv::LINE_AA);
        cv::circle(*pMatSrc, p2, 3, cv::Scalar(0, 0, 255), cv::FILLED, cv::LINE_AA);*/

        vPoints.push_back(p1);
        vPoints.push_back(p2);
    }
}

