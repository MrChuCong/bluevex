Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Specialized

Namespace Memory

    Public Class Pathing

        Public Structure MapInfo_t
            Public LevelsNear As Structures.Dicto(Of Long, Exit_T)
            Public Npcs As Structures.Dicto(Of Long, Entities)
            Public Objects As Structures.Dicto(Of Long, Entities)
            Public Exits As Structures.Dicto(Of Long, Point)
            Public MapPosX As Integer
            Public MapPosY As Integer
            Public MapSizeX As Integer
            Public MapSizeY As Integer
            Public LevelNo As Integer
            Public Bytes(,) As Integer
            Public ClientPID As Integer

            Structure Exit_T
                Dim First As Point
                Dim Second As Point
            End Structure

        End Structure

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
        Private Const MEM_COMMIT = &H1000&
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
            Dim DwUnitAddr As UInteger
            Dim DwRoomAddr As UInteger
            'Dim dwPlayerId As UInteger
            Dim dwActAddr As UInteger
        End Structure

        Public Structure Entities
            Dim Pos As Point
            Dim ID As UInteger
        End Structure

#End Region

        Dim Playerinfo As Playerinfo_t
        Dim m_map As CMatrix

        Dim Mapinfo As New Dictionary(Of Integer, MapInfo_t)

        Dim D2client As System.Diagnostics.ProcessModule
        Dim D2common As System.Diagnostics.ProcessModule

        Dim LevelNo As Integer

        Dim MemEditor As MemEditor


        Sub New(Optional ByVal DiabloPID As Integer = 0)
            MemEditor = New MemEditor(DiabloPID)
            D2client = MemEditor.GetModule("D2Client.dll")
            D2common = MemEditor.GetModule("D2Common.dll")
        End Sub

