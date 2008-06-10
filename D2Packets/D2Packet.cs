using System;
using ETUtils;
using D2Data;
using System.Runtime.InteropServices;

namespace D2Packets
{
	/// <summary>
	/// Base class for Diablo II Packets
	/// </summary>
	public class D2Packet
	{
		public readonly Origin Origin;
		public readonly byte[] Data;

		public D2Packet(byte[] data, Origin origin) 
		{
			this.Data = data;
			this.Origin = origin;
        }

        #region ToString & co

        // TODO: accept format strings	// {0} prefix, {1} hex, {2} string...
		public string ToDataString()
		{
			return ByteConverter.ToHexString(this.Data);
		}

		public string ToLongDataString()
		{
			return ByteConverter.ToFormatedHexString(this.Data);
		}
		
		public string ToInfoString()
		{
			return this.ToInfoString(IncludeType, NameValueSeparator, ItemSeparator, ItemFormat, StartFormat);
		}
		public string ToInfoString(bool includeType)
		{
			return this.ToInfoString(includeType, NameValueSeparator, ItemSeparator, ItemFormat, StartFormat);
		}

		public string ToLongInfoString()
		{
			return this.ToInfoString(IncludeType, NameValueSeparator, LongItemSeparator, LongItemFormat, LongStartFormat);
		}
		public string ToLongInfoString(bool includeType)
		{
			return this.ToInfoString(includeType, NameValueSeparator, LongItemSeparator, LongItemFormat, LongStartFormat);
		}

		public string ToInfoString(bool includeType, string nameValueSeparator)
		{
			return this.ToInfoString(includeType, nameValueSeparator, ItemSeparator, ItemFormat, StartFormat);
		}
		public string ToInfoString(bool includeType, string nameValueSeparator, string itemSeparator)
		{
			return this.ToInfoString(includeType, nameValueSeparator, itemSeparator, ItemFormat, StartFormat);
		}
		public string ToInfoString(bool includeType, string nameValueSeparator, string itemSeparator, string itemFormat)
		{
			return this.ToInfoString(includeType, nameValueSeparator, itemSeparator, itemFormat, StartFormat);
		}
		public string ToInfoString(bool includeType, string nameValueSeparator, string itemSeparator, string itemFormat, string startFormat)
		{
			Type type = this.GetType();
			// Don't dump uninherited classes
			if (type.BaseType == typeof(D2Packet))
				return null;

			// Packet type
			string startStr;
			if (includeType) startStr = String.Concat(type.BaseType.Namespace, ".", type.Name);
			else startStr = type.Name;

			return String.Format(startFormat, startStr) 
				+ StringUtils.ToFormatedInfoString(this, includeType, nameValueSeparator, itemSeparator, itemFormat);;
		}

		public static bool IncludeType = true;
		public static string NameValueSeparator = ": ";

		public static string ItemSeparator = "; ";
		public static string StartFormat = "{0}" + ItemSeparator;
		public static string ItemFormat = "{0}{1}{2}";

		public static string LongItemSeparator = Environment.NewLine + "         ";
		public static string LongStartFormat = "{0}" + LongItemSeparator;
		public static string LongItemFormat = "{0}{1,-20}{2}";
        
        #endregion ToString & co

        #region Packet Sizes

        public static readonly int[] GSPacketSizeArray = new int[] {
		/*	0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F	*/
/* 0 */		1,   8,   1,   12,  1,   1,   1,   6,   6,   11,  6,   6,   9,   13,  12,  16, 
/* 1 */		16,  8,   26,  14,  18,  11,  -1,  -1,  15,  2,   2,   3,   5,   3,   4,   6, 
/* 2 */		10,  12,  12,  13,  90,  90,  -1,  40,  103, 97,  15,  0,   8,   0,   0,   0, 
/* 3 */		0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   34,  8, 
/* 4 */		13,  0,   6,   0,   0,   13,  0,   11,  11,  0,   0,   0,   16,  17,  7,   1, 
/* 5 */		15,  14,  42,  10,  3,   0,   0,   14,  7,   26,  40,  -1,  5,   6,   38,  5, 
/* 6 */		7,   2,   7,   21,  0,   7,   7,   16,  21,  12,  12,  16,  16,  10,  1,   1, 
/* 7 */		1,   1,   1,   32,  10,  13,  6,   2,   21,  6,   13,  8,   6,   18,  5,   10, 
/* 8 */		4,   20,  29,  0,   0,   0,   0,   0,   0,   2,   6,   6,   11,  7,   10,  33, 
/* 9 */		13,  26,  6,   8,   -1,  13,  9,   1,   7,   16,  17,  7,   -1,  -1,  7,   8, 
/* A */		10,  7,   8,   24,  3,   8,   -1,  7,   -1,  7,   -1,  7,   -1,  0,   -1,  0, 
/* B */		1
		};

