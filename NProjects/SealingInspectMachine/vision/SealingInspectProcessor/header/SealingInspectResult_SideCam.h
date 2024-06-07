#pragma once


class AFX_EXT_CLASS CSealingInspectResult_SideCam
{
public:
	CSealingInspectResult_SideCam(void);
	~CSealingInspectResult_SideCam(void);

public:
	BOOL              m_bStatusFrame1;
	BOOL              m_bStatusFrame2;
	BOOL              m_bStatusFrame3;
	BOOL              m_bStatusFrame4;
	BOOL              m_bStatusFrame5;
	BOOL              m_bStatusFrame6;
	BOOL              m_bStatusFrame7;
	BOOL              m_bStatusFrame8;
	BOOL              m_bStatusFrame9;
	BOOL              m_bStatusFrame10;
	BOOL              m_bInspectComplete;
};