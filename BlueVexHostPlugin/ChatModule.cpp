#include "ChatModule.h"
#include "IPacket.h"
#include <windows.h>

HostChatModule::HostChatModule(IProxy* proxy) :
	_proxy(proxy)
{
	wrapper = IBlueVexWrapper::CreateInstance();
	wrapper->InitChatModule(proxy,this);
}

void __stdcall HostChatModule::Destroy()
{
	IBlueVexWrapper::Destroy(wrapper);
	delete this;
}

void HostChatModule::OnRelayDataToServer(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayChatDataToServer(bytes,packet->GetSize(),packet);

}

void HostChatModule::OnRelayDataToClient(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayChatDataToClient(bytes,packet->GetSize(),packet);
}

void HostChatModule::Update()
{

}