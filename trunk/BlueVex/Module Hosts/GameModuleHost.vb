Imports System.Runtime.InteropServices

Public Class GameModuleHost
    Inherits IModuleHost
    Implements IGame

#Region " Base Functions "

    Sub New(ByVal Funcs As IntPtr)
        MyBase.New(Funcs)
    End Sub

    Overrides Sub LoadModules()
        Dim GameModule As IGameModule
        'Initialize Each Module
        For i As Integer = 1 To AvailableGameModules.Count
            'GameModule = DirectCast(PluginServices.CreateInstance(AvailableGameModules(i)), IGameModule)
            GameModule = DirectCast(AvailableGameModules(i), IGameModule)
            If My.Settings.DisabledModules Is Nothing Then My.Settings.DisabledModules = New Collections.Specialized.StringCollection
            If Not My.Settings.DisabledModules.Contains(GameModule.Name) Then
                Log.WriteLine("Loading " & GameModule.Name)
                GameModule.Initialize(Me)
                LoadedModules.Add(GameModule)
            End If
        Next
    End Sub

    Public Sub WriteToLog(ByVal Text As String) Implements IGame.WriteToLog
        MyRedVexInfo.WriteLog(Text)
    End Sub

#End Region

#Region " Packet Methods "

    Public Event OnSendPacket(ByRef Packet As Packet) Implements IGame.OnSendPacket
    Public Event OnReceivePacket(ByRef Packet As Packet) Implements IGame.OnReceivePacket

    Public Sub SendPacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IGame.SendPacket
        RelayDataToServer(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByRef bytes() As Byte) Implements IGame.SendPacket
        SendPacket(bytes, bytes.Length)
    End Sub

    Public Sub SendPacket(ByVal Packet As D2Packets.D2Packet) Implements IGame.SendPacket
        SendPacket(Packet.Data, Packet.Data.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte, ByVal length As Integer) Implements IGame.ReceivePacket
        RelayDataToClient(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByRef bytes() As Byte) Implements IGame.ReceivePacket
        ReceivePacket(bytes, bytes.Length)
    End Sub

    Public Sub ReceivePacket(ByVal Packet As D2Packets.D2Packet) Implements IGame.ReceivePacket
        ReceivePacket(Packet.Data, Packet.Data.Length)
    End Sub

#End Region

