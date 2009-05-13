Imports System.Runtime.InteropServices

Public Class ChatModuleHost
    Inherits IModuleHost
    Implements IChat

#Region " Base Functions "

    Sub New(ByVal Funcs As IntPtr)
        MyBase.New(Funcs)
    End Sub

    Overrides Sub LoadModules()
        Dim ChatModule As IChatModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableChatModules.Count
            ChatModule = DirectCast(PluginServices.CreateInstance(AvailableChatModules(i)), IChatModule)
            If My.Settings.DisabledModules Is Nothing Then My.Settings.DisabledModules = New Collections.Specialized.StringCollection
            If Not My.Settings.DisabledModules.Contains(ChatModule.Name) Then
                Log.WriteLine("Loading " & ChatModule.Name)
                ChatModule.Initialize(Me)
                LoadedModules.Add(ChatModule)
            End If
        Next
    End Sub

    Public Sub WriteToLog(ByVal Text As String) Implements IChat.WriteToLog
        MyRedVexInfo.WriteLog(Text)
    End Sub

#End Region

#Region " Packet Methods "

    Public Event OnSendPacket(ByRef Packet As Packet) Implements IChat.OnSendPacket
    Public Event OnReceivePacket(ByRef Packet As Packet) Implements IChat.OnReceivePacket

    Public Sub SendPacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IChat.SendPacket

        'Make the Header
        '(Byte) 0xFF
        '(Byte) Packet ID
        '(Word) Packet Size
        '  ...  Packet Core 
        'Dim Bufbyte(length + 3) As Byte
        'Bufbyte(0) = &HFF
        ''PacketID
        'Bufbyte(1) = bytes(0)
        ''Packet Length
        'Bufbyte = PutInArray(Bufbyte, 2, Bufbyte.Length)

        ''Copy the Packet's core
        'For i As Integer = 0 To bytes.Length - 2
        '    Bufbyte(4 + i) = bytes(1 + i)
        'Next

        'RelayDataToServer(Bufbyte, Bufbyte.Length)

        RelayDataToServer(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByRef bytes() As Byte) Implements IChat.SendPacket
        SendPacket(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByVal Packet As D2Packets.D2Packet) Implements IChat.SendPacket
        SendPacket(Packet.Data, Packet.Data.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IChat.ReceivePacket

        'Make the Header
        '(Byte) 0xFF
        '(Byte) Packet ID
        '(Word) Packet Size
        '  ...  Packet Core 
        'Dim Bufbyte(length + 3) As Byte
        'Bufbyte(0) = &HFF
        ''PacketID
        'Bufbyte(1) = bytes(0)
        ''Packet Length
        'Bufbyte = PutInArray(Bufbyte, 2, Bufbyte.Length)

        ''Copy the Packet's core
        'For i As Integer = 0 To bytes.Length - 2
        '    Bufbyte(4 + i) = bytes(1 + i)
        'Next

        'RelayDataToClient(Bufbyte, Bufbyte.Length)

        RelayDataToClient(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte) Implements IChat.ReceivePacket
        ReceivePacket(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByVal Packet As D2Packets.D2Packet) Implements IChat.ReceivePacket
        ReceivePacket(Packet.Data, Packet.Data.Length)
    End Sub

#End Region

#Region " Chat Client Events "

    Public Event OnAdInfoRequest(ByVal Packet As BnetClient.AdInfoRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnAdInfoRequest
    Public Event OnBnetAuthRequest(ByVal Packet As BnetClient.BnetAuthRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetAuthRequest
    Public Event OnBnetConnectionRequest(ByVal Packet As BnetClient.BnetConnectionRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetConnectionRequest
    Public Event OnBnetLogonRequest(ByVal Packet As BnetClient.BnetLogonRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetLogonRequest
    Public Event OnBnetPong(ByVal Packet As BnetClient.BnetPong, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetPong
    Public Event OnChannelListRequest(ByVal Packet As BnetClient.ChannelListRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnChannelListRequest
    Public Event OnChatCommand(ByVal Packet As BnetClient.ChatCommand, ByRef Flag As Packet.PacketFlag) Implements IChat.OnChatCommand
    Public Event OnDisplayAd(ByVal Packet As BnetClient.DisplayAd, ByRef Flag As Packet.PacketFlag) Implements IChat.OnDisplayAd
    Public Event OnEnterChatRequest(ByVal Packet As BnetClient.EnterChatRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnEnterChatRequest
    Public Event OnExtraWorkResponse(ByVal Packet As BnetClient.ExtraWorkResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnExtraWorkResponse
    Public Event OnFileTimeRequest(ByVal Packet As BnetClient.FileTimeRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnFileTimeRequest
    Public Event OnJoinChannel(ByVal Packet As BnetClient.JoinChannel, ByRef Flag As Packet.PacketFlag) Implements IChat.OnJoinChannel
    Public Event OnKeepAlive(ByVal Packet As BnetClient.KeepAlive, ByRef Flag As Packet.PacketFlag) Implements IChat.OnKeepAlive
    Public Event OnLeaveChat(ByVal Packet As BnetClient.LeaveChat, ByRef Flag As Packet.PacketFlag) Implements IChat.OnLeaveChat
    Public Event OnLeaveGame(ByVal Packet As BnetClient.LeaveGame, ByRef Flag As Packet.PacketFlag) Implements IChat.OnLeaveGame
    Public Event OnNewsInfoRequest(ByVal Packet As BnetClient.NewsInfoRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnNewsInfoRequest
    Public Event OnNotifyJoin(ByVal Packet As BnetClient.NotifyJoin, ByRef Flag As Packet.PacketFlag) Implements IChat.OnNotifyJoin
    Public Event OnQueryRealms(ByVal Packet As BnetClient.QueryRealms, ByRef Flag As Packet.PacketFlag) Implements IChat.OnQueryRealms
    Public Event OnRealmLogonRequest(ByVal Packet As BnetClient.RealmLogonRequest, ByRef Flag As Packet.PacketFlag) Implements IChat.OnRealmLogonRequest
    Public Event OnStartGame(ByVal Packet As BnetClient.StartGame, ByRef Flag As Packet.PacketFlag) Implements IChat.OnStartGame

#End Region

#Region " Chat Server Events "

    Public Event OnAdInfo(ByVal packet As BnetServer.AdInfo, ByRef Flag As Packet.PacketFlag) Implements IChat.OnAdInfo
    Public Event OnBnetAuthResponse(ByVal packet As BnetServer.BnetAuthResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetAuthResponse
    Public Event OnBnetConnectionResponse(ByVal packet As BnetServer.BnetConnectionResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetConnectionResponse
    Public Event OnBnetLogonResponse(ByVal packet As BnetServer.BnetLogonResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetLogonResponse
    Public Event OnBnetPing(ByVal packet As BnetServer.BnetPing, ByRef Flag As Packet.PacketFlag) Implements IChat.OnBnetPing
    Public Event OnChannelList(ByVal packet As BnetServer.ChannelList, ByRef Flag As Packet.PacketFlag) Implements IChat.OnChannelList
    Public Event OnChatEvent(ByVal packet As BnetServer.ChatEvent, ByRef Flag As Packet.PacketFlag) Implements IChat.OnChatEvent
    Public Event OnEnterChatResponse(ByVal packet As BnetServer.EnterChatResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnEnterChatResponse
    Public Event OnExtraWorkInfo(ByVal packet As BnetServer.ExtraWorkInfo, ByRef Flag As Packet.PacketFlag) Implements IChat.OnExtraWorkInfo
    Public Event OnFileTimeInfo(ByVal packet As BnetServer.FileTimeInfo, ByRef Flag As Packet.PacketFlag) Implements IChat.OnFileTimeInfo
    Public Event OnNewsInfo(ByVal packet As BnetServer.NewsInfo, ByRef Flag As Packet.PacketFlag) Implements IChat.OnNewsInfo
    Public Event OnQueryRealmsResponse(ByVal packet As BnetServer.QueryRealmsResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnQueryRealmsResponse
    Public Event OnRealmLogonResponse(ByVal packet As BnetServer.RealmLogonResponse, ByRef Flag As Packet.PacketFlag) Implements IChat.OnRealmLogonResponse
    Public Event OnRequiredExtraWorkInfo(ByVal packet As BnetServer.RequiredExtraWorkInfo, ByRef Flag As Packet.PacketFlag) Implements IChat.OnRequiredExtraWorkInfo
    Public Event OnServerKeepAlive(ByVal packet As BnetServer.KeepAlive, ByRef Flag As Packet.PacketFlag) Implements IChat.OnServerKeepAlive

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)

        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag
        Packet.Data = CutBytes(Packet.Data, 0, 1)

        Select Case Packet.Data(0)
            Case D2Packets.BnetClientPacket.AdInfoRequest
                RaiseEvent OnAdInfoRequest(New BnetClient.AdInfoRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.BnetAuthRequest
                RaiseEvent OnBnetAuthRequest(New BnetClient.BnetAuthRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.BnetConnectionRequest
                RaiseEvent OnBnetConnectionRequest(New BnetClient.BnetConnectionRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.BnetLogonRequest
                RaiseEvent OnBnetLogonRequest(New BnetClient.BnetLogonRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.BnetPong
                RaiseEvent OnBnetPong(New BnetClient.BnetPong(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.ChannelListRequest
                RaiseEvent OnChannelListRequest(New BnetClient.ChannelListRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.ChatCommand
                RaiseEvent OnChatCommand(New BnetClient.ChatCommand(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.DisplayAd
                RaiseEvent OnDisplayAd(New BnetClient.DisplayAd(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.EnterChatRequest
                RaiseEvent OnEnterChatRequest(New BnetClient.EnterChatRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.ExtraWorkResponse
                RaiseEvent OnExtraWorkResponse(New BnetClient.ExtraWorkResponse(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.FileTimeRequest
                RaiseEvent OnFileTimeRequest(New BnetClient.FileTimeRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.JoinChannel
                RaiseEvent OnJoinChannel(New BnetClient.JoinChannel(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.KeepAlive
                RaiseEvent OnKeepAlive(New BnetClient.KeepAlive(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.LeaveChat
                RaiseEvent OnLeaveChat(New BnetClient.LeaveChat(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.LeaveGame
                RaiseEvent OnLeaveGame(New BnetClient.LeaveGame(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.NewsInfoRequest
                LastDate = 0
                RaiseEvent OnNewsInfoRequest(New BnetClient.NewsInfoRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.NotifyJoin
                RaiseEvent OnNotifyJoin(New BnetClient.NotifyJoin(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.QueryRealms
                RaiseEvent OnQueryRealms(New BnetClient.QueryRealms(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.RealmLogonRequest
                RaiseEvent OnRealmLogonRequest(New BnetClient.RealmLogonRequest(Packet.Data), Flag)
            Case D2Packets.BnetClientPacket.StartGame
                RaiseEvent OnStartGame(New BnetClient.StartGame(Packet.Data), Flag)
        End Select
        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If
        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag
        Packet.Data = CutBytes(Packet.Data, 0, 1)
        Select Case Packet.Data(0)
            Case D2Packets.BnetServerPacket.AdInfo
                RaiseEvent OnAdInfo(New BnetServer.AdInfo(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.BnetAuthResponse
                RaiseEvent OnBnetAuthResponse(New BnetServer.BnetAuthResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.BnetConnectionResponse
                RaiseEvent OnBnetConnectionResponse(New BnetServer.BnetConnectionResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.BnetLogonResponse
                RaiseEvent OnBnetLogonResponse(New BnetServer.BnetLogonResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.BnetPing
                RaiseEvent OnBnetPing(New BnetServer.BnetPing(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.ChannelList
                RaiseEvent OnChannelList(New BnetServer.ChannelList(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.ChatEvent
                RaiseEvent OnChatEvent(New BnetServer.ChatEvent(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.EnterChatResponse
                RaiseEvent OnEnterChatResponse(New BnetServer.EnterChatResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.ExtraWorkInfo
                RaiseEvent OnExtraWorkInfo(New BnetServer.ExtraWorkInfo(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.FileTimeInfo
                RaiseEvent OnFileTimeInfo(New BnetServer.FileTimeInfo(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.NewsInfo
                RaiseEvent OnNewsInfo(New BnetServer.NewsInfo(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.QueryRealmsResponse
                RaiseEvent OnQueryRealmsResponse(New BnetServer.QueryRealmsResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.RealmLogonResponse
                RaiseEvent OnRealmLogonResponse(New BnetServer.RealmLogonResponse(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.RequiredExtraWorkInfo
                RaiseEvent OnRequiredExtraWorkInfo(New BnetServer.RequiredExtraWorkInfo(Packet.Data), Flag)
            Case D2Packets.BnetServerPacket.KeepAlive
                RaiseEvent OnServerKeepAlive(New BnetServer.KeepAlive(Packet.Data), Flag)
        End Select
        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If
        RaiseEvent OnReceivePacket(Packet)
    End Sub

#End Region

#Region " Helpers "

    Private LastDate As Integer = 0
    Private NewsDate As Integer = 0
    Public Sub AddNews(ByVal Text As String) Implements IChat.AddNews
        Dim News As String = Text

        Dim Buffer(News.Length + 21) As Byte

        Buffer(0) = &HFF
        Buffer(1) = 70
        Buffer(2) = BitConverter.GetBytes(Buffer.Length)(0)
        Buffer(3) = BitConverter.GetBytes(Buffer.Length)(1)
        Buffer(4) = 1

        Dim ts As TimeSpan = (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0))
        Dim unixTime As UInt32 = ts.TotalSeconds + (NewsDate * 86400)
        NewsDate += 1
        LastDate += 1

        Buffer(5) = BitConverter.GetBytes(unixTime)(0)
        Buffer(6) = BitConverter.GetBytes(unixTime)(1)
        Buffer(7) = BitConverter.GetBytes(unixTime)(2)
        Buffer(8) = BitConverter.GetBytes(unixTime)(3)

        Buffer(9) = BitConverter.GetBytes(unixTime - (LastDate * 86400))(0)
        Buffer(10) = BitConverter.GetBytes(unixTime - (LastDate * 86400))(1)
        Buffer(11) = BitConverter.GetBytes(unixTime - (LastDate * 86400))(2)
        Buffer(12) = BitConverter.GetBytes(unixTime - (LastDate * 86400))(3)

        Buffer(13) = BitConverter.GetBytes(unixTime)(0)
        Buffer(14) = BitConverter.GetBytes(unixTime)(1)
        Buffer(15) = BitConverter.GetBytes(unixTime)(2)
        Buffer(16) = BitConverter.GetBytes(unixTime)(3)

        Buffer(17) = BitConverter.GetBytes(unixTime)(0)
        Buffer(18) = BitConverter.GetBytes(unixTime)(1)
        Buffer(19) = BitConverter.GetBytes(unixTime)(2)
        Buffer(20) = BitConverter.GetBytes(unixTime)(3)
        Dim offset As Integer = 21
        For i As Integer = 0 To News.Length - 1
            Buffer(offset) = System.Text.Encoding.UTF32.GetBytes(News(i))(0)
            offset += 1
        Next
        Buffer(offset) = &H0

        Dim info As New BnetServer.NewsInfo(Buffer)

        ReceivePacket(Buffer)
    End Sub

#End Region

End Class