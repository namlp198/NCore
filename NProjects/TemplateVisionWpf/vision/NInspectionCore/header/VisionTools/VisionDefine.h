#pragma once

#define MAX_CAMERA_INSP_COUNT 2
#define MAX_THREAD_COUNT 1
#define MAX_FRAME_COUNT 15

enum emVisionTools
{
	emLocatorTool,
	emSelectROITool,
};

enum emVisionParams
{
	emParamLocatorTool,
	emParamSelectROITool,
	emParamCountPixel,
	emParamCalculateArea
};

enum emVisionResults
{
	emLocatorToolResult,
	emSelectROIToolResult,
	emCountPixelResult,
	emCalculateAreaResult,
};

enum emAlgorithms
{
	emCountPixel,
	emCalculateArea,
	emCalculateCoordinate,
	emCountBlob,
	emFindLine,
	emFindCircle,
	emOCR
};