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

#include "Matrix.h"
#include "CMemory.h"

#ifndef COLLISIONMAP_H
#define COLLISIONMAP_H

#define MAP_DATA_INVALID	-1    // Invalid
#define MAP_DATA_CLEANED	11110 // Cleaned for start/end positions
#define MAP_DATA_FILLED		11111 // Filled gaps
#define MAP_DATA_THICKENED	11113 // Wall thickened
#define MAP_DATA_AVOID		11115 // Should be avoided

///////////////////////////////////////////////////
// Unit Types
///////////////////////////////////////////////////
#define UNIT_TYPE_PLAYER	0
#define UNIT_TYPE_NPC		1
#define UNIT_TYPE_OBJECT	2
#define UNIT_TYPE_MISSILE	3
#define UNIT_TYPE_ITEM		4
#define UNIT_TYPE_TILE		5

struct MapInfo_t
{
	DWORD MapSizeX;
	DWORD MapSizeY;
	DWORD MapPosX;
	DWORD MapPosY;
	DWORD LevelNo;
	DWORD Exit1ID;
	DWORD Exit1X;
	DWORD Exit1Y;
	DWORD Exit2ID;
	DWORD Exit2X;
	DWORD Exit2Y;
	DWORD Exit3ID;
	DWORD Exit3X;
	DWORD Exit3Y;
};

struct DLLInfo_t
{
	DWORD D2Client;
	DWORD D2Common;
};

struct PlayerInfo_t
{
	DWORD dwPlayerId;
	DWORD dwUnitAddr;
	DWORD dwActAddr;
	DWORD dwRoomAddr;
};

struct CollMap {
	DWORD dwPosGameX;				//0x00
	DWORD dwPosGameY;				//0x04
	DWORD dwSizeGameX;				//0x08
	DWORD dwSizeGameY;				//0x0C
	DWORD dwPosRoomX;				//0x10
	DWORD dwPosRoomY;				//0x14
	DWORD dwSizeRoomX;				//0x18
	DWORD dwSizeRoomY;				//0x1C
	DWORD MapStart;					//0x20
	DWORD MapEnd;					//0x22
};

typedef CMatrix<WORD, WORD> WordMatrix;

class CCollisionMap {

private:
	CMemory Memory;
	BOOL MemoryReady;

	DLLInfo_t DLLInfo;
	PlayerInfo_t PlayerInfo;
	
	WordMatrix m_map;
	
public:

	MapInfo_t MapInfo;
	
	BOOL GenerateMap(VOID);
	BOOL InitMemory(CHAR* Title, CHAR* Class);	// Will prepare the Memory Reader
	BOOL InitMemory(DWORD ProcessId);			// Same just with the ProcessId
	BOOL AddRoomData(DWORD LevelNo, DWORD x, DWORD y);
	BOOL RemoveRoomData(DWORD LevelNo, DWORD x, DWORD y);
	WORD GetCollision(DWORD x, DWORD y, BOOL Abs);
	BOOL GetMapSize(POINT* pPoint);
	BOOL DestroyMap(VOID);
	BOOL DumpMap(CHAR* Path);
	BOOL CopyMapData(WordMatrix& rBuffer);
	BOOL RelativeToAbs(POINT* pPoint);
	BOOL AbsToRelative(POINT* pPoint);
	BOOL Freeze(VOID);
	BOOL UnFreeze(VOID);
	BOOL MyPosition(POINT* pPoint);
	VOID MakeBlank(WordMatrix& rMatrix, POINT pos);
	BOOL ThickenWalls(WordMatrix& rMatrix, int nThickenBy);
	BOOL MapToAbsScreen(POINT *pPos);
};

#endif