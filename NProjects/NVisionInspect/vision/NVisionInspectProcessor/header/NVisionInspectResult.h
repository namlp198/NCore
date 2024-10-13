#pragma once
#include "NVisionInspectDefine.h"

struct CNVisionInspectResult_Cam1
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	TCHAR m_sResultString[MAX_STRING_SIZE_RESULT];
};
struct CNVisionInspectResult_Cam2
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	TCHAR m_sResultString[MAX_STRING_SIZE_RESULT];
};
struct CNVisionInspectResult_Cam3
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};
struct CNVisionInspectResult_Cam4
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};
struct CNVisionInspectResult_Cam5
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};
struct CNVisionInspectResult_Cam6
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};
struct CNVisionInspectResult_Cam7
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};
struct CNVisionInspectResult_Cam8
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
};

#pragma region Fake Camera
struct CNVisionInspectResult_CountPixel 
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	float m_fNumberOfPixel;
};
struct CNVisionInspectResult_Decode
{
public:
	BOOL m_bResultStatus;
	BOOL m_bInspectCompleted;
	TCHAR m_sResultString[MAX_STRING_SIZE_RESULT];
};
#pragma endregion

class AFX_EXT_CLASS CNVisionInspectResult
{
public:
	CNVisionInspectResult(void);
	~CNVisionInspectResult(void);
public:
	CNVisionInspectResult_Cam1 m_NVisionInspRes_Cam1;
	CNVisionInspectResult_Cam2 m_NVisionInspRes_Cam2;
	CNVisionInspectResult_Cam3 m_NVisionInspRes_Cam3;
	CNVisionInspectResult_Cam4 m_NVisionInspRes_Cam4;
	CNVisionInspectResult_Cam5 m_NVisionInspRes_Cam5;
	CNVisionInspectResult_Cam6 m_NVisionInspRes_Cam6;
	CNVisionInspectResult_Cam7 m_NVisionInspRes_Cam7;
	CNVisionInspectResult_Cam8 m_NVisionInspRes_Cam8;
};
class AFX_EXT_CLASS CNVisionInspectResult_FakeCam
{
public:
	CNVisionInspectResult_FakeCam(void);
	~CNVisionInspectResult_FakeCam(void);
public:
	CNVisionInspectResult_CountPixel m_NVisonInspectResCntPxl;
	CNVisionInspectResult_Decode m_NVisonInspectResDecode;
};