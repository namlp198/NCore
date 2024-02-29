// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"

#include <afx.h>
#include <afxmt.h>
#include <cstringt.h>
#include <afxwin.h>
#include <windef.h>
// Warning
#pragma warning (disable : 4819)
#pragma warning (disable : 4005)
#pragma warning (disable : 4996)

// Resource
#include "resource.h"

#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment(lib, "gdiplus")

// Include files to use the pylon API.
#include <pylon/PylonIncludes.h>

// Make the string converter from this project available globally.
#include "PylonStringHelpers.h"

#endif //PCH_H
