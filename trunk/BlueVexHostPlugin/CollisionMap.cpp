///////////////////////////////////////////////////////////////////////////////////////
// CollisionMap.cpp
//
// Created by Sheppard
//
// Credits:
// Abin - For the CollisionMap example at D2HackIt, Matrix Class and his TeleportPath Class!
// Rhin - Original ExitLevel code
//
// Started 12 Oct 2007
// Last Update: 8 Nov 2007
///////////////////////////////////////////////////////////////////////////////////////

#include "D2Pathing.h"

BOOL CCollisionMap::GenerateMap(VOID)
{
	/* We need some Process informations before we can start reading Diablo's memory */
	if(!Memory.WindowHandle)
		return FALSE;

	if(m_map.IsCreated())
		m_map.Destroy();

	DWORD dwTemp;
	
	CCollisionMap::DLLInfo.D2Client = Memory.GetModule("D2Client.dll");
	CCollisionMap::DLLInfo.D2Common = Memory.GetModule("D2Common.dll");
	
	/* Player Structure Informations */
	if(!Memory.ReadMemory(CCollisionMap::DLLInfo.D2Client + 0x11C1E0,&CCollisionMap::PlayerInfo.dwUnitAddr,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(CCollisionMap::PlayerInfo.dwUnitAddr + 0x0C,&CCollisionMap::PlayerInfo.dwPlayerId,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(CCollisionMap::PlayerInfo.dwUnitAddr + 0x1C,&CCollisionMap::PlayerInfo.dwActAddr,sizeof(DWORD)))
		return FALSE;

	if(!Memory.ReadMemory(CCollisionMap::PlayerInfo.dwUnitAddr + 0x2C,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x1C, &CCollisionMap::PlayerInfo.dwRoomAddr,sizeof(DWORD)))
		return FALSE;
	
	/* Map Information*/
	if(!Memory.ReadMemory(CCollisionMap::PlayerInfo.dwUnitAddr + 0x2C,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x1C,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x38,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x1C,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x04,&CCollisionMap::MapInfo.MapPosX,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x08,&CCollisionMap::MapInfo.MapPosY,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x0C,&CCollisionMap::MapInfo.MapSizeX,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x10,&CCollisionMap::MapInfo.MapSizeY,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x14,&CCollisionMap::MapInfo.LevelNo,sizeof(DWORD)))
		return FALSE;
	
	if(!CCollisionMap::m_map.Create(CCollisionMap::MapInfo.MapSizeX*5,CCollisionMap::MapInfo.MapSizeY*5,MAP_DATA_INVALID))
		return FALSE;

	/* Preparing to loop throught all Rooms2 */
	if(!Memory.ReadMemory(dwTemp + 0x204,&dwTemp,sizeof(DWORD)))
		return FALSE;

	CCollisionMap::MapInfo.Exit1ID = 0;
	CCollisionMap::MapInfo.Exit1X = 0;
	CCollisionMap::MapInfo.Exit1Y = 0;
	CCollisionMap::MapInfo.Exit2ID = 0;
	CCollisionMap::MapInfo.Exit2X = 0;
	CCollisionMap::MapInfo.Exit2Y = 0;
	CCollisionMap::MapInfo.Exit3ID = 0;
	CCollisionMap::MapInfo.Exit3X = 0;
	CCollisionMap::MapInfo.Exit3Y = 0;

	int CurrentExit = 1;
	System::Collections::ArrayList^ FoundExits = gcnew System::Collections::ArrayList();

	while(dwTemp)
	{		
		int Exit = 0;
		DWORD dwRoom = NULL;
		DWORD MapX;
		DWORD MapY;

		BOOL AddedRoom = FALSE;

		if(!Memory.ReadMemory(dwTemp + 0xE8,&dwRoom,sizeof(DWORD)))
			return FALSE;
		if(!Memory.ReadMemory(dwTemp + 0x20,&MapX,sizeof(DWORD)))
			return FALSE;
		if(!Memory.ReadMemory(dwTemp + 0x24,&MapY,sizeof(DWORD)))
			return FALSE;	
		
		if(!dwRoom) {
			AddedRoom = TRUE;
			AddRoomData(MapInfo.LevelNo,MapX,MapY);
			if(!Memory.ReadMemory(dwTemp + 0xE8,&dwRoom,sizeof(DWORD)))
				return FALSE;
		}



//////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////

		DWORD dwRoomsNear;
		DWORD pRoom2Near;

		if(!Memory.ReadMemory(dwTemp + 0x10,&dwRoomsNear,sizeof(DWORD)))
			return FALSE;

		if(!Memory.ReadMemory(dwTemp + 0x30,&pRoom2Near,sizeof(DWORD)))
			return FALSE;
		
		System::String^ mystring = "";

		for (DWORD i = 0; i < dwRoomsNear; i++)
		{
			DWORD pNear;
			DWORD pLevel;
			DWORD dwLevelNo;
			DWORD dwPosX;
			DWORD dwPosY;
			DWORD dwSizeX;
			DWORD dwSizeY;

			if(!Memory.ReadMemory(pRoom2Near+(i*sizeof(DWORD)),&pNear,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pNear + 0x1C,&pLevel,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pLevel + 0x14,&dwLevelNo,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pLevel + 0x04,&dwPosX,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pLevel + 0x08,&dwPosY,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pLevel + 0x0C,&dwSizeX,sizeof(DWORD)))
				return FALSE;

			if(!Memory.ReadMemory(pLevel + 0x10,&dwSizeY,sizeof(DWORD)))
				return FALSE;

			//std::pair<POINT, POINT> newlevel;

			//mystring = mystring + int(dwLevelNo) + " " + (int(dwPosX)-int(MapX)) + "," + (int(dwPosY)-int(MapY)) + "    ";
			
			//if(dwLevelNo == 8){
				//System::Diagnostics::Debug::WriteLine(int(dwLevelNo) + " " + int(dwPosX) + " " + int(dwPosY));
			//}

			if(dwLevelNo != MapInfo.LevelNo && !FoundExits->Contains(int(dwLevelNo))){
				Exit = dwLevelNo;
				FoundExits->Add(int(dwLevelNo));
			}

			//newlevel.first.x = dwPosX * 5;
			//newlevel.first.y = dwPosY * 5;
			//newlevel.second.x = dwSizeX * 5;
			//newlevel.second.y = dwSizeY * 5;

			//maps[MapInfo.LevelNo].levelsnear[dwLevelNo].push_back(newlevel);
		}

		//System::Diagnostics::Debug::WriteLine(mystring);


//////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////

		/*DWORD pUnit = NULL;
		
		if(!Memory.ReadMemory(dwTemp + 0x34,&pUnit,sizeof(DWORD)))
			return FALSE;

		while (pUnit)
		{
			DWORD dwType;
			DWORD dwClassId;
			DWORD dwPosX;
			DWORD dwPosY;

			if(!Memory.ReadMemory(pUnit + 0x1C,&dwType,sizeof(DWORD)))
				return FALSE;
			if(!Memory.ReadMemory(pUnit + 0x0C,&dwClassId,sizeof(DWORD)))
				return FALSE;
			if(!Memory.ReadMemory(pUnit + 0x18,&dwPosX,sizeof(DWORD)))
				return FALSE;
			if(!Memory.ReadMemory(pUnit + 0x08,&dwPosY,sizeof(DWORD)))
				return FALSE;

			if (dwType == UNIT_TYPE_NPC)
			{
				//maps[MapInfo.LevelNo].npcs[dwClassId].push_back(std::pair<short, short>(MapX*5+dwPosX, MapY*5+dwPosY));
			}

			if (dwType == UNIT_TYPE_OBJECT)
			{
				//maps[MapInfo.LevelNo].objects[dwClassId].push_back(std::pair<short, short>(MapX*5+dwPosX, MapY*5+dwPosY));
			}

			if (dwType == UNIT_TYPE_TILE)
			{
				DWORD pRoomTile = NULL;

				if(!Memory.ReadMemory(dwTemp + 0x00,&pRoomTile,sizeof(DWORD)))
					return FALSE;
				
				while (pRoomTile)
				{
					DWORD pNum = NULL;
					DWORD nNum = NULL;
					
					if(!Memory.ReadMemory(pRoomTile + 0x0C,&pNum,sizeof(DWORD)))
						return FALSE;
					if(!Memory.ReadMemory(pNum,&nNum,sizeof(DWORD)))
						return FALSE;

					if (nNum == dwClassId)
					{
						DWORD pRoom2;
						DWORD pLevel;
						DWORD dwLevelNo;
	
						if(!Memory.ReadMemory(pRoomTile + 0x04,&pRoom2,sizeof(DWORD)))
							return FALSE;

						if(!Memory.ReadMemory(pRoom2 + 0x1C,&pLevel,sizeof(DWORD)))
							return FALSE;

						if(!Memory.ReadMemory(pLevel + 0x14,&dwLevelNo,sizeof(DWORD)))
							return FALSE;

						if(CurrentExit == 1){
							CCollisionMap::MapInfo.Exit1ID = dwLevelNo;
							CCollisionMap::MapInfo.Exit1X = dwPosX;
							CCollisionMap::MapInfo.Exit1Y = dwPosY;
						}

						if(CurrentExit == 2){
							CCollisionMap::MapInfo.Exit2ID = dwLevelNo;
							CCollisionMap::MapInfo.Exit2X = dwPosX;
							CCollisionMap::MapInfo.Exit2Y = dwPosY;
						}

						if(CurrentExit == 3){
							CCollisionMap::MapInfo.Exit3ID = dwLevelNo;
							CCollisionMap::MapInfo.Exit3X = dwPosX;
							CCollisionMap::MapInfo.Exit3Y = dwPosY;
						}

						CurrentExit += 1;

						//maps[MapInfo.LevelNo].exits[dwLevelNo].first = MapX*5+dwPosX;
						//maps[MapInfo.LevelNo].exits[dwLevelNo].second = MapY*5+dwPosY;
					}


					if(!Memory.ReadMemory(pRoomTile + 0x08,&pRoomTile,sizeof(DWORD)))
						return FALSE;

				}
			}

			if(!Memory.ReadMemory(pUnit + 0x14,&pUnit,sizeof(DWORD)))
				return FALSE;

		}

//////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////
		*/

		if(!Memory.ReadMemory(dwRoom + 0x60,&dwRoom,sizeof(DWORD)))
			return FALSE;
		
		CollMap* pCol = new CollMap;
		
		if(!Memory.ReadMemory(dwRoom,pCol,sizeof(CollMap)))
			return FALSE;
		
		/* Building CollisionMap here */

		int x = pCol->dwPosGameX - MapInfo.MapPosX*5;
		int y = pCol->dwPosGameY - MapInfo.MapPosY*5;
		int cx = pCol->dwSizeGameX;
		int cy = pCol->dwSizeGameY;

		System::Diagnostics::Debug::WriteLine(int(x) + " " + int(y));
		if(Exit != 0){

						if(CurrentExit == 1){
							CCollisionMap::MapInfo.Exit1ID = Exit;
							CCollisionMap::MapInfo.Exit1X = x+20;
							CCollisionMap::MapInfo.Exit1Y = y+20;
						}

						if(CurrentExit == 2){
							CCollisionMap::MapInfo.Exit2ID = Exit;
							CCollisionMap::MapInfo.Exit2X = x+20;
							CCollisionMap::MapInfo.Exit2Y = y+20;
						}

						if(CurrentExit == 3){
							CCollisionMap::MapInfo.Exit3ID = Exit;
							CCollisionMap::MapInfo.Exit3X = x+20;
							CCollisionMap::MapInfo.Exit3Y = y+20;
						}

			CurrentExit += 1;
		}

		

		WORD p = NULL;
		DWORD pAddr;

		if(!Memory.ReadMemory(dwRoom + 0x20,&pAddr,sizeof(DWORD)))
			return FALSE;
		
		if(!Memory.ReadMemory(pAddr,&p,sizeof(WORD)))
			return FALSE;
		pAddr+=sizeof(WORD);

		int nLimitX = x + cx;
		int nLimitY = y + cy;		
		if(m_map.IsValidIndex(x,y))
		{
			for (int j = y; j < nLimitY; j++)		
			{
				for (int i = x; i < nLimitX; i++)
				{

					m_map[i][j] = p;
						if (m_map[i][j] == 1024)
							m_map[i][j] = MAP_DATA_AVOID;
					if(!Memory.ReadMemory(pAddr,&p,sizeof(WORD)))
						return FALSE;	
					pAddr+=sizeof(WORD);
				}
			}
		}

		if(AddedRoom)
			RemoveRoomData(MapInfo.LevelNo,MapX,MapY);

		if(!Memory.ReadMemory(dwTemp + 0x38,&dwTemp,sizeof(DWORD)))
			return FALSE;
	}
	return TRUE;
}

BOOL CCollisionMap::ThickenWalls(WordMatrix& rMatrix, int nThickenBy)
{
	if (!rMatrix.IsCreated() || nThickenBy <= 0)
		return FALSE;

	const int CX = rMatrix.GetCX();
	const int CY = rMatrix.GetCY();
	
	for (int i = 0; i < CX; i++)
	{
		for (int j = 0; j < CY; j++)
		{
			if ((rMatrix[i][j] % 2) == 0 || rMatrix[i][j] == MAP_DATA_THICKENED)
				continue;

			for (int x = i - nThickenBy; x <= i + nThickenBy; x++)
			{
				for (int y = j - nThickenBy; y <= j + nThickenBy; y++)
				{
					if (!rMatrix.IsValidIndex(x, y))
						continue;

					if ((rMatrix[x][y] % 2) == 0)
						rMatrix[x][y] = MAP_DATA_THICKENED;
				}
			}
		}
	}

	return TRUE;
}

VOID CCollisionMap::MakeBlank(WordMatrix& rMatrix, POINT pos)
{
	if (!rMatrix.IsCreated())
		return;

	for (int i = pos.x - 1; i <= pos.x + 1; i++)
	{
		for (int j = pos.y - 1; j <= pos.y + 1; j++)
		{
			if (rMatrix.IsValidIndex(i, j))
				rMatrix[i][j] = MAP_DATA_CLEANED;
		}
	}
}


BOOL CCollisionMap::MyPosition(POINT* pPoint)
{
	if(!Memory.WindowHandle)
		return FALSE;

	if(!CCollisionMap::DLLInfo.D2Client)
		CCollisionMap::DLLInfo.D2Client = Memory.GetModule("D2Client.dll");

	DWORD dwTemp;

	if(!Memory.ReadMemory(CCollisionMap::DLLInfo.D2Client + 0x11C1E0,&CCollisionMap::PlayerInfo.dwUnitAddr,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(CCollisionMap::PlayerInfo.dwUnitAddr + 0x2C,&dwTemp,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x02,&pPoint->x,sizeof(WORD)))
		return FALSE;
	if(!Memory.ReadMemory(dwTemp + 0x06,&pPoint->y,sizeof(WORD)))
		return FALSE;

	return TRUE;
}

/*
Original Diablo II MapToAbsScreen function

6FDAA830 >/$ 8B5424 04      MOV EDX,DWORD PTR SS:[ESP+4]
6FDAA834  |. 8B02           MOV EAX,DWORD PTR DS:[EDX]               ;  EAX = MapX Coord
6FDAA836  |. 56             PUSH ESI
6FDAA837  |. 8B7424 0C      MOV ESI,DWORD PTR SS:[ESP+C]
6FDAA83B  |. 8B0E           MOV ECX,DWORD PTR DS:[ESI]               ;  ECX = MapY Coord
6FDAA83D  |. 57             PUSH EDI
6FDAA83E  |. 8BF8           MOV EDI,EAX
6FDAA840  |. 2BF9           SUB EDI,ECX                              ;  MapX - MapY
6FDAA842  |. C1E7 04        SHL EDI,4                                ;  (MapX - MapY) << 4
6FDAA845  |. 03C8           ADD ECX,EAX                              ;  MapY + MapX
6FDAA847  |. 893A           MOV DWORD PTR DS:[EDX],EDI               ;  Done, moving the calculated X position to stack
6FDAA849  |. C1E1 03        SHL ECX,3                                ;  (MapY + MapX) << 3
6FDAA84C  |. 5F             POP EDI
6FDAA84D  |. 890E           MOV DWORD PTR DS:[ESI],ECX               ;  Done, moving the calculated Y position to stack
6FDAA84F  |. 5E             POP ESI
6FDAA850  \. C2 0800        RETN 8	
*/

BOOL CCollisionMap::MapToAbsScreen(POINT *pPos)
{
	if(!m_map.IsCreated())
		return FALSE;
	if(!Memory.WindowHandle)
		return FALSE;

	DWORD dwTemp;
	DWORD MouseXOffset = NULL, MouseYOffset = NULL;

	if(!Memory.ReadMemory(DLLInfo.D2Client + 0x11B678,&MouseXOffset,sizeof(DWORD)))
		return FALSE;
	if(!Memory.ReadMemory(DLLInfo.D2Client + 0x11C3E8,&dwTemp,sizeof(DWORD)))
		return FALSE;

	MouseXOffset -= dwTemp;

	if(!Memory.ReadMemory(DLLInfo.D2Client + 0x11B674,&MouseYOffset,sizeof(DWORD)))
		return FALSE;

	DWORD newX = (pPos->x - pPos->y) << 4;
	DWORD newY = (pPos->y + pPos->x) << 3;

	pPos->x = newX - MouseXOffset;
	pPos->y = newY - MouseYOffset;
	return TRUE;
}

BOOL CCollisionMap::Freeze(VOID)
{
	if(!Memory.WindowHandle)
		return FALSE;
	SuspendThread(Memory.WindowHandle);
	return TRUE;
}

BOOL CCollisionMap::UnFreeze(VOID)
{
	if(!Memory.WindowHandle)
		return FALSE;
	ResumeThread(Memory.WindowHandle);
	return TRUE;
}

BOOL CCollisionMap::DumpMap(CHAR* Path)
{
	if(!m_map.IsCreated())
		return FALSE;

	FILE *fp = fopen(Path, "w+");
	if(fp == NULL )
		return FALSE;

	POINT PlayerPos = {0,0};

	if(!CCollisionMap::MyPosition(&PlayerPos)) {
		fclose(fp);
		return FALSE;
	}

	if(!CCollisionMap::AbsToRelative(&PlayerPos))
	{
		fclose(fp);
		return FALSE;
	}

	fprintf(fp, "LevelNo: %d (Size: %d * %d)\n ",MapInfo.LevelNo, m_map.GetCX(), m_map.GetCY());
	fprintf(fp, "\n\n");


	const int CX = m_map.GetCX();
	const int CY = m_map.GetCY();

	for (int y = 0; y < CY; y++)
	{		
		for (int x = 0; x < CX; x++)
		{
			CHAR ch;
			if(PlayerPos.x == x && PlayerPos.y == y)
				ch = 'P';
			else
				ch = (m_map[x][y] % 2) ? 'X' : ' ';
			fprintf(fp, "%c", ch); // X - unreachable
		}

		fprintf(fp, "%c", '\n');
	}
	
	fclose(fp);

	return TRUE;
}

BOOL CCollisionMap::CopyMapData(WordMatrix& rBuffer)
{
	m_map.ExportData(rBuffer);
	return rBuffer.IsCreated();
}

BOOL CCollisionMap::AddRoomData(DWORD LevelNo, DWORD x, DWORD y)
{
	BYTE AsmStub[] = {
	0x68, 00,00,00,00, // Pushing the pAct to the Stack
	0x68, 00,00,00,00, // Pushing the LevelId to the Stack
	0x68, 00,00,00,00, // Pushing the xPos to the Stack
	0x68, 00,00,00,00, // Pushing the yPos to the Stack
	0x68, 00,00,00,00, // Pushing the pRoom to the Stack
	0xB8, 00,00,00,00, // MOV EAX, 0x00
	0xFF, 0xD0,		   // Call Eax
	0xC3,
	};

	//EXPTYPE(D2COMMON, AddRoomData,void,__stdcall,(Act * ptAct, int LevelId, int Xpos, int Ypos, Room1* pRoom))

	*(DWORD*)&AsmStub[1] = PlayerInfo.dwRoomAddr;
	*(DWORD*)&AsmStub[6] = y;
	*(DWORD*)&AsmStub[11] = x;
	*(DWORD*)&AsmStub[16] = LevelNo;
	*(DWORD*)&AsmStub[21] = PlayerInfo.dwActAddr;
	*(DWORD*)&AsmStub[26] = CCollisionMap::DLLInfo.D2Common + 0x26690;

	LPVOID Address = VirtualAllocEx(Memory.WindowHandle, NULL, sizeof(AsmStub), MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	Memory.WriteMemory((DWORD)Address,AsmStub,sizeof(AsmStub));
	HANDLE pHandle = CreateRemoteThread(Memory.WindowHandle,NULL,sizeof(DWORD),(LPTHREAD_START_ROUTINE)Address,NULL,NULL,NULL);
	WaitForSingleObject(pHandle,INFINITE);
	CloseHandle(pHandle);
	VirtualFreeEx(Memory.WindowHandle,(LPVOID)Address,sizeof(AsmStub),MEM_RELEASE);

	return TRUE;
}

BOOL CCollisionMap::RemoveRoomData(DWORD LevelNo, DWORD x, DWORD y)
{
	BYTE AsmStub[] = {
	0x68, 00,00,00,00, // Pushing the pAct to the Stack
	0x68, 00,00,00,00, // Pushing the LevelId to the Stack
	0x68, 00,00,00,00, // Pushing the xPos to the Stack
	0x68, 00,00,00,00, // Pushing the yPos to the Stack
	0x68, 00,00,00,00, // Pushing the pRoom to the Stack
	0xB8, 00,00,00,00, // MOV EAX, 0x00
	0xFF, 0xD0,		   // Call Eax
	0xC3,
	};

	*(DWORD*)&AsmStub[1] = PlayerInfo.dwRoomAddr;
	*(DWORD*)&AsmStub[6] = y;
	*(DWORD*)&AsmStub[11] = x;
	*(DWORD*)&AsmStub[16] = LevelNo;
	*(DWORD*)&AsmStub[21] = PlayerInfo.dwActAddr;
	*(DWORD*)&AsmStub[26] = CCollisionMap::DLLInfo.D2Common + 0x26550;

	LPVOID Address = VirtualAllocEx(Memory.WindowHandle, NULL, sizeof(AsmStub), MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	Memory.WriteMemory((DWORD)Address,AsmStub,sizeof(AsmStub));
	HANDLE pHandle = CreateRemoteThread(Memory.WindowHandle,NULL,sizeof(DWORD),(LPTHREAD_START_ROUTINE)Address,NULL,NULL,NULL);
	WaitForSingleObject(pHandle,INFINITE);
	CloseHandle(pHandle);
	VirtualFreeEx(Memory.WindowHandle,(LPVOID)Address,sizeof(AsmStub),MEM_RELEASE);

	return TRUE;
}

BOOL CCollisionMap::InitMemory(CHAR* Title, CHAR* Class)
{
	return Memory.LoadWindow(Title,Class);
}

BOOL CCollisionMap::InitMemory(DWORD ProcessId)
{
	return Memory.LoadWindow(ProcessId);
}

BOOL CCollisionMap::DestroyMap(VOID)
{
	if (!m_map.IsCreated())
		return FALSE;
	m_map.Destroy();
	return TRUE;
}

BOOL CCollisionMap::RelativeToAbs(POINT* pPoint)
{
	if (!m_map.IsCreated())
		return FALSE;
	pPoint->x += MapInfo.MapPosX * 5;
	pPoint->y += MapInfo.MapPosY * 5;
	return TRUE;
}

BOOL CCollisionMap::AbsToRelative(POINT* pPoint)
{
	if (!m_map.IsCreated())
		return FALSE;
	pPoint->x -= MapInfo.MapPosX * 5;
	pPoint->y -= MapInfo.MapPosY * 5;
	return TRUE;
}

WORD CCollisionMap::GetCollision(DWORD x, DWORD y, BOOL Abs)
{
	if (!m_map.IsCreated())
		return MAP_DATA_INVALID;
	if(Abs)
	{
		x -= MapInfo.MapPosX;
		y -= MapInfo.MapPosY;
	}
	return m_map[x][y];
}

BOOL CCollisionMap::GetMapSize(POINT* pPoint)
{
	if (!m_map.IsCreated())
		return FALSE;
	pPoint->x = m_map.GetCX();
	pPoint->y = m_map.GetCY();
	return TRUE;
}