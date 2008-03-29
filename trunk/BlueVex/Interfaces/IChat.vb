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

    Event OnAdInfoRequest(ByVal Packet As BnetClient.AdInfoRequest)
    Event OnBnetAuthRequest(ByVal Packet As BnetClient.BnetAuthRequest)
    Event OnBnetConnectionRequest(ByVal Packet As BnetClient.BnetConnectionRequest)
    Event OnBnetLogonRequest(ByVal Packet As BnetClient.BnetLogonRequest)
    Event OnBnetPong(ByVal Packet As BnetClient.BnetPong)
    Event OnChannelListRequest(ByVal Packet As BnetClient.ChannelListRequest)
    Event OnChatCommand(ByVal Packet As BnetClient.ChatCommand)
    Event OnDisplayAd(ByVal Packet As BnetClient.DisplayAd)
    Event OnEnterChatRequest(ByVal Packet As BnetClient.EnterChatRequest)
    Event OnExtraWorkResponse(ByVal Packet As BnetClient.ExtraWorkResponse)
    Event OnFileTimeRequest(ByVal Packet As BnetClient.FileTimeRequest)
    Event OnJoinChannel(ByVal Packet As BnetClient.JoinChannel)
    Event OnKeepAlive(ByVal Packet As BnetClient.KeepAlive)
    Event OnLeaveChat(ByVal Packet As BnetClient.LeaveChat)
    Event OnLeaveGame(ByVal Packet As BnetClient.LeaveGame)
    Event OnNewsInfoRequest(ByVal Packet As BnetClient.NewsInfoRequest)
    Event OnNotifyJoin(ByVal Packet As BnetClient.NotifyJoin)
    Event OnQueryRealms(ByVal Packet As BnetClient.QueryRealms)
    Event OnRealmLogonRequest(ByVal Packet As BnetClient.RealmLogonRequest)
    Event OnStartGame(ByVal Packet As BnetClient.StartGame)

#End Region

#Region " Chat Server "

    Event OnAdInfo(ByVal packet As BnetServer.AdInfo)
    Event OnBnetAuthResponse(ByVal packet As BnetServer.BnetAuthResponse)
    Event OnBnetConnectionResponse(ByVal packet As BnetServer.BnetConnectionResponse)
    Event OnBnetLogonResponse(ByVal packet As BnetServer.BnetLogonResponse)
    Event OnBnetPing(ByVal packet As BnetServer.BnetPing)
    Event OnChannelList(ByVal packet As BnetServer.ChannelList)
    Event OnChatEvent(ByVal packet As BnetServer.ChatEvent)
    Event OnEnterChatResponse(ByVal packet As BnetServer.EnterChatResponse)
    Event OnExtraWorkInfo(ByVal packet As BnetServer.ExtraWorkInfo)
    Event OnFileTimeInfo(ByVal packet As BnetServer.FileTimeInfo)
    Event OnServerKeepAlive(ByVal packet As BnetServer.KeepAlive)
    'Event OnNewsInfo(ByVal packet As BnetServer.NewsInfo)' Bugged in D2Packets.dll
    Event OnQueryRealmsResponse(ByVal packet As BnetServer.QueryRealmsResponse)
    Event OnRealmLogonResponse(ByVal packet As BnetServer.RealmLogonResponse)
    Event OnRequiredExtraWorkInfo(ByVal packet As BnetServer.RequiredExtraWorkInfo)

#End Region

#Region " Helpers "

    Sub AddNews(ByVal Text As String)

#End Region

End Interface