#include "D2Pathing.h"

#pragma comment(lib, "shlwapi.lib")

BOOL CMemory::LoadWindow(CHAR* Title, CHAR* Class)
{
	/*HWND hWnd = FindWindow(Title,Class);
	if(!hWnd)
		return FALSE;
	GetWindowThreadProcessId(hWnd,&WindowId);
	CMemory::WindowHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, CMemory::WindowId);
	if(!CMemory::WindowHandle)
		return FALSE;*/
	return TRUE;
}

BOOL CMemory::LoadWindow(DWORD ProcessId)
{
	WindowId = ProcessId;
	CMemory::WindowHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, CMemory::WindowId);
	if(!CMemory::WindowHandle)
		return FALSE;
	return TRUE;
}

BOOL CMemory::ReadMemory(DWORD Address, LPVOID Buffer, DWORD dwSize)
{
	DWORD ReadBytes;
	if(!CMemory::WindowHandle)
		return FALSE;
	ReadProcessMemory(CMemory::WindowHandle,(LPVOID)Address,Buffer,dwSize,&ReadBytes);
	return (dwSize == ReadBytes);
}

BOOL CMemory::WriteMemory(DWORD Address, LPVOID Buffer, DWORD dwSize)
{
	DWORD WroteBytes;
	if(!CMemory::WindowHandle)
		return FALSE;
	WriteProcessMemory(CMemory::WindowHandle,(LPVOID)Address,Buffer,dwSize,&WroteBytes);
	return (dwSize == WroteBytes);
}

DWORD CMemory::GetModule(CHAR* pModule)
{
	if(!CMemory::WindowHandle)
		return NULL;

	MODULEENTRY32 modEntry;
	HANDLE tlh = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, WindowId);

	modEntry.dwSize = sizeof(MODULEENTRY32);
    Module32First(tlh, &modEntry);

	do
	{
		if(!stricmp(modEntry.szModule, pModule))
			return (DWORD)modEntry.hModule;
		modEntry.dwSize = sizeof(MODULEENTRY32);
	}
	while(Module32Next(tlh, &modEntry));

	return NULL;
}