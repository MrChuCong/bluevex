Imports D2PacketsVB
Imports D2PacketsVB.D2Packets

Public Interface IRealm
    Sub WriteToLog(ByVal Text As String)

#Region " Packet Methods "

    Sub ReceivePacket(ByRef bytes() As Byte)
    Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer)
    Sub ReceivePacket(ByVal Packet As D2Packet)
    Sub SendPacket(ByRef bytes() As Byte)
    Sub SendPacket(ByRef bytes() As Byte, ByVal length As Integer)
    Sub SendPacket(ByVal Packet As D2Packet)
    Event OnReceivePacket(ByRef Packet As Packet)
    Event OnSendPacket(ByRef Packet As Packet)

#End Region

#Region " Realm Client "

    Event OnCancelGameCreation(ByVal Packet As RealmClient.CancelGameCreation, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterCreationRequest(ByVal Packet As RealmClient.CharacterCreationRequest, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterDeletionRequest(ByVal Packet As RealmClient.CharacterDeletionRequest, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterListRequest(ByVal Packet As RealmClient.CharacterListRequest, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterLogonRequest(ByVal Packet As RealmClient.CharacterLogonRequest, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterUpgradeRequest(ByVal Packet As RealmClient.CharacterUpgradeRequest, ByRef Flag As Packet.PacketFlag)
    Event OnCreateGameRequest(ByVal Packet As RealmClient.CreateGameRequest, ByRef Flag As Packet.PacketFlag)
    Event OnGameInfoRequest(ByVal Packet As RealmClient.GameInfoRequest, ByRef Flag As Packet.PacketFlag)
    Event OnGameListRequest(ByVal Packet As RealmClient.GameListRequest, ByRef Flag As Packet.PacketFlag)
    Event OnJoinGameRequest(ByVal Packet As RealmClient.JoinGameRequest, ByRef Flag As Packet.PacketFlag)
    Event OnMessageOfTheDayRequest(ByVal Packet As RealmClient.MessageOfTheDayRequest, ByRef Flag As Packet.PacketFlag)
    Event OnRealmStartupRequest(ByVal Packet As RealmClient.RealmStartupRequest, ByRef Flag As Packet.PacketFlag)

#End Region

#Region " Realm Server "

    Event OnCharacterCreationResponse(ByVal Packet As RealmServer.CharacterCreationResponse, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterDeletionResponse(ByVal Packet As RealmServer.CharacterDeletionResponse, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterList(ByVal Packet As RealmServer.CharacterList, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterLogonResponse(ByVal Packet As RealmServer.CharacterLogonResponse, ByRef Flag As Packet.PacketFlag)
    Event OnCharacterUpgradeResponse(ByVal Packet As RealmServer.CharacterUpgradeResponse, ByRef Flag As Packet.PacketFlag)
    Event OnCreateGameResponse(ByVal Packet As RealmServer.CreateGameResponse, ByRef Flag As Packet.PacketFlag)
    Event OnGameCreationQueue(ByVal Packet As RealmServer.GameCreationQueue, ByRef Flag As Packet.PacketFlag)
    Event OnGameInfo(ByVal Packet As RealmServer.GameInfo, ByRef Flag As Packet.PacketFlag)
    Event OnGameList(ByVal Packet As RealmServer.GameList, ByRef Flag As Packet.PacketFlag)
    Event OnJoinGameResponse(ByVal Packet As RealmServer.JoinGameResponse, ByRef Flag As Packet.PacketFlag)
    Event OnMessageOfTheDay(ByVal Packet As RealmServer.MessageOfTheDay, ByRef Flag As Packet.PacketFlag)
    Event OnRealmStartupResponse(ByVal Packet As RealmServer.RealmStartupResponse, ByRef Flag As Packet.PacketFlag)

#End Region

End Interface