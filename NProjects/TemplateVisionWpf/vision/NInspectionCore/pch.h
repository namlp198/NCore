// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"

#include <afxv_dll.h>
#include <afxver_.h>
//#include <afxstr.h>
#include <afxmt.h>
#include <cstringt.h>
#include <afxwin.h>
#include <afxext.h>
#include <windef.h>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/features2d.hpp>

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

#pragma comment(lib, "MvCameraControl.lib")

#pragma comment(lib, "GCBase_MD_VC141_v3_1_Basler_pylon.lib")
#pragma comment(lib, "GenApi_MD_VC141_v3_1_Basler_pylon.lib")
#pragma comment(lib, "PylonBase_v7_4.lib")
#pragma comment(lib, "PylonC.lib")
#pragma comment(lib, "PylonC_v7_4.lib")
#pragma comment(lib, "PylonDataProcessing_v1_3.lib")
#pragma comment(lib, "PylonGUI_v7_4.lib")
#pragma comment(lib, "PylonUtility_v7_4.lib")

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

#pragma comment(lib, "MvCameraControl.lib")
#pragma comment(lib, "RapidXML_Release_New.lib")

#endif
#endif //PCH_H
