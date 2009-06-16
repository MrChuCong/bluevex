Imports System
Imports D2Data
Imports D2Packets
Imports ETUtils

Namespace RealmClient


    ''' <summary>
    ''' Base class for Battle.net Client Packets
    ''' </summary>
    Public Class RCPacket
        Inherits D2Packet

        Public ReadOnly PacketID As RealmClientPacket

        Public Sub New(ByVal Data As Byte())

            Dim PacketData(Data.Length - 2) As Byte
            Buffer.BlockCopy(Data, 2, PacketData, 0, Data.Length - 3)
            Me.PacketID = PacketData(0)

            MyBase.Insert(PacketData)

        End Sub
        Public Sub New(ByVal Id As RealmClientPacket)
            PacketID = Id
            InsertByte(Id)
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

    ''' <summary>
    ''' RC Packet 0x01 - Realm Startup Request - Request realm connection startup using the information from Bnet server.
    ''' </summary>
    Public Class RealmStartupRequest
        Inherits RCPacket

        Public ReadOnly StartupData As Byte()
        Public ReadOnly UserName As String

        Sub New(ByVal StartupData As Byte(), ByVal UserName As String)
            MyBase.New(RealmClientPacket.RealmStartupRequest)

            InsertByteArray(StartupData)
            InsertCString(UserName)

            Me.StartupData = StartupData
            Me.UserName = UserName

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)

            Dim destinationArray As Byte() = New Byte(64 - 1) {}
            Array.Copy(data, 1, destinationArray, 0, 16)
            Array.Copy(data, 25, destinationArray, 16, 48)
            Me.StartupData = destinationArray

            Me.UserName = ByteConverter.GetNullString(data, &H41)

        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x02 - Character Creation Request - Request creation of a new realm character in the current account.
    ''' </summary>
    Public Class CharacterCreationRequest
        Inherits RCPacket

        Public ReadOnly [Class] As CharacterClass
        Public ReadOnly Flags As CharacterFlags
        Public ReadOnly CharacterName As String

        Sub New(ByVal CharClass As CharacterClass, ByVal CharName As String, ByVal Ladder As Boolean)
            MyBase.New(RealmClientPacket.CharacterCreationRequest)

            InsertUInt32(CharClass)

            If Ladder Then
                InsertInt16(&H60)
                Me.Flags = &H60
            Else
                InsertInt16(&H20)
                Me.Flags = &H20
            End If

            InsertCString(CharName)

            Me.Class = CharClass
            Me.CharacterName = CharName

        End Sub

        Sub New(ByVal Data As Byte())
            MyBase.New(Data)

            Me.Class = BitConverter.ToInt32(Data, 1)
            Me.Flags = BitConverter.ToInt16(Data, 5)
            Me.CharacterName = ByteConverter.GetNullString(Data, 7)

        End Sub

    End Class


    ''' <summary>
    ''' RC Packet 0x03 - Create Game Request - Request to join a game - Must be sent after sucessful game creation
    ''' </summary>
    Public Class CreateGameRequest
        Inherits RCPacket

        Public ReadOnly RequestID As UShort
        Public ReadOnly Difficulty As D2Data.GameDifficulty

        Public ReadOnly MaxPlayers As Byte

        Public ReadOnly GameName As String
        Public ReadOnly Password As String
        Public ReadOnly Description As String
        Public ReadOnly LevelRestriction As SByte

        Public Sub New(ByVal RequestId As UShort, ByVal Difficulty As D2Data.GameDifficulty, ByVal MaxPlayers As Byte, ByVal GameName As String, Optional ByVal Password As String = "", Optional ByVal Description As String = "")
            MyBase.New(RealmClientPacket.CreateGameRequest)

            InsertUInt16(RequestId)
            InsertByte(0)
            InsertByte(Difficulty * 16)

            InsertUInt16(0)
            InsertByte(1)

            InsertByte(&HFF)
            InsertByte(MaxPlayers)

            'InsertCString Function does not include all chars
            For i = 0 To GameName.Length - 1
                InsertByte(System.Convert.ToByte(GameName(i)))
            Next
            InsertByte(0)

            If Password IsNot Nothing Then
                InsertCString(Password)
            Else
                InsertByte(0)
            End If

            If Description IsNot Nothing Then
                InsertCString(Description)
            Else
                InsertByte(0)
            End If

            Me.RequestID = RequestId
            Me.Difficulty = Difficulty
            Me.MaxPlayers = MaxPlayers
            Me.GameName = GameName
            Me.Password = Password
            Me.Description = Description

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Password = Nothing
            Me.Description = Nothing
            Me.RequestID = BitConverter.ToUInt16(data, 1)

            Me.Difficulty = (data(4) >> 4)

            Me.LevelRestriction = CSByte(data(8))
            Me.MaxPlayers = data(9)

            Me.GameName = ByteConverter.GetNullString(data, 10)

            If (data.Length > (13 + Me.GameName.Length)) Then
                Me.Password = ByteConverter.GetNullString(data, (11 + Me.GameName.Length))
            End If
            If (data.Length > ((13 + Me.GameName.Length) + IIf((Me.Password Is Nothing), 0, Me.Password.Length))) Then
                Me.Description = ByteConverter.GetNullString(data, ((12 + Me.GameName.Length) + IIf((Me.Password Is Nothing), 0, Me.Password.Length)))
            End If
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x04 - Join Game Request - Request to join a game - Must be sent after sucessful game creation
    ''' </summary>
    Public Class JoinGameRequest
        Inherits RCPacket

        Public ReadOnly RequestID As UInt16
        Public ReadOnly GameName As String
        Public ReadOnly Password As String

        Public Sub New(ByVal RequestID As UShort, ByVal GameName As String, Optional ByVal Password As String = "")
            MyBase.New(RealmClientPacket.JoinGameRequest)

            InsertUInt16(RequestID)

            'InsertCString Function does not include all chars
            For i = 0 To GameName.Length - 1
                InsertByte(System.Convert.ToByte(GameName(i)))
            Next
            InsertByte(0)

            InsertCString(Password)

            Me.RequestID = RequestID
            Me.GameName = GameName
            Me.Password = Password

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Password = Nothing
            Me.RequestID = BitConverter.ToUInt16(data, 1)
            Me.GameName = ByteConverter.GetNullString(data, 3)
            If (data.Length > (5 + Me.GameName.Length)) Then
                Me.Password = ByteConverter.GetNullString(data, (4 + Me.GameName.Length))
            End If
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x05 - Game List Request - Request a list of availiable games.
    ''' </summary>
    Public Class GameListRequest
        Inherits RCPacket
        ' Fields
        Public ReadOnly RequestID As UInt16

        Public Sub New(ByVal RequestID As UShort)
            MyBase.New(RealmClientPacket.GameListRequest)

            InsertUInt16(RequestID)
            InsertInt32(0)
            InsertByte(0)

            Me.RequestID = RequestID

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.RequestID = BitConverter.ToUInt16(data, 1)
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x06 - Game Info Request - Request information for a particular game.
    ''' </summary>
    Public Class GameInfoRequest
        Inherits RCPacket

        Public ReadOnly GameName As String
        Public ReadOnly RequestID As UInt16

        Public Sub New(ByVal RequestId As UShort, ByVal GameName As String)
            MyBase.New(RealmClientPacket.GameInfoRequest)

            InsertUInt16(RequestId)

            'InsertCString Function does not include all chars
            For i = 0 To GameName.Length - 1
                InsertByte(System.Convert.ToByte(GameName(i)))
            Next
            InsertByte(0)

            Me.RequestID = RequestId
            Me.GameName = GameName

        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.RequestID = BitConverter.ToUInt16(data, 1)
            Me.GameName = ByteConverter.GetNullString(data, 3)
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x07 - Character Logon Request - Requests picking a character (sucess bringing you to lobby...)
    ''' </summary>
    Public Class CharacterLogonRequest
        Inherits RCPacket

        Public ReadOnly Charname As String

        Public Sub New(ByVal Charname As String)
            MyBase.New(RealmClientPacket.CharacterLogonRequest)
            InsertCString(Charname)
            Me.Charname = Charname
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Charname = ByteConverter.GetNullString(data, 1)
        End Sub


    End Class


    ''' <summary>
    ''' RC Packet 0x0A - Character Deletion Request - Request deletion of a realm character in the current account.
    ''' </summary>
    Public Class CharacterDeletionRequest
        Inherits RCPacket

        Public ReadOnly Cookie As UInt32
        Public ReadOnly Charname As String

        Public Sub New(ByVal Charname As String)
            MyBase.New(RealmClientPacket.CharacterDeletionRequest)
            InsertCString(Charname)

            Me.Charname = Charname
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Cookie = BitConverter.ToUInt16(data, 1)
            Me.Charname = ByteConverter.GetNullString(data, 3)
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x12 - Message Of The Day Request - Sent after logon to request RS 0x12
    ''' </summary>
    Public Class MessageOfTheDayRequest
        Inherits RCPacket

        Public Sub New()
            MyBase.New(RealmClientPacket.MessageOfTheDayRequest)
        End Sub

        Public Sub New(ByVal Data() As Byte)
            MyBase.New(Data)
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x13 - Cancel Game Creation - Cancels a currently pending game creation.
    ''' Note: pressing the cancel button after the game was created and client attempts to join won't trigger this packet.
    ''' </summary>
    Public Class CancelGameCreation
        Inherits RCPacket
        Public Sub New(ByVal data As Byte())
            MyBase.New(RealmClientPacket.CancelGameCreation)
        End Sub

    End Class

    ''' <summary>
    ''' RC Packet 0x18 - Character Upgrade Request - Requests upgrading a chracter from classic to expansion.
    ''' </summary>
    Public Class CharacterUpgradeRequest
        Inherits RCPacket
        Public ReadOnly CharName As String

        Public Sub New(ByVal CharName As String)
            MyBase.New(RealmClientPacket.CharacterUpgradeRequest)
            InsertCString(CharName)

            Me.CharName = CharName
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.CharName = ByteConverter.GetNullString(data, 1)
        End Sub


    End Class

    ''' <summary>
    ''' RC Packet 0x19 - Character List Request - Request a list of characters for the current account (with timestamps)
    ''' </summary>
    Public Class CharacterListRequest
        Inherits RCPacket

        Public ReadOnly Number As Integer

        Public Sub New(Optional ByVal Number As Integer = 8)
            MyBase.New(RealmClientPacket.CharacterListRequest)

            InsertInt32(Number)

            Me.Number = Number
        End Sub

        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
            Me.Number = BitConverter.ToInt32(data, 1)
        End Sub

    End Class

End Namespace
