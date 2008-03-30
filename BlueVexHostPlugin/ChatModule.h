#pragma once
#include "IModule.h"
#include "IBlueVexWrapper.h"


class HostChatModule :
	public IModule
{
public:
	HostChatModule(IProxy* proxy);
	void __stdcall Destroy();
	void _stdcall OnRelayDataToServer(IPacket* packet, const IModule* owner);
	void _stdcall OnRelayDataToClient(IPacket* packet, const IModule* owner);
	void _stdcall Update();

private:
	IProxy* _proxy;
	IBlueVexWrapper *wrapper;
};
