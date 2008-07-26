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

        'Make Header and put 0 at the end
        '(Word) Packet Size
        '(Byte) Packet ID
        ' ...   Packet Core
        'Dim Bufbyte(bytes.Length + 2) As Byte

        'Bufbyte = PutInArray(Bufbyte, 0, Bufbyte.Length)



        'For i As Integer = 0 To bytes.Length - 1
        '    Bufbyte(i + 2) = bytes(i)
        'Next

        'RelayDataToServer(Bufbyte, Bufbyte.Length)

        RelayDataToServer(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByRef bytes() As Byte) Implements IRealm.SendPacket
        SendPacket(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByVal Packet As D2Packets.D2Packet) Implements IRealm.SendPacket
        SendPacket(Packet.Data, Packet.Data.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IRealm.ReceivePacket

        'Make Header and put 0 at the end
        '(Word) Packet Size
        '(Byte) Packet ID
        ' ...   Packet Core
        'Dim Bufbyte(bytes.Length + 2) As Byte

        'Bufbyte = PutInArray(Bufbyte, 0, Bufbyte.Length)

        'For i As Integer = 0 To bytes.Length - 1
        '    Bufbyte(i + 2) = bytes(i)
        'Next
        'RelayDataToClient(Bufbyte, Bufbyte.Length)

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

    Public Event OnCancelGameCreation(ByVal Packet As RealmClient.CancelGameCreation, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCancelGameCreation
    Public Event OnCharacterCreationRequest(ByVal Packet As RealmClient.CharacterCreationRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterCreationRequest
    Public Event OnCharacterDeletionRequest(ByVal Packet As RealmClient.CharacterDeletionRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterDeletionRequest
    Public Event OnCharacterListRequest(ByVal Packet As RealmClient.CharacterListRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterListRequest
    Public Event OnCharacterLogonRequest(ByVal Packet As RealmClient.CharacterLogonRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterLogonRequest
    Public Event OnCharacterUpgradeRequest(ByVal Packet As RealmClient.CharacterUpgradeRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterUpgradeRequest
    Public Event OnCreateGameRequest(ByVal Packet As RealmClient.CreateGameRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCreateGameRequest
    Public Event OnGameInfoRequest(ByVal Packet As RealmClient.GameInfoRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnGameInfoRequest
    Public Event OnGameListRequest(ByVal Packet As RealmClient.GameListRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnGameListRequest
    Public Event OnJoinGameRequest(ByVal Packet As RealmClient.JoinGameRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnJoinGameRequest
    Public Event OnMessageOfTheDayRequest(ByVal Packet As RealmClient.MessageOfTheDayRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnMessageOfTheDayRequest
    Public Event OnRealmStartupRequest(ByVal Packet As RealmClient.RealmStartupRequest, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnRealmStartupRequest

#End Region

#Region " Realm Server Events "

    Public Event OnCharacterCreationResponse(ByVal Packet As RealmServer.CharacterCreationResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterCreationResponse
    Public Event OnCharacterDeletionResponse(ByVal Packet As RealmServer.CharacterDeletionResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterDeletionResponse
    Public Event OnCharacterList(ByVal Packet As RealmServer.CharacterList, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterList
    Public Event OnCharacterLogonResponse(ByVal Packet As RealmServer.CharacterLogonResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterLogonResponse
    Public Event OnCharacterUpgradeResponse(ByVal Packet As RealmServer.CharacterUpgradeResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCharacterUpgradeResponse
    Public Event OnCreateGameResponse(ByVal Packet As RealmServer.CreateGameResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnCreateGameResponse
    Public Event OnGameCreationQueue(ByVal Packet As RealmServer.GameCreationQueue, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnGameCreationQueue
    Public Event OnGameInfo(ByVal Packet As RealmServer.GameInfo, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnGameInfo
    Public Event OnGameList(ByVal Packet As RealmServer.GameList, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnGameList
    Public Event OnJoinGameResponse(ByVal Packet As RealmServer.JoinGameResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnJoinGameResponse
    Public Event OnMessageOfTheDay(ByVal Packet As RealmServer.MessageOfTheDay, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnMessageOfTheDay
    Public Event OnRealmStartupResponse(ByVal Packet As RealmServer.RealmStartupResponse, ByRef Flag As Packet.PacketFlag) Implements IRealm.OnRealmStartupResponse

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)
        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag
        Packet.Data = CutBytes(Packet.Data, 0, 2)

        Select Case Packet.Data(0)
            Case D2Packets.RealmClientPacket.CancelGameCreation
                RaiseEvent OnCancelGameCreation(New RealmClient.CancelGameCreation(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CharacterCreationRequest
                RaiseEvent OnCharacterCreationRequest(New RealmClient.CharacterCreationRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CharacterDeletionRequest
                RaiseEvent OnCharacterDeletionRequest(New RealmClient.CharacterDeletionRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CharacterListRequest
                RaiseEvent OnCharacterListRequest(New RealmClient.CharacterListRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CharacterLogonRequest
                RaiseEvent OnCharacterLogonRequest(New RealmClient.CharacterLogonRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CharacterUpgradeRequest
                RaiseEvent OnCharacterUpgradeRequest(New RealmClient.CharacterUpgradeRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.CreateGameRequest
                RaiseEvent OnCreateGameRequest(New RealmClient.CreateGameRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.GameInfoRequest
                RaiseEvent OnGameInfoRequest(New RealmClient.GameInfoRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.GameListRequest
                RaiseEvent OnGameListRequest(New RealmClient.GameListRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.JoinGameRequest
                RaiseEvent OnJoinGameRequest(New RealmClient.JoinGameRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.MessageOfTheDayRequest
                RaiseEvent OnMessageOfTheDayRequest(New RealmClient.MessageOfTheDayRequest(Packet.Data), Flag)
            Case D2Packets.RealmClientPacket.RealmStartupRequest
                RaiseEvent OnRealmStartupRequest(New RealmClient.RealmStartupRequest(Packet.Data), Flag)
        End Select

        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If

        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag

        Packet.Data = CutBytes(Packet.Data, 0, 2)

        Select Case Packet.Data(0)
            Case D2Packets.RealmServerPacket.CharacterCreationResponse
                RaiseEvent OnCharacterCreationResponse(New RealmServer.CharacterCreationResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.CharacterDeletionResponse
                RaiseEvent OnCharacterDeletionResponse(New RealmServer.CharacterDeletionResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.CharacterList
                RaiseEvent OnCharacterList(New RealmServer.CharacterList(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.CharacterLogonResponse
                RaiseEvent OnCharacterLogonResponse(New RealmServer.CharacterLogonResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.CharacterUpgradeResponse
                RaiseEvent OnCharacterUpgradeResponse(New RealmServer.CharacterUpgradeResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.CreateGameResponse
                RaiseEvent OnCreateGameResponse(New RealmServer.CreateGameResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.GameCreationQueue
                RaiseEvent OnGameCreationQueue(New RealmServer.GameCreationQueue(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.GameInfo
                RaiseEvent OnGameInfo(New RealmServer.GameInfo(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.GameList
                RaiseEvent OnGameList(New RealmServer.GameList(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.JoinGameResponse
                RaiseEvent OnJoinGameResponse(New RealmServer.JoinGameResponse(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.MessageOfTheDay
                RaiseEvent OnMessageOfTheDay(New RealmServer.MessageOfTheDay(Packet.Data), Flag)
            Case D2Packets.RealmServerPacket.RealmStartupResponse
                RaiseEvent OnRealmStartupResponse(New RealmServer.RealmStartupResponse(Packet.Data), Flag)
        End Select

        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If

        RaiseEvent OnReceivePacket(Packet)
    End Sub

#End Region

End Class