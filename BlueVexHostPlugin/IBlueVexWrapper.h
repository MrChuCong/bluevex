#pragma once

#include "PluginTypes.h"
#include <string>

#define DLLAPI  __declspec(dllexport)

class     IBlueVexWrapper
{
public:

	virtual void    InitPlugin(RedVexInfo* Funcs) = 0;
	virtual void    DestroyPlugin() = 0;
	virtual void    UpdatePlugin() = 0;
	virtual void    InitGameModule(IProxy* proxy, IModule* module) = 0;
	virtual void	OnRelayGameDataToClient(const unsigned char* bytes,int length,IPacket* packet) = 0;
	virtual void	OnRelayGameDataToServer(const unsigned char* bytes,int length,IPacket* packet) = 0;
	virtual void    InitChatModule(IProxy* proxy, IModule* module) = 0;
	virtual void	OnRelayChatDataToClient(const unsigned char* bytes,int length,IPacket* packet) = 0;
	virtual void	OnRelayChatDataToServer(const unsigned char* bytes,int length,IPacket* packet) = 0;
	virtual void    InitRealmModule(IProxy* proxy, IModule* module) = 0;
	virtual void	OnRelayRealmDataToClient(const unsigned char* bytes,int length,IPacket* packet) = 0;
	virtual void	OnRelayRealmDataToServer(const unsigned char* bytes,int length,IPacket* packet) = 0;

	static IBlueVexWrapper	*CreateInstance();
	static void					Destroy(IBlueVexWrapper *instance);
	static void					Update(IBlueVexWrapper *instance);
};

