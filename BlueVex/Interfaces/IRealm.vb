Public Interface IRealm
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

#Region " Realm Client "

    Event OnCancelGameCreation(ByVal Packet As RealmClient.CancelGameCreation)
    Event OnCharacterCreationRequest(ByVal Packet As RealmClient.CharacterCreationRequest)
    Event OnCharacterDeletionRequest(ByVal Packet As RealmClient.CharacterDeletionRequest)
    Event OnCharacterListRequest(ByVal Packet As RealmClient.CharacterListRequest)
    Event OnCharacterLogonRequest(ByVal Packet As RealmClient.CharacterLogonRequest)
    Event OnCharacterUpgradeRequest(ByVal Packet As RealmClient.CharacterUpgradeRequest)
    Event OnCreateGameRequest(ByVal Packet As RealmClient.CreateGameRequest)
    Event OnGameInfoRequest(ByVal Packet As RealmClient.GameInfoRequest)
    Event OnGameListRequest(ByVal Packet As RealmClient.GameListRequest)
    Event OnJoinGameRequest(ByVal Packet As RealmClient.JoinGameRequest)
    Event OnMessageOfTheDayRequest(ByVal Packet As RealmClient.MessageOfTheDayRequest)
    Event OnRealmStartupRequest(ByVal Packet As RealmClient.RealmStartupRequest)

#End Region

#Region " Realm Server "

    Event OnCharacterCreationResponse(ByVal Packet As RealmServer.CharacterCreationResponse)
    Event OnCharacterDeletionResponse(ByVal Packet As RealmServer.CharacterDeletionResponse)
    Event OnCharacterList(ByVal Packet As RealmServer.CharacterList)
    Event OnCharacterLogonResponse(ByVal Packet As RealmServer.CharacterLogonResponse)
    Event OnCharacterUpgradeResponse(ByVal Packet As RealmServer.CharacterUpgradeResponse)
    Event OnCreateGameResponse(ByVal Packet As RealmServer.CreateGameResponse)
    Event OnGameCreationQueue(ByVal Packet As RealmServer.GameCreationQueue)
    Event OnGameInfo(ByVal Packet As RealmServer.GameInfo)
    Event OnGameList(ByVal Packet As RealmServer.GameList)
    Event OnJoinGameResponse(ByVal Packet As RealmServer.JoinGameResponse)
    Event OnMessageOfTheDay(ByVal Packet As RealmServer.MessageOfTheDay)
    Event OnRealmStartupResponse(ByVal Packet As RealmServer.RealmStartupResponse)

#End Region

End Interface