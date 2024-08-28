#pragma once

class AFX_EXT_CLASS CNVisionInspectStatus
{
public:
	CNVisionInspectStatus(void);
	~CNVisionInspectStatus(void);

public:
	void	SetSimulation(BOOL bSimulation) { m_bSimulation = bSimulation; };
	BOOL	GetSimulation() { return m_bSimulation; };
	void	SetStreaming(BOOL bStreaming) { m_bStreaming = bStreaming; };
	BOOL	GetStreaming() { return m_bStreaming; };
	void	SetInspectRunning(BOOL bRunning) { m_bInspectRunning = bRunning; };
	BOOL	GetInspectRunning() { return m_bInspectRunning; };

private:
	BOOL m_bSimulation;
	BOOL m_bStreaming;
	BOOL m_bInspectRunning;
	int m_nCurrentFrame;
};