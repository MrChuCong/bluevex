''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''All Credits go to ApacheChief - The Code was Edited by DezimtoX''''''''''''''''
''''''''Original Thread: http://www.edgeofnowhere.cc/viewtopic.php?t=362418'''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Imports System
Imports System.Text
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Imports D2Data
Imports BlueVex.Memory

Namespace Memory

    Namespace ClientDetection
        Public Module z

            ''' <summary> 
            ''' Get the Client in which the player is at a certain position.
            ''' </summary> 
            Public Function ClientPIDFromPos(ByVal PositionFromPackets As Point) As Integer
                Dim Pos As Point = PositionFromPackets
                Dim MemEditor As New MemEditor

                Dim DiabloProcesses As Process() = Process.GetProcesses
                For Each Process As Process In DiabloProcesses
                    '***Don't change this, it works no matter the window name.
                    If Process.ProcessName = "Diablo II" Then
                        'Sometime the packets are not that accurate. Range of 7 Units.
                        If InRange(Wrapped.GetMyPosition(Process.Id), Pos, 7) Then
                            Return Process.Id
                        End If
                    End If
                Next
                Return Nothing
            End Function

            ''' <summary> 
            ''' Return all clients with the specified Status.
            ''' </summary> 
            Public Function ClientPIDFromStatus(ByVal Status As Wrapped.OutOfGameState) As List(Of Process)
                Dim Clients As New List(Of Process)

                Dim DiabloProcesses As Process() = Process.GetProcesses
                For Each Process As Process In DiabloProcesses
                    'If it's a diablo Client
                    If Process.ProcessName = "Diablo II" Then
                        'If It's on the status asked.
                        If Wrapped.GetClientStatus(Process.Id) = Status Then
                            Clients.Add(Process)
                        End If
                    End If
                Next
                Return Clients
            End Function

        End Module
    End Namespace

    Namespace Wrapped

        Public Module Functions
            Public Function GetMyPosition(Optional ByVal DiabloPID As Integer = 0) As Point

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule
                Dim dwUnitAddr As Int32
                Dim Dwtemp As Int32

                If MemEditor.Success = False Then Return Nothing

                d2client = MemEditor.GetModule("D2Client.dll")
                If d2client Is Nothing Then Return Nothing

                '1.11B
                'dwUnitAddr = MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11C1E0, 4)

                '1.12
                dwUnitAddr = MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11C3D0, 4)

                Dwtemp = MemEditor.ReadMemoryLong(dwUnitAddr + &H2C, 4)

                GetMyPosition.X = MemEditor.ReadMemoryShort(Dwtemp + &H2)
                GetMyPosition.Y = MemEditor.ReadMemoryShort(Dwtemp + &H6)
                MemEditor.mCloseProcess()

            End Function

            Public Function GetAreaID(Optional ByVal DiabloPID As Integer = 0) As Long

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule

                If MemEditor.Success = False Then Return Nothing

                d2client = MemEditor.GetModule("D2Client.dll")
                If d2client Is Nothing Then Return Nothing

                '1.12
                Return MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11BCEC, Len(New Int32))

                '1.11B

                'Dim dwUnitAddr As Int32
                'Dim Dwtemp As Int32
                'Dim AreaId As Long

                'dwUnitAddr = MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11C1E0, Len(New Int32))
                'Dwtemp = MemEditor.ReadMemoryLong(dwUnitAddr + &H2C, 4)

                ''Map Information
                'Dwtemp = MemEditor.ReadMemoryLong(dwUnitAddr + &H2C, 4)
                'Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)
                'Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H38, 4)
                'Dwtemp = MemEditor.ReadMemoryLong(Dwtemp + &H1C, 4)
                'AreaId = MemEditor.ReadMemoryLong(Dwtemp + &H14, 4)
                'MemEditor.mCloseProcess()
                'Return AreaId
            End Function

            Public Function GetPlayerStats(Optional ByVal DiabloPID As Integer = 0) As PlayerStats_t

                Dim Memoryzer As New Raw.Functions(DiabloPID)

                Dim RawPlayerStats As List(Of Raw.Structures.Stat) = Memoryzer.GetPlayerStats

                Dim PlayerStats As New PlayerStats_t


                For i As Integer = 0 To RawPlayerStats.Count - 1
                    With PlayerStats
                        Select Case RawPlayerStats(i).dwStatId

                            Case Raw.Structures.Stats.DEXTERITY
                                .Dex = RawPlayerStats(i).dwStatValue
                            Case Raw.Structures.Stats.ENERGY
                                .Energy = RawPlayerStats(i).dwStatValue
                            Case Raw.Structures.Stats.VITALITY
                                .Vit = RawPlayerStats(i).dwStatValue
                            Case Raw.Structures.Stats.STRENGTH
                                .Str = RawPlayerStats(i).dwStatValue

                            Case Raw.Structures.Stats.HITPOINTS
                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .life = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .life = RawPlayerStats(i).dwStatValue
                                End If

                            Case Raw.Structures.Stats.MAXHP
                                Dim HexValue As String
                                HexValue = Tools.HexToDec(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .BaseMaxLife = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .BaseMaxLife = RawPlayerStats(i).dwStatValue
                                End If
                            Case Raw.Structures.Stats.MANA

                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .Mana = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .Mana = RawPlayerStats(i).dwStatValue
                                End If

                            Case Raw.Structures.Stats.MAXMANA
                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .BaseMaxMana = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .BaseMaxMana = RawPlayerStats(i).dwStatValue
                                End If

                            Case Raw.Structures.Stats.STAMINA
                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .Stam = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .Stam = RawPlayerStats(i).dwStatValue
                                End If
                            Case Raw.Structures.Stats.MAXSTAMINA
                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
                                If HexValue.EndsWith("00") Then
                                    .BaseMaxStam = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .BaseMaxStam = RawPlayerStats(i).dwStatValue
                                End If

                            Case Raw.Structures.Stats.LEVEL
                                .level = RawPlayerStats(i).dwStatValue
                            Case Raw.Structures.Stats.EXPERIENCE
                                .Xp = RawPlayerStats(i).dwStatValue

                            Case Raw.Structures.Stats.NEWSKILLS
                                .SkillPointRem = RawPlayerStats(i).dwStatValue
                            Case Raw.Structures.Stats.STATPTS
                                .StatPointRem = RawPlayerStats(i).dwStatValue
                        End Select
                    End With
                Next

                Return PlayerStats
            End Function

            Public Function GetClientStatus(Optional ByVal DiabloPID As Integer = 0) As OutOfGameState
                Dim Memoryzer As New Raw.Functions(DiabloPID)
                Return Memoryzer.GetState()
            End Function

        End Module

        Public Enum OutOfGameState
            InGame
            Lobby
            CharacterSelect
            Login
            Start
        End Enum
        Public Enum UniqueMonster
            Bishibosh
            Bonebreak
            Coldcrow
            Rakanishu
            TreeheadWoodFist
            Griswold
            TheCountess
            PitspawnFouldog
            FlamespikeTheCrawler
            Boneash
            Radament
            BloodwitchTheWild
            Fangskin
            Beetleburst
            Leatherarm
            ColdwormTheBurrower
            FireEye
            DarkElder
            TheSummoner
            AncientKaaTheSoulless
            TheSmith
            WebMageTheBurning
            WitchDoctorEndugu
            Stormtree
            SarinaTheBattlemaid
            IcehawkRiftwing
            IsmailVilehand
            GelebFlamefinger
            BremmSparkfist
            ToorcIcefist
            WyandVoidfinger
            MafferDragonhand
            WingedDeath
            TheTormentor
            Taintbreeder
            RiftwraithTheCannibal
            InfectorofSouls
            LordDeSeis
            GrandVizierofChaos
            TheCowKing
            Corpsefire
            TheFeatureCreep
            SiegeBoss
            AncientBarbarian1
            AncientBarbarian2
            AncientBarbarian3
            AxeDweller
            BonesawBreaker
            DacFarren
            MegaflowRectifier
            EyebackUnleashed
            ThreashSocket
            Pindleskin
            SnapchipShatter
            AnodizedElite
            VinvearMolech
            SharpToothSayer
            MagmaTorquer
            BlazeRipper
            Frozenstein
            NihlathakBoss
            BaalSubject1
            BaalSubject2
            BaalSubject3
            BaalSubject4
            BaalSubject5
        End Enum

        Public Structure PlayerStats_t

            Dim Str As Integer
            Dim Energy As Integer
            Dim Dex As Integer
            Dim Vit As Integer

            Dim StatPointRem As Integer
            Dim SkillPointRem As Integer

            Dim life As Integer
            Dim BaseMaxLife As Integer

            Dim Mana As Integer
            Dim BaseMaxMana As Integer

            Dim Stam As Integer
            Dim BaseMaxStam As Integer

            Dim level As Integer
            Dim Xp As Long

        End Structure

        Public Enum Enchants

            ExtraStrong = 5
            ExtraFast = 6
            Cursed = 7
            MagicResistant = 8
            FireEnchanted = 9
            Champion = 16
            LightningEnchanted = 17
            ColdEnchanted = 18
            Thief = 24
            ManaBurn = 25
            Teleportation = 26
            SpectralHit = 27
            StoneSkin = 28
            MultipleShots = 29
            Ghostly = 36
            Fanatic = 37
            Possessed = 38
            Berserker = 39

        End Enum

    End Namespace


    Namespace Raw

        ''' <summary> 
        ''' Not User-Friendly.
        ''' </summary> 
        Public Class Functions


            Public Reader As MemEditor
            Dim D2client As System.Diagnostics.ProcessModule
            Dim D2Win As System.Diagnostics.ProcessModule

            Public Sub New(Optional ByVal DiabloPID As Integer = 0)
                Me.Reader = New MemEditor(DiabloPID)
                D2client = Reader.GetModule("D2Client.dll")
                D2Win = Reader.GetModule("D2Win.dll")
            End Sub

            Public Function IsInGame() As Boolean
                Return Me.GetPlayerUnit().dwUnitId <> 0
            End Function

            Public Function GetPlayerUnit() As Structures.UnitAny

                '1.11B
                'Dim PlayerPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2client.BaseAddress) + &H11C1E0)

                '1.12
                Dim PlayerPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2client.BaseAddress) + &H11C3D0)


                Return Tools.Tools.ByteToStruct(Reader.ReadMemoryAOB(PlayerPointer, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
            End Function

            Public Function GetPlayerStats() As List(Of Structures.Stat)
                Dim tempList As New List(Of Structures.Stat)()

                Dim statList As Structures.StatList = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerUnit.ptrStats, Marshal.SizeOf(GetType(Structures.StatList))), GetType(Structures.StatList))
                While True
                    For i As Integer = 0 To statList.wStatCount1 - 1
                        tempList.Add(Tools.ByteToStruct(Reader.ReadMemoryAOB((CInt(statList.pStat) + (i * 8)), Marshal.SizeOf(GetType(Structures.Stat))), GetType(Structures.Stat)))
                    Next

                    If statList.pNext = IntPtr.Zero Then
                        Exit While
                    End If
                    statList = Tools.ByteToStruct(Reader.ReadMemoryAOB(statList.pNext, Marshal.SizeOf(GetType(Structures.StatList))), GetType(Structures.StatList))
                End While

                Return tempList
            End Function

            Public Function GetCurrentAct() As Structures.Act
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerUnit.ptrAct, Marshal.SizeOf(GetType(Structures.Act))), GetType(Structures.Act))
            End Function

            Public Function GetPlayerPath() As Structures.Path
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerUnit.ptrPath, Marshal.SizeOf(GetType(Structures.Path))), GetType(Structures.Path))
            End Function

            Public Function GetPlayerRoom1() As Structures.Room1
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerPath.pRoom1, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
            End Function
            Public Function GetPlayerRoom2() As Structures.Room2
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerRoom1.pRoom2, Marshal.SizeOf(GetType(Structures.Room2))), GetType(Structures.Room2))
            End Function
            Public Function GetCurrentLevel() As Structures.Level
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerRoom2.pLevel, Marshal.SizeOf(GetType(Structures.Level))), GetType(Structures.Level))
            End Function

            Public Function GetAllRoom2() As List(Of Structures.Room2)
                Return Me.GetAllRoom2(Me.GetCurrentLevel)
            End Function

            Public Function GetAllRoom2(ByVal level As Structures.Level) As List(Of Structures.Room2)
                Dim room2List As New List(Of Structures.Room2)

                ' Save first room 
                Dim TempRoom As Structures.Room2 = Tools.ByteToStruct(Reader.ReadMemoryAOB(level.ptrRoom2First, Marshal.SizeOf(GetType(Structures.Room2))), GetType(Structures.Room2))

                room2List.Add(TempRoom)

                While True
                    ' Get next room 
                    TempRoom = Tools.ByteToStruct(Reader.ReadMemoryAOB(TempRoom.pRoom2Next, Marshal.SizeOf(GetType(Structures.Room2))), GetType(Structures.Room2))

                    room2List.Add(TempRoom)

                    If TempRoom.pRoom2Next = IntPtr.Zero Then
                        Exit While
                    End If
                End While

                Return room2List
            End Function

            Public Function GetPresetUnit(ByVal room2 As Structures.Room2) As Structures.PresetUnit
                Dim presetUnitPointer As IntPtr = room2.pPreset

                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(presetUnitPointer, Marshal.SizeOf(GetType(Structures.PresetUnit))), GetType(Structures.PresetUnit))

            End Function
            Public Function GetTiles(ByVal room2 As Structures.Room2) As List(Of Structures.RoomTile)
                Dim tileList As New List(Of Structures.RoomTile)

                ' Save first tile 
                Dim temptile As Structures.RoomTile = Tools.ByteToStruct(Reader.ReadMemoryAOB(room2.pRoomTiles, Marshal.SizeOf(GetType(Structures.RoomTile))), GetType(Structures.RoomTile))

                tileList.Add(temptile)

                While True
                    ' Get next room 
                    temptile = Tools.ByteToStruct(Reader.ReadMemoryAOB(temptile.pNext, Marshal.SizeOf(GetType(Structures.RoomTile))), GetType(Structures.RoomTile))

                    tileList.Add(temptile)

                    If temptile.pNext = IntPtr.Zero Then
                        Exit While
                    End If
                End While

                Return tileList
            End Function

            Public Function GetGameInfo() As Structures.GameInfo

                'Update this to 1.12

                'Dim GameInfoPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2client.BaseAddress) + 1103384)

                'Return Tools.ByteToStruct(Reader.ReadMemoryAOB(GameInfoPointer, Marshal.SizeOf(GetType(Structures.GameInfo))), GetType(Structures.GameInfo))

                Return Nothing

            End Function

            Public Function GetPlayerSkills() As Dictionary(Of SkillType, Integer)

                Dim info As Structures.Info = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetPlayerUnit.ptrInfo, Marshal.SizeOf(GetType(Structures.Info))), GetType(Structures.Info))
                Dim skillList As New Dictionary(Of SkillType, Integer)()
                ' Save first tile 
                Dim tempSkill As Structures.Skill = Tools.ByteToStruct(Reader.ReadMemoryAOB(info.pFirstSkill, Marshal.SizeOf(GetType(Structures.Skill))), GetType(Structures.Skill))
                Dim skillInfo As Structures.SkillInfo = Tools.ByteToStruct(Reader.ReadMemoryAOB(tempSkill.pSkillInfo, Marshal.SizeOf(GetType(Structures.SkillInfo))), GetType(Structures.SkillInfo))
                skillList.Add(DirectCast(skillInfo.wSkillId, SkillType), CInt(tempSkill.dwSkillLevel))
                While True
                    ' Get next Skill
                    tempSkill = Tools.ByteToStruct(Reader.ReadMemoryAOB(tempSkill.pNextSkill, Marshal.SizeOf(GetType(Structures.Skill))), GetType(Structures.Skill))
                    skillInfo = Tools.ByteToStruct(Reader.ReadMemoryAOB(tempSkill.pSkillInfo, Marshal.SizeOf(GetType(Structures.SkillInfo))), GetType(Structures.SkillInfo))
                    skillList.Add(DirectCast(skillInfo.wSkillId, SkillType), CInt(tempSkill.dwSkillLevel))
                    If tempSkill.pNextSkill = IntPtr.Zero Then
                        Exit While
                    End If
                End While
                Return skillList
            End Function

            ''' <summary> 
            ''' Must be in the same act as the unit 
            ''' </summary> 
            ''' <param name="code"></param> 
            ''' <returns></returns> 
            ''' 
            Public Function GetUnit(ByVal code As NPCCode) As Structures.UnitAny

                Dim room1 As Structures.Room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetCurrentAct.pRoom1, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))

                While True
                    Dim unit As Structures.UnitAny = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pUnitFirst, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    While True
                        If unit.dwTxtFileNo = CInt(code) Then
                            Return unit
                        End If

                        If unit.ptrListNext = IntPtr.Zero Then
                            Exit While
                        End If
                        unit = Tools.ByteToStruct(Reader.ReadMemoryAOB(unit.ptrListNext, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    End While

                    If room1.pRoomNext = IntPtr.Zero Then
                        Exit While
                    End If
                    room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pRoomNext, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                End While

                Return New Structures.UnitAny
            End Function

            ''' <summary> 
            ''' Must be in the same act as the unit 
            ''' </summary> 
            ''' <param name="code"></param> 
            ''' <returns></returns> 
            Public Function GetUnit(ByVal code As Wrapped.UniqueMonster) As Structures.UnitAny
                Dim room1 As Structures.Room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetCurrentAct.pRoom1, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))

                While True
                    Dim unit As Structures.UnitAny = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pUnitFirst, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))

                    While True
                        'If DirectCast(Me.GetMonsterData(unit).wUniqueNo, UniqueMonster) = code Then
                        If Me.GetMonsterData(unit).wUniqueNo = code Then
                            Return unit
                        End If

                        If unit.ptrListNext = IntPtr.Zero Then
                            Exit While
                        End If
                        unit = Tools.ByteToStruct(Reader.ReadMemoryAOB(unit.ptrListNext, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    End While

                    If room1.pRoomNext = IntPtr.Zero Then
                        Exit While
                    End If
                    room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pRoomNext, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                End While

                Return New Structures.UnitAny
            End Function

            ''' <summary> 
            ''' Gets monsters in the same act 
            ''' </summary> 
            ''' <returns></returns> 
            Public Function GetUnits() As List(Of Structures.UnitAny)
                Dim tempList As New List(Of Structures.UnitAny)

                Dim room1 As Structures.Room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetCurrentAct.pRoom1, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                While True
                    Dim unit As Structures.UnitAny = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pUnitFirst, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))

                    While True
                        tempList.Add(unit)

                        If unit.ptrListNext = IntPtr.Zero Then
                            Exit While
                        End If
                        unit = Tools.ByteToStruct(Reader.ReadMemoryAOB(unit.ptrListNext, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    End While

                    If room1.pRoomNext = IntPtr.Zero Then
                        Exit While
                    End If
                    room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pRoomNext, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                End While

                Return tempList
            End Function

            ''' <summary> 
            ''' Gets monsters in the same level of specified type 
            ''' </summary> 
            ''' <returns></returns> 
            ''' 
            Public Function GetUnits(ByVal code As NPCCode) As List(Of Structures.UnitAny)
                Dim tempList As New List(Of Structures.UnitAny)

                Dim room1 As Structures.Room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetCurrentAct.pRoom1, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                While True
                    Dim unit As Structures.UnitAny = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pUnitFirst, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    While True
                        If unit.dwTxtFileNo = code Then
                            'If DirectCast(unit.dwTxtFileNo, NPCCode) = code Then
                            tempList.Add(unit)
                        End If

                        If unit.ptrListNext = IntPtr.Zero Then
                            Exit While
                        End If
                        unit = Tools.ByteToStruct(Reader.ReadMemoryAOB(unit.ptrListNext, Marshal.SizeOf(GetType(Structures.UnitAny))), GetType(Structures.UnitAny))
                    End While

                    If room1.pRoomNext = IntPtr.Zero Then
                        Exit While
                    End If
                    room1 = Tools.ByteToStruct(Reader.ReadMemoryAOB(room1.pRoomNext, Marshal.SizeOf(GetType(Structures.Room1))), GetType(Structures.Room1))
                End While
                Return tempList
            End Function

            Public Function GetMonsterData(ByVal code As Wrapped.UniqueMonster) As Structures.MonsterData
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(Me.GetUnit(code).ptrMonsterData, Marshal.SizeOf(GetType(Structures.MonsterData))), GetType(Structures.MonsterData))
            End Function
            Public Function GetMonsterData(ByVal monster As Structures.UnitAny) As Structures.MonsterData
                Return Tools.ByteToStruct(Reader.ReadMemoryAOB(monster.ptrMonsterData, Marshal.SizeOf(GetType(Structures.MonsterData))), GetType(Structures.MonsterData))
            End Function

            Public Function GetMonsterData(ByVal monsters As List(Of Structures.UnitAny)) As List(Of Structures.MonsterData)
                Dim tempList As New List(Of Structures.MonsterData)()

                For Each monster As Structures.UnitAny In monsters
                    tempList.Add(Me.GetMonsterData(monster))
                Next

                Return tempList
            End Function

            Public Function GetControls() As List(Of Structures.Control)
                Dim tempList As New List(Of Structures.Control)

                '1.11B
                'Dim firstcontrolptr As IntPtr = Reader.ReadMemoryInt(CInt(D2Win.BaseAddress) + 132232)

                '1.12
                Dim firstcontrolptr As IntPtr = Reader.ReadMemoryInt(CInt(D2Win.BaseAddress) + &H5C718)

                Dim control As Structures.Control = Tools.ByteToStruct(Reader.ReadMemoryAOB(firstcontrolptr, Marshal.SizeOf(GetType(Structures.Control))), GetType(Structures.Control))

                While True
                    tempList.Add(control)

                    If control.pNext = IntPtr.Zero Then
                        Exit While
                    End If

                    control = Tools.ByteToStruct(Reader.ReadMemoryAOB(control.pNext, Marshal.SizeOf(GetType(Structures.Control))), GetType(Structures.Control))
                End While

                Return tempList
            End Function

            Public Function GetState() As Wrapped.OutOfGameState
                Dim controls As List(Of Structures.Control) = Me.GetControls()

                For Each control As Structures.Control In controls

                    Dim text As String = New System.Text.UnicodeEncoding().GetString(control.wText)

                    If text.Contains("CHANNEL") Then
                        Return Wrapped.OutOfGameState.Lobby
                    End If

                    If text.Contains("CONVERT TO") Then
                        Return Wrapped.OutOfGameState.CharacterSelect
                    End If

                    If text.Contains("LOG IN") Then
                        Return Wrapped.OutOfGameState.Login
                    End If

                    If text.Contains("GATEWAY") Then
                        Return Wrapped.OutOfGameState.Start

                    End If
                Next

                Return Wrapped.OutOfGameState.InGame
            End Function

            Public Function GetMonsterEnchants(ByVal code As NPCCode) As List(Of Byte)
                Return Me.GetMonsterEnchants(Me.GetUnit(code))
            End Function

            Public Function GetMonsterEnchants(ByVal code As Wrapped.UniqueMonster) As List(Of Byte)
                Return Me.GetMonsterEnchants(Me.GetUnit(code))
            End Function

            Public Function GetMonsterEnchants(ByVal monster As Structures.UnitAny) As List(Of Byte)
                Return Me.GetMonsterEnchants(Me.GetMonsterData(monster))
            End Function

            Public Function GetMonsterEnchants(ByVal monster As Structures.MonsterData) As List(Of Byte)
                'Dim tempList As New List(Of Enchant)
                Dim tempList As New List(Of Byte)
                For Each b As Byte In monster.anEnchants
                    'tempList.Add(DirectCast(b, Enchant))
                    tempList.Add(b)
                Next

                Return tempList
            End Function

        End Class

        ' Thanks to MyndFyre for this first example! 

        Namespace Structures

            Enum Stats
                STRENGTH = &H0
                ENERGY = &H10000
                DEXTERITY = &H20000
                VITALITY = &H30000
                STATPTS = &H40000
                NEWSKILLS = &H50000
                HITPOINTS = &H60000
                MAXHP = &H70000
                MANA = &H80000
                MAXMANA = &H90000
                STAMINA = &HA0000
                MAXSTAMINA = &HB0000
                LEVEL = &HC0000
                EXPERIENCE = &HD0000
                GOLD = &HE0000
                GOLDBANK = &HF0000
                ITEM_ARMOR_PERCENT = &H100000
                ITEM_MAXDAMAGE_PERCENT = &H110000
                ITEM_MINDAMAGE_PERCENT = &H120000
                TOHIT = &H130000
                TOBLOCK = &H140000
                MINDAMAGE = &H150000
                MAXDAMAGE = &H160000
                SECONDARY_MINDAMAGE = &H170000
                SECONDARY_MAXDAMAGE = &H180000
                DAMAGEPERCENT = &H190000
                MANARECOVERY = &H1A0000
                MANARECOVERYBONUS = &H1B0000
                STAMINARECOVERYBONUS = &H1C0000
                LASTEXP = &H1D0000
                NEXTEXP = &H1E0000
                ARMORCLASS = &H1F0000
                ARMORCLASS_VS_MISSILE = &H200000
                ARMORCLASS_VS_HTH = &H210000
                NORMAL_DAMAGE_REDUCTION = &H220000
                MAGIC_DAMAGE_REDUCTION = &H230000
                DAMAGERESIST = &H240000
                MAGICRESIST = &H250000
                MAXMAGICRESIST = &H260000
                FIRERESIST = &H270000
                MAXFIRERESIST = &H280000
                LIGHTRESIST = &H290000
                MAXLIGHTRESIST = &H2A0000
                COLDRESIST = &H2B0000
                MAXCOLDRESIST = &H2C0000
                POISONRESIST = &H2D0000
                MAXPOISONRESIST = &H2E0000
            End Enum

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure UnitAny
                <FieldOffset(0)> _
                Public dwType As UInteger
                <FieldOffset(4)> _
                Public dwTxtFileNo As UInteger
                <FieldOffset(8)> _
                Public _1 As UInteger
                <FieldOffset(12)> _
                Public dwUnitId As UInteger
                <FieldOffset(16)> _
                Public dwMode As UInteger

                ' first union 
                <FieldOffset(20)> _
                Public ptrPlayerData As IntPtr
                <FieldOffset(20)> _
                Public ptrItemData As IntPtr
                <FieldOffset(20)> _
                Public ptrMonsterData As IntPtr
                <FieldOffset(20)> _
                Public ptrObjectData As IntPtr

                <FieldOffset(24)> _
                Public dwAct As UInteger
                <FieldOffset(28)> _
                Public ptrAct As IntPtr
                <FieldOffset(32)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public dwSeed As UInteger()
                <FieldOffset(40)> _
                Public _2 As UInteger

                ' second union 
                <FieldOffset(44)> _
                Public ptrPath As IntPtr
                <FieldOffset(44)> _
                Public ptrItemPath As IntPtr
                <FieldOffset(44)> _
                Public ptrObjectPath As IntPtr

                <FieldOffset(48)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> _
                Public _3 As UInteger()
                <FieldOffset(68)> _
                Public dwGfxFrame As UInteger
                <FieldOffset(72)> _
                Public dwFrameRemain As UInteger
                <FieldOffset(76)> _
                Public wFrameRate As UShort
                <FieldOffset(78)> _
                Public _4 As UShort
                <FieldOffset(80)> _
                Public ptrGfxUnk As IntPtr
                <FieldOffset(84)> _
                Public ptrGfxInfo As IntPtr
                <FieldOffset(88)> _
                Public _5 As UInteger
                <FieldOffset(92)> _
                Public ptrStats As IntPtr
                <FieldOffset(96)> _
                Public ptrInventory As IntPtr
                <FieldOffset(100)> _
                Public ptrtLight As IntPtr
                <FieldOffset(104)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> _
                Public _6 As UInteger()

                <FieldOffset(140)> _
                Public wX As UShort
                <FieldOffset(142)> _
                Public wY As UShort

                <FieldOffset(144)> _
                Public _7 As UInteger

                <FieldOffset(148)> _
                Public dwOwnerType As UInteger
                <FieldOffset(152)> _
                Public dwOwnerId As UInteger

                <FieldOffset(156)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                Public _8 As UInteger()

                <FieldOffset(168)> _
                Public ptrInfo As IntPtr

                <FieldOffset(172)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                Public _9 As UInteger()

                <FieldOffset(196)> _
                Public dwFlags As UInteger
                <FieldOffset(200)> _
                Public dwFlags2 As UInteger

                <FieldOffset(204)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> _
                Public _10 As UInteger()

                <FieldOffset(224)> _
                Public ptrChangedNext As IntPtr
                <FieldOffset(228)> _
                Public ptrRoomNext As IntPtr
                <FieldOffset(232)> _
                Public ptrListNext As IntPtr
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure StatList
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> _
                Public _1 As UInteger()
                Public pStat As IntPtr
                Public wStatCount1 As UInt16
                Public wStatCount2 As UInt16
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _2 As UInteger()
                Public _3 As IntPtr
                Public _4 As UInteger
                Public pNext As IntPtr
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Stat
                Public dwStatId As UInteger
                Public dwStatValue As UInteger
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure SkillInfo
                Public wSkillId As UInt16
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Skill
                Public pSkillInfo As IntPtr
                Public pNextSkill As IntPtr
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
                Public _1 As UInteger()
                Public dwSkillLevel As UInteger
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _2 As UInteger()
                Public dwFlags As UInteger
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Info
                Public pGame1C As IntPtr
                Public pFirstSkill As IntPtr
                Public pLeftSkill As IntPtr
                Public pRightSkill As IntPtr
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure MonsterData
                <FieldOffset(0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=22)> _
                Public _1 As Byte()

                '[FieldOffset(0x16)] public byte fFlags; 
                <FieldOffset(22)> _
                Public Flags As Byte

                <FieldOffset(23)> _
                Public _2 As UInt16
                <FieldOffset(24)> _
                Public _3 As UInteger
                <FieldOffset(28)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> _
                Public anEnchants As Byte()
                <FieldOffset(37)> _
                Public _4 As Byte
                <FieldOffset(38)> _
                Public wUniqueNo As UInt16
                <FieldOffset(40)> _
                Public _5 As Byte

                <FieldOffset(44)> _
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=28)> _
                Public wName As String
            End Structure

            <Flags()> _
            Public Enum MonsterDataFlags
                None
                Minion = 10
                Champion = 16
            End Enum

        End Namespace

        Namespace Structures

            <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
            Public Structure GameInfo
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                Public _1 As UInteger()
                Public _1a As UInt16
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=24)> _
                Public szGameName As String
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=86)> _
                Public szGameServerIp As String
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=48)> _
                Public szAccountName As String
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=24)> _
                Public szCharName As String
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=24)> _
                Public szRealmName As String
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=343)> _
                Public _2 As UInteger()
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=24)> _
                Public szGamePassword As String
            End Structure
            ' 
            ' I'm trying to represent "CHAR szGameName[0x18];" in a C# struct, I'm sure I have the correct offsets and such, but I get overlapping errors unless I use: "[MarshalAs(UnmanagedType.U1, SizeConst=24)] public byte szGameName;" -- which doesn't give me the character array, but the number 97 
            ' 

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Control
                Public dwType As UInteger
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _1 As UInteger()
                Public dwPosX As UInteger
                Public dwPosY As UInteger
                Public dwSizeX As UInteger
                Public dwSizeY As UInteger
                Public fnCallback As UInteger
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> _
                Public _3 As UInteger()
                Public pNext As IntPtr
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> _
                Public _4 As UInteger()
                Public dwSelectStart As UInteger
                Public dwSelectEnd As UInteger
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> _
                Public wText As Byte()
                Public dwCursorPos As UInteger
                Public dwIsCloaked As UInteger
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure PresetUnit

                <FieldOffset(0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _1 As UInteger()
                <FieldOffset(8)> _
                Public dwPosY As UInteger
                <FieldOffset(12)> _
                Public dwClassID As Integer
                <FieldOffset(16)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> _
                Public _2 As UInteger()
                <FieldOffset(20)> _
                Public pPresetNext As IntPtr
                <FieldOffset(24)> _
                Public dwPosX As UInteger
                <FieldOffset(28)> _
                Public dwType As UInteger
            End Structure

        End Namespace

        Namespace Structures

            ''' <summary> 
            ''' Sizeof = 0x230 
            ''' </summary> 
            ''' 
            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Level
                <FieldOffset(0)> _
                Public _1 As UInteger
                '0x00 
                <FieldOffset(4)> _
                Public dwPosX As Integer
                '0x04 
                <FieldOffset(8)> _
                Public dwPosY As Integer
                '0x08 
                <FieldOffset(12)> _
                Public dwSizeX As Integer
                '0x0C 
                <FieldOffset(16)> _
                Public dwSizeY As Integer
                '0x10 
                <FieldOffset(20)> _
                Public nLevelNo As UInteger
                <FieldOffset(24)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=120)> _
                Public _1a As UInteger()
                <FieldOffset(504)> _
                Public seed As UInteger
                <FieldOffset(508)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> _
                Public _2 As UInteger()
                <FieldOffset(516)> _
                Public ptrRoom2First As IntPtr
                <FieldOffset(520)> _
                Public ptrDrlgMisc As IntPtr
                <FieldOffset(524)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
                Public _3 As UInteger()
                <FieldOffset(556)> _
                Public ptrLevelNext As IntPtr
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Act
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _1 As UInteger()
                Public pMisc As IntPtr
                Public pRoom1 As IntPtr
                Public _2 As UInteger
                Public dwAct As UInteger
                Public pfnCallBack As UInteger
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure ActMisc
                <FieldOffset(0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=33)> _
                Public _1 As UInteger()
                <FieldOffset(132)> _
                Public ptrDrlgAct As IntPtr
                <FieldOffset(136)> _
                Public nBossTombLvl As UInteger
                <FieldOffset(140)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=248)> _
                Public _2 As UInteger()
                <FieldOffset(1132)> _
                Public ptrLevelFirst As IntPtr
                <FieldOffset(1136)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _3 As UInteger()
                <FieldOffset(1144)> _
                Public nStaffTombLvl As UInteger
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Path
                <FieldOffset(0)> _
                Public xOffset As UInt16
                <FieldOffset(2)> _
                Public xPos As UInt16
                <FieldOffset(4)> _
                Public yOffset As UInt16
                <FieldOffset(6)> _
                Public yPos As UInt16

                <FieldOffset(8)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _1 As UInteger()
                <FieldOffset(16)> _
                Public xTarget As UInt16
                <FieldOffset(18)> _
                Public yTarget As UInt16
                <FieldOffset(20)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _2 As UInteger()
                <FieldOffset(28)> _
                Public pRoom1 As IntPtr
                <FieldOffset(32)> _
                Public pRoomUnk As IntPtr
                <FieldOffset(36)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                Public _3 As UInteger()
                <FieldOffset(48)> _
                Public pUnit As IntPtr
                <FieldOffset(52)> _
                Public dwFlags As UInteger
                <FieldOffset(56)> _
                Public _4 As UInteger
                <FieldOffset(60)> _
                Public dwPathType As UInteger
                <FieldOffset(64)> _
                Public dwPrevPathType As UInteger
                <FieldOffset(68)> _
                Public dwUnitSize As UInteger
                <FieldOffset(72)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> _
                Public _5 As UInteger()
                <FieldOffset(88)> _
                Public pTargetUnit As IntPtr
                <FieldOffset(92)> _
                Public dwTargetType As UInteger
                <FieldOffset(96)> _
                Public dwTargetId As UInteger
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Room1
                <FieldOffset(0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public dwSeed As UInteger()
                <FieldOffset(8)> _
                Public dwXStart As UInteger
                <FieldOffset(12)> _
                Public dwYStart As UInteger
                <FieldOffset(16)> _
                Public dwXSize As UInteger
                <FieldOffset(20)> _
                Public dwYSize As UInteger
                <FieldOffset(24)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> _
                Public _1 As UInteger()
                <FieldOffset(52)> _
                Public pRoomsNear As IntPtr
                <FieldOffset(56)> _
                Public pRoom2 As IntPtr
                <FieldOffset(60)> _
                Public pUnitFirst As IntPtr
                <FieldOffset(64)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                Public _2 As UInteger()
                <FieldOffset(88)> _
                Public _1s As IntPtr
                <FieldOffset(92)> _
                Public _3 As UInteger
                <FieldOffset(96)> _
                Public Coll As IntPtr
                <FieldOffset(100)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _4 As UInteger()
                <FieldOffset(108)> _
                Public pAct As IntPtr
                <FieldOffset(112)> _
                Public _5 As UInteger
                <FieldOffset(116)> _
                Public pRoomNext As IntPtr
                <FieldOffset(120)> _
                Public nUnknown As Integer
                <FieldOffset(124)> _
                Public dwRoomsNear As UInteger
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Room2
                <FieldOffset(0)> _
                Public pRoomTiles As IntPtr
                <FieldOffset(4)> _
                Public _1 As UInteger
                <FieldOffset(8)> _
                Public dwPresetType As UInteger
                <FieldOffset(12)> _
                Public _2 As UInteger
                <FieldOffset(16)> _
                Public dwRoomsNear As UInteger
                <FieldOffset(20)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _3 As UInteger()
                <FieldOffset(28)> _
                Public pLevel As IntPtr
                <FieldOffset(32)> _
                Public dwPosX As UInteger
                <FieldOffset(36)> _
                Public dwPosY As UInteger
                <FieldOffset(40)> _
                Public dwSizeX As UInteger
                <FieldOffset(44)> _
                Public dwSizeY As UInteger
                <FieldOffset(48)> _
                Public pRoom2Near As IntPtr
                <FieldOffset(52)> _
                Public pPreset As IntPtr
                <FieldOffset(56)> _
                Public pRoom2Next As IntPtr
                <FieldOffset(60)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=34)> _
                Public _4 As UInteger()
                <FieldOffset(196)> _
                Public pRoom2Prev As IntPtr
                <FieldOffset(200)> _
                Public pRoom2Other As IntPtr
                <FieldOffset(204)> _
                Public _5 As UInteger
                <FieldOffset(208)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public dwSeed As UInteger()
                <FieldOffset(216)> _
                Public _6 As UInteger
                <FieldOffset(220)> _
                Public pType2Info As IntPtr
                <FieldOffset(224)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _7 As UInteger()
                <FieldOffset(232)> _
                Public pRoom1 As IntPtr
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure RoomTile
                <FieldOffset(0)> _
                Public _1 As UInteger
                <FieldOffset(4)> _
                Public pRoom2 As IntPtr
                <FieldOffset(8)> _
                Public pNext As IntPtr
                <FieldOffset(12)> _
                Public nNum As IntPtr
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure LevelText
                <FieldOffset(0)> _
                Public dwLevelNo As UInteger
                <FieldOffset(4)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=60)> _
                Public _1 As UInteger()
                <FieldOffset(244)> _
                Public _2 As Byte
                <FieldOffset(245)> _
                <MarshalAs(UnmanagedType.LPStr, SizeConst:=40)> _
                Public szName As String
                <FieldOffset(285)> _
                <MarshalAs(UnmanagedType.LPStr, SizeConst:=40)> _
                Public szEntranceText As String
                <FieldOffset(325)> _
                <MarshalAs(UnmanagedType.LPStr, SizeConst:=41)> _
                Public szLevelDesc As String
                <FieldOffset(366)> _
                <MarshalAs(UnmanagedType.LPStr, SizeConst:=40)> _
                Public wName As String
                <FieldOffset(446)> _
                <MarshalAs(UnmanagedType.LPStr, SizeConst:=40)> _
                Public wEntranceText As String
                <FieldOffset(406)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
                Public nObjGroup As Byte()
                <FieldOffset(414)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
                Public nObjPrb As Byte()
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Coll
                <FieldOffset(0)> _
                Public dwPosGameX As UInteger
                <FieldOffset(4)> _
                Public dwPosGameY As UInteger
                <FieldOffset(8)> _
                Public dwSizeGameX As UInteger
                <FieldOffset(12)> _
                Public dwSizeGameY As UInteger
                <FieldOffset(16)> _
                Public dwPosRoomX As UInteger
                <FieldOffset(20)> _
                Public dwPosRoomY As UInteger
                <FieldOffset(24)> _
                Public dwSizeRoomX As UInteger
                <FieldOffset(28)> _
                Public dwSizeRoomY As UInteger
                <FieldOffset(32)> _
                Public pMapStart As IntPtr
                <FieldOffset(34)> _
                Public pMapEnd As IntPtr
            End Structure
        End Namespace

    End Namespace
End Namespace