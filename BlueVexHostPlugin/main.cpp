#include <windows.h>
#include "GameModule.h"
#include "ChatModule.h"
#include "RealmModule.h"

//All plugins should have this string for security reasons.
extern const char PatchRegion[];
const char PatchRegion[] = "q9fvn4q2hb3456223434hs0sj3q5gfamkzc32vhsdpopdj028qhe";

#include "PluginTypes.h"
#include "IModule.h"
#include <string>

PluginInfo Info;

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
		Funcs->WriteLog("BlueVex Diablo II Proxy\nCore Beta 5.3 June 2008\n(c) 2008 by Pleh\n");

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