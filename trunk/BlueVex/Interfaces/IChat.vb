Public Interface IChat
    Sub WriteToLog(ByVal Text As String)

#Region " Packet Methods "

    Sub ReceivePacket(ByRef bytes() As Byte)
    Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer)
    Sub ReceivePacket(ByVal Packet As D2Packets.D2Packet)
    Sub SendPacket(ByRef bytes() As Byte)
    Sub SendPacket(ByRef bytes() As Byte, ByVal length As Integer)
    Sub SendPacket(ByVal Packet As D2Packets.D2Packet)
    Event OnReceivePacket(ByRef Packet As Packet)
    Event OnSendPacket(ByRef Packet As Packet)

#End Region

#Region " Chat Client "

    Event OnAdInfoRequest(ByVal Packet As BnetClient.AdInfoRequest, ByRef Flag As Packet.PacketFlag)
    Event OnBnetAuthRequest(ByVal Packet As BnetClient.BnetAuthRequest, ByRef Flag As Packet.PacketFlag)
    Event OnBnetConnectionRequest(ByVal Packet As BnetClient.BnetConnectionRequest, ByRef Flag As Packet.PacketFlag)
    Event OnBnetLogonRequest(ByVal Packet As BnetClient.BnetLogonRequest, ByRef Flag As Packet.PacketFlag)
    Event OnBnetPong(ByVal Packet As BnetClient.BnetPong, ByRef Flag As Packet.PacketFlag)
    Event OnChannelListRequest(ByVal Packet As BnetClient.ChannelListRequest, ByRef Flag As Packet.PacketFlag)
    Event OnChatCommand(ByVal Packet As BnetClient.ChatCommand, ByRef Flag As Packet.PacketFlag)
    Event OnDisplayAd(ByVal Packet As BnetClient.DisplayAd, ByRef Flag As Packet.PacketFlag)
    Event OnEnterChatRequest(ByVal Packet As BnetClient.EnterChatRequest, ByRef Flag As Packet.PacketFlag)
    Event OnExtraWorkResponse(ByVal Packet As BnetClient.ExtraWorkResponse, ByRef Flag As Packet.PacketFlag)
    Event OnFileTimeRequest(ByVal Packet As BnetClient.FileTimeRequest, ByRef Flag As Packet.PacketFlag)
    Event OnJoinChannel(ByVal Packet As BnetClient.JoinChannel, ByRef Flag As Packet.PacketFlag)
    Event OnKeepAlive(ByVal Packet As BnetClient.KeepAlive, ByRef Flag As Packet.PacketFlag)
    Event OnLeaveChat(ByVal Packet As BnetClient.LeaveChat, ByRef Flag As Packet.PacketFlag)
    Event OnLeaveGame(ByVal Packet As BnetClient.LeaveGame, ByRef Flag As Packet.PacketFlag)
    Event OnNewsInfoRequest(ByVal Packet As BnetClient.NewsInfoRequest, ByRef Flag As Packet.PacketFlag)
    Event OnNotifyJoin(ByVal Packet As BnetClient.NotifyJoin, ByRef Flag As Packet.PacketFlag)
    Event OnQueryRealms(ByVal Packet As BnetClient.QueryRealms, ByRef Flag As Packet.PacketFlag)
    Event OnRealmLogonRequest(ByVal Packet As BnetClient.RealmLogonRequest, ByRef Flag As Packet.PacketFlag)
    Event OnStartGame(ByVal Packet As BnetClient.StartGame, ByRef Flag As Packet.PacketFlag)

#End Region

#Region " Chat Server "

    Event OnAdInfo(ByVal packet As BnetServer.AdInfo, ByRef Flag As Packet.PacketFlag)
    Event OnBnetAuthResponse(ByVal packet As BnetServer.BnetAuthResponse, ByRef Flag As Packet.PacketFlag)
    Event OnBnetConnectionResponse(ByVal packet As BnetServer.BnetConnectionResponse, ByRef Flag As Packet.PacketFlag)
    Event OnBnetLogonResponse(ByVal packet As BnetServer.BnetLogonResponse, ByRef Flag As Packet.PacketFlag)
    Event OnBnetPing(ByVal packet As BnetServer.BnetPing, ByRef Flag As Packet.PacketFlag)
    Event OnChannelList(ByVal packet As BnetServer.ChannelList, ByRef Flag As Packet.PacketFlag)
    Event OnChatEvent(ByVal packet As BnetServer.ChatEvent, ByRef Flag As Packet.PacketFlag)
    Event OnEnterChatResponse(ByVal packet As BnetServer.EnterChatResponse, ByRef Flag As Packet.PacketFlag)
    Event OnExtraWorkInfo(ByVal packet As BnetServer.ExtraWorkInfo, ByRef Flag As Packet.PacketFlag)
    Event OnFileTimeInfo(ByVal packet As BnetServer.FileTimeInfo, ByRef Flag As Packet.PacketFlag)
    Event OnServerKeepAlive(ByVal packet As BnetServer.KeepAlive, ByRef Flag As Packet.PacketFlag)
    'Event OnNewsInfo(ByVal packet As BnetServer.NewsInfo, ByRef Flag As Packet.PacketFlag)' Bugged in D2Packets.dll
    Event OnQueryRealmsResponse(ByVal packet As BnetServer.QueryRealmsResponse, ByRef Flag As Packet.PacketFlag)
    Event OnRealmLogonResponse(ByVal packet As BnetServer.RealmLogonResponse, ByRef Flag As Packet.PacketFlag)
    Event OnRequiredExtraWorkInfo(ByVal packet As BnetServer.RequiredExtraWorkInfo, ByRef Flag As Packet.PacketFlag)

#End Region

#Region " Helpers "

    Sub AddNews(ByVal Text As String)

#End Region

End Interface