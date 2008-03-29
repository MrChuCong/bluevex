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
        RelayDataToServer(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByRef bytes() As Byte) Implements IChat.SendPacket
        SendPacket(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByVal Packet As D2Packets.D2Packet) Implements IChat.SendPacket
        SendPacket(Packet.Data, Packet.Data.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IChat.ReceivePacket
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

    Public Event OnAdInfoRequest(ByVal Packet As BnetClient.AdInfoRequest) Implements IChat.OnAdInfoRequest
    Public Event OnBnetAuthRequest(ByVal Packet As BnetClient.BnetAuthRequest) Implements IChat.OnBnetAuthRequest
    Public Event OnBnetConnectionRequest(ByVal Packet As BnetClient.BnetConnectionRequest) Implements IChat.OnBnetConnectionRequest
    Public Event OnBnetLogonRequest(ByVal Packet As BnetClient.BnetLogonRequest) Implements IChat.OnBnetLogonRequest
    Public Event OnBnetPong(ByVal Packet As BnetClient.BnetPong) Implements IChat.OnBnetPong
    Public Event OnChannelListRequest(ByVal Packet As BnetClient.ChannelListRequest) Implements IChat.OnChannelListRequest
    Public Event OnChatCommand(ByVal Packet As BnetClient.ChatCommand) Implements IChat.OnChatCommand
    Public Event OnDisplayAd(ByVal Packet As BnetClient.DisplayAd) Implements IChat.OnDisplayAd
    Public Event OnEnterChatRequest(ByVal Packet As BnetClient.EnterChatRequest) Implements IChat.OnEnterChatRequest
    Public Event OnExtraWorkResponse(ByVal Packet As BnetClient.ExtraWorkResponse) Implements IChat.OnExtraWorkResponse
    Public Event OnFileTimeRequest(ByVal Packet As BnetClient.FileTimeRequest) Implements IChat.OnFileTimeRequest
    Public Event OnJoinChannel(ByVal Packet As BnetClient.JoinChannel) Implements IChat.OnJoinChannel
    Public Event OnKeepAlive(ByVal Packet As BnetClient.KeepAlive) Implements IChat.OnKeepAlive
    Public Event OnLeaveChat(ByVal Packet As BnetClient.LeaveChat) Implements IChat.OnLeaveChat
    Public Event OnLeaveGame(ByVal Packet As BnetClient.LeaveGame) Implements IChat.OnLeaveGame
    Public Event OnNewsInfoRequest(ByVal Packet As BnetClient.NewsInfoRequest) Implements IChat.OnNewsInfoRequest
    Public Event OnNotifyJoin(ByVal Packet As BnetClient.NotifyJoin) Implements IChat.OnNotifyJoin
    Public Event OnQueryRealms(ByVal Packet As BnetClient.QueryRealms) Implements IChat.OnQueryRealms
    Public Event OnRealmLogonRequest(ByVal Packet As BnetClient.RealmLogonRequest) Implements IChat.OnRealmLogonRequest
    Public Event OnStartGame(ByVal Packet As BnetClient.StartGame) Implements IChat.OnStartGame

#End Region

#Region " Chat Server Events "

    Public Event OnAdInfo(ByVal packet As BnetServer.AdInfo) Implements IChat.OnAdInfo
    Public Event OnBnetAuthResponse(ByVal packet As BnetServer.BnetAuthResponse) Implements IChat.OnBnetAuthResponse
    Public Event OnBnetConnectionResponse(ByVal packet As BnetServer.BnetConnectionResponse) Implements IChat.OnBnetConnectionResponse
    Public Event OnBnetLogonResponse(ByVal packet As BnetServer.BnetLogonResponse) Implements IChat.OnBnetLogonResponse
    Public Event OnBnetPing(ByVal packet As BnetServer.BnetPing) Implements IChat.OnBnetPing
    Public Event OnChannelList(ByVal packet As BnetServer.ChannelList) Implements IChat.OnChannelList
    Public Event OnChatEvent(ByVal packet As BnetServer.ChatEvent) Implements IChat.OnChatEvent
    Public Event OnEnterChatResponse(ByVal packet As BnetServer.EnterChatResponse) Implements IChat.OnEnterChatResponse
    Public Event OnExtraWorkInfo(ByVal packet As BnetServer.ExtraWorkInfo) Implements IChat.OnExtraWorkInfo
    Public Event OnFileTimeInfo(ByVal packet As BnetServer.FileTimeInfo) Implements IChat.OnFileTimeInfo
    'Public Event OnNewsInfo(ByVal packet As BnetServer.NewsInfo) Implements IChat.OnNewsInfo
    Public Event OnQueryRealmsResponse(ByVal packet As BnetServer.QueryRealmsResponse) Implements IChat.OnQueryRealmsResponse
    Public Event OnRealmLogonResponse(ByVal packet As BnetServer.RealmLogonResponse) Implements IChat.OnRealmLogonResponse
    Public Event OnRequiredExtraWorkInfo(ByVal packet As BnetServer.RequiredExtraWorkInfo) Implements IChat.OnRequiredExtraWorkInfo
    Public Event OnServerKeepAlive(ByVal packet As BnetServer.KeepAlive) Implements IChat.OnServerKeepAlive

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)
        Select Case Packet.Data(1)
            Case D2Packets.BnetClientPacket.AdInfoRequest
                RaiseEvent OnAdInfoRequest(New BnetClient.AdInfoRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.BnetAuthRequest
                RaiseEvent OnBnetAuthRequest(New BnetClient.BnetAuthRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.BnetConnectionRequest
                RaiseEvent OnBnetConnectionRequest(New BnetClient.BnetConnectionRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.BnetLogonRequest
                RaiseEvent OnBnetLogonRequest(New BnetClient.BnetLogonRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.BnetPong
                RaiseEvent OnBnetPong(New BnetClient.BnetPong(Packet.Data))
            Case D2Packets.BnetClientPacket.ChannelListRequest
                RaiseEvent OnChannelListRequest(New BnetClient.ChannelListRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.ChatCommand
                RaiseEvent OnChatCommand(New BnetClient.ChatCommand(Packet.Data))
            Case D2Packets.BnetClientPacket.DisplayAd
                RaiseEvent OnDisplayAd(New BnetClient.DisplayAd(Packet.Data))
            Case D2Packets.BnetClientPacket.EnterChatRequest
                RaiseEvent OnEnterChatRequest(New BnetClient.EnterChatRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.ExtraWorkResponse
                RaiseEvent OnExtraWorkResponse(New BnetClient.ExtraWorkResponse(Packet.Data))
            Case D2Packets.BnetClientPacket.FileTimeRequest
                RaiseEvent OnFileTimeRequest(New BnetClient.FileTimeRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.JoinChannel
                RaiseEvent OnJoinChannel(New BnetClient.JoinChannel(Packet.Data))
            Case D2Packets.BnetClientPacket.KeepAlive
                RaiseEvent OnKeepAlive(New BnetClient.KeepAlive(Packet.Data))
            Case D2Packets.BnetClientPacket.LeaveChat
                RaiseEvent OnLeaveChat(New BnetClient.LeaveChat(Packet.Data))
            Case D2Packets.BnetClientPacket.LeaveGame
                RaiseEvent OnLeaveGame(New BnetClient.LeaveGame(Packet.Data))
            Case D2Packets.BnetClientPacket.NewsInfoRequest
                LastDate = 0
                RaiseEvent OnNewsInfoRequest(New BnetClient.NewsInfoRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.NotifyJoin
                RaiseEvent OnNotifyJoin(New BnetClient.NotifyJoin(Packet.Data))
            Case D2Packets.BnetClientPacket.QueryRealms
                RaiseEvent OnQueryRealms(New BnetClient.QueryRealms(Packet.Data))
            Case D2Packets.BnetClientPacket.RealmLogonRequest
                RaiseEvent OnRealmLogonRequest(New BnetClient.RealmLogonRequest(Packet.Data))
            Case D2Packets.BnetClientPacket.StartGame
                RaiseEvent OnStartGame(New BnetClient.StartGame(Packet.Data))
        End Select
        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Select Case Packet.Data(1)
            Case D2Packets.BnetServerPacket.AdInfo
                RaiseEvent OnAdInfo(New BnetServer.AdInfo(Packet.Data))
            Case D2Packets.BnetServerPacket.BnetAuthResponse
                RaiseEvent OnBnetAuthResponse(New BnetServer.BnetAuthResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.BnetConnectionResponse
                RaiseEvent OnBnetConnectionResponse(New BnetServer.BnetConnectionResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.BnetLogonResponse
                RaiseEvent OnBnetLogonResponse(New BnetServer.BnetLogonResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.BnetPing
                RaiseEvent OnBnetPing(New BnetServer.BnetPing(Packet.Data))
            Case D2Packets.BnetServerPacket.ChannelList
                RaiseEvent OnChannelList(New BnetServer.ChannelList(Packet.Data))
            Case D2Packets.BnetServerPacket.ChatEvent
                RaiseEvent OnChatEvent(New BnetServer.ChatEvent(Packet.Data))
            Case D2Packets.BnetServerPacket.EnterChatResponse
                RaiseEvent OnEnterChatResponse(New BnetServer.EnterChatResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.ExtraWorkInfo
                RaiseEvent OnExtraWorkInfo(New BnetServer.ExtraWorkInfo(Packet.Data))
            Case D2Packets.BnetServerPacket.FileTimeInfo
                RaiseEvent OnFileTimeInfo(New BnetServer.FileTimeInfo(Packet.Data))
            Case D2Packets.BnetServerPacket.NewsInfo
                Packet.Flag = BlueVex.Packet.PacketFlag.PacketFlag_Dead
                'RaiseEvent OnNewsInfo(New BnetServer.NewsInfo(Packet.Data))
            Case D2Packets.BnetServerPacket.QueryRealmsResponse
                RaiseEvent OnQueryRealmsResponse(New BnetServer.QueryRealmsResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.RealmLogonResponse
                RaiseEvent OnRealmLogonResponse(New BnetServer.RealmLogonResponse(Packet.Data))
            Case D2Packets.BnetServerPacket.RequiredExtraWorkInfo
                RaiseEvent OnRequiredExtraWorkInfo(New BnetServer.RequiredExtraWorkInfo(Packet.Data))
            Case D2Packets.BnetServerPacket.KeepAlive
                RaiseEvent OnServerKeepAlive(New BnetServer.KeepAlive(Packet.Data))
        End Select
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