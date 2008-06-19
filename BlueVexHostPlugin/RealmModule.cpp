#include "RealmModule.h"
#include "IPacket.h"
#include <windows.h>

HostRealmModule::HostRealmModule(IProxy* proxy) :
	_proxy(proxy)
{
	wrapper = IBlueVexWrapper::CreateInstance();
	wrapper->InitRealmModule(proxy,this);
}

void __stdcall HostRealmModule::Destroy()
{
	IBlueVexWrapper::Destroy(wrapper);
	delete this;
}

void __stdcall HostRealmModule::Update()
{
	IBlueVexWrapper::Update(wrapper);
	delete this;
}

void HostRealmModule::OnRelayDataToServer(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayRealmDataToServer(bytes,packet->GetSize(),packet);

}

void HostRealmModule::OnRelayDataToClient(IPacket* packet, const IModule* owner)
{
	const unsigned char* bytes = static_cast<const unsigned char*>(packet->GetData());
	wrapper->OnRelayRealmDataToClient(bytes,packet->GetSize(),packet);
}