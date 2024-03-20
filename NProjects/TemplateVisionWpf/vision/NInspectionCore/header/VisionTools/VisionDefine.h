#pragma once

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