#Region "Private"

        <StructLayout(LayoutKind.Explicit)> _
        Private Structure AsmStub

            <FieldOffset(0)> _
                Dim _Push1 As Byte
            <FieldOffset(1)> _
                Dim Param1 As Integer

            <FieldOffset(5)> _
                Dim _Push2 As Byte
            <FieldOffset(6)> _
                Dim Param2 As Integer

            <FieldOffset(10)> _
                Dim _Push3 As Byte
            <FieldOffset(11)> _
                Dim Param3 As Integer

            <FieldOffset(15)> _
                Dim _Push4 As Byte
            <FieldOffset(16)> _
                Dim Param4 As Integer

            <FieldOffset(20)> _
                Dim _Push5 As Byte
            <FieldOffset(21)> _
                Dim Param5 As Integer

            <FieldOffset(25)> _
                Dim _PushAddr As Byte
            <FieldOffset(26)> _
                Dim FuncAddr As Integer

            <FieldOffset(30)> _
                Dim _FF As Byte
            <FieldOffset(31)> _
                Dim _D0 As Byte
            <FieldOffset(32)> _
                Dim _C3 As Byte

            Sub New(ByVal RoomAddr As Integer, ByVal Y As Integer, ByVal X As Integer, ByVal LevelNo As Integer, ByVal dwActAddr As Integer, ByVal FuncAddr As Integer)

                _Push1 = &H68
                Param1 = RoomAddr

                _Push2 = &H68
                Param2 = Y

                _Push3 = &H68
                Param3 = X

                _Push4 = &H68
                Param4 = LevelNo

                _Push5 = &H68
                Param5 = dwActAddr

                _PushAddr = &HB8
                Me.FuncAddr = FuncAddr

                _FF = &HFF
                _D0 = &HD0
                _C3 = &HC3

            End Sub
        End Structure

        Private Sub AddRoomData(ByVal LevelNo As Long, ByVal X As Long, ByVal Y As Long)
            Try
                Dim AsmStub As New AsmStub(Playerinfo.DwRoomAddr, Y, X, LevelNo, Playerinfo.dwActAddr, D2common.BaseAddress.ToInt32 + &H568F0)


                Dim Address As Integer = VirtualAllocEx(MemEditor.netProcHandle.Handle.ToInt32, Nothing, (Marshal.SizeOf(AsmStub)), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
                MemEditor.WriteMemory(Address, Tools.StructToBytes(AsmStub))
                Dim Phandle As Long = CreateRemoteThread(MemEditor.netProcHandle.Handle.ToInt32, 0, 4, Address, Nothing, Nothing, Nothing)
                WaitForSingleObject(Phandle, 10000)
                CloseHandle(Phandle)
                VirtualFreeEx(MemEditor.netProcHandle.Handle.ToInt32, Address, (Marshal.SizeOf(AsmStub)), MEM_RELEASE)
            Catch
            End Try
        End Sub

        Private Sub RemoveRoomData(ByVal LevelNo As Long, ByVal X As Long, ByVal Y As Long)

            Dim AsmStub As New AsmStub(Playerinfo.DwRoomAddr, Y, X, LevelNo, Playerinfo.dwActAddr, D2common.BaseAddress.ToInt32 + &H56830)

            Dim Address As Integer = VirtualAllocEx(MemEditor.netProcHandle.Handle.ToInt32, Nothing, (Marshal.SizeOf(AsmStub)), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
            MemEditor.WriteMemory(Address, Tools.StructToBytes(AsmStub))
            Dim Phandle As Long = CreateRemoteThread(MemEditor.netProcHandle.Handle.ToInt32, 0, Len(New Int32), Address, Nothing, Nothing, Nothing)
            WaitForSingleObject(Phandle, 10000)
            CloseHandle(Phandle)
            VirtualFreeEx(MemEditor.netProcHandle.Handle.ToInt32, Address, (Marshal.SizeOf(AsmStub)), MEM_RELEASE)

        End Sub

        Private Function InscribeInArray(ByVal Array As Byte(), ByVal Index As Integer, ByVal Value As Object) As Byte()

            Dim BufferByte As Byte() = BitConverter.GetBytes(Value)
            Dim ArrayBuf As Byte() = Array

            For i As Integer = Index To (BufferByte.Length + Index) - 1
                If BufferByte(i - Index) <> 0 Then
                    ArrayBuf(i) = BufferByte(i - Index)
                End If
            Next
            Return ArrayBuf
        End Function

#End Region

#Region "Public"

        Public Function GetMapFromMemory() As MapInfo_t
            Dim Dwtemp As Long

            '1.12
            'D2Client + PlayerUnit
            Playerinfo.DwUnitAddr = MemEditor.ReadMemoryLong(D2client.BaseAddress.ToInt32 + &H11C3D0, 4)

            'UnitAny - dwUnitId
            'Playerinfo.dwPlayerId = MemEditor.ReadMemoryInt(Playerinfo.DwUnitAddr + &HC)

            'UnitAny - pAct
            Playerinfo.dwActAddr = MemEditor.ReadMemoryLong(Playerinfo.DwUnitAddr + &H1C, 4)

            'UnitAny - pPath
            Dwtemp = MemEditor.ReadMemoryLong(Playerinfo.DwUnitAddr + &H2C, 4)

            'pPath - pRoom1
            Playerinfo.DwRoomAddr = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)

            'Map Information
            'UnitAny - pPath
            Dwtemp = MemEditor.ReadMemoryLong(Playerinfo.DwUnitAddr + &H2C, 4)

            'pPath - pRoom1
            Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)

            'Room1 - pRoom2
            Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H70, 4)

            'Room2 - pLevel
            Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H0, 4)


            'Put the mapinfo in a buffer so we can easily change datas.

            Dim BufferMapInfo As New MapInfo_t
            'Initialize the Arrays
            BufferMapInfo.LevelsNear = New Structures.Dicto(Of Long, MapInfo_t.Exit_T)
            BufferMapInfo.Exits = New Structures.Dicto(Of Long, Point)
            BufferMapInfo.Npcs = New Structures.Dicto(Of Long, Entities)
            BufferMapInfo.Objects = New Structures.Dicto(Of Long, Entities)

            'Save the Client's ID
            BufferMapInfo.ClientPID = MemEditor.netProcHandle.Id

            'Get the General Informations of the map.

            'pLevel - dwLevelNo
            BufferMapInfo.LevelNo = MemEditor.ReadMemoryLong(Dwtemp + &H94, 4)

            'pLevel - dwPosX
            BufferMapInfo.MapPosX = MemEditor.ReadMemoryLong(Dwtemp + &H6C, 4)

            'pLevel - dwPosY
            BufferMapInfo.MapPosY = MemEditor.ReadMemoryLong(Dwtemp + &H70, 4)

            'pLevel - dwSizeX
            BufferMapInfo.MapSizeX = MemEditor.ReadMemoryLong(Dwtemp + &H74, 4)

            'pLevel - dwSizeY
            BufferMapInfo.MapSizeY = MemEditor.ReadMemoryLong(Dwtemp + &H78, 4)

            'If the map has already been loaded
            If Mapinfo.ContainsKey(LevelNo) Then
                'Overwrite it
                Mapinfo(LevelNo) = BufferMapInfo
            Else
                'Create it
                Mapinfo.Add(LevelNo, BufferMapInfo)
            End If

            m_map = New CMatrix(Mapinfo(LevelNo).MapSizeX * 5, Mapinfo(LevelNo).MapSizeY * 5, MAP_DATA_INVALID)

            'Preparing to Loop trought all Rooms2

            'pLevel - pRoom2First
            Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H21C, 4)

            ''''''''''''''''''''''''''''''''
            '''''''' Get the Units '''''''''
            ''''''''''''''''''''''''''''''''
            While Dwtemp
                Dim Exitt As Integer = 0

                Dim DwRoom As Long = Nothing
                Dim MapX As Long = Nothing
                Dim MapY As Long = Nothing

                Dim AddedRoom As Boolean = False

                'Room2 - pRoom1
                DwRoom = MemEditor.ReadMemoryLong(Dwtemp + &HD8, 4)

                'Room2 - dwPosX
                MapX = MemEditor.ReadMemoryLong(Dwtemp + &H2C, 4)

                'Room2 - dwPosY
                MapY = MemEditor.ReadMemoryLong(Dwtemp + &H30, 4)

                If DwRoom = 0 Then
                    AddedRoom = True
                    AddRoomData(Mapinfo(LevelNo).LevelNo, MapX, MapY)
                    'Room2 - pRoom1
                    DwRoom = MemEditor.ReadMemoryLong(Dwtemp + &HD8, 4)
                End If

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ''''''''''''''''''''''''''Find exits Code'''''''''''''''''''''
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim dwRoomsNear As Int32
                Dim pRoom2Near As Int32

                'Room2 - dwRoomsNear
                dwRoomsNear = MemEditor.ReadMemoryLong(Dwtemp + &H8, 4)

                'Room2 - pRoom2Near
                pRoom2Near = MemEditor.ReadMemoryLong(Dwtemp + &H10, 4)

                For i2 As Integer = 0 To dwRoomsNear - 1
                    Dim pNear As Int32
                    Dim pLevel As Int32
                    Dim dwLevelNo As Int32
                    Dim dwPosX As Int32
                    Dim dwPosY As Int32
                    Dim dwSizeX As Int32
                    Dim dwSizeY As Int32

                    'Room2 - ???  *** To Check Doesn't seem wrong
                    pNear = MemEditor.ReadMemoryLong(pRoom2Near + (i2 * 4), 4)

                    'Room2 - pLevel
                    pLevel = MemEditor.ReadMemoryLong(pNear + &H0, 4)

                    'Level - dwLevelNo
                    dwLevelNo = MemEditor.ReadMemoryLong(pLevel + &H94, 4)
                    'Level - PosX
                    dwPosX = MemEditor.ReadMemoryLong(pLevel + &H6C, 4)
                    'Level - PosY
                    dwPosY = MemEditor.ReadMemoryLong(pLevel + &H70, 4)
                    'Level - SizeX
                    dwSizeX = MemEditor.ReadMemoryLong(pLevel + &H74, 4)
                    'Level - SizeY
                    dwSizeY = MemEditor.ReadMemoryLong(pLevel + &H78, 4)

                    Dim Newlevel As MapInfo_t.Exit_T
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

                'Room2 - pPreset
                pUnit = MemEditor.ReadMemoryLong(Dwtemp + &HC4, 4)


                While (pUnit <> Nothing)

                    Dim dwType As Int32
                    Dim dwClassId As Int32
                    Dim dwPosX As Int32
                    Dim dwPosY As Int32

                    'PresetUnit - dwType
                    dwType = MemEditor.ReadMemoryLong(pUnit + &H1C, 4)
                    'PresetUnit - dwTxtFileNo
                    dwClassId = MemEditor.ReadMemoryLong(pUnit + &H0, 4)
                    'PresetUnit - dwPosX
                    dwPosX = MemEditor.ReadMemoryLong(pUnit + &HC, 4)
                    'PresetUnit - dwPosY
                    dwPosY = MemEditor.ReadMemoryLong(pUnit + &H14, 4)

                    If dwType = UNIT_TYPE_NPC Then
                        If Mapinfo(LevelNo).Npcs.ContainsKey(dwClassId) = False Then
                            Dim Buf As New Entities
                            Buf.Pos = New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY)
                            Buf.ID = dwClassId
                            Mapinfo(LevelNo).Npcs.Add(dwClassId, Buf)
                        End If
                    ElseIf dwType = UNIT_TYPE_OBJECT Then
                        If Mapinfo(LevelNo).Objects.ContainsKey(dwClassId) = False Then
                            Dim Buf As New Entities
                            Buf.Pos = New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY)
                            Buf.ID = dwClassId
                            Mapinfo(LevelNo).Objects.Add(dwClassId, Buf)
                        End If

                    ElseIf dwType = UNIT_TYPE_TILE Then
                        Dim pRoomTile As Int32

                        'Room2 - pRoomTiles To Check
                        pRoomTile = MemEditor.ReadMemoryLong(Dwtemp + &HC, 4)

                        While pRoomTile <> Nothing
                            Dim pNum As Int32
                            Dim nNum As Int32

                            'RoomTile - nNum To Check
                            pNum = MemEditor.ReadMemoryLong(pRoomTile + &H0, 4)
                            nNum = MemEditor.ReadMemoryLong(pNum, 4)

                            If nNum = dwClassId Then
                                Dim pRoom2 As Int32
                                Dim pLevel As Int32
                                Dim dwLevelNo As Int32

                                'RoomTile - pRoom2  To Check
                                pRoom2 = MemEditor.ReadMemoryLong(pRoomTile + &H4, 4)
                                'Room2 - pLevel
                                pLevel = MemEditor.ReadMemoryLong(pRoom2 + &H0, 4)
                                'Level - dwLevelNo
                                dwLevelNo = MemEditor.ReadMemoryLong(pLevel + &H94, 4)

                                If Mapinfo(LevelNo).Exits.Keys.Contains(dwLevelNo) = False Then
                                    Mapinfo(LevelNo).Exits.Add(dwLevelNo, New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY))
                                Else
                                    Mapinfo(LevelNo).Exits.ItemBykey(dwLevelNo) = New Point(MapX * 5 + dwPosX, MapY * 5 + dwPosY)
                                End If

                            End If
                            'RoomTile - pNext
                            pRoomTile = MemEditor.ReadMemoryLong(pRoomTile + &H10, 4)
                        End While

                    End If
                    'PresetUnit - pPresetNext
                    pUnit = MemEditor.ReadMemoryLong(pUnit + &H18, 4)
                End While

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '''''''''''''''''''Come back to the function''''''''''''''''''
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                'Room1 - pColl
                Dim dwCol As Int32
                dwCol = MemEditor.ReadMemoryLong(DwRoom + &H50, 4)

                Dim CollMapByte As Byte()
                Dim pcol As CollMap_t

                'CollMap - struct
                CollMapByte = MemEditor.ReadMemoryAOB(dwCol, Marshal.SizeOf(GetType(CollMap_t)))

                pcol = Tools.BytesToStruct(CollMapByte, pcol.GetType)

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
                'Room2 - pRoom2Other
                Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &HD4, 4)
            End While

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
            'Map not existing, Return's empty
            Return New MapInfo_t
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

        Public Function BitmapFromMapInfo(ByVal MapInfo As MapInfo_t, ByVal Color As Drawing.Color) As Bitmap

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
                        b.SetPixel(x, y, Color)
                    End If
                Next
            Next
            Return b
        End Function

#End Region

#Region "Class"
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
        Public Function RelativeToAbs(ByVal Point As Point, ByVal mapinfo As MapInfo_t) As Point
            RelativeToAbs.X = Point.X + mapinfo.MapPosX * 5
            RelativeToAbs.Y = Point.Y + mapinfo.MapPosY * 5
            Return RelativeToAbs
        End Function
        'List
        Public Function RelativeToAbs(ByVal Point As List(Of Point), ByVal Mapinfo As MapInfo_t) As List(Of Point)
            RelativeToAbs = New List(Of Point)
            If Not Point Is Nothing Then
                'Each point are transfer into relative
                For i As Integer = 0 To Point.Count - 1
                    RelativeToAbs.Add(RelativeToAbs(Point(i), Mapinfo))
                Next
            End If
            Return RelativeToAbs
        End Function
        'Single Point
        Public Function AbsToRelative(ByVal Point As Point, ByVal Mapinfo As MapInfo_t) As Point
            AbsToRelative.X = Math.Abs(Point.X - Mapinfo.MapPosX * 5)
            AbsToRelative.Y = Math.Abs(Point.Y - Mapinfo.MapPosY * 5)
            Return AbsToRelative
        End Function
        'List
        Public Function AbsToRelative(ByVal Point As List(Of Point), ByVal Mapinfo As MapInfo_t) As List(Of Point)
            AbsToRelative = New List(Of Point)
            If Not Point Is Nothing Then
                'Each point are transfer into relative
                For i As Integer = 0 To Point.Count - 1
                    AbsToRelative.Add(AbsToRelative(Point(i), Mapinfo))
                Next
            End If
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

        Public Function GetWalkPath(ByVal StartPoint As Point, ByVal EndPoint As Point, ByVal Mapinfo As MapInfo_t) As List(Of Point)

            Dim Map As CellData(,) = MapInfoToCellData(Mapinfo)
            Dim Heap As New BinaryHeap()
            Dim StartX, StartY, EndX, EndY As Int16

            StartPoint = AbsToRelative(StartPoint, Mapinfo)
            EndPoint = AbsToRelative(EndPoint, Mapinfo)

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
                Return RelativeToAbs(Points, Mapinfo)
            End If

            Return Nothing
        End Function

        Public Function PathToWaypoint(ByVal Mapinfo As MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
            If Mapinfo.MapSizeX = Nothing Then Return Nothing
            Dim WpList() As Integer = {&H77, &H9D, &H9C, &H143, &H120, &H192, &HED, &H144, &H18E, &HEE, &H1AD, &H1F0, &H1FF, &H1EE}
            'Switch so User choose his Client Instance.
            Dim StartPoint As Point = Wrapped.GetMyPosition(Mapinfo.ClientPID)

            'Check if our map contains one of those ids.
            For i As Integer = 0 To WpList.Length - 1
                If Mapinfo.Objects.Keys.Contains(WpList(i)) Then
                    If Walk Then
                        Return GetWalkPath(StartPoint, Mapinfo.Objects.ItemBykey(WpList(i)).Pos, Mapinfo)
                    Else
                        Return GetTeleportPath(StartPoint, Mapinfo.Objects.ItemBykey(WpList(i)).Pos, Mapinfo, Distance)
                    End If
                End If
            Next
            Return Nothing
        End Function
        Public Function PathToObject(ByVal ObjectId As Integer, ByVal Mapinfo As MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
            If Mapinfo.MapSizeX = Nothing Then Return Nothing
            Dim StartPoint As Point = Wrapped.GetMyPosition(Mapinfo.ClientPID)
            If Mapinfo.Objects.Keys.Contains(ObjectId) Then
                If Walk Then
                    Return GetWalkPath(StartPoint, Mapinfo.Objects.ItemBykey(ObjectId).Pos, Mapinfo)
                Else
                    Return GetTeleportPath(StartPoint, Mapinfo.Objects.ItemBykey(ObjectId).Pos, Mapinfo, Distance)
                End If
            End If
            Return Nothing
        End Function
        Public Function PathToNpc(ByVal NPCId As Integer, ByVal Mapinfo As MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
            If Mapinfo.MapSizeX = Nothing Then Return Nothing
            Dim StartPoint As Point = Wrapped.GetMyPosition(Mapinfo.ClientPID)
            If Mapinfo.Npcs.Keys.Contains(NPCId) Then
                If Walk Then
                    Return GetWalkPath(StartPoint, Mapinfo.Npcs.ItemBykey(NPCId).Pos, Mapinfo)
                Else
                    Return GetTeleportPath(StartPoint, Mapinfo.Npcs.ItemBykey(NPCId).Pos, Mapinfo, Distance)
                End If
            End If
            Return Nothing
        End Function

        Public Function PathToLevel(ByVal LevelId As Integer, ByVal Mapinfo As MapInfo_t, ByVal Walk As Boolean, Optional ByVal Distance As Integer = 40) As List(Of Point)
            If Mapinfo.MapSizeX = Nothing Then Return Nothing
            Dim StartPoint As Point = Wrapped.GetMyPosition(Mapinfo.ClientPID)

            If Mapinfo.Exits.Keys.Contains(LevelId) Then

                If Walk Then
                    Return GetWalkPath(StartPoint, Mapinfo.Exits.ItemBykey(LevelId), Mapinfo)
                Else
                    Return GetTeleportPath(StartPoint, Mapinfo.Exits.ItemBykey(LevelId), Mapinfo, Distance)
                End If
            Else
                Dim ptExitPoints(Mapinfo.MapSizeX * 2, 2) As Point
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
                    Dim iter As MapInfo_t.Exit_T = Mapinfo.LevelsNear.ItemBykey(LevelId)
                    Dim J As Integer = 0
                    While J < nTotalPoints

                        If (ptCenters(J).X + Mapinfo.MapPosX * 5) >= iter.First.X - 5 And (ptCenters(J).X + Mapinfo.MapPosX * 5) - 5 <= (iter.First.X + iter.Second.X) Then
                            If (ptCenters(J).Y + Mapinfo.MapPosY * 5) >= iter.First.Y - 5 And (ptCenters(J).Y + Mapinfo.MapPosY * 5) - 5 <= (iter.First.Y + iter.Second.Y) Then
                                Dim EndPoint As New Point(ptCenters(J).X + Mapinfo.MapPosX * 5, ptCenters(J).Y + Mapinfo.MapPosY * 5)


                                If Walk Then
                                    Dim ThePath As List(Of Point) = GetWalkPath(StartPoint, EndPoint, Mapinfo)

                                    'Find a Point IN the level.
                                    Dim ComparePos As Point = StartPoint - EndPoint
                                    If Math.Abs(ComparePos.X) > Math.Abs(ComparePos.Y) Then

                                        EndPoint.X -= (Math.Abs(ComparePos.X) / ComparePos.X) * 5
                                    Else
                                        EndPoint.Y -= (Math.Abs(ComparePos.Y) / ComparePos.Y) * 5
                                    End If
                                    ThePath.Add(EndPoint)
                                    Return ThePath
                                Else
                                    Dim ThePath As List(Of Point) = GetTeleportPath(StartPoint, EndPoint, Mapinfo, Distance)

                                    'Find a Point IN the level.
                                    Dim ComparePos As Point = StartPoint - EndPoint
                                    If Math.Abs(ComparePos.X) > Math.Abs(ComparePos.Y) Then

                                        EndPoint.X -= (Math.Abs(ComparePos.X) / ComparePos.X) * 5
                                    Else
                                        EndPoint.Y -= (Math.Abs(ComparePos.Y) / ComparePos.Y) * 5
                                    End If
                                    ThePath.Add(EndPoint)
                                    Return ThePath

                                End If
                            End If
                        End If
                        J += 1
                    End While
                End If
            End If
            Return Nothing
        End Function

#Region "Teleport Path"

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''''''''''''''''''''''''Abin's Teleport Path''''''''''''''''''''''''
        '''''''''''''''''''' Ported to .Net by Dezimtox'''''''''''''''''''''
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Private M_ptstart As Point
        Private M_ptEnd As Point
        Private Tp_Range As Integer
        Private M_pptable As Integer(,)

        Private m_nCX As Integer
        Private m_nCY As Integer
        Private Const RANGE_INVALID As Integer = 10000

        Private Enum PathState
            PATH_FAIL = 0 'Failed, error occurred or no available path
            PATH_CONTINUE = 1 'Path OK, destination not reached yet
            PATH_REACHED = 2
        End Enum

        Private Function CalculateDistance(ByVal X1 As Integer, ByVal Y1 As Integer, ByVal X2 As Integer, ByVal Y2 As Integer) As Integer
            Return Math.Sqrt((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2))
        End Function
        Private Function CalculateDistance(ByVal Point1 As Point, ByVal Point2 As Point) As Integer
            Return CalculateDistance(Point1.X, Point1.Y, Point2.X, Point2.Y)
        End Function

        Private Function IsValidIndex(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return x >= 0 And x < m_nCX And y >= 0 And y < m_nCY
        End Function
        Private Sub MakeDistanceTable(ByRef Mapinfo As MapInfo_t)
            ReDim M_pptable(Mapinfo.MapSizeX, Mapinfo.MapSizeY)
            For x As Integer = 0 To Mapinfo.MapSizeX - 1
                For y As Integer = 0 To Mapinfo.MapSizeY - 1
                    If Mapinfo.Bytes(x, y) Mod 2 = 0 Then
                        M_pptable(x, y) = CalculateDistance(x, y, M_ptEnd.X, M_ptEnd.Y)
                    Else
                        M_pptable(x, y) = RANGE_INVALID
                    End If
                Next
            Next
            M_pptable(M_ptEnd.X, M_ptEnd.Y) = 1
        End Sub
        Private Sub Block(ByVal pos As Point, ByVal nRange As Integer)
            nRange = Math.Max(nRange, 1)
            For i As Integer = pos.X - nRange To pos.X + nRange - 1
                For j As Integer = pos.Y - nRange To pos.Y + nRange - 1
                    If IsValidIndex(i, j) Then
                        M_pptable(i, j) = RANGE_INVALID
                    End If
                Next
            Next
        End Sub
        Private Function GetBestMove(ByRef pos As Point, Optional ByVal nAdjust As Integer = 2) As Integer
            If CalculateDistance(M_ptEnd, pos) <= Tp_Range Then
                pos = M_ptEnd
                Return PathState.PATH_REACHED 'We reached the destination
            End If

            If IsValidIndex(pos.X, pos.Y) = False Then
                Return PathState.PATH_FAIL 'fail
            End If

            Block(pos, nAdjust)

            Dim Px As Integer
            Dim Py As Integer
            Dim BestX As Integer
            Dim BestY As Integer

            Dim value As Integer = RANGE_INVALID

            For Px = pos.X - Tp_Range To pos.X + Tp_Range
                For Py = pos.Y - Tp_Range To pos.Y + Tp_Range
                    If Not IsValidIndex(Px, Py) Then
                        Continue For
                    End If
                    If M_pptable(Px, Py) < value And CalculateDistance(Px, Py, pos.X, pos.Y) <= Tp_Range Then
                        value = M_pptable(Px, Py)
                        BestX = Px
                        BestY = Py

                    End If
                Next

            Next
            If value >= RANGE_INVALID Then
                Return PathState.PATH_FAIL 'Pathing failed
            End If
            pos = New Point(BestX, BestY)
            Block(pos, nAdjust)
            Return PathState.PATH_CONTINUE 'Ok but not reached yet
        End Function
        Private Function GetRedundancy(ByVal lpPath As List(Of Point), ByVal pos As Point) As Integer
            If lpPath.Count = 0 Then Return -1
            For i As Integer = 1 To lpPath.Count - 1
                If CalculateDistance(lpPath(i).X, lpPath(i).Y, pos.X, pos.Y) <= Tp_Range / 2 Then
                    Return i
                End If
            Next
            Return -1
        End Function

        Public Function GetTeleportPath(ByVal ptStart As Point, ByVal ptEnd As Point, ByVal Mapinfo As MapInfo_t, ByVal Distance As Integer) As List(Of Point)
            Dim Path As New List(Of Point)

            M_ptstart = AbsToRelative(ptStart, Mapinfo)
            M_ptEnd = AbsToRelative(ptEnd, Mapinfo)

            Tp_Range = Distance

            m_nCX = Mapinfo.MapSizeX
            m_nCY = Mapinfo.MapSizeY
            MakeDistanceTable(Mapinfo)
            'Starting point Not in the Path.
            'Path.Add(ptStart)
            Dim dwFound As Integer = 1
            Dim Pos As Point = M_ptstart
            Dim bOK As Boolean
            Dim nRes As Integer = GetBestMove(Pos)
            While nRes <> PathState.PATH_FAIL
                If nRes = PathState.PATH_REACHED Then
                    bOK = True
                    Path.Add(M_ptEnd)
                    dwFound += 1
                    Exit While
                End If
                Dim nRedundancy As Integer = GetRedundancy(Path, Pos)
                If nRedundancy = -1 Then
                    Path.Add(Pos)
                    dwFound += 1
                Else
                    dwFound = nRedundancy + 1
                    Path.RemoveRange(dwFound, Path.Count - dwFound)
                End If
                nRes = GetBestMove(Pos)
            End While

            If Not bOK Then
                dwFound = 0
            End If

            Return RelativeToAbs(Path, Mapinfo)
        End Function
#End Region

#End Region

#End Region

    End Class

End Namespace