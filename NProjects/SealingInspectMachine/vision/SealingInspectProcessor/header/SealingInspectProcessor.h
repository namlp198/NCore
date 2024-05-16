#pragma once
#include "SealingInspectCore_TopCam.h"
#include "SealingInspectCore_SideCam.h"
#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectResult.h"
#include "SealingInspectSystemSetting.h"

class AFX_EXT_CLASS CSealingInspectProcessor 
{
public:
	CSealingInspectProcessor();
	~CSealingInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadSystemSetting();
	BOOL LoadRecipe();
	BOOL CreateBuffer();
};