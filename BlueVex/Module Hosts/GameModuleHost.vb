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
            GameModule = DirectCast(PluginServices.CreateInstance(AvailableGameModules(i)), IGameModule)
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

    Public Event OnAddBeltItem(ByVal Packet As GameClient.AddBeltItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAddBeltItem
    Public Event OnBuyItem(ByVal Packet As GameClient.BuyItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnBuyItem
    Public Event OnCainIdentifyItems(ByVal Packet As GameClient.CainIdentifyItems, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCainIdentifyItems
    Public Event OnCastLeftSkill(ByVal Packet As GameClient.CastLeftSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastLeftSkill
    Public Event OnCastLeftSkillOnTarget(ByVal Packet As GameClient.CastLeftSkillOnTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastLeftSkillOnTarget
    Public Event OnCastLeftSkillOnTargetStopped(ByVal Packet As GameClient.CastLeftSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastLeftSkillOnTargetStopped
    Public Event OnCastRightSkill(ByVal Packet As GameClient.CastRightSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastRightSkill
    Public Event OnCastRightSkillOnTarget(ByVal Packet As GameClient.CastRightSkillOnTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastRightSkillOnTarget
    Public Event OnCastRightSkillOnTargetStopped(ByVal Packet As GameClient.CastRightSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCastRightSkillOnTargetStopped
    Public Event OnChangeMercEquipment(ByVal Packet As GameClient.ChangeMercEquipment, ByRef Flag As Packet.PacketFlag) Implements IGame.OnChangeMercEquipment
    Public Event OnClickButton(ByVal Packet As GameClient.ClickButton, ByRef Flag As Packet.PacketFlag) Implements IGame.OnClickButton
    Public Event OnCloseQuest(ByVal Packet As GameClient.CloseQuest, ByRef Flag As Packet.PacketFlag) Implements IGame.OnCloseQuest
    Public Event OnDisplayQuestMessage(ByVal Packet As GameClient.DisplayQuestMessage, ByRef Flag As Packet.PacketFlag) Implements IGame.OnDisplayQuestMessage
    Public Event OnDropGold(ByVal Packet As GameClient.DropGold, ByRef Flag As Packet.PacketFlag) Implements IGame.OnDropGold
    Public Event OnDropItem(ByVal Packet As GameClient.DropItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnDropItem
    Public Event OnDropItemToContainer(ByVal Packet As GameClient.DropItemToContainer, ByRef Flag As Packet.PacketFlag) Implements IGame.OnDropItemToContainer
    Public Event OnEmbedItem(ByVal Packet As GameClient.EmbedItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnEmbedItem
    Public Event OnEnterGame(ByVal Packet As GameClient.EnterGame, ByRef Flag As Packet.PacketFlag) Implements IGame.OnEnterGame
    Public Event OnEquipItem(ByVal Packet As GameClient.EquipItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnEquipItem
    Public Event OnExitGame(ByVal Packet As GameClient.ExitGame, ByRef Flag As Packet.PacketFlag) Implements IGame.OnExitGame
    Public Event OnGameLogonRequest(ByVal Packet As GameClient.GameLogonRequest, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameLogonRequest
    Public Event OnGoToLocation(ByVal Packet As GameClient.GoToLocation, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGoToLocation
    Public Event OnGoToTarget(ByVal Packet As GameClient.GoToTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGoToTarget
    Public Event OnGoToTownFolk(ByVal Packet As GameClient.GoToTownFolk, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGoToTownFolk
    Public Event OnHireMercenary(ByVal Packet As GameClient.HireMercenary, ByRef Flag As Packet.PacketFlag) Implements IGame.OnHireMercenary
    Public Event OnHoverUnit(ByVal Packet As GameClient.HoverUnit, ByRef Flag As Packet.PacketFlag) Implements IGame.OnHoverUnit
    Public Event OnIdentifyGambleItem(ByVal Packet As GameClient.IdentifyGambleItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnIdentifyGambleItem
    Public Event OnIdentifyItem(ByVal Packet As GameClient.IdentifyItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnIdentifyItem
    Public Event OnIncrementAttribute(ByVal Packet As GameClient.IncrementAttribute, ByRef Flag As Packet.PacketFlag) Implements IGame.OnIncrementAttribute
    Public Event OnIncrementSkill(ByVal Packet As GameClient.IncrementSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnIncrementSkill
    Public Event OnInventoryItemToBelt(ByVal Packet As GameClient.InventoryItemToBelt, ByRef Flag As Packet.PacketFlag) Implements IGame.OnInventoryItemToBelt
    Public Event OnItemToCube(ByVal Packet As GameClient.ItemToCube, ByRef Flag As Packet.PacketFlag) Implements IGame.OnItemToCube
    Public Event OnPartyRequest(ByVal Packet As GameClient.PartyRequest, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPartyRequest
    Public Event OnPickItem(ByVal Packet As GameClient.PickItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPickItem
    Public Event OnPickItemFromContainer(ByVal Packet As GameClient.PickItemFromContainer, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPickItemFromContainer
    Public Event OnPing(ByVal Packet As GameClient.Ping, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPing
    Public Event OnRecastLeftSkill(ByVal Packet As GameClient.RecastLeftSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastLeftSkill
    Public Event OnRecastLeftSkillOnTarget(ByVal Packet As GameClient.RecastLeftSkillOnTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastLeftSkillOnTarget
    Public Event OnRecastLeftSkillOnTargetStopped(ByVal Packet As GameClient.RecastLeftSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastLeftSkillOnTargetStopped
    Public Event OnRecastRightSkill(ByVal Packet As GameClient.RecastRightSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastRightSkill
    Public Event OnRecastRightSkillOnTarget(ByVal Packet As GameClient.RecastRightSkillOnTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastRightSkillOnTarget
    Public Event OnRecastRightSkillOnTargetStopped(ByVal Packet As GameClient.RecastRightSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRecastRightSkillOnTargetStopped
    Public Event OnRemoveBeltItem(ByVal Packet As GameClient.RemoveBeltItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRemoveBeltItem
    Public Event OnRequestQuestLog(ByVal Packet As GameClient.RequestQuestLog, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRequestQuestLog
    Public Event OnRequestReassign(ByVal Packet As GameClient.RequestReassign, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRequestReassign
    Public Event OnRespawn(ByVal Packet As GameClient.Respawn, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRespawn
    Public Event OnResurrectMerc(ByVal Packet As GameClient.ResurrectMerc, ByRef Flag As Packet.PacketFlag) Implements IGame.OnResurrectMerc
    Public Event OnRunToLocation(ByVal Packet As GameClient.RunToLocation, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRunToLocation
    Public Event OnRunToTarget(ByVal Packet As GameClient.RunToTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRunToTarget
    Public Event OnSelectSkill(ByVal Packet As GameClient.SelectSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSelectSkill
    Public Event OnSellItem(ByVal Packet As GameClient.SellItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSellItem
    Public Event OnSendCharacterSpeech(ByVal Packet As GameClient.SendCharacterSpeech, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSendCharacterSpeech
    Public Event OnSendMessage(ByVal Packet As GameClient.SendMessage, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSendMessage
    Public Event OnSendOverheadMessage(ByVal Packet As GameClient.SendOverheadMessage, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSendOverheadMessage
    Public Event OnSetPlayerRelation(ByVal Packet As GameClient.SetPlayerRelation, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetPlayerRelation
    Public Event OnSetSkillHotkey(ByVal Packet As GameClient.SetSkillHotkey, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetSkillHotkey
    Public Event OnStackItems(ByVal Packet As GameClient.StackItems, ByRef Flag As Packet.PacketFlag) Implements IGame.OnStackItems
    Public Event OnSwapBeltItem(ByVal Packet As GameClient.SwapBeltItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSwapBeltItem
    Public Event OnSwapContainerItem(ByVal Packet As GameClient.SwapContainerItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSwapContainerItem
    Public Event OnSwapEquippedItem(ByVal Packet As GameClient.SwapEquippedItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSwapEquippedItem
    Public Event OnSwitchWeapons(ByVal Packet As GameClient.SwitchWeapons, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSwitchWeapons
    Public Event OnTownFolkCancelInteraction(ByVal Packet As GameClient.TownFolkCancelInteraction, ByRef Flag As Packet.PacketFlag) Implements IGame.OnTownFolkCancelInteraction
    Public Event OnTownFolkInteract(ByVal Packet As GameClient.TownFolkInteract, ByRef Flag As Packet.PacketFlag) Implements IGame.OnTownFolkInteract
    Public Event OnTownFolkMenuSelect(ByVal Packet As GameClient.TownFolkMenuSelect, ByRef Flag As Packet.PacketFlag) Implements IGame.OnTownFolkMenuSelect
    Public Event OnTownFolkRepair(ByVal Packet As GameClient.TownFolkRepair, ByRef Flag As Packet.PacketFlag) Implements IGame.OnTownFolkRepair
    Public Event OnUnequipItem(ByVal Packet As GameClient.UnequipItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUnequipItem
    Public Event OnUnitInteract(ByVal Packet As GameClient.UnitInteract, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUnitInteract
    Public Event OnUpdatePosition(ByVal Packet As GameClient.UpdatePosition, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdatePosition
    Public Event OnUseBeltItem(ByVal Packet As GameClient.UseBeltItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUseBeltItem
    Public Event OnUseInventoryItem(ByVal Packet As GameClient.UseInventoryItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUseInventoryItem
    Public Event OnWalkToLocation(ByVal Packet As GameClient.WalkToLocation, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWalkToLocation
    Public Event OnWalkToTarget(ByVal Packet As GameClient.WalkToTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWalkToTarget

    Public Event OnWardenResponse(ByVal Packet As GameClient.WardenResponse, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWardenResponse
    Public Event OnWaypointInteract(ByVal Packet As GameClient.WaypointInteract, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWaypointInteract

#End Region

#Region " Game Server Events "

    Public Event OnAboutPlayer(ByVal Packet As GameServer.AboutPlayer, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAboutPlayer
    Public Event OnAcceptTrade(ByVal Packet As GameServer.AcceptTrade, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAcceptTrade
    Public Event OnAddUnit(ByVal Packet As GameServer.AddUnit, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAddUnit
    Public Event OnAssignGameObject(ByVal Packet As GameServer.AssignGameObject, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignGameObject
    Public Event OnAssignMerc(ByVal Packet As GameServer.AssignMerc, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignMerc
    Public Event OnAssignNPC(ByVal Packet As GameServer.AssignNPC, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignNPC
    Public Event OnAssignPlayer(ByVal Packet As GameServer.AssignPlayer, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignPlayer
    Public Event OnAssignPlayerCorpse(ByVal Packet As GameServer.AssignPlayerCorpse, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignPlayerCorpse
    Public Event OnAssignPlayerToParty(ByVal Packet As GameServer.AssignPlayerToParty, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignPlayerToParty
    Public Event OnAssignSkill(ByVal Packet As GameServer.Assignskill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignSkill
    Public Event OnAssignSkillHotkey(ByVal Packet As GameServer.AssignSkillHotkey, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignSkillHotkey
    Public Event OnAssignWarp(ByVal Packet As GameServer.AssignWarp, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAssignWarp
    Public Event OnAttributeNotification(ByVal Packet As GameServer.AttributeNotification, ByRef Flag As Packet.PacketFlag) Implements IGame.OnAttributeNotification
    Public Event OnDelayedState(ByVal Packet As GameServer.DelayedState, ByRef Flag As Packet.PacketFlag) Implements IGame.OnDelayedState
    Public Event OnEndState(ByVal Packet As GameServer.EndState, ByRef Flag As Packet.PacketFlag) Implements IGame.OnEndState
    Public Event OnGainExperience(ByVal Packet As GameServer.GainExperience, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGainExperience
    Public Event OnGameHandshake(ByVal Packet As GameServer.GameHandshake, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameHandshake
    Public Event OnGameLoading(ByVal Packet As GameServer.GameLoading, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameLoading
    Public Event OnGameLogonReceipt(ByVal Packet As GameServer.GameLogonReceipt, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameLogonReceipt
    Public Event OnGameLogonSuccess(ByVal Packet As GameServer.GameLogonSuccess, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameLogonSuccess
    Public Event OnGameLogoutSuccess(ByVal Packet As GameServer.GameLogoutSuccess, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameLogoutSuccess
    Public Event OnReceiveMessage(ByVal Packet As GameServer.GameMessage, ByRef Flag As Packet.PacketFlag) Implements IGame.OnReceiveMessage
    Public Event OnGameOver(ByVal Packet As GameServer.GameOver, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGameOver
    Public Event OnGoldTrade(ByVal Packet As GameServer.GoldTrade, ByRef Flag As Packet.PacketFlag) Implements IGame.OnGoldTrade
    Public Event OnInformationMessage(ByVal Packet As GameServer.InformationMessage, ByRef Flag As Packet.PacketFlag) Implements IGame.OnInformationMessage
    '**************
    Public Event OnOwnedItemAction(ByVal Packet As GameServer.OwnedItemAction, ByRef Flag As Packet.PacketFlag) Implements IGame.OnOwnedItemAction
    '**************
    Public Event OnItemAction(ByVal Packet As GameServer.ItemAction, ByRef Flag As Packet.PacketFlag) Implements IGame.OnItemAction
    Public Event OnItemTriggerSkill(ByVal Packet As GameServer.ItemTriggerSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnItemTriggerSkill
    Public Event OnLoadAct(ByVal Packet As GameServer.LoadAct, ByRef Flag As Packet.PacketFlag) Implements IGame.OnLoadAct
    Public Event OnLoadDone(ByVal Packet As GameServer.LoadDone, ByRef Flag As Packet.PacketFlag) Implements IGame.OnLoadDone
    Public Event OnMapAdd(ByVal Packet As GameServer.MapAdd, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMapAdd
    Public Event OnMapRemove(ByVal Packet As GameServer.MapRemove, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMapRemove
    Public Event OnMercAttributeNotification(ByVal Packet As GameServer.MercAttributeNotification, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMercAttributeNotification
    Public Event OnMercForHire(ByVal Packet As GameServer.MercForHire, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMercForHire
    Public Event OnMercForHireListStart(ByVal Packet As GameServer.MercForHireListStart, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMercForHireListStart
    Public Event OnMercGainExperience(ByVal Packet As GameServer.MercGainExperience, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMercGainExperience
    Public Event OnMonsterAttack(ByVal Packet As GameServer.MonsterAttack, ByRef Flag As Packet.PacketFlag) Implements IGame.OnMonsterAttack
    Public Event OnNPCAction(ByVal Packet As GameServer.NPCAction, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCAction
    Public Event OnNPCGetHit(ByVal Packet As GameServer.NPCGetHit, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCGetHit
    Public Event OnNPCHeal(ByVal Packet As GameServer.NPCHeal, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCHeal
    Public Event OnNPCInfo(ByVal Packet As GameServer.NPCInfo, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCInfo
    Public Event OnNPCMove(ByVal Packet As GameServer.NPCMove, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCMove
    Public Event OnNPCMoveToTarget(ByVal Packet As GameServer.NPCMoveToTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCMoveToTarget
    Public Event OnNPCStop(ByVal Packet As GameServer.NPCStop, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCStop
    Public Event OnNPCWantsInteract(ByVal Packet As GameServer.NPCWantsInteract, ByRef Flag As Packet.PacketFlag) Implements IGame.OnNPCWantsInteract
    Public Event OnOpenWaypoint(ByVal Packet As GameServer.OpenWaypoint, ByRef Flag As Packet.PacketFlag) Implements IGame.OnOpenWaypoint
    Public Event OnPartyMemberPulse(ByVal Packet As GameServer.PartyMemberPulse, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPartyMemberPulse
    Public Event OnPartyMemberUpdate(ByVal Packet As GameServer.PartyMemberUpdate, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPartyMemberUpdate
    Public Event OnPartyRefresh(ByVal Packet As GameServer.PartyRefresh, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPartyRefresh
    Public Event OnPlayerAttributeNotification(ByVal Packet As GameServer.PlayerAttributeNotification, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerAttributeNotification
    Public Event OnPlayerClearCursor(ByVal Packet As GameServer.PlayerClearCursor, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerClearCursor
    Public Event OnPlayerCorpseVisible(ByVal Packet As GameServer.PlayerCorpseVisible, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerCorpseVisible
    Public Event OnPlayerInGame(ByVal Packet As GameServer.PlayerInGame, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerInGame
    Public Event OnPlayerInSight(ByVal Packet As GameServer.PlayerInSight, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerInSight
    Public Event OnPlayerKillCount(ByVal Packet As GameServer.PlayerKillCount, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerKillCount
    Public Event OnPlayerLeaveGame(ByVal Packet As GameServer.PlayerLeaveGame, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerLeaveGame
    Public Event OnPlayerLifeManaChange(ByVal Packet As GameServer.PlayerLifeManaChange, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerLifeManaChange
    Public Event OnPlayerMove(ByVal Packet As GameServer.PlayerMove, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerMove
    Public Event OnPlayerMoveToTarget(ByVal Packet As GameServer.PlayerMoveToTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerMoveToTarget
    Public Event OnPlayerPartyRelationship(ByVal Packet As GameServer.PlayerPartyRelationship, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerPartyRelationship
    Public Event OnPlayerReassign(ByVal Packet As GameServer.PlayerReassign, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerReassign
    Public Event OnPlayerRelationship(ByVal Packet As GameServer.PlayerRelationship, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerRelationship
    Public Event OnPlayerStop(ByVal Packet As GameServer.PlayerStop, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlayerStop
    Public Event OnPlaySound(ByVal Packet As GameServer.PlaySound, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPlaySound
    Public Event OnPong(ByVal Packet As GameServer.Pong, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPong
    Public Event OnPortalInfo(ByVal Packet As GameServer.PortalInfo, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPortalInfo
    Public Event OnPortalOwnership(ByVal Packet As GameServer.PortalOwnership, ByRef Flag As Packet.PacketFlag) Implements IGame.OnPortalOwnership
    Public Event OnQuestItemState(ByVal Packet As GameServer.QuestItemState, ByRef Flag As Packet.PacketFlag) Implements IGame.OnQuestItemState
    Public Event OnRelator1(ByVal Packet As GameServer.Relator1, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRelator1
    Public Event OnRelator2(ByVal Packet As GameServer.Relator2, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRelator2
    Public Event OnRemoveGroundUnit(ByVal Packet As GameServer.RemoveGroundUnit, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRemoveGroundUnit
    Public Event OnReportKill(ByVal Packet As GameServer.ReportKill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnReportKill
    Public Event OnRequestLogonInfo(ByVal Packet As GameServer.RequestLogonInfo, ByRef Flag As Packet.PacketFlag) Implements IGame.OnRequestLogonInfo
    Public Event OnSetGameObjectMode(ByVal Packet As GameServer.SetGameObjectMode, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetGameObjectMode
    Public Event OnSetItemState(ByVal Packet As GameServer.SetItemState, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetItemState
    Public Event OnSetNPCMode(ByVal Packet As GameServer.SetNPCMode, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetNPCMode
    Public Event OnSetState(ByVal Packet As GameServer.SetState, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSetState
    Public Event OnSkillsLog(ByVal Packet As GameServer.SkillsLog, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSkillsLog
    Public Event OnSmallGoldAdd(ByVal Packet As GameServer.SmallGoldAdd, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSmallGoldAdd
    Public Event OnSummonAction(ByVal Packet As GameServer.SummonAction, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSummonAction
    Public Event OnSwitchWeaponSet(ByVal Packet As GameServer.SwitchWeaponSet, ByRef Flag As Packet.PacketFlag) Implements IGame.OnSwitchWeaponSet
    Public Event OnTransactionComplete(ByVal Packet As GameServer.TransactionComplete, ByRef Flag As Packet.PacketFlag) Implements IGame.OnTransactionComplete
    Public Event OnUnitUseSkill(ByVal Packet As GameServer.UnitUseSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUnitUseSkill
    Public Event OnUnitUseSkillOnTarget(ByVal Packet As GameServer.UnitUseSkillOnTarget, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUnitUseSkillOnTarget
    Public Event OnUnloadDone(ByVal Packet As GameServer.UnloadDone, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUnloadDone
    Public Event OnUpdateGameQuestLog(ByVal Packet As GameServer.UpdateGameQuestLog, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateGameQuestLog
    Public Event OnUpdateItemStats(ByVal Packet As GameServer.UpdateItemStats, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateItemStats
    Public Event OnUpdateItemUI(ByVal Packet As GameServer.UpdateItemUI, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateItemUI
    Public Event OnUpdatePlayerItemSkill(ByVal Packet As GameServer.UpdatePlayerItemSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdatePlayerItemSkill
    Public Event OnUpdateQuestInfo(ByVal Packet As GameServer.UpdateQuestInfo, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateQuestInfo
    Public Event OnUpdateQuestLog(ByVal Packet As GameServer.UpdateQuestLog, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateQuestLog
    Public Event OnUpdateSkill(ByVal Packet As GameServer.UpdateSkill, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUpdateSkill
    Public Event OnUseSpecialItem(ByVal Packet As GameServer.UseSpecialItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUseSpecialItem
    Public Event OnUseStackableItem(ByVal Packet As GameServer.UseStackableItem, ByRef Flag As Packet.PacketFlag) Implements IGame.OnUseStackableItem
    Public Event OnWalkVerify(ByVal Packet As GameServer.WalkVerify, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWalkVerify
    Public Event OnWardenCheck(ByVal Packet As GameServer.WardenCheck, ByRef Flag As Packet.PacketFlag) Implements IGame.OnWardenCheck

#End Region

#Region " Packet Event Raisers "

    Overrides Sub InterptetPacketToServer(ByRef Packet As Packet)
        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag

        Select Case Packet.Data(0)
            Case D2Packets.GameClientPacket.AddBeltItem
                RaiseEvent OnAddBeltItem(New GameClient.AddBeltItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.BuyItem
                RaiseEvent OnBuyItem(New GameClient.BuyItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CainIdentifyItems
                RaiseEvent OnCainIdentifyItems(New GameClient.CainIdentifyItems(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastLeftSkill
                RaiseEvent OnCastLeftSkill(New GameClient.CastLeftSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastLeftSkillOnTarget
                RaiseEvent OnCastLeftSkillOnTarget(New GameClient.CastLeftSkillOnTarget(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastLeftSkillOnTargetStopped
                RaiseEvent OnCastLeftSkillOnTargetStopped(New GameClient.CastLeftSkillOnTargetStopped(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastRightSkill
                RaiseEvent OnCastRightSkill(New GameClient.CastRightSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastRightSkillOnTarget
                RaiseEvent OnCastRightSkillOnTarget(New GameClient.CastRightSkillOnTarget(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CastRightSkillOnTargetStopped
                RaiseEvent OnCastRightSkillOnTargetStopped(New GameClient.CastRightSkillOnTargetStopped(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.ChangeMercEquipment
                RaiseEvent OnChangeMercEquipment(New GameClient.ChangeMercEquipment(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.ClickButton
                RaiseEvent OnClickButton(New GameClient.ClickButton(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.CloseQuest
                RaiseEvent OnCloseQuest(New GameClient.CloseQuest(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.DisplayQuestMessage
                RaiseEvent OnDisplayQuestMessage(New GameClient.DisplayQuestMessage(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.DropGold
                RaiseEvent OnDropGold(New GameClient.DropGold(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.DropItem
                RaiseEvent OnDropItem(New GameClient.DropItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.DropItemToContainer
                RaiseEvent OnDropItemToContainer(New GameClient.DropItemToContainer(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.EmbedItem
                RaiseEvent OnEmbedItem(New GameClient.EmbedItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.EnterGame
                RaiseEvent OnEnterGame(New GameClient.EnterGame(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.EquipItem
                RaiseEvent OnEquipItem(New GameClient.EquipItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.ExitGame
                RaiseEvent OnExitGame(New GameClient.ExitGame(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.GameLogonRequest
                RaiseEvent OnGameLogonRequest(New GameClient.GameLogonRequest(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.GoToTownFolk
                RaiseEvent OnGoToTownFolk(New GameClient.GoToTownFolk(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.HireMercenary
                RaiseEvent OnHireMercenary(New GameClient.HireMercenary(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.HoverUnit
                RaiseEvent OnHoverUnit(New GameClient.HoverUnit(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.IdentifyGambleItem
                RaiseEvent OnIdentifyGambleItem(New GameClient.IdentifyGambleItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.IdentifyItem
                RaiseEvent OnIdentifyItem(New GameClient.IdentifyItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.IncrementAttribute
                RaiseEvent OnIncrementAttribute(New GameClient.IncrementAttribute(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.IncrementSkill
                RaiseEvent OnIncrementSkill(New GameClient.IncrementSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.InventoryItemToBelt
                RaiseEvent OnInventoryItemToBelt(New GameClient.InventoryItemToBelt(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.ItemToCube
                RaiseEvent OnItemToCube(New GameClient.ItemToCube(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.PartyRequest
                RaiseEvent OnPartyRequest(New GameClient.PartyRequest(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.PickItem
                RaiseEvent OnPickItem(New GameClient.PickItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.PickItemFromContainer
                RaiseEvent OnPickItemFromContainer(New GameClient.PickItemFromContainer(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.Ping
                RaiseEvent OnPing(New GameClient.Ping(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastLeftSkill
                RaiseEvent OnRecastLeftSkill(New GameClient.RecastLeftSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastLeftSkillOnTarget
                RaiseEvent OnRecastLeftSkillOnTarget(New GameClient.RecastLeftSkillOnTarget(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastLeftSkillOnTargetStopped
                RaiseEvent OnRecastLeftSkillOnTargetStopped(New GameClient.RecastLeftSkillOnTargetStopped(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastRightSkill
                RaiseEvent OnRecastRightSkill(New GameClient.RecastRightSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastRightSkillOnTarget
                RaiseEvent OnRecastRightSkillOnTarget(New GameClient.RecastRightSkillOnTarget(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RecastRightSkillOnTargetStopped
                RaiseEvent OnRecastRightSkillOnTargetStopped(New GameClient.RecastRightSkillOnTargetStopped(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RemoveBeltItem
                RaiseEvent OnRemoveBeltItem(New GameClient.RemoveBeltItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RequestQuestLog
                RaiseEvent OnRequestQuestLog(New GameClient.RequestQuestLog(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RequestReassign
                RaiseEvent OnRequestReassign(New GameClient.RequestReassign(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.Respawn
                RaiseEvent OnRespawn(New GameClient.Respawn(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.ResurrectMerc
                RaiseEvent OnResurrectMerc(New GameClient.ResurrectMerc(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RunToLocation
                RaiseEvent OnRunToLocation(New GameClient.RunToLocation(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.RunToTarget
                RaiseEvent OnRunToTarget(New GameClient.RunToTarget(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SelectSkill
                RaiseEvent OnSelectSkill(New GameClient.SelectSkill(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SellItem
                RaiseEvent OnSellItem(New GameClient.SellItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SendCharacterSpeech
                RaiseEvent OnSendCharacterSpeech(New GameClient.SendCharacterSpeech(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SendMessage
                HandleSentMessagePacket(Packet)
                RaiseEvent OnSendMessage(New GameClient.SendMessage(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SendOverheadMessage
                RaiseEvent OnSendOverheadMessage(New GameClient.SendOverheadMessage(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SetPlayerRelation
                RaiseEvent OnSetPlayerRelation(New GameClient.SetPlayerRelation(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SetSkillHotkey
                RaiseEvent OnSetSkillHotkey(New GameClient.SetSkillHotkey(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.StackItems
                RaiseEvent OnStackItems(New GameClient.StackItems(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SwapBeltItem
                RaiseEvent OnSwapBeltItem(New GameClient.SwapBeltItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SwapContainerItem
                RaiseEvent OnSwapContainerItem(New GameClient.SwapContainerItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SwapEquippedItem
                RaiseEvent OnSwapEquippedItem(New GameClient.SwapEquippedItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.SwitchWeapons
                RaiseEvent OnSwitchWeapons(New GameClient.SwitchWeapons(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.TownFolkCancelInteraction
                RaiseEvent OnTownFolkCancelInteraction(New GameClient.TownFolkCancelInteraction(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.TownFolkInteract
                RaiseEvent OnTownFolkInteract(New GameClient.TownFolkInteract(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.TownFolkMenuSelect
                RaiseEvent OnTownFolkMenuSelect(New GameClient.TownFolkMenuSelect(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.TownFolkRepair
                RaiseEvent OnTownFolkRepair(New GameClient.TownFolkRepair(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.UnequipItem
                RaiseEvent OnUnequipItem(New GameClient.UnequipItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.UnitInteract
                RaiseEvent OnUnitInteract(New GameClient.UnitInteract(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.UpdatePosition
                RaiseEvent OnUpdatePosition(New GameClient.UpdatePosition(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.UseBeltItem
                RaiseEvent OnUseBeltItem(New GameClient.UseBeltItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.UseInventoryItem
                RaiseEvent OnUseInventoryItem(New GameClient.UseInventoryItem(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.WalkToLocation
                RaiseEvent OnWalkToLocation(New GameClient.WalkToLocation(Packet.Data), Flag)

            Case D2Packets.GameClientPacket.WalkToTarget
                RaiseEvent OnWalkToTarget(New GameClient.WalkToTarget(Packet.Data), Flag)

            Case D2Packets.GameClientPacket.WardenResponse
                RaiseEvent OnWardenResponse(New GameClient.WardenResponse(Packet.Data), Flag)
            Case D2Packets.GameClientPacket.WaypointInteract
                RaiseEvent OnWaypointInteract(New GameClient.WaypointInteract(Packet.Data), Flag)
        End Select
        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If
        RaiseEvent OnSendPacket(Packet)
    End Sub

    Overrides Sub InterptetPacketToClient(ByRef Packet As Packet)
        Dim Flag As Packet.PacketFlag
        Flag = Packet.Flag

        Select Case Packet.Data(0)
            Case D2Packets.GameServerPacket.AboutPlayer
                RaiseEvent OnAboutPlayer(New GameServer.AboutPlayer(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AcceptTrade
                RaiseEvent OnAcceptTrade(New GameServer.AcceptTrade(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AddUnit
                RaiseEvent OnAddUnit(New GameServer.AddUnit(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignGameObject
                RaiseEvent OnAssignGameObject(New GameServer.AssignGameObject(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignMerc
                RaiseEvent OnAssignMerc(New GameServer.AssignMerc(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignNPC
                RaiseEvent OnAssignNPC(New GameServer.AssignNPC(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignPlayer
                RaiseEvent OnAssignPlayer(New GameServer.AssignPlayer(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignPlayerCorpse
                RaiseEvent OnAssignPlayerCorpse(New GameServer.AssignPlayerCorpse(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignPlayerToParty
                RaiseEvent OnAssignPlayerToParty(New GameServer.AssignPlayerToParty(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.Assignskill
                RaiseEvent OnAssignSkill(New GameServer.Assignskill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignSkillHotkey
                RaiseEvent OnAssignSkillHotkey(New GameServer.AssignSkillHotkey(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AssignWarp
                RaiseEvent OnAssignWarp(New GameServer.AssignWarp(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AttributeByte
                RaiseEvent OnAttributeNotification(New GameServer.AttributeByte(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AttributeDWord
                RaiseEvent OnAttributeNotification(New GameServer.AttributeDWord(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.AttributeWord
                RaiseEvent OnAttributeNotification(New GameServer.AttributeWord(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.ByteToExperience
                RaiseEvent OnGainExperience(New GameServer.ByteToExperience(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.DelayedState
                RaiseEvent OnDelayedState(New GameServer.DelayedState(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.DWordToExperience
                RaiseEvent OnGainExperience(New GameServer.DWordToExperience(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.EndState
                RaiseEvent OnEndState(New GameServer.EndState(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameHandshake
                RaiseEvent OnGameHandshake(New GameServer.GameHandshake(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameLoading
                RaiseEvent OnGameLoading(New GameServer.GameLoading(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameLogonReceipt
                RaiseEvent OnGameLogonReceipt(New GameServer.GameLogonReceipt(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameLogonSuccess
                RaiseEvent OnGameLogonSuccess(New GameServer.GameLogonSuccess(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameLogoutSuccess
                RaiseEvent OnGameLogoutSuccess(New GameServer.GameLogoutSuccess(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameMessage
                RaiseEvent OnReceiveMessage(New GameServer.GameMessage(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GameOver
                RaiseEvent OnGameOver(New GameServer.GameOver(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.GoldTrade
                RaiseEvent OnGoldTrade(New GameServer.GoldTrade(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.InformationMessage
                RaiseEvent OnInformationMessage(New GameServer.InformationMessage(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.ItemTriggerSkill
                RaiseEvent OnItemTriggerSkill(New GameServer.ItemTriggerSkill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.LoadAct
                RaiseEvent OnLoadAct(New GameServer.LoadAct(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.LoadDone
                RaiseEvent OnLoadDone(New GameServer.LoadDone(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MapAdd
                RaiseEvent OnMapAdd(New GameServer.MapAdd(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MapRemove
                RaiseEvent OnMapRemove(New GameServer.MapRemove(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercAttributeByte
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeByte(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercAttributeDWord
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeDWord(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercAttributeWord
                RaiseEvent OnMercAttributeNotification(New GameServer.MercAttributeWord(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercByteToExperience
                RaiseEvent OnMercGainExperience(New GameServer.MercByteToExperience(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercForHire
                RaiseEvent OnMercForHire(New GameServer.MercForHire(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercForHireListStart
                RaiseEvent OnMercForHireListStart(New GameServer.MercForHireListStart(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MercWordToExperience
                RaiseEvent OnMercGainExperience(New GameServer.MercWordToExperience(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.MonsterAttack
                RaiseEvent OnMonsterAttack(New GameServer.MonsterAttack(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCAction
                RaiseEvent OnNPCAction(New GameServer.NPCAction(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCGetHit
                RaiseEvent OnNPCGetHit(New GameServer.NPCGetHit(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCHeal
                RaiseEvent OnNPCHeal(New GameServer.NPCHeal(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCInfo
                RaiseEvent OnNPCInfo(New GameServer.NPCInfo(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCMove
                RaiseEvent OnNPCMove(New GameServer.NPCMove(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCMoveToTarget
                RaiseEvent OnNPCMoveToTarget(New GameServer.NPCMoveToTarget(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCStop
                RaiseEvent OnNPCStop(New GameServer.NPCStop(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.NPCWantsInteract
                RaiseEvent OnNPCWantsInteract(New GameServer.NPCWantsInteract(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.OpenWaypoint
                RaiseEvent OnOpenWaypoint(New GameServer.OpenWaypoint(Packet.Data), Flag)
                '***********************
            Case D2Packets.GameServerPacket.OwnedItemAction
                RaiseEvent OnOwnedItemAction(New GameServer.OwnedItemAction(Packet.Data), Flag)
                '***********************
            Case D2Packets.GameServerPacket.PartyMemberPulse
                RaiseEvent OnPartyMemberPulse(New GameServer.PartyMemberPulse(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PartyMemberUpdate
                RaiseEvent OnPartyMemberUpdate(New GameServer.PartyMemberUpdate(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PartyRefresh
                RaiseEvent OnPartyRefresh(New GameServer.PartyRefresh(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerAttributeNotification
                RaiseEvent OnPlayerAttributeNotification(New GameServer.PlayerAttributeNotification(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerClearCursor
                RaiseEvent OnPlayerClearCursor(New GameServer.PlayerClearCursor(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerCorpseVisible
                RaiseEvent OnPlayerCorpseVisible(New GameServer.PlayerCorpseVisible(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerInGame
                RaiseEvent OnPlayerInGame(New GameServer.PlayerInGame(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerInSight
                RaiseEvent OnPlayerInSight(New GameServer.PlayerInSight(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerKillCount
                RaiseEvent OnPlayerKillCount(New GameServer.PlayerKillCount(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerLeaveGame
                RaiseEvent OnPlayerLeaveGame(New GameServer.PlayerLeaveGame(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerLifeManaChange
                RaiseEvent OnPlayerLifeManaChange(New GameServer.PlayerLifeManaChange(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerMove
                RaiseEvent OnPlayerMove(New GameServer.PlayerMove(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerMoveToTarget
                RaiseEvent OnPlayerMoveToTarget(New GameServer.PlayerMoveToTarget(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerPartyRelationship
                RaiseEvent OnPlayerPartyRelationship(New GameServer.PlayerPartyRelationship(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerReassign
                RaiseEvent OnPlayerReassign(New GameServer.PlayerReassign(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerRelationship
                RaiseEvent OnPlayerRelationship(New GameServer.PlayerRelationship(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlayerStop
                RaiseEvent OnPlayerStop(New GameServer.PlayerStop(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PlaySound
                RaiseEvent OnPlaySound(New GameServer.PlaySound(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.Pong
                RaiseEvent OnPong(New GameServer.Pong(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PortalInfo
                RaiseEvent OnPortalInfo(New GameServer.PortalInfo(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.PortalOwnership
                RaiseEvent OnPortalOwnership(New GameServer.PortalOwnership(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.QuestItemState
                RaiseEvent OnQuestItemState(New GameServer.QuestItemState(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.Relator1
                RaiseEvent OnRelator1(New GameServer.Relator1(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.Relator2
                RaiseEvent OnRelator2(New GameServer.Relator2(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.RemoveGroundUnit
                RaiseEvent OnRemoveGroundUnit(New GameServer.RemoveGroundUnit(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.ReportKill
                RaiseEvent OnReportKill(New GameServer.ReportKill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.RequestLogonInfo
                RaiseEvent OnRequestLogonInfo(New GameServer.RequestLogonInfo(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SetGameObjectMode
                RaiseEvent OnSetGameObjectMode(New GameServer.SetGameObjectMode(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SetItemState
                RaiseEvent OnSetItemState(New GameServer.SetItemState(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SetNPCMode
                RaiseEvent OnSetNPCMode(New GameServer.SetNPCMode(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SetState
                RaiseEvent OnSetState(New GameServer.SetState(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SkillsLog
                RaiseEvent OnSkillsLog(New GameServer.SkillsLog(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SmallGoldAdd
                RaiseEvent OnSmallGoldAdd(New GameServer.SmallGoldAdd(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SummonAction
                RaiseEvent OnSummonAction(New GameServer.SummonAction(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.SwitchWeaponSet
                RaiseEvent OnSwitchWeaponSet(New GameServer.SwitchWeaponSet(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.TransactionComplete
                RaiseEvent OnTransactionComplete(New GameServer.TransactionComplete(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UnitUseSkill
                RaiseEvent OnUnitUseSkill(New GameServer.UnitUseSkill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UnitUseSkillOnTarget
                RaiseEvent OnUnitUseSkillOnTarget(New GameServer.UnitUseSkillOnTarget(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UnloadDone
                RaiseEvent OnUnloadDone(New GameServer.UnloadDone(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateGameQuestLog
                RaiseEvent OnUpdateGameQuestLog(New GameServer.UpdateGameQuestLog(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateItemStats
                RaiseEvent OnUpdateItemStats(New GameServer.UpdateItemStats(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateItemUI
                RaiseEvent OnUpdateItemUI(New GameServer.UpdateItemUI(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdatePlayerItemSkill
                RaiseEvent OnUpdatePlayerItemSkill(New GameServer.UpdatePlayerItemSkill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateQuestInfo
                RaiseEvent OnUpdateQuestInfo(New GameServer.UpdateQuestInfo(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateQuestLog
                RaiseEvent OnUpdateQuestLog(New GameServer.UpdateQuestLog(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UpdateSkill
                RaiseEvent OnUpdateSkill(New GameServer.UpdateSkill(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UseSpecialItem
                RaiseEvent OnUseSpecialItem(New GameServer.UseSpecialItem(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.UseStackableItem
                RaiseEvent OnUseStackableItem(New GameServer.UseStackableItem(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.WalkVerify
                RaiseEvent OnWalkVerify(New GameServer.WalkVerify(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.WardenCheck
                RaiseEvent OnWardenCheck(New GameServer.WardenCheck(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.WordToExperience
                RaiseEvent OnGainExperience(New GameServer.WordToExperience(Packet.Data), Flag)
            Case D2Packets.GameServerPacket.WorldItemAction
                RaiseEvent OnItemAction(New GameServer.WorldItemAction(Packet.Data), Flag)
        End Select
        If Packet.Flag <> Flag Then
            Packet.Flag = Flag
        End If
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

    Public Sub Echo(ByVal Message As String) Implements IGame.Echo
        Dim Echoer As New GameServer.GameMessage(D2Data.GameMessageType.GameMessage, Message)
        ReceivePacket(Echoer.Data)
    End Sub

    Sub HandleSentMessagePacket(ByRef Packet As Packet)

        Dim PacketObject As New GameClient.SendMessage(Packet.Data)

        'There's a plugin for redvex to do this. It's best to Remove it.
        'If PacketObject.Message.StartsWith(".") And Not PacketObject.Message.StartsWith("..") Then Packet.Flag = Packet.PacketFlag.PacketFlag_Dead

        'Usefull function, I will leave it here.
        If PacketObject.Message = ".Map" Then
            Dim MapBitmap As New Memory.Pathing.Pather
            Dim MapInfo As Memory.Pathing.MapInfo_t
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