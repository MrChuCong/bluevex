Public Interface IGame
    Sub WriteToLog(ByVal Text As String)

#Region " Old "

    'ReadOnly Property CurrentPlayer() As LocalPlayer
    'ReadOnly Property Players() As Collections.Generic.SortedList(Of UInt32, Player)
    'ReadOnly Property NPCs() As Collections.Generic.SortedList(Of UInt32, NPC)
    'ReadOnly Property Monsters() As Collections.Generic.SortedList(Of UInt32, NPC)
    'Item Events
    'Event OnItemDrop(ByVal Item As Item)
    'Event OnRemoveItemFromGround(ByVal ID As UInt32)
    'Event OnWorldItemAction(ByVal ActionType As ItemActionType, ByVal Item As Item)
    'Event OnOwnedItemAction(ByVal ActionType As ItemActionType, ByVal Item As Item)
    'Interaction
    'Sub Interact(ByVal Type As UnitType, ByVal ID As UInt32)
    'Sub UseInventoryItem(ByVal ID As UInt32)
    'Sub UseBeltItem(ByVal ID As UInt32, ByVal ToMerc As Boolean)
    'Sub UseClosestWP(ByVal Destination As WaypointDestination)
    'Sub PickUpItem(ByVal ID As UInt32)
    'Sub SelectSkill(ByVal Skill As SkillType, ByVal LeftSlot As Boolean)
    'Sub SwitchWeapons()
    'Sub UseLeftSkillOnLocation(ByVal x As UInt16, ByVal y As UInt16)
    'Sub UseRightSkillOnLocation(ByVal x As UInt16, ByVal y As UInt16)
    'Sub UseLeftSkillOnObject(ByVal Type As UnitType, ByVal ID As UInt32, ByVal Shift As Boolean)
    'Sub UseRightSkillOnObject(ByVal Type As UnitType, ByVal ID As UInt32, ByVal Shift As Boolean)
    'Sub BuyItem(ByVal ID As UInt32, Optional ByVal Ex As Modifier = Modifier.Standard)
    'Sub SellItem(ByVal ID As UInt32)
    'Sub ItemToBelt(ByVal ID As UInt32, ByVal Location As BeltLocation)
    'Sub RemoveBeltItem(ByVal ID As UInt32)
    'Sub SwitchBeltItem(ByVal CursorID As UInt32, ByVal TargetID As UInt32)
    'Sub ItemToCube(ByVal ID As UInt32, ByVal CubeID As UInt32)
    'Sub ItemToBuffer(ByVal ID As UInt32, ByVal x As UShort, ByVal y As UShort, ByVal Type As BufferType)
    'Sub DropItem(ByVal ID As UInt32)
    'Sub MercItemToCursor(ByVal Location As MercEquipLocation)
    'Sub CursorItemToMerc()
    'Sub RequestObjectUpdate(ByVal Type As UnitType, ByVal ID As UInt32)
    'Sub AskToResMerc()
    'Sub AskToRepairAll()
    'Sub AskToIdentifyAll()
    'Sub TPScrollToBook(ByVal ScrollID As UInt32)
    'Sub DropGold(ByVal Amount As Integer)
    'Death
    'Event OnSlain(ByVal PlayerName As String)
    'Event OnSlainByPlayer(ByVal PlayerName As String, ByVal SlainByName As String, ByVal SlainByClass As CharacterClass)
    'Event OnSlainByMonster(ByVal PlayerName As String, ByVal Monster As NPCType)
    'Event OnSlainByObject(ByVal PlayerName As String, ByVal ObjectID As GameObjectID)
    'Party Changes
    'Event OnPartyChange(ByVal PlayerID As UInt32, ByVal Relationship As PartyRelationshipType)
    'Player Join/Leave Events
    'Event OnPlayerJoin(ByVal PlayerID As UInt32, ByVal PlayerClass As CharacterClass, ByVal PlayerName As String, ByVal PlayerLevel As UInt16, ByVal PartyID As UInt16)
    'Event OnPlayerLeave(ByVal PlayerClass As CharacterClass, ByVal PlayerName As String, ByVal PlayerLevel As UInt16, ByVal PartyID As UInt16, ByVal Reason As String)
    'Sub [Exit]()
    'Event OnGameStart()
    'NPCs
    'Event OnAssignNPC(ByVal ID As UInt32, ByVal Type As NPCType, ByVal Life As Integer, ByVal x As UInt16, ByVal y As UInt16)
    'Event OnNPCMove(ByVal ID As UInt32, ByVal x As UInt16, ByVal y As UInt16)
    'Event OnNPCMoveToObject(ByVal ID As UInt32, ByVal MovementType As Byte, ByVal currentX As UInt16, ByVal currentY As UInt16, ByVal targetType As UnitType, ByVal targetID As UInt32)
    'Event OnMonsterAttacking(ByVal MonsterID As UInt32, ByVal AttackType As UInt16, ByVal Target As UInt32, ByVal TargetType As UnitType, ByVal x As UInt16, ByVal y As UInt16)
    'Event OnMonsterDeath(ByVal MonsterID As UInt32)
    'World
    'Event OnAssignWorldObject(ByVal UnitID As UInt32, ByVal ObjectID As GameObjectID, ByVal x As UInt16, ByVal y As UInt16, ByVal ObjectState As GameObjectState, ByVal InteractionType As GameInteractiveObjectType, ByVal Destination As AreaLevel)
    'Event OnAssignWarp(ByVal Type As UnitType, ByVal ID As UInt32, ByVal Warp As WarpType, ByVal x As UInt16, ByVal y As UInt16)
    'Reassign Event
    'Event OnReassign(ByVal Type As UnitType, ByVal ID As UInt32, ByVal x As UInt16, ByVal y As UInt16, ByVal Reassign As Boolean)
    'Player Stat Change
    'Event OnLifeManaChange(ByVal x As UInt16, ByVal y As UInt16, ByVal Stamina As UInt16, ByVal Life As UInt16, ByVal Mana As UInt16)
    'Run/Walk Methods
    'Event OnMove(ByVal x As UInt16, ByVal y As UInt16, ByVal Walk As Boolean)
    'Sub Move(ByVal x As UInt16, ByVal y As UInt16, Optional ByVal Walk As Boolean = False)
    'Sub Move(ByVal Type As UnitType, ByVal ID As UInt32, Optional ByVal walk As Boolean = False)
    'Event OnMoved(ByVal x As UInt16, ByVal y As UInt16, ByVal Stamina As UInt16)
    'Event OnAreaChange(ByVal OldArea As AreaLevel, ByVal NewArea As AreaLevel)
    'Event OnActChange(ByVal OldAct As Act, ByVal NewAct As Act)
    'Corpse Pickup
    'Event OnVisibleCorpse(ByVal PlayerID As UInteger, ByVal CorpseID As UInteger)
    'Event OnCorpsePickup()
    'Useful Methods
    'Sub TalkToNPC(ByVal NPC As TownFolk)
    'Sub CancelNPC()
    'Function InTown() As Boolean
    'Sub GoToTown()
    'Sub GoToStash()
    'Sub TPToTown()
    'Sub TalkToRepairNPC()
    'Sub TalkToMercNPC()
    'Sub TalkToHealNPC()
    'Sub TalkToDeckardCain()
    'Sub ResMerc()
    'Sub RepairAll()
    'Sub IdentifyAll()
    'Sub PickupCorpse()
    'Chat Methods
    'Event OnReceiveMessage(ByVal Packet As GameServer.GameMessage)
    'Event OnSendMessage(ByVal Packet As GameClient.SendMessage)
    Sub SendMessage(ByVal Message As String)
    Sub ReceiveMessage(ByVal Name As String, ByVal Message As String)
    Sub ReceiveMessage(ByVal Message As String)