#Region " Game Client Events "

    Public Event OnAddBeltItem(ByVal Packet As GameClient.AddBeltItem) Implements IGame.OnAddBeltItem
    Public Event OnBuyItem(ByVal Packet As GameClient.BuyItem) Implements IGame.OnBuyItem
    Public Event OnCainIdentifyItems(ByVal Packet As GameClient.CainIdentifyItems) Implements IGame.OnCainIdentifyItems
    Public Event OnCastLeftSkill(ByVal Packet As GameClient.CastLeftSkill) Implements IGame.OnCastLeftSkill
    Public Event OnCastLeftSkillOnTarget(ByVal Packet As GameClient.CastLeftSkillOnTarget) Implements IGame.OnCastLeftSkillOnTarget
    Public Event OnCastLeftSkillOnTargetStopped(ByVal Packet As GameClient.CastLeftSkillOnTargetStopped) Implements IGame.OnCastLeftSkillOnTargetStopped
    Public Event OnCastRightSkill(ByVal Packet As GameClient.CastRightSkill) Implements IGame.OnCastRightSkill
    Public Event OnCastRightSkillOnTarget(ByVal Packet As GameClient.CastRightSkillOnTarget) Implements IGame.OnCastRightSkillOnTarget
    Public Event OnCastRightSkillOnTargetStopped(ByVal Packet As GameClient.CastRightSkillOnTargetStopped) Implements IGame.OnCastRightSkillOnTargetStopped
    Public Event OnChangeMercEquipment(ByVal Packet As GameClient.ChangeMercEquipment) Implements IGame.OnChangeMercEquipment
    Public Event OnClickButton(ByVal Packet As GameClient.ClickButton) Implements IGame.OnClickButton
    Public Event OnCloseQuest(ByVal Packet As GameClient.CloseQuest) Implements IGame.OnCloseQuest
    Public Event OnDisplayQuestMessage(ByVal Packet As GameClient.DisplayQuestMessage) Implements IGame.OnDisplayQuestMessage
    Public Event OnDropGold(ByVal Packet As GameClient.DropGold) Implements IGame.OnDropGold
    Public Event OnDropItem(ByVal Packet As GameClient.DropItem) Implements IGame.OnDropItem
    Public Event OnDropItemToContainer(ByVal Packet As GameClient.DropItemToContainer) Implements IGame.OnDropItemToContainer
    Public Event OnEmbedItem(ByVal Packet As GameClient.EmbedItem) Implements IGame.OnEmbedItem
    Public Event OnEnterGame(ByVal Packet As GameClient.EnterGame) Implements IGame.OnEnterGame
    Public Event OnEquipItem(ByVal Packet As GameClient.EquipItem) Implements IGame.OnEquipItem
    Public Event OnExitGame(ByVal Packet As GameClient.ExitGame) Implements IGame.OnExitGame
    Public Event OnGameLogonRequest(ByVal Packet As GameClient.GameLogonRequest) Implements IGame.OnGameLogonRequest
    Public Event OnGoToLocation(ByVal Packet As GameClient.GoToLocation) Implements IGame.OnGoToLocation
    Public Event OnGoToTarget(ByVal Packet As GameClient.GoToTarget) Implements IGame.OnGoToTarget
    Public Event OnGoToTownFolk(ByVal Packet As GameClient.GoToTownFolk) Implements IGame.OnGoToTownFolk
    Public Event OnHireMercenary(ByVal Packet As GameClient.HireMercenary) Implements IGame.OnHireMercenary
    Public Event OnHoverUnit(ByVal Packet As GameClient.HoverUnit) Implements IGame.OnHoverUnit
    Public Event OnIdentifyGambleItem(ByVal Packet As GameClient.IdentifyGambleItem) Implements IGame.OnIdentifyGambleItem
    Public Event OnIdentifyItem(ByVal Packet As GameClient.IdentifyItem) Implements IGame.OnIdentifyItem
    Public Event OnIncrementAttribute(ByVal Packet As GameClient.IncrementAttribute) Implements IGame.OnIncrementAttribute
    Public Event OnIncrementSkill(ByVal Packet As GameClient.IncrementSkill) Implements IGame.OnIncrementSkill
    Public Event OnInventoryItemToBelt(ByVal Packet As GameClient.InventoryItemToBelt) Implements IGame.OnInventoryItemToBelt
    Public Event OnItemToCube(ByVal Packet As GameClient.ItemToCube) Implements IGame.OnItemToCube
    Public Event OnPartyRequest(ByVal Packet As GameClient.PartyRequest) Implements IGame.OnPartyRequest
    Public Event OnPickItem(ByVal Packet As GameClient.PickItem) Implements IGame.OnPickItem
    Public Event OnPickItemFromContainer(ByVal Packet As GameClient.PickItemFromContainer) Implements IGame.OnPickItemFromContainer
    Public Event OnPing(ByVal Packet As GameClient.Ping) Implements IGame.OnPing
    Public Event OnRecastLeftSkill(ByVal Packet As GameClient.RecastLeftSkill) Implements IGame.OnRecastLeftSkill
    Public Event OnRecastLeftSkillOnTarget(ByVal Packet As GameClient.RecastLeftSkillOnTarget) Implements IGame.OnRecastLeftSkillOnTarget
    Public Event OnRecastLeftSkillOnTargetStopped(ByVal Packet As GameClient.RecastLeftSkillOnTargetStopped) Implements IGame.OnRecastLeftSkillOnTargetStopped
    Public Event OnRecastRightSkill(ByVal Packet As GameClient.RecastRightSkill) Implements IGame.OnRecastRightSkill
    Public Event OnRecastRightSkillOnTarget(ByVal Packet As GameClient.RecastRightSkillOnTarget) Implements IGame.OnRecastRightSkillOnTarget
    Public Event OnRecastRightSkillOnTargetStopped(ByVal Packet As GameClient.RecastRightSkillOnTargetStopped) Implements IGame.OnRecastRightSkillOnTargetStopped
    Public Event OnRemoveBeltItem(ByVal Packet As GameClient.RemoveBeltItem) Implements IGame.OnRemoveBeltItem
    Public Event OnRequestQuestLog(ByVal Packet As GameClient.RequestQuestLog) Implements IGame.OnRequestQuestLog
    Public Event OnRequestReassign(ByVal Packet As GameClient.RequestReassign) Implements IGame.OnRequestReassign
    Public Event OnRespawn(ByVal Packet As GameClient.Respawn) Implements IGame.OnRespawn
    Public Event OnResurrectMerc(ByVal Packet As GameClient.ResurrectMerc) Implements IGame.OnResurrectMerc
    Public Event OnRunToLocation(ByVal Packet As GameClient.RunToLocation) Implements IGame.OnRunToLocation
    Public Event OnRunToTarget(ByVal Packet As GameClient.RunToTarget) Implements IGame.OnRunToTarget
    Public Event OnSelectSkill(ByVal Packet As GameClient.SelectSkill) Implements IGame.OnSelectSkill
    Public Event OnSellItem(ByVal Packet As GameClient.SellItem) Implements IGame.OnSellItem
    Public Event OnSendCharacterSpeech(ByVal Packet As GameClient.SendCharacterSpeech) Implements IGame.OnSendCharacterSpeech
    Public Event OnSendMessage(ByVal Packet As GameClient.SendMessage) Implements IGame.OnSendMessage
    Public Event OnSendOverheadMessage(ByVal Packet As GameClient.SendOverheadMessage) Implements IGame.OnSendOverheadMessage
    Public Event OnSetPlayerRelation(ByVal Packet As GameClient.SetPlayerRelation) Implements IGame.OnSetPlayerRelation
    Public Event OnSetSkillHotkey(ByVal Packet As GameClient.SetSkillHotkey) Implements IGame.OnSetSkillHotkey
    Public Event OnStackItems(ByVal Packet As GameClient.StackItems) Implements IGame.OnStackItems
    Public Event OnSwapBeltItem(ByVal Packet As GameClient.SwapBeltItem) Implements IGame.OnSwapBeltItem
    Public Event OnSwapContainerItem(ByVal Packet As GameClient.SwapContainerItem) Implements IGame.OnSwapContainerItem
    Public Event OnSwapEquippedItem(ByVal Packet As GameClient.SwapEquippedItem) Implements IGame.OnSwapEquippedItem
    Public Event OnSwitchWeapons(ByVal Packet As GameClient.SwitchWeapons) Implements IGame.OnSwitchWeapons
    Public Event OnTownFolkCancelInteraction(ByVal Packet As GameClient.TownFolkCancelInteraction) Implements IGame.OnTownFolkCancelInteraction
    Public Event OnTownFolkInteract(ByVal Packet As GameClient.TownFolkInteract) Implements IGame.OnTownFolkInteract
    Public Event OnTownFolkMenuSelect(ByVal Packet As GameClient.TownFolkMenuSelect) Implements IGame.OnTownFolkMenuSelect
    Public Event OnTownFolkRepair(ByVal Packet As GameClient.TownFolkRepair) Implements IGame.OnTownFolkRepair
    Public Event OnUnequipItem(ByVal Packet As GameClient.UnequipItem) Implements IGame.OnUnequipItem
    Public Event OnUnitInteract(ByVal Packet As GameClient.UnitInteract) Implements IGame.OnUnitInteract
    Public Event OnUpdatePosition(ByVal Packet As GameClient.UpdatePosition) Implements IGame.OnUpdatePosition
    Public Event OnUseBeltItem(ByVal Packet As GameClient.UseBeltItem) Implements IGame.OnUseBeltItem
    Public Event OnUseInventoryItem(ByVal Packet As GameClient.UseInventoryItem) Implements IGame.OnUseInventoryItem
    Public Event OnWalkToLocation(ByVal Packet As GameClient.WalkToLocation) Implements IGame.OnWalkToLocation
    Public Event OnWalkToTarget(ByVal Packet As GameClient.WalkToTarget) Implements IGame.OnWalkToTarget
    Public Event OnWardenResponse(ByVal Packet As GameClient.WardenResponse) Implements IGame.OnWardenResponse
    Public Event OnWaypointInteract(ByVal Packet As GameClient.WaypointInteract) Implements IGame.OnWaypointInteract

#End Region

