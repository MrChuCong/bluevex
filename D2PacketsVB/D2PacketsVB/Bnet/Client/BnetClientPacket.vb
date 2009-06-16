Namespace BnetClient
    Public Enum BnetClientPacket

        KeepAlive = 0
        EnterChatRequest = 10
        ChannelListRequest = 11
        JoinChannel = 12
        ChatCommand = 14
        LeaveChat = 16
        AdInfoRequest = 21
        StartGame = 28
        LeaveGame = 31
        DisplayAd = 33
        NotifyJoin = 34
        BnetPong = 37
        FileTimeRequest = 51
        BnetLogonRequest = 58
        RealmLogonRequest = 62
        QueryRealms = 64
        NewsInfoRequest = 70
        ExtraWorkResponse = 75
        BnetConnectionRequest = 80
        BnetAuthRequest = 81

        Invalid = 131

    End Enum
End Namespace
