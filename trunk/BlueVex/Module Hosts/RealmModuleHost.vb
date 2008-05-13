Public Class RealmModuleHost
    Inherits IModuleHost
    Implements IRealm

#Region " Base Functions "

    Sub New(ByVal Funcs As IntPtr)
        MyBase.New(Funcs)
    End Sub

    Overrides Sub LoadModules()
        Dim RealmModule As IRealmModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableRealmModules.Count
            RealmModule = DirectCast(PluginServices.CreateInstance(AvailableRealmModules(i)), IRealmModule)
            If My.Settings.DisabledModules Is Nothing Then My.Settings.DisabledModules = New Collections.Specialized.StringCollection
            If Not My.Settings.DisabledModules.Contains(RealmModule.Name) Then
                Log.WriteLine("Loading " & RealmModule.Name)
                RealmModule.Initialize(Me)
                LoadedModules.Add(RealmModule)
            End If
        Next
    End Sub

    Public Sub WriteToLog(ByVal Text As String) Implements IRealm.WriteToLog
        MyRedVexInfo.WriteLog(Text)
    End Sub

#End Region

#Region " Packet Methods "

    Public Event OnSendPacket(ByRef Packet As Packet) Implements IRealm.OnSendPacket
    Public Event OnReceivePacket(ByRef Packet As Packet) Implements IRealm.OnReceivePacket

    Public Sub SendPacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IRealm.SendPacket
        RelayDataToServer(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByRef bytes() As Byte) Implements IRealm.SendPacket
        SendPacket(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByVal Packet As D2Packets.D2Packet) Implements IRealm.SendPacket
        SendPacket(Packet.Data, Packet.Data.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IRealm.ReceivePacket
        RelayDataToClient(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte) Implements IRealm.ReceivePacket
        ReceivePacket(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByVal Packet As D2Packets.D2Packet) Implements IRealm.ReceivePacket
        ReceivePacket(Packet.Data, Packet.Data.Length)
    End Sub

#End Region

#Region " Realm Client Events "

    Public Event OnCancelGameCreation(ByVal Packet As RealmClient.CancelGameCreation) Implements IRealm.OnCancelGameCreation
    Public Event OnCharacterCreationRequest(ByVal Packet As RealmClient.CharacterCreationRequest) Implements IRealm.OnCharacterCreationRequest
    Public Event OnCharacterDeletionRequest(ByVal Packet As RealmClient.CharacterDeletionRequest) Implements IRealm.OnCharacterDeletionRequest
    Public Event OnCharacterListRequest(ByVal Packet As RealmClient.CharacterListRequest) Implements IRealm.OnCharacterListRequest
    Public Event OnCharacterLogonRequest(ByVal Packet As RealmClient.CharacterLogonRequest) Implements IRealm.OnCharacterLogonRequest
    Public Event OnCharacterUpgradeRequest(ByVal Packet As RealmClient.CharacterUpgradeRequest) Implements IRealm.OnCharacterUpgradeRequest
    Public Event OnCreateGameRequest(ByVal Packet As RealmClient.CreateGameRequest) Implements IRealm.OnCreateGameRequest
    Public Event OnGameInfoRequest(ByVal Packet As RealmClient.GameInfoRequest) Implements IRealm.OnGameInfoRequest
    Public Event OnGameListRequest(ByVal Packet As RealmClient.GameListRequest) Implements IRealm.OnGameListRequest
    Public Event OnJoinGameRequest(ByVal Packet As RealmClient.JoinGameRequest) Implements IRealm.OnJoinGameRequest
    Public Event OnMessageOfTheDayRequest(ByVal Packet As RealmClient.MessageOfTheDayRequest) Implements IRealm.OnMessageOfTheDayRequest
    Public Event OnRealmStartupRequest(ByVal Packet As RealmClient.RealmStartupRequest) Implements IRealm.OnRealmStartupRequest

#End Region

#Region " Realm Server Events "

    Public Event OnCharacterCreationResponse(ByVal Packet As RealmServer.CharacterCreationResponse) Implements IRealm.OnCharacterCreationResponse
    Public Event OnCharacterDeletionResponse(ByVal Packet As RealmServer.CharacterDeletionResponse) Implements IRealm.OnCharacterDeletionResponse
    Public Event OnCharacterList(ByVal Packet As RealmServer.CharacterList) Implements IRealm.OnCharacterList
    Public Event OnCharacterLogonResponse(ByVal Packet As RealmServer.CharacterLogonResponse) Implements IRealm.OnCharacterLogonResponse
    Public Event OnCharacterUpgradeResponse(ByVal Packet As RealmServer.CharacterUpgradeResponse) Implements IRealm.OnCharacterUpgradeResponse
    Public Event OnCreateGameResponse(ByVal Packet As RealmServer.CreateGameResponse) Implements IRealm.OnCreateGameResponse
    Public Event OnGameCreationQueue(ByVal Packet As RealmServer.GameCreationQueue) Implements IRealm.OnGameCreationQueue
    Public Event OnGameInfo(ByVal Packet As RealmServer.GameInfo) Implements IRealm.OnGameInfo
    Public Event OnGameList(ByVal Packet As RealmServer.GameList) Implements IRealm.OnGameList
    Public Event OnJoinGameResponse(ByVal Packet As RealmServer.JoinGameResponse) Implements IRealm.OnJoinGameResponse
    Public Event OnMessageOfTheDay(ByVal Packet As RealmServer.MessageOfTheDay) Implements IRealm.OnMessageOfTheDay
    Public Event OnRealmStartupResponse(ByVal Packet As RealmServer.RealmStartupResponse) Implements IRealm.OnRealmStartupResponse

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)
        Select Case Packet.Data(2)
            Case D2Packets.RealmClientPacket.CancelGameCreation
                RaiseEvent OnCancelGameCreation(New RealmClient.CancelGameCreation(Packet.Data))
            Case D2Packets.RealmClientPacket.CharacterCreationRequest
                RaiseEvent OnCharacterCreationRequest(New RealmClient.CharacterCreationRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.CharacterDeletionRequest
                RaiseEvent OnCharacterDeletionRequest(New RealmClient.CharacterDeletionRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.CharacterListRequest
                RaiseEvent OnCharacterListRequest(New RealmClient.CharacterListRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.CharacterLogonRequest
                RaiseEvent OnCharacterLogonRequest(New RealmClient.CharacterLogonRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.CharacterUpgradeRequest
                RaiseEvent OnCharacterUpgradeRequest(New RealmClient.CharacterUpgradeRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.CreateGameRequest
                RaiseEvent OnCreateGameRequest(New RealmClient.CreateGameRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.GameInfoRequest
                RaiseEvent OnGameInfoRequest(New RealmClient.GameInfoRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.GameListRequest
                RaiseEvent OnGameListRequest(New RealmClient.GameListRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.JoinGameRequest
                RaiseEvent OnJoinGameRequest(New RealmClient.JoinGameRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.MessageOfTheDayRequest
                RaiseEvent OnMessageOfTheDayRequest(New RealmClient.MessageOfTheDayRequest(Packet.Data))
            Case D2Packets.RealmClientPacket.RealmStartupRequest
                RaiseEvent OnRealmStartupRequest(New RealmClient.RealmStartupRequest(Packet.Data))
        End Select
        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Select Case Packet.Data(2)
            Case D2Packets.RealmServerPacket.CharacterCreationResponse
                RaiseEvent OnCharacterCreationResponse(New RealmServer.CharacterCreationResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.CharacterDeletionResponse
                RaiseEvent OnCharacterDeletionResponse(New RealmServer.CharacterDeletionResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.CharacterList
                RaiseEvent OnCharacterList(New RealmServer.CharacterList(Packet.Data))
            Case D2Packets.RealmServerPacket.CharacterLogonResponse
                RaiseEvent OnCharacterLogonResponse(New RealmServer.CharacterLogonResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.CharacterUpgradeResponse
                RaiseEvent OnCharacterUpgradeResponse(New RealmServer.CharacterUpgradeResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.CreateGameResponse
                RaiseEvent OnCreateGameResponse(New RealmServer.CreateGameResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.GameCreationQueue
                RaiseEvent OnGameCreationQueue(New RealmServer.GameCreationQueue(Packet.Data))
            Case D2Packets.RealmServerPacket.GameInfo
                RaiseEvent OnGameInfo(New RealmServer.GameInfo(Packet.Data))
            Case D2Packets.RealmServerPacket.GameList
                RaiseEvent OnGameList(New RealmServer.GameList(Packet.Data))
            Case D2Packets.RealmServerPacket.JoinGameResponse
                RaiseEvent OnJoinGameResponse(New RealmServer.JoinGameResponse(Packet.Data))
            Case D2Packets.RealmServerPacket.MessageOfTheDay
                RaiseEvent OnMessageOfTheDay(New RealmServer.MessageOfTheDay(Packet.Data))
            Case D2Packets.RealmServerPacket.RealmStartupResponse
                RaiseEvent OnRealmStartupResponse(New RealmServer.RealmStartupResponse(Packet.Data))
        End Select
        RaiseEvent OnReceivePacket(Packet)
    End Sub

#End Region

End Class