#Region " Game Server Events "

    Public Event OnAboutPlayer(ByVal Packet As GameServer.AboutPlayer) Implements IGame.OnAboutPlayer
    Public Event OnAcceptTrade(ByVal Packet As GameServer.AcceptTrade) Implements IGame.OnAcceptTrade
    Public Event OnAddUnit(ByVal Packet As GameServer.AddUnit) Implements IGame.OnAddUnit
    Public Event OnAssignGameObject(ByVal Packet As GameServer.AssignGameObject) Implements IGame.OnAssignGameObject
    Public Event OnAssignMerc(ByVal Packet As GameServer.AssignMerc) Implements IGame.OnAssignMerc
    Public Event OnAssignNPC(ByVal Packet As GameServer.AssignNPC) Implements IGame.OnAssignNPC
    Public Event OnAssignPlayer(ByVal Packet As GameServer.AssignPlayer) Implements IGame.OnAssignPlayer
    Public Event OnAssignPlayerCorpse(ByVal Packet As GameServer.AssignPlayerCorpse) Implements IGame.OnAssignPlayerCorpse
    Public Event OnAssignPlayerToParty(ByVal Packet As GameServer.AssignPlayerToParty) Implements IGame.OnAssignPlayerToParty
    Public Event OnAssignSkill(ByVal Packet As GameServer.AssignSkill) Implements IGame.OnAssignSkill
    Public Event OnAssignSkillHotkey(ByVal Packet As GameServer.AssignSkillHotkey) Implements IGame.OnAssignSkillHotkey
    Public Event OnAssignWarp(ByVal Packet As GameServer.AssignWarp) Implements IGame.OnAssignWarp
    Public Event OnAttributeNotification(ByVal Packet As GameServer.AttributeNotification) Implements IGame.OnAttributeNotification
    Public Event OnDelayedState(ByVal Packet As GameServer.DelayedState) Implements IGame.OnDelayedState
    Public Event OnEndState(ByVal Packet As GameServer.EndState) Implements IGame.OnEndState
    Public Event OnGainExperience(ByVal Packet As GameServer.GainExperience) Implements IGame.OnGainExperience
    Public Event OnGameHandshake(ByVal Packet As GameServer.GameHandshake) Implements IGame.OnGameHandshake
    Public Event OnGameLoading(ByVal Packet As GameServer.GameLoading) Implements IGame.OnGameLoading
    Public Event OnGameLogonReceipt(ByVal Packet As GameServer.GameLogonReceipt) Implements IGame.OnGameLogonReceipt
    Public Event OnGameLogonSuccess(ByVal Packet As GameServer.GameLogonSuccess) Implements IGame.OnGameLogonSuccess
    Public Event OnGameLogoutSuccess(ByVal Packet As GameServer.GameLogoutSuccess) Implements IGame.OnGameLogoutSuccess
    Public Event OnReceiveMessage(ByVal Packet As GameServer.GameMessage) Implements IGame.OnReceiveMessage
    Public Event OnGameOver(ByVal Packet As GameServer.GameOver) Implements IGame.OnGameOver
    Public Event OnGoldTrade(ByVal Packet As GameServer.GoldTrade) Implements IGame.OnGoldTrade
    Public Event OnInformationMessage(ByVal Packet As GameServer.InformationMessage) Implements IGame.OnInformationMessage
    Public Event OnItemAction(ByVal Packet As GameServer.ItemAction) Implements IGame.OnItemAction
    Public Event OnItemTriggerSkill(ByVal Packet As GameServer.ItemTriggerSkill) Implements IGame.OnItemTriggerSkill
    Public Event OnLoadAct(ByVal Packet As GameServer.LoadAct) Implements IGame.OnLoadAct
    Public Event OnLoadDone(ByVal Packet As GameServer.LoadDone) Implements IGame.OnLoadDone
    Public Event OnMapAdd(ByVal Packet As GameServer.MapAdd) Implements IGame.OnMapAdd
    Public Event OnMapRemove(ByVal Packet As GameServer.MapRemove) Implements IGame.OnMapRemove
    Public Event OnMercAttributeNotification(ByVal Packet As GameServer.MercAttributeNotification) Implements IGame.OnMercAttributeNotification
    Public Event OnMercForHire(ByVal Packet As GameServer.MercForHire) Implements IGame.OnMercForHire
    Public Event OnMercForHireListStart(ByVal Packet As GameServer.MercForHireListStart) Implements IGame.OnMercForHireListStart
    Public Event OnMercGainExperience(ByVal Packet As GameServer.MercGainExperience) Implements IGame.OnMercGainExperience
    Public Event OnMonsterAttack(ByVal Packet As GameServer.MonsterAttack) Implements IGame.OnMonsterAttack
    Public Event OnNPCAction(ByVal Packet As GameServer.NPCAction) Implements IGame.OnNPCAction
    Public Event OnNPCGetHit(ByVal Packet As GameServer.NPCGetHit) Implements IGame.OnNPCGetHit
    Public Event OnNPCHeal(ByVal Packet As GameServer.NPCHeal) Implements IGame.OnNPCHeal
    Public Event OnNPCInfo(ByVal Packet As GameServer.NPCInfo) Implements IGame.OnNPCInfo
    Public Event OnNPCMove(ByVal Packet As GameServer.NPCMove) Implements IGame.OnNPCMove
    Public Event OnNPCMoveToTarget(ByVal Packet As GameServer.NPCMoveToTarget) Implements IGame.OnNPCMoveToTarget
    Public Event OnNPCStop(ByVal Packet As GameServer.NPCStop) Implements IGame.OnNPCStop
    Public Event OnNPCWantsInteract(ByVal Packet As GameServer.NPCWantsInteract) Implements IGame.OnNPCWantsInteract
    Public Event OnOpenWaypoint(ByVal Packet As GameServer.OpenWaypoint) Implements IGame.OnOpenWaypoint
    Public Event OnPartyMemberPulse(ByVal Packet As GameServer.PartyMemberPulse) Implements IGame.OnPartyMemberPulse
    Public Event OnPartyMemberUpdate(ByVal Packet As GameServer.PartyMemberUpdate) Implements IGame.OnPartyMemberUpdate
    Public Event OnPartyRefresh(ByVal Packet As GameServer.PartyRefresh) Implements IGame.OnPartyRefresh
    Public Event OnPlayerAttributeNotification(ByVal Packet As GameServer.PlayerAttributeNotification) Implements IGame.OnPlayerAttributeNotification
    Public Event OnPlayerClearCursor(ByVal Packet As GameServer.PlayerClearCursor) Implements IGame.OnPlayerClearCursor
    Public Event OnPlayerCorpseVisible(ByVal Packet As GameServer.PlayerCorpseVisible) Implements IGame.OnPlayerCorpseVisible
    Public Event OnPlayerInGame(ByVal Packet As GameServer.PlayerInGame) Implements IGame.OnPlayerInGame
    Public Event OnPlayerInSight(ByVal Packet As GameServer.PlayerInSight) Implements IGame.OnPlayerInSight
    Public Event OnPlayerKillCount(ByVal Packet As GameServer.PlayerKillCount) Implements IGame.OnPlayerKillCount
    Public Event OnPlayerLeaveGame(ByVal Packet As GameServer.PlayerLeaveGame) Implements IGame.OnPlayerLeaveGame
    Public Event OnPlayerLifeManaChange(ByVal Packet As GameServer.PlayerLifeManaChange) Implements IGame.OnPlayerLifeManaChange
    Public Event OnPlayerMove(ByVal Packet As GameServer.PlayerMove) Implements IGame.OnPlayerMove
    Public Event OnPlayerMoveToTarget(ByVal Packet As GameServer.PlayerMoveToTarget) Implements IGame.OnPlayerMoveToTarget
    Public Event OnPlayerPartyRelationship(ByVal Packet As GameServer.PlayerPartyRelationship) Implements IGame.OnPlayerPartyRelationship
    Public Event OnPlayerReassign(ByVal Packet As GameServer.PlayerReassign) Implements IGame.OnPlayerReassign
    Public Event OnPlayerRelationship(ByVal Packet As GameServer.PlayerRelationship) Implements IGame.OnPlayerRelationship
    Public Event OnPlayerStop(ByVal Packet As GameServer.PlayerStop) Implements IGame.OnPlayerStop
    Public Event OnPlaySound(ByVal Packet As GameServer.PlaySound) Implements IGame.OnPlaySound
    Public Event OnPong(ByVal Packet As GameServer.Pong) Implements IGame.OnPong
    Public Event OnPortalInfo(ByVal Packet As GameServer.PortalInfo) Implements IGame.OnPortalInfo
    Public Event OnPortalOwnership(ByVal Packet As GameServer.PortalOwnership) Implements IGame.OnPortalOwnership
    Public Event OnQuestItemState(ByVal Packet As GameServer.QuestItemState) Implements IGame.OnQuestItemState
    Public Event OnRelator1(ByVal Packet As GameServer.Relator1) Implements IGame.OnRelator1
    Public Event OnRelator2(ByVal Packet As GameServer.Relator2) Implements IGame.OnRelator2
    Public Event OnRemoveGroundUnit(ByVal Packet As GameServer.RemoveGroundUnit) Implements IGame.OnRemoveGroundUnit
    Public Event OnReportKill(ByVal Packet As GameServer.ReportKill) Implements IGame.OnReportKill
    Public Event OnRequestLogonInfo(ByVal Packet As GameServer.RequestLogonInfo) Implements IGame.OnRequestLogonInfo
    Public Event OnSetGameObjectMode(ByVal Packet As GameServer.SetGameObjectMode) Implements IGame.OnSetGameObjectMode
    Public Event OnSetItemState(ByVal Packet As GameServer.SetItemState) Implements IGame.OnSetItemState
    Public Event OnSetNPCMode(ByVal Packet As GameServer.SetNPCMode) Implements IGame.OnSetNPCMode
    Public Event OnSetState(ByVal Packet As GameServer.SetState) Implements IGame.OnSetState
    Public Event OnSkillsLog(ByVal Packet As GameServer.SkillsLog) Implements IGame.OnSkillsLog
    Public Event OnSmallGoldAdd(ByVal Packet As GameServer.SmallGoldAdd) Implements IGame.OnSmallGoldAdd
    Public Event OnSummonAction(ByVal Packet As GameServer.SummonAction) Implements IGame.OnSummonAction
    Public Event OnSwitchWeaponSet(ByVal Packet As GameServer.SwitchWeaponSet) Implements IGame.OnSwitchWeaponSet
    Public Event OnTransactionComplete(ByVal Packet As GameServer.TransactionComplete) Implements IGame.OnTransactionComplete
    Public Event OnUnitUseSkill(ByVal Packet As GameServer.UnitUseSkill) Implements IGame.OnUnitUseSkill
    Public Event OnUnitUseSkillOnTarget(ByVal Packet As GameServer.UnitUseSkillOnTarget) Implements IGame.OnUnitUseSkillOnTarget
    Public Event OnUnloadDone(ByVal Packet As GameServer.UnloadDone) Implements IGame.OnUnloadDone
    Public Event OnUpdateGameQuestLog(ByVal Packet As GameServer.UpdateGameQuestLog) Implements IGame.OnUpdateGameQuestLog
    Public Event OnUpdateItemStats(ByVal Packet As GameServer.UpdateItemStats) Implements IGame.OnUpdateItemStats
    Public Event OnUpdateItemUI(ByVal Packet As GameServer.UpdateItemUI) Implements IGame.OnUpdateItemUI
    Public Event OnUpdatePlayerItemSkill(ByVal Packet As GameServer.UpdatePlayerItemSkill) Implements IGame.OnUpdatePlayerItemSkill
    Public Event OnUpdateQuestInfo(ByVal Packet As GameServer.UpdateQuestInfo) Implements IGame.OnUpdateQuestInfo
    Public Event OnUpdateQuestLog(ByVal Packet As GameServer.UpdateQuestLog) Implements IGame.OnUpdateQuestLog
    Public Event OnUpdateSkill(ByVal Packet As GameServer.UpdateSkill) Implements IGame.OnUpdateSkill
    Public Event OnUseSpecialItem(ByVal Packet As GameServer.UseSpecialItem) Implements IGame.OnUseSpecialItem
    Public Event OnUseStackableItem(ByVal Packet As GameServer.UseStackableItem) Implements IGame.OnUseStackableItem
    Public Event OnWalkVerify(ByVal Packet As GameServer.WalkVerify) Implements IGame.OnWalkVerify
    Public Event OnWardenCheck(ByVal Packet As GameServer.WardenCheck) Implements IGame.OnWardenCheck

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)
        Select Case Packet.Data(0)
            Case D2Packets.GameClientPacket.AddBeltItem
                RaiseEvent OnAddBeltItem(New GameClient.AddBeltItem(Packet.Data))
            Case D2Packets.GameClientPacket.BuyItem
                RaiseEvent OnBuyItem(New GameClient.BuyItem(Packet.Data))
            Case D2Packets.GameClientPacket.CainIdentifyItems
                RaiseEvent OnCainIdentifyItems(New GameClient.CainIdentifyItems(Packet.Data))
            Case D2Packets.GameClientPacket.CastLeftSkill
                RaiseEvent OnCastLeftSkill(New GameClient.CastLeftSkill(Packet.Data))
            Case D2Packets.GameClientPacket.CastLeftSkillOnTarget
                RaiseEvent OnCastLeftSkillOnTarget(New GameClient.CastLeftSkillOnTarget(Packet.Data))
            Case D2Packets.GameClientPacket.CastLeftSkillOnTargetStopped
                RaiseEvent OnCastLeftSkillOnTargetStopped(New GameClient.CastLeftSkillOnTargetStopped(Packet.Data))
            Case D2Packets.GameClientPacket.CastRightSkill
                RaiseEvent OnCastRightSkill(New GameClient.CastRightSkill(Packet.Data))
            Case D2Packets.GameClientPacket.CastRightSkillOnTarget
                RaiseEvent OnCastRightSkillOnTarget(New GameClient.CastRightSkillOnTarget(Packet.Data))
            Case D2Packets.GameClientPacket.CastRightSkillOnTargetStopped
                RaiseEvent OnCastRightSkillOnTargetStopped(New GameClient.CastRightSkillOnTargetStopped(Packet.Data))
            Case D2Packets.GameClientPacket.ChangeMercEquipment
                RaiseEvent OnChangeMercEquipment(New GameClient.ChangeMercEquipment(Packet.Data))
            Case D2Packets.GameClientPacket.ClickButton
                RaiseEvent OnClickButton(New GameClient.ClickButton(Packet.Data))
            Case D2Packets.GameClientPacket.CloseQuest
                RaiseEvent OnCloseQuest(New GameClient.CloseQuest(Packet.Data))
            Case D2Packets.GameClientPacket.DisplayQuestMessage
                RaiseEvent OnDisplayQuestMessage(New GameClient.DisplayQuestMessage(Packet.Data))
            Case D2Packets.GameClientPacket.DropGold
                RaiseEvent OnDropGold(New GameClient.DropGold(Packet.Data))
            Case D2Packets.GameClientPacket.DropItem
                RaiseEvent OnDropItem(New GameClient.DropItem(Packet.Data))
            Case D2Packets.GameClientPacket.DropItemToContainer
                RaiseEvent OnDropItemToContainer(New GameClient.DropItemToContainer(Packet.Data))
            Case D2Packets.GameClientPacket.EmbedItem
                RaiseEvent OnEmbedItem(New GameClient.EmbedItem(Packet.Data))
            Case D2Packets.GameClientPacket.EnterGame
                RaiseEvent OnEnterGame(New GameClient.EnterGame(Packet.Data))
            Case D2Packets.GameClientPacket.EquipItem
                RaiseEvent OnEquipItem(New GameClient.EquipItem(Packet.Data))
            Case D2Packets.GameClientPacket.ExitGame
                RaiseEvent OnExitGame(New GameClient.ExitGame(Packet.Data))
            Case D2Packets.GameClientPacket.GameLogonRequest
                RaiseEvent OnGameLogonRequest(New GameClient.GameLogonRequest(Packet.Data))
            Case D2Packets.GameClientPacket.GoToTownFolk
                RaiseEvent OnGoToTownFolk(New GameClient.GoToTownFolk(Packet.Data))
            Case D2Packets.GameClientPacket.HireMercenary
                RaiseEvent OnHireMercenary(New GameClient.HireMercenary(Packet.Data))
            Case D2Packets.GameClientPacket.HoverUnit
                RaiseEvent OnHoverUnit(New GameClient.HoverUnit(Packet.Data))
            Case D2Packets.GameClientPacket.IdentifyGambleItem
                RaiseEvent OnIdentifyGambleItem(New GameClient.IdentifyGambleItem(Packet.Data))
            Case D2Packets.GameClientPacket.IdentifyItem
                RaiseEvent OnIdentifyItem(New GameClient.IdentifyItem(Packet.Data))
            Case D2Packets.GameClientPacket.IncrementAttribute
                RaiseEvent OnIncrementAttribute(New GameClient.IncrementAttribute(Packet.Data))
            Case D2Packets.GameClientPacket.IncrementSkill
                RaiseEvent OnIncrementSkill(New GameClient.IncrementSkill(Packet.Data))
            Case D2Packets.GameClientPacket.InventoryItemToBelt
                RaiseEvent OnInventoryItemToBelt(New GameClient.InventoryItemToBelt(Packet.Data))
            Case D2Packets.GameClientPacket.ItemToCube
                RaiseEvent OnItemToCube(New GameClient.ItemToCube(Packet.Data))
            Case D2Packets.GameClientPacket.PartyRequest
                RaiseEvent OnPartyRequest(New GameClient.PartyRequest(Packet.Data))
            Case D2Packets.GameClientPacket.PickItem
                RaiseEvent OnPickItem(New GameClient.PickItem(Packet.Data))
            Case D2Packets.GameClientPacket.PickItemFromContainer
                RaiseEvent OnPickItemFromContainer(New GameClient.PickItemFromContainer(Packet.Data))
            Case D2Packets.GameClientPacket.Ping
                RaiseEvent OnPing(New GameClient.Ping(Packet.Data))
            Case D2Packets.GameClientPacket.RecastLeftSkill
                RaiseEvent OnRecastLeftSkill(New GameClient.RecastLeftSkill(Packet.Data))
            Case D2Packets.GameClientPacket.RecastLeftSkillOnTarget
                RaiseEvent OnRecastLeftSkillOnTarget(New GameClient.RecastLeftSkillOnTarget(Packet.Data))
            Case D2Packets.GameClientPacket.RecastLeftSkillOnTargetStopped
                RaiseEvent OnRecastLeftSkillOnTargetStopped(New GameClient.RecastLeftSkillOnTargetStopped(Packet.Data))
            Case D2Packets.GameClientPacket.RecastRightSkill
                RaiseEvent OnRecastRightSkill(New GameClient.RecastRightSkill(Packet.Data))
            Case D2Packets.GameClientPacket.RecastRightSkillOnTarget
                RaiseEvent OnRecastRightSkillOnTarget(New GameClient.RecastRightSkillOnTarget(Packet.Data))
            Case D2Packets.GameClientPacket.RecastRightSkillOnTargetStopped
                RaiseEvent OnRecastRightSkillOnTargetStopped(New GameClient.RecastRightSkillOnTargetStopped(Packet.Data))
            Case D2Packets.GameClientPacket.RemoveBeltItem
                RaiseEvent OnRemoveBeltItem(New GameClient.RemoveBeltItem(Packet.Data))
            Case D2Packets.GameClientPacket.RequestQuestLog
                RaiseEvent OnRequestQuestLog(New GameClient.RequestQuestLog(Packet.Data))
            Case D2Packets.GameClientPacket.RequestReassign
                RaiseEvent OnRequestReassign(New GameClient.RequestReassign(Packet.Data))
            Case D2Packets.GameClientPacket.Respawn
                RaiseEvent OnRespawn(New GameClient.Respawn(Packet.Data))
            Case D2Packets.GameClientPacket.ResurrectMerc
                RaiseEvent OnResurrectMerc(New GameClient.ResurrectMerc(Packet.Data))
            Case D2Packets.GameClientPacket.RunToLocation
                RaiseEvent OnRunToLocation(New GameClient.RunToLocation(Packet.Data))
            Case D2Packets.GameClientPacket.RunToTarget
                RaiseEvent OnRunToTarget(New GameClient.RunToTarget(Packet.Data))
            Case D2Packets.GameClientPacket.SelectSkill
                RaiseEvent OnSelectSkill(New GameClient.SelectSkill(Packet.Data))
            Case D2Packets.GameClientPacket.SellItem
                RaiseEvent OnSellItem(New GameClient.SellItem(Packet.Data))
            Case D2Packets.GameClientPacket.SendCharacterSpeech
                RaiseEvent OnSendCharacterSpeech(New GameClient.SendCharacterSpeech(Packet.Data))
            Case D2Packets.GameClientPacket.SendMessage
                HandleSentMessagePacket(Packet)
                RaiseEvent OnSendMessage(New GameClient.SendMessage(Packet.Data))
            Case D2Packets.GameClientPacket.SendOverheadMessage
                RaiseEvent OnSendOverheadMessage(New GameClient.SendOverheadMessage(Packet.Data))
            Case D2Packets.GameClientPacket.SetPlayerRelation
                RaiseEvent OnSetPlayerRelation(New GameClient.SetPlayerRelation(Packet.Data))
            Case D2Packets.GameClientPacket.SetSkillHotkey
                RaiseEvent OnSetSkillHotkey(New GameClient.SetSkillHotkey(Packet.Data))
            Case D2Packets.GameClientPacket.StackItems
                RaiseEvent OnStackItems(New GameClient.StackItems(Packet.Data))
            Case D2Packets.GameClientPacket.SwapBeltItem
                RaiseEvent OnSwapBeltItem(New GameClient.SwapBeltItem(Packet.Data))
            Case D2Packets.GameClientPacket.SwapContainerItem
                RaiseEvent OnSwapContainerItem(New GameClient.SwapContainerItem(Packet.Data))
            Case D2Packets.GameClientPacket.SwapEquippedItem
                RaiseEvent OnSwapEquippedItem(New GameClient.SwapEquippedItem(Packet.Data))
            Case D2Packets.GameClientPacket.SwitchWeapons
                RaiseEvent OnSwitchWeapons(New GameClient.SwitchWeapons(Packet.Data))
            Case D2Packets.GameClientPacket.TownFolkCancelInteraction
                RaiseEvent OnTownFolkCancelInteraction(New GameClient.TownFolkCancelInteraction(Packet.Data))
            Case D2Packets.GameClientPacket.TownFolkInteract
                RaiseEvent OnTownFolkInteract(New GameClient.TownFolkInteract(Packet.Data))
            Case D2Packets.GameClientPacket.TownFolkMenuSelect
                RaiseEvent OnTownFolkMenuSelect(New GameClient.TownFolkMenuSelect(Packet.Data))
            Case D2Packets.GameClientPacket.TownFolkRepair
                RaiseEvent OnTownFolkRepair(New GameClient.TownFolkRepair(Packet.Data))
            Case D2Packets.GameClientPacket.UnequipItem
                RaiseEvent OnUnequipItem(New GameClient.UnequipItem(Packet.Data))
            Case D2Packets.GameClientPacket.UnitInteract
                RaiseEvent OnUnitInteract(New GameClient.UnitInteract(Packet.Data))
            Case D2Packets.GameClientPacket.UpdatePosition
                RaiseEvent OnUpdatePosition(New GameClient.UpdatePosition(Packet.Data))
            Case D2Packets.GameClientPacket.UseBeltItem
                RaiseEvent OnUseBeltItem(New GameClient.UseBeltItem(Packet.Data))
            Case D2Packets.GameClientPacket.UseInventoryItem
                RaiseEvent OnUseInventoryItem(New GameClient.UseInventoryItem(Packet.Data))
            Case D2Packets.GameClientPacket.WalkToLocation
                RaiseEvent OnWalkToLocation(New GameClient.WalkToLocation(Packet.Data))
            Case D2Packets.GameClientPacket.WalkToTarget
                RaiseEvent OnWalkToTarget(New GameClient.WalkToTarget(Packet.Data))
            Case D2Packets.GameClientPacket.WardenResponse
                RaiseEvent OnWardenResponse(New GameClient.WardenResponse(Packet.Data))
            Case D2Packets.GameClientPacket.WaypointInteract
                RaiseEvent OnWaypointInteract(New GameClient.WaypointInteract(Packet.Data))
        End Select
        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Select Case Packet.Data(0)
            Case D2Packets.GameServerPacket.AboutPlayer
                RaiseEvent OnAboutPlayer(New GameServer.AboutPlayer(Packet.Data))
            Case D2Packets.GameServerPacket.AcceptTrade
                RaiseEvent OnAcceptTrade(New GameServer.AcceptTrade(Packet.Data))
            Case D2Packets.GameServerPacket.AddUnit
                RaiseEvent OnAddUnit(New GameServer.AddUnit(Packet.Data))
            Case D2Packets.GameServerPacket.AssignGameObject
                RaiseEvent OnAssignGameObject(New GameServer.AssignGameObject(Packet.Data))
            Case D2Packets.GameServerPacket.AssignMerc
                RaiseEvent OnAssignMerc(New GameServer.AssignMerc(Packet.Data))
            Case D2Packets.GameServerPacket.AssignNPC
                RaiseEvent OnAssignNPC(New GameServer.AssignNPC(Packet.Data))
            Case D2Packets.GameServerPacket.AssignPlayer
                RaiseEvent OnAssignPlayer(New GameServer.AssignPlayer(Packet.Data))
            Case D2Packets.GameServerPacket.AssignPlayerCorpse
                RaiseEvent OnAssignPlayerCorpse(New GameServer.AssignPlayerCorpse(Packet.Data))
            Case D2Packets.GameServerPacket.AssignPlayerToParty
                RaiseEvent OnAssignPlayerToParty(New GameServer.AssignPlayerToParty(Packet.Data))
            Case D2Packets.GameServerPacket.AssignSkill
                RaiseEvent OnAssignSkill(New GameServer.AssignSkill(Packet.Data))
            Case D2Packets.GameServerPacket.AssignSkillHotkey
                RaiseEvent OnAssignSkillHotkey(New GameServer.AssignSkillHotkey(Packet.Data))
            Case D2Packets.GameServerPacket.AssignWarp
                RaiseEvent OnAssignWarp(New GameServer.AssignWarp(Packet.Data))
            Case D2Packets.GameServerPacket.AttributeByte
                RaiseEvent OnAttributeNotification(New GameServer.AttributeByte(Packet.Data))
            Case D2Packets.GameServerPacket.AttributeDWord
                RaiseEvent OnAttributeNotification(New GameServer.AttributeDWord(Packet.Data))
            Case D2Packets.GameServerPacket.AttributeWord
                RaiseEvent OnAttributeNotification(New GameServer.AttributeWord(Packet.Data))
            Case D2Packets.GameServerPacket.ByteToExperience
                RaiseEvent OnGainExperience(New GameServer.ByteToExperience(Packet.Data))
            Case D2Packets.GameServerPacket.DelayedState
                RaiseEvent OnDelayedState(New GameServer.DelayedState(Packet.Data))
            Case D2Packets.GameServerPacket.DWordToExperience
                RaiseEvent OnGainExperience(New GameServer.DWordToExperience(Packet.Data))
            Case D2Packets.GameServerPacket.EndState
                RaiseEvent OnEndState(New GameServer.EndState(Packet.Data))
            Case D2Packets.GameServerPacket.GameHandshake
                RaiseEvent OnGameHandshake(New GameServer.GameHandshake(Packet.Data))
            Case D2Packets.GameServerPacket.GameLoading
                RaiseEvent OnGameLoading(New GameServer.GameLoading(Packet.Data))
            Case D2Packets.GameServerPacket.GameLogonReceipt
                RaiseEvent OnGameLogonReceipt(New GameServer.GameLogonReceipt(Packet.Data))
            Case D2Packets.GameServerPacket.GameLogonSuccess
                RaiseEvent OnGameLogonSuccess(New GameServer.GameLogonSuccess(Packet.Data))
            Case D2Packets.GameServerPacket.GameLogoutSuccess
                RaiseEvent OnGameLogoutSuccess(New GameServer.GameLogoutSuccess(Packet.Data))
            Case D2Packets.GameServerPacket.GameMessage
                RaiseEvent OnReceiveMessage(New GameServer.GameMessage(Packet.Data))
            Case D2Packets.GameServerPacket.GameOver
                RaiseEvent OnGameOver(New GameServer.GameOver(Packet.Data))
            Case D2Packets.GameServerPacket.GoldTrade
                RaiseEvent OnGoldTrade(New GameServer.GoldTrade(Packet.Data))
            Case D2Packets.GameServerPacket.InformationMessage
                RaiseEvent OnInformationMessage(New GameServer.InformationMessage(Packet.Data))
            Case D2Packets.GameServerPacket.ItemTriggerSkill
                RaiseEvent OnItemTriggerSkill(New GameServer.ItemTriggerSkill(Packet.Data))
            Case D2Packets.GameServerPacket.LoadAct
                RaiseEvent OnLoadAct(New GameServer.LoadAct(Packet.Data))
            Case D2Packets.GameServerPacket.LoadDone
                RaiseEvent OnLoadDone(New GameServer.LoadDone(Packet.Data))
            Case D2Packets.GameServerPacket.MapAdd
                RaiseEvent OnMapAdd(New GameServer.MapAdd(Packet.Data))
            Case D2Packets.GameServerPacket.MapRemove
                RaiseEvent OnMapRemove(New GameServer.MapRemove(Packet.Data))
            Case D2Packets.GameServerPacket.MercAttributeByte
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeByte(Packet.Data))
            Case D2Packets.GameServerPacket.MercAttributeDWord
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeDWord(Packet.Data))
            Case D2Packets.GameServerPacket.MercAttributeWord
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeWord(Packet.Data))
            Case D2Packets.GameServerPacket.MercByteToExperience
                RaiseEvent OnGainExperience(New GameServer.MercByteToExperience(Packet.Data))
            Case D2Packets.GameServerPacket.MercForHire
                RaiseEvent OnMercForHire(New GameServer.MercForHire(Packet.Data))
            Case D2Packets.GameServerPacket.MercForHireListStart
                RaiseEvent OnMercForHireListStart(New GameServer.MercForHireListStart(Packet.Data))
            Case D2Packets.GameServerPacket.MercWordToExperience
                RaiseEvent OnGainExperience(New GameServer.MercWordToExperience(Packet.Data))
            Case D2Packets.GameServerPacket.MonsterAttack
                RaiseEvent OnMonsterAttack(New GameServer.MonsterAttack(Packet.Data))
            Case D2Packets.GameServerPacket.NPCAction
                RaiseEvent OnNPCAction(New GameServer.NPCAction(Packet.Data))
            Case D2Packets.GameServerPacket.NPCGetHit
                RaiseEvent OnNPCGetHit(New GameServer.NPCGetHit(Packet.Data))
            Case D2Packets.GameServerPacket.NPCHeal
                RaiseEvent OnNPCHeal(New GameServer.NPCHeal(Packet.Data))
            Case D2Packets.GameServerPacket.NPCInfo
                RaiseEvent OnNPCInfo(New GameServer.NPCInfo(Packet.Data))
            Case D2Packets.GameServerPacket.NPCMove
                RaiseEvent OnNPCMove(New GameServer.NPCMove(Packet.Data))
            Case D2Packets.GameServerPacket.NPCMoveToTarget
                RaiseEvent OnNPCMoveToTarget(New GameServer.NPCMoveToTarget(Packet.Data))
            Case D2Packets.GameServerPacket.NPCStop
                RaiseEvent OnNPCStop(New GameServer.NPCStop(Packet.Data))
            Case D2Packets.GameServerPacket.NPCWantsInteract
                RaiseEvent OnNPCWantsInteract(New GameServer.NPCWantsInteract(Packet.Data))
            Case D2Packets.GameServerPacket.OpenWaypoint
                RaiseEvent OnOpenWaypoint(New GameServer.OpenWaypoint(Packet.Data))
                'Quick Fix, Prevent crashing *** gotta Do something about it!
                'Case D2Packets.GameServerPacket.OwnedItemAction
                '   RaiseEvent OnItemAction(New GameServer.OwnedItemAction(Packet.Data))
            Case D2Packets.GameServerPacket.PartyMemberPulse
                RaiseEvent OnPartyMemberPulse(New GameServer.PartyMemberPulse(Packet.Data))
            Case D2Packets.GameServerPacket.PartyMemberUpdate
                RaiseEvent OnPartyMemberUpdate(New GameServer.PartyMemberUpdate(Packet.Data))
            Case D2Packets.GameServerPacket.PartyRefresh
                RaiseEvent OnPartyRefresh(New GameServer.PartyRefresh(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerAttributeNotification
                RaiseEvent OnPlayerAttributeNotification(New GameServer.PlayerAttributeNotification(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerClearCursor
                RaiseEvent OnPlayerClearCursor(New GameServer.PlayerClearCursor(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerCorpseVisible
                RaiseEvent OnPlayerCorpseVisible(New GameServer.PlayerCorpseVisible(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerInGame
                RaiseEvent OnPlayerInGame(New GameServer.PlayerInGame(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerInSight
                RaiseEvent OnPlayerInSight(New GameServer.PlayerInSight(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerKillCount
                RaiseEvent OnPlayerKillCount(New GameServer.PlayerKillCount(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerLeaveGame
                RaiseEvent OnPlayerLeaveGame(New GameServer.PlayerLeaveGame(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerLifeManaChange
                RaiseEvent OnPlayerLifeManaChange(New GameServer.PlayerLifeManaChange(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerMove
                RaiseEvent OnPlayerMove(New GameServer.PlayerMove(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerMoveToTarget
                RaiseEvent OnPlayerMoveToTarget(New GameServer.PlayerMoveToTarget(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerPartyRelationship
                RaiseEvent OnPlayerPartyRelationship(New GameServer.PlayerPartyRelationship(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerReassign
                RaiseEvent OnPlayerReassign(New GameServer.PlayerReassign(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerRelationship
                RaiseEvent OnPlayerRelationship(New GameServer.PlayerRelationship(Packet.Data))
            Case D2Packets.GameServerPacket.PlayerStop
                RaiseEvent OnPlayerStop(New GameServer.PlayerStop(Packet.Data))
            Case D2Packets.GameServerPacket.PlaySound
                RaiseEvent OnPlaySound(New GameServer.PlaySound(Packet.Data))
            Case D2Packets.GameServerPacket.Pong
                RaiseEvent OnPong(New GameServer.Pong(Packet.Data))
            Case D2Packets.GameServerPacket.PortalInfo
                RaiseEvent OnPortalInfo(New GameServer.PortalInfo(Packet.Data))
            Case D2Packets.GameServerPacket.PortalOwnership
                RaiseEvent OnPortalOwnership(New GameServer.PortalOwnership(Packet.Data))
            Case D2Packets.GameServerPacket.QuestItemState
                RaiseEvent OnQuestItemState(New GameServer.QuestItemState(Packet.Data))
            Case D2Packets.GameServerPacket.Relator1
                RaiseEvent OnRelator1(New GameServer.Relator1(Packet.Data))
            Case D2Packets.GameServerPacket.Relator2
                RaiseEvent OnRelator2(New GameServer.Relator2(Packet.Data))
            Case D2Packets.GameServerPacket.RemoveGroundUnit
                RaiseEvent OnRemoveGroundUnit(New GameServer.RemoveGroundUnit(Packet.Data))
            Case D2Packets.GameServerPacket.ReportKill
                RaiseEvent OnReportKill(New GameServer.ReportKill(Packet.Data))
            Case D2Packets.GameServerPacket.RequestLogonInfo
                RaiseEvent OnRequestLogonInfo(New GameServer.RequestLogonInfo(Packet.Data))
            Case D2Packets.GameServerPacket.SetGameObjectMode
                RaiseEvent OnSetGameObjectMode(New GameServer.SetGameObjectMode(Packet.Data))
            Case D2Packets.GameServerPacket.SetItemState
                RaiseEvent OnSetItemState(New GameServer.SetItemState(Packet.Data))
            Case D2Packets.GameServerPacket.SetNPCMode
                RaiseEvent OnSetNPCMode(New GameServer.SetNPCMode(Packet.Data))
            Case D2Packets.GameServerPacket.SetState
                RaiseEvent OnSetState(New GameServer.SetState(Packet.Data))
            Case D2Packets.GameServerPacket.SkillsLog
                RaiseEvent OnSkillsLog(New GameServer.SkillsLog(Packet.Data))
            Case D2Packets.GameServerPacket.SmallGoldAdd
                RaiseEvent OnSmallGoldAdd(New GameServer.SmallGoldAdd(Packet.Data))
            Case D2Packets.GameServerPacket.SummonAction
                RaiseEvent OnSummonAction(New GameServer.SummonAction(Packet.Data))
            Case D2Packets.GameServerPacket.SwitchWeaponSet
                RaiseEvent OnSwitchWeaponSet(New GameServer.SwitchWeaponSet(Packet.Data))
            Case D2Packets.GameServerPacket.TransactionComplete
                RaiseEvent OnTransactionComplete(New GameServer.TransactionComplete(Packet.Data))
            Case D2Packets.GameServerPacket.UnitUseSkill
                RaiseEvent OnUnitUseSkill(New GameServer.UnitUseSkill(Packet.Data))
            Case D2Packets.GameServerPacket.UnitUseSkillOnTarget
                RaiseEvent OnUnitUseSkillOnTarget(New GameServer.UnitUseSkillOnTarget(Packet.Data))
            Case D2Packets.GameServerPacket.UnloadDone
                RaiseEvent OnUnloadDone(New GameServer.UnloadDone(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateGameQuestLog
                RaiseEvent OnUpdateGameQuestLog(New GameServer.UpdateGameQuestLog(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateItemStats
                RaiseEvent OnUpdateItemStats(New GameServer.UpdateItemStats(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateItemUI
                RaiseEvent OnUpdateItemUI(New GameServer.UpdateItemUI(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateQuestInfo
                RaiseEvent OnUpdateQuestInfo(New GameServer.UpdateQuestInfo(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateQuestLog
                RaiseEvent OnUpdateQuestLog(New GameServer.UpdateQuestLog(Packet.Data))
            Case D2Packets.GameServerPacket.UpdateSkill
                RaiseEvent OnUpdateSkill(New GameServer.UpdateSkill(Packet.Data))
            Case D2Packets.GameServerPacket.UseSpecialItem
                RaiseEvent OnUseSpecialItem(New GameServer.UseSpecialItem(Packet.Data))
            Case D2Packets.GameServerPacket.UseStackableItem
                RaiseEvent OnUseStackableItem(New GameServer.UseStackableItem(Packet.Data))
            Case D2Packets.GameServerPacket.WalkVerify
                RaiseEvent OnWalkVerify(New GameServer.WalkVerify(Packet.Data))
            Case D2Packets.GameServerPacket.WardenCheck
                RaiseEvent OnWardenCheck(New GameServer.WardenCheck(Packet.Data))
            Case D2Packets.GameServerPacket.WordToExperience
                RaiseEvent OnGainExperience(New GameServer.WordToExperience(Packet.Data))
            Case D2Packets.GameServerPacket.WorldItemAction
                RaiseEvent OnItemAction(New GameServer.WorldItemAction(Packet.Data))
        End Select
        RaiseEvent OnReceivePacket(Packet)
    End Sub

#End Region

#Region " Chat Methods "

    Public Sub SendMessage(ByVal Message As String) Implements IGame.SendMessage
        Dim PacketObject As New GameClient.SendMessage(D2Data.GameMessageType.GameMessage, Message)
        SendPacket(PacketObject.Data)
    End Sub

    Public Sub ReceiveMessage(ByVal Name As String, ByVal Message As String) Implements IGame.ReceiveMessage
        Dim PacketObject As New GameServer.GameMessage(D2Data.GameMessageType.GameMessage, &H2, Name, Message)
        ReceivePacket(PacketObject.Data)
    End Sub

    Sub HandleSentMessagePacket(ByRef Packet As Packet)

        Dim PacketObject As New GameClient.SendMessage(Packet.Data)

        'There's a plugin for redvex to do this. It's best to Remove it.
        'If PacketObject.Message.StartsWith(".") And Not PacketObject.Message.StartsWith("..") Then Packet.Flag = Packet.PacketFlag.PacketFlag_Dead
        'Usefull function, I will leave it here.
        If PacketObject.Message = ".Map" Then
            Dim MapBitmap As New Pathing
            Dim MapInfo As Pathing.MapInfo_t
            Dim b As Bitmap
            MapInfo = MapBitmap.GetMapFromMemory
            b = MapBitmap.BitmapFromMapInfo(MapInfo)
            Main.Invoke(New OpenMapFormDelegate(AddressOf OpenMapForm), New Object() {b})
        End If

        'RaiseEvent OnSendMessage(PacketObject)
    End Sub

    Delegate Sub OpenMapFormDelegate(ByVal b As Bitmap)
    Sub OpenMapForm(ByVal b As Bitmap)
        Dim f As New MapForm
        f.BackgroundImage = b
        f.ClientSize = New Size(b.Width, b.Height)
        f.FormBorderStyle = FormBorderStyle.FixedDialog
        f.Show()
    End Sub

#End Region

End Class