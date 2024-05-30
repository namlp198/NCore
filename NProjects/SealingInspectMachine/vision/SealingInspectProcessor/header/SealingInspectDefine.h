#pragma once

#define MAX_CAMERA_INSPECT_COUNT 4

#define MAX_THREAD_COUNT 1

/* TOP Cam */
#define MAX_TOPCAM_COUNT 2
#define MAX_IMAGE_BUFFER_TOPCAM 2
#define FRAME_WIDTH_TOPCAM 2448
#define FRAME_HEIGHT_TOPCAM 2048
//#define FRAME_WIDTH_TOP_CAM 4000
//#define FRAME_HEIGHT_TOP_CAM 3036
#define MAX_FRAME_WAIT_TOPCAM 2

/* SIDE Cam */
#define MAX_SIDECAM_COUNT 2
#define MAX_IMAGE_BUFFER_SIDECAM 4
#define FRAME_WIDTH_SIDECAM 2448
#define FRAME_HEIGHT_SIDECAM 2048
#define MAX_FRAME_WAIT_SIDECAM 4

#define NUMBER_OF_CHANNEL 3
#define FRAME_COUNT 1
#define FRAME_DEPTH 3
#define NUMBER_OF_SET_INSPECT 2
#define MAX_FRAME_COUNT 15
#define MAX_CAM_COUNT_1SET 2

#define SAVE_IMAGE_QUALITY_RATIO			100
#define MAX_STRING_SIZE						256
#define NUMBER_OF_LIGHT_CONTROLLER          2

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


typedef enum { emUNKNOWN = 0, emInspectCavity_Cavity1, emInspectCavity_Cavity2 } emInspectCavity;