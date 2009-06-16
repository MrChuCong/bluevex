Imports System

Namespace RealmClient

    Public Enum RealmClientPacket As Byte

        RealmStartupRequest = 1
        CharacterCreationRequest = 2
        CreateGameRequest = 3
        JoinGameRequest = 4
        GameListRequest = 5
        GameInfoRequest = 6
        CharacterLogonRequest = 7
        CharacterDeletionRequest = 10
        MessageOfTheDayRequest = 18
        CancelGameCreation = 19
        CharacterUpgradeRequest = 24
        CharacterListRequest = 25

        Invalid = 48
        ' Probably 0x20, leaving wiggle room just in case...
    End Enum

End Namespace

