using System;
using System.Text;
using D2Data;
using D2Packets;
using ETUtils;
using System.Collections.Generic;
namespace GameServer
{
	/// <summary>
	/// Base class for Game Server Packet objects
	/// </summary>
	public class GSPacket : D2Packet
	{
		public readonly GameServerPacket PacketID;

		public GSPacket(byte[] data) : base(data, Origin.GameServer)
		{
			this.PacketID = (GameServerPacket) data[0];
		}
	}
	
	/// <summary>
	/// GS Packet 0x00 - Game Loading - Sent on game join after 0x01, before we know if join is sucesful...
	/// </summary>
	public class GameLoading : GSPacket
	{
		public GameLoading(byte[] data) : base(data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x01 - Game Logon Result - Acknoledgement of valid game join request (GC: 0x01). 
    /// The join can still fail after this...
    /// This packet has all it's info twice and makes my brain hurt.
    /// Not to mention it's all info we already have from Realm server...
	/// </summary>
	public class GameLogonReceipt : GSPacket
	{
        public readonly GameDifficulty Difficulty;
        public readonly bool Hardcore;
        public readonly bool Expansion;
        public readonly bool Ladder;

        /// <summary>
        /// A version of some kind?
        /// </summary>
        public readonly byte Unknown2;

		public GameLogonReceipt(byte[] data) : base(data)
		{
            this.Difficulty = (GameDifficulty)data[1];
            this.Unknown2 = data[2];
            this.Hardcore = (data[3] & 8) == 8 ? true : false;
            // Ladder, expansion:  30 00 01 01
            // Expansion, nl:      10 00 01 00
            // Classic, ladder:    20 00 00 01
            // Classic, nl:        00 00 00 00
            // Flag: 0x10 = Expansion, 0x20 = Ladder
            // 00
            this.Expansion = data[6] == 1 ? true : false;
            this.Ladder = data[7] == 1 ? true : false;
		}
	}

	/// <summary>
	/// GS Packet 0x02 - Game Logon Success - Sent after 0x00 if the game logon is sucessful.
    /// Otherwise the connection is dropped.
	/// </summary>
	public class GameLogonSuccess : GSPacket
	{
		public GameLogonSuccess(byte[] data) : base(data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x03 - Load Act - Sent on game join or act change before map and game objects data.
	/// </summary>
	public class LoadAct : GSPacket
	{
		public readonly ActLocation Act;
		public readonly AreaLevel TownArea;
		public readonly uint MapId;

		public string Unknown8 { get { return ByteConverter.ToHexString(this.Data, 8, 4); } }

		public LoadAct(byte[] data) : base(data)
		{
			this.Act = (ActLocation) data[1];
			this.MapId = BitConverter.ToUInt32(data, 2);
			this.TownArea = (AreaLevel) BitConverter.ToUInt16(data, 6);
		}
	}

	/// <summary>
	/// GS Packet 0x04 - Act Loaded - Loading screen lights up
	/// </summary>
	public class LoadDone : GSPacket
	{
		public LoadDone(byte[] data) : base(data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x05 - Unload Done - Sent before 0x03 on act change or after 0xB0 on game quit (but not if GS connection is interrupted.)
	/// </summary>
	public class UnloadDone : GSPacket
	{
		public UnloadDone(byte[] data) : base(data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x06 - Game Logout Success - Sent on game quit if the connection with Battle.net is not dropped.
	/// </summary>
	public class GameLogoutSuccess : GSPacket
	{
		public GameLogoutSuccess(byte[] data) : base(data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x07 - Map Add
	/// </summary>
	public class MapAdd : GSPacket
	{
		public readonly ushort X;
		public readonly ushort Y;
		public readonly AreaLevel Area;

		public MapAdd(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
            this.Area = (AreaLevel)data[5];
		}
	}

	/// <summary>
	/// GS Packet 0x08 - Map Remove
	/// </summary>
	public class MapRemove : GSPacket
	{
		public readonly ushort FloorX;
		public readonly ushort FloorY;
		public readonly byte FloorID;

		public MapRemove(byte[] data) : base(data)
		{
			this.FloorX = BitConverter.ToUInt16(data, 1);
			this.FloorY = BitConverter.ToUInt16(data, 3);
			this.FloorID = data[5];
		}
	}

	/// <summary>
	/// GS Packet 0x09 - Assign Warp - Notifies you that a door to another area has come into your range of view.
	/// </summary>
	// The doorside byte will by 0x04 when you get with in a screens distance from it, and the ID will be 
	// for the the door the other side. Otherwise it will be the door ID for the area you are in.
	public class AssignWarp : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint DoorwayID;
		public readonly WarpType Warp;
		public readonly ushort DoorX;
		public readonly ushort DoorY;

		public AssignWarp(byte[] data) :base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.DoorwayID = BitConverter.ToUInt32(data, 2);
			this.Warp = (WarpType) data[6];
			this.DoorX = BitConverter.ToUInt16(data, 7);
			this.DoorY = BitConverter.ToUInt16(data, 9);
		}
	}

	/// <summary>
	/// GS Packet 0x0A - Remove Ground Unit - Usually because it's no longer in proximity or was picked up.
	/// </summary>
	public class RemoveGroundUnit : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public RemoveGroundUnit(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x0B - Game Handshake - Sent as part of game join data... ID field will always be that of the recving client.
	/// </summary>
	public class GameHandshake : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public GameHandshake(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x0C - Computer Player Get Hit
	/// The server will send you this packet when ever a computer player recv's a hit/dmg with in your range of view (4 screens)
	/// The reassion for this packet is so the client knows what effect to display the monster doing when recving the damage (like blood SFX etc)
	/// </summary>
	public class NPCGetHit : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly ushort Life;
		public readonly ushort UnknownFX;

		public NPCGetHit(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.UnknownFX = BitConverter.ToUInt16(data, 6);
			this.Life = data[8];
		}
	} 

	/// <summary>
	/// GS Packet 0x0D - Player Stop - This is a very common packet when another human player does actions / moves...
	/// </summary>
	public class PlayerStop : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly ushort X;
		public readonly ushort Y;
		public readonly byte Life;
		public readonly byte Unknown1;
		public readonly byte Unknown2;

		public PlayerStop(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Unknown1 = data[6];
			this.X = BitConverter.ToUInt16(data, 7);
			this.Y = BitConverter.ToUInt16(data, 9);
			this.Unknown2 = data[11];
			this.Life = data[12];
		}
	}

	/// <summary>
	/// GS Packet 0x0E - Game Object Mode Change
	/// </summary>
	public class SetGameObjectMode : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly GameObjectMode Mode;
		public readonly bool CanChangeBack;
		public readonly byte Unknown6;

        public SetGameObjectMode(byte[] data)
            : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Unknown6 = data[6];
			this.CanChangeBack = BitConverter.ToBoolean(data, 7);
			this.Mode = (GameObjectMode) BitConverter.ToUInt32(data, 8);
		}
	}

	/// <summary>
	/// GS Packet 0x0F - Player Move - another player is moving to a new location in your range of view.
	/// </summary>
	public class PlayerMove : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly byte MovementType;	//TODO: make enum... 0x23=Run, 0x01=Walk, 0x20=knocked back ??
		public readonly ushort TargetX;
		public readonly ushort TargetY;
		public readonly ushort CurrentX;
		public readonly ushort CurrentY;
		public readonly byte Unknown12;

		public PlayerMove(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.MovementType = data[6];
			this.TargetX = BitConverter.ToUInt16(data, 7);
			this.TargetY = BitConverter.ToUInt16(data, 9);
			this.Unknown12 = data[12];
			this.CurrentX = BitConverter.ToUInt16(data, 12);
			this.CurrentY = BitConverter.ToUInt16(data, 14);
		}
	}

	/// <summary>
	/// GS Packet 0x10 - Player Move To Unit - Another human player is moving toward another unit.
	/// </summary>
    public class PlayerMoveToTarget : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly byte MovementType;	//TODO: make enum (flags ? value here is above & 1 !?!? ... 0x24=Run, 0x00=Walk
		public readonly UnitType TargetType;
		public readonly uint TargetUID;
		public readonly ushort TargetX;
		public readonly ushort TargetY;

		public PlayerMoveToTarget(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data,2);
			this.MovementType = data[6];
			this.TargetType = (UnitType) data[7];
			this.TargetUID = BitConverter.ToUInt32(data,8);
			this.TargetX = BitConverter.ToUInt16(data,12);
			this.TargetY = BitConverter.ToUInt16(data,14);
		}
	}

	/// <summary>
	/// GS Packet 0x11 - Report Kill - A player or their pet has killed something.
	/// </summary>
	// TEST: Only applies to collateral damage with multi shot attack (like chain lightning etc) ??
	// Seems it applies to Me kills too and UnitType is killer type not victim ??
	// Does it apply only to monster kills or PVP / merc death / etc ??
	public class ReportKill : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public ReportKill(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x15 - Reasign - Assigns a player and his pets at a location.
	/// Sent when out of sync, joining game, teleporting or using a warp, portal or waypoint.
	/// If reassign is false (0) then this packet should be ignored (???)
	/// </summary>
	public class PlayerReassign : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly ushort X;
		public readonly ushort Y;
		public readonly bool Reassign;

		public PlayerReassign(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.X = BitConverter.ToUInt16(data, 6);
			this.Y = BitConverter.ToUInt16(data, 8);
			this.Reassign = (data[10] == 0 ? false : true);
		}
	}

	/// <summary>
	/// GS Packet 0x19 - SmallGoldAdd - Notifies you that gold has been picked up or transfered from bank to inventory.
	/// If amount is larger than 254, a PlayerAttribute: Gold packet will be sent instead. What a useless packet ^^
	/// </summary>
	public class SmallGoldAdd : GSPacket
	{
		public readonly byte Amount;

		public SmallGoldAdd(byte[] data) : base (data)
		{
			this.Amount = data[1];
		}
	}

	/// <summary>
	/// GS Packet Wrapper - GainExperience - Wrapper for packet 1A, 1B, 1C
	/// The first packet when joining game just notifies you of your current experience, don't add it to a cached value !
	/// </summary>
	public class GainExperience : GSPacket
	{
		protected uint _Experience;
		public uint Experience { get{ return this._Experience; } }

		public GainExperience(byte[] data) : base(data)
		{
		}
	}
	/// <summary>
	/// GS Packet 0x1A - Byte To Experience
	/// </summary>
	public class ByteToExperience : GainExperience
	{
		public static readonly bool WRAPPED = true;

		public ByteToExperience(byte[] data) : base(data)
		{
			this._Experience = data[1];
		}
	}
	/// <summary>
	/// GS Packet 0x1B - Word to experience
	/// </summary>
	public class WordToExperience : GainExperience
	{
		public static readonly bool WRAPPED = true;

		public WordToExperience(byte[] data) : base(data)
		{
			this._Experience = BitConverter.ToUInt16(data,1);
		}
	}
	/// <summary>
	/// GS Packet 0x1C - Dword to Experience
	/// </summary>
	public class DWordToExperience : GainExperience
	{
		public static readonly bool WRAPPED = true;

		public DWordToExperience(byte[] data) : base(data)
		{
			//TODO: Last Byte Needs Research - maybe flagged ??
			this._Experience = BitConverter.ToUInt32(data,1);
		}
	}

	/// <summary>
	/// GS Packet Wrapper - PlayerAttribute - Wrapper for packets 0x1D, 0x1E and 0x1F
	/// </summary>
    public class AttributeNotification : GSPacket
	{
		protected StatType attribute;
		protected uint val;

		public StatType Attribute { get{ return this.attribute; } }
		public uint Value { get{ return this.val; } }
        
        public AttributeNotification(byte[] data)
            : base(data)
		{
			this.attribute = (StatType) data[1];
		}

	}
	/// <summary>
    /// GS Packet 0x1D - Attribute Byte - notifies you of base attributes with a rating no higher than a single byte.
	/// </summary>
	public class AttributeByte : AttributeNotification
	{
		public static readonly bool WRAPPED = true;

		public AttributeByte(byte[] data) : base (data)
		{
			this.val = data[2];
		}
	}
	/// <summary>
    /// GS Packet 0x1E - Attribute Word - notifies you of base attributes with a rating no higher than two bytes.
	/// </summary>
	public class AttributeWord : AttributeNotification
	{
		public static readonly bool WRAPPED = true;

		public AttributeWord(byte[] data) : base(data)
		{
			if (data[2] == 0)
				this.val = data[3];
			else
				this.val = BitConverter.ToUInt16(data, 2);
		}
	}
	/// <summary>
    /// GS Packet 0x1F - Attribute Double Word - notifies you of base attributes with a rating no higher than four bytes.
	/// </summary>
	public class AttributeDWord : AttributeNotification
	{
		//TODO: some values can be flagged ((Max)Life/Mana)... extract them.
		//From a single mana packet check, byte 2 was a c0 flag, actual mana bytes 3-4 and byte 5 null...
        // BUT! For GoldBank, all four bytes are use for value! I'll need some code that checks each att's length.
		public static readonly bool WRAPPED = true;

		protected int flags;
		public int Flags { get { return this.flags; } }

		public AttributeDWord(byte[] data) : base (data)
		{
			this.flags = data[2];
			this.val = (uint)((data[5] << 16) | (data[4] << 8) | data[3]);
		}
	}

	/// <summary>
	/// GS Packet 0x20 - Player Attribute Notification - Notifies you of something / someones stats points.
	/// </summary>
    public class PlayerAttributeNotification : GSPacket
	{
        protected uint uID;
        protected StatType attribute;
        protected StatBase stat;
		protected uint value;

        public uint UID { get { return this.uID; } }
        public StatType Attribute { get { return this.attribute; } }
        public StatBase Stat { get { return this.stat; } }
        public uint Value { get { return this.value; } }


        public PlayerAttributeNotification(byte[] data)
            : base(data)
        {
            this.uID = BitConverter.ToUInt32(data, 1);
            BaseStat stat = BaseStat.Get(data[5]);
            int num = BitConverter.ToInt32(data, 6);
            if (stat.ValShift > 0)
            {
                num = num >> stat.ValShift;
            }
            if (stat.Signed)
            {
                this.stat = new SignedStat(stat, num);
            }
            else
            {
                this.stat = new UnsignedStat(stat, (uint)num);
            }
        }
	}

	/// <summary>
	/// GS Packet 0x21 - Update Skill - Sent when assigning a stat point to a skill or equipping an item proving a new skill.
	/// Note that when equipping an item providing a new skill, the actual item's +skills bonuses will not be counted in the Bonus value 
    /// (e.g. CtA skills bonus value will be 1 less than real value.)
	/// Also this is not sent for charged skill which are handled differently or for skills you already have when equipping items.
	/// </summary>
    public class UpdateSkill : GSPacket
	{
		public readonly SkillType Skill;
		public readonly int BaseLevel;
		public readonly int Bonus;
		public readonly UnitType UnitType;
		public readonly uint UID;

        /// <summary>
        ///TODO: seems to have something to do with skill being a class skill / having a BaseLevel
        ///It's always 0 for skills provided by CtA and was 6f for an actual skill point spent on my zon...
        /// </summary>
		public string Unknown11 { get { return ByteConverter.ToHexString(this.Data, 11, 1); } }

        public UpdateSkill(byte[] data)
            : base(data)
        {
            this.UnitType = (UnitType)((byte)BitConverter.ToUInt16(data, 1));
            this.UID = BitConverter.ToUInt32(data, 3);
            this.Skill = (SkillType)BitConverter.ToUInt16(data, 7);
            this.BaseLevel = data[9];
            this.Bonus = data[10];
        }

	}
	
	/// <summary>
	/// GS Packet 0x22 - Update Player Item Skill - Updates availiability and/or quantity of an item provided skill (id/tp...)
	/// </summary>
	public class UpdatePlayerItemSkill : GSPacket
	{
		public readonly uint PlayerUID;
		public readonly SkillType Skill;
		public readonly int Quantity;

		public string Unknown1 { get { return ByteConverter.ToHexString(this.Data, 1, 2); } }
		public string Unknown10 { get { return ByteConverter.ToHexString(this.Data, 10, 2); } }

		public UpdatePlayerItemSkill(byte[] data) : base(data)
		{
			//this.Unknown1 = BitConverter.ToUInt16(data, 1);
			this.PlayerUID = BitConverter.ToUInt32(data, 3);
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 7);
			this.Quantity = data[9];
			//this.Unknown10 = BitConverter.ToUInt16(data, 10);
		}
	}

	/// <summary>
    /// GS Packet 0x23 - Player Assign Skill - Sent in response to GameClient.SelectSkill when selecting a skill.
	/// </summary>
	public class Assignskill : GSPacket
	{
		public static readonly uint NULL_UInt32 = 0xFFFFFFFF;

		public readonly SkillHand Hand;
		public readonly SkillType Skill;
		public readonly uint ChargedItemUID;
		public readonly UnitType UnitType;
		public readonly uint UID;

		public Assignskill(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Hand = (SkillHand) data[6];
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 7);
			this.ChargedItemUID = BitConverter.ToUInt32(data, 9);
		}
	}