#End Region

#Region " Game Client Events "

    Event OnAddBeltItem(ByVal Packet As GameClient.AddBeltItem, ByRef Flag As Packet.PacketFlag)
    Event OnBuyItem(ByVal Packet As GameClient.BuyItem, ByRef Flag As Packet.PacketFlag)
    Event OnCainIdentifyItems(ByVal Packet As GameClient.CainIdentifyItems, ByRef Flag As Packet.PacketFlag)
    Event OnCastLeftSkill(ByVal Packet As GameClient.CastLeftSkill, ByRef Flag As Packet.PacketFlag)
    Event OnCastLeftSkillOnTarget(ByVal Packet As GameClient.CastLeftSkillOnTarget, ByRef Flag As Packet.PacketFlag)
    Event OnCastLeftSkillOnTargetStopped(ByVal Packet As GameClient.CastLeftSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag)
    Event OnCastRightSkill(ByVal Packet As GameClient.CastRightSkill, ByRef Flag As Packet.PacketFlag)
    Event OnCastRightSkillOnTarget(ByVal Packet As GameClient.CastRightSkillOnTarget, ByRef Flag As Packet.PacketFlag)
    Event OnCastRightSkillOnTargetStopped(ByVal Packet As GameClient.CastRightSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag)
    Event OnChangeMercEquipment(ByVal Packet As GameClient.ChangeMercEquipment, ByRef Flag As Packet.PacketFlag)
    Event OnClickButton(ByVal Packet As GameClient.ClickButton, ByRef Flag As Packet.PacketFlag)
    Event OnCloseQuest(ByVal Packet As GameClient.CloseQuest, ByRef Flag As Packet.PacketFlag)
    Event OnDisplayQuestMessage(ByVal Packet As GameClient.DisplayQuestMessage, ByRef Flag As Packet.PacketFlag)
    Event OnDropGold(ByVal Packet As GameClient.DropGold, ByRef Flag As Packet.PacketFlag)
    Event OnDropItem(ByVal Packet As GameClient.DropItem, ByRef Flag As Packet.PacketFlag)
    Event OnDropItemToContainer(ByVal Packet As GameClient.DropItemToContainer, ByRef Flag As Packet.PacketFlag)
    Event OnEmbedItem(ByVal Packet As GameClient.EmbedItem, ByRef Flag As Packet.PacketFlag)
    Event OnEnterGame(ByVal Packet As GameClient.EnterGame, ByRef Flag As Packet.PacketFlag)
    Event OnEquipItem(ByVal Packet As GameClient.EquipItem, ByRef Flag As Packet.PacketFlag)
    Event OnExitGame(ByVal Packet As GameClient.ExitGame, ByRef Flag As Packet.PacketFlag)
    Event OnGameLogonRequest(ByVal Packet As GameClient.GameLogonRequest, ByRef Flag As Packet.PacketFlag)
    Event OnGoToLocation(ByVal Packet As GameClient.GoToLocation, ByRef Flag As Packet.PacketFlag)
    Event OnGoToTarget(ByVal Packet As GameClient.GoToTarget, ByRef Flag As Packet.PacketFlag)
    Event OnGoToTownFolk(ByVal Packet As GameClient.GoToTownFolk, ByRef Flag As Packet.PacketFlag)
    Event OnHireMercenary(ByVal Packet As GameClient.HireMercenary, ByRef Flag As Packet.PacketFlag)
    Event OnHoverUnit(ByVal Packet As GameClient.HoverUnit, ByRef Flag As Packet.PacketFlag)
    Event OnIdentifyGambleItem(ByVal Packet As GameClient.IdentifyGambleItem, ByRef Flag As Packet.PacketFlag)
    Event OnIdentifyItem(ByVal Packet As GameClient.IdentifyItem, ByRef Flag As Packet.PacketFlag)
    Event OnIncrementAttribute(ByVal Packet As GameClient.IncrementAttribute, ByRef Flag As Packet.PacketFlag)
    Event OnIncrementSkill(ByVal Packet As GameClient.IncrementSkill, ByRef Flag As Packet.PacketFlag)
    Event OnInventoryItemToBelt(ByVal Packet As GameClient.InventoryItemToBelt, ByRef Flag As Packet.PacketFlag)
    Event OnItemToCube(ByVal Packet As GameClient.ItemToCube, ByRef Flag As Packet.PacketFlag)
    Event OnPartyRequest(ByVal Packet As GameClient.PartyRequest, ByRef Flag As Packet.PacketFlag)
    Event OnPickItem(ByVal Packet As GameClient.PickItem, ByRef Flag As Packet.PacketFlag)
    Event OnPickItemFromContainer(ByVal Packet As GameClient.PickItemFromContainer, ByRef Flag As Packet.PacketFlag)
    Event OnPing(ByVal Packet As GameClient.Ping, ByRef Flag As Packet.PacketFlag)
    Event OnRecastLeftSkill(ByVal Packet As GameClient.RecastLeftSkill, ByRef Flag As Packet.PacketFlag)
    Event OnRecastLeftSkillOnTarget(ByVal Packet As GameClient.RecastLeftSkillOnTarget, ByRef Flag As Packet.PacketFlag)
    Event OnRecastLeftSkillOnTargetStopped(ByVal Packet As GameClient.RecastLeftSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag)
    Event OnRecastRightSkill(ByVal Packet As GameClient.RecastRightSkill, ByRef Flag As Packet.PacketFlag)
    Event OnRecastRightSkillOnTarget(ByVal Packet As GameClient.RecastRightSkillOnTarget, ByRef Flag As Packet.PacketFlag)
    Event OnRecastRightSkillOnTargetStopped(ByVal Packet As GameClient.RecastRightSkillOnTargetStopped, ByRef Flag As Packet.PacketFlag)
    Event OnRemoveBeltItem(ByVal Packet As GameClient.RemoveBeltItem, ByRef Flag As Packet.PacketFlag)
    Event OnRequestQuestLog(ByVal Packet As GameClient.RequestQuestLog, ByRef Flag As Packet.PacketFlag)
    Event OnRequestReassign(ByVal Packet As GameClient.RequestReassign, ByRef Flag As Packet.PacketFlag)
    Event OnRespawn(ByVal Packet As GameClient.Respawn, ByRef Flag As Packet.PacketFlag)
    Event OnResurrectMerc(ByVal Packet As GameClient.ResurrectMerc, ByRef Flag As Packet.PacketFlag)
    Event OnRunToLocation(ByVal Packet As GameClient.RunToLocation, ByRef Flag As Packet.PacketFlag)
    Event OnRunToTarget(ByVal Packet As GameClient.RunToTarget, ByRef Flag As Packet.PacketFlag)
    Event OnSelectSkill(ByVal Packet As GameClient.SelectSkill, ByRef Flag As Packet.PacketFlag)
    Event OnSellItem(ByVal Packet As GameClient.SellItem, ByRef Flag As Packet.PacketFlag)
    Event OnSendCharacterSpeech(ByVal Packet As GameClient.SendCharacterSpeech, ByRef Flag As Packet.PacketFlag)
    Event OnSendMessage(ByVal Packet As GameClient.SendMessage, ByRef Flag As Packet.PacketFlag)
    Event OnSendOverheadMessage(ByVal Packet As GameClient.SendOverheadMessage, ByRef Flag As Packet.PacketFlag)
    Event OnSetPlayerRelation(ByVal Packet As GameClient.SetPlayerRelation, ByRef Flag As Packet.PacketFlag)
    Event OnSetSkillHotkey(ByVal Packet As GameClient.SetSkillHotkey, ByRef Flag As Packet.PacketFlag)
    Event OnStackItems(ByVal Packet As GameClient.StackItems, ByRef Flag As Packet.PacketFlag)
    Event OnSwapBeltItem(ByVal Packet As GameClient.SwapBeltItem, ByRef Flag As Packet.PacketFlag)
    Event OnSwapContainerItem(ByVal Packet As GameClient.SwapContainerItem, ByRef Flag As Packet.PacketFlag)
    Event OnSwapEquippedItem(ByVal Packet As GameClient.SwapEquippedItem, ByRef Flag As Packet.PacketFlag)
    Event OnSwitchWeapons(ByVal Packet As GameClient.SwitchWeapons, ByRef Flag As Packet.PacketFlag)
    Event OnTownFolkCancelInteraction(ByVal Packet As GameClient.TownFolkCancelInteraction, ByRef Flag As Packet.PacketFlag)
    Event OnTownFolkInteract(ByVal Packet As GameClient.TownFolkInteract, ByRef Flag As Packet.PacketFlag)
    Event OnTownFolkMenuSelect(ByVal Packet As GameClient.TownFolkMenuSelect, ByRef Flag As Packet.PacketFlag)
    Event OnTownFolkRepair(ByVal Packet As GameClient.TownFolkRepair, ByRef Flag As Packet.PacketFlag)
    Event OnUnequipItem(ByVal Packet As GameClient.UnequipItem, ByRef Flag As Packet.PacketFlag)
    Event OnUnitInteract(ByVal Packet As GameClient.UnitInteract, ByRef Flag As Packet.PacketFlag)
    Event OnUpdatePosition(ByVal Packet As GameClient.UpdatePosition, ByRef Flag As Packet.PacketFlag)
    Event OnUseBeltItem(ByVal Packet As GameClient.UseBeltItem, ByRef Flag As Packet.PacketFlag)
    Event OnUseInventoryItem(ByVal Packet As GameClient.UseInventoryItem, ByRef Flag As Packet.PacketFlag)
    Event OnWalkToLocation(ByVal Packet As GameClient.WalkToLocation, ByRef Flag As Packet.PacketFlag)
    Event OnWalkToTarget(ByVal Packet As GameClient.WalkToTarget, ByRef Flag As Packet.PacketFlag)
    Event OnWardenResponse(ByVal Packet As GameClient.WardenResponse, ByRef Flag As Packet.PacketFlag)
    Event OnWaypointInteract(ByVal Packet As GameClient.WaypointInteract, ByRef Flag As Packet.PacketFlag)

