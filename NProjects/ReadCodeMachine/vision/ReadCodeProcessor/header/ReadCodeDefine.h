#pragma once

#define MAX_CAMERA_INSPECT_COUNT 1
#define MAX_THREAD_COUNT 1

#define MAX_FRAME_COUNT 15
#define MAX_FRAME_WAIT 10

#define FRAME_DEPTH 8
#define NUMBER_OF_CHANNEL_ORIGINAL 1
#define NUMBER_OF_CHANNEL_BGR 3
#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024

#define MAX_IMAGE_BUFFER 1

#define MAX_ROI_INSPECT 3

#define NUMBER_OF_SET_INSPECT 1
#define MAX_STRING_SIZE		  256
#define MAX_STRING_SIZE_RESULT 1000

typedef enum { InspectStatus_Inspecting, InspectStatus_Stopping, InspectStatus_Busy } emInspectStatus;