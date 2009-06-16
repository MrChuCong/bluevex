Imports System
Imports D2Data

Imports ETUtils
Imports D2Packets
Imports System.Globalization

Namespace BnetClient

    Public Class BcPacket
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

            MyBase.InsertByteArray(PacketData)

        End Sub

        Public Overrides ReadOnly Property Data() As Byte()
            Get
                Dim dst As Byte() = New Byte(MyBase.Count + 3) {}
                Dim dat As Byte() = MyBase.GetData
                dst(0) = &HFF
                dst(1) = Packetid
                Dim bytes As Byte() = BitConverter.GetBytes(CUShort((MyBase.Count + 4 And &HFFFF)))
                dst(2) = bytes(0)
                dst(3) = bytes(1)
                Buffer.BlockCopy(dat, 0, dst, 4, dat.Length)
                Return dst
            End Get
        End Property

    End Class

    Public Class AdInfoRequest
        Inherits BcPacket

        ' Fields
        Public ReadOnly Client As BattleNetClient
        Public ReadOnly Id As UInteger
        Public ReadOnly Platform As BattleNetPlatform
        Public ReadOnly Timestamp As DateTime

        Public Sub New(ByVal Id As UInteger, ByVal TimeStamp As UInteger, Optional ByVal Expansion As Boolean = True)
            MyBase.new(BnetClientPacket.AdInfoRequest)

            InsertDwordString("IX86")     ' (DWORD)  Platform Code

            If Expansion Then
                InsertDwordString("D2XP") ' (DWORD)  Product Code
                Me.Client = BattleNetClient.Diablo2LoD
            Else
                InsertDwordString("D2DV") ' (DWORD)  Product Code
                Me.Client = BattleNetClient.Diablo2
            End If

            InsertUInt32(Id)
            InsertUInt32(TimeStamp)

            Me.Platform = BattleNetPlatform.Ix86

            Me.Id = Id
            Me.Timestamp = DateTime.FromBinary(TimeStamp)


        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Platform = DirectCast(BitConverter.ToUInt32(data, 4), BattleNetPlatform)
            Me.Client = DirectCast(BitConverter.ToUInt32(data, 8), BattleNetClient)
            Me.Id = BitConverter.ToUInt32(data, 12)
            Me.Timestamp = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 16))
        End Sub

    End Class


    Public Class BnetAuthRequest
        Inherits BcPacket

        ' Fields
        Public ReadOnly clientToken As UInt32
        Public ReadOnly gameHash As UInt32
        Public ReadOnly gameInfo As String
        Public ReadOnly gameVersion As UInt32
        Public ReadOnly keyCount As UInt32
        Public ReadOnly keys As Struct.CDKeyInfo()
        Public ReadOnly ownerName As String
        Public ReadOnly useSpawn As Boolean

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.clientToken = BitConverter.ToUInt32(data, 3)
            Me.gameVersion = BitConverter.ToUInt32(data, 7)
            Me.gameHash = BitConverter.ToUInt32(data, 11)
            Me.keyCount = BitConverter.ToUInt32(data, 15)
            Me.useSpawn = (BitConverter.ToUInt32(data, &H13) = 1)
            Me.keys = New Struct.CDKeyInfo(Me.keyCount - 1) {}
            Dim offset As Integer = &H17
            Dim i As Integer
            For i = 0 To Me.keyCount - 1
                Me.keys(i) = New Struct.CDKeyInfo(data, offset)
                offset = (offset + &H24)
            Next i
            Me.gameInfo = ByteConverter.GetNullString(data, offset)
            Me.ownerName = ByteConverter.GetNullString(data, ((offset + Me.gameInfo.Length) + 1))
        End Sub

    End Class

    Public Class BnetConnectionRequest
        Inherits BcPacket

        Public ReadOnly client As BattleNetClient
        Public ReadOnly countryAbbreviation As String
        Public ReadOnly countryName As String
        Public Shared ReadOnly CurrentD2LoDVersion As UInt32 = 11
        Public Shared ReadOnly CurrentD2Version As UInt32 = 11
        Public ReadOnly language As UInt32
        Public ReadOnly languageID As UInt32
        Public ReadOnly localeID As UInt32
        Public ReadOnly localIP As Net.IPAddress
        Public ReadOnly platform As BattleNetPlatform
        Public ReadOnly protocol As UInt32
        Public ReadOnly timeZoneBias As UInt32
        Public ReadOnly version As UInt32


        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.protocol = BitConverter.ToUInt32(data, 3)
            Me.platform = DirectCast(BitConverter.ToUInt32(data, 7), BattleNetPlatform)
            Me.client = DirectCast(BitConverter.ToUInt32(data, 11), BattleNetClient)
            Me.version = BitConverter.ToUInt32(data, 15)
            Me.language = BitConverter.ToUInt32(data, &H13)
            Me.localIP = New Net.IPAddress(CLng(BitConverter.ToUInt32(data, &H17)))
            Me.timeZoneBias = BitConverter.ToUInt32(data, &H1B)
            Me.localeID = BitConverter.ToUInt32(data, &H1F)
            Me.languageID = BitConverter.ToUInt32(data, &H23)
            Me.countryAbbreviation = ByteConverter.GetNullString(data, &H27)
            Me.countryName = ByteConverter.GetNullString(data, (40 + Me.countryAbbreviation.Length))
        End Sub

        ' Methods
        Public Sub New(Optional ByVal Expansion As Boolean = True, Optional ByVal VersionByte As Integer = &HC)
            MyBase.New(BnetClientPacket.BnetConnectionRequest)

            InsertInt32(&H0)                                                 ' (DWORD)  Protocol ID
            InsertDwordString("IX86")                                        ' (DWORD)  Platform Code

            If Expansion Then
                InsertDwordString("D2XP")                                    ' (DWORD)  Product Code
            Else
                InsertDwordString("D2DV")                                    ' (DWORD)  Product Code
            End If

            InsertInt32(VersionByte)                                         ' (DWORD)  Version Byte
            InsertInt32(CultureInfo.CurrentUICulture.LCID)                   ' (DWORD)  Product Language
            InsertInt32(&H100007F)                                           ' (DWORD)   Local IP

            InsertInt32(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes) ' (DWORD)  Timezone Bias

            InsertInt32(CultureInfo.CurrentUICulture.LCID)                   ' (DWORD)  Locale ID
            InsertInt32(CultureInfo.CurrentUICulture.LCID)                   ' (DWORD)  Language ID
            InsertCString(RegionInfo.CurrentRegion.ThreeLetterISORegionName) ' (STRING) Country Abbreviation
            InsertCString(RegionInfo.CurrentRegion.DisplayName)              ' (STRING) Country name

        End Sub

    End Class

    Public Class BnetLogonRequest
        Inherits BcPacket

        Public ReadOnly clientToken As UInt32
        Public ReadOnly serverToken As UInt32
        Public ReadOnly username As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)

            Me.clientToken = BitConverter.ToUInt32(data, 4)
            Me.serverToken = BitConverter.ToUInt32(data, 8)
            Me.username = ByteConverter.GetNullString(data, 32)

        End Sub

    End Class

    Public Class BnetPong
        Inherits BcPacket

        Public ReadOnly timestamp As UInt32

        ' Methods
        Public Sub New(ByVal TimeStamp As UInteger)

            MyBase.New(BnetClientPacket.BnetPong)
            InsertUInt32(TimeStamp)

            Me.timestamp = TimeStamp
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.timestamp = BitConverter.ToUInt32(data, 4)
        End Sub


    End Class

    Public Class ChannelListRequest
        Inherits BcPacket

        Public ReadOnly client As BattleNetClient

        ' Methods
        Public Sub New()
            MyBase.New(BnetClientPacket.ChannelListRequest)
            InsertInt32(0)
            client = BattleNetClient.Unknown
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.client = DirectCast(BitConverter.ToUInt32(data, 4), BattleNetClient)
        End Sub

    End Class

    Public Class ChatCommand
        Inherits BcPacket

        Public ReadOnly Message As String

        ' Methods
        Public Sub New(ByVal Message As String)
            MyBase.New(BnetClientPacket.ChatCommand)
            InsertCString(Message, System.Text.Encoding.UTF8)
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Message = ByteConverter.GetNullString(data, 4)
        End Sub


    End Class

    Public Class DisplayAd
        Inherits BcPacket

        Public ReadOnly Client As BattleNetClient
        Public ReadOnly Filename As String
        Public ReadOnly ID As UInt32
        Public ReadOnly Platform As BattleNetPlatform
        Public ReadOnly URL As String

        ' Methods
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Platform = DirectCast(BitConverter.ToUInt32(data, 4), BattleNetPlatform)
            Me.Client = DirectCast(BitConverter.ToUInt32(data, 8), BattleNetClient)
            Me.ID = BitConverter.ToUInt32(data, 12)
            If (data(15) <> 0) Then
                Me.Filename = ByteConverter.GetNullString(data, 16)
            End If
            Dim index As Integer = (17 + IIf((Me.Filename Is Nothing), 0, Me.Filename.Length))
            If (data(index) <> 0) Then
                Me.url = ByteConverter.GetNullString(data, index)
            End If
        End Sub


    End Class

    Public Class EnterChatRequest
        Inherits BcPacket

        Public ReadOnly Name As String
        Public ReadOnly Realm As String

        Public Sub New(ByVal Username As String)
            MyBase.New(BnetClientPacket.EnterChatRequest)

            InsertCString(Username)
            InsertByte(0)

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Name = ByteConverter.GetNullString(data, 4)
            Me.Realm = ByteConverter.GetString(data, (5 + Me.Name.Length), -1, 44)
        End Sub

    End Class

    Public Class ExtraWorkResponse
        Inherits BcPacket

        Public ReadOnly client As Integer
        Public ReadOnly resultLength As Integer

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.client = BitConverter.ToUInt16(data, 4)
            Me.resultLength = BitConverter.ToUInt16(data, 6)
        End Sub

    End Class

    Public Class FileTimeRequest
        Inherits BcPacket

        Public ReadOnly filename As String
        Public ReadOnly requestID As UInt32

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.requestID = BitConverter.ToUInt32(data, 4)
            Me.filename = ByteConverter.GetNullString(data, 12)
        End Sub


    End Class


    Public Enum JoinChannelFlags
        ' Fields
        AutoJoin = 5
        Create = 2
        NormalJoin = 0
    End Enum
    Public Class JoinChannel
        Inherits BcPacket

        Protected Flags As JoinChannelFlags
        Protected Name As String

        Public Sub New(ByVal Name As String, Optional ByVal FirstJoin As Boolean = False)
            MyBase.New(BnetClientPacket.JoinChannel)

            If FirstJoin Then
                InsertInt32(&H5)
                InsertCString("Diablo II")
            Else
                InsertInt32(&H2)
                InsertCString(Name)
            End If

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Flags = BitConverter.ToInt32(data, 4)
            Me.Name = ByteConverter.GetNullString(data, 8)
        End Sub


    End Class

    Public Class KeepAlive
        Inherits BcPacket

        ' Methods
        Public Sub New()
            MyBase.New(BnetClientPacket.KeepAlive)
        End Sub
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

    End Class

    Public Class LeaveChat
        Inherits BcPacket

        Public Sub New()
            MyBase.New(BnetClientPacket.LeaveChat)
        End Sub
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

    End Class

    Public Class LeaveGame
        Inherits BcPacket

        Public Sub New()
            MyBase.New(BnetClientPacket.LeaveGame)
        End Sub
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

    End Class

    Public Class NewsInfoRequest
        Inherits BcPacket
        Public ReadOnly Since As DateTime

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Since = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 4))
        End Sub


    End Class

    Public Class NotifyJoin
        Inherits BcPacket


        Public ReadOnly client As BattleNetClient
        Public ReadOnly name As String
        Public ReadOnly password As String
        Public ReadOnly version As UInt32

        Sub New(ByVal Client As D2Data.BattleNetClient, ByVal GameName As String, Optional ByVal GamePassword As String = "", Optional ByVal Version As Integer = &HC)
            MyBase.New(BnetClientPacket.NotifyJoin)

            InsertUInt32(Client)
            InsertUInt32(Version)
            InsertCString(GameName)
            InsertCString(GamePassword)

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.client = DirectCast(BitConverter.ToUInt32(data, 4), BattleNetClient)
            Me.version = BitConverter.ToUInt32(data, 8)
            Me.name = ByteConverter.GetNullString(data, 12)
            Me.password = ByteConverter.GetNullString(data, (13 + Me.name.Length))
        End Sub


    End Class

    Public Class QueryRealms
        Inherits BcPacket

        ' Methods
        Public Sub New()
            MyBase.New(BnetClientPacket.QueryRealms)
        End Sub
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub
    End Class

    Public Class RealmLogonRequest
        Inherits BcPacket

        Public ReadOnly cookie As UInt32
        Public ReadOnly realm As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.cookie = BitConverter.ToUInt32(data, 4)
            Me.realm = ByteConverter.GetNullString(data, 28)
        End Sub


    End Class

    Public Class StartGame
        Inherits BcPacket

        Public ReadOnly flags As StartGameFlags
        Public ReadOnly name As String
        Public ReadOnly password As String
        Public ReadOnly statString As String

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.flags = BitConverter.ToInt32(data, 4)
            Me.name = ByteConverter.GetNullString(data, 24)
            If ((Me.flags And StartGameFlags.Private) = StartGameFlags.Private) Then
                Me.password = ByteConverter.GetNullString(data, (25 + Me.name.Length))
            End If
            Dim offset As Integer = ((26 + Me.name.Length) + IIf((Me.password Is Nothing), 0, Me.password.Length))
            If (data.Length > (offset + 1)) Then
                Me.statString = ByteConverter.GetNullString(data, offset)
            End If
        End Sub

    End Class

    <Flags()> _
    Public Enum StartGameFlags
        [Public]
        [Private]
    End Enum

End Namespace



