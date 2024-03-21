#pragma once

class AFX_EXT_CLASS CTempInspectStatus
{
public:
	CTempInspectStatus(void);
	~CTempInspectStatus(void);

public:
	// Getter
	BOOL               GetInspectRunning() { return m_bInspectRunning; }
	int		           GetInsertFrameIndex() { return m_nInsertFrameIndex; };

	// Setter
	void               SetInspectRunning(BOOL inspRunning) { m_bInspectRunning = inspRunning; }
	void               SetInsertFrameIndex(int nFrameIdx) { m_nInsertFrameIndex = nFrameIdx; }

	void	           AddInsertFrameIndex() { m_nInsertFrameIndex += 1; };

private:
	BOOL                                m_bInspectRunning;

	// Camera CallBack Count..
	int									m_nInsertFrameIndex;
};