Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Specialized
Imports MEC

Public Class Pathing
#Region "Structures"
    Private Structure CollMap_t

        Dim dwPosGameX As Int32 '0x00 0
        Dim dwPosGameY As Int32 '0x04 4

        Dim dwSizeGameX As Int32 '0x08 8
        Dim dwSizeGameY As Int32 '0x0C 12

        Dim dwPosRoomX As Int32 '0x10 16
        Dim dwPosRoomY As Int32 '0x14 20

        Dim dwSizeRoomX As Int32 '0x18 24
        Dim dwSizeRoomY As Int32 '0x1C 28
        Dim MapStart As Int32 '0x20    32
        Dim MapEnd As Int32 '0x22      34
    End Structure
    Private Structure Playerinfo_t
        Dim dwPlayerId As Int32
        Dim dwUnitAddr As Int32
        Dim dwActAddr As Int32
        Dim dwRoomAddr As Int32
    End Structure
    Public Structure MapInfo_t
        Public LevelsNear As OrderedDictionary(Of Long, Exit_T)
        Public Npcs As OrderedDictionary(Of Long, Point)
        Public Objects As OrderedDictionary(Of Long, Point)
        Public Exits As OrderedDictionary(Of Long, Point)
        Public MapPosX As Integer
        Public MapPosY As Integer
        Public MapSizeX As Integer
        Public MapSizeY As Integer
        Public LevelNo As Integer
        Public Bytes(,) As Integer
    End Structure
    Public Structure Exit_T
        Dim First As Point
        Dim Second As Point
    End Structure
#End Region
#Region "Declarations"
    Private Declare Function WaitForSingleObject Lib "kernel32" Alias "WaitForSingleObject" (ByVal hHandle As Integer, ByVal dwMilliseconds As Integer) As Integer
    Private Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As Integer, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As Integer
    Private Declare Function VirtualFreeEx Lib "kernel32" (ByVal hProcess As Integer, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Integer

    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Integer) As Integer
    Private Declare Function CreateRemoteThread Lib "kernel32" ( _
          ByVal hProcess As Integer, _
          ByVal lpThreadAttributes As Integer, _
          ByVal dwStackSize As Integer, _
          ByVal lpStartAddress As Integer, _
          ByVal lpParameter As Integer, _
          ByVal dwCreationFlags As Integer, _
          ByRef lpThreadId As Integer) As Integer

    Private Const MEM_RELEASE = &H8000&
    Private Const MEM_COMMIT As Long = &H1000&
    Private Const PAGE_EXECUTE_READWRITE = &H40

#End Region
#Region "Constants"
    'Collision Map Constants
    Private Const MAP_DATA_INVALID = -1 'Invalid
    Private Const MAP_DATA_CLEANED = 11110 'Cleaned for start/end positions
    Private Const MAP_DATA_FILLED = 11111 'Filled gaps
    Private Const MAP_DATA_THICKENED = 11113 'Wall thickened
    Private Const MAP_DATA_AVOID = 11115 'Should be avoided

    Private Const UNIT_TYPE_PLAYER = 0
    Private Const UNIT_TYPE_NPC = 1
    Private Const UNIT_TYPE_OBJECT = 2
    Private Const UNIT_TYPE_MISSILE = 3
    Private Const UNIT_TYPE_ITEM = 4
    Private Const UNIT_TYPE_TILE = 5
#End Region

    Dim Playerinfo As Playerinfo_t
    Dim m_map As CMatrix

    Dim Mapinfo As New Dictionary(Of Integer, MapInfo_t)

    Dim D2client As System.Diagnostics.ProcessModule
    Dim D2common As System.Diagnostics.ProcessModule
    Dim MemEditor As New MemEdit()
    Dim LevelNo As Integer

#Region "Private"
    Private Sub AddRoomData(ByVal LevelNo As Long, ByVal X As Long, ByVal Y As Long)
        Dim BufferByte As Byte()

        Dim AsmStub As Byte() = _
        {&H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &HB8, 0, 0, 0, 0, _
         &HFF, &HD0, _
         &HC3}

        'AsmStub(1) = Playerinfo.dwRoomAddr
        BufferByte = BitConverter.GetBytes(Playerinfo.dwRoomAddr)
        Dim i As Integer
        For i = 1 To BufferByte.Length
            If BufferByte(i - 1) <> 0 Then
                AsmStub(i) = BufferByte(i - 1)
            End If
        Next
        'AsmStub(6) = Y
        BufferByte = BitConverter.GetBytes(Y)
        For i = 6 To BufferByte.Length + 5
            If BufferByte(i - 6) <> 0 Then
                AsmStub(i) = BufferByte(i - 6)
            End If
        Next
        'AsmStub(11) = X
        BufferByte = BitConverter.GetBytes(X)
        For i = 11 To BufferByte.Length + 10
            If BufferByte(i - 11) <> 0 Then
                AsmStub(i) = BufferByte(i - 11)
            End If
        Next
        'AsmStub(16) = LevelNo
        BufferByte = BitConverter.GetBytes(LevelNo)
        For i = 16 To BufferByte.Length + 15
            If BufferByte(i - 16) <> 0 Then
                AsmStub(i) = BufferByte(i - 16)
            End If
        Next
        'AsmStub(21) = Playerinfo.dwActAddr
        BufferByte = BitConverter.GetBytes(Playerinfo.dwActAddr)
        For i = 21 To BufferByte.Length + 20
            If BufferByte(i - 21) <> 0 Then
                AsmStub(i) = BufferByte(i - 21)
            End If
        Next
        'AsmStub(26) = D2common.BaseAddress.ToInt32 + &H26690
        BufferByte = BitConverter.GetBytes(D2common.BaseAddress.ToInt32 + &H26690)
        For i = 26 To BufferByte.Length + 25
            If BufferByte(i - 26) <> 0 Then
                AsmStub(i) = BufferByte(i - 26)
            End If
        Next

        Dim Address As Integer = VirtualAllocEx(MemEditor.netProcHandle.Handle.ToInt32, Nothing, (Len(AsmStub(0)) * AsmStub.Length), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        MemEditor.WriteMemory(Address, AsmStub)
        Dim Phandle As Long = CreateRemoteThread(MemEditor.netProcHandle.Handle.ToInt32, 0, 4, Address, Nothing, Nothing, Nothing)
        WaitForSingleObject(Phandle, 10000)
        CloseHandle(Phandle)
        VirtualFreeEx(MemEditor.netProcHandle.Handle.ToInt32, Address, (Len(AsmStub(0)) * AsmStub.Length), MEM_RELEASE)
    End Sub
    Private Sub RemoveRoomData(ByVal LevelNo As Long, ByVal X As Long, ByVal Y As Long)
        Dim BufferByte As Byte()
        Dim AsmStub As Byte() = _
        {&H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &H68, 0, 0, 0, 0, _
         &HB8, 0, 0, 0, 0, _
         &HFF, &HD0, _
         &HC3}

        'AsmStub(1) = Playerinfo.dwRoomAddr
        BufferByte = BitConverter.GetBytes(Playerinfo.dwRoomAddr)
        Dim i As Integer
        For i = 1 To BufferByte.Length
            If BufferByte(i - 1) <> 0 Then
                AsmStub(i) = BufferByte(i - 1)
            End If
        Next
        'AsmStub(6) = Y
        BufferByte = BitConverter.GetBytes(Y)
        For i = 6 To BufferByte.Length + 5
            If BufferByte(i - 6) <> 0 Then
                AsmStub(i) = BufferByte(i - 6)
            End If
        Next
        'AsmStub(11) = X
        BufferByte = BitConverter.GetBytes(X)
        For i = 11 To BufferByte.Length + 10
            If BufferByte(i - 11) <> 0 Then
                AsmStub(i) = BufferByte(i - 11)
            End If
        Next
        'AsmStub(16) = LevelNo
        BufferByte = BitConverter.GetBytes(LevelNo)
        For i = 16 To BufferByte.Length + 15
            If BufferByte(i - 16) <> 0 Then
                AsmStub(i) = BufferByte(i - 16)
            End If
        Next
        'AsmStub(21) = Playerinfo.dwActAddr
        BufferByte = BitConverter.GetBytes(Playerinfo.dwActAddr)
        For i = 21 To BufferByte.Length + 20
            If BufferByte(i - 21) <> 0 Then
                AsmStub(i) = BufferByte(i - 21)
            End If
        Next
        'AsmStub(26) = D2common.BaseAddress.ToInt32 + &H26690
        BufferByte = BitConverter.GetBytes(D2common.BaseAddress.ToInt32 + &H26550)
        For i = 26 To BufferByte.Length + 25
            If BufferByte(i - 26) <> 0 Then
                AsmStub(i) = BufferByte(i - 26)
            End If
        Next

        Dim Address As Integer = VirtualAllocEx(MemEditor.netProcHandle.Handle.ToInt32, Nothing, (Len(AsmStub(0)) * AsmStub.Length), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        MemEditor.WriteMemory(Address, AsmStub)
        Dim Phandle As Long = CreateRemoteThread(MemEditor.netProcHandle.Handle.ToInt32, 0, Len(New Int32), Address, Nothing, Nothing, Nothing)
        WaitForSingleObject(Phandle, 10000)
        CloseHandle(Phandle)
        VirtualFreeEx(MemEditor.netProcHandle.Handle.ToInt32, Address, (Len(AsmStub(0)) * AsmStub.Length), MEM_RELEASE)
    End Sub
    Private Function ByteToCollMap(ByVal CollMapByte() As Byte) As CollMap_t
        Dim pcol As CollMap_t
        Dim Buffer As String = ""
        Dim i As Integer = 3

        While i >= 0
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwPosGameX = HexToDec(Buffer)
        Buffer = ""

        i = 7
        While i > 3
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwPosGameY = HexToDec(Buffer)
        Buffer = ""

        i = 11
        While i > 7
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwSizeGameX = HexToDec(Buffer)
        Buffer = ""

        i = 15
        While i > 11
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwSizeGameY = HexToDec(Buffer)
        Buffer = ""

        i = 19
        While i > 15
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwPosRoomX = HexToDec(Buffer)
        Buffer = ""

        i = 23
        While i > 19
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwPosRoomY = HexToDec(Buffer)
        Buffer = ""

        i = 27
        While i > 23
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwSizeRoomX = HexToDec(Buffer)
        Buffer = ""

        i = 31
        While i > 27
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.dwSizeRoomY = HexToDec(Buffer)
        Buffer = ""

        i = 35
        While i > 31
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.MapStart = HexToDec(Buffer)
        Buffer = ""

        i = 39
        While i > 35
            Select Case Len(DecToHex(CollMapByte(i)))
                Case 1
                    Buffer &= Int(0).ToString & DecToHex(CollMapByte(i).ToString)
                Case 0
                    Buffer &= "00"
                Case Else
                    Buffer &= DecToHex(CollMapByte(i))
            End Select
            i -= 1
        End While
        pcol.MapEnd = HexToDec(Buffer)
        Return pcol
    End Function
    Private Function DecToHex(ByVal DecVal As Double) As String
        Dim a As Double, b As Double, c As String, d As Double
        a = DecVal
        For b = 1 To Int(Math.Log(DecVal) / Math.Log(16)) + 1
            d = CDbl(a Mod 16)
            Select Case d
                Case 0 To 9
                    c = d
                Case Else
                    c = Chr(55 + d)
            End Select
            DecToHex = c & DecToHex
            a = CDbl(Int(a / 16))
        Next b

    End Function
    Private Function HexToDec(ByVal HexVal As String) As Double
        Dim TotalDec As Double, a As Double, c As Double
        For a = 1 To Len(HexVal)
            Select Case (Mid(HexVal, a, 1))
                Case 0 To 9
                    c = (Mid(HexVal, a, 1))
                Case Else
                    c = (Asc(Mid(HexVal, a, 1)) - 55)
            End Select
            TotalDec = (TotalDec * 16) + c
        Next a
        HexToDec = TotalDec
    End Function
#End Region

#Region "Public"

    Public Function MyPosition() As Point
        Dim MemReader As New MemEdit

        If MemReader.mOpenProcess("Diablo II") = Nothing Then Return Nothing
        D2client = MemReader.GetModule("D2Client.dll")
        If D2client Is Nothing Then Return Nothing
        Dim Dwtemp As Int32
        Playerinfo.dwUnitAddr = MemReader.ReadMemoryLong(D2client.BaseAddress.ToInt32 + &H11C1E0, 4)
        Dwtemp = MemReader.ReadMemoryLong(Playerinfo.dwUnitAddr + &H2C, 4)

        MyPosition.X = MemReader.ReadMemoryShort(Dwtemp + &H2)
        MyPosition.Y = MemReader.ReadMemoryShort(Dwtemp + &H6)
        MemReader.mCloseProcess()
    End Function

    Public Function GetMapFromMemory() As MapInfo_t
        Dim Dwtemp As Long

        If MemEditor.mOpenProcess("Diablo II") = 0 Then
            Return Nothing
        End If

        D2client = MemEditor.GetModule("D2Client.dll")
        D2common = MemEditor.GetModule("D2Common.dll")
        If D2client Is Nothing Then Return Nothing
        Playerinfo.dwUnitAddr = MemEditor.ReadMemoryLong(D2client.BaseAddress.ToInt32 + &H11C1E0, Len(New Int32))
        Playerinfo.dwPlayerId = MemEditor.ReadMemoryLong(Playerinfo.dwUnitAddr + &HC, 4)
        Playerinfo.dwActAddr = MemEditor.ReadMemoryLong(Playerinfo.dwUnitAddr + &H1C, 4)

        Dwtemp = MemEditor.ReadMemoryLong(Playerinfo.dwUnitAddr + &H2C, 4)
        Playerinfo.dwRoomAddr = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)

        'Map Information
        Dwtemp = MemEditor.ReadMemoryLong(Playerinfo.dwUnitAddr + &H2C, 4)
        Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)
        Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H38, 4)
        Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)

        LevelNo = MemEditor.ReadMemoryLong(Dwtemp + &H14, 4)

        'Put the mapinfo in a buffer so we can easily change datas.
        Dim BufferMapInfo As New MapInfo_t
        'Initialize the Arrays
        BufferMapInfo.LevelsNear = New OrderedDictionary(Of Long, Exit_T)
        BufferMapInfo.Exits = New OrderedDictionary(Of Long, Point)
        BufferMapInfo.Npcs = New OrderedDictionary(Of Long, Point)
        BufferMapInfo.Objects = New OrderedDictionary(Of Long, Point)

        'Get the General Informations of the map.
        BufferMapInfo.LevelNo = LevelNo
        BufferMapInfo.MapPosX = MemEditor.ReadMemoryLong(Dwtemp + &H4, 4)
        BufferMapInfo.MapPosX = MemEditor.ReadMemoryLong(Dwtemp + &H4, 4)
        BufferMapInfo.MapPosY = MemEditor.ReadMemoryLong(Dwtemp + &H8, 4)
        BufferMapInfo.MapSizeX = MemEditor.ReadMemoryLong(Dwtemp + &HC, 4)
        BufferMapInfo.MapSizeY = MemEditor.ReadMemoryLong(Dwtemp + &H10, 4)

        'If the map has already been loaded
        If Mapinfo.ContainsKey(LevelNo) Then
            'Overwrite it
            Mapinfo(LevelNo) = BufferMapInfo
        Else
            'Create it
            Mapinfo.Add(LevelNo, BufferMapInfo)
        End If

        m_map = New CMatrix(Mapinfo(LevelNo).MapSizeX * 5, Mapinfo(LevelNo).MapSizeY * 5, MAP_DATA_INVALID)

        'Preparing to Loop trought all Rooms
        Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H204, 4)

        While Dwtemp
            Dim Exitt As Integer = 0

            Dim DwRoom As Long = Nothing
            Dim MapX As Long = Nothing
            Dim MapY As Long = Nothing

            Dim AddedRoom As Boolean = False

            DwRoom = MemEditor.ReadMemoryLong(Dwtemp + &HE8, 4)
            MapX = MemEditor.ReadMemoryLong(Dwtemp + &H20, 4)
            MapY = MemEditor.ReadMemoryLong(Dwtemp + &H24, 4)

            If DwRoom = 0 Then
                AddedRoom = True
                AddRoomData(Mapinfo(LevelNo).LevelNo, MapX, MapY)
                DwRoom = MemEditor.ReadMemoryLong(Dwtemp + &HE8, 4)
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''''''''''''''''''''''''''Find exits Code'''''''''''''''''''''
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dwRoomsNear As Int32
            Dim pRoom2Near As Int32

            dwRoomsNear = MemEditor.ReadMemoryLong(Dwtemp + &H10, 4)
            pRoom2Near = MemEditor.ReadMemoryLong(Dwtemp + &H30, 4)

            For i2 As Integer = 0 To dwRoomsNear - 1
                Dim pNear As Int32
                Dim pLevel As Int32
                Dim dwLevelNo As Int32
                Dim dwPosX As Int32
                Dim dwPosY As Int32
                Dim dwSizeX As Int32
                Dim dwSizeY As Int32

                pNear = MemEditor.ReadMemoryLong(pRoom2Near + (i2 * 4), 4)
                pLevel = MemEditor.ReadMemoryLong(pNear + &H1C, 4)

                dwLevelNo = MemEditor.ReadMemoryLong(pLevel + &H14, 4)

                dwPosX = MemEditor.ReadMemoryLong(pLevel + &H4, 4)
                dwPosY = MemEditor.ReadMemoryLong(pLevel + &H8, 4)
                dwSizeX = MemEditor.ReadMemoryLong(pLevel + &HC, 4)
                dwSizeY = MemEditor.ReadMemoryLong(pLevel + &H10, 4)

                Dim Newlevel As Exit_T
                Newlevel.First.X = dwPosX * 5
                Newlevel.First.Y = dwPosY * 5
                Newlevel.Second.X = dwSizeX * 5
                Newlevel.Second.Y = dwSizeY * 5

                If Mapinfo(LevelNo).LevelsNear.Keys.Contains(dwLevelNo) = False Then
                    Mapinfo(LevelNo).LevelsNear.Add(dwLevelNo, Newlevel)
                Else
                    Mapinfo(LevelNo).LevelsNear.ItemBykey(dwLevelNo) = Newlevel
                End If
            Next


            Dim pUnit As Int32
            pUnit = MemEditor.ReadMemoryLong(Dwtemp + &H34, 4)

            While (pUnit <> Nothing)

                Dim dwType As Int32
                Dim dwClassId As Int32
                Dim dwPosX As Int32
                Dim dwPosY As Int32

                dwType = MemEditor.ReadMemoryLong(pUnit + &H1C, 4)
                dwClassId = MemEditor.ReadMemoryLong(pUnit + &HC, 4)
                dwPosX = MemEditor.ReadMemoryLong(pUnit + &H18, 4)
                dwPosY = MemEditor.ReadMemoryLong(pUnit + &H8, 4)

                If dwType = UNIT_TYPE_NPC Then
                    If Mapinfo(LevelNo).Npcs.ContainsKey(dwClassId) = False Then
                        Mapinfo(LevelNo).Npcs.Add(dwClassId, New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY))
                    End If
                ElseIf dwType = UNIT_TYPE_OBJECT Then
                    If Mapinfo(LevelNo).Objects.ContainsKey(dwClassId) = False Then
                        Mapinfo(LevelNo).Objects.Add(dwClassId, New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY))
                    End If

                ElseIf dwType = UNIT_TYPE_TILE Then
                    Dim pRoomTile As Int32

                    pRoomTile = MemEditor.ReadMemoryLong(Dwtemp + &H0, 4)

                    While pRoomTile <> Nothing
                        Dim pNum As Int32
                        Dim nNum As Int32

                        pNum = MemEditor.ReadMemoryLong(pRoomTile + &HC, 4)
                        nNum = MemEditor.ReadMemoryLong(pNum, 4)

                        If nNum = dwClassId Then
                            Dim pRoom2 As Int32
                            Dim pLevel As Int32
                            Dim dwLevelNo As Int32
                            pRoom2 = MemEditor.ReadMemoryLong(pRoomTile + &H4, 4)
                            pLevel = MemEditor.ReadMemoryLong(pRoom2 + &H1C, 4)
                            dwLevelNo = MemEditor.ReadMemoryLong(pLevel + &H14, 4)

                            If Mapinfo(LevelNo).Exits.Keys.Contains(dwLevelNo) = False Then
                                Mapinfo(LevelNo).Exits.Add(dwLevelNo, New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY))
                            Else
                                Mapinfo(LevelNo).Exits.ItemBykey(dwLevelNo) = New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY)
                            End If

                        End If
                        pRoomTile = MemEditor.ReadMemoryLong(pRoomTile + &H8, 4)
                    End While

                End If
                pUnit = MemEditor.ReadMemoryLong(pUnit + &H14, 4)
            End While


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''Come back to the function''''''''''''''''''
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dwCol As Int32
            dwCol = MemEditor.ReadMemoryLong(DwRoom + &H60, 4)

            Dim CollMapByte As Byte()
            Dim pcol As CollMap_t

            CollMapByte = MemEditor.ReadMemoryAOB(dwCol, Len(New CollMap_t))
            pcol = ByteToCollMap(CollMapByte)

            ' ***Building CollisionMap here ***

            Dim X As Integer = pcol.dwPosGameX - Mapinfo(LevelNo).MapPosX * 5
            Dim Y As Integer = pcol.dwPosGameY - Mapinfo(LevelNo).MapPosY * 5
            Dim Cx As Integer = pcol.dwSizeGameX
            Dim Cy As Integer = pcol.dwSizeGameY

            Dim P As Long = Nothing ' Word
            Dim pAddr As Int32 'Dword

            pAddr = MemEditor.ReadMemoryLong(dwCol + &H20, 4)
            P = MemEditor.ReadMemoryByte(pAddr)
            pAddr += 2

            Dim nLimitX As Integer = X + Cx
            Dim nLimitY As Integer = Y + Cy

            If m_map.IsValidIndex(X, Y) Then
                Dim i As Integer
                Dim j As Integer
                For j = Y To nLimitY - 1
                    For i = X To nLimitX - 1
                        'Remember the coordinate 
                        m_map.SetXY(i, j, P)
                        'If it's an invalid spot but divisible by 2
                        If m_map.GetAt(i, j) = "1024" Then
                            'It's invalid...
                            m_map.SetXY(i, j, MAP_DATA_INVALID)
                        End If
                        P = MemEditor.ReadMemoryInt(pAddr)
                        pAddr += 2
                    Next
                Next
            End If

            'If we added a room
            If AddedRoom Then
                'Clear the memory.
                RemoveRoomData(Mapinfo(LevelNo).LevelNo, MapX, MapY)
            End If
            Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H38, 4)
        End While
        'Close the memory.
        MemEditor.mCloseProcess()

        'Copy the map to a text file (Good for testing)
        'WriteMapToFile("C:\map.txt", Mapinfo(LevelNo))

        'Put the mapinfo in a buffer so we can easily edit the values.
        BufferMapInfo = Mapinfo(LevelNo)

        'Get the actual Map Size
        BufferMapInfo.MapSizeX = BufferMapInfo.MapSizeX * 5
        BufferMapInfo.MapSizeY = BufferMapInfo.MapSizeY * 5

        'Get ready to transfer MapData
        ReDim BufferMapInfo.Bytes(BufferMapInfo.MapSizeX, BufferMapInfo.MapSizeY)
        BufferMapInfo.Bytes = m_map.MapData
        Mapinfo(LevelNo) = BufferMapInfo

        Return Mapinfo(LevelNo)

    End Function
    Public Function GetPreviousMapInfo(ByVal LevelNo As D2Data.AreaLevel) As MapInfo_t
        'If the map has already been registered.
        If Mapinfo.ContainsKey(LevelNo) Then
            'Return the infos.
            Return Mapinfo(LevelNo)
        End If
        'Map not existing, return nothing
        Return Nothing

    End Function

    Public Sub WriteMapToFile(ByVal Path As String, ByVal Mapdata As MapInfo_t)

        Dim oWrite As System.IO.StreamWriter
        oWrite = IO.File.CreateText(Path)

        Dim I As Integer
        Dim J As Integer
        Dim pointt As Point

        While J < m_map.m_cY - 1
            I = 0
            While I < m_map.m_cX - 1
                pointt = New Point(I, J)
                If Mapdata.Bytes(I, J) Mod 2 = 0 Then
                    oWrite.Write(" ")
                Else
                    oWrite.Write("X")
                End If
                I += 1
            End While
            oWrite.WriteLine()
            J += 1
        End While
        oWrite.Close()
    End Sub
#End Region

#Region "Classes"

    Private Class CMatrix
        Public m_cX As Integer
        Public m_cY As Integer
        Public MapData(,) As Integer
        Public Created As Boolean

        Sub New(ByVal Cx As Integer, ByVal Cy As Integer)
            Destroy()
            m_cX = Cx
            m_cY = Cy
            ReDim MapData(Cx, Cy)
            Created = True
        End Sub
        Sub New(ByVal Cx As Integer, ByVal Cy As Integer, ByVal Argument As Integer)
            Destroy()
            m_cX = Cx
            m_cY = Cy
            ReDim MapData(Cx, Cy)

            Dim I As Integer
            Dim J As Integer

            While I < m_cX
                J = 0
                While J < m_cY
                    MapData(I, J) = Argument
                    J += 1
                End While
                I += 1
            End While
            Created = True
        End Sub
        Sub Destroy()
            MapData = Nothing
            m_cX = 0
            m_cY = 0
        End Sub
        Sub SetXY(ByVal X As Integer, ByVal Y As Integer, ByVal Argument As Integer)
            If IsValidIndex(X, Y) Then
                MapData(X, Y) = Argument
            End If
        End Sub
        Public Function GetAt(ByVal X As Integer, ByVal Y As Integer) As Integer
            If IsValidIndex(X, Y) Then
                Return MapData(X, Y)
            End If
        End Function
        Public Function IsValidIndex(ByVal X As Integer, ByVal Y As Integer) As Boolean
            Return (X >= 0 And X < m_cX And Y >= 0 And Y < m_cY)
        End Function
    End Class

    Public Class OrderedDictionary(Of TKey, TValue)
        Implements IDictionary(Of TKey, TValue)

        Private m_dictionary As New Dictionary(Of TKey, TValue)
        Private m_keysInOrder As New List(Of TKey)

        Default Public Overloads Property Item(ByVal index As Integer) As TValue
            Get
                Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(m_keysInOrder(index))
            End Get
            Set(ByVal value As TValue)
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(m_keysInOrder(index)) = value
            End Set
        End Property
        Default Public Overloads Property Item(ByVal key As TKey) As TValue Implements IDictionary(Of TKey, TValue).Item
            Get
                Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(key)
            End Get
            Set(ByVal value As TValue)
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Item(key) = value
            End Set
        End Property

        Public Property ItemBykey(ByVal key As TKey) As TValue
            Get
                Return m_dictionary(key)
            End Get
            Set(ByVal Value As TValue)
                m_dictionary(key) = Value
            End Set
        End Property

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Clear
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Clear()
            m_keysInOrder.Clear()
        End Sub
        Public Function Contains(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Contains
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Contains(item)
        End Function
        Public Sub CopyTo(ByVal array() As KeyValuePair(Of TKey, TValue), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).CopyTo
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).CopyTo(array, arrayIndex)
        End Sub
        Public ReadOnly Property Count() As Integer Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Count
            Get
                Return CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Count
            End Get
        End Property
        Public ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).IsReadOnly
            Get
                Return CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).IsReadOnly
            End Get
        End Property
        Public Function Remove(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).Remove
            Return CType(m_dictionary, IDictionary(Of TKey, TValue)).Remove(key)
            m_keysInOrder.Remove(key)
        End Function
        Public Function Remove(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Remove
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Remove(item)
            m_keysInOrder.Remove(item.Key)
        End Function
        Public Sub Add(ByVal key As TKey, ByVal value As TValue) Implements IDictionary(Of TKey, TValue).Add
            If m_dictionary.ContainsKey(key) = False Then
                CType(m_dictionary, IDictionary(Of TKey, TValue)).Add(key, value)
                m_keysInOrder.Add(key)
            End If

        End Sub
        Public Sub Add(ByVal item As KeyValuePair(Of TKey, TValue)) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Add
            CType(m_dictionary, ICollection(Of KeyValuePair(Of TKey, TValue))).Add(item)
            m_keysInOrder.Add(item.Key)
        End Sub
        Public Function ContainsKey(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).ContainsKey
            CType(m_dictionary, IDictionary(Of TKey, TValue)).ContainsKey(key)
        End Function
        Public ReadOnly Property Keys() As ICollection(Of TKey) Implements IDictionary(Of TKey, TValue).Keys
            Get
                Return m_keysInOrder
            End Get
        End Property
        Public Function TryGetValue(ByVal key As TKey, ByRef value As TValue) As Boolean Implements IDictionary(Of TKey, TValue).TryGetValue
            Return CType(m_dictionary, IDictionary(Of TKey, TValue)).TryGetValue(key, value)
        End Function
        Public ReadOnly Property Values() As ICollection(Of TValue) Implements IDictionary(Of TKey, TValue).Values
            Get
                Dim rtnVal As New List(Of TValue)

                For Each key As TKey In m_keysInOrder
                    rtnVal.Add(m_dictionary(key))
                Next

                Return rtnVal
            End Get
        End Property
        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, TValue)) Implements IEnumerable(Of KeyValuePair(Of TKey, TValue)).GetEnumerator
            Return New Enumerator(Me)
        End Function
        Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return New Enumerator(Me)
        End Function
        Public Structure Enumerator
            Implements IEnumerator(Of KeyValuePair(Of TKey, TValue))

            Private m_dictionary As OrderedDictionary(Of TKey, TValue)
            Private m_nextIndex As Integer
            Private m_current As KeyValuePair(Of TKey, TValue)

            Friend Sub New(ByVal orderedDictionary As OrderedDictionary(Of TKey, TValue))
                m_dictionary = orderedDictionary
                m_nextIndex = 0
                m_current = New KeyValuePair(Of TKey, TValue)
            End Sub

            Public ReadOnly Property Current() As KeyValuePair(Of TKey, TValue) Implements IEnumerator(Of KeyValuePair(Of TKey, TValue)).Current
                Get
                    Return New KeyValuePair(Of TKey, TValue)(m_current.Key, m_current.Value)
                End Get
            End Property

            Private ReadOnly Property Current1() As Object Implements IEnumerator.Current
                Get
                    Return Current
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Dim rtnVal As Boolean = False

                If m_nextIndex < m_dictionary.Count Then
                    rtnVal = True
                    Dim key As TKey = m_dictionary.m_keysInOrder(m_nextIndex)
                    m_current = New KeyValuePair(Of TKey, TValue)(key, m_dictionary.Item(key))
                    m_nextIndex += 1
                Else
                    m_current = New KeyValuePair(Of TKey, TValue)
                End If

                Return rtnVal
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset
                m_nextIndex = 0
            End Sub

            Public Sub Dispose() Implements System.IDisposable.Dispose
            End Sub
        End Structure
    End Class
#End Region

#Region "Pathfinding"


    Private Structure CellData
        Public OCList As Int16
        Public GCost As Int16
        Public HCost As Int16
        Public FCost As Int16
        Public ParentX, ParentY As Int16
        Public Wall As Boolean
        Public MaxX As Integer
        Public MaxY As Integer
    End Structure

#Region " Binary Heap Objects "

    Private Structure BinHeapData
        'Data Structure to hold FScore and Location
        Friend Score As Int16
        Friend X As Int16
        Friend Y As Int16
    End Structure

    Private Class BinaryHeap

#Region " Declares "
        Private lSize As Int16 'Size of the heap array
        Private Heap() As BinHeapData 'Heap Array
#End Region

#Region " Properties "

        'Return heap count
        Public ReadOnly Property Count() As Int16
            Get
                Return lSize
            End Get
        End Property

        'Return X position
        Public ReadOnly Property GetX() As Int16
            Get
                Return Heap(1).X
            End Get
        End Property

        'Return Y position
        Public ReadOnly Property GetY() As Int16
            Get
                Return Heap(1).Y
            End Get
        End Property

        'Return the lowest score
        Public ReadOnly Property GetScore() As Int16
            Get
                Return Heap(1).Score
            End Get
        End Property

#End Region

#Region " Subs "

        'Reset the heap
        Public Sub ResetHeap()
            lSize = 0
            ReDim Heap(0)
        End Sub

        'Remove the Root Object from the heap
        Public Sub RemoveRoot()

            'If only the root exists
            If lSize <= 1 Then
                lSize = 0
                ReDim Heap(0)
                Exit Sub
            End If

            'First copy the very bottom object to the top
            Heap(1) = Heap(lSize)

            'Resize the count
            lSize -= 1

            'Shrink the array
            ReDim Preserve Heap(lSize)

            'Sort the top item to it's correct position
            Dim Parent As Int16 = 1
            Dim ChildIndex As Int16 = 1

            'Sink the item to it's correct location
            While True
                ChildIndex = Parent
                If 2 * ChildIndex + 1 <= lSize Then
                    'Find the lowest value of the 2 child nodes
                    If Heap(ChildIndex).Score >= Heap(2 * ChildIndex).Score Then Parent = 2 * ChildIndex
                    If Heap(Parent).Score >= Heap(2 * ChildIndex + 1).Score Then Parent = 2 * ChildIndex + 1
                Else 'Just process the one node
                    If 2 * ChildIndex <= lSize Then
                        If Heap(ChildIndex).Score >= Heap(2 * ChildIndex).Score Then Parent = 2 * ChildIndex
                    End If
                End If

                'Swap out the child/parent
                If Parent <> ChildIndex Then
                    Dim tHeap As BinHeapData = Heap(ChildIndex)
                    Heap(ChildIndex) = Heap(Parent)
                    Heap(Parent) = tHeap
                Else
                    Exit While
                End If

            End While

        End Sub

        'Add the new element to the heap
        Public Sub Add(ByVal inScore As Int16, ByVal inX As Int16, ByVal inY As Int16)

            '**We will be ignoring the (0) place in the heap array because
            '**it's easier to handle the heap with a base of (1..?)

            'Increment the array count
            lSize += 1

            'Make room in the array
            ReDim Preserve Heap(lSize)

            'Store the data
            With Heap(lSize)
                .Score = inScore
                .X = inX
                .Y = inY
            End With

            'Bubble the item to its correct location
            Dim sPos As Int16 = lSize

            While sPos <> 1
                If Heap(sPos).Score <= Heap(sPos / 2).Score Then
                    Dim tHeap As BinHeapData = Heap(sPos / 2)
                    Heap(sPos / 2) = Heap(sPos)
                    Heap(sPos) = tHeap
                    sPos /= 2
                Else
                    Exit While
                End If
            End While

        End Sub

#End Region

    End Class
#End Region

#Region " Find Path "

    Private Const inOpened = 1
    Private Const inClosed = 2

    'Single Point
    Public Function RelativeToAbs(ByVal Point As Point, ByVal mapinfo As Pathing.MapInfo_t) As Point
        RelativeToAbs.X = Point.X + mapinfo.MapPosX * 5
        RelativeToAbs.Y = Point.Y + mapinfo.MapPosY * 5
        Return RelativeToAbs
    End Function
    'List
    Public Function RelativeToAbs(ByVal Point As List(Of Point), ByVal Mapinfo As Pathing.MapInfo_t) As List(Of Point)
        RelativeToAbs = New List(Of Point)
        If Not point Is Nothing Then
            'Each point are transfer into relative
            For i As Integer = 0 To Point.Count - 1
                RelativeToAbs.Add(RelativeToAbs(Point(i), Mapinfo))
            Next
        End If
        Return RelativeToAbs
    End Function
    Public Function AbsToRelative(ByVal Point As Point, ByVal Mapinfo As Pathing.MapInfo_t) As Point
        AbsToRelative.X = Math.Abs(Point.X - Mapinfo.MapPosX * 5)
        AbsToRelative.Y = Math.Abs(Point.Y - Mapinfo.MapPosY * 5)
        Return AbsToRelative
    End Function

    Private Function BitmapToCellData(ByVal MapImage As Bitmap) As CellData(,)
        Dim Map(MapImage.Width - 1, MapImage.Height - 1) As CellData

        For x As Integer = 0 To MapImage.Width - 1
            For y As Integer = 0 To MapImage.Height - 1
                If MapImage.GetPixel(x, y).ToArgb = 0 Then
                    Map(x, y).Wall = True
                End If
            Next
        Next

        Map(0, 0).MaxX = MapImage.Width
        Map(0, 0).MaxY = MapImage.Height

        Return Map
    End Function

    Private Function MapInfoToCellData(ByVal Mapinfo As MapInfo_t) As CellData(,)
        Dim Map(Mapinfo.MapSizeX - 1, Mapinfo.MapSizeY - 1) As CellData
        For X As Integer = 0 To Mapinfo.MapSizeX - 1
            For Y As Integer = 0 To Mapinfo.MapSizeY - 1
                If Not Mapinfo.Bytes(X, Y) Mod 2 = 0 Then
                    Map(X, Y).Wall = True
                End If
            Next
        Next
        Map(0, 0).MaxX = Mapinfo.MapSizeX
        Map(0, 0).MaxY = Mapinfo.MapSizeY
        Return Map
    End Function
    Public Function BitmapFromMapInfo(ByVal MapInfo As Pathing.MapInfo_t) As Bitmap

        If MapInfo.MapSizeX = 0 Or MapInfo.MapSizeY = 0 Then
            Return Nothing
        End If

        Dim LastDownPixel As Integer
        Dim LastRightPixel As Integer

        'This is so we don't collect useless data from the bitmap. 
        'Sometime maps are too big For what they really are.
        'This doesn't change anything except the memory usage
        For y As Integer = 0 To MapInfo.MapSizeY - 1
            For x As Integer = 0 To MapInfo.MapSizeX - 1
                If MapInfo.Bytes(x, y) Mod 2 = 0 Then
                    If x > LastRightPixel Then
                        'LastPixel On the right
                        LastRightPixel = x
                    End If
                    If y > LastDownPixel Then
                        'Set LastPixel Down
                        LastDownPixel = y
                    End If
                End If
            Next
        Next
        Dim b As New Bitmap(LastRightPixel, LastDownPixel)
        For y As Integer = 0 To LastDownPixel - 1
            For x As Integer = 0 To LastRightPixel - 1
                If MapInfo.Bytes(x, y) Mod 2 = 0 Then
                    b.SetPixel(x, y, Color.Blue)
                End If
            Next
        Next
        Return b
    End Function

    Private Function FindClosestFloor(ByVal Map As CellData(,), ByVal StartX As Integer, ByVal StartY As Integer, ByVal MaxX As Integer, ByVal MaxY As Integer) As Point
        Dim offset As Integer = 1

        While True
            For i As Integer = -offset To offset
                If StartX + i < 0 Or StartX + i > MaxX Then Exit For
                If StartY + offset < 0 Or StartY + offset > MaxY Then Exit For
                If Not Map(StartX + i, StartY + offset).Wall Then Return New Point(StartX + i, StartY + offset)
            Next
            For i As Integer = -offset + 1 To offset - 1
                If StartX + offset < 0 Or StartX + offset > MaxX Then Exit For
                If StartY + i < 0 Or StartY + i > MaxY Then Exit For
                If Not Map(StartX + offset, StartY + i).Wall Then Return New Point(StartX + offset, StartY + i)
            Next
            For i As Integer = -offset To offset
                If StartX + i < 0 Or StartX + i > MaxX Then Exit For
                If StartY - offset < 0 Or StartY - offset > MaxY Then Exit For
                If Not Map(StartX + i, StartY - offset).Wall Then Return New Point(StartX + i, StartY - offset)
            Next
            For i As Integer = -offset + 1 To offset - 1
                If StartX - offset < 0 Or StartX - offset > MaxX Then Exit For
                If StartY + i < 0 Or StartY + i > MaxY Then Exit For
                If Not Map(StartX - offset, StartY + i).Wall Then Return New Point(StartX - offset, StartY + i)
            Next
            offset += 1
        End While

        Return Nothing
    End Function
    Private Function GetWalkPath(ByVal StartPoint As Point, ByVal EndPoint As Point, ByVal Mapinfo As MapInfo_t) As List(Of Point)

        Dim Map As CellData(,) = MapInfoToCellData(Mapinfo)
        Dim Heap As New BinaryHeap()
        Dim StartX, StartY, EndX, EndY As Int16
        StartX = StartPoint.X
        StartY = StartPoint.Y
        EndX = EndPoint.X
        EndY = EndPoint.Y

        Dim MaxX As Integer = Map(0, 0).MaxX
        Dim MaxY As Integer = Map(0, 0).MaxY

        For yC As Int16 = 0 To MaxY - 1
            For xC As Int16 = 0 To MaxX - 1
                With Map(xC, yC)
                    .FCost = 0
                    .GCost = 0
                    .HCost = 0
                    .OCList = 0
                    .ParentX = 0
                    .ParentY = 0
                End With
            Next
        Next

        Dim PathFound, PathHunt As Boolean
        Dim ParentX, ParentY As Int16

        Dim xCnt, yCnt As Int16

        'Make sure the starting point and ending point are not the same
        If (StartX = EndX) And (StartY = EndY) Then Return Nothing

        'Make sure the start and end point is not a wall
        If Map(StartX, StartY).Wall Then
            Dim ClosestPoint As Point = FindClosestFloor(Map, StartX, StartY, MaxX - 1, MaxY - 1)
            StartX = ClosestPoint.X
            StartY = ClosestPoint.Y
        End If
        If Map(EndX, EndY).Wall Then
            Dim ClosestPoint As Point = FindClosestFloor(Map, EndX, EndY, MaxX - 1, MaxY - 1)
            EndX = ClosestPoint.X
            EndY = ClosestPoint.Y
        End If

        'Set the flags
        PathFound = False
        PathHunt = True

        'Put the starting point on the open list
        Map(StartX, StartY).OCList = inOpened
        Heap.Add(0, StartX, StartY)

        'Find the children
        While PathHunt
            If Heap.Count <> 0 Then
                'Get the parent node
                ParentX = Heap.GetX
                ParentY = Heap.GetY

                'Remove the root
                Map(ParentX, ParentY).OCList = inClosed
                Heap.RemoveRoot()

                'Find the available children to add to the open list
                For yCnt = (ParentY - 1) To (ParentY + 1)
                    For xCnt = (ParentX - 1) To (ParentX + 1)

                        'Make sure we are not out of bounds
                        If xCnt <> -1 And xCnt <> MaxX And yCnt <> -1 And yCnt < MaxY Then

                            'Make sure it's not on the closed list
                            If Map(xCnt, yCnt).OCList <> inClosed Then

                                'Make sure no wall
                                If Map(xCnt, yCnt).Wall = False Then

                                    'Don't cut across corners
                                    Dim CanWalk As Boolean = True
                                    If xCnt = ParentX - 1 Then
                                        If yCnt = ParentY - 1 Then
                                            If Map(ParentX - 1, ParentY).Wall = True Or Map(ParentX, ParentY - 1).Wall = True Then CanWalk = False
                                        ElseIf yCnt = ParentY + 1 Then
                                            If Map(ParentX, ParentY + 1).Wall = True Or Map(ParentX - 1, ParentY).Wall = True Then CanWalk = False
                                        End If
                                    ElseIf xCnt = ParentX + 1 Then
                                        If yCnt = ParentY - 1 Then
                                            If Map(ParentX, ParentY - 1).Wall = True Or Map(ParentX + 1, ParentY).Wall = True Then CanWalk = False
                                        ElseIf yCnt = ParentY + 1 Then
                                            If Map(ParentX + 1, ParentY).Wall = True Or Map(ParentX, ParentY + 1).Wall = True Then CanWalk = False
                                        End If
                                    End If

                                    'If we can move this way
                                    If CanWalk = True Then
                                        If Map(xCnt, yCnt).OCList <> inOpened Then

                                            'Calculate the GCost
                                            If Math.Abs(xCnt - ParentX) = 1 And Math.Abs(yCnt - ParentY) = 1 Then
                                                Map(xCnt, yCnt).GCost = Map(ParentX, ParentY).GCost + 14
                                            Else
                                                Map(xCnt, yCnt).GCost = Map(ParentX, ParentY).GCost + 10
                                            End If

                                            'Calculate the HCost
                                            Map(xCnt, yCnt).HCost = 10 * (Math.Abs(xCnt - EndX) + Math.Abs(yCnt - EndY))
                                            Map(xCnt, yCnt).FCost = (Map(xCnt, yCnt).GCost + Map(xCnt, yCnt).HCost)

                                            'Add the parent value
                                            Map(xCnt, yCnt).ParentX = ParentX
                                            Map(xCnt, yCnt).ParentY = ParentY

                                            'Add the item to the heap
                                            Heap.Add(Map(xCnt, yCnt).FCost, xCnt, yCnt)

                                            'Add the item to the open list
                                            Map(xCnt, yCnt).OCList = inOpened

                                        Else
                                            'We will check for better value
                                            Dim AddedGCost As Int16
                                            If Math.Abs(xCnt - ParentX) = 1 And Math.Abs(yCnt - ParentY) = 1 Then
                                                AddedGCost = 14
                                            Else
                                                AddedGCost = 10
                                            End If
                                            Dim tempCost As Int16 = Map(ParentX, ParentY).GCost + AddedGCost

                                            If tempCost < Map(xCnt, yCnt).GCost Then
                                                Map(xCnt, yCnt).GCost = tempCost
                                                Map(xCnt, yCnt).ParentX = ParentX
                                                Map(xCnt, yCnt).ParentY = ParentY
                                                If Map(xCnt, yCnt).OCList = inOpened Then
                                                    Dim NewCost As Int16 = Map(xCnt, yCnt).HCost + Map(xCnt, yCnt).GCost
                                                    Heap.Add(NewCost, xCnt, yCnt)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next
            Else
                PathFound = False
                PathHunt = False
                Return Nothing
            End If

            'If we find a path
            If Map(EndX, EndY).OCList = inOpened Then
                PathFound = True
                PathHunt = False
            End If

        End While

        If PathFound Then
            Dim Points As New List(Of Point)
            Dim tX As Int16 = EndX
            Dim tY As Int16 = EndY
            Points.Add(New Point(tX, tY))

            While True
                Dim sX As Int16 = Map(tX, tY).ParentX
                Dim sY As Int16 = Map(tX, tY).ParentY
                Points.Add(New Point(sX, sY))
                tX = sX
                tY = sY
                If tX = StartX And tY = StartY Then Exit While
            End While
            Points.Reverse()
            Return Points
        End If

        Return Nothing
    End Function
    Private Function GetTeleportPath(ByVal StartPoint As Point, ByVal EndPoint As Point, ByVal Mapinfo As MapInfo_t, ByVal dist As Integer) As List(Of Point)
        On Error Resume Next
        Dim Heap As New BinaryHeap()

        Dim Map As CellData(,) = MapInfoToCellData(Mapinfo)

        Dim StartX, StartY, EndX, EndY As Int16
        StartX = StartPoint.X
        StartY = StartPoint.Y
        EndX = EndPoint.X
        EndY = EndPoint.Y

        Dim MaxX As Integer = Map(0, 0).MaxX
        Dim MaxY As Integer = Map(0, 0).MaxY

        For yC As Int16 = 0 To MaxY - 1
            For xC As Int16 = 0 To MaxX - 1
                With Map(xC, yC)
                    .FCost = 0
                    .GCost = 0
                    .HCost = 0
                    .OCList = 0
                    .ParentX = 0
                    .ParentY = 0
                End With
            Next
        Next

        Dim PathFound, PathHunt As Boolean
        Dim ParentX, ParentY As Int16

        Dim xCnt, yCnt As Int16

        'Make sure the starting point and ending point are not the same
        If (StartX = EndX) And (StartY = EndY) Then Return Nothing

        'Make sure the start and end point is not a wall
        If Map(StartX, StartY).Wall Then
            Dim ClosestPoint As Point = FindClosestFloor(Map, StartX, StartY, MaxX - 1, MaxY - 1)
            StartX = ClosestPoint.X
            StartY = ClosestPoint.Y
        End If

        If Map(EndX, EndY).Wall Then
            Dim ClosestPoint As Point = FindClosestFloor(Map, EndX, EndY, MaxX - 1, MaxY - 1)
            EndX = ClosestPoint.X
            EndY = ClosestPoint.Y
        End If

        'Set the flags
        PathFound = False
        PathHunt = True

        'Put the starting point on the open list
        Map(StartX, StartY).OCList = inOpened
        Heap.Add(0, StartX, StartY)

        'Find the children
        While PathHunt
            If Heap.Count <> 0 Then
                'Get the parent node
                ParentX = Heap.GetX
                ParentY = Heap.GetY

                'Remove the root
                Map(ParentX, ParentY).OCList = inClosed
                Heap.RemoveRoot()

                'Find the available children to add to the open list
                For yCnt = (ParentY - 1) To (ParentY + 1)
                    For xCnt = (ParentX - 1) To (ParentX + 1)

                        'Make sure we are not out of bounds
                        If xCnt <> -1 And xCnt <> MaxX And yCnt <> -1 And yCnt < MaxY Then
                            'Make sure it's not on the closed list
                            If Map(xCnt, yCnt).OCList <> inClosed Then
                                'Make sure no wall
                                If Map(xCnt, yCnt).Wall = False Then
                                    If Map(xCnt, yCnt).OCList <> inOpened Then
                                        'Calculate the GCost
                                        If Math.Abs(xCnt - ParentX) = 1 And Math.Abs(yCnt - ParentY) = 1 Then
                                            Map(xCnt, yCnt).GCost = Map(ParentX, ParentY).GCost + 14
                                        Else
                                            Map(xCnt, yCnt).GCost = Map(ParentX, ParentY).GCost + 10
                                        End If

                                        'Calculate the HCost
                                        Map(xCnt, yCnt).HCost = 10 * (Math.Abs(xCnt - EndX) + Math.Abs(yCnt - EndY))
                                        Map(xCnt, yCnt).FCost = (Map(xCnt, yCnt).GCost + Map(xCnt, yCnt).HCost)

                                        'Add the parent value
                                        Map(xCnt, yCnt).ParentX = ParentX
                                        Map(xCnt, yCnt).ParentY = ParentY

                                        'Add the item to the heap
                                        Heap.Add(Map(xCnt, yCnt).FCost, xCnt, yCnt)

                                        'Add the item to the open list
                                        Map(xCnt, yCnt).OCList = inOpened

                                    Else
                                        'We will check for better value
                                        Dim AddedGCost As Int16
                                        If Math.Abs(xCnt - ParentX) = 1 And Math.Abs(yCnt - ParentY) = 1 Then
                                            AddedGCost = 14
                                        Else
                                            AddedGCost = 10
                                        End If
                                        Dim tempCost As Int16 = Map(ParentX, ParentY).GCost + AddedGCost

                                        If tempCost < Map(xCnt, yCnt).GCost Then
                                            Map(xCnt, yCnt).GCost = tempCost
                                            Map(xCnt, yCnt).ParentX = ParentX
                                            Map(xCnt, yCnt).ParentY = ParentY
                                            If Map(xCnt, yCnt).OCList = inOpened Then
                                                Dim NewCost As Int16 = Map(xCnt, yCnt).HCost + Map(xCnt, yCnt).GCost
                                                Heap.Add(NewCost, xCnt, yCnt)
                                            End If
                                        End If
                                    End If

                                End If
                            End If
                        End If
                    Next
                Next
            Else
                PathFound = False
                PathHunt = False
                Return Nothing
            End If

            'If we find a path
            If Map(EndX, EndY).OCList = inOpened Then
                PathFound = True
                PathHunt = False
            End If

        End While

        If PathFound Then
            Dim Points As New List(Of Point)
            Dim tX As Int16 = EndX
            Dim tY As Int16 = EndY
            Points.Add(New Point(tX, tY))
            Dim i As Integer = 1

            While True
                Dim sX As Int16 = Map(tX, tY).ParentX
                Dim sY As Int16 = Map(tX, tY).ParentY

                If (i Mod dist = 0) Then
                    Points.Add(New Point(sX, sY))
                End If

                tX = sX
                tY = sY
                If tX = StartX And tY = StartY Then Exit While
                i += 1
            End While
            Points.Reverse()
            Return Points
        End If

        Return Nothing
    End Function

    Public Function PathToWaypoint(ByVal Mapinfo As Pathing.MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
        If Mapinfo.MapSizeX = Nothing Then Return Nothing
        Dim WpList() As Integer = {&H77, &H9D, &H9C, &H143, &H120, &H192, &HED, &H144, &H18E, &HEE, &H1AD, &H1F0, &H1FF, &H1EE}
        Dim StartPoint = MyPosition()
        'Check if our map contains one of those ids.
        For i As Integer = 0 To WpList.Length - 1
            If Mapinfo.Objects.Keys.Contains(WpList(i)) Then
                If Walk Then
                    Return RelativeToAbs(GetWalkPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Objects.ItemBykey(WpList(i)), Mapinfo), Mapinfo), Mapinfo)
                Else
                    Return RelativeToAbs(GetTeleportPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Objects.ItemBykey(WpList(i)), Mapinfo), Mapinfo, Distance), Mapinfo)
                End If
            End If
        Next
        Return Nothing
    End Function
    Public Function PathToObject(ByVal ObjectId As Integer, ByVal Mapinfo As Pathing.MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
        If Mapinfo.MapSizeX = Nothing Then Return Nothing
        Dim StartPoint = MyPosition()
        If Mapinfo.Objects.Keys.Contains(ObjectId) Then
            If Walk Then
                Return RelativeToAbs(GetWalkPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Objects.ItemBykey(ObjectId), Mapinfo), Mapinfo), Mapinfo)
            Else
                Return RelativeToAbs(GetTeleportPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Objects.ItemBykey(ObjectId), Mapinfo), Mapinfo, Distance), Mapinfo)
            End If
        End If
        Return Nothing
    End Function
    Public Function PathToNpc(ByVal NPCId As Integer, ByVal Mapinfo As Pathing.MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
        If Mapinfo.MapSizeX = Nothing Then Return Nothing
        Dim StartPoint = MyPosition()
        If Mapinfo.Npcs.Keys.Contains(NPCId) Then
            If Walk Then
                Return RelativeToAbs(GetWalkPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Npcs.ItemBykey(NPCId), Mapinfo), Mapinfo), Mapinfo)
            Else
                Return RelativeToAbs(GetTeleportPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Npcs.ItemBykey(NPCId), Mapinfo), Mapinfo, Distance), Mapinfo)
            End If
        End If
        Return Nothing
    End Function
    Public Function PathToLevel(ByVal LevelId As Integer, ByVal Mapinfo As Pathing.MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
        If Mapinfo.MapSizeX = Nothing Then Return Nothing
        Dim StartPoint = MyPosition()

        If Mapinfo.Exits.Keys.Contains(LevelId) Then

            If Walk Then
                Return RelativeToAbs(GetWalkPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Exits.ItemBykey(LevelId), Mapinfo), Mapinfo), Mapinfo)
            Else
                Return RelativeToAbs(GetTeleportPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(Mapinfo.Exits.ItemBykey(LevelId), Mapinfo), Mapinfo, Distance), Mapinfo)
            End If
        Else
            Dim ptExitPoints(Mapinfo.MapSizeX, 2) As Point
            Dim nTotalPoints As Integer = 0
            Dim nCurrentExit As Integer = 0

            Dim I As Integer = 0

            While I < Mapinfo.MapSizeX
                If Mapinfo.Bytes(I, 0) Mod 2 = 0 Then
                    ptExitPoints(nTotalPoints, 0).X = I
                    ptExitPoints(nTotalPoints, 0).Y = 0
                    I += 1
                    While I < Mapinfo.MapSizeX
                        If Mapinfo.Bytes(I, 0) Mod 2 = 0 Then
                            ptExitPoints(nTotalPoints, 1).X = I - 1
                            ptExitPoints(nTotalPoints, 1).Y = 0
                            Exit While
                        End If
                        I += 1
                    End While
                    nTotalPoints += 1
                End If
                I += 1
            End While
            I = 0

            While I < Mapinfo.MapSizeX
                If Mapinfo.Bytes(I, Mapinfo.MapSizeY - 1) Mod 2 = 0 Then
                    ptExitPoints(nTotalPoints, 0).X = I
                    ptExitPoints(nTotalPoints, 0).Y = Mapinfo.MapSizeY - 1
                    I += 1
                    While I < Mapinfo.MapSizeX
                        If Mapinfo.Bytes(I, Mapinfo.MapSizeY - 1) Mod 2 = 0 Then
                            ptExitPoints(nTotalPoints, 1).X = I - 1
                            ptExitPoints(nTotalPoints, 1).Y = Mapinfo.MapSizeY - 1
                            Exit While
                        End If
                        I += 1
                    End While
                    nTotalPoints += 1
                End If
                I += 1
            End While
            I = 0
            While I < Mapinfo.MapSizeY
                If Mapinfo.Bytes(0, I) Mod 2 = 0 Then
                    ptExitPoints(nTotalPoints, 0).X = 0
                    ptExitPoints(nTotalPoints, 0).Y = I
                    I += 1
                    While I < Mapinfo.MapSizeY
                        If Mapinfo.Bytes(0, I) Mod 2 = 0 Then
                            ptExitPoints(nTotalPoints, 1).X = 0
                            ptExitPoints(nTotalPoints, 1).Y = I - 1
                            Exit While
                        End If
                        I += 1
                    End While
                    nTotalPoints += 1
                End If
                I += 1
            End While
            I = 0

            While I < Mapinfo.MapSizeY

                If Mapinfo.Bytes(Mapinfo.MapSizeX - 1, I) Mod 2 = 0 Then
                    ptExitPoints(nTotalPoints, 0).X = Mapinfo.MapSizeX - 1
                    ptExitPoints(nTotalPoints, 0).Y = I
                    I += 1
                    While I < Mapinfo.MapSizeY
                        If Mapinfo.Bytes(Mapinfo.MapSizeX - 1, I) Mod 2 = 0 Then
                            ptExitPoints(nTotalPoints, 1).X = Mapinfo.MapSizeX - 1
                            ptExitPoints(nTotalPoints, 1).Y = I - 1
                            Exit While
                        End If
                        I += 1
                    End While
                    nTotalPoints += 1
                End If
                I += 1
            End While

            Dim ptCenters(nTotalPoints) As Point
            I = 0
            While I < nTotalPoints

                Dim nXDiff As Integer = ptExitPoints(I, 1).X - ptExitPoints(I, 0).X
                Dim nYDiff As Integer = ptExitPoints(I, 1).Y - ptExitPoints(I, 0).Y
                Dim nXCenter As Integer = 0
                Dim nYCenter As Integer = 0

                If nXDiff > 0 Then
                    If nXDiff Mod 2 > 0 Then
                        nXCenter = ptExitPoints(I, 0).X + ((nXDiff - (nXDiff Mod 2)) / 2)
                    Else
                        nXCenter = ptExitPoints(I, 0).X + (nXDiff / 2)
                    End If
                End If
                If nYDiff > 0 Then
                    If nYDiff Mod 2 > 0 Then
                        nYCenter = ptExitPoints(I, 0).Y + ((nYDiff - (nYDiff Mod 2)) / 2)
                    Else
                        nYCenter = ptExitPoints(I, 0).Y + (nYDiff / 2)
                    End If
                End If
                If nXCenter = 0 Then
                    ptCenters(I).X = ptExitPoints(I, 0).X
                Else
                    ptCenters(I).X = nXCenter
                End If
                If nYCenter = 0 Then
                    ptCenters(I).Y = ptExitPoints(I, 0).Y
                Else
                    ptCenters(I).Y = nYCenter
                End If
                I += 1
            End While

            If Mapinfo.LevelsNear.Keys.Contains(LevelId) Then
                Dim iter As Exit_T = Mapinfo.LevelsNear.ItemBykey(LevelId)
                Dim J As Integer = 0
                While j < nTotalPoints
                    If (ptCenters(J).X + Mapinfo.MapPosX * 5) >= iter.First.X - 5 And (ptCenters(J).X + Mapinfo.MapPosX * 5) - 5 <= (iter.First.X + iter.Second.X) Then
                        If (ptCenters(J).Y + Mapinfo.MapPosY * 5) >= iter.First.Y - 5 And (ptCenters(J).Y + Mapinfo.MapPosY * 5) - 5 <= (iter.First.Y + iter.Second.Y) Then
                            Dim EndPoint As New Point(ptCenters(J).X + Mapinfo.MapPosX * 5, ptCenters(J).Y + Mapinfo.MapPosY * 5)
                            If Walk Then
                                Return RelativeToAbs(GetWalkPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(EndPoint, Mapinfo), Mapinfo), Mapinfo)
                            Else
                                Return RelativeToAbs(GetTeleportPath(AbsToRelative(StartPoint, Mapinfo), AbsToRelative(EndPoint, Mapinfo), Mapinfo, Distance), Mapinfo)
                            End If
                        End If
                    End If
                    J += 1
                End While
            End If
        End If
        Return Nothing
    End Function
#End Region

#End Region
End Class


