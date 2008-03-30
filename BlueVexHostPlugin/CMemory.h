#ifndef CMEMORY_H
#define CMEMORY_H

class CMemory
{
private:

public:
	BOOL LoadWindow(CHAR* Title, CHAR* Class);
	BOOL LoadWindow(DWORD ProcessId);
	BOOL ReadMemory(DWORD Address, LPVOID Buffer, DWORD dwSize);
	BOOL WriteMemory(DWORD Address, LPVOID Buffer, DWORD dwSize);
	DWORD GetModule(CHAR* pModule);

	HANDLE WindowHandle;
	DWORD WindowId;
};

#endif