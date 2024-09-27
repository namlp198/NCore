#pragma once

#pragma once

#define MAX_CAMERA_INSPECT_COUNT 8
#define MAX_THREAD_COUNT 1
#define MAX_IMAGE_BUFFER 10

#define MAX_FRAME_COUNT 15
#define MAX_FRAME_WAIT 10

#define FRAME_DEPTH 8
#define NUMBER_OF_CHANNEL_ORIGINAL 1
#define NUMBER_OF_CHANNEL_BGR 3
#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024

#define MAX_IMAGE_BUFFER 1

#define MAX_ROI_INSPECT 3

#define MAX_STRING_SIZE		  256
#define MAX_STRING_SIZE_RESULT 1000

// Scalar
#define BLUE_COLOR cv::Scalar(255, 0, 0)
#define GREEN_COLOR cv::Scalar(0, 255, 0)
#define RED_COLOR cv::Scalar(0, 0, 255)
#define PINK_COLOR cv::Scalar(255, 0, 255)
#define YELLOW_COLOR cv::Scalar(0, 255, 255)
#define ORANGE_COLOR cv::Scalar(0, 128, 255)
#define CYAN_COLOR cv::Scalar(255, 255, 0)
#define BLACK_COLOR cv::Scalar(0, 0, 0)
#define WHITE_COLOR cv::Scalar(255, 255, 255)
#define GRAY_COLOR cv::Scalar(128, 128, 128)

#define	PI							3.1415926535897932384626433832795
#define	RAD2DEG(radian)				((radian*180.0)/PI)
#define	DEG2RAD(degree)				((degree*PI)/180.0)

typedef enum { InspectStatus_Inspecting, InspectStatus_Stopping, InspectStatus_Busy } emInspectStatus;
typedef enum
{
	CameraBrand_Hik = 0,
	CameraBrand_Basler,
	CameraBrand_Jai,
	CameraBrand_IRayple
} emCameraBrand;

typedef enum
{
	InspectTool_Locator =0,
	InspectTool_CountPixel = 1,
	InspectTool_CountBlob = 2,
	InspectTool_Calib = 3,
	InspectTool_ColorSpace = 4,
	InspectTool_FindLine = 5,
	InspectTool_FindCircle = 6,
	InspectTool_PCA = 7,
	InspectTool_TrainOCR = 8,
	InspectTool_OCR = 9,
	InspectTool_TemplateMatchingRotate = 10,
	InspectTool_Decode = 11
} emInspectTool;