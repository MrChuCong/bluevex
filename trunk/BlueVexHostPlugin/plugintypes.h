#pragma once

#include <windows.h>
#include "IModule.h"

typedef void(__stdcall *DestroyPlugin)(void* Info);
typedef IModule*(__stdcall *ModuleCreator)(IProxy* proxy, ModuleKind Kind);

struct PluginInfo {
	  const char* Name;
	  const char* Author;
	  int SDKVersion;
	  DestroyPlugin Destroy;
	  ModuleCreator Create;
	};

typedef void(__stdcall *WriteLogType)(char* Text);
typedef HWND*(__stdcall *GetWindowHandleType)();

struct RedVexInfo {
	  WriteLogType WriteLog;
	  GetWindowHandleType GetWindowHandle;
	};

