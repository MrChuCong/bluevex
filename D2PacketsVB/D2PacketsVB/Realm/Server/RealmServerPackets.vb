Imports System
Imports D2Data
Imports ETUtils
Imports D2PacketsVB.D2Packets

Namespace RealmServer

    ''' <summary>
    ''' Base class for Battle.net Client Packets
    ''' </summary>
    Public Class RSPacket
        Inherits D2Packet

        Public ReadOnly PacketID As RealmServerPacket

        Public Sub New(ByVal Data As Byte())

            Dim PacketData(Data.Length - 2) As Byte
            Buffer.BlockCopy(Data, 2, PacketData, 0, Data.Length - 3)
            Me.PacketID = PacketData(0)

            MyBase.Insert(PacketData)

        End Sub

        Public Sub New(ByVal ID As RealmServerPacket)
            PacketID = ID
            InsertByte(PacketID)
        End Sub

        Public Overrides ReadOnly Property Data() As Byte()
            Get

                Dim BaseData As Byte() = MyBase.GetData

                Dim PacketBuf(BaseData.Length + 1) As Byte

                Dim len As Byte() = BitConverter.GetBytes(CUShort((Count + 2 And 65535)))

                PacketBuf(0) = len(0)
                PacketBuf(1) = len(1)

                Buffer.BlockCopy(BaseData, 0, PacketBuf, 2, BaseData.Length)

                Return PacketBuf


            End Get
        End Property

    End Class

    Public Enum RealmCharacterActionResult
        Success = 0
        ''' <summary>
        ''' Character already exists or account already has maximum number of characters (currently 8)
        ''' </summary>
        CharacterOverlap = 20
        ''' <summary>
        ''' Character name is longer than 15 characters or contains illegal characters.
        ''' </summary>
        InvalidCharacterName = 21
        ''' <summary>
        ''' Invalid character name specified for action
        ''' </summary>
        CharacterNotFound = 70
        ''' <summary>
        ''' Invalid character name specified for character deletion
        ''' </summary>
        CharacterDoesNotExist = 73
        ''' <summary>
        ''' The action (logon, upgrade, etc. has failed for an unspecified reason)
        ''' </summary>
        Failed = 122
        ''' <summary>
        ''' Cannot perform any action but delete on expired characters...
        ''' </summary>
        CharacterExpired = 123
        ''' <summary>
        ''' When trying to upgrade the character
        ''' </summary>
        CharacterAlreadyExpansion = 124
    End Enum

    Public Enum RealmStartupResult
        Success = 0
        NoBattleNetConnection = 12
        InvalidCDKey = 126
        'TESTME: key in use? key banned? key invalid?
        TemporaryIPBan = 127
        ' "Your connection has been temporarily restricted from this realm. Please try to log in at another time"
    End Enum

    ''' <summary>
    ''' RS Packet 0x01 - Realm Startup - Result of connection start request (RC 0x01)
    ''' </summary>
    Public Class RealmStartupResponse
        Inherits RSPacket
        Public ReadOnly Result As RealmStartupResult

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Result = DirectCast(BitConverter.ToInt32(Me.Data, 3), RealmStartupResult)
        End Sub

        Public Sub New(ByVal Result As RealmStartupResult)
            MyBase.New(RealmServerPacket.RealmStartupResponse)
            InsertInt32(Result)

            Me.Result = Result
        End Sub

    End Class

    ''' <summary>
    ''' RS Packet 0x02 - Character Creation Response - Result of character creation request (reply to RC 0x02)
    ''' </summary>
    Public Class CharacterCreationResponse
        Inherits RSPacket
        Public ReadOnly Result As RealmCharacterActionResult

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Result = DirectCast(BitConverter.ToInt32(Data, 3), RealmCharacterActionResult)
        End Sub

        Public Sub New(ByVal Result As RealmCharacterActionResult)
            MyBase.New(RealmServerPacket.CharacterCreationResponse)
            InsertInt32(Result)

            Me.Result = Result
        End Sub
    End Class

    Public Enum CreateGameResult
        Sucess = 0
        ' This does NOT automatically join the game - the client must also send packet RC 0x04
        InvalidGameName = 30
        GameAlreadyExists = 31
        DeadHardcoreCharacter = 110
        AlreadyInGame = 114
    End Enum

    ''' <summary>
    ''' RS Packet 0x03 - Create Game - Reply to join request (RC 0x04)
    ''' </summary>
    Public Class CreateGameResponse
        Inherits RSPacket
        Public ReadOnly RequestID As UShort
        Public ReadOnly Result As CreateGameResult

        ' If game creation succeeded, this is a nonzero value whose meaning is unknown.
        Public ReadOnly Unknown As UInteger

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.RequestID = BitConverter.ToUInt16(Data, 3)
            Me.Unknown = BitConverter.ToUInt32(Data, 5)

            Me.Result = DirectCast(BitConverter.ToInt32(Data, 9), CreateGameResult)
        End Sub

        Public Sub New(ByVal RequestID As UShort, ByVal Unknown As UInteger, ByVal Result As CreateGameResult)
            MyBase.New(RealmServerPacket.CreateGameResponse)

            InsertUInt16(RequestID)
            InsertUInt32(Unknown)
            InsertInt32(DirectCast(Result, Int32))
        End Sub

    End Class

    Public Enum JoinGameResult
        Sucess = 0
        ' Terminate the connection with the MCP and initiate with D2GS.
        PasswordIncorrect = 41
        GameDoesNotExist = 42
        GameFull = 43
        LevelRequirementsNotMet = 44
        ' You do not meet the level requirements for this game.
        DeadHardcoreCharacter = 110
        ' A dead hardcore character cannot join a game
        UnableToJoinHardcoreGame = 113
        ' A non-hardcore character cannot join a game created by a Hardcore character
        UnableToJoinNightmareGame = 115
        UnableToJoinHellGame = 116
        UnableToJoinExpansionGame = 120
        ' A non-expansion character cannot join a game created by an Expansion character.
        UnableToJoinClassicGame = 121
        ' A Expansion character cannot join a game created by a non-expansion character.
        UnableToJoinLadderGame = 125
        ' A non-ladder character cannot join a game created by a Ladder character.
    End Enum

    ''' <summary>
    ''' RS Packet 0x04 - Join Game - Reply to join request (RC 0x04)
    ''' </summary>
    Public Class JoinGameResponse
        Inherits RSPacket
        Public ReadOnly RequestID As UShort
        Public ReadOnly GameToken As UShort
        Public ReadOnly GameServerIP As System.Net.IPAddress
        Public ReadOnly GameHash As UInteger
        Public ReadOnly Result As JoinGameResult
        Public ReadOnly Unknown As UShort

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.RequestID = BitConverter.ToUInt16(Data, 3)
            Me.GameToken = BitConverter.ToUInt16(Data, 5)
            Me.Unknown = BitConverter.ToUInt16(Data, 7)
            Me.GameServerIP = New System.Net.IPAddress(BitConverter.ToUInt32(Data, 9))
            Me.GameHash = BitConverter.ToUInt32(Data, 13)
            Me.Result = DirectCast(BitConverter.ToInt32(Data, 17), JoinGameResult)
        End Sub

        Public Sub New(ByVal RequestId As UShort, ByVal GameToken As UShort, ByVal Unknown As UShort, ByVal GameServerip As System.Net.IPAddress, ByVal GameHash As UInteger, ByVal Result As JoinGameResult)
            MyBase.New(RealmServerPacket.JoinGameResponse)
            InsertUInt16(RequestId)
            InsertUInt16(GameToken)
            InsertUInt16(Unknown)
            InsertByteArray(GameServerip.GetAddressBytes)
            InsertUInt32(GameHash)
            InsertInt32(Result)

            Me.RequestID = RequestId
            Me.GameToken = GameToken
            Me.Unknown = Unknown
            Me.GameServerIP = GameServerip
            Me.GameHash = GameHash
            Me.Result = Result

        End Sub
    End Class

    ''' <summary>
    ''' RS Packet 0x05 - Game List - Availiable game to list (sent once for each game, in reply to RC 0x05)
    ''' </summary>
    Public Class GameList
        Inherits RSPacket
        ' Fields


        Public ReadOnly description As String
        Public ReadOnly flags As GameFlags
        Public ReadOnly index As UInteger
        Public ReadOnly name As String
        Public ReadOnly playerCount As Byte
        Public ReadOnly requestID As UShort

        ' Methods
        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.requestID = BitConverter.ToUInt16(Data, 3)
            Me.index = BitConverter.ToUInt32(Data, 5)
            Me.playerCount = Data(9)
            Me.flags = DirectCast(BitConverter.ToUInt32(Data, 10), GameFlags)
            If (Me.flags And GameFlags.Valid) = GameFlags.Valid Then
                Me.name = ByteConverter.GetNullString(Data, 14)
                If Data.Length > (16 + Me.name.Length) Then
                    Me.description = ByteConverter.GetNullString(Data, 15 + Me.name.Length)
                End If
            End If
        End Sub

        Public Sub New(ByVal requestid As UShort, ByVal Index As UInteger, ByVal PlayerCount As Byte, ByVal Flags As GameFlags, ByVal Name As String, ByVal description As String)
            MyBase.New(RealmServerPacket.GameList)
            InsertUInt16(requestid)
            InsertUInt32(Index)
            InsertByte(PlayerCount)
            InsertUInt32(DirectCast(Flags, UInt32))
            If (Flags And GameFlags.Valid) = GameFlags.Valid Then
                InsertCString(Name)
                InsertCString(description)
            End If

            InsertByte(0)


            Me.requestID = requestid
            Me.index = Index
            Me.playerCount = PlayerCount
            Me.flags = Flags
            Me.name = Name
            Me.description = description

        End Sub

    End Class

    Public Class CharacterBaseInfo
        Public Name As String
        Public [Class] As CharacterClass
        Public Level As Integer

        Public Sub New(ByVal name As String, ByVal charClass As Integer, ByVal level As Integer)
            Me.Name = name
            Me.[Class] = DirectCast(charClass, CharacterClass)
            Me.Level = level
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return StringUtils.ToFormatedInfoString(Me, False, ": ", ", ")
        End Function
    End Class

    <Flags()> _
    Public Enum GameFlags As UInteger
        Empty = 131072
        Expansion = 1048576
        GameDestroyed = 4294967294
        Hardcore = 2048
        Hell = 8192
        Ladder = 2097152
        Nightmare = 4096
        ServerDown = 4294967295
        Valid = 4
    End Enum

    ''' <summary>
    ''' RS Packet 0x06 - Game Info - Provides information about a particular game (reply to RC 0x06)
    ''' </summary>
    Public Class GameInfo
        Inherits RSPacket
        ' Fields
        Protected m_creatorLevel As Integer
        Protected m_flags As GameFlags
        Protected m_levelRestriction As Integer
        Protected m_maxLevel As Integer
        Protected m_maxPlayers As Integer
        Protected m_minLevel As Integer
        Public Shared ReadOnly NULL_Int32 As Integer = -1
        Protected m_playerCount As Integer
        Protected m_players As CharacterBaseInfo()
        Protected m_requestID As UShort
        Protected m_uptime As TimeSpan

        ' Methods
        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.m_minLevel = -1
            Me.m_maxLevel = -1

            Me.m_requestID = BitConverter.ToUInt16(Data, 3)
            Me.m_flags = DirectCast(BitConverter.ToUInt32(Data, 5), GameFlags)
            Me.m_uptime = New TimeSpan(BitConverter.ToUInt32(Data, 9) * 10000000)
            Me.m_creatorLevel = Data(13)
            Me.m_levelRestriction = CSByte(Data(16))
            If Data(12) <> 255 Then
                Me.m_minLevel = Math.Max(1, Data(15) - Data(16))
                Me.m_maxLevel = Math.Min(99, Data(15) + Data(16))
            End If
            Me.m_maxPlayers = Data(17)
            Me.m_playerCount = Data(18)
            Me.m_players = New CharacterBaseInfo(Me.m_playerCount - 1) {}
            Dim offset As Integer = 52
            For i As Integer = 0 To Me.m_playerCount - 1
                Me.m_players(i) = New CharacterBaseInfo(ByteConverter.GetNullString(Data, offset), Data(19 + i), Data(35 + i))
                offset += Me.m_players(i).Name.Length + 1
            Next
        End Sub

        ' Properties
        Public ReadOnly Property CreatorLevel() As Integer
            Get
                Return Me.m_creatorLevel
            End Get
        End Property

        Public ReadOnly Property Flags() As GameFlags
            Get
                Return Me.m_flags
            End Get
        End Property

        Public ReadOnly Property LevelRestriction() As Integer
            Get
                Return Me.m_levelRestriction
            End Get
        End Property

        Public ReadOnly Property MaxLevel() As Integer
            Get
                Return Me.m_maxLevel
            End Get
        End Property

        Public ReadOnly Property MaxPlayers() As Integer
            Get
                Return Me.m_maxPlayers
            End Get
        End Property

        Public ReadOnly Property MinLevel() As Integer
            Get
                Return Me.m_minLevel
            End Get
        End Property

        Public ReadOnly Property PlayerCount() As Integer
            Get
                Return Me.m_playerCount
            End Get
        End Property

        Public ReadOnly Property Players() As CharacterBaseInfo()
            Get
                Return Me.m_players
            End Get
        End Property

        Public ReadOnly Property RequestID() As UShort
            Get
                Return Me.m_requestID
            End Get
        End Property

        Public ReadOnly Property Uptime() As TimeSpan
            Get
                Return Me.m_uptime
            End Get
        End Property
    End Class

    ''' <summary>
    ''' RS Packet 0x07 - Character Logon Response - Character log attempt result, sent in reply to RC 0x07
    ''' </summary>
    Public Class CharacterLogonResponse
        Inherits RSPacket
        Public ReadOnly Result As RealmCharacterActionResult

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Result = DirectCast(BitConverter.ToInt32(Data, 5), RealmCharacterActionResult)
        End Sub

        Public Sub New(ByVal Result As RealmCharacterActionResult)
            MyBase.New(RealmServerPacket.CharacterLogonResponse)
            InsertInt32(Result)
            Me.Result = Result
        End Sub
    End Class

    ''' <summary>
    ''' RS Packet 0x0A - Character Deletion Response - Result of character deletion, sent in reply to RC 0x0A
    ''' </summary>
    Public Class CharacterDeletionResponse
        Inherits RSPacket
        Public ReadOnly Result As RealmCharacterActionResult

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Result = DirectCast(BitConverter.ToInt32(Data, 3), RealmCharacterActionResult)
        End Sub

        Public Sub New(ByVal Result As RealmCharacterActionResult)
            MyBase.New(RealmServerPacket.CharacterDeletionResponse)
            InsertInt32(Result)
            Me.Result = Result
        End Sub
    End Class

    ''' <summary>
    ''' RS Packet 0x12 - Message of the Day - Sent after logon in reply to RC 0x12.
    ''' More like message of the year ^^
    ''' </summary>
    Public Class MessageOfTheDay
        Inherits RSPacket
        Public ReadOnly Message As String

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            ' unknown : starting bytes before first 0...
            ' supposedly some kind of header but sometimes null, otherwise control characters so unlikely to be a string...
            Dim offset As Integer = 3
            While Data(System.Math.Max(System.Threading.Interlocked.Increment(offset), offset - 1)) <> 0
                Continue While
            End While
            Me.Message = ByteConverter.GetNullString(Data, offset)
        End Sub
    End Class

    ''' <summary>
    ''' RS Packet 0x14 - Game Creation Queue - Initinialise waiting queue or update queue position.
    ''' </summary>
    Public Class GameCreationQueue
        Inherits RSPacket
        Public ReadOnly Position As UInteger

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Position = BitConverter.ToUInt32(Data, 3)
        End Sub
        Public Sub New(ByVal Position As UInteger)
            MyBase.New(RealmServerPacket.GameCreationQueue)
            InsertUInt32(Position)

            Me.Position = Position
        End Sub

    End Class

    ''' <summary>
    ''' RS Packet 0x18 - CharacterUpgradeResult - Character upgrade attempt result, sent in reply to RC 0x18
    ''' </summary>
    Public Class CharacterUpgradeResponse
        Inherits RSPacket
        Public ReadOnly Result As RealmCharacterActionResult

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Result = Convert.ToInt32(BitConverter.ToUInt32(Data, 3))
        End Sub

    End Class

    ''' <summary>
    ''' RS Packet 0x19 - Character List - Request a list of characters for the current account (with timestamps)
    ''' </summary>
    Public Class CharacterList
        Inherits RSPacket
        Public ReadOnly Requested As UInteger
        Public ReadOnly Total As UInteger
        Public ReadOnly Listed As UInteger
        Public ReadOnly Characters As D2Packets.CharacterInfo()

        Public Sub New(ByVal PacketData As Byte())
            MyBase.New(PacketData)
            Me.Requested = BitConverter.ToUInt16(Data, 3)
            Me.Total = BitConverter.ToUInt32(Data, 5)
            Me.Listed = BitConverter.ToUInt16(Data, 9)
            Me.Characters = New D2Packets.CharacterInfo(Me.Listed - 1) {}
            Dim startIndex As Integer = 11
            Dim i As Integer = 0

            While (i < Me.Listed) AndAlso (startIndex < Data.Length)
                Me.Characters(i) = New D2Packets.CharacterInfo()
                Me.Characters(i).Expires = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(Data, startIndex))

                startIndex += 4
                Me.Characters(i).Name = ByteConverter.GetNullString(Data, startIndex)
                startIndex += Me.Characters(i).Name.Length + 1

                StatString.ParseD2StatString(Data, startIndex, Me.Characters(i).ClientVersion, Me.Characters(i).[Class], Me.Characters(i).Level, Me.Characters(i).Flags, _
                 Me.Characters(i).Act, Me.Characters(i).Title)

                startIndex = ByteConverter.GetBytePosition(Data, 0, startIndex) + 1
                i += 1
            End While

        End Sub
    End Class

End Namespace
