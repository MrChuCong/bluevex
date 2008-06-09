#include "IBlueVexWrapper.h"
#include "BlueVexWrapper.h"
#include "IPacket.h"
#include "IProxy.h"
#include <windows.h>
#include <string>

void	BlueVexWrapper::InitPlugin(RedVexInfo* Funcs)
{
	_managedPlugin = gcnew Plugin();
	_managedPlugin->InitPlugin(System::IntPtr(Funcs));
}

IBlueVexWrapper	*IBlueVexWrapper::CreateInstance()
{
	return ((IBlueVexWrapper *)new BlueVexWrapper());
}

void	IBlueVexWrapper::Destroy(IBlueVexWrapper *instance)
{
	instance->DestroyPlugin();
	delete instance;
}

void	BlueVexWrapper::DestroyPlugin()
{
	if(_GameModule)_GameModule->Destroy();
	if(_ChatModule)_ChatModule->Destroy();
	if(_RealmModule)_RealmModule->Destroy();
	_managedPlugin->DestroyPlugin();
}

void	IBlueVexWrapper::Update(IBlueVexWrapper *instance)
{
	instance->UpdatePlugin();
}

void	BlueVexWrapper::UpdatePlugin()
{
	if(_GameModule)_GameModule->Update();
	if(_ChatModule)_ChatModule->Update();
	if(_RealmModule)_RealmModule->Update();
	_managedPlugin->UpdatePlugin();
}

void	BlueVexWrapper::RelayToClient(const unsigned char* bytes, int length, IProxy* proxy, IModule* module) //System::IntPtr _proxy, System::IntPtr _module, 
{
	IPacket* packet = proxy->CreatePacket(bytes, length);
    packet->SetFlag(IPacket::PacketFlag_Hidden);
	proxy->RelayDataToClient(packet,module);
	delete packet;
}

void	BlueVexWrapper::RelayToServer(const unsigned char* bytes, int length, IProxy* proxy, IModule* module)
{
	IPacket* packet = proxy->CreatePacket(bytes, length);
    packet->SetFlag(IPacket::PacketFlag_Hidden);
	proxy->RelayDataToServer(packet,module);
	delete packet;
}

void	BlueVexWrapper::SetFlag(IPacket* packet,IPacket::PacketFlag flag)
{
	packet->SetFlag(flag);
}

void	BlueVexWrapper::InitGameModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	_GameModule = gcnew BlueVex::GameModuleHost(System::IntPtr(Funcs));
}

void	BlueVexWrapper::InitChatModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	_ChatModule = gcnew BlueVex::ChatModuleHost(System::IntPtr(Funcs));
}

void	BlueVexWrapper::InitRealmModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	_RealmModule = gcnew BlueVex::RealmModuleHost(System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayGameDataToClient(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_GameModule->OnRelayDataToClient(data,System::IntPtr(packet),System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayGameDataToServer(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_GameModule->OnRelayDataToServer(data,System::IntPtr(packet),System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayChatDataToClient(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_ChatModule->OnRelayDataToClient(data,System::IntPtr(packet),System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayChatDataToServer(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_ChatModule->OnRelayDataToServer(data,System::IntPtr(packet),System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayRealmDataToClient(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_RealmModule->OnRelayDataToClient(data,System::IntPtr(packet),System::IntPtr(Funcs));
}

void	BlueVexWrapper::OnRelayRealmDataToServer(const unsigned char* bytes,int length,IPacket* packet)
{
	array<unsigned char,1>^ data = gcnew array<unsigned char,1>(length);
	int i;
	for (i=0; i<length; i++) data[i] = bytes[i];
	PacketFunctionInfo* Funcs = new PacketFunctionInfo;
	Funcs->SetFlag = &BlueVexWrapper::SetFlag;
	_RealmModule->OnRelayDataToServer(data,System::IntPtr(packet),System::IntPtr(Funcs));
}