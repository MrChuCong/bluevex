#include "GameModule.h"
#include "IPacket.h"
#include <windows.h>

HostGameModule::HostGameModule(IProxy* proxy) :
	_proxy(proxy)
{
	wrapper = IBlueVexWrapper::CreateInstance();
	wrapper->InitGameModule(_proxy,this);
}

void __stdcall HostGameModule::Destroy()
{
	IBlueVexWrapper::Destroy(wrapper);
	delete this;
}

void HostGameModule::OnRelayDataToServer(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayGameDataToServer(bytes,packet->GetSize(),packet);
}

void HostGameModule::OnRelayDataToClient(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayGameDataToClient(bytes,packet->GetSize(),packet);
}

void HostGameModule::Update()
{
	IBlueVexWrapper::Update(wrapper);
}