#End Region

#Region " Game Server Events "

    Event OnAboutPlayer(ByVal Packet As GameServer.AboutPlayer, ByRef Flag As Packet.PacketFlag)
    Event OnAcceptTrade(ByVal Packet As GameServer.AcceptTrade, ByRef Flag As Packet.PacketFlag)
    Event OnAddUnit(ByVal Packet As GameServer.AddUnit, ByRef Flag As Packet.PacketFlag)
    Event OnAssignGameObject(ByVal Packet As GameServer.AssignGameObject, ByRef Flag As Packet.PacketFlag)
    Event OnAssignMerc(ByVal Packet As GameServer.AssignMerc, ByRef Flag As Packet.PacketFlag)
    Event OnAssignNPC(ByVal Packet As GameServer.AssignNPC, ByRef Flag As Packet.PacketFlag)
    Event OnAssignPlayer(ByVal Packet As GameServer.AssignPlayer, ByRef Flag As Packet.PacketFlag)
    Event OnAssignPlayerCorpse(ByVal Packet As GameServer.AssignPlayerCorpse, ByRef Flag As Packet.PacketFlag)
    Event OnAssignPlayerToParty(ByVal Packet As GameServer.AssignPlayerToParty, ByRef Flag As Packet.PacketFlag)
    Event OnAssignSkill(ByVal Packet As GameServer.AssignSkill, ByRef Flag As Packet.PacketFlag)
    Event OnAssignSkillHotkey(ByVal Packet As GameServer.AssignSkillHotkey, ByRef Flag As Packet.PacketFlag)
    Event OnAssignWarp(ByVal Packet As GameServer.AssignWarp, ByRef Flag As Packet.PacketFlag)
    Event OnAttributeNotification(ByVal Packet As GameServer.AttributeNotification, ByRef Flag As Packet.PacketFlag)
    Event OnDelayedState(ByVal Packet As GameServer.DelayedState, ByRef Flag As Packet.PacketFlag)
    Event OnEndState(ByVal Packet As GameServer.EndState, ByRef Flag As Packet.PacketFlag)
    Event OnGainExperience(ByVal Packet As GameServer.GainExperience, ByRef Flag As Packet.PacketFlag)
    Event OnGameHandshake(ByVal Packet As GameServer.GameHandshake, ByRef Flag As Packet.PacketFlag)
    Event OnGameLoading(ByVal Packet As GameServer.GameLoading, ByRef Flag As Packet.PacketFlag)
    Event OnGameLogonReceipt(ByVal Packet As GameServer.GameLogonReceipt, ByRef Flag As Packet.PacketFlag)
    Event OnGameLogonSuccess(ByVal Packet As GameServer.GameLogonSuccess, ByRef Flag As Packet.PacketFlag)
    Event OnGameLogoutSuccess(ByVal Packet As GameServer.GameLogoutSuccess, ByRef Flag As Packet.PacketFlag)
    Event OnReceiveMessage(ByVal Packet As GameServer.GameMessage, ByRef Flag As Packet.PacketFlag)
    Event OnGameOver(ByVal Packet As GameServer.GameOver, ByRef Flag As Packet.PacketFlag)
    Event OnGoldTrade(ByVal Packet As GameServer.GoldTrade, ByRef Flag As Packet.PacketFlag)
    Event OnInformationMessage(ByVal Packet As GameServer.InformationMessage, ByRef Flag As Packet.PacketFlag)

    Event OnOwnedItemAction(ByVal Packet As GameServer.OwnedItemAction, ByRef Flag As Packet.PacketFlag)

    Event OnItemTriggerSkill(ByVal Packet As GameServer.ItemTriggerSkill, ByRef Flag As Packet.PacketFlag)
    Event OnLoadAct(ByVal Packet As GameServer.LoadAct, ByRef Flag As Packet.PacketFlag)
    Event OnLoadDone(ByVal Packet As GameServer.LoadDone, ByRef Flag As Packet.PacketFlag)
    Event OnMapAdd(ByVal Packet As GameServer.MapAdd, ByRef Flag As Packet.PacketFlag)
    Event OnMapRemove(ByVal Packet As GameServer.MapRemove, ByRef Flag As Packet.PacketFlag)
    Event OnMercAttributeNotification(ByVal Packet As GameServer.MercAttributeNotification, ByRef Flag As Packet.PacketFlag)
    Event OnMercForHire(ByVal Packet As GameServer.MercForHire, ByRef Flag As Packet.PacketFlag)
    Event OnMercForHireListStart(ByVal Packet As GameServer.MercForHireListStart, ByRef Flag As Packet.PacketFlag)
    Event OnMercGainExperience(ByVal Packet As GameServer.MercGainExperience, ByRef Flag As Packet.PacketFlag)
    Event OnMonsterAttack(ByVal Packet As GameServer.MonsterAttack, ByRef Flag As Packet.PacketFlag)
    Event OnNPCAction(ByVal Packet As GameServer.NPCAction, ByRef Flag As Packet.PacketFlag)
    Event OnNPCGetHit(ByVal Packet As GameServer.NPCGetHit, ByRef Flag As Packet.PacketFlag)
    Event OnNPCHeal(ByVal Packet As GameServer.NPCHeal, ByRef Flag As Packet.PacketFlag)
    Event OnNPCInfo(ByVal Packet As GameServer.NPCInfo, ByRef Flag As Packet.PacketFlag)
    Event OnNPCMove(ByVal Packet As GameServer.NPCMove, ByRef Flag As Packet.PacketFlag)
    Event OnNPCMoveToTarget(ByVal Packet As GameServer.NPCMoveToTarget, ByRef Flag As Packet.PacketFlag)
    Event OnNPCStop(ByVal Packet As GameServer.NPCStop, ByRef Flag As Packet.PacketFlag)
    Event OnNPCWantsInteract(ByVal Packet As GameServer.NPCWantsInteract, ByRef Flag As Packet.PacketFlag)
    Event OnOpenWaypoint(ByVal Packet As GameServer.OpenWaypoint, ByRef Flag As Packet.PacketFlag)
    Event OnPartyMemberPulse(ByVal Packet As GameServer.PartyMemberPulse, ByRef Flag As Packet.PacketFlag)
    Event OnPartyMemberUpdate(ByVal Packet As GameServer.PartyMemberUpdate, ByRef Flag As Packet.PacketFlag)
    Event OnPartyRefresh(ByVal Packet As GameServer.PartyRefresh, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerAttributeNotification(ByVal Packet As GameServer.PlayerAttributeNotification, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerClearCursor(ByVal Packet As GameServer.PlayerClearCursor, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerCorpseVisible(ByVal Packet As GameServer.PlayerCorpseVisible, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerInGame(ByVal Packet As GameServer.PlayerInGame, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerInSight(ByVal Packet As GameServer.PlayerInSight, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerKillCount(ByVal Packet As GameServer.PlayerKillCount, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerLeaveGame(ByVal Packet As GameServer.PlayerLeaveGame, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerLifeManaChange(ByVal Packet As GameServer.PlayerLifeManaChange, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerMove(ByVal Packet As GameServer.PlayerMove, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerMoveToTarget(ByVal Packet As GameServer.PlayerMoveToTarget, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerPartyRelationship(ByVal Packet As GameServer.PlayerPartyRelationship, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerReassign(ByVal Packet As GameServer.PlayerReassign, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerRelationship(ByVal Packet As GameServer.PlayerRelationship, ByRef Flag As Packet.PacketFlag)
    Event OnPlayerStop(ByVal Packet As GameServer.PlayerStop, ByRef Flag As Packet.PacketFlag)
    Event OnPlaySound(ByVal Packet As GameServer.PlaySound, ByRef Flag As Packet.PacketFlag)
    Event OnPong(ByVal Packet As GameServer.Pong, ByRef Flag As Packet.PacketFlag)
    Event OnPortalInfo(ByVal Packet As GameServer.PortalInfo, ByRef Flag As Packet.PacketFlag)
    Event OnPortalOwnership(ByVal Packet As GameServer.PortalOwnership, ByRef Flag As Packet.PacketFlag)
    Event OnQuestItemState(ByVal Packet As GameServer.QuestItemState, ByRef Flag As Packet.PacketFlag)
    Event OnRelator1(ByVal Packet As GameServer.Relator1, ByRef Flag As Packet.PacketFlag)
    Event OnRelator2(ByVal Packet As GameServer.Relator2, ByRef Flag As Packet.PacketFlag)
    Event OnRemoveGroundUnit(ByVal Packet As GameServer.RemoveGroundUnit, ByRef Flag As Packet.PacketFlag)
    Event OnReportKill(ByVal Packet As GameServer.ReportKill, ByRef Flag As Packet.PacketFlag)
    Event OnRequestLogonInfo(ByVal Packet As GameServer.RequestLogonInfo, ByRef Flag As Packet.PacketFlag)
    Event OnSetGameObjectMode(ByVal Packet As GameServer.SetGameObjectMode, ByRef Flag As Packet.PacketFlag)
    Event OnSetItemState(ByVal Packet As GameServer.SetItemState, ByRef Flag As Packet.PacketFlag)
    Event OnSetNPCMode(ByVal Packet As GameServer.SetNPCMode, ByRef Flag As Packet.PacketFlag)
    Event OnSetState(ByVal Packet As GameServer.SetState, ByRef Flag As Packet.PacketFlag)
    Event OnSkillsLog(ByVal Packet As GameServer.SkillsLog, ByRef Flag As Packet.PacketFlag)
    Event OnSmallGoldAdd(ByVal Packet As GameServer.SmallGoldAdd, ByRef Flag As Packet.PacketFlag)
    Event OnSummonAction(ByVal Packet As GameServer.SummonAction, ByRef Flag As Packet.PacketFlag)
    Event OnSwitchWeaponSet(ByVal Packet As GameServer.SwitchWeaponSet, ByRef Flag As Packet.PacketFlag)
    Event OnTransactionComplete(ByVal Packet As GameServer.TransactionComplete, ByRef Flag As Packet.PacketFlag)
    Event OnUnitUseSkill(ByVal Packet As GameServer.UnitUseSkill, ByRef Flag As Packet.PacketFlag)
    Event OnUnitUseSkillOnTarget(ByVal Packet As GameServer.UnitUseSkillOnTarget, ByRef Flag As Packet.PacketFlag)
    Event OnUnloadDone(ByVal Packet As GameServer.UnloadDone, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateGameQuestLog(ByVal Packet As GameServer.UpdateGameQuestLog, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateItemStats(ByVal Packet As GameServer.UpdateItemStats, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateItemUI(ByVal Packet As GameServer.UpdateItemUI, ByRef Flag As Packet.PacketFlag)
    Event OnUpdatePlayerItemSkill(ByVal Packet As GameServer.UpdatePlayerItemSkill, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateQuestInfo(ByVal Packet As GameServer.UpdateQuestInfo, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateQuestLog(ByVal Packet As GameServer.UpdateQuestLog, ByRef Flag As Packet.PacketFlag)
    Event OnUpdateSkill(ByVal Packet As GameServer.UpdateSkill, ByRef Flag As Packet.PacketFlag)
    Event OnUseSpecialItem(ByVal Packet As GameServer.UseSpecialItem, ByRef Flag As Packet.PacketFlag)
    Event OnUseStackableItem(ByVal Packet As GameServer.UseStackableItem, ByRef Flag As Packet.PacketFlag)
    Event OnWalkVerify(ByVal Packet As GameServer.WalkVerify, ByRef Flag As Packet.PacketFlag)
    Event OnWardenCheck(ByVal Packet As GameServer.WardenCheck, ByRef Flag As Packet.PacketFlag)
    Event OnWorldItemAction(ByVal Packet As GameServer.WorldItemAction, ByRef Flag As Packet.PacketFlag)

#End Region

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

    'Hot Keys
    'Sub RegisterHotKey(ByVal Key As System.Windows.Forms.Keys, ByVal Modifiers As HotKey.HotKeyModifiers)
    'Event OnHotKey(ByVal Key As System.Windows.Forms.Keys, ByVal Modifiers As HotKey.HotKeyModifiers)
End Interface