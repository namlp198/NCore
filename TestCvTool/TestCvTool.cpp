
#ifdef _DEBUG
#pragma comment(lib, "opencv_core480d.lib")
#pragma comment(lib, "opencv_features2d480d.lib")
#pragma comment(lib, "opencv_flann480d.lib")
#pragma comment(lib, "opencv_highgui480d.lib")
#pragma comment(lib, "opencv_imgcodecs480d.lib")
#pragma comment(lib, "opencv_imgproc480d.lib")
#pragma comment(lib, "opencv_videoio480d.lib")
#pragma comment(lib, "opencv_ml480d.lib")
#pragma comment(lib, "opencv_dnn480d.lib")
#pragma comment(lib, "opencv_ccalib480d.lib")
#pragma comment(lib, "opencv_calib3d480d.lib")
#pragma comment(lib, "opencv_aruco480d.lib")
#else
#pragma comment(lib, "opencv_core480.lib")
#pragma comment(lib, "opencv_features2d480.lib")
#pragma comment(lib, "opencv_flann480.lib")
#pragma comment(lib, "opencv_highgui480.lib")
#pragma comment(lib, "opencv_imgcodecs480.lib")
#pragma comment(lib, "opencv_imgproc480.lib")
#pragma comment(lib, "opencv_videoio480.lib")
#pragma comment(lib, "opencv_ml480.lib")
#pragma comment(lib, "opencv_dnn480.lib")
#pragma comment(lib, "opencv_ccalib480.lib")
#pragma comment(lib, "opencv_calib3d480.lib")
#pragma comment(lib, "opencv_aruco480.lib")

#pragma comment(lib, "NCvToolCore_Release64.lib")

#endif // _DEBUG

#include <iostream>
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/highgui.hpp>
#include <FindLineTool.h>

int main()
{
    cv::namedWindow("source", cv::WINDOW_AUTOSIZE);
    cv::Mat mat = cv::imread("D:\\NCore\\TestCvTool\\imgs\\test03.jpg", cv::IMREAD_COLOR);

    if (mat.empty())
    {
        return EXIT_FAILURE;
    }
    // select ROI
    cv::Rect rectROI = cv::selectROI("source", mat, false);
    FindLineTool finder;
    std::vector<cv::Point2f> vPoints;
    finder.FindLineWithHoughLine(&mat, rectROI, vPoints);

    cv::imshow("source", mat);
    cv::waitKey(0);

    return EXIT_SUCCESS;
}