        public static int GetGSPacketSize(byte[] data)
        {
            return GetGSPacketSize(data, 0, data.Length);
        }
        public static int GetGSPacketSize(byte[] data, int offset, int length)
        {
            if (data[offset] > GSPacketSizeArray.Length)
                return 0;

            int pLen = GSPacketSizeArray[data[offset]];
            if (pLen == -1)
            {
                switch (data[offset])
                {
                    //case 0x16:
                    //case 0x17:
                    //case 0xA6:

                    case 0x26:
                        if (length < 13) break;
                        int isOverhead = (data[offset + 1] == 5) ? 1 : 0;
                        for (int j = 10 + isOverhead; j < length; j++)
                        {
                            if (data[j + offset] == 0)
                            {	// Find 2 null terminated strings for normal messages (name + msg)
                                if (isOverhead == 0)
                                    isOverhead = 1;
                                else
                                {
                                    pLen = j + 1;
                                    break;
                                }
                            }
                        }
                        break;
                    case 0x5B:
                        if (length < 3) break;
                        pLen = BitConverter.ToUInt16(data, offset + 1);
                        break;
                    case 0x94:
                        if (length < 2) break;
                        pLen = 6 + (data[offset + 1] * 3);
                        break;
                    case 0x9C:
                    case 0x9D:
                        if (length < 3) break;
                        pLen = data[offset + 2];
                        break;
                    case 0xA8:
                    case 0xAA:
                        if (length < 7) break;
                        pLen = data[offset + 6];
                        break;
                    case 0xAC:
                        if (length < 13) break;
                        pLen = data[offset + 12];
                        break;
                    case 0xAE:
                        if (length < 4) break;
                        pLen = 3 + BitConverter.ToUInt16(data, offset + 1);
                        break;
                    default:
                        pLen = 0;
                        break;
                }
            }
            return pLen;
        }

        public static readonly int[] GCPacketSizeArray = new int[] {
		/*	0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F	*/
/* 0 */		0,   5,   9,   5,   9,   5,   9,   9,   5,   9,   9,   1,   5,   9,   9,   5, 
/* 1 */		9,   9,   1,   9,   -1,  -1,  13,  5,   17,  5,   9,   9,   3,   9,   9,   17, 
/* 2 */		13,  9,   5,   9,   5,   9,   13,  9,   9,   9,   9,   0,   0,   1,   3,   9, 
/* 3 */		9,   9,   17,  17,  5,   17,  9,   5,   13,  5,   3,   3,   9,   5,   5,   3, 
/* 4 */		1,   1,   1,   1,   17,  9,   13,  13,  1,   9,   0,   9,   5,   3,   0,   7, 
/* 5 */		9,   9,   5,   1,   1,   0,   0,   0,   3,   17,  0,   0,   0,   7,   6,   5, 
/* 6 */		1,   3,   5,   5,   9,   17,  -1,  0,   37,  1,   1,   1,   1,   13,  0,   1,
		};

        public static int GetGCPacketSize(byte[] data)
        {
            return GetGCPacketSize(data, 0, data.Length);
        }
        public static int GetGCPacketSize(byte[] data, int offset, int length)
        {
            if (data[offset] >= GCPacketSizeArray.Length)
                return 0;

            int pLen = GCPacketSizeArray[data[offset]];
            if (pLen == -1)
            {
                switch (data[offset])
                {
                    case 0x14:
                    case 0x15: // 3 + 3 * NULLSTRING
                        pLen = 3;
                        for (int i = 0; i < 3; i++)
                        {
                            int j = 0;
                            while(true)
                            {
                                if (length < j + pLen + offset)
                                    return -1;
                                if (data[j + pLen + offset] == 0)
                                {
                                    pLen += j + 1;
                                    break;
                                }
                                j++;
                            }
                        }
                        break;
                    case 0x66:
                        if (length < offset + 5) pLen = 0;
                        pLen = 3 + BitConverter.ToUInt16(data, offset + 1);
                        break;
                    default:
                        pLen = 0;
                        break;
                }
            }
            return pLen;
        }

        #endregion Packet Sizes

        #region Packet Types

		static D2Packet()
		{
            BSPacketTypes[0x00] = typeof(BnetServer.KeepAlive);
            BSPacketTypes[0x0A] = typeof(BnetServer.EnterChatResponse);
            BSPacketTypes[0x0B] = typeof(BnetServer.ChannelList);
			BSPacketTypes[0x0F] = typeof(BnetServer.ChatEvent);
            BSPacketTypes[0x15] = typeof(BnetServer.AdInfo);
            BSPacketTypes[0x25] = typeof(BnetServer.BnetPing);
            BSPacketTypes[0x33] = typeof(BnetServer.FileTimeInfo);
            BSPacketTypes[0x3A] = typeof(BnetServer.BnetLogonResponse);
			BSPacketTypes[0x3E] = typeof(BnetServer.RealmLogonResponse);
            BSPacketTypes[0x40] = typeof(BnetServer.QueryRealmsResponse);
            BSPacketTypes[0x46] = typeof(BnetServer.NewsInfo);
            BSPacketTypes[0x4A] = typeof(BnetServer.ExtraWorkInfo);
            BSPacketTypes[0x4C] = typeof(BnetServer.RequiredExtraWorkInfo);
            BSPacketTypes[0x50] = typeof(BnetServer.BnetConnectionResponse);
            BSPacketTypes[0x51] = typeof(BnetServer.BnetAuthResponse);



            BCPacketTypes[0x00] = typeof(BnetClient.KeepAlive);
            BCPacketTypes[0x10] = typeof(BnetClient.LeaveChat);
            BCPacketTypes[0x15] = typeof(BnetClient.AdInfoRequest);
            BCPacketTypes[0x1C] = typeof(BnetClient.StartGame);
            BCPacketTypes[0x1F] = typeof(BnetClient.LeaveGame);
            BCPacketTypes[0x21] = typeof(BnetClient.DisplayAd);
            BCPacketTypes[0x22] = typeof(BnetClient.NotifyJoin);
            BCPacketTypes[0x25] = typeof(BnetClient.BnetPong);
            BCPacketTypes[0x33] = typeof(BnetClient.FileTimeRequest);
            BCPacketTypes[0x3A] = typeof(BnetClient.BnetLogonRequest);
            BCPacketTypes[0x3E] = typeof(BnetClient.RealmLogonRequest);
            BCPacketTypes[0x40] = typeof(BnetClient.QueryRealms);
            BCPacketTypes[0x46] = typeof(BnetClient.NewsInfoRequest);
            BCPacketTypes[0x4B] = typeof(BnetClient.ExtraWorkResponse);
            BCPacketTypes[0x50] = typeof(BnetClient.BnetConnectionRequest);
            BCPacketTypes[0x51] = typeof(BnetClient.BnetAuthRequest);
            BCPacketTypes[0x0A] = typeof(BnetClient.EnterChatRequest);
            BCPacketTypes[0x0B] = typeof(BnetClient.ChannelListRequest);
            BCPacketTypes[0x0C] = typeof(BnetClient.JoinChannel);
            BCPacketTypes[0x0E] = typeof(BnetClient.ChatCommand);



            RSPacketTypes[0x01] = typeof(RealmServer.RealmStartupResponse);
            RSPacketTypes[0x02] = typeof(RealmServer.CharacterCreationResponse);
            RSPacketTypes[0x03] = typeof(RealmServer.CreateGameResponse);
            RSPacketTypes[0x04] = typeof(RealmServer.JoinGameResponse);
            RSPacketTypes[0x05] = typeof(RealmServer.GameList);
            RSPacketTypes[0x06] = typeof(RealmServer.GameInfo);
            RSPacketTypes[0x07] = typeof(RealmServer.CharacterLogonResponse);
            RSPacketTypes[0x0A] = typeof(RealmServer.CharacterDeletionResponse);
			RSPacketTypes[0x12] = typeof(RealmServer.MessageOfTheDay);
			RSPacketTypes[0x14] = typeof(RealmServer.GameCreationQueue);
            RSPacketTypes[0x18] = typeof(RealmServer.CharacterUpgradeResponse);
			RSPacketTypes[0x19] = typeof(RealmServer.CharacterList);
			


			RCPacketTypes[0x01] = typeof(RealmClient.RealmStartupRequest);
			RCPacketTypes[0x02] = typeof(RealmClient.CharacterCreationRequest);
			RCPacketTypes[0x03] = typeof(RealmClient.CreateGameRequest);
			RCPacketTypes[0x04] = typeof(RealmClient.JoinGameRequest);
			RCPacketTypes[0x05] = typeof(RealmClient.GameListRequest);
			RCPacketTypes[0x06] = typeof(RealmClient.GameInfoRequest);
			RCPacketTypes[0x07] = typeof(RealmClient.CharacterLogonRequest);
			RCPacketTypes[0x0A] = typeof(RealmClient.CharacterDeletionRequest);
            RCPacketTypes[0x12] = typeof(RealmClient.MessageOfTheDayRequest);
			RCPacketTypes[0x13] = typeof(RealmClient.CancelGameCreation);
			RCPacketTypes[0x18] = typeof(RealmClient.CharacterUpgradeRequest);
            RCPacketTypes[0x19] = typeof(RealmClient.CharacterListRequest);
			


			GSPacketTypes[0x00] = typeof(GameServer.GameLoading);
			GSPacketTypes[0x01] = typeof(GameServer.GameLogonReceipt);
			GSPacketTypes[0x02] = typeof(GameServer.GameLogonSuccess);
			GSPacketTypes[0x03] = typeof(GameServer.LoadAct);
			GSPacketTypes[0x04] = typeof(GameServer.LoadDone);
			GSPacketTypes[0x05] = typeof(GameServer.UnloadDone);
			GSPacketTypes[0x06] = typeof(GameServer.GameLogoutSuccess);
			GSPacketTypes[0x07] = typeof(GameServer.MapAdd);
			GSPacketTypes[0x08] = typeof(GameServer.MapRemove);
			GSPacketTypes[0x09] = typeof(GameServer.AssignWarp);
			GSPacketTypes[0x0A] = typeof(GameServer.RemoveGroundUnit);
			GSPacketTypes[0x0B] = typeof(GameServer.GameHandshake);
			GSPacketTypes[0x0C] = typeof(GameServer.NPCGetHit);
			GSPacketTypes[0x0D] = typeof(GameServer.PlayerStop);
            GSPacketTypes[0x0E] = typeof(GameServer.SetGameObjectMode);
			GSPacketTypes[0x0F] = typeof(GameServer.PlayerMove);
            GSPacketTypes[0x10] = typeof(GameServer.PlayerMoveToTarget);
			GSPacketTypes[0x11] = typeof(GameServer.ReportKill);
			GSPacketTypes[0x15] = typeof(GameServer.PlayerReassign);
			GSPacketTypes[0x19] = typeof(GameServer.SmallGoldAdd);
			GSPacketTypes[0x1A] = typeof(GameServer.ByteToExperience);
			GSPacketTypes[0x1B] = typeof(GameServer.WordToExperience);
			GSPacketTypes[0x1C] = typeof(GameServer.DWordToExperience);
			GSPacketTypes[0x1D] = typeof(GameServer.AttributeByte);
			GSPacketTypes[0x1E] = typeof(GameServer.AttributeWord);
			GSPacketTypes[0x1F] = typeof(GameServer.AttributeDWord);
            GSPacketTypes[0x20] = typeof(GameServer.PlayerAttributeNotification);
            GSPacketTypes[0x21] = typeof(GameServer.UpdateSkill);
			GSPacketTypes[0x22] = typeof(GameServer.UpdatePlayerItemSkill);
			GSPacketTypes[0x23] = typeof(GameServer.Assignskill);
			GSPacketTypes[0x26] = typeof(GameServer.GameMessage);
            GSPacketTypes[0x27] = typeof(GameServer.NPCInfo);
            GSPacketTypes[0x28] = typeof(GameServer.UpdateQuestInfo);
            GSPacketTypes[0x29] = typeof(GameServer.UpdateGameQuestLog);
			GSPacketTypes[0x2A] = typeof(GameServer.TransactionComplete);
            GSPacketTypes[0x2C] = typeof(GameServer.PlaySound);
            GSPacketTypes[0x3E] = typeof(GameServer.UpdateItemStats);
			GSPacketTypes[0x3F] = typeof(GameServer.UseStackableItem);
			GSPacketTypes[0x42] = typeof(GameServer.PlayerClearCursor);
			GSPacketTypes[0x47] = typeof(GameServer.Relator1);
			GSPacketTypes[0x48] = typeof(GameServer.Relator2);
			GSPacketTypes[0x4C] = typeof(GameServer.UnitUseSkillOnTarget);
			GSPacketTypes[0x4D] = typeof(GameServer.UnitUseSkill);
			GSPacketTypes[0x4E] = typeof(GameServer.MercForHire);
			GSPacketTypes[0x4F] = typeof(GameServer.MercForHireListStart);
			GSPacketTypes[0x51] = typeof(GameServer.AssignGameObject);
            GSPacketTypes[0x52] = typeof(GameServer.UpdateQuestLog);
			GSPacketTypes[0x53] = typeof(GameServer.PartyRefresh);
			GSPacketTypes[0x59] = typeof(GameServer.AssignPlayer);
			GSPacketTypes[0x5A] = typeof(GameServer.InformationMessage);
			GSPacketTypes[0x5B] = typeof(GameServer.PlayerInGame);
            GSPacketTypes[0x5C] = typeof(GameServer.PlayerLeaveGame);
            GSPacketTypes[0x5D] = typeof(GameServer.QuestItemState);
			GSPacketTypes[0x60] = typeof(GameServer.PortalInfo);
			GSPacketTypes[0x63] = typeof(GameServer.OpenWaypoint);
            GSPacketTypes[0x65] = typeof(GameServer.PlayerKillCount);
			GSPacketTypes[0x67] = typeof(GameServer.NPCMove);
			GSPacketTypes[0x68] = typeof(GameServer.NPCMoveToTarget);
            GSPacketTypes[0x69] = typeof(GameServer.SetNPCMode);
			GSPacketTypes[0x6B] = typeof(GameServer.NPCAction);
			GSPacketTypes[0x6C] = typeof(GameServer.MonsterAttack);
			GSPacketTypes[0x6D] = typeof(GameServer.NPCStop);
            GSPacketTypes[0x74] = typeof(GameServer.PlayerCorpseVisible);
			GSPacketTypes[0x75] = typeof(GameServer.AboutPlayer);
            GSPacketTypes[0x76] = typeof(GameServer.PlayerInSight);
			GSPacketTypes[0x77] = typeof(GameServer.UpdateItemUI);
			GSPacketTypes[0x78] = typeof(GameServer.AcceptTrade);
			GSPacketTypes[0x79] = typeof(GameServer.GoldTrade);
			GSPacketTypes[0x7A] = typeof(GameServer.SummonAction);
			GSPacketTypes[0x7B] = typeof(GameServer.AssignSkillHotkey);
			GSPacketTypes[0x7C] = typeof(GameServer.UseSpecialItem);
            GSPacketTypes[0x7D] = typeof(GameServer.SetItemState);
			GSPacketTypes[0x7F] = typeof(GameServer.PartyMemberUpdate);
			GSPacketTypes[0x81] = typeof(GameServer.AssignMerc);
			GSPacketTypes[0x82] = typeof(GameServer.PortalOwnership);
			GSPacketTypes[0x8A] = typeof(GameServer.NPCWantsInteract);
			GSPacketTypes[0x8B] = typeof(GameServer.PlayerPartyRelationship);
			GSPacketTypes[0x8C] = typeof(GameServer.PlayerRelationship);
			GSPacketTypes[0x8D] = typeof(GameServer.AssignPlayerToParty);
			GSPacketTypes[0x8E] = typeof(GameServer.AssignPlayerCorpse);
			GSPacketTypes[0x8F] = typeof(GameServer.Pong);
			GSPacketTypes[0x90] = typeof(GameServer.PartyMemberPulse);
			GSPacketTypes[0x94] = typeof(GameServer.SkillsLog);
			GSPacketTypes[0x95] = typeof(GameServer.PlayerLifeManaChange);
			GSPacketTypes[0x96] = typeof(GameServer.WalkVerify);
            GSPacketTypes[0x97] = typeof(GameServer.SwitchWeaponSet);
            GSPacketTypes[0x99] = typeof(GameServer.ItemTriggerSkill);
			GSPacketTypes[0x9C] = typeof(GameServer.WorldItemAction);
			GSPacketTypes[0x9D] = typeof(GameServer.OwnedItemAction);
			GSPacketTypes[0x9E] = typeof(GameServer.MercAttributeByte);
			GSPacketTypes[0x9F] = typeof(GameServer.MercAttributeWord);
			GSPacketTypes[0xA0] = typeof(GameServer.MercAttributeDWord);
            GSPacketTypes[0xA1] = typeof(GameServer.MercByteToExperience);
            GSPacketTypes[0xA2] = typeof(GameServer.MercWordToExperience);
			GSPacketTypes[0xA7] = typeof(GameServer.DelayedState);
			GSPacketTypes[0xA8] = typeof(GameServer.SetState);
			GSPacketTypes[0xA9] = typeof(GameServer.EndState);
			GSPacketTypes[0xAA] = typeof(GameServer.AddUnit);
			GSPacketTypes[0xAB] = typeof(GameServer.NPCHeal);
			GSPacketTypes[0xAC] = typeof(GameServer.AssignNPC);
			GSPacketTypes[0xAE] = typeof(GameServer.WardenCheck);
			GSPacketTypes[0xAF] = typeof(GameServer.RequestLogonInfo);
			GSPacketTypes[0xB0] = typeof(GameServer.GameOver);
			
			GCPacketTypes[0x01] = typeof(GameClient.WalkToLocation);
            GCPacketTypes[0x02] = typeof(GameClient.WalkToTarget);
			GCPacketTypes[0x03] = typeof(GameClient.RunToLocation);
            GCPacketTypes[0x04] = typeof(GameClient.RunToTarget);
			GCPacketTypes[0x05] = typeof(GameClient.CastLeftSkill);
			GCPacketTypes[0x06] = typeof(GameClient.CastLeftSkillOnTarget);
			GCPacketTypes[0x07] = typeof(GameClient.CastLeftSkillOnTargetStopped);
			GCPacketTypes[0x08] = typeof(GameClient.RecastLeftSkill);
			GCPacketTypes[0x09] = typeof(GameClient.RecastLeftSkillOnTarget);
			GCPacketTypes[0x0A] = typeof(GameClient.RecastLeftSkillOnTargetStopped);
			GCPacketTypes[0x0C] = typeof(GameClient.CastRightSkill);
			GCPacketTypes[0x0D] = typeof(GameClient.CastRightSkillOnTarget);
			GCPacketTypes[0x0E] = typeof(GameClient.CastRightSkillOnTargetStopped);
			GCPacketTypes[0x0F] = typeof(GameClient.RecastRightSkill);
			GCPacketTypes[0x10] = typeof(GameClient.RecastRightSkillOnTarget);
			GCPacketTypes[0x11] = typeof(GameClient.RecastRightSkillOnTargetStopped);
			GCPacketTypes[0x13] = typeof(GameClient.UnitInteract);
            GCPacketTypes[0x14] = typeof(GameClient.SendOverheadMessage);
			GCPacketTypes[0x15] = typeof(GameClient.SendMessage);
			GCPacketTypes[0x16] = typeof(GameClient.PickItem);
			GCPacketTypes[0x17] = typeof(GameClient.DropItem);
            GCPacketTypes[0x18] = typeof(GameClient.DropItemToContainer);
            GCPacketTypes[0x19] = typeof(GameClient.PickItemFromContainer);
			GCPacketTypes[0x1A] = typeof(GameClient.EquipItem);
			GCPacketTypes[0x1D] = typeof(GameClient.SwapEquippedItem);
			GCPacketTypes[0x1C] = typeof(GameClient.UnequipItem);
			GCPacketTypes[0x1F] = typeof(GameClient.SwapContainerItem);
			GCPacketTypes[0x20] = typeof(GameClient.UseInventoryItem);
			GCPacketTypes[0x21] = typeof(GameClient.StackItems);
			GCPacketTypes[0x23] = typeof(GameClient.AddBeltItem);
			GCPacketTypes[0x24] = typeof(GameClient.RemoveBeltItem);
			GCPacketTypes[0x25] = typeof(GameClient.SwapBeltItem);
			GCPacketTypes[0x26] = typeof(GameClient.UseBeltItem);
			GCPacketTypes[0x27] = typeof(GameClient.IdentifyItem);
            GCPacketTypes[0x29] = typeof(GameClient.EmbedItem);
			GCPacketTypes[0x2A] = typeof(GameClient.ItemToCube);
			GCPacketTypes[0x2F] = typeof(GameClient.TownFolkInteract);
			GCPacketTypes[0x30] = typeof(GameClient.TownFolkCancelInteraction);
            GCPacketTypes[0x31] = typeof(GameClient.DisplayQuestMessage);
			GCPacketTypes[0x32] = typeof(GameClient.BuyItem);
			GCPacketTypes[0x33] = typeof(GameClient.SellItem);
			GCPacketTypes[0x34] = typeof(GameClient.CainIdentifyItems);
			GCPacketTypes[0x35] = typeof(GameClient.TownFolkRepair);
			GCPacketTypes[0x36] = typeof(GameClient.HireMercenary);
			GCPacketTypes[0x37] = typeof(GameClient.IdentifyGambleItem);
            GCPacketTypes[0x38] = typeof(GameClient.TownFolkMenuSelect);
			GCPacketTypes[0x3A] = typeof(GameClient.IncrementAttribute);
			GCPacketTypes[0x3B] = typeof(GameClient.IncrementSkill);
			GCPacketTypes[0x3C] = typeof(GameClient.SelectSkill);
            GCPacketTypes[0x3D] = typeof(GameClient.HoverUnit);
			GCPacketTypes[0x3F] = typeof(GameClient.SendCharacterSpeech);
			GCPacketTypes[0x40] = typeof(GameClient.RequestQuestLog);
            GCPacketTypes[0x41] = typeof(GameClient.Respawn);
			GCPacketTypes[0x49] = typeof(GameClient.WaypointInteract);
            GCPacketTypes[0x4B] = typeof(GameClient.RequestReassign);
			GCPacketTypes[0x4F] = typeof(GameClient.ClickButton);
			GCPacketTypes[0x50] = typeof(GameClient.DropGold);
            GCPacketTypes[0x51] = typeof(GameClient.SetSkillHotkey);
            GCPacketTypes[0x58] = typeof(GameClient.CloseQuest);
			GCPacketTypes[0x59] = typeof(GameClient.GoToTownFolk);
			GCPacketTypes[0x5D] = typeof(GameClient.SetPlayerRelation);
			GCPacketTypes[0x5E] = typeof(GameClient.PartyRequest);
			GCPacketTypes[0x5F] = typeof(GameClient.UpdatePosition);
			GCPacketTypes[0x60] = typeof(GameClient.SwitchWeapons);
			GCPacketTypes[0x61] = typeof(GameClient.ChangeMercEquipment);
			GCPacketTypes[0x62] = typeof(GameClient.ResurrectMerc);
            GCPacketTypes[0x63] = typeof(GameClient.InventoryItemToBelt);
            GCPacketTypes[0x66] = typeof(GameClient.WardenResponse);
			GCPacketTypes[0x68] = typeof(GameClient.GameLogonRequest);
			GCPacketTypes[0x69] = typeof(GameClient.ExitGame);
			GCPacketTypes[0x6B] = typeof(GameClient.EnterGame);
			GCPacketTypes[0x6D] = typeof(GameClient.Ping);
		}

        public const int BS_PACKET_COUNT = (int)BnetServerPacket.Invalid;
        public const int BS_PACKET_ID = (int)Origin.BattleNetServer;

        public const int BC_PACKET_COUNT = (int)BnetClientPacket.Invalid;
        public const int BC_PACKET_ID = (int)Origin.BattleNetClient;

        public const int RS_PACKET_COUNT = (int)RealmServerPacket.Invalid;
        public const int RS_PACKET_ID = (int)Origin.RealmServer;

        public const int RC_PACKET_COUNT = (int)RealmClientPacket.Invalid;
        public const int RC_PACKET_ID = (int)Origin.RealmClient;

        public const int GS_PACKET_COUNT = (int)GameServerPacket.Invalid;
        public const int GS_PACKET_ID = (int)Origin.GameServer;

        public const int GC_PACKET_COUNT = (int)GameClientPacket.Invalid;
        public const int GC_PACKET_ID = (int)Origin.GameClient;

        public static readonly Type[] BSPacketTypes = new Type[BS_PACKET_COUNT];
        public static readonly Type[] BCPacketTypes = new Type[BC_PACKET_COUNT];
        public static readonly Type[] RSPacketTypes = new Type[RS_PACKET_COUNT];
        public static readonly Type[] RCPacketTypes = new Type[RC_PACKET_COUNT];
        public static readonly Type[] GSPacketTypes = new Type[GS_PACKET_COUNT];
        public static readonly Type[] GCPacketTypes = new Type[GC_PACKET_COUNT];

		#endregion Packet Types
	}

    public class StatString
    {

        public static void ParseD2StatString(byte[] data, int index, ref int clientVersion, ref BattleNetCharacter characterType, ref int characterLevel, ref CharacterFlags characterFlags, ref int characterAct, ref CharacterTitle characterTitle)
        {
            int num2;
            clientVersion = data[index];
            int num = data[index + 13] - 1;
            if ((num < 0) || (num > 6))
            {
                characterType = BattleNetCharacter.Unknown;
            }
            else
            {
                characterType = (BattleNetCharacter)num;
                if (CharactersInfo.Genders[(int)characterType])
                {
                    characterFlags |= CharacterFlags.Female;
                }
            }
            characterLevel = data[index + 0x19];
            characterFlags |= (CharacterFlags)data[index + 0x1a];
            int num3 = (data[index + 0x1b] & 0x3e) >> 1;
            if ((characterFlags & CharacterFlags.Expansion) == CharacterFlags.Expansion)
            {
                num2 = num3 / 5;
                num3 = num3 % 5;
            }
            else
            {
                num2 = num3 / 4;
                num3 = num3 % 4;
            }
            if (num2 == 3)
            {
                characterAct = 0x29a;
            }
            else
            {
                characterAct = num3 + 1;
            }
            if ((characterFlags & CharacterFlags.Hardcore) == CharacterFlags.Hardcore)
            {
                num2 |= 4;
            }
            if ((characterFlags & CharacterFlags.Expansion) == CharacterFlags.Expansion)
            {
                num2 |= 0x20;
            }
            if ((characterFlags & CharacterFlags.Female) == CharacterFlags.Female)
            {
                num2 |= 0x100;
            }
            characterTitle = (CharacterTitle)num2;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RealmInfo
    {
        public readonly uint Unknown;
        public readonly string Name;
        public readonly string Description;
        public RealmInfo(byte[] data, int offset)
        {
            this.Unknown = BitConverter.ToUInt32(data, offset);
            this.Name = ByteConverter.GetNullString(data, offset + 4);
            this.Description = ByteConverter.GetNullString(data, (offset + 5) + this.Name.Length);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class CharacterInfo
    {
        // Fields
        public int Act;
        public BattleNetCharacter Class;
        public int ClientVersion;
        public DateTime Expires;
        public CharacterFlags Flags;
        public int Level;
        public string Name;
        public CharacterTitle Title;

        // Methods
        public override string ToString()
        {
            return StringUtils.ToFormatedInfoString(this, false, ": ", ", ");
        }
    }

}
