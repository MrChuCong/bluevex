#include "IBlueVexWrapper.h"
#include "BlueVexWrapper.h"
#include "IPacket.h"
#include "IProxy.h"
#include "CollisionMap.h"
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
	delete instance;
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

System::IntPtr	BlueVexWrapper::GetMap(DWORD PID)
{
	//EnableDebugPriv();
	CCollisionMap cMap;
	cMap.InitMemory(PID);
	cMap.GenerateMap();

	CMatrix<WORD, WORD> buffer;
	cMap.CopyMapData(buffer);

	const int CX = buffer.GetCX();
	const int CY = buffer.GetCY();

	unsigned char* data = new unsigned char[CX*CY];
	int i = 0;

	for (int y = 0; y < CY; y++)
	{		
		for (int x = 0; x < CX; x++)
		{
			data[i] = buffer[x][y];
			i++;
		}
	}

	MapInfo* info = new MapInfo;
	info->X = cMap.MapInfo.MapPosX;
	info->Y = cMap.MapInfo.MapPosY;
	info->Width = CX;
	info->Height = CY;
	info->bytes = data;
	info->LevelNo = cMap.MapInfo.LevelNo;
	info->Exit1ID = cMap.MapInfo.Exit1ID;
	info->Exit1X = cMap.MapInfo.Exit1X;
	info->Exit1Y = cMap.MapInfo.Exit1Y;
	info->Exit2ID = cMap.MapInfo.Exit2ID;
	info->Exit2X = cMap.MapInfo.Exit2X;
	info->Exit2Y = cMap.MapInfo.Exit2Y;
	info->Exit3ID = cMap.MapInfo.Exit3ID;
	info->Exit3X = cMap.MapInfo.Exit3X;
	info->Exit3Y = cMap.MapInfo.Exit3Y;
	return System::IntPtr(info);
}

void	BlueVexWrapper::InitGameModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	Funcs->GetMap = &BlueVexWrapper::GetMap;
	_GameModule = gcnew BlueVex::GameModuleHost(System::IntPtr(Funcs));
}

void	BlueVexWrapper::InitChatModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	Funcs->GetMap = &BlueVexWrapper::GetMap;
	_ChatModule = gcnew BlueVex::ChatModuleHost(System::IntPtr(Funcs));
}

void	BlueVexWrapper::InitRealmModule(IProxy* proxy, IModule* module)
{
	FunctionInfo* Funcs = new FunctionInfo;
	Funcs->RelayToClient = &BlueVexWrapper::RelayToClient;
	Funcs->RelayToServer = &BlueVexWrapper::RelayToServer;
	Funcs->_proxy = proxy;
	Funcs->_module = module;
	Funcs->GetMap = &BlueVexWrapper::GetMap;
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