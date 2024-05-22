#pragma once


class AFX_EXT_CLASS CSealingInspectResult_TopCam
{
public:
	CSealingInspectResult_TopCam(void);
	~CSealingInspectResult_TopCam(void);

public:
	BOOL              m_bStatusFrame1;
	BOOL              m_bStatusFrame2;
	BOOL              m_bInspectComplete;
};