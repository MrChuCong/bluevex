﻿Imports System
Imports D2Data
Imports ETUtils
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Net
Imports D2Packets


Namespace BnetServer

    Public Class BsPacket
        Inherits D2Packet

        ' Fields
        Public ReadOnly Packetid As Byte

        ' Methods
        Public Sub New(ByVal Id As Byte)
            Packetid = Id
        End Sub

        Public Sub New(ByVal Data() As Byte)

            Me.Packetid = Data(1)

            Dim PacketData(Data.Length - 4) As Byte
            Buffer.BlockCopy(Data, 4, PacketData, 0, Data.Length - 4)

            Me.Packetid = Data(1)

            MyBase.Insert(PacketData)

        End Sub
        Public Overrides ReadOnly Property Data() As Byte()
            Get
                Dim dst As Byte() = New Byte(MyBase.Count + 4) {}
                Dim Basedata As Byte() = MyBase.GetData
                dst(0) = &HFF
                dst(1) = Packetid
                Dim bytes As Byte() = BitConverter.GetBytes(CUShort((MyBase.Count + 5 And &HFFFF)))
                dst(2) = bytes(0)
                dst(3) = bytes(1)
                Buffer.BlockCopy(Basedata, 0, dst, 4, Basedata.Length)
                Return dst
            End Get
        End Property

    End Class

    Public Class AdInfo
        Inherits BsPacket

        ' Fields
        Public ReadOnly extension As String
        Public ReadOnly filename As String
        Public ReadOnly id As UInteger
        Public ReadOnly timestamp As DateTime
        Public ReadOnly url As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)

            Me.id = BitConverter.ToUInt32(data, 4)
            Me.extension = ByteConverter.GetString(data, 8, 4)
            Me.timestamp = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 12))
            Me.filename = ByteConverter.GetNullString(data, &H14)
            Me.url = ByteConverter.GetNullString(data, (21 + Me.filename.Length))

        End Sub

    End Class

    Public Class BnetAuthResponse
        Inherits BsPacket
        ' Fields
        Public ReadOnly info As String
        Public ReadOnly result As BnetAuthResult

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.result = BitConverter.ToInt32(data, 4)
            If (data.Length > 8) Then
                Me.info = ByteConverter.GetNullString(data, 8)
            End If
        End Sub

    End Class

    Public Enum BnetAuthResult
        BannedCDKey = 514
        BuggedVersion = 258
        CDKeyInUse = 513
        InvalidCDKey = 512
        InvalidVersion = 257
        OldVersion = 256
        Success = 0
        WrongProduct = 515
    End Enum

    Public Class BnetConnectionResponse
        Inherits BsPacket
        ' Fields
        Public ReadOnly logonType As UInteger
        Public ReadOnly serverToken As UInteger
        Public ReadOnly udpValue As UInteger
        Public ReadOnly versionFileName As String
        Public ReadOnly versionFileTime As DateTime
        Public ReadOnly versionFormulae As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.logonType = BitConverter.ToUInt32(data, 4)
            Me.serverToken = BitConverter.ToUInt32(data, 8)
            Me.udpValue = BitConverter.ToUInt32(data, 12)
            Me.versionFileTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 16))
            Me.versionFileName = ByteConverter.GetNullString(data, &H18)
            Me.versionFormulae = ByteConverter.GetNullString(data, (&H19 + Me.versionFileName.Length))
        End Sub


    End Class

    Public Class BnetLogonResponse
        Inherits BsPacket

        Public ReadOnly reason As String
        Public ReadOnly result As BnetLogonResult

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.result = BitConverter.ToInt32(data, 4)
            If (data.Length > 7) Then
                Me.reason = ByteConverter.GetNullString(data, 8)
            End If
        End Sub


    End Class

    Public Enum BnetLogonResult
        Success
        AccountDoesNotExist
        PasswordIncorrect
    End Enum

    Public Class BnetPing
        Inherits BsPacket
        ' Fields
        Public ReadOnly timestamp As UInteger

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.timestamp = BitConverter.ToUInt32(data, 4)
        End Sub


    End Class


    Public Class ChannelList
        Inherits BsPacket

        Public ReadOnly Channels As List(Of String)

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.channels = New List(Of String)
            Dim offset As Integer = 4
            Dim i As Integer = 0
            Do While (offset < (data.Length - 1))
                Me.channels.Add(ByteConverter.GetNullString(data, offset))
                offset = (offset + (Me.channels.Item(i).Length + 1))
                i += 1
            Loop
        End Sub


    End Class

    Public Class ChatEvent
        Inherits BsPacket



        ' Fields
        Protected m_account As String
        Protected m_characterAct As Integer
        Protected m_characterFlags As CharacterFlags
        Protected m_characterLevel As Integer
        Protected m_characterTitle As CharacterTitle
        Protected m_characterType As BattleNetCharacter
        Protected m_client As BattleNetClient
        Protected m_clientVersion As Integer
        Protected m_eventType As ChatEventType
        Protected m_flags As UInt32
        Protected m_message As String
        Protected m_name As String
        Protected m_ping As UInt32
        Protected m_realm As String

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.m_clientVersion = -1
            Me.m_characterType = BattleNetCharacter.Unknown
            Me.m_characterLevel = -1
            Me.m_characterAct = -1
            Me.m_characterTitle = CharacterTitle.None

            Me.m_eventType = DirectCast(BitConverter.ToUInt32(data, 4), ChatEventType)

            Me.m_flags = BitConverter.ToUInt32(data, 8)
            Me.m_ping = BitConverter.ToUInt32(data, 12)

            Dim length As Integer = ByteConverter.GetByteOffset(data, 0, 28)
            Dim num2 As Integer = ByteConverter.GetByteOffset(data, 42, 28, length)

            If (num2 > 0) Then
                Me.m_name = ByteConverter.GetString(data, 28, num2)
                length = (length - (num2 + 1))
                num2 = (num2 + 29)
            ElseIf (num2 = 0) Then
                num2 = 29
                length -= 1
                Me.m_characterType = BattleNetCharacter.OpenCharacter
            Else
                num2 = 28
            End If
            Me.m_account = ByteConverter.GetString(data, num2, length)
            length = (length + (num2 + 1))
            If (Me.m_eventType <> ChatEventType.ChannelLeave) Then
                If ((Me.m_eventType = ChatEventType.ChannelJoin) OrElse (Me.m_eventType = ChatEventType.ChannelUser)) Then
                    If ((data.Length - length) > 3) Then
                        Me.m_client = DirectCast(BitConverter.ToUInt32(data, length), BattleNetClient)
                        length = (length + 4)
                    End If
                    If ((((Me.Client <> BattleNetClient.StarcraftShareware) AndAlso (Me.Client <> BattleNetClient.Starcraft)) AndAlso (Me.Client <> BattleNetClient.StarcraftBroodWar)) AndAlso ((Me.Client = BattleNetClient.Diablo2) OrElse (Me.Client = BattleNetClient.Diablo2LoD))) Then
                        If (Me.Client = BattleNetClient.Diablo2LoD) Then
                            Me.m_characterFlags = (Me.CharacterFlags Or CharacterFlags.Expansion)
                        End If
                        If ((data.Length - length) >= 4) Then
                            Me.m_realm = ByteConverter.GetString(data, length, -1, 44)
                            length = (length + (Me.Realm.Length + 1))
                            If (data.Length >= length) Then
                                length = (length + (ByteConverter.GetByteOffset(data, 44, length) + 1))
                                If (((length <> -1) AndAlso (data.Length > length)) AndAlso ((data.Length - length) >= 33)) Then
                                    Struct.StatString.ParseD2StatString(data, length, Me.m_clientVersion, Me.m_characterType, Me.m_characterLevel, Me.m_characterFlags, Me.m_characterAct, Me.m_characterTitle)
                                End If
                            End If
                        End If
                    End If
                Else
                    Me.m_message = ByteConverter.GetNullString(data, length)
                End If
            End If
        End Sub

        ' Properties
        Public ReadOnly Property Account() As String
            Get
                Return Me.m_account
            End Get
        End Property

        Public ReadOnly Property CharacterAct() As Integer
            Get
                Return Me.m_characterAct
            End Get
        End Property

        Public ReadOnly Property CharacterFlags() As CharacterFlags
            Get
                Return Me.m_characterFlags
            End Get
        End Property

        Public ReadOnly Property CharacterLevel() As Integer
            Get
                Return Me.m_characterLevel
            End Get
        End Property

        Public ReadOnly Property CharacterTitle() As CharacterTitle
            Get
                Return Me.m_characterTitle
            End Get
        End Property

        Public ReadOnly Property CharacterType() As BattleNetCharacter
            Get
                Return Me.m_characterType
            End Get
        End Property

        Public ReadOnly Property Client() As BattleNetClient
            Get
                Return Me.m_client
            End Get
        End Property

        Public ReadOnly Property ClientVersion() As Integer
            Get
                Return Me.m_clientVersion
            End Get
        End Property

        Public ReadOnly Property [Event]() As ChatEventType
            Get
                Return Me.m_eventType
            End Get
        End Property

        Public ReadOnly Property Flags() As UInt32
            Get
                Return Me.m_flags
            End Get
        End Property

        Public ReadOnly Property Message() As String
            Get
                Return Me.m_message
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        Public ReadOnly Property Ping() As UInt32
            Get
                Return Me.m_ping
            End Get
        End Property

        Public ReadOnly Property Realm() As String
            Get
                Return Me.m_realm
            End Get
        End Property

    End Class

    Public Enum ChatEventType As UInteger
        Broadcast = 18
        ChannelDoesNotExist = 14
        ChannelFull = 13
        ChannelInfo = 7
        ChannelJoin = 2
        ChannelLeave = 3
        ChannelMessage = 5
        ChannelRestricted = 15
        ChannelUser = 1
        Emote = 23
        [Error] = 19
        ReceiveWhisper = 4
        ServerBroadcast = 6
        UserFlags = 9
        WhisperSent = 10
    End Enum

    Public Class EnterChatResponse
        Inherits BsPacket
        ' Fields
        Public ReadOnly account As String
        Public ReadOnly characterAct As Integer
        Public ReadOnly characterFlags As CharacterFlags
        Public ReadOnly characterLevel As Integer
        Public ReadOnly characterTitle As CharacterTitle
        Public ReadOnly characterType As BattleNetCharacter
        Public ReadOnly client As BattleNetClient
        Public ReadOnly clientVersion As Integer
        Public ReadOnly name As String
        Public ReadOnly realm As String
        Public ReadOnly username As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.clientVersion = -1
            Me.characterType = BattleNetCharacter.Unknown
            Me.characterLevel = -1
            Me.characterAct = -1
            Me.characterTitle = characterTitle.None
            Me.username = ByteConverter.GetNullString(data, 4)
            Dim startIndex As Integer = (5 + Me.username.Length)
            Me.client = DirectCast(BitConverter.ToUInt32(data, startIndex), BattleNetClient)

            If (data(startIndex = (startIndex + 4)) = 0) Then
                Me.account = ByteConverter.GetNullString(data, (startIndex + 1))
            Else
                Me.realm = ByteConverter.GetString(data, startIndex, -1, &H2C)
                startIndex = (startIndex + (1 + Me.realm.Length))
                Me.name = ByteConverter.GetString(data, startIndex, -1, &H2C)
                startIndex = (startIndex + (1 + Me.name.Length))
                Dim num2 As Integer = ByteConverter.GetByteOffset(data, 0, startIndex)
                Me.account = ByteConverter.GetNullString(data, ((startIndex + num2) + 1))
                If (Me.client = BattleNetClient.Diablo2LoD) Then
                    Me.characterFlags = (Me.characterFlags Or characterFlags.Expansion)
                End If
                Struct.StatString.ParseD2StatString(data, startIndex, (Me.clientVersion), (Me.characterType), (Me.characterLevel), (Me.characterFlags), (Me.characterAct), (Me.characterTitle))
            End If
        End Sub

    End Class

    Public Class ExtraWorkInfo
        Inherits BsPacket

        Public ReadOnly filename As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.filename = ByteConverter.GetNullString(data, 4)
        End Sub

    End Class

    Public Class FileTimeInfo
        Inherits BsPacket
        ' Fields
        Public ReadOnly filename As String
        Public ReadOnly Filetime As DateTime
        Public ReadOnly requestID As UInteger

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.requestID = BitConverter.ToUInt32(data, 4)
            Me.Filetime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 12))
            Me.filename = ByteConverter.GetNullString(data, &H14)
        End Sub


    End Class

    Public Class KeepAlive
        Inherits BsPacket
        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure NewsEntry

        Public ReadOnly timestamp As DateTime
        Public ReadOnly content As String

        Public Sub New(ByRef Data As Byte(), ByVal Offset As Integer)

            Me.timestamp = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(Data, Offset))
            Me.content = ByteConverter.GetNullString(Data, Offset + 4)

        End Sub

        Public Overloads Overrides Function ToString() As String
            Return String.Format("Timestamp: {0}, Content: {1}", Me.timestamp, Me.content)
        End Function

    End Structure

    Public Class NewsInfo
        Inherits BsPacket

        ' Fields
        Public ReadOnly EntriesCount As Integer
        Public ReadOnly entries As NewsEntry()
        Public ReadOnly lastLogon As DateTime
        Public ReadOnly newestEntry As DateTime
        Public ReadOnly oldestEntry As DateTime

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)

            Me.EntriesCount = data(4)
            Me.lastLogon = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 5))
            Me.oldestEntry = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 9))
            Me.newestEntry = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 13))
            Me.entries = New NewsEntry(Me.EntriesCount - 1) {}
            Dim offset As Integer = &H11
            Dim i As Integer
            For i = 0 To Me.entries.Length - 1
                Me.entries(i) = New NewsEntry(data, offset)
                offset = (offset + (5 + Me.entries(i).content.Length))
            Next i
        End Sub


    End Class

    Public Class QueryRealmsResponse
        Inherits BsPacket
        ' Fields

        Public ReadOnly RealmsCount As UInteger
        Public ReadOnly Realms As Struct.RealmInfo()

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)

            Me.RealmsCount = BitConverter.ToUInt32(data, 8)
            Me.Realms = New Struct.RealmInfo(Me.Count - 1) {}
            Dim offset As Integer = 12
            Dim i As Integer
            For i = 0 To Me.Count - 1
                Me.Realms(i) = New Struct.RealmInfo(data, offset)
                offset = (offset + ((6 + Me.Realms(i).Name.Length) + Me.Realms(i).Description.Length))
            Next i
        End Sub

    End Class

    Public Class RealmLogonResponse
        Inherits BsPacket

        Public ReadOnly cookie As UInt32
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Public ReadOnly realmServerIP As IPAddress
        Public ReadOnly realmServerPort As Integer
        Public ReadOnly result As RealmLogonResult
        Public ReadOnly unknown As UInt16
        Public ReadOnly username As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.realmServerPort = -1
            Me.cookie = BitConverter.ToUInt32(data, 4)
            If (data.Length < &H4B) Then
                Me.result = DirectCast(BitConverter.ToUInt32(data, 8), RealmLogonResult)
            Else
                Me.result = RealmLogonResult.Success
                Me.realmServerIP = New IPAddress(CLng(BitConverter.ToUInt32(data, &H14)))
                Me.realmServerPort = BEBitConverter.ToUInt16(data, &H18)
                Me.username = ByteConverter.GetNullString(data, &H4C)
                Me.unknown = BitConverter.ToUInt16(data, (&H4C + Me.username.Length))
            End If
        End Sub

        Public ReadOnly Property StartupData() As Byte()
            Get
                If (Me.result <> RealmLogonResult.Success) Then
                    Return Nothing
                End If
                Dim destinationArray As Byte() = New Byte(64 - 1) {}
                Array.Copy(GetData, 0, destinationArray, 0, 16)
                Array.Copy(GetData, 24, destinationArray, 16, 48)
                Return destinationArray
            End Get
        End Property

    End Class

    Public Enum RealmLogonResult As UInteger
        LogonFailed = 2147483650
        RealmUnavailable = 2147483649
        Success = 0
    End Enum

    Public Class RequiredExtraWorkInfo
        Inherits BsPacket
        ' Fields
        Public ReadOnly filename As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.filename = ByteConverter.GetNullString(data, 4)
        End Sub
    End Class

End Namespace
