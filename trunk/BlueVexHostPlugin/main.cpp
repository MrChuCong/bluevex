#include <windows.h>
#include "GameModule.h"
#include "ChatModule.h"
#include "RealmModule.h"

//All plugins should have this string for security reasons.
extern const char PatchRegion[];
const char PatchRegion[] = "q9fvn4q2hb3456223434hs0sj3q5gfamkzc32vhsdpopdj028qhe";

#include "PluginTypes.h"
#include "IModule.h"
#include "IBlueVexWrapper.h"
#include <string>

PluginInfo Info;

BOOL EnableDebugPriv(VOID) {
   HANDLE hToken;
   LUID sedebugnameValue;
   TOKEN_PRIVILEGES tkp;
   if(OpenProcessToken(GetCurrentProcess(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken )) {
      if(LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &sedebugnameValue )) {
         tkp.PrivilegeCount=1;
         tkp.Privileges[0].Luid = sedebugnameValue;
         tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
         if(AdjustTokenPrivileges(hToken, FALSE, &tkp, sizeof tkp, NULL, NULL )) {
            CloseHandle(hToken);
            return TRUE;
         }
         printf("AdjustTokenPrivileges failed (%d). SeDebugPrivilege is not available.\n", GetLastError());
      } else
         printf("LookupPrivilegeValue failed (%d). SeDebugPrivilege is not available.\n", GetLastError());
   } else
      printf("OpenProcessToken failed (%d). SeDebugPrivilege is not available.\n", GetLastError());
   CloseHandle(hToken);
   return FALSE;
} 

#pragma unmanaged
int __stdcall DllMain(HINSTANCE instance, int reason, void* reserved)
{
	//EnableDebugPriv();
	return 1;
}

void __stdcall FreePlugin(PluginInfo* Info)
{
   //Cleanup Stuff
}

IModule* __stdcall CreateModule(IProxy* proxy, ModuleKind Kind)
{

switch(Kind)
	{
	case GameModule: return new HostGameModule(proxy); break;
	case ChatModule: return new HostChatModule(proxy); break;
	case RealmModule: return new HostRealmModule(proxy); break;
	}

return 0;
}

extern "C"
{
	__declspec(dllexport) PluginInfo* __stdcall InitPlugin(RedVexInfo* Funcs)
	{
		Funcs->WriteLog("BlueVex Diablo II Proxy\nCore Version 1.0 Jan 10 2008\n(c) 2008 by Pleh\n");

		IBlueVexWrapper *wrapper = IBlueVexWrapper::CreateInstance();
		wrapper->InitPlugin(Funcs);
		IBlueVexWrapper::Destroy(wrapper);

		Info.Name = "BlueVex Host Plugin";
		Info.Author = "Pleh";
		Info.SDKVersion = 3;
		Info.Destroy = (DestroyPlugin)&FreePlugin;
		Info.Create = &CreateModule;
		return &Info;
	}
}