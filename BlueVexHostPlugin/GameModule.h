#pragma once
#include "IModule.h"
#include "IBlueVexWrapper.h"


class HostGameModule :
	public IModule
{
public:
	HostGameModule(IProxy* proxy);
	void __stdcall Destroy();
	void __stdcall Update();
	void _stdcall OnRelayDataToServer(IPacket* packet, const IModule* owner);
	void _stdcall OnRelayDataToClient(IPacket* packet, const IModule* owner);

private:
	IProxy* _proxy;
	IBlueVexWrapper *wrapper;
};
