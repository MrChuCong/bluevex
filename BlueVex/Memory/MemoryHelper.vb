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
            Public Function ClientProcessFromPos(ByVal PositionFromPackets As Point) As Process
                Dim Pos As Point = PositionFromPackets
                Dim MemEditor As New MemEditor

                Dim DiabloProcesses As Process() = Process.GetProcesses
                For Each Process As Process In DiabloProcesses
                    'It works no matter the window name.
                    If Process.ProcessName = "Diablo II" Then
                        'Sometime the packets are not that accurate. Range of 7 Units.
                        If InRange(Wrapped.GetMyPosition(Process.Id), Pos, 7) Then
                            Return Process
                        End If
                    End If
                Next
                Return Nothing
            End Function

            ''' <summary> 
            ''' Return all clients with the specified Status.
            ''' </summary> 
            Public Function ClientsProcessFromStatus(ByVal Status As Wrapped.OutOfGameState) As List(Of Process)
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

            Public Function ClientProcessFromCharName(ByVal CharName As String) As Process

                Dim DiabloProcesses As Process() = Process.GetProcesses
                For Each Process As Process In DiabloProcesses
                    'If it's a diablo Client
                    If Process.ProcessName = "Diablo II" Then
                        'If It's on the status asked.
                        If Wrapped.GetMyPlayerName(Process.Id).ToLower.Contains(CharName.ToLower) Then
                            Return Process
                        End If
                    End If
                Next
                Return New Process
            End Function

            ''' <summary>
            ''' Get a list of all the D2Clients currently running
            ''' </summary>
            ''' <returns></returns>
            Public Function GetDiabloClients() As List(Of Process)

                Dim Clients As New List(Of Process)

                Dim DiabloProcesses As Process() = Process.GetProcesses

                For Each Process As Process In DiabloProcesses
                    'If it's a diablo Client
                    If Process.ProcessName = "Diablo II" Then
                        Clients.Add(Process)
                    End If
                Next
                Return Clients

            End Function


            Public Function ClientFromWindowName(ByVal Name As String) As List(Of Process)

                Dim Clients As New List(Of Process)
                Dim DiabloProcesses As Process() = Process.GetProcesses

                For Each Process As Process In DiabloProcesses
                    'If it's a diablo Client
                    If Process.ProcessName = "Diablo II" Then
                        'Name we search.
                        If Process.MainWindowTitle.ToLower = Name.ToLower Then
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

            Public Function GetClientsCharName() As List(Of String)
                Dim BufNames As New List(Of String)

                Dim ListClient As List(Of Process) = ClientDetection.GetDiabloClients()

                For i As Integer = 0 To ListClient.Count - 1
                    Dim TempName As String = Wrapped.GetMyPlayerName(ListClient(i).Id)
                    'If the name is selected.
                    If Not TempName = "" Then
                        'If client is past the Char Select screen.
                        If GetClientStatus(ListClient(i).Id) < OutOfGameState.CharacterSelect Then
                            BufNames.Add(TempName.ToLower)
                        End If
                    End If
                Next
                Return BufNames
            End Function

            ''' <summary>
            ''' Get the Player Name when you're in Chat/Game
            ''' </summary>
            Public Function GetMyPlayerName(Optional ByVal DiabloPID As Integer = 0) As String
                Dim GetBnetData As New Raw.Functions(DiabloPID)

                Dim Mes As New Raw.Structures.GameInfo

                Mes = GetBnetData.GetGameInfo
                Return Mes.szCharName
            End Function

            Public Function GetMyPosition(Optional ByVal DiabloPID As Integer = 0) As Point

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule
                Dim dwUnitAddr As Int32
                Dim Dwtemp As Int32

                If MemEditor.Success = False Then Return New Point

                Try
                    d2client = MemEditor.GetModule("D2Client.dll")
                    'Player Pointer
                    dwUnitAddr = MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11C3D0, 4)
                    'UnitAny - pPath
                    Dwtemp = MemEditor.ReadMemoryLong(dwUnitAddr + &H2C, 4)
                    'Path - Xpos
                    GetMyPosition.X = MemEditor.ReadMemoryShort(Dwtemp + &H2)
                    'Path - Ypos
                    GetMyPosition.Y = MemEditor.ReadMemoryShort(Dwtemp + &H6)
                    MemEditor.mCloseProcess()
                    Return GetMyPosition
                Catch
                    Return New Point
                End Try
            End Function

            Public Function GetAreaID(Optional ByVal DiabloPID As Integer = 0) As Long

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule

                If MemEditor.Success = False Then Return 0

                Try
                    d2client = MemEditor.GetModule("D2Client.dll")
                    Return MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H12340C, Len(New Int32))
                Catch
                    Return 0
                End Try
            End Function

            Public Function GetDifficulty(Optional ByVal DiabloPID As Integer = 0) As D2Data.GameDifficulty

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule

                If MemEditor.Success = False Then Return 5

                Try
                    d2client = MemEditor.GetModule("D2Client.dll")
                    Return MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11BFF4, Len(New Int32))
                Catch
                    Return 5
                End Try
            End Function

            Public Function GetMapSeed(Optional ByVal DiabloPID As Integer = 0) As UInteger()

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule

                If MemEditor.Success = False Then Return Nothing

                Try
                    d2client = MemEditor.GetModule("D2Client.dll")
                    Dim GetPath As New Raw.Functions(DiabloPID)
                    Dim dwtemp As UInteger

                    'UnitAny - pPath
                    dwtemp = GetPath.GetPlayerUnit.pPath

                    'pPath - pRoom1
                    dwtemp = MemEditor.ReadMemoryLong(dwtemp + &H1C, 4)

                    'Room1 - pRoom2
                    dwtemp = MemEditor.ReadMemoryLong(dwtemp + &H70, 4)

                    'Room2 - pLevel
                    dwtemp = MemEditor.ReadMemoryLong(dwtemp + &H0, 4)
                    Dim Level As New Raw.Structures.Level
                    Level = MemEditor.ReadMemoryStruct(dwtemp, Level.GetType)
                    'pLevel - DwSeed
                    Return Level.dwSeed
                Catch
                    Return Nothing
                End Try
            End Function

            Public Function GetPlayerStats(Optional ByVal DiabloPID As Integer = 0) As PlayerStats_t

                Dim Memoryzer As New Raw.Functions(DiabloPID)

                Dim RawPlayerStats As List(Of Raw.Structures.Stat) = Memoryzer.GetPlayerStats

                Dim PlayerStats As New PlayerStats_t


                For i As Integer = 0 To RawPlayerStats.Count - 1
                    With PlayerStats

                        'Select Case RawPlayerStats(i).dwStatId
                        Select Case RawPlayerStats(i).wStatIndex
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
                                    .Life = Tools.HexToDec(HexValue.Remove(HexValue.Length - 2))
                                Else
                                    .Life = RawPlayerStats(i).dwStatValue
                                End If

                            Case Raw.Structures.Stats.MAXHP
                                Dim HexValue As String
                                HexValue = Tools.DecToHex(RawPlayerStats(i).dwStatValue)
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
                                .Level = RawPlayerStats(i).dwStatValue
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

            Public Function GetStaffTomb(Optional ByVal DiabloPID As Integer = 0) As UInteger

                Dim MemEditor As New MemEditor(DiabloPID)
                Dim d2client As System.Diagnostics.ProcessModule

                If MemEditor.Success = False Then Return Nothing

                Try
                    d2client = MemEditor.GetModule("D2Client.dll")
                    Dim DwUnitAddr As UInteger
                    Dim dwtemp As UInteger

                    DwUnitAddr = MemEditor.ReadMemoryLong(d2client.BaseAddress.ToInt32 + &H11C3D0, Len(New Int32))
                    'UnitAny - pAct
                    dwtemp = MemEditor.ReadMemoryLong(DwUnitAddr + &H1C, Len(New Int32))

                    'Act - pMisc
                    dwtemp = MemEditor.ReadMemoryLong(dwtemp + &H38, Len(New Int32))
                    'ActMisc - DwStaffTombLvl
                    Return MemEditor.ReadMemoryLong(dwtemp + &H3C0, Len(New Int32))
                Catch
                    Return Nothing
                End Try
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

            Dim Life As Integer
            Dim BaseMaxLife As Integer

            Dim Mana As Integer
            Dim BaseMaxMana As Integer

            Dim Stam As Integer
            Dim BaseMaxStam As Integer

            Dim Level As Integer
            Dim Xp As Long

        End Structure

        Public Enum Enchants As Byte

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
            Dim D2Launch As System.Diagnostics.ProcessModule

            Public Sub New(Optional ByVal DiabloPID As Integer = 0)
                Me.Reader = New MemEditor(DiabloPID)
                D2client = Reader.GetModule("D2Client.dll")
                D2Win = Reader.GetModule("D2Win.dll")
                D2Launch = Reader.GetModule("D2Launch.dll")
            End Sub

            Public Function IsInGame() As Boolean
                Return Me.GetPlayerUnit().dwUnitId <> 0
            End Function

            Public Function GetPlayerUnit() As Structures.UnitAny
                Try
                    Dim PlayerPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2client.BaseAddress) + &H11C3D0)

                    Return Reader.ReadMemoryStruct(PlayerPointer, GetType(Structures.UnitAny))
                Catch
                    Return New Structures.UnitAny
                End Try
            End Function

            Public Function GetPlayerStats() As List(Of Structures.Stat)
                Dim tempList As New List(Of Structures.Stat)()
                Try
                    Dim statList As Structures.StatList = Reader.ReadMemoryStruct(Me.GetPlayerUnit.pStats, GetType(Structures.StatList))

                    While True
                        For i As Integer = 0 To statList.wStatCount1 - 1
                            tempList.Add(Reader.ReadMemoryStruct((CInt(statList.pStat) + (i * 8)), GetType(Structures.Stat)))
                        Next

                        If statList.pNext = IntPtr.Zero Then
                            Exit While
                        End If
                        statList = Reader.ReadMemoryStruct(statList.pNext, GetType(Structures.StatList))
                    End While

                    Return tempList
                Catch
                    Return New List(Of Structures.Stat)
                End Try
            End Function

            Public Function GetCurrentAct() As Structures.Act
                Try
                    Return Reader.ReadMemoryStruct(Me.GetPlayerUnit.pAct, GetType(Structures.Act))
                Catch
                    Return New Structures.Act
                End Try
            End Function

            Public Function GetPlayerPath() As Structures.Path
                Try
                    Return Reader.ReadMemoryStruct(Me.GetPlayerUnit.pPath, GetType(Structures.Path))
                Catch ex As Exception
                    Return New Structures.Path
                End Try
            End Function

            Public Function GetPlayerRoom1() As Structures.Room1
                Try
                    Return Reader.ReadMemoryStruct(Me.GetPlayerPath.pRoom1, GetType(Structures.Room1))
                Catch ex As Exception
                    Return New Structures.Room1
                End Try
            End Function
            Public Function GetPlayerRoom2() As Structures.Room2
                Try
                    Return Reader.ReadMemoryStruct(Me.GetPlayerRoom1.pRoom2, GetType(Structures.Room2))
                Catch ex As Exception
                    Return New Structures.Room2
                End Try
            End Function
            Public Function GetCurrentLevel() As Structures.Level
                Try
                    Return Reader.ReadMemoryStruct(Me.GetPlayerRoom2.pLevel, GetType(Structures.Level))
                Catch ex As Exception
                    Return New Structures.Level
                End Try
            End Function

            Public Function GetAllRoom2() As List(Of Structures.Room2)
                Return Me.GetAllRoom2(Me.GetCurrentLevel)
            End Function

            Public Function GetAllRoom2(ByVal level As Structures.Level) As List(Of Structures.Room2)
                Dim room2List As New List(Of Structures.Room2)
                Try

                    ' Save first room 
                    Dim TempRoom As Structures.Room2 = Reader.ReadMemoryStruct(level.pRoom2First, GetType(Structures.Room2))

                    room2List.Add(TempRoom)

                    While True
                        ' Get next room 
                        TempRoom = Reader.ReadMemoryStruct(TempRoom.pRoom2Next, GetType(Structures.Room2))

                        room2List.Add(TempRoom)

                        If TempRoom.pRoom2Next = IntPtr.Zero Then
                            Exit While
                        End If
                    End While
                    Return room2List

                Catch ex As Exception
                    Return New List(Of Structures.Room2)
                End Try
            End Function

            Public Function GetPresetUnit(ByVal room2 As Structures.Room2) As Structures.PresetUnit
                Dim presetUnitPointer As IntPtr = room2.pPreset
                Try
                    Return Reader.ReadMemoryStruct(presetUnitPointer, GetType(Structures.PresetUnit))
                Catch ex As Exception
                    Return New Structures.PresetUnit
                End Try
            End Function
            Public Function GetTiles(ByVal room2 As Structures.Room2) As List(Of Structures.RoomTile)
                Dim tileList As New List(Of Structures.RoomTile)
                Try
                    ' Save first tile 
                    Dim temptile As Structures.RoomTile = Reader.ReadMemoryStruct(room2.pRoomTiles, GetType(Structures.RoomTile))

                    tileList.Add(temptile)

                    While True
                        ' Get next room 
                        temptile = Reader.ReadMemoryStruct(temptile.pNext, GetType(Structures.RoomTile))

                        tileList.Add(temptile)

                        If temptile.pNext = IntPtr.Zero Then
                            Exit While
                        End If
                    End While

                    Return tileList

                Catch ex As Exception
                    Return New List(Of Structures.RoomTile)
                End Try
            End Function

            Public Function GetGameInfo() As Structures.GameInfo
                Try
                    'Based On D2Client, get Value In Game
                    'Dim GameInfoPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2client.BaseAddress) + &H11B908)
                    'Based On D2Launch, Gets Value when Diablo Launched.
                    Dim GameInfoPointer As IntPtr = Reader.ReadMemoryInt(CInt(D2Launch.BaseAddress) + &H25ACC)
                    Return Reader.ReadMemoryStruct(GameInfoPointer, GetType(Structures.GameInfo))

                Catch ex As Exception
                    Return New Structures.GameInfo
                End Try
            End Function

            Public Function GetPlayerSkills() As Dictionary(Of SkillType, Integer)
                Try
                    Dim info As Structures.Info = Reader.ReadMemoryStruct(Me.GetPlayerUnit.pInfo, GetType(Structures.Info))
                    Dim skillList As New Dictionary(Of SkillType, Integer)()
                    ' Save first tile 
                    Dim tempSkill As Structures.Skill = Reader.ReadMemoryStruct(info.pFirstSkill, GetType(Structures.Skill))
                    Dim skillInfo As Structures.SkillInfo = Reader.ReadMemoryStruct(tempSkill.pSkillInfo, GetType(Structures.SkillInfo))
                    skillList.Add(DirectCast(skillInfo.wSkillId, SkillType), CInt(tempSkill.dwSkillLevel))
                    While True
                        ' Get next Skill
                        tempSkill = Reader.ReadMemoryStruct(tempSkill.pNextSkill, GetType(Structures.Skill))
                        skillInfo = Reader.ReadMemoryStruct(tempSkill.pSkillInfo, GetType(Structures.SkillInfo))
                        skillList.Add(DirectCast(skillInfo.wSkillId, SkillType), CInt(tempSkill.dwSkillLevel))
                        If tempSkill.pNextSkill = IntPtr.Zero Then
                            Exit While
                        End If
                    End While
                    Return skillList

                Catch ex As Exception
                    Return New Dictionary(Of SkillType, Integer)
                End Try
            End Function

            ''' <summary> 
            ''' Must be in the same act as the unit 
            ''' </summary> 
            ''' <param name="code"></param> 
            ''' <returns></returns> 
            ''' 
            Public Function GetUnit(ByVal code As NPCCode) As Structures.UnitAny
                Try
                    Dim room1 As Structures.Room1 = Reader.ReadMemoryStruct(Me.GetCurrentAct.pRoom1, GetType(Structures.Room1))

                    While True
                        Dim unit As Structures.UnitAny = Reader.ReadMemoryStruct(room1.pUnitFirst, GetType(Structures.UnitAny))
                        While True
                            If unit.dwTxtFileNo = CInt(code) Then
                                Return unit
                            End If

                            If unit.pListNext = IntPtr.Zero Then
                                Exit While
                            End If
                            unit = Reader.ReadMemoryStruct(unit.pListNext, GetType(Structures.UnitAny))
                        End While

                        If room1.pRoomNext = IntPtr.Zero Then
                            Exit While
                        End If
                        room1 = Reader.ReadMemoryStruct(room1.pRoomNext, GetType(Structures.Room1))
                    End While

                    Return New Structures.UnitAny

                Catch ex As Exception
                    Return New Structures.UnitAny
                End Try
            End Function

            ''' <summary> 
            ''' Must be in the same act as the unit 
            ''' </summary> 
            ''' <param name="code"></param> 
            ''' <returns></returns> 
            Public Function GetUnit(ByVal code As Wrapped.UniqueMonster) As Structures.UnitAny
                Try
                    Dim room1 As Structures.Room1 = Reader.ReadMemoryStruct(Me.GetCurrentAct.pRoom1, GetType(Structures.Room1))

                    While True
                        Dim unit As Structures.UnitAny = Reader.ReadMemoryStruct(room1.pUnitFirst, GetType(Structures.UnitAny))

                        While True
                            'If DirectCast(Me.GetMonsterData(unit).wUniqueNo, UniqueMonster) = code Then
                            If Me.GetMonsterData(unit).wUniqueNo = code Then
                                Return unit
                            End If

                            If unit.pListNext = IntPtr.Zero Then
                                Exit While
                            End If
                            unit = Reader.ReadMemoryStruct(unit.pListNext, GetType(Structures.UnitAny))
                        End While

                        If room1.pRoomNext = IntPtr.Zero Then
                            Exit While
                        End If
                        room1 = Reader.ReadMemoryStruct(room1.pRoomNext, GetType(Structures.Room1))
                    End While

                    Return New Structures.UnitAny

                Catch ex As Exception
                    Return New Structures.UnitAny
                End Try
            End Function

            ''' <summary> 
            ''' Gets monsters in the same act 
            ''' </summary> 
            ''' <returns></returns> 
            Public Function GetUnits() As List(Of Structures.UnitAny)
                Dim tempList As New List(Of Structures.UnitAny)
                Try
                    Dim room1 As Structures.Room1 = Reader.ReadMemoryStruct(Me.GetCurrentAct.pRoom1, GetType(Structures.Room1))
                    While True
                        Dim unit As Structures.UnitAny = Reader.ReadMemoryStruct(room1.pUnitFirst, GetType(Structures.UnitAny))

                        While True
                            tempList.Add(unit)

                            If unit.pListNext = IntPtr.Zero Then
                                Exit While
                            End If
                            unit = Reader.ReadMemoryStruct(unit.pListNext, GetType(Structures.UnitAny))
                        End While

                        If room1.pRoomNext = IntPtr.Zero Then
                            Exit While
                        End If
                        room1 = Reader.ReadMemoryStruct(room1.pRoomNext, GetType(Structures.Room1))
                    End While

                    Return tempList
                Catch ex As Exception
                    Return New List(Of Structures.UnitAny)
                End Try
            End Function

            ''' <summary> 
            ''' Gets monsters in the same level of specified type 
            ''' </summary> 
            ''' <returns></returns> 
            ''' 
            Public Function GetUnits(ByVal code As NPCCode) As List(Of Structures.UnitAny)
                Dim tempList As New List(Of Structures.UnitAny)
                Try
                    Dim room1 As Structures.Room1 = Reader.ReadMemoryStruct(Me.GetCurrentAct.pRoom1, GetType(Structures.Room1))
                    While True
                        Dim unit As Structures.UnitAny = Reader.ReadMemoryStruct(room1.pUnitFirst, GetType(Structures.UnitAny))
                        While True
                            If unit.dwTxtFileNo = code Then
                                'If DirectCast(unit.dwTxtFileNo, NPCCode) = code Then
                                tempList.Add(unit)
                            End If

                            If unit.pListNext = IntPtr.Zero Then
                                Exit While
                            End If
                            unit = Reader.ReadMemoryStruct(unit.pListNext, GetType(Structures.UnitAny))
                        End While

                        If room1.pRoomNext = IntPtr.Zero Then
                            Exit While
                        End If
                        room1 = Reader.ReadMemoryStruct(room1.pRoomNext, GetType(Structures.Room1))
                    End While
                    Return tempList
                Catch ex As Exception
                    Return New List(Of Structures.UnitAny)
                End Try
            End Function

            Public Function GetMonsterData(ByVal code As Wrapped.UniqueMonster) As Structures.MonsterData
                Try
                    Return Reader.ReadMemoryStruct(Me.GetUnit(code).pMonsterData, GetType(Structures.MonsterData))
                Catch ex As Exception
                    Return New Structures.MonsterData
                End Try
            End Function
            Public Function GetMonsterData(ByVal monster As Structures.UnitAny) As Structures.MonsterData
                Try
                    Return Reader.ReadMemoryStruct(monster.pMonsterData, GetType(Structures.MonsterData))
                Catch ex As Exception
                    Return New Structures.MonsterData
                End Try
            End Function

            Public Function GetMonsterData(ByVal monsters As List(Of Structures.UnitAny)) As List(Of Structures.MonsterData)
                Dim tempList As New List(Of Structures.MonsterData)()
                Try
                    For Each monster As Structures.UnitAny In monsters
                        tempList.Add(Me.GetMonsterData(monster))
                    Next

                    Return tempList
                Catch ex As Exception
                    Return New List(Of Structures.MonsterData)
                End Try
            End Function

            Public Function GetControls() As List(Of Structures.Control)
                Dim tempList As New List(Of Structures.Control)

                Try
                    '1.12
                    Dim firstcontrolptr As IntPtr = Reader.ReadMemoryInt(CInt(D2Win.BaseAddress) + &H5C718)

                    Dim control As Structures.Control = Reader.ReadMemoryStruct(firstcontrolptr, GetType(Structures.Control))

                    While True
                        tempList.Add(control)

                        If control.pNext = IntPtr.Zero Then
                            Exit While
                        End If

                        control = Reader.ReadMemoryStruct(control.pNext, GetType(Structures.Control))
                    End While

                    Return tempList
                Catch ex As Exception
                    Return New List(Of Structures.Control)
                End Try
            End Function

            Public Function GetState() As Wrapped.OutOfGameState
                Dim controls As List(Of Structures.Control) = Me.GetControls()
                Try
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

                Catch ex As Exception
                    Return 0
                End Try
            End Function

            Public Function GetMonsterEnchants(ByVal code As NPCCode) As List(Of Wrapped.Enchants)
                Return Me.GetMonsterEnchants(Me.GetUnit(code))
            End Function

            Public Function GetMonsterEnchants(ByVal code As Wrapped.UniqueMonster) As List(Of Wrapped.Enchants)
                Return Me.GetMonsterEnchants(Me.GetUnit(code))
            End Function

            Public Function GetMonsterEnchants(ByVal monster As Structures.UnitAny) As List(Of Wrapped.Enchants)
                Return Me.GetMonsterEnchants(Me.GetMonsterData(monster))
            End Function

            Public Function GetMonsterEnchants(ByVal monster As Structures.MonsterData) As List(Of Wrapped.Enchants)
                Dim tempList As New List(Of Wrapped.Enchants)

                For Each b As Byte In monster.anEnchants
                    tempList.Add(DirectCast(b, Wrapped.Enchants))
                Next

                Return tempList
            End Function

        End Class


        Namespace Structures

            Public Enum Stats
                STRENGTH = &H0
                ENERGY = &H1
                DEXTERITY = &H2
                VITALITY = &H3
                STATPTS = &H4
                NEWSKILLS = &H5
                HITPOINTS = &H6
                MAXHP = &H7
                MANA = &H8
                MAXMANA = &H9
                STAMINA = &HA
                MAXSTAMINA = &HB
                LEVEL = &HC
                EXPERIENCE = &HD
                GOLD = &HE
                GOLDBANK = &HF
                ITEM_ARMOR_PERCENT = &H10
                ITEM_MAXDAMAGE_PERCENT = &H11
                ITEM_MINDAMAGE_PERCENT = &H12
                TOHIT = &H13
                TOBLOCK = &H14
                MINDAMAGE = &H15
                MAXDAMAGE = &H16
                SECONDARY_MINDAMAGE = &H17
                SECONDARY_MAXDAMAGE = &H18
                DAMAGEPERCENT = &H19
                MANARECOVERY = &H1A
                MANARECOVERYBONUS = &H1B
                STAMINARECOVERYBONUS = &H1C
                LASTEXP = &H1D
                NEXTEXP = &H1E
                ARMORCLASS = &H1F
                ARMORCLASS_VS_MISSILE = &H20
                ARMORCLASS_VS_HTH = &H21
                NORMAL_DAMAGE_REDUCTION = &H22
                MAGIC_DAMAGE_REDUCTION = &H23
                DAMAGERESIST = &H24
                MAGICRESIST = &H25
                MAXMAGICRESIST = &H26
                FIRERESIST = &H27
                MAXFIRERESIST = &H28
                LIGHTRESIST = &H29
                MAXLIGHTRESIST = &H2A
                COLDRESIST = &H2B
                MAXCOLDRESIST = &H2C
                POISONRESIST = &H2D
                MAXPOISONRESIST = &H2E
            End Enum
            <Flags()> _
            Public Enum MonsterDataFlags
                None
                Minion = 10
                Champion = 16
            End Enum

        End Namespace

        'Misc
        Namespace Structures

            <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
            Public Structure GameInfo

                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                Public _1 As UInteger() '0x00

                Public _1a As UInt16 '0x18

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H18)> _
                Public szGameName As String '0x1A

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H56)> _
                Public szGameServerIp As String '0x32

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H30)> _
                Public szAccountName As String '0x88

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H18)> _
                Public szCharName As String '0xB8

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H18)> _
                Public szRealmName As String '0xD0

                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H157)> _
                Public _2 As Byte() '0xE8

                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H18)> _
                Public szGamePassword As String '0x23F
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Control
                Public dwType As UInteger           '0x00
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _1 As UInteger()             '0x04
                Public dwPosX As UInteger           '0x0C
                Public dwPosY As UInteger           '0x10
                Public dwSizeX As UInteger          '0x14
                Public dwSizeY As UInteger          '0x18
                Public fnCallback As UInteger       '0x1C
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> _
                Public _3 As UInteger()             '0x20
                Public pNext As IntPtr              '0x3C
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> _
                Public _4 As UInteger()             '0x40
                Public dwSelectStart As UInteger    '0x54
                Public dwSelectEnd As UInteger      '0x58
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> _
                Public wText As Byte()              '0x5C
                Public dwCursorPos As UInteger      '0x15C
                Public dwIsCloaked As UInteger      '0x160
            End Structure

        End Namespace

        'Unit/Player
        Namespace Structures

            <StructLayout(LayoutKind.Explicit)> _
           Public Structure Path
                <FieldOffset(&H0)> _
                Dim xOffset As UShort                  '0x00
                <FieldOffset(&H2)> _
                Dim xPos As UShort                         '0x02
                <FieldOffset(&H4)> _
                Dim yOffset As UShort                  '0x04
                <FieldOffset(&H6)> _
                Dim yPos As UShort                         '0x06
                <FieldOffset(&H8)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                    Dim _1() As UInteger                    '0x08
                <FieldOffset(&H10)> _
                Dim xTarget As UShort                  '0x10
                <FieldOffset(&H12)> _
                Dim yTarget As UShort                  '0x12
                <FieldOffset(&H14)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                    Dim _2() As UInteger                    '0x14
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' 

                <FieldOffset(&H1C)> _
                Dim pRoom1 As IntPtr    '0x1C
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' 
                <FieldOffset(&H20)> _
                Dim pRoomUnk As IntPtr      '0x20
                <FieldOffset(&H24)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                    Dim _3() As UInteger                    '0x24
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                ''' 
                <FieldOffset(&H30)> _
                Dim pUnit As IntPtr '0x30
                <FieldOffset(&H34)> _
                Dim dwFlags As UInteger                  '0x34
                <FieldOffset(&H38)> _
                Dim _4 As UInteger                       '0x38
                <FieldOffset(&H3C)> _
                Dim dwPathType As UInteger               '0x3C
                <FieldOffset(&H40)> _
                Dim dwPrevPathType As UInteger           '0x40
                <FieldOffset(&H44)> _
                Dim dwUnitSize As UInteger               '0x44
                <FieldOffset(&H48)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> _
                    Dim _5() As UInteger                     '0x48
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                ''' 
                <FieldOffset(&H58)> _
                Dim pTargetUnit As IntPtr '0x58
                <FieldOffset(&H5C)> _
                Dim dwTargetType As UInteger                 '0x5C
                <FieldOffset(&H60)> _
                Dim dwTargetId As UInteger               '0x60
                <FieldOffset(&H64)> _
                Dim bDirection As Byte '0x64
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Stat
                Dim wSubIndex As UShort                    '0x00
                Dim wStatIndex As UShort               '0x02
                Dim dwStatValue As UInteger              '0x04
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure StatList
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> _
                    Dim _1() As UInteger                    '0x00
                ''' <summary>
                ''' Stat
                ''' </summary>
                Dim pStat As IntPtr '0x24
                Dim wStatCount1 As UShort              '0x28
                Dim wStatCount2 As UShort              '0x2A
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                    Dim _2() As UInteger                   '0x2C
                ''' <summary>
                ''' BYTE
                ''' </summary>
                Dim _3 As IntPtr    '0x34
                Dim _4 As UInteger                       '0x38
                ''' <summary>
                ''' StatList
                ''' </summary>
                Dim pNext As IntPtr     '0x3C

            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure SkillInfo
                Dim wSkillId As UShort                     '0x00
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Skill
                ''' <summary>
                ''' SkillInfo
                ''' </summary>
                ''' 
                <FieldOffset(0)> _
                Dim pSkillInfo As IntPtr    '0x00
                ''' <summary>
                ''' Skill
                ''' </summary>
                ''' 
                <FieldOffset(4)> _
                Dim pNextSkill As IntPtr '0x04
                <FieldOffset(8)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
                    Dim _1() As UInteger                    '0x08
                <FieldOffset(&H28)> _
                Dim dwSkillLevel As UInteger                 '0x28
                <FieldOffset(&H2C)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                    Dim _2() As UInteger                    '0x2C
                <FieldOffset(&H30)> _
                Dim dwFlags As UInteger                  '0x30
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Info
                ''' <summary>
                ''' BYTE
                ''' </summary>
                Dim pGame1C As IntPtr   '0x00
                ''' <summary>
                ''' Skill
                ''' </summary>
                Dim pFirstSkill As IntPtr '0x04
                ''' <summary>
                ''' Skill
                ''' </summary>
                Dim pLeftSkill As IntPtr '0x08
                ''' <summary>
                ''' Skill
                ''' </summary>
                Dim pRightSkill As IntPtr '0x0C
            End Structure

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
                ''' <summary>
                ''' PlayerData
                ''' </summary>
                <FieldOffset(20)> _
                Public pPlayerData As IntPtr
                ''' <summary>
                ''' ItemData
                ''' </summary>
                <FieldOffset(20)> _
                Public pItemData As IntPtr
                ''' <summary>
                ''' MonsterData
                ''' </summary>
                <FieldOffset(20)> _
                Public pMonsterData As IntPtr
                ''' <summary>
                ''' ObjectData
                ''' </summary>
                <FieldOffset(20)> _
                Public pObjectData As IntPtr

                <FieldOffset(24)> _
                Public dwAct As UInteger
                ''' <summary>
                ''' Act
                ''' </summary>
                <FieldOffset(28)> _
                Public pAct As IntPtr
                <FieldOffset(32)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public dwSeed As UInteger()
                <FieldOffset(40)> _
                Public _2 As UInteger

                ' second union 
                ''' <summary>
                ''' Path
                ''' </summary>
                <FieldOffset(44)> _
                Public pPath As IntPtr
                ''' <summary>
                ''' ItemPath
                ''' </summary>
                <FieldOffset(44)> _
                Public pItemPath As IntPtr
                ''' <summary>
                ''' ObjectPath
                ''' </summary>
                <FieldOffset(44)> _
                Public pObjectPath As IntPtr

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
                ''' <summary>
                ''' Byte
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(80)> _
                Public pGfxUnk As IntPtr
                ''' <summary>
                ''' Dword
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(84)> _
                Public pGfxInfo As IntPtr
                <FieldOffset(88)> _
                Public _5 As UInteger
                ''' <summary>
                ''' StatList
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(92)> _
                Public pStats As IntPtr
                ''' <summary>
                ''' Inventory
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(96)> _
                Public pInventory As IntPtr
                ''' <summary>
                ''' Light
                ''' </summary>
                <FieldOffset(100)> _
                Public ptLight As IntPtr
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
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Public _8 As UInteger()
                ''' <summary>
                ''' OverheadMsg
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(164)> _
                Public pOMsg As IntPtr
                ''' <summary>
                ''' Info
                ''' </summary>
                ''' <remarks></remarks>
                <FieldOffset(168)> _
                Public pInfo As IntPtr

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
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                <FieldOffset(224)> _
                Public pChangedNext As IntPtr
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                <FieldOffset(228)> _
                Public pRoomNext As IntPtr
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                <FieldOffset(232)> _
                Public pListNext As IntPtr
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure ItemData
                Dim dwQuality As UInteger                '0x00
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _1() As UInteger                    '0x04
                Dim dwItemFlags As UInteger              '0x0C 1 = Owned by player, 0xFFFFFFFF = Not owned
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _2() As UInteger                    '0x10
                Dim dwFlags As UInteger                  '0x18
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                Dim _3() As UInteger                    '0x1C
                Dim dwQuality2 As UInteger               '0x28
                Dim dwItemLevel As UInteger              '0x2C
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _4() As UInteger                    '0x30
                Dim wPrefix As UShort                  '0x38
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _5() As UShort                        '0x3A
                Dim wSuffix As UShort                  '0x3E
                Dim _6 As UInteger                       '0x40
                Dim BodyLocation As Byte '0x44
                Dim ItemLocation As Byte '0x45 Non-body/belt location (Body/Belt == 0xFF)
                Dim _7 As Byte '0x46
                Dim _8 As UShort                       '0x47
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> _
                Dim _9() As UInteger                    '0x48
                ''' <summary>
                ''' Inventory
                ''' </summary>
                Dim pOwnerInventory As IntPtr '0x5C
                Dim _10 As UInteger                     '0x60
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                Dim pNextInvItem As IntPtr '0x64
                Dim _11 As Byte '0x68
                Dim NodePage As Byte '0x69 Actual location, this is the most reliable by far
                Dim _12 As UShort                      '0x6A
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                Dim _13() As UInteger                   '0x6C
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                Dim pOwner As IntPtr    '0x84

            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure MonsterData
                <FieldOffset(&H0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H16)> _
                    Public _1 As Byte() '0x00
                <FieldOffset(&H16)> _
                Public Flags As Byte '0x16
                <FieldOffset(&H17)> _
                Public _2 As UShort  '0x17
                <FieldOffset(&H18)> _
                Public _3 As UInteger  '0x18
                <FieldOffset(&H1C)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> _
                    Public anEnchants As Byte() '0x1C
                <FieldOffset(&H25)> _
                Public _4 As Byte '0x25
                <FieldOffset(&H26)> _
                Public wUniqueNo As UInt16 '0x26
                <FieldOffset(&H28)> _
                Public _5 As Byte '0x28
                <FieldOffset(&H2C)> _
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=28)> _
                    Public wName As String '0x2C
            End Structure

        End Namespace

        'Map
        Namespace Structures

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

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure RoomTile
                ''' <summary>
                ''' Dword
                ''' </summary>
                ''' 
                Dim nNum As IntPtr      '0x00
                ''' <summary>
                ''' Room2
                ''' </summary>
                ''' 
                Dim pRoom2 As IntPtr    '0x04
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _1() As UInteger    '0x08
                ''' <summary>
                ''' RoomTile
                ''' </summary>
                ''' 
                Dim pNext As IntPtr     '0x10
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure CollMap
                <FieldOffset(&H0)> _
                Dim dwPosGameX As UInteger      '0x00
                <FieldOffset(&H4)> _
                Dim dwPosGameY As UInteger      '0x04
                <FieldOffset(&H8)> _
                Dim dwSizeGameX As UInteger     '0x08
                <FieldOffset(&HC)> _
                Dim dwSizeGameY As UInteger     '0x0C
                <FieldOffset(&H10)> _
                Dim dwPosRoomX As UInteger      '0x10
                <FieldOffset(&H14)> _
                Dim dwPosRoomY As UInteger      '0x14
                <FieldOffset(&H18)> _
                Dim dwSizeRoomX As UInteger     '0x18
                <FieldOffset(&H1C)> _
                Dim dwSizeRoomY As UInteger     '0x1C
                ''' <summary>
                ''' Word
                ''' </summary>
                ''' 
                <FieldOffset(&H20)> _
                Dim pMapStart As IntPtr         '0x20		
                ''' <summary>
                ''' Word
                ''' </summary>
                ''' 
                <FieldOffset(&H22)> _
                Dim pMapEnd As IntPtr           '0x22
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure PresetUnit
                Dim dwTxtFileNo As UInteger     '0x00
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _1() As UInteger            '0x04
                Dim dwPosX As UInteger          '0x0C
                Dim _2 As UInteger              '0x10
                Dim dwPosY As UInteger          '0x14
                ''' <summary>
                ''' PresetUnit
                ''' </summary>
                ''' 
                Dim pPresetNext As IntPtr       '0x18
                Dim dwType As UInteger          '0x1C
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Level
                <FieldOffset(&H0)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H50)> _
                    Dim _1() As Byte                    '0x00
                <FieldOffset(&H50)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                    Dim dwSeed() As UInteger            '0x50
                <FieldOffset(&H58)> _
                Dim _2 As UInteger                      '0x58
                ''' <summary>
                ''' Level
                ''' </summary>
                ''' 
                <FieldOffset(&H5C)> _
                Dim pNextLevel As IntPtr                '0x5C
                <FieldOffset(&H60)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC)> _
                    Dim _3() As Byte                    '0x60
                <FieldOffset(&H6C)> _
                Dim dwPosX As UInteger                  '0x6C
                <FieldOffset(&H70)> _
                Dim dwPosY As UInteger                  '0x70
                <FieldOffset(&H74)> _
                Dim dwSizeX As UInteger                 '0x74
                <FieldOffset(&H78)> _
                Dim dwSizeY As UInteger                 '0x78
                <FieldOffset(&H7C)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                    Dim _4() As UInteger                '0x7C
                <FieldOffset(&H94)> _
                Dim dwLevelNo As UInteger               '0x94
                <FieldOffset(&H98)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H61)> _
                    Dim _5() As UInteger                '0x98
                ''' <summary>
                ''' Room2
                ''' </summary>
                ''' 
                <FieldOffset(&H21C)> _
                Dim pRoom2First As IntPtr               '0x21C		
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure Room2
                ''' <summary>
                ''' Level
                ''' </summary>
                <FieldOffset(0)> _
                Dim pLevel As IntPtr            '0x00
                <FieldOffset(&H4)> _
                Dim _1 As UInteger              '0x04
                <FieldOffset(&H8)> _
                Dim dwRoomsNear As UInteger     '0x08
                ''' <summary>
                ''' RoomTile
                ''' </summary>
                <FieldOffset(&HC)> _
                Dim pRoomTiles As IntPtr        '0x0C
                ''' <summary>
                ''' Room2
                ''' </summary>
                ''' 
                <FieldOffset(&H10)> _
                Dim pRoom2Near As IntPtr        '0x10			
                <FieldOffset(&H14)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)> _
                    Dim _3() As UInteger        '0x14
                <FieldOffset(&H2C)> _
                Dim dwPosX As UInteger          '0x2C
                <FieldOffset(&H30)> _
                Dim dwPosY As UInteger          '0x30
                <FieldOffset(&H34)> _
                Dim dwSizeX As UInteger         '0x34
                <FieldOffset(&H38)> _
                Dim dwSizeY As UInteger         '0x38
                ''' <summary>
                ''' Dword
                ''' </summary>
                ''' 
                <FieldOffset(&H3C)> _
                Dim pType2Info As IntPtr        '0x3C
                <FieldOffset(&H40)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H20)> _
                    Dim _4() As UInteger        '0x40
                <FieldOffset(&HC0)> _
                Dim dwPresetType As UInteger    '0xC0
                ''' <summary>
                ''' PresetUnit
                ''' </summary>
                ''' 
                <FieldOffset(&HC4)> _
                Dim pPreset As IntPtr           '0xC4
                <FieldOffset(&HC8)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                    Dim _5() As UInteger        '0xC8
                ''' <summary>
                ''' Room2
                ''' </summary>
                ''' 
                <FieldOffset(&HD4)> _
                Dim pRoom2Next As IntPtr        '0xD4
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' 
                <FieldOffset(&HD8)> _
                Dim pRoom1 As IntPtr            '0xD8
            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Room1
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' 
                Dim pRoomsNear As IntPtr        '0x00
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _1() As UInteger            '0x04
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim dwSeed() As UInteger        '0x0C
                Dim _2 As UInteger              '0x14
                Dim dwXStart As UInteger        '0x18
                Dim dwYStart As UInteger        '0x1C
                Dim dwXSize As UInteger         '0x20
                Dim dwYSize As UInteger         '0x24
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> _
                    Dim _3() As UInteger        '0x28
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' 
                Dim pRoomNext As IntPtr         '0x38
                Dim _4 As UInteger              '0x3C
                ''' <summary>
                ''' UnitAny
                ''' </summary>
                ''' 
                Dim pUnitFirst As IntPtr        '0x40
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
                Dim _5() As UInteger            '0x44
                ''' <summary>
                ''' CollMap
                ''' </summary>
                ''' 
                Dim Coll As IntPtr              '0x50
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> _
                Dim _6() As UInteger            '0x54
                ''' <summary>
                ''' Room2
                ''' </summary>
                ''' 
                Dim pRoom2 As IntPtr            '0x70
                Dim _7 As UInteger              '0x74
                Dim dwRoomsNear As UInteger     '0x78
            End Structure

            <StructLayout(LayoutKind.Explicit)> _
            Public Structure ActMisc
                <FieldOffset(&H0)> _
                Public _1 As UInteger             '0x00
                <FieldOffset(&H4)> _
                Public pAct As IntPtr '0x04
                <FieldOffset(&H8)> _
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H468)> _
                    Public _2() As Byte '0x08
                <FieldOffset(&H3C0)> _
                Public dwStaffTombLevel As UInteger '0x470
                <FieldOffset(&H470)> _
                Public pLevelFirst As UInteger '0x470

            End Structure

            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Act

                Dim dwSeed As UInteger '0x00
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H30)> _
                Dim _1() As Byte '0x04
                ''' <summary>
                ''' Room1
                ''' </summary>
                ''' <remarks></remarks>
                Dim pRoom1 As IntPtr    '0x34
                ''' <summary>
                ''' ActMisc
                ''' </summary>
                ''' <remarks></remarks>
                Dim pMisc As IntPtr     '0x38
                <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
                Dim _2() As UInteger    '0x40
                Dim dwAct As UInteger   '0x44
            End Structure

        End Namespace
    End Namespace
End Namespace