	/// <summary>
	/// GS Packet 0x26 - Game Chat - Game message or whisper comming from in game player or player / shrine overhead message.
	/// If a shrine displays a overhead message, the message will be a 4 character number specifying the message.
	/// </summary>
	public class GameMessage : GSPacket
	{
		//TODO: make enum. 3684 = "You Feel Refreshed", 3694 = "You No Longer Fear Posion", etc.
		public static readonly int NULL_UInt32 = 0;

		public readonly GameMessageType MessageType;
		public readonly UnitType UnitType = UnitType.Player;
		public readonly uint UID;
        public readonly Int32  Random;
		public readonly string PlayerName = null;
		public readonly string Message;

		public string Unknown2 { get { return ByteConverter.ToHexString(this.Data, 2, 1); } }
		public string Unknown3_8 { get {
			if (this.MessageType == GameMessageType.OverheadMessage)
				return ByteConverter.ToHexString(this.Data, 8, 2);
			return ByteConverter.ToHexString(this.Data, 3, 6);
		} }

        public GameMessage(byte[] data)
            : base(data)
        {
            this.UnitType = UnitType.NotApplicable;
            this.Random = -1;
            this.MessageType = (GameMessageType)((ushort)BitConverter.ToInt16(data, 1));
            if (this.MessageType == GameMessageType.OverheadMessage)
            {
                this.UnitType = (UnitType)data[3];
                this.UID = BitConverter.ToUInt32(data, 4);
                this.Random = BitConverter.ToUInt16(data, 8);
                this.Message = ByteConverter.GetNullString(data, 11);
            }
            else
            {
                this.PlayerName = ByteConverter.GetNullString(data, 10);
                this.Message = ByteConverter.GetNullString(data, 11 + this.PlayerName.Length);
            }
        }
        public GameMessage(GameMessageType type, byte charFlags, string charName, string message)
            : base(Build(type, charFlags, charName, message))
        {
            this.UnitType = UnitType.NotApplicable;
            this.Random = -1;
            this.MessageType = type;
            this.PlayerName = charName;
            this.Message = message;
        }

        public GameMessage(UnitType type, uint uid, ushort random, string message)
            : base(Build(type, uid, random, message))
        {
            this.UnitType = UnitType.NotApplicable;
            this.Random = -1;
            this.MessageType = GameMessageType.OverheadMessage;
            this.UID = uid;
            this.Random = random;
            this.Message = message;
        }


        public static byte[] Build(GameMessageType type, byte charFlags, string charName, string message)
        {
            if ((charName == null) || (charName.Length == 0))
            {
                throw new ArgumentException("charName");
            }
            if ((message == null) || (message.Length == 0))
            {
                throw new ArgumentException("message");
            }
            byte[] buffer = new byte[(12 + charName.Length) + message.Length];
            buffer[0] = 0x26;
            buffer[1] = (byte)type;
            buffer[3] = 2;
            buffer[9] = charFlags;
            for (int i = 0; i < charName.Length; i++)
            {
                buffer[10 + i] = (byte)charName[i];
            }
            int num2 = 0;
            int num3 = 11 + charName.Length;
            while (num2 < message.Length)
            {
                buffer[num3 + num2] = (byte)message[num2];
                num2++;
            }
            return buffer;
        }
        
        public static byte[] Build(GameMessageType type, string charName, string message)
        {
            if ((charName == null) || (charName.Length == 0))
            {
                throw new ArgumentException("charName");
            }
            if ((message == null) || (message.Length == 0))
            {
                throw new ArgumentException("message");
            }
            byte[] buffer = new byte[(12 + charName.Length) + message.Length];
            buffer[0] = 0x26;
            buffer[1] = (byte)type;
            buffer[3] = 2;
            buffer[9] = 5;
            for (int i = 0; i < charName.Length; i++)
            {
                buffer[10 + i] = (byte)charName[i];
            }
            int num2 = 0;
            int num3 = 11 + charName.Length;
            while (num2 < message.Length)
            {
                buffer[num3 + num2] = (byte)message[num2];
                num2++;
            }
            return buffer;
        }
        

        public static byte[] Build(GameMessageType type, string message)
        {
            if ((message == null) || (message.Length == 0))
            {
                throw new ArgumentException("message");
            }
            byte[] buffer = new byte[((12 + 1) + message.Length)];
            buffer[0] = 0x26;
            buffer[1] = (byte)type;
            buffer[3] = 2;
            buffer[9] = 5;
            buffer[10] = (byte)0;
            int num2 = 0;
            int num3 = 11;

            while (num2 < message.Length)
            {
                buffer[num3 + num2] = (byte)message[num2];
                num2++;
            }

            return buffer;
        }

        public static byte[] Build(UnitType type, uint uid, ushort random, string message)
        {
            if ((message == null) || (message.Length == 0))
            {
                throw new ArgumentException("message");
            }
            byte[] buffer = new byte[12 + message.Length];
            buffer[0] = 0x26;
            buffer[1] = 5;
            buffer[3] = (byte)type;
            buffer[4] = (byte)uid;
            buffer[5] = (byte)(uid >> 8);
            buffer[6] = (byte)(uid >> 0x10);
            buffer[7] = (byte)(uid >> 0x18);
            buffer[8] = (byte)random;
            buffer[9] = (byte)(random >> 8);
            for (int i = 0; i < message.Length; i++)
            {
                buffer[11 + i] = (byte)message[i];
            }
            return buffer;
        }

	}

	/// <summary>
    /// GS Packet 0x27 - NPC Info - If UID if player UID, this contains mercenary's info.
	/// Otherwise, it's an NPC Interact, UID is that of NPC and info is null
	/// </summary>
    public class NPCInfo : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public string Unknown6 { get { return ByteConverter.ToHexString(this.Data, 6, 34); } }

		public NPCInfo(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	public enum QuestType
	{
		SpokeToWarriv			= 0,
		DenOfEvil				= 1,
		SistersBurialGrounds	= 2,
		ToolsOfTheTrade			= 3,
		TheSearchForCain		= 4,
		TheForgottenTower		= 5,
		SistersToTheSlaughter	= 6,
		CanGoToAct2				= 7,
		SpokeToJerhyn			= 8,
		RadamentsLair			= 9,
		TheHoradricStaff		= 10,
		TaintedSun			    = 11,
		ArcaneSanctuary		    = 12,
		TheSummoner				= 13,
		TheSevenTombs			= 14,
		CanGoToAct3				= 15,
		SpokeToHratli			= 16,
		LamEsensTome			= 17,
		KhalimsWill				= 18,
		BladeOfTheOldReligion	= 19,
		TheGoldenBird			= 20,
		TheBlackenedTemple		= 21,
		TheGuardian				= 22,
		CanGoToAct4				= 23,
		SpokeToTyrael			= 24,
		TheFallenAngel			= 25,
		TerrorsEnd				= 26,
		HellsForge				= 27,
        /* TODO: extra sub quests here !!!
        1A   a4q3   The Hellforge
        1B          (referred to)
                    (also items "hfh" and "dss" -- "hfh" is Hellforge Hammer,
                     but "dss" not in item tables)
        1C          (referred to in Hellforge code)
        1D          <<not used???>>
        1E   a2q7   vestigial reference???
        1F   a2q8   vestigial reference???
        20   a3q7   vestigial reference???
        21          <<not used???>>
        22          <<not used???>>
        */
		CanGoToAct5				= 28,
        // TODO: extra sub quests here !!!
		TheSecretCowLevel		= 32,
        // TODO: extra sub quests here !!!
		SeigeOnHarrogath		= 35,
		RescueOnMountArreat		= 36,
		PrisonOfIce				= 37,
        BetrayalOfHarrogath     = 38,
		RiteOfPassage			= 39,
		EveOfDestruction		= 40,
	}

	[Flags]
	public enum QuestState
	{
        Event1          = 1,
        Event2          = 2,
        Event4          = 4,
        Event8          = 8,
        Active          = 0x10,
        /// <summary>
        /// Previous quest's main goal is accomplished !?!
        /// </summary>
        Enabled         = 0x20,
        Unknown0x40     = 0x40,     //TODO:
        /// <summary>
        /// An NPC wants to give the quest, or quest was given by an NPC.
        /// </summary>
        FromNPC         = 0x80,
	}
    [Flags]
	public enum QuestStanding
	{
		Complete			    = 1,
        RewardPending           = 2,
		Started				    = 4,
        LeftTownWhileStarted    = 8,
        EnteredFirstArea        = 0x10,
        Event0x20               = 0x20,
        Event0x40               = 0x40,
        Event0x80               = 0x80,
	}

    public class QuestInfo
    {
        public readonly QuestType Type;
        public QuestState State;
        public QuestStanding Standing;

        public QuestInfo(QuestType type, QuestState state, QuestStanding standing)
        {
            this.Type = type;
            this.State = state;
            this.Standing = standing;
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}. {2}", this.Type, this.State, this.Standing);
        }
    }

    public enum QuestInfoUpdateType
    {
        NPCInteract = 1,
        QuestLog = 6
    }

	/// <summary>
    /// GS Packet 0x28 - Player Quest Info - Provides info about current quest states.
	/// Sent when entering game, opening the quest panel and when interacting with a town folk.
	/// </summary>
    public class UpdateQuestInfo : GSPacket
	{
		public static readonly uint NULL_UInt32 = 0;

        public readonly QuestInfo[] Quests;
        public readonly QuestInfoUpdateType Type;
		public readonly uint UID;

		public string Unknown88 { get { return ByteConverter.ToHexString(this.Data, 88); } }

        public UpdateQuestInfo(byte[] data)
            : base(data)
        {
            this.Type = (QuestInfoUpdateType)data[1];
            this.UID = BitConverter.ToUInt32(data, 2);
            this.Quests = new QuestInfo[0x29];
            for (int i = 0; i < 0x29; i++)
            {
                this.Quests[i] = new QuestInfo((QuestType)i, (QuestState)data[6 + (i * 2)], (QuestStanding)data[7 + (i * 2)]);
            }
        }
	}

    public enum GameQuestState
    {
        /// <summary>
        /// The creator had not completed the quest and it has not been done in this game yet.
        /// </summary>
        Open        = 0,
        /// <summary>
        /// The creator has already completed this quest.
        /// </summary>
	    Closed      = 0x8000,
        /// <summary>
        /// The quest has already been completed in game.
        /// </summary>
    	Completed   = 0x2000,
    }
    public class GameQuestInfo
    {
        public readonly QuestType Type;
        public readonly GameQuestState State;

        public GameQuestInfo(QuestType type, GameQuestState state)
        {
            this.Type = type;
            this.State = state;
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.Type, this.State);
        }
    }
	/// <summary>
    /// GS Packet 0x29 - Game Quest Log - Provides info about current quest states for the current game.
	/// Sent when entering game and interacting with a town folk.
	/// </summary>
    public class UpdateGameQuestLog : GSPacket
    {
        public readonly GameQuestInfo[] Quests;
        public string Unknown82 { get { return ByteConverter.ToHexString(this.Data, 83); } }

        public UpdateGameQuestLog(byte[] data)
            : base(data)
        {
            this.Quests = new GameQuestInfo[41];
            for (int i = 0; i < 41; i++)
                this.Quests[i] = new GameQuestInfo((QuestType)i, (GameQuestState)BitConverter.ToUInt16(data, 1 + i * 2));
        }
    }

	public enum TransactionType
	{
		Hire		= 0,
		Repair		= 1,
		Sell		= 3,
		Buy			= 4,
		ToStack	    = 5,
	}
	/// <summary>
	/// GS Packet 0x2A - Transaction Complete - Notifies that buying / selling an item or hireing a merc succeded and assigns it a new UID.
	/// </summary>
	public class TransactionComplete : GSPacket
	{
		public readonly TransactionType Type;
		public readonly uint UID;
		public readonly uint GoldLeft;

		public string Unknown2 { get { return ByteConverter.ToHexString(this.Data, 2, 5); } }

		public TransactionComplete(byte[] data) : base(data)
		{
			this.Type = (TransactionType) data[1];
			this.UID = BitConverter.ToUInt32(data, 7);
			this.GoldLeft = BitConverter.ToUInt32(data, 11);
		}
	}

	/// <summary>
	/// GS Packet 0x2C - Play Sound
	/// </summary>
	public class PlaySound : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly GameSound Sound;

        public PlaySound(byte[] data)
            : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Sound = (GameSound) BitConverter.ToUInt16(data, 6);
		}
	}
	
	/// <summary>
    /// GS Packet 0x3E - Update Container Item - Updates a container item's information (quantity).
	/// </summary>
    public class UpdateItemStats : GSPacket
	{
        public readonly int Quantity;

        public string Unknown1 { get { return ByteConverter.ToHexString(this.Data, 1, 6); } }
        public string Unknown9 { get { return ByteConverter.ToHexString(this.Data, 9); } }

		public UpdateItemStats(byte[] data) : base(data)
		{
            this.Quantity = (data[7] >> 5) + (data[8] << 3);
		}
	}
	
	/// <summary>
	/// GS Packet 0x3F - Use Stackable Item - Use an item like a scroll, tome or potion
	/// </summary>
	public class UseStackableItem : GSPacket
	{
		public readonly bool ActionClick;
		public readonly uint UID;

        /// <summary>
        /// Action Click ? FF FF : DA 00
        /// </summary>
		public string Unknown6 { get { return ByteConverter.ToHexString(this.Data, 6); } }

		public UseStackableItem(byte[] data) : base(data)
		{
			this.ActionClick = (data[1] == 0xFF ? true : false);
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}
	
	/// <summary>
	/// GS Packet 0x42 - Player Clear Cursor - Player's cursor is emptied.
    /// E.g. sold an item or give it to merc...
	/// </summary>
	public class PlayerClearCursor : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint PlayerUID;

		public PlayerClearCursor(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.PlayerUID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x47 - Relator 1 - This is sent in relation to another packet, usually to assign a UID.
    /// It's (almost?) always seen along with it's twin, 0x48.
	/// </summary>
	public class Relator1 : GSPacket
	{
		public readonly uint UID;
		public readonly ushort Unknown1;
		public readonly uint Unknown7;

		public Relator1(byte[] data) : base (data)
		{
			this.Unknown1 = BitConverter.ToUInt16(data, 1);
			this.UID = BitConverter.ToUInt32(data, 3);
			this.Unknown7 = BitConverter.ToUInt32(data, 7);
		}
	}

	/// <summary>
    /// GS Packet 0x48 - Relator 2 - This is sent in relation to another packet, usually to assign a UID.
    /// It's (almost?) always seen along with it's twin, 0x47.
	/// </summary>
	public class Relator2 : GSPacket
	{
		public readonly uint UID;
		public readonly ushort Unknown1;
		public readonly uint Unknown7;

		public Relator2(byte[] data) : base (data)
		{
			this.Unknown1 = BitConverter.ToUInt16(data, 1);
			this.UID = BitConverter.ToUInt32(data, 3);
			this.Unknown7 = BitConverter.ToUInt32(data, 7);
		}
	}

	/// <summary>
	/// GS Packet 0x4C - Unit Use Skill On Target - Another unit uses a skill on another target unit.
	/// </summary>
	public class UnitUseSkillOnTarget : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly SkillType Skill;
		public readonly uint TargetUID;

		public string Unknown8 { get { return ByteConverter.ToHexString(this.Data, 8, 2); } }
		public string Unknown14 { get { return ByteConverter.ToHexString(this.Data, 14, 2); } }

		public UnitUseSkillOnTarget(byte[] data) : base ( data )
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 6);
			// 8-9 Unknown
			this.TargetUID = BitConverter.ToUInt32(data, 10);
			// 14-15 Unknown
		}
	}

	/// <summary>
	/// GS Packet 0x4D - Unit Use Skill - Another unit uses a skill not targeted at a unit.
	/// Also a player activating a shrine (in witch case X / Y are null)
	/// </summary>
	public class UnitUseSkill : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly SkillType Skill;
		public readonly ushort X;
		public readonly ushort Y;

		public string Unknown10 { get { return ByteConverter.ToHexString(this.Data, 10, 1); } }
		public string Unknown15 { get { return ByteConverter.ToHexString(this.Data, 15, 2); } }

		public UnitUseSkill(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			if (this.UnitType == UnitType.GameObject)
				return;

			this.Skill = (SkillType) BitConverter.ToUInt32(data, 6);
			// 10 Unknown (0x09)
			this.X = BitConverter.ToUInt16(data, 11);
			this.Y = BitConverter.ToUInt16(data, 13);
			// 15-16 Unknown
		}
	}

	/// <summary>
	/// GS Packet 0x4E - Merc For Hire - Sent once for each merc availiable when interacting with a town folk selling mercs.
	/// </summary>
	public class MercForHire : GSPacket
	{
		public ushort MercID;
		public string Unknown3 { get { return ByteConverter.ToHexString(this.Data, 3, 4); } }

		public MercForHire(byte[] data) : base (data)
		{
			this.MercID = BitConverter.ToUInt16(data, 1);
		}
	}

	/// <summary>
	/// GS Packet 0x4F - Merc For Hire List Start - Sent before the 0x4E packets when interacting with a slaver town folk.
	/// </summary>
	public class MercForHireListStart : GSPacket
	{
		public MercForHireListStart(byte[] data) : base (data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x51 - Game Object Assignment - Identifies objects when they comme in range.
	/// </summary>
	public class AssignGameObject : GSPacket
	{
		public readonly GameObjectID Object;
		public readonly GameObjectInteractType Type;
		public readonly GameObjectMode Mode;
		public readonly int X;
		public readonly int Y;
		public readonly uint UID;

		public AssignGameObject(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Object = (GameObjectID) BitConverter.ToUInt16(data, 6);
			this.X = BitConverter.ToUInt16(data, 8);
			this.Y = BitConverter.ToUInt16(data, 10);
			this.Mode = (GameObjectMode) data[12];
			this.Type = (GameObjectInteractType) data[13];
		}
	}

    public class QuestLog
    {
        public readonly QuestType Type;
        public readonly int State;

        public QuestLog(QuestType type, int state)
        {
            this.Type = type;
            this.State = state;
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.Type, this.State.ToString("x2"));
        }
    }
	/// <summary>
	/// GS Packet 0x52 - Player Quest Log - Provides state of quests for the quest panel display ?!?
	/// </summary>
    public class UpdateQuestLog : GSPacket
    {
        public readonly QuestLog[] Quests;

        public UpdateQuestLog(byte[] data)
            : base(data)
        {
            this.Quests = new QuestLog[0x29];
            for (int i = 0; i < 0x29; i++)
            {
                this.Quests[i] = new QuestLog((QuestType)i, data[i + 1]);
            }
        }
    }    

	/// <summary>
	/// GS Packet 0x53 - Party Refresh - You should receive this packet about once per minute, followed by a 0x47 and a 0x48 packet for each player with in your view.
	/// The last DWORD seems to count up slowly and goes back to 00 at around 0xAA and the boolean BYTE should toggle between 0x00 and 0x80 for every packet.
	/// </summary>
	public class PartyRefresh : GSPacket
	{
		public readonly uint SlotNumber;	// UNKNOWN: Not a party ID, not sure about it being some kind of player ID...
		public readonly byte Alternator;	// Alternates between 0x00 and 0x80 every packet
		public readonly uint Count;			// Increments by 8 every packet untill 0xAA after which it restarts at 0

		public PartyRefresh(byte[] data) : base(data)
		{
			this.SlotNumber = BitConverter.ToUInt32(data, 1);
			this.Alternator = data[5];
			this.Count = BitConverter.ToUInt32(data, 4);
		}
	}

	/// <summary>
	/// GS Packet 0x59 - Player Assign - Notifies that a player is standing/coming into your range of view.
	/// X and Y will be null if player is the receiving player.
	/// </summary>
	public class AssignPlayer : GSPacket
	{
		public readonly uint UID;
		public readonly CharacterClass Class;
		public readonly string Name;
		public readonly ushort X;
		public readonly ushort Y;

		public AssignPlayer(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.Class = (CharacterClass) data[5];
			this.Name = ByteConverter.GetNullString(data, 6, 16);
			this.X = BitConverter.ToUInt16(data, 22);
			this.Y = BitConverter.ToUInt16(data, 24);
		}
	}

	public enum InformationMessageType
	{

      	/// <summary>
		/// Player Has Dropped Due To Time Out
		/// </summary>
        DroppedFromGame = 0,
        /// <summary>
		/// Player Has Joined The Game
		/// </summary>
        JoinedGame = 2,
        /// <summary>
		/// Player Has Left The Game
		/// </summary>
        LeftGame = 3,
        /// <summary>
		/// Player Is Not In The Game (answer from @wisper command)
		/// The server will send you this packet after you have used the @charname command to wisper a player that is not in the game.
		/// </summary>
        NotInGame = 4,
        /// <summary>
		/// A Player Has Been Slained
		/// </summary>
        PlayerSlain = 6,
        		/// <summary>
		/// Player To Player Relations
		/// The server will send you this packet to notify you of any changes that a D2 client would use to set up partys/hostile players/looting etc.
		/// This packet is a very important packet when it comes to party relations and should be the packet you take note of when adding party support into your bot.
		/// TEST: does this mean AboutPlayer etc are unreliable ??
		/// </summary>
        PlayerRelation = 7,
        SoJsSoldToMerchants = 0x11,
        DiabloWalksTheEarth = 0x12
	}
	/// <summary>
	/// GS Packet 0x5A - Player Infomation - Notifies of various player events and status updates.
	/// </summary>
	public class InformationMessage : GSPacket
	{
		public static readonly int NULL_UInt32 = 0;

		public readonly InformationMessageType Type;
		public readonly byte ActionType;
		public readonly uint ObjectUID;
		public readonly string SubjectName = null;
		public readonly string ObjectName = null;

		public readonly UnitType SlayerType = UnitType.NotApplicable;
		public readonly CharacterClass CharClass = CharacterClass.NotApplicable;
		public readonly GameObjectID SlayerObject = GameObjectID.NotApplicable;
		public readonly NPCCode SlayerMonster = NPCCode.NotApplicable;

        public readonly PlayerInformationActionType InformationType = PlayerInformationActionType.None;
		public readonly PlayerRelationActionType RelationType = PlayerRelationActionType.NotApplicable;

        public readonly Int32 Amount;


        public InformationMessage(byte[] data)
            : base(data)
        {
            this.SlayerType = UnitType.NotApplicable;
            this.CharClass = CharacterClass.NotApplicable;
            this.SlayerObject = (GameObjectID)0x23d;
            this.SlayerMonster = NPCCode.NotApplicable;
            this.InformationType = PlayerInformationActionType.None;
            this.RelationType = PlayerRelationActionType.NotApplicable;
            this.Amount = -1;
            this.Type = (InformationMessageType)data[1];
            this.ActionType = data[2];
            switch (this.Type)
            {
                case InformationMessageType.DroppedFromGame:
                case InformationMessageType.JoinedGame:
                case InformationMessageType.LeftGame:
                    this.SubjectName = ByteConverter.GetNullString(data, 8);
                    this.ObjectName = ByteConverter.GetNullString(data, 0x18);
                    return;

                case ((InformationMessageType)1):
                case ((InformationMessageType)5):
                    break;

                case InformationMessageType.NotInGame:
                    this.SubjectName = ByteConverter.GetNullString(data, 8);
                    return;

                case InformationMessageType.PlayerSlain:
                    this.SlayerType  = (UnitType)data[7];
                    this.SubjectName  = ByteConverter.GetNullString(data, 8);
                    if (this.SlayerType != UnitType.Player)
                    {
                        if (this.SlayerType == UnitType.NPC)
                        {
                            this.SlayerMonster = (NPCCode)BitConverter.ToUInt32(data, 3);
                            return;
                        }
                        if (this.SlayerType == UnitType.GameObject)
                        {
                            this.SlayerObject = (GameObjectID)BitConverter.ToUInt32(data, 3);
                            return;
                        }
                        break;
                    }
                    this.CharClass  = (CharacterClass)BitConverter.ToUInt32(data, 3);
                    this.ObjectName = ByteConverter.GetNullString(data, 0x18);
                    return;

                case InformationMessageType.PlayerRelation:
                    this.InformationType = (PlayerInformationActionType)this.ActionType;
                    this.ObjectUID = BitConverter.ToUInt32(data, 3);
                    this.RelationType = (PlayerRelationActionType)data[7];
                    return;

                case InformationMessageType.SoJsSoldToMerchants:
                    this.Amount = BitConverter.ToInt32(data, 3);
                    break;

                default:
                    return;
            }
        }


	}

	/// <summary>
	/// GS Packet 0x5B - Player In Game - Sent for each player in game (including you) on join.
    /// It is also sent for every player that joins afterwards, followed by a 0x5A packet of type JoinedGame.
	/// </summary>
	public class PlayerInGame : GSPacket
	{
		public readonly string Name;
		public readonly CharacterClass Class;
		public readonly ushort Level;
		public readonly short PartyID;
		public readonly uint UID;
		
		// [28+] null padding !?! 8 bytes if length is 36... 10 wasted bytes if it's always length 36... investigate.
		public string Unknown28 { get { return ByteConverter.ToHexString(this.Data, 28); } }

		public PlayerInGame(byte[] data) : base(data)
		{
			// [1-2] ushort length
			this.UID = BitConverter.ToUInt32(data, 3);
			this.Class = (CharacterClass) data[7];
			this.Name = ByteConverter.GetNullString(data, 8);
			this.Level = BitConverter.ToUInt16(data, 24);
			this.PartyID = BitConverter.ToInt16(data, 26);
		}
	}

    /// <summary>
    /// GS Packet 0x5C - When a Player leaves the game.
    /// </summary>
    public class PlayerLeaveGame : GSPacket 
    {
        public readonly uint uid;

        public PlayerLeaveGame(byte[] data)
            : base(data)
        {
            this.uid = BitConverter.ToUInt32(data, 1);
        }

        public PlayerLeaveGame(uint uid)
            : base(Build(uid))
        {
            this.uid = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x5c, ((byte)uid), ((byte)(uid >> 8)), ((byte)(uid >> 0x10)), ((byte)(uid >> 0x18)) };
        }
    }

    /// <summary>
    /// GS Packet 0X5C - Pl
    /// </summary>
    public class QuestItemState : GSPacket
    {

        public string Unknown1
        {
            get
            {
                return ByteConverter.ToHexString(base.Data , 1);
            }
        }

        public QuestItemState(byte[] data)
            : base(data)
        {
        }
    }
	[Flags]
	public enum TownPortalState
	{
		ToTown		= 0,
		Unknown		= 1,
		ToWildArea	= 2,
		Used		= 4,
	}
	/// <summary>
	/// GS Packet 0x60 - Portal Info - A portal comes into your range of view.
	/// This packet should be linked to a GS packet 0x82 to assign an owner to the portal.
	/// </summary>
	public class PortalInfo : GSPacket
	{
		public readonly AreaLevel Destination;
		public readonly TownPortalState State;
		public readonly uint UID;

		public PortalInfo(byte[] data) : base (data)
		{
			this.State = (TownPortalState) data[1];
			this.Destination = (AreaLevel) data[2];
			this.UID = BitConverter.ToUInt32(data, 3);
		}
	}
	
	/// <summary>
	/// GS Packet 0x63 - Open Waypoint - Notifies to open a waypoint's menu and which destinations are availiable.
	/// </summary>
	public class OpenWaypoint : GSPacket
	{
		public readonly uint UID;
		public readonly WaypointsAvailiable Waypoints;

		public OpenWaypoint(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.Waypoints = (WaypointsAvailiable) BitConverter.ToUInt64(data, 7);
		}
	}
	
	/// <summary>
    /// GS Packet 0x65 - Player Kill Count - Updates the player's kill count for the current game.
    /// Sent on game join with value of 0 and after each kill.
	/// </summary>
	public class PlayerKillCount : GSPacket
	{
		public readonly uint UID;
        public readonly int Count;

		public PlayerKillCount(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.Count = BitConverter.ToUInt16(data, 5);
		}
	}
    
	/// <summary>
    /// GS Packet 0x67 - NPC Move - An NPC is moving to a new location in range of view.
	/// </summary>
	public class NPCMove : GSPacket
	{
        public readonly uint UID;
        public readonly ushort X;
        public readonly ushort Y;
        public readonly byte Unknown5;

        public string Unknown10 { get { return ByteConverter.ToHexString(this.Data, 10, 2); } }

		// Found in alot of movement packets...
		public string Unknown12 { get { return ByteConverter.ToHexString(this.Data, 12, 4); } }

		public NPCMove(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
            this.Unknown5 = data[5];
			this.X = BitConverter.ToUInt16(data, 6);
            this.Y = BitConverter.ToUInt16(data, 8);
		}
	}

	/// <summary>
    /// GS Packet 0x68 - NPC Move To Target - An NPC is moving to an object in range of view.
	/// </summary>
	public class NPCMoveToTarget : GSPacket
	{
		public readonly uint UID;
		public readonly byte MovementType;
		public readonly ushort TargetX;
		public readonly ushort TargetY;
		public readonly UnitType TargetType;
		public readonly uint TargetUID;

		public string Unknown15 { get { return ByteConverter.ToHexString(this.Data, 15, 2); } }
		// Found in alot of movement packets...
		public string Unknown17 { get { return ByteConverter.ToHexString(this.Data, 17, 4); } }

		public NPCMoveToTarget(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.MovementType = data[5];
			this.TargetX = BitConverter.ToUInt16(data, 6);
			this.TargetY = BitConverter.ToUInt16(data, 8);
			this.TargetType = (UnitType) data[10];
			this.TargetUID = BitConverter.ToUInt32(data, 11);
		}
	}

	// Not the same as MonMode...
	public enum NpcStateType
	{
        Alive = 0x06,
        /// <summary>
        /// Null X and Y means the monster displays a 'in air' dying animation..
        /// </summary>
        Dying = 0x08,
	    /// <summary>
        /// Dead monsters coming into view as well.
	    /// </summary>
		Dead  = 0x09,
	}
	/// <summary>
	/// GS Packet 0x69 - Computer State Update - Assigns a NPC when it comes into view or update it's state when modified.
	/// </summary>
    public class SetNPCMode : GSPacket
	{
		public readonly uint UID;
		public readonly NpcStateType State;
		public readonly ushort X;
		public readonly ushort Y;
		public readonly byte Life;

		public string Unknown11 { get { return this.Data[11].ToString("x2"); } }

		public SetNPCMode(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.State = (NpcStateType) data[5];
			this.X = BitConverter.ToUInt16(data, 6);
			this.Y = BitConverter.ToUInt16(data, 8);
			this.Life = data[10];
		}
	}

	/// <summary>
	/// GS Packet 0x6B - NPC Action - A NPC does a action at a location.
	/// </summary>
	public class NPCAction : GSPacket
	{
		public readonly uint UID;
		public readonly ushort ActionType;
		public readonly ushort X;
		public readonly ushort Y;

		public NPCAction(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.ActionType = data[5];
			this.X = BitConverter.ToUInt16(data, 12);
			this.Y = BitConverter.ToUInt16(data, 14);
		}
		/*  A few action types
			Warriv:    0x0C = has a strech
			Charsi:    0x0C = Cast/Hit a wepon
		*/
	}

	/// <summary>
	/// GS Packet 0x6C - Monster Attack - A monster is attacking something in your range of view.
	/// </summary>
	public class MonsterAttack : GSPacket
	{
		public readonly uint UID;
		public readonly ushort AttackType;
		public readonly uint TargetUID;
		public readonly UnitType TargetType;
		public readonly ushort X;
		public readonly ushort Y;

		public MonsterAttack(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.AttackType = BitConverter.ToUInt16(data, 5);
			this.TargetUID = BitConverter.ToUInt32(data, 7);
			this.TargetType = (UnitType) data[11];
			this.X = BitConverter.ToUInt16(data, 12);
			this.Y = BitConverter.ToUInt16(data, 14);
		}
	}

	/// <summary>
	/// GS Packet 0x6D - NPC Stop - A NPC needs to be redrawn in a stand still state.
	/// </summary>
	// This is a very common packet when a computer player does any form of move, as afterwards
	// (for a split second in most case's) the computer player must be drawn in a stand still state.
	public class NPCStop : GSPacket
	{
		public readonly uint UID;
		public readonly ushort X;
		public readonly ushort Y;
		public readonly byte Life;

		public NPCStop(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.X = BitConverter.ToUInt16(data, 5);
			this.Y = BitConverter.ToUInt16(data, 7);
			this.Life = data[9];
		}
	}
    
    /// <summary>
    /// GS Packet 0x74 - Player Corpse Come into View
	/// </summary>
    public class PlayerCorpseVisible : GSPacket
    {

        public readonly  bool Assign;
        public readonly uint CorpseUID;
        public readonly uint PlayerUID;

    public PlayerCorpseVisible(byte[] data) : base(data)
    {
        this.Assign = Convert.ToBoolean(data[1]);
        this.PlayerUID = BitConverter.ToUInt32(data, 2);
        this.CorpseUID = BitConverter.ToUInt32(data, 6);
    }

    public PlayerCorpseVisible(bool assign, uint playerUID, uint corpseUID)
        : base(Build(assign, playerUID, corpseUID))
    {
        this.Assign = assign;
        this.PlayerUID = playerUID;
        this.CorpseUID = corpseUID;
    }

    public static byte[] Build(bool assign, uint playerUID, uint corpseUID)
    {
        return new byte[] { 0x74, (assign ? ((byte)1) : ((byte)0)), ((byte)playerUID), ((byte)(playerUID >> 8)), ((byte)(playerUID >> 0x10)), ((byte)(playerUID >> 0x18)), ((byte)corpseUID), ((byte)(corpseUID >> 8)), ((byte)(corpseUID >> 0x10)), ((byte)(corpseUID >> 0x18)) };
    }

    }

	/// <summary>
    /// GS Packet 0x75 - About Player - Refresh relationship information with a player.
	/// </summary>
	public class AboutPlayer : GSPacket
	{
		public readonly uint UID;
		public readonly short PartyID;
		public readonly ushort Level;
		public readonly PlayerRelationshipType Relationship;
		public readonly bool IsInMyParty;

		public string Unknown12 { get { return this.Data[12].ToString("x2"); } }

		public AboutPlayer(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.PartyID = BitConverter.ToInt16(data, 5);
			this.Level = BitConverter.ToUInt16(data, 7);
			this.Relationship = (PlayerRelationshipType) BitConverter.ToUInt16(data, 9);
			this.IsInMyParty = BitConverter.ToBoolean(data, 11);
		}
	}

	/// <summary>
    /// GS Packet 0x76 - PlayerInSight - Player comes in sight
	/// </summary>
    public class PlayerInSight : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public PlayerInSight(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x77 - Update Item UI - Notifies of item related UI changes (mostly trade related but also stash and cube).
	/// </summary>
	public class UpdateItemUI : GSPacket
	{
		public readonly ItemUIAction Type;

		public UpdateItemUI(byte[] data) : base (data)
		{
			this.Type = (ItemUIAction) data[1];
		}
	}

	/// <summary>
	/// GS Packet 0x78 - Accept Trade - Notifies you of who you are tradin with after you have accepted a trade request.
	/// </summary>
	public class AcceptTrade : GSPacket
	{
		public readonly string PlayerName;
		public readonly uint PlayerUID;

		public AcceptTrade(byte[] data) : base (data)
		{
			this.PlayerName = ByteConverter.GetNullString(data, 1, 16);
			this.PlayerUID = BitConverter.ToUInt32(data, 17);
		}
	}

	/// <summary>
	/// GS Packet 0x79 - Gold Trade - Updates the amount of gold the trading partner is offering.
	/// Will be sent with a value of 0 when entering trade and every time the the amount or trade status is changing.
	/// </summary>
	public class GoldTrade : GSPacket
	{
		public readonly bool MyGold;
		public readonly uint Amount;

		public GoldTrade(byte[] data) : base (data)
		{
			this.MyGold = BitConverter.ToBoolean(data, 1);
			this.Amount = BitConverter.ToUInt32(data, 2);
		}
	}

	public enum SummonActionType
	{
		UnsummonedOrLostSight	= 0,
		SummonedOrReasigned		= 1,
	}
	/// <summary>
	/// GS Pacekt 0x7A - Summon Action - A player (un)summons a pet.
	/// Sent even if the player is not on the same area / act as you, or in your party.
	/// If action is 0 then all fields other than the monster ID will be null.
	/// </summary>
	public class SummonAction : GSPacket
	{
		public readonly SummonActionType ActionType;
		public readonly byte SkillTree;
		public readonly ushort PetType;
		public readonly uint PlayerUID;
		public readonly uint PetUID;

		public SummonAction(byte[] data) : base (data)
		{
			this.ActionType = (SummonActionType) data[1];
			this.SkillTree = data[2];
			this.PetType = BitConverter.ToUInt16(data, 3);
			this.PlayerUID = BitConverter.ToUInt32(data, 5);
			this.PetUID = BitConverter.ToUInt32(data, 9);
		}
	}
	
	/// <summary>
	/// GS Packet 0x7B - Assign Skill Hotkey - Sent on game join to assign each skill to a hotkey slot
	/// </summary>
	public class AssignSkillHotkey : GSPacket
	{
		public static readonly uint NULL_UInt32 = 0xFFFFFFFF;

		public readonly int Slot;
		public readonly SkillType Skill;
		public readonly uint ChargedItemUID;

		public AssignSkillHotkey(byte[] data) : base (data)
		{
			this.Slot = data[1];
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 2);
			this.ChargedItemUID = BitConverter.ToUInt32(data, 4);
		}
	}

	public enum SpecialItemType
	{
		TomeOrScroll	= 4,
	}
	/// <summary>
	/// GS Packet 0x7C - Modify Item - Use a special item !?
    /// Only known when identify / portal scroll / tome is used...
	/// </summary>
	public class UseSpecialItem : GSPacket
	{
		public readonly SpecialItemType Action;
		public readonly uint UID;

		public UseSpecialItem(byte[] data) : base (data)
		{
			this.Action = (SpecialItemType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}


    public enum ItemStateType
    {
        Broken = 1,
        Full = 2
    }

    /// <summary>
    /// GS Packet 0X7D
    /// </summary>
    public class SetItemState : GSPacket
    {

        public readonly uint ItemUID;
        public readonly UnitType OwnerType;
        public readonly uint OwnerUID;
        public readonly ItemStateType State;
        public readonly ItemStateType State2;
        public readonly byte Unknown10;
        public readonly byte Unknown17 ;

        public SetItemState(byte[] data)
            : base(data)
        {
            this.OwnerType = (UnitType)data[1];
            this.OwnerUID = BitConverter.ToUInt32(data, 2);
            this.ItemUID = BitConverter.ToUInt32(data, 6);
            this.Unknown10 = data[10];
            this.State = (ItemStateType)BitConverter.ToUInt32(data, 11);
            this.State2 = (ItemStateType)BitConverter.ToUInt16(data, 15);
            this.Unknown17 = data[0x11];
        }

        public SetItemState(UnitType ownerType, uint ownerUID, uint itemUID, ItemStateType state)
            : base(Build(ownerType, ownerUID, itemUID, state))
        {
            this.OwnerType = ownerType;
            this.OwnerUID = ownerUID;
            this.ItemUID = itemUID;
            this.State = state;
            this.State2 = state;
        }

        public static byte[] Build(UnitType ownerType, uint ownerUID, uint itemUID, ItemStateType state)
        {
            byte[] buffer = new byte[0x12];
            buffer[0] = 0x7d;
            buffer[1] = (byte)ownerType;
            buffer[2] = (byte)ownerUID;
            buffer[3] = (byte)(ownerUID >> 8);
            buffer[4] = (byte)(ownerUID >> 0x10);
            buffer[5] = (byte)(ownerUID >> 0x18);
            buffer[6] = (byte)itemUID;
            buffer[7] = (byte)(itemUID >> 8);
            buffer[8] = (byte)(itemUID >> 0x10);
            buffer[9] = (byte)(itemUID >> 0x18);
            buffer[11] = (byte)state;
            buffer[12] = (byte)(((int)state) >> 8);
            buffer[13] = (byte)(((int)state) >> 0x10);
            buffer[14] = (byte)(((int)state) >> 0x18);
            buffer[15] = (byte)state;
            buffer[0x10] = (byte)(((ushort)state) >> 8);
            return buffer;
        }

 






    }

	/// <summary>
	/// GS Packet 0x7F - Party Member Update - Updates a party member's information
	/// </summary>
	public class PartyMemberUpdate : GSPacket
	{
        /// <summary>
        /// TEST: 1 = Player, 0 = Pet ??
        /// </summary>
        public readonly byte MemberType;
		public readonly ushort LifePercent;
		public readonly AreaLevel Area;
		public readonly uint UID;

		public PartyMemberUpdate(byte[] data) : base (data)
		{
			this.MemberType = data[1];
			this.LifePercent = BitConverter.ToUInt16(data, 2);
			this.UID = BitConverter.ToUInt32(data, 4);
			this.Area = (AreaLevel) BitConverter.ToUInt16(data, 8);
		}
	}

	/// <summary>
	/// GS Packet 0x81 - Merc Assignment - Assigns a mercenary when hired or resurrected (also sent for party member's merc).
	/// </summary>
	public class AssignMerc : GSPacket
	{
		public readonly NPCCode ID;
		public readonly uint OwnerUID;
		public readonly uint UID;

		public string Unknown1 { get { return ByteConverter.ToHexString(this.Data, 1, 1); } }
		public string Unknown5 { get { return ByteConverter.ToHexString(this.Data, 12, 8); } }

		public AssignMerc(byte[] data) : base (data)
		{
			this.ID = (NPCCode) BitConverter.ToUInt16(data, 2);
			this.OwnerUID = BitConverter.ToUInt32(data, 4);
			this.UID = BitConverter.ToUInt32(data, 8);
		}
	}

	/// <summary>
	/// GS Packet 0x82 - PortalOwnership - Notifies you of a portal's ownership infomation when it comes into your range of view.
	/// </summary>
	public class PortalOwnership : GSPacket
	{
		public readonly uint OwnerUID;
		public readonly string OwnerName;
		public readonly uint PortalLocalUID;
		public readonly uint PortalRemoteUID;

		public PortalOwnership(byte[] data) : base (data)
		{
			this.OwnerUID = BitConverter.ToUInt32(data, 1);
			this.OwnerName = ByteConverter.GetNullString(data, 5, 16);
			this.PortalLocalUID = BitConverter.ToUInt32(data, 21);
			this.PortalRemoteUID = BitConverter.ToUInt32(data, 25);
		}
	}

	/// <summary>
	/// GS Packet 0x8A - NPC Wants Interact - Town folk wants to interact with player, normally for quests.
	/// </summary>
	public class NPCWantsInteract : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public NPCWantsInteract(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0x8B - Player Party Relationship - Update the party specific relationship flags (in a party, invites...)
	/// This is always relative to you unlike regulagar flags (0x8C) which you get for all players.
	/// </summary>
	public class PlayerPartyRelationship : GSPacket
	{
		public readonly uint UID;
		public readonly PartyRelationshipType Relationship;

		public PlayerPartyRelationship(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data,1);
			this.Relationship = (PartyRelationshipType) data[5];
		}
	}

	/// <summary>
	/// GS Packet 0x8C - Player Relationship Notification - Update the relationship flags (squelch, etc) between two players.
	/// This packet will cover all players in the game, including your own character.
	/// If the relationship is changed by more than one player a packet will be sent for each player. 
	/// E.g. turning hostile on someone will generaly make him hostile to you automatically...
	/// </summary>
	public class PlayerRelationship : GSPacket
	{
		public readonly uint SubjectUID;
		public readonly uint ObjectUID;
		public readonly PlayerRelationshipType Relations;

		public PlayerRelationship(byte[] data) : base (data)
		{
			this.SubjectUID = BitConverter.ToUInt32(data, 1);
			this.ObjectUID = BitConverter.ToUInt32(data, 5);
			this.Relations = (PlayerRelationshipType) BitConverter.ToUInt16(data, 9);
		}
	}

	/// <summary>
	/// GS Packet 0x8D - Assign Player To Party - Notifies you that a player is now in a party.
	/// </summary>
	public class AssignPlayerToParty : GSPacket
	{
		public readonly uint UID;
		public readonly int PartyNumber;

		public AssignPlayerToParty(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.PartyNumber = BitConverter.ToInt16(data, 5);
		}
	}

	/// <summary>
	/// GS Packet 0x8E - Corpse Assign - Assigns a corpse UID to a slain player or remove a picked up corpse.
	/// </summary>
	public class AssignPlayerCorpse : GSPacket
	{
		public readonly bool Assign;
		public readonly uint PlayerUID;
		public readonly uint CorpseUID;

		public AssignPlayerCorpse(byte[] data) : base (data)
		{
			this.Assign = Convert.ToBoolean(data[1]);
			this.PlayerUID = BitConverter.ToUInt32(data, 2);
			this.CorpseUID = BitConverter.ToUInt32(data, 6);
		}

        public AssignPlayerCorpse(bool assign, uint playerUID, uint corpseUID)
            : base(Build(assign, playerUID, corpseUID))
        {
            this.Assign = assign;
            this.PlayerUID = playerUID;
            this.CorpseUID = corpseUID;
        }

        public static byte[] Build(bool assign, uint playerUID, uint corpseUID)
        {
            return new byte[] { 0x8e, (assign ? ((byte)1) : ((byte)0)), ((byte)playerUID), ((byte)(playerUID >> 8)), ((byte)(playerUID >> 0x10)), ((byte)(playerUID >> 0x18)), ((byte)corpseUID), ((byte)(corpseUID >> 8)), ((byte)(corpseUID >> 0x10)), ((byte)(corpseUID >> 0x18)) };
        }
	}

	/// <summary>
	/// GS Packet 0x8F - Pong - Response to GC: 0x6D; data is null.
	/// </summary>
	public class Pong : GSPacket
	{
		public Pong(byte[] data) : base (data)
		{
		}
	}

	/// <summary>
	/// GS Packet 0x90 - Party Pulse - You should receive this packet every second for each party member not within range.
	/// </summary>
	public class PartyMemberPulse : GSPacket
	{
		public readonly uint UID;
		public readonly uint X;
		public readonly uint Y;

		public PartyMemberPulse(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.X = BitConverter.ToUInt32(data, 5);
			this.Y = BitConverter.ToUInt32(data, 9);
		}
	}

	/// <summary>
	/// GS Packet 0x94 - Skills Log - Notifies you of your current (on game join) or upgraded skills and their base level.
	/// </summary>
	public class SkillsLog : GSPacket
	{
		public uint UID;
		public readonly BaseSkillLevel[] Skills;

		public SkillsLog(byte[] data) : base (data)
		{
			this.Skills = new BaseSkillLevel[data[1]];
			this.UID = BitConverter.ToUInt32(data, 2);
			for (int i=0; i < data[1]; i++)
				this.Skills[i] = new BaseSkillLevel(BitConverter.ToUInt16(data, 6 + i * 3), data[8 + i * 3]);
		}
	}

	[Flags]
	public enum LifeManaFlags
	{
		None = 0,
		Life0x8000 = 1,
		Mana0x8000 = 2,
		Mana0x4000 = 4,
	}
	/// <summary>
	/// GS Packet 0x95 - player health mana changed
	/// </summary>
	public class PlayerLifeManaChange : GSPacket
	{
		public readonly ushort Life;
		public readonly ushort Mana;
		public readonly LifeManaFlags Flags = 0;

		public string Unknown5 { get { return ByteConverter.ToHexString(this.Data, 5, 8); } }

		public PlayerLifeManaChange(byte[] data) : base(data)
		{
			this.Life = BitConverter.ToUInt16(data, 1);
			this.Mana = (ushort) (2 * BitConverter.ToUInt16(data, 3));
			if ((this.Life & 0x8000) == 0x8000)
			{
				this.Life ^= 0x8000;
				this.Flags |= LifeManaFlags.Life0x8000;
			}
			if ((this.Mana & 0x8000) == 0x8000)
			{
				this.Mana ^= 0x8000;
				this.Flags |= LifeManaFlags.Mana0x8000;
			}
			if ((this.Mana & 0x4000) == 0x4000)
			{
				this.Mana ^= 0x4000;
				this.Flags |= LifeManaFlags.Mana0x4000;
			}
		}
	}

	[Flags]
	public enum WalkVerifyFlags
	{
		None			= 0,
		Stamina0x8000	= 1,
		X0x8000			= 2,
		Y0x8000			= 4,
	}
	/// <summary>
	/// GS Packet 0x96 - Walk Verify - Used to update your stamina and position to sync up with during run / walk
	/// </summary>
	public class WalkVerify : GSPacket
	{
		public readonly int X;
		public readonly int Y;
		/// <summary>
		/// Some kind of state or count... if 0, the player is done moving; otherwise expect another 0x96 shortly
		/// </summary>
		public readonly ushort State;
		public readonly int Stamina;
		public readonly WalkVerifyFlags Flags;

		public WalkVerify(byte[] data) : base (data)
		{
			this.Stamina = BitConverter.ToUInt16(data, 1);
			if ((this.Stamina & 0x8000) == 0x8000)
			{
				this.Stamina ^= 0x8000;
				this.Flags |= WalkVerifyFlags.Stamina0x8000;
			}
			
			this.X = BitConverter.ToUInt16(data, 3);
			if ((this.X & 0x8000) == 0x8000)
			{
				this.X ^= 0x8000;
				this.Flags |= WalkVerifyFlags.X0x8000;
			}
			this.X = this.X * 2;
			
			this.Y = BitConverter.ToUInt16(data, 5);
			if ((this.Y & 0x8000) == 0x8000)
			{
				this.Y ^= 0x8000;
				this.Flags |= WalkVerifyFlags.Y0x8000;
			}
			this.Y = this.Y * 2;
			
			this.State = BitConverter.ToUInt16(data, 7);
		}
	}

    public class SwitchWeaponSet : GSPacket
    {
        public SwitchWeaponSet()
            : base(Build())
        {
        }

        public SwitchWeaponSet(byte[] data)
            : base(data)
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x97 };
        }
    }

    public enum ItemEventCause
    {
        Target,
        Owner
    }
    public class ItemTriggerSkill : GSPacket
    {
        public readonly ItemEventCause cause;
        public readonly byte level;
        public readonly UnitType ownerType;
        public readonly uint ownerUID;
        public readonly SkillType skill;
        public readonly UnitType targetType;
        public readonly uint targetUID;


        public ItemTriggerSkill(byte[] data)
            : base(data)
        {
            this.ownerType = (UnitType)data[1];
            this.ownerUID = BitConverter.ToUInt32(data, 2);
            this.skill = (SkillType)BitConverter.ToUInt16(data, 6);
            this.level = data[8];
            this.targetType = (UnitType)data[9];
            this.targetUID = BitConverter.ToUInt32(data, 10);
            this.cause = (ItemEventCause)BitConverter.ToUInt16(data, 14);
        }
    }


	/// <summary>
	/// GS Packet Wrapper - Item Action - Wrapper for WorldItemAction and OwnedItemAction
	/// </summary>
	public class ItemAction : GSPacket
	{
		#region Fields

        protected ItemActionType action;
        protected BaseItem baseItem;
        protected ItemCategory category;
        protected CharacterClass charClass;
        protected int color;
        protected ItemContainer container;
        protected ItemDestination destination;
        protected ItemFlags flags;
        protected int graphic;
        protected int level;
        protected EquipmentLocation location;
        protected List<MagicPrefixType> magicPrefixes;
        protected List<MagicSuffixType> magicSuffixes;
        protected List<StatBase> mods;
        protected string name;
        protected ItemAffix prefix;
        protected ItemQuality quality;
        protected BaseRuneword runeword;
        protected int runewordID;
        protected int runewordParam;
        protected List<StatBase>[] setBonuses;
        protected BaseSetItem setItem;
        protected List<StatBase> stats;
        protected ItemAffix suffix;
        protected SuperiorItemType superiorType;
        protected uint uid;
        protected BaseUniqueItem uniqueItem;
        protected int unknown1;
        protected int use;
        protected int usedSockets;
        protected ItemVersion version;
        protected int x;
        protected int y;


		#endregion Fields

		#region Properties

        public ItemActionType Action { get { return this.action;}  }
        public BaseItem BaseItem { get { return this.baseItem ;} }
        public ItemCategory Category { get { return this.category; } }
        public CharacterClass Class { get { return this.charClass; } }
        public int Color { get { return this.color; } }
        public ItemContainer Container { get { return this.container; } }
        public ItemDestination Destination { get { return this.destination; } }
        public ItemFlags Flags { get { return this.flags; } }
        public int Graphic { get { return this.graphic; } }
        public int Level { get { return this.level; } }
        public EquipmentLocation Location { get { return this.location; } }
        public List<MagicPrefixType> MagicPrefixes { get { return this.magicPrefixes; } }
        public List<MagicSuffixType> MagicSuffixes { get { return this.magicSuffixes; } }
        public List<StatBase> Mods { get { return this.mods; } }
        public string Name { get { return this.name; } }
        public ItemAffix Prefix { get { return this.prefix; } }
        public ItemQuality Quality { get { return this.quality; } }
        public BaseRuneword Runeword { get { return this.runeword; } }
        public int RunewordID { get { return this.runewordID; } }
        public int RunewordParam { get { return this.runewordParam; } }
        public List<StatBase>[] SetBonuses { get { return this.setBonuses; } }
        public BaseSetItem SetItem { get { return this.setItem; } }

        public List<StatBase> Stats { get { return this.stats; } }
        public ItemAffix Suffix { get { return this.suffix; } }
        public SuperiorItemType SuperiorType { get { return this.superiorType; } }
        public uint UID { get { return this.uid; } }
        public BaseUniqueItem UniqueItem { get { return this.uniqueItem; } }
        public int Unknown1 { get { return this.unknown1; } }
        public int Use { get { return this.use; } }
        public int UsedSockets { get { return this.usedSockets; } }
        public ItemVersion Version { get { return this.version; } }
        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }


		#endregion Properties

        public ItemAction(byte[] data)
            : base(data)
        {
            this.superiorType = SuperiorItemType.NotApplicable;
            this.charClass = CharacterClass.NotApplicable;
            this.level = -1;
            this.usedSockets = -1;
            this.use = -1;
            this.graphic = -1;
            this.color = -1;
            this.stats = new List<StatBase>();
            this.unknown1 = -1;
            this.runewordID = -1;
            this.runewordParam = -1;
            BitReader br = new BitReader(data, 1);
            this.action = (ItemActionType)br.ReadByte();
            br.SkipBytes(1);
            this.category = (ItemCategory)br.ReadByte();
            this.uid = br.ReadUInt32();
            if (data[0] == 0x9d)
            {
                br.SkipBytes(5);
            }
            this.flags = (ItemFlags)br.ReadUInt32();
            this.version = (ItemVersion)br.ReadByte();
            this.unknown1 = br.ReadByte(2);
            this.destination = (ItemDestination)br.ReadByte(3);
            if (this.destination == ItemDestination.Ground)
            {
                this.x = br.ReadUInt16();
                this.y = br.ReadUInt16();
            }
            else
            {
                this.location = (EquipmentLocation)br.ReadByte(4);
                this.x = br.ReadByte(4);
                this.y = br.ReadByte(3);
                this.container = (ItemContainer)br.ReadByte(4);
            }
            if ((this.action == ItemActionType.AddToShop) || (this.action == ItemActionType.RemoveFromShop))
            {
                int num = ((int)this.container) | 0x80;
                if ((num & 1) == 1)
                {
                    num--;
                    this.y += 8;
                }
                this.container = (ItemContainer)num;
            }
            else if (this.container == ItemContainer.Unspecified)
            {
                if (this.location == EquipmentLocation.NotApplicable)
                {
                    if ((this.Flags & ItemFlags.InSocket) == ItemFlags.InSocket)
                    {
                        this.container = ItemContainer.Item;
                        this.y = -1;
                    }
                    else if ((this.action == ItemActionType.PutInBelt) || (this.action == ItemActionType.RemoveFromBelt))
                    {
                        this.container = ItemContainer.Belt;
                        this.y = this.x / 4;
                        this.x = this.x % 4;
                    }
                }
                else
                {
                    this.x = -1;
                    this.y = -1;
                }
            }
            if ((this.flags & ItemFlags.Ear) == ItemFlags.Ear)
            {
                this.charClass = (CharacterClass)br.ReadByte(3);
                this.level = br.ReadByte(7);
                this.name = br.ReadString(7, '\0', 0x10);
                this.baseItem = BaseItem.Get(ItemType.Ear);
            }
            else
            {
                this.baseItem = BaseItem.GetByID(this.category, br.ReadUInt32());
                if (this.baseItem.Type == ItemType.Gold)
                {
                    this.stats.Add(new SignedStat(BaseStat.Get(StatType.Quantity), br.ReadInt32(br.ReadBoolean(1) ? 0x20 : 12)));
                }
                else
                {
                    this.usedSockets = br.ReadByte(3);
                    if ((this.flags & (ItemFlags.Compact | ItemFlags.Gamble)) == ItemFlags.None)
                    {
                        BaseStat stat;
                        int num2;
                        this.level = br.ReadByte(7);
                        this.quality = (ItemQuality)br.ReadByte(4);
                        if (br.ReadBoolean(1))
                        {
                            this.graphic = br.ReadByte(3);
                        }
                        if (br.ReadBoolean(1))
                        {
                            this.color = br.ReadInt32(11);
                        }
                        if ((this.flags & ItemFlags.Identified) == ItemFlags.Identified)
                        {
                            switch (this.quality)
                            {
                                case ItemQuality.Inferior:
                                    this.prefix = new ItemAffix(ItemAffixType.InferiorPrefix, br.ReadByte(3));
                                    break;

                                case ItemQuality.Superior:
                                    this.prefix = new ItemAffix(ItemAffixType.SuperiorPrefix, 0);
                                    this.superiorType = (SuperiorItemType)br.ReadByte(3);
                                    break;

                                case ItemQuality.Magic:
                                    this.prefix = new ItemAffix(ItemAffixType.MagicPrefix, br.ReadUInt16(11));
                                    this.suffix = new ItemAffix(ItemAffixType.MagicSuffix, br.ReadUInt16(11));
                                    break;

                                case ItemQuality.Set:
                                    this.setItem = BaseSetItem.Get(br.ReadUInt16(12));
                                    break;

                                case ItemQuality.Rare:
                                case ItemQuality.Crafted:
                                    this.prefix = new ItemAffix(ItemAffixType.RarePrefix, br.ReadByte(8));
                                    this.suffix = new ItemAffix(ItemAffixType.RareSuffix, br.ReadByte(8));
                                    break;

                                case ItemQuality.Unique:
                                    if (this.baseItem.Code != "std")
                                    {
                                        this.uniqueItem = BaseUniqueItem.Get(br.ReadUInt16(12));
                                    }
                                    break;
                            }
                        }
                        if ((this.quality == ItemQuality.Rare) || (this.quality == ItemQuality.Crafted))
                        {
                            this.magicPrefixes = new List<MagicPrefixType>();
                            this.magicSuffixes = new List<MagicSuffixType>();
                            for (int i = 0; i < 3; i++)
                            {
                                if (br.ReadBoolean(1))
                                {
                                    this.magicPrefixes.Add((MagicPrefixType)br.ReadUInt16(11));
                                }
                                if (br.ReadBoolean(1))
                                {
                                    this.magicSuffixes.Add((MagicSuffixType)br.ReadUInt16(11));
                                }
                            }
                        }
                        if ((this.Flags & ItemFlags.Runeword) == ItemFlags.Runeword)
                        {
                            this.runewordID = br.ReadUInt16(12);
                            this.runewordParam = br.ReadUInt16(4);
                            num2 = -1;
                            if (this.runewordParam == 5)
                            {
                                num2 = this.runewordID - (this.runewordParam * 5);
                                if (num2 < 100)
                                {
                                    num2--;
                                }
                            }
                            else if (this.runewordParam == 2)
                            {
                                num2 = ((this.runewordID & 0x3ff) >> 5) + 2;
                            }

                            //br.set_ByteOffset(br.get_ByteOffset() - 2);
                            br.ByteOffset = br.ByteOffset - 2;

                            this.runewordParam = br.ReadUInt16();
                            this.runewordID = num2;
                            if (num2 == -1)
                            {
                                throw new Exception("Unknown Runeword: " + this.runewordParam);
                            }
                            this.runeword = BaseRuneword.Get(num2);
                        }
                        if ((this.Flags & ItemFlags.Personalized) == ItemFlags.Personalized)
                        {
                            this.name = br.ReadString(7, '\0', 0x10);
                        }
                        if (this.baseItem is BaseArmor)
                        {
                            stat = BaseStat.Get(StatType.ArmorClass);
                            this.stats.Add(new SignedStat(stat, br.ReadInt32(stat.SaveBits) - stat.SaveAdd));
                        }
                        if ((this.baseItem is BaseArmor) || (this.baseItem is BaseWeapon))
                        {
                            stat = BaseStat.Get(StatType.MaxDurability);
                            num2 = br.ReadInt32(stat.SaveBits);
                            this.stats.Add(new SignedStat(stat, num2));
                            if (num2 > 0)
                            {
                                stat = BaseStat.Get(StatType.Durability);
                                this.stats.Add(new SignedStat(stat, br.ReadInt32(stat.SaveBits)));
                            }
                        }
                        if ((this.Flags & (ItemFlags.None | ItemFlags.Socketed)) == (ItemFlags.None | ItemFlags.Socketed))
                        {
                            stat = BaseStat.Get(StatType.Sockets);
                            this.stats.Add(new SignedStat(stat, br.ReadInt32(stat.SaveBits)));
                        }
                        if (this.baseItem.Stackable)
                        {
                            if (this.baseItem.Useable)
                            {
                                this.use = br.ReadByte(5);
                            }
                            this.stats.Add(new SignedStat(BaseStat.Get(StatType.Quantity), br.ReadInt32(9)));
                        }
                        if ((this.Flags & ItemFlags.Identified) == ItemFlags.Identified)
                        {
                            StatBase base2;
                            int num4 = (this.Quality == ItemQuality.Set) ? br.ReadByte(5) : -1;
                            this.mods = new List<StatBase>();
                            while ((base2 = ReadStat(br)) != null)
                            {
                                this.mods.Add(base2);
                            }
                            if ((this.flags & ItemFlags.Runeword) == ItemFlags.Runeword)
                            {
                                while ((base2 = ReadStat(br)) != null)
                                {
                                    this.mods.Add(base2);
                                }
                            }
                            if (num4 > 0)
                            {
                                this.setBonuses = new List<StatBase>[5];
                                for (int j = 0; j < 5; j++)
                                {
                                    if ((num4 & (((int)1) << j)) != 0)
                                    {
                                        this.setBonuses[j] = new List<StatBase>();
                                        while ((base2 = ReadStat(br)) != null)
                                        {
                                            this.setBonuses[j].Add(base2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static StatBase ReadStat(BitReader br)
        {
            int index = br.ReadInt32(9);
            if (index == 0x1ff)
            {
                return null;
            }
            BaseStat stat = BaseStat.Get(index);
            if (stat.SaveParamBits == -1)
            {
                if (stat.OpBase == StatType.Level)
                {
                    return new PerLevelStat(stat, br.ReadInt32(stat.SaveBits));
                }
                switch (stat.Type)
                {
                    case StatType.MaxDamagePercent:
                    case StatType.MinDamagePercent:
                        return new DamageRangeStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(stat.SaveBits));

                    case StatType.FireMinDamage:
                    case StatType.LightMinDamage:
                    case StatType.MagicMinDamage:
                        return new DamageRangeStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.Get((int)(stat.Index + 1)).SaveBits));

                    case StatType.FireMaxDamage:
                    case StatType.LightMaxDamage:
                    case StatType.MagicMaxDamage:
                        goto Label_0350;

                    case StatType.ColdMinDamage:
                        return new ColdDamageStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.Get(StatType.ColdMaxDamage).SaveBits), br.ReadInt32(BaseStat.Get(StatType.ColdLength).SaveBits));

                    case StatType.ReplenishDurability:
                    case StatType.ReplenishQuantity:
                        return new ReplenishStat(stat, br.ReadInt32(stat.SaveBits));

                    case StatType.PoisonMinDamage:
                        return new PoisonDamageStat(stat, br.ReadInt32(stat.SaveBits), br.ReadInt32(BaseStat.Get(StatType.PoisonMaxDamage).SaveBits), br.ReadInt32(BaseStat.Get(StatType.PoisonLength).SaveBits));
                }
            }
            else
            {
                switch (stat.Type)
                {
                    case StatType.SingleSkill:
                    case StatType.NonClassSkill:
                        return new SkillBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits));

                    case StatType.ElementalSkillBonus:
                        return new ElementalSkillsBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits));

                    case StatType.ClassSkillsBonus:
                        return new ClassSkillsBonusStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits));

                    case StatType.Aura:
                        return new AuraStat(stat, br.ReadInt32(stat.SaveParamBits), br.ReadInt32(stat.SaveBits));

                    case StatType.Reanimate:
                        return new ReanimateStat(stat, br.ReadUInt32(stat.SaveParamBits), br.ReadUInt32(stat.SaveBits));

                    case StatType.SkillOnAttack:
                    case StatType.SkillOnKill:
                    case StatType.SkillOnDeath:
                    case StatType.SkillOnStriking:
                    case StatType.SkillOnLevelUp:
                    case StatType.SkillOnGetHit:
                        return new SkillOnEventStat(stat, br.ReadInt32(6), br.ReadInt32(10), br.ReadInt32(stat.SaveBits));

                    case StatType.ChargedSkill:
                        return new ChargedSkillStat(stat, br.ReadInt32(6), br.ReadInt32(10), br.ReadInt32(8), br.ReadInt32(8));

                    case StatType.SkillTabBonus:
                        return new SkillTabBonusStat(stat, br.ReadInt32(3), br.ReadInt32(3), br.ReadInt32(10), br.ReadInt32(stat.SaveBits));
                }
                throw new Exception("Invalid stat: " + stat.Index.ToString());
            }
        Label_0350:
            if (stat.Signed)
            {
                int num2 = br.ReadInt32(stat.SaveBits);
                if (stat.SaveAdd > 0)
                {
                    num2 -= stat.SaveAdd;
                }
                return new SignedStat(stat, num2);
            }
            uint val = br.ReadUInt32(stat.SaveBits);
            if (stat.SaveAdd > 0)
            {
                val -= (uint)stat.SaveAdd;
            }
            return new UnsignedStat(stat, val);
        }

	}

	/// <summary>
	/// GS Packet 0x9C - World Item Action
	/// </summary>
	public class WorldItemAction : ItemAction
	{
		public static readonly bool WRAPPED = true;
		public static readonly int NULL_Int32 = -1;

		public WorldItemAction(byte[] data) : base(data)
		{
		}
	}
	/// <summary>
	/// GS Packet 0x9D - Owned Item Action
	/// </summary>
	public class OwnedItemAction : ItemAction
	{
		public static readonly bool WRAPPED = true;
		public static readonly int NULL_Int32 = -1;

		public readonly UnitType OwnerType;
		public readonly uint OwnerUID;

		public OwnedItemAction(byte[] data) : base(data)
		{
			this.OwnerType = (UnitType) data[8];
			this.OwnerUID = BitConverter.ToUInt32(data, 9);
		}
	}

	/// <summary>
	/// GS Packet Wrapper - Merc Attribute - Wrapper for packets 0x9E, 0x9F and 0xA0
    /// TEST: also used for pets !?!
	/// </summary>
	public class MercAttribute : GSPacket
	{
		protected StatType attribute;
		protected uint uid;
		protected uint val;

		public StatType Attribute { get{ return this.attribute; } }
		public uint Value { get{ return this.val; } }
		public uint UID { get{ return this.uid; } }

		public MercAttribute(byte[] data) : base(data)
		{
			this.attribute = (StatType) data[1];
			this.uid = BitConverter.ToUInt32(data, 2);
		}
	}
	/// <summary>
	/// GS Packet 0x9E - Merc Attribute Byte - Notifies of a mercenary's single base attribute as a single byte.
	/// </summary>
	public class MercAttributeByte : MercAttribute
	{
		public static readonly bool WRAPPED = true;

		public MercAttributeByte(byte[] data) : base (data)
		{
			this.val = data[6];
		}
	}
	/// <summary>
    /// GS Packet 0x9F - Merc Attribute Word - Notifies of a mercenary's single base attribute as a word.
	/// </summary>
	public class MercAttributeWord : MercAttribute
	{
		public static readonly bool WRAPPED = true;

		public MercAttributeWord(byte[] data) : base(data)
		{
			if (data[6] == 0)
				this.val = data[7];
			else
				this.val = BitConverter.ToUInt16(data, 6);
		}
	}
	/// <summary>
    /// GS Packet 0xA0 - Merc Attribute DWord - Notifies of a mercenary's single base attribute as a DWord.
	/// </summary>
	public class MercAttributeDWord : MercAttribute
	{
		public static readonly bool WRAPPED = true;

		public MercAttributeDWord(byte[] data) : base (data)
        {
            //TODO: some values can be flagged ((Max)Life/Mana)... extract them.
            //From a single mana packet check, byte 2 was a c0 flag, actual mana bytes 3-4 and byte 5 null...
			this.val = BitConverter.ToUInt32(data, 6);
		}
	}

    /// <summary>
    /// GS Packet Wrapper - GainExperience - Wrapper for packet 1A, 1B, 1C
    /// The first packet when joining game just notifies you of your current experience, don't add it to a cached value !
    /// </summary>
    public class MercGainExperience : GSPacket
    {
        protected uint _Experience;

        public uint Experience { get { return this._Experience; } }

        public MercGainExperience(byte[] data)
            : base(data)
        {

        }
    }
    /// <summary>
    /// GS Packet 0xA1 - Byte To Experience
    /// </summary>
    public class MercByteToExperience : MercGainExperience
    {
        public static readonly bool WRAPPED = true;

        public MercByteToExperience(byte[] data)
            : base(data)
        {
            base._Experience  = data[6];
        }
    }
    /// <summary>
    /// GS Packet 0xA2 - Word to experience
    /// </summary>
    public class MercWordToExperience : MercGainExperience
    {
        public static readonly bool WRAPPED = true;

        public MercWordToExperience(byte[] data)
            : base(data)
        {
            base._Experience   = BitConverter.ToUInt16(data, 6);
        }
    }

	/// <summary>
	/// GS Packet 0xA7 - Delayed State - A player casts/selects a long lasting skill like a frozen orb/blizzard etc.
	/// Only seems related to a few auras and long lasting spells and when a monster gets frozen / stuned.
	/// </summary>
	public class DelayedState : GSPacket
	{
		public readonly BaseState State;
		public readonly UnitType UnitType;
		public readonly uint UID;

		public DelayedState(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data,2);
			this.State = BaseState.Get(data[6]);
		}
	}

	/// <summary>
	/// GS Packet 0xA8 - Set State - Notifies to start applying potion / aura / cast delay state effect.
	/// The server will send you this packet followed by a 0x47 and 0x48 for every player coverd by the aura/spell.
	/// </summary>
	public class SetState : GSPacket
	{
		public readonly BaseState State;
		public readonly UnitType UnitType;
		public readonly uint UID;

		public SetState(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.State = BaseState.Get(data[7]);
		}
	}

	/// <summary>
	/// GS Packet 0xA9 - End State - Notifies a potion / aura's effect is over or the unit no longer is coverd by it.
	/// </summary>
	public class EndState : GSPacket
	{
		public readonly BaseState State;
		public readonly UnitType UnitType;
		public readonly uint UID;

		public EndState(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.State = BaseState.Get(data[6]);
		}
	}

	/// <summary>
	/// GS Packet 0xAA - Add Unit - Create new Unit or mark old as valid (generally within 4 screen of player).
	/// </summary>
	public class AddUnit : GSPacket
	{
		public readonly UnitType Type;
		public readonly uint UID;

		public string Unknown7 { get { return ByteConverter.ToHexString(this.Data, 7); } }

		public AddUnit(byte[] data) : base(data)
		{
			this.Type = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
		}
	}

	/// <summary>
	/// GS Packet 0xAB - NPC Heal - An NPC within your range of view has gained some life.
	/// </summary>
	public class NPCHeal : GSPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly byte Life;

		public NPCHeal(byte[] data) : base (data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 2);
			this.Life = data[6];
		}
	}

	/// <summary>
	/// GS Packet 0xAC - NPC Assignment - A player (??) or NPC is being asigned to a location with in your range of view.
	/// </summary>
	public class AssignNPC : GSPacket
	{
		public readonly uint UID;
		public readonly NPCCode ID;
		public readonly ushort X;
		public readonly ushort Y;
		public readonly byte Life;

		public string Unknown13 { get { return ByteConverter.ToHexString(this.Data , 13); } }

		public AssignNPC(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.ID = (NPCCode) BitConverter.ToUInt16(data, 5);
			this.X = BitConverter.ToUInt16(data, 7);
			this.Y = BitConverter.ToUInt16(data, 9);
			this.Life = data[11];
			//data[12] = packet length
		}
	}
    
	/// <summary>
	/// GS Packet 0xAE - Warden Check - Teh Unholy Guardian of Secrets...
	/// </summary>
	public class WardenCheck : GSPacket
    {
        public byte[] WardenData
        {
            get
            {
                byte[] destinationArray = new byte[this.DataLength];
                Array.Copy(base.Data, 3, destinationArray, 0, this.DataLength);
                return destinationArray;
            }
        }

        public readonly int DataLength;

        public WardenCheck(byte[] data)
            : base(data)
        {
            this.DataLength = BitConverter.ToUInt16(data, 1);
        }

	}
    
	/// <summary>
	/// GS Packet 0xAF - Request Logon Info - First packet sent by Game Server when connection is established. Sent uncompressed !
	/// </summary>
	public class RequestLogonInfo : GSPacket
	{
		public readonly byte ProtocolVersion;

		public RequestLogonInfo(byte[] data) : base (data)
		{
			this.ProtocolVersion = data[1];
		}
	}

	/// <summary>
	/// GS Packet 0xB0 - Game Over - Sent when game server connnection is dropped
	/// </summary>
	public class GameOver : GSPacket
	{
		public GameOver(byte[] data) : base (data)
		{
		}
	}

}