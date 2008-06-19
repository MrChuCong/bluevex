#pragma once

#include <vcclr.h>
#include <string>
#include "IBlueVexWrapper.h"
#include "IPacket.h"

using namespace BlueVex;

class     BlueVexWrapper : IBlueVexWrapper
{
private:
	gcroot<Plugin ^>  _managedPlugin;
    gcroot<GameModuleHost ^>  _GameModule;
	gcroot<ChatModuleHost ^>  _ChatModule;
	gcroot<RealmModuleHost ^>  _RealmModule;
    
	IProxy* _proxy;
	IModule* _module;
	std::string test;

public:
    BlueVexWrapper() { }

	typedef void(__thiscall BlueVexWrapper::*RelayType)(const unsigned char* bytes, int length, IProxy* proxy, IModule* module);
	typedef System::IntPtr(__clrcall BlueVexWrapper::*GetMapType)(DWORD PID);
	struct FunctionInfo {
		RelayType RelayToClient;
		RelayType RelayToServer;
		IProxy* _proxy;
		IModule* _module;
	};

	typedef void(__thiscall BlueVexWrapper::*SetFlagType)(IPacket* packet,IPacket::PacketFlag flag);
	struct PacketFunctionInfo {
		SetFlagType SetFlag;
	};
    
    void    InitPlugin(RedVexInfo* Funcs);
	void    DestroyPlugin();
	void    UpdatePlugin();

	void    InitGameModule(IProxy* proxy, IModule* module);
	void	OnRelayGameDataToClient(const unsigned char* bytes,int length,IPacket* packet);
	void	OnRelayGameDataToServer(const unsigned char* bytes,int length,IPacket* packet);
	void    InitChatModule(IProxy* proxy, IModule* module);
	void	OnRelayChatDataToClient(const unsigned char* bytes,int length,IPacket* packet);
	void	OnRelayChatDataToServer(const unsigned char* bytes,int length,IPacket* packet);
	void    InitRealmModule(IProxy* proxy, IModule* module);
	void	OnRelayRealmDataToClient(const unsigned char* bytes,int length,IPacket* packet);
	void	OnRelayRealmDataToServer(const unsigned char* bytes,int length,IPacket* packet);

	void	RelayToClient(const unsigned char* bytes, int length, IProxy* proxy, IModule* module);
	void	RelayToServer(const unsigned char* bytes, int length, IProxy* proxy, IModule* module);
	void	SetFlag(IPacket* packet,IPacket::PacketFlag flag);
	
};