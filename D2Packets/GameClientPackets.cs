using System;
using D2Data;
using D2Packets;
using ETUtils;

namespace GameClient
{
	public enum PartyAction
	{
		Invite = 0x06,
		CancelInvite = 0x07,
		AcceptInvite = 0x08,
    }

    public enum PlayerRelationType
    {
        Loot = 1,
        Mute = 2,
        Squelch = 3,
        Hostile = 4,
    }

	/// <summary>
	/// Base class for Game Client Packet objects
	/// </summary>
	public class GCPacket : D2Packet
	{
		public readonly GameClientPacket PacketID;

		public GCPacket(byte[] data) : base(data, Origin.GameClient)
		{
			this.PacketID = (GameClientPacket) data[0];
		}
	}

	/// <summary>
	/// GC Packet Wrapper - GoToLocation - Wrapper for packet 01 and 03
	/// </summary>
	public class GoToLocation : GCPacket
	{
		protected int _X;
		protected int _Y;
		public int X { get{ return this._X; } }
		public int Y { get{ return this._Y; } }

		public GoToLocation(byte[] data) : base(data)
		{
			this._X = BitConverter.ToUInt16(data, 1);
			this._Y = BitConverter.ToUInt16(data, 3);
		}
	}
	/// <summary>
	/// GC Packet 0x01 - Walk To Location
	/// </summary>
	public class WalkToLocation : GoToLocation
	{
		public static readonly bool WRAPPED = true;

		public WalkToLocation(byte[] data) : base(data)
		{
		}
        public WalkToLocation(int x, int y) : base(WalkToLocation.Build(x, y))
        {
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x01,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}
	/// <summary>
	/// GC Packet 0x03 - Run To Location
	/// </summary>
	public class RunToLocation : GoToLocation
	{
		public static readonly bool WRAPPED = true;
		public RunToLocation(byte[] data) : base(data)
		{
		}
        public RunToLocation(int x, int y) : base(RunToLocation.Build(x, y))
        {
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x03,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}
	
	/// <summary>
    /// GC Packet Wrapper - GoToTarget - Wrapper for packet 02 and 04
	/// </summary>
	public class GoToTarget : GCPacket
	{
		protected UnitType _UnitType;
		protected uint _UID;
		public UnitType UnitType { get{ return this._UnitType; } }
		public uint UID { get{ return this._UID; } }

        public GoToTarget(byte[] data)
            : base(data)
		{
			this._UnitType = (UnitType) BitConverter.ToUInt32(data, 1);
			this._UID = BitConverter.ToUInt32(data, 5);
		}
	}
	/// <summary>
	/// GC Packet 0x02 - Walk To Unit
	/// </summary>
    public class WalkToTarget : GoToTarget
	{
		public static readonly bool WRAPPED = true;
		public WalkToTarget(byte[] data) : base(data)
		{
		}
        public WalkToTarget(UnitType target, uint uid)
            : base(Build(target, uid))
        {
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x02, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}
	/// <summary>
	/// GC Packet 0x04 - Run To Target
	/// </summary>
    public class RunToTarget : GoToTarget
	{
		public static readonly bool WRAPPED = true;
		public RunToTarget(byte[] data) : base(data)
		{
		}
        public RunToTarget(UnitType target, uint uid)
            : base(Build(target, uid))
        {
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x04, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x05 - Cast Left Skill - Casts the currently set left hand skill on a target location.
	/// </summary>
	public class CastLeftSkill : GCPacket
	{
		public readonly int X;
		public readonly int Y;

		public CastLeftSkill(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
		}
        public CastLeftSkill(int x, int y) : base(CastLeftSkill.Build(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x05,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x06 - Cast Left Skill On Target - Casts the currently set left hand skill on a target unit.
	/// </summary>
	public class CastLeftSkillOnTarget : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public CastLeftSkillOnTarget(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public CastLeftSkillOnTarget(UnitType target, uint uid) : base(CastLeftSkillOnTarget.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x06, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x07 - Cast Left Skill On Target Stopped - Casts the currently set left hand skill on a target unit.
	/// </summary>
	public class CastLeftSkillOnTargetStopped : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public CastLeftSkillOnTargetStopped(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public CastLeftSkillOnTargetStopped(UnitType target, uint uid)
            : base(CastLeftSkillOnTargetStopped.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x07, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x08 - Recast Left Skill - Recasts the currently set left hand skill on a target location when spamming.
	/// </summary>
	public class RecastLeftSkill : GCPacket
	{
		public readonly int X;
		public readonly int Y;

		public RecastLeftSkill(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
		}
        public RecastLeftSkill(int x, int y) : base(RecastLeftSkill.Build(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x08,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x09 - Recast Left Skill On Target - Recasts the currently set left hand skill on a target unit when spamming.
	/// </summary>
	public class RecastLeftSkillOnTarget : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public RecastLeftSkillOnTarget(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public RecastLeftSkillOnTarget(UnitType target, uint uid) : base(RecastLeftSkillOnTarget.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x09, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x0A - Recast Left Skill On Target Stopped - Recasts the currently set left hand skill on a target unit when spamming.
	/// </summary>
	public class RecastLeftSkillOnTargetStopped : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public RecastLeftSkillOnTargetStopped(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public RecastLeftSkillOnTargetStopped(UnitType target, uint uid)
            : base(RecastLeftSkillOnTargetStopped.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x0A, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x0C - Cast Right Skill - Casts the currently set right hand skill on a target location.
	/// </summary>
	public class CastRightSkill : GCPacket
	{
		public readonly int X;
		public readonly int Y;

		public CastRightSkill(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
		}
        public CastRightSkill(int x, int y) : base(CastRightSkill.Build(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x0C,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x0D - Cast Right Skill On Target - Casts the currently set right hand skill on a target unit.
	/// </summary>
	public class CastRightSkillOnTarget : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public CastRightSkillOnTarget(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public CastRightSkillOnTarget(UnitType target, uint uid) : base(CastRightSkillOnTarget.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x0D, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x0E - Cast Right Skill On Target Stopped - Casts the currently set right hand skill on a target unit.
	/// </summary>
	public class CastRightSkillOnTargetStopped : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public CastRightSkillOnTargetStopped(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public CastRightSkillOnTargetStopped(UnitType target, uint uid)
            : base(CastRightSkillOnTargetStopped.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x0E, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x0F - Recast Right Skill - Recasts the currently set right hand skill on a target location when spamming.
	/// </summary>
	public class RecastRightSkill : GCPacket
	{
		public readonly int X;
		public readonly int Y;

		public RecastRightSkill(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
		}
        public RecastRightSkill(int x, int y) : base(RecastRightSkill.Build(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x0F,
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x10 - Recast Right Skill On Target - Recasts the currently set right hand skill on a target unit when spamming.
	/// </summary>
	public class RecastRightSkillOnTarget : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public RecastRightSkillOnTarget(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public RecastRightSkillOnTarget(UnitType target, uint uid) : base(RecastRightSkillOnTarget.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x10, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x11 - Recast Right Skill On Target Stopped - Recasts the currently set right hand skill on a target unit when spamming.
	/// </summary>
	public class RecastRightSkillOnTargetStopped : GCPacket
	{
		public readonly UnitType Target;
		public readonly uint UID;

		public RecastRightSkillOnTargetStopped(byte[] data) : base(data)
		{
			this.Target = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public RecastRightSkillOnTargetStopped(UnitType target, uint uid)
            : base(RecastRightSkillOnTargetStopped.Build(target, uid))
        {
            this.Target = target;
            this.UID = uid;
        }

        public static byte[] Build(UnitType target, uint uid)
        {
            return new byte[] { 0x11, 
                (byte)target, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x13 - Unit Interact - Try to interact with a unit. Result will vary depending on unit type...
	/// </summary>
	public class UnitInteract : GCPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public UnitInteract(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public UnitInteract(UnitType unitType, uint uid) : base(UnitInteract.Build(unitType, uid))
        {
            this.UnitType = unitType;
            this.UID = uid;
        }

        public static byte[] Build(UnitType unitType, uint uid)
        {
            return new byte[] { 0x13, 
                (byte)unitType, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x14 - Overhead Message - Display an overhead message.
	/// </summary>
	public class SendOverheadMessage : GCPacket
	{
		public readonly string Message;
		
        /// <summary>
        /// Probably the MessageType to match other message packets...
        /// Unused since not needed for single message type packet.
        /// </summary>
		public string Unknown1 { get { return ByteConverter.ToHexString(this.Data, 1, 2); } }
        /// <summary>
        /// Probably just the same unused reserved nullstrings as below...
        /// </summary>
		public string UnknownE { get { return ByteConverter.ToHexString(this.Data, 4 + this.Message.Length, 2); } }

		public SendOverheadMessage(byte[] data) : base(data)
		{
			this.Message = ByteConverter.GetNullString(data, 3);
		}
        public SendOverheadMessage(string message)
            : base(Build(message))
        {
            this.Message = message;
        }
        public static byte[] Build(string message)
        {
            if (message == null || message.Length == 0)
                throw new ArgumentException();
            byte[] bytes = new byte[6 + message.Length];
            bytes[0] = 0x14;
            for (int i = 0; i < message.Length; i++)
                bytes[3 + i] = (byte)message[i];
            return bytes;
        }
	}

	/// <summary>
	/// GC Packet 0x15 - Send Message - Send a public game message or whisper to a player.
	/// </summary>
	public class SendMessage : GCPacket
	{
		public readonly GameMessageType Type;
		public readonly string Message;
		public readonly string Recipient = null;

		public SendMessage(byte[] data) : base(data)
		{
			this.Type = (GameMessageType) BitConverter.ToUInt16(data, 1);
			this.Message = ByteConverter.GetNullString(data, 3);
			if (this.Type == GameMessageType.GameWhisper)
				this.Recipient = ByteConverter.GetNullString(data, 4 + this.Message.Length);
        }
        public SendMessage(GameMessageType type, string message) : base(SendMessage.Build(type, message))
        {
            this.Type = type;
            this.Message = message;
        }
        public SendMessage(GameMessageType type, string message, string recipient)
            : base(SendMessage.Build(type, message, recipient))
        {
            this.Type = type;
            this.Message = message;
            this.Recipient = recipient;
        }

        public static byte[] Build(GameMessageType type, string message)
        {
            return SendMessage.Build(type, message, null);
        }
        public static byte[] Build(GameMessageType type, string message, string recipient)
        {
            if (message == null || message.Length == 0)
                throw new ArgumentException();
            int r = recipient != null ? recipient.Length : 0;
            byte[] bytes = new byte[6 + message.Length + r];
            bytes[0] = 0x15;
            bytes[1] = (byte)type;
            for (int i = 0; i < message.Length; i++)
                bytes[3 + i] = (byte)message[i];
            if (r > 0)
            {
                r = 4 + message.Length;
                for (int i = 0; i < recipient.Length; i++)
                    bytes[r + i] = (byte)recipient[i];
            }
            return bytes;
        }
	}

	/// <summary>
	/// GC Packet 0x16 - PickItem - Pick up an item from the ground.
	/// </summary>
	public class PickItem : GCPacket
	{
		public readonly uint RequestID;
		public readonly uint UID;
		public readonly bool ToCursor = false;

		public PickItem(byte[] data) : base(data)
		{
			this.RequestID = BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
			if (data[9] == 1)
				this.ToCursor = true;
        }
        public PickItem(uint uid, bool toCursor) : base(PickItem.Build(uid, toCursor, 4))
        {
            this.UID = uid;
            this.ToCursor = toCursor;
            this.RequestID = 4;
        }
        public PickItem(uint uid, bool toCursor, uint requestID) : base(PickItem.Build(uid, toCursor, requestID))
        {
            this.UID = uid;
            this.ToCursor = toCursor;
            this.RequestID = requestID;
        }

        public static byte[] Build(uint uid, bool toCursor)
        {
            //TESTME: requestID? do we need to worry about it?
            return PickItem.Build(uid, toCursor, 4);
        }
        public static byte[] Build(uint uid, bool toCursor, uint requestID)
        {
            return new byte[] { 0x16, 
                (byte)(requestID & 0xFF), (byte)((requestID >> 8) & 0xFF), (byte)((requestID >> 16) & 0xFF), (byte)(requestID >> 24),
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24),
                (byte)(toCursor ? 1 : 0), 0, 0, 0
            };
        }
	}

	/// <summary>
	/// GC Packet 0x17 - DropItem - Drop the cursor item to the ground.
	/// </summary>
	public class DropItem : GCPacket
	{
		public readonly uint UID;

		public DropItem(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
		}
        public DropItem(uint uid) : base(DropItem.Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x17, 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24)
            };
        }
	}
    
	public enum ItemContainerGC
	{
		Inventory		= 0,
		Trade			= 2,
		Cube			= 3,
		Stash			= 4,
	}
	/// <summary>
    /// GC Packet 0x18 - Drop item from cursor to an ItemContainerGC
	/// </summary>
	public class DropItemToContainer : GCPacket
	{
		public readonly uint UID;
		public readonly ItemContainerGC Container;
		public readonly int X;
		public readonly int Y;

		public DropItemToContainer(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.X = data[5];
			this.Y = data[9];
			this.Container = (ItemContainerGC) data[13];
		}

        public DropItemToContainer(uint uid, ItemContainerGC container, int x, int y)
            : base(Build(uid, container, x, y))
        {
            this.UID = uid;
            this.Container = container;
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(uint uid, ItemContainerGC container, int x, int y)
        {
            return new byte[] { 0x18, 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24),
                (byte)x, 0, 0, 0,
                (byte)y, 0, 0, 0,
                (byte)container, 0, 0, 0
            };
        }
	}

	/// <summary>
	/// GC Packet 0x19 - Remove Container Item - Remove an item from a container and place it on cursor.
	/// </summary>
	public class PickItemFromContainer : GCPacket
	{
		public readonly uint UID;

		public PickItemFromContainer(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
		}
        public PickItemFromContainer(uint uid)
            : base(Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x19, 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x1A - Equip Item - Equip an item on the cursor to a specified body location.
	/// Item must be on cursor, of right type for location and location must be empty.
	/// </summary>
	public class EquipItem : GCPacket
	{
		public readonly uint ItemUID;
		public readonly EquipmentLocation Location;

		public EquipItem(byte[] data) : base(data)
		{
			this.ItemUID = BitConverter.ToUInt32(data,1);
			this.Location = (EquipmentLocation) data[5];
		}
        public EquipItem(uint itemUID, EquipmentLocation location) : base(EquipItem.Build(itemUID, location))
        {
            this.ItemUID = itemUID;
            this.Location = location;
        }

        public static byte[] Build(uint itemUID, EquipmentLocation location)
        {
            return new byte[] { 0x1A, 
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                (byte)location, 0, 0, 0
            };
        }
	}

	/// <summary>
	/// GC Packet 0x1D - Swap Equipped Item - Swaps an equipped item with another one on cursor.
	/// </summary>
	public class SwapEquippedItem : GCPacket
	{
		public readonly uint ItemUID;
		public readonly EquipmentLocation Location;

		public SwapEquippedItem(byte[] data) : base(data)
		{
			this.ItemUID = BitConverter.ToUInt32(data,1);
			this.Location = (EquipmentLocation) data[5];
		}
        public SwapEquippedItem(uint itemUID, EquipmentLocation location)
            : base(SwapEquippedItem.Build(itemUID, location))
        {
            this.ItemUID = itemUID;
            this.Location = location;
        }

        public static byte[] Build(uint itemUID, EquipmentLocation location)
        {
            return new byte[] { 0x1D, 
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                (byte)location, 0, 0, 0
            };
        }
	}

	/// <summary>
	/// GC Packet 0x1C - Unequip Item - Removes an equiped item from body and places on the cursor.
	/// </summary>
	public class UnequipItem : GCPacket
	{
		public readonly EquipmentLocation Location;

		public UnequipItem(byte[] data) : base(data)
		{
			this.Location = (EquipmentLocation) data[1];
		}
        public UnequipItem(EquipmentLocation location) : base(UnequipItem.Build(location))
        {
            this.Location = location;
        }

        public static byte[] Build(EquipmentLocation location)
        {
            return new byte[] { 0x1C, (byte)location, 0 };
        }
	}

	/// <summary>
	/// GC Packet 0x1F - Swap Container Item - Swap cursor (subject) item with another item in a container (object).
	/// Don't send this packet to replace items in trade container, as trade doesn't support item replacement.
	/// </summary>
	public class SwapContainerItem : GCPacket
	{
		public readonly uint SubjectUID;
		public readonly uint ObjectUID;
		public readonly int X;
		public readonly int Y;

		public SwapContainerItem(byte[] data) : base(data)
		{
			this.SubjectUID = BitConverter.ToUInt32(data, 1);
			this.ObjectUID = BitConverter.ToUInt32(data, 5);
			this.X = BitConverter.ToInt32(data, 9);
			this.Y = BitConverter.ToInt32(data, 13);
		}
        public SwapContainerItem(uint subjectUID, uint objectUID, int x, int y)
            : base(SwapContainerItem.Build(subjectUID, objectUID, x, y))
        {
            this.SubjectUID = subjectUID;
            this.ObjectUID = objectUID;
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(uint subjectUID, uint objectUID, int x, int y)
        {
            return new byte[] { 0x1F,
                (byte)(subjectUID & 0xFF), (byte)((subjectUID >> 8) & 0xFF), (byte)((subjectUID >> 16) & 0xFF), (byte)((subjectUID >> 24) & 0xFF),
                (byte)(objectUID & 0xFF), (byte)((objectUID >> 8) & 0xFF), (byte)((objectUID >> 16) & 0xFF), (byte)((objectUID >> 24) & 0xFF),
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF), (byte)((x >> 16) & 0xFF), (byte)((x >> 24) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF), (byte)((y >> 16) & 0xFF), (byte)((y >> 24) & 0xFF)
            };
        }
	}
	
	/// <summary>
	/// GC Packet 0x20 - Use Inventory Item - Use an item (drink potion, use tome, open cube, etc.) in inventory.
	/// X and Y are your standing location for the area you are in, not item position in inventory !
	/// </summary>
	public class UseInventoryItem : GCPacket
	{
		public readonly uint UID;
		public readonly int MeX;
		public readonly int MeY;

		public UseInventoryItem(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.MeX = BitConverter.ToInt32(data, 5);
			this.MeY = BitConverter.ToInt32(data, 9);
		}
        public UseInventoryItem(uint uid, int meX, int meY) : base(UseInventoryItem.Build(uid, meX, meY))
        {
            this.UID = uid;
            this.MeX = meX;
            this.MeY = meY;
        }

        public static byte[] Build(uint uid, int x, int y)
        {
            return new byte[] { 0x20,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF),
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF), (byte)((x >> 16) & 0xFF), (byte)((x >> 24) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF), (byte)((y >> 16) & 0xFF), (byte)((y >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x21 - Stack Items - Stack cursor (subject) item into another item (object).
	/// </summary>
	public class StackItems : GCPacket
	{
		public readonly uint SubjectUID;
		public readonly uint ObjectUID;

		public StackItems(byte[] data) : base(data)
		{
			this.SubjectUID = BitConverter.ToUInt32(data, 1);
			this.ObjectUID = BitConverter.ToUInt32(data, 5);
        }
        public StackItems(uint subjectUID, uint objectUID) : base(StackItems.Build(subjectUID, objectUID))
        {
            this.SubjectUID = subjectUID;
            this.ObjectUID = objectUID;
        }

        public static byte[] Build(uint subjectUID, uint objectUID)
        {
            return new byte[] { 0x21,
                (byte)(subjectUID & 0xFF), (byte)((subjectUID >> 8) & 0xFF), (byte)((subjectUID >> 16) & 0xFF), (byte)((subjectUID >> 24) & 0xFF),
                (byte)(objectUID & 0xFF), (byte)((objectUID >> 8) & 0xFF), (byte)((objectUID >> 16) & 0xFF), (byte)((objectUID >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x23 - Add Belt Item - Place the item on cursor into specified position in belt. Must be a potion or scroll...
	/// </summary>
	public class AddBeltItem : GCPacket
	{
		public readonly uint UID;
		public readonly int X;
		public readonly int Y;

		public AddBeltItem(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.X = data[5] % 4;
			this.Y = data[5] / 4;
		}
        public AddBeltItem(uint uid, int x, int y) : base(AddBeltItem.Build(uid, x, y))
        {
            this.UID = uid;
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(uint uid, int x, int y)
        {
            return new byte[] { 0x23,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF),
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x24 - Remove Belt Item - Remove an item (potion or scroll) from belt and place it on cursor.
	/// </summary>
	public class RemoveBeltItem : GCPacket
	{
		public readonly uint UID;

		public RemoveBeltItem(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
		}
        public RemoveBeltItem(uint uid) : base(RemoveBeltItem.Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x24,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x25 - Swap Belt Item - Place cursor item belt and replace it with the specified item in it's location. 
	/// </summary>
	public class SwapBeltItem : GCPacket
	{
		public readonly uint OldItemUID;
		public readonly uint NewItemUID;

		public SwapBeltItem(byte[] data) : base(data)
		{
			this.OldItemUID = BitConverter.ToUInt32(data, 1);
			this.NewItemUID = BitConverter.ToUInt32(data, 5);
		}
        public SwapBeltItem(uint oldItemUID, uint newItemUID) : base(SwapBeltItem.Build(oldItemUID, newItemUID))
        {
            this.OldItemUID = oldItemUID;
            this.NewItemUID = newItemUID;
        }

        public static byte[] Build(uint oldItemUID, uint newItemUID)
        {
            return new byte[] { 0x25,
                (byte)(oldItemUID & 0xFF), (byte)((oldItemUID >> 8) & 0xFF), (byte)((oldItemUID >> 16) & 0xFF), (byte)((oldItemUID >> 24) & 0xFF),
                (byte)(newItemUID & 0xFF), (byte)((newItemUID >> 8) & 0xFF), (byte)((newItemUID >> 16) & 0xFF), (byte)((newItemUID >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x26 - Use Belt Item - Consume an item (potion or scroll) in belt.
	/// </summary>
	public class UseBeltItem  : GCPacket
	{
		public readonly uint ItemUID;
		public readonly bool ToMerc;
		
		public string Unknown9 { get { return ByteConverter.ToHexString(this.Data, 9, 4); } }

		public UseBeltItem(byte[] data) : base(data)
		{
            this.ItemUID = BitConverter.ToUInt32(data, 1);
			if (BitConverter.ToUInt32(data, 5) == 1)
				this.ToMerc = true;
		}
        public UseBeltItem(uint itemUID, bool toMerc) : base(UseBeltItem.Build(itemUID, toMerc))
        {
            this.ItemUID = itemUID;
            this.ToMerc = toMerc;
        }
        public UseBeltItem(uint itemUID, bool toMerc, uint unknown9)
            : base(UseBeltItem.Build(itemUID, toMerc, unknown9))
        {
            this.ItemUID = itemUID;
            this.ToMerc = toMerc;
        }

        public static byte[] Build(uint itemUID, bool toMerc)
        {
            return UseBeltItem.Build(itemUID, toMerc, 0);
        }
        public static byte[] Build(uint itemUID, bool toMerc, uint unknown9)
        {
            return new byte[] { 0x26,
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                (byte)(toMerc ? 1 : 0), 0, 0, 0,
                (byte)(unknown9 & 0xFF), (byte)((unknown9 >> 8) & 0xFF), (byte)((unknown9 >> 16) & 0xFF), (byte)((unknown9 >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x27 - Identify Item - Use an identify scoll on an item.
	/// If already indentified, id cursor is canceled and scoll is not used. If not clicking on an item, UID is that of scoll and action is canceled.
	/// </summary>
	public class IdentifyItem : GCPacket
	{
		public readonly uint ItemUID;
		public readonly uint ScrollUID;

		public IdentifyItem(byte[] data) : base (data)
		{
			this.ItemUID = BitConverter.ToUInt32(data, 1);
			this.ScrollUID = BitConverter.ToUInt32(data, 5);
		}
        public IdentifyItem(uint itemUID, uint scrollUID) : base(IdentifyItem.Build(itemUID, scrollUID))
        {
            this.ItemUID = itemUID;
            this.ScrollUID = scrollUID;
        }

        public static byte[] Build(uint itemUID, uint scrollUID)
        {
            return new byte[] { 0x27,
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                (byte)(scrollUID & 0xFF), (byte)((scrollUID >> 8) & 0xFF), (byte)((scrollUID >> 16) & 0xFF), (byte)((scrollUID >> 24) & 0xFF)
            };
        }
	}
    
    /// <summary>
    /// GC Packet 0x29 - Embed Item - Put an item into a container item (e.g. scroll in tome.)
	/// </summary>
	public class EmbedItem : GCPacket
	{
		public readonly uint SubjectUID;
		public readonly uint ObjectUID;

		public EmbedItem(byte[] data) : base (data)
		{
            this.SubjectUID = BitConverter.ToUInt32(data, 1);
            this.ObjectUID = BitConverter.ToUInt32(data, 5);
		}
        public EmbedItem(uint subjectUID, uint objectUID) : base(EmbedItem.Build(subjectUID, objectUID))
        {
            this.SubjectUID = subjectUID;
            this.ObjectUID = objectUID;
        }

        public static byte[] Build(uint subjectUID, uint objectUID)
        {
            return new byte[] { 0x29,
                (byte)(subjectUID & 0xFF), (byte)((subjectUID >> 8) & 0xFF), (byte)((subjectUID >> 16) & 0xFF), (byte)((subjectUID >> 24) & 0xFF),
                (byte)(objectUID & 0xFF), (byte)((objectUID >> 8) & 0xFF), (byte)((objectUID >> 16) & 0xFF), (byte)((objectUID >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x2A - Item To Cube - Drop an item in the Horadric cube by clicking on it with the item on cursor.
    /// Note that a regular DropItemToContainer packet is used if the item is dropped at a location in the cube and not on it.
	/// </summary>
	public class ItemToCube : GCPacket
	{
		public readonly uint ItemUID;
		public readonly uint CubeUID;

		public ItemToCube(byte[] data) : base(data)
		{
			this.ItemUID = BitConverter.ToUInt32(data, 1);
			this.CubeUID = BitConverter.ToUInt32(data, 5);
		}
        public ItemToCube(uint itemUID, uint cubeUID) : base(ItemToCube.Build(itemUID, cubeUID))
        {
            this.ItemUID = itemUID;
            this.CubeUID = cubeUID;
        }

        public static byte[] Build(uint itemUID, uint cubeUID)
        {
            return new byte[] { 0x2A,
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                (byte)(cubeUID & 0xFF), (byte)((cubeUID >> 8) & 0xFF), (byte)((cubeUID >> 16) & 0xFF), (byte)((cubeUID >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x2F - Town Folk Interact - Actual interact request sent after npc is in interaction range.
	/// A regular UnitInteract is sometimes(?) also sent first...
	/// </summary>
	public class TownFolkInteract : GCPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public TownFolkInteract(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public TownFolkInteract(UnitType unitType, uint uid) : base(TownFolkInteract.Build(unitType, uid))
        {
            this.UnitType = unitType;
            this.UID = uid;
        }

        public static byte[] Build(UnitType unitType, uint uid)
        {
            return new byte[] { 0x2F,
                (byte)unitType, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x30 - Town Folk Cancel Interaction - Close interact menu or trade etc. screen and break interact with town folk.
	/// </summary>
	public class TownFolkCancelInteraction : GCPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;

		public TownFolkCancelInteraction(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public TownFolkCancelInteraction(UnitType unitType, uint uid)
            : base(TownFolkCancelInteraction.Build(unitType, uid))
        {
            this.UnitType = unitType;
            this.UID = uid;
        }

        public static byte[] Build(UnitType unitType, uint uid)
        {
            return new byte[] { 0x30,
                (byte)unitType, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

    /// <summary>
    /// GC Packet 0x31 DisplayQuestMessage
    /// </summary>
    public class DisplayQuestMessage: GCPacket 
    {
        public readonly  uint message;
        public readonly uint uid;

        public DisplayQuestMessage(byte[] data)
            : base(data)
        {
            this.uid = BitConverter.ToUInt32(data, 1);
            this.message = BitConverter.ToUInt32(data, 5);
        }

        public DisplayQuestMessage(uint uid, uint message)
            : base(Build(uid, message))
        {
            this.uid = uid;
            this.message = message;
        }

        public static byte[] Build(uint uid, uint message)
        {
            return new byte[] { 0x31, ((byte)uid), ((byte)(uid >> 8)), ((byte)(uid >> 0x10)), ((byte)(uid >> 0x18)), ((byte)message), ((byte)(message >> 8)), ((byte)(message >> 0x10)), ((byte)(message >> 0x18)) };
        }
    }


	public enum TradeType : ushort
	{
		BuyItem		= 0,
		//Repair	= 1, // ?? Repair a single item first unknown DWORD is sometimes(?) 1, 0 if repair all... add here?
		GambleItem	= 2,
		SellItem	= 4,
	}
	[Flags]
	public enum BuyFlags : ushort
	{
		None		= 0,
		FillStack	= 0x8000,
	}
	/// <summary>
	/// GC Packet 0x32 - Buy Item - Buy an item from a town folk.
	/// </summary>
	public class BuyItem : GCPacket
	{
		public readonly uint SellerUID;
		public readonly uint ItemUID;
		public readonly uint Cost;
		public readonly TradeType Type;
		public readonly BuyFlags Flags;

		public BuyItem(byte[] data) : base(data)
		{
			this.SellerUID = BitConverter.ToUInt32(data, 1);
			this.ItemUID = BitConverter.ToUInt32(data, 5);
			this.Type = (TradeType) BitConverter.ToUInt16(data, 9);
			this.Flags = (BuyFlags) BitConverter.ToUInt16(data, 11);
			this.Cost = BitConverter.ToUInt32(data, 13);
		}
        public BuyItem(uint sellerUID, uint itemUID, uint cost, BuyFlags flags)
            : base(BuyItem.Build(sellerUID, itemUID, cost, flags))
        {
            this.SellerUID = sellerUID;
            this.ItemUID = itemUID;
            this.Cost = cost;
            this.Type = TradeType.BuyItem;
            this.Flags = flags;
        }

        public static byte[] Build(uint sellerUID, uint itemUID, uint cost, BuyFlags flags)
        {
            return new byte[] { 0x32,
                (byte)(sellerUID & 0xFF), (byte)((sellerUID >> 8) & 0xFF), (byte)((sellerUID >> 16) & 0xFF), (byte)((sellerUID >> 24) & 0xFF),
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                0, 0, (byte)((ushort)flags & 0xFF), (byte)(((ushort)flags >> 8) & 0xFF),
                (byte)(cost & 0xFF), (byte)((cost >> 8) & 0xFF), (byte)((cost >> 16) & 0xFF), (byte)((cost >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x33 - Sell Item - Sell an item to a town folk.
	/// </summary>
	public class SellItem : GCPacket
	{
		public readonly uint BuyerUID;
		public readonly uint ItemUID;
		public readonly uint Cost;
		public readonly TradeType Type;

		public SellItem(byte[] data) : base(data)
		{
			this.BuyerUID = BitConverter.ToUInt32(data, 1);
			this.ItemUID = BitConverter.ToUInt32(data, 5);
			this.Type = (TradeType) BitConverter.ToUInt32(data, 9);
			this.Cost = BitConverter.ToUInt32(data, 13);
		}
        public SellItem(uint buyerUID, uint itemUID, uint cost) : base(SellItem.Build(buyerUID, itemUID, cost))
        {
            this.BuyerUID = buyerUID;
            this.ItemUID = itemUID;
            this.Cost = cost;
            this.Type = TradeType.SellItem;
        }

        public static byte[] Build(uint buyerUID, uint itemUID, uint cost)
        {
            return new byte[] { 0x33,
                (byte)(buyerUID & 0xFF), (byte)((buyerUID >> 8) & 0xFF), (byte)((buyerUID >> 16) & 0xFF), (byte)((buyerUID >> 24) & 0xFF),
                (byte)(itemUID & 0xFF), (byte)((itemUID >> 8) & 0xFF), (byte)((itemUID >> 16) & 0xFF), (byte)((itemUID >> 24) & 0xFF),
                4, 0, 0, 0,
                (byte)(cost & 0xFF), (byte)((cost >> 8) & 0xFF), (byte)((cost >> 16) & 0xFF), (byte)((cost >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x34 - Cain Identify Items - Select the Identify Items option when interacted with Cain.
	/// </summary>
	public class CainIdentifyItems : GCPacket
	{
		public readonly uint UID;

		public CainIdentifyItems(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
		}
        public CainIdentifyItems(uint uid) : base(CainIdentifyItems.Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x34,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF)
            };
        }
	}

	public enum RepairType
    {
        RepairAll   = 0,
		RepairItem	= 1,
	}
	/// <summary>
	/// GC Packet 0x35 - Town Folk Repair - Ask a blacksmith to repair one or all item(s).
	/// </summary>
	public class TownFolkRepair : GCPacket
	{
		public static readonly uint NULL_UInt32 = 0;

		public readonly uint BlacksmithUID;
		public readonly uint ItemUID;
		public readonly RepairType Type;

		public string Unknown9 { get { return ByteConverter.ToHexString(this.Data, 13, 4); } }

		public TownFolkRepair(byte[] data) : base(data)
		{
			this.BlacksmithUID = BitConverter.ToUInt32(data, 1);
			this.ItemUID = BitConverter.ToUInt32(data, 5);
            this.Type = (RepairType) BitConverter.ToUInt32(data, 9);
		}
        public TownFolkRepair(uint blacksmithUID) : base(TownFolkRepair.Build(blacksmithUID))
        {
            this.BlacksmithUID = blacksmithUID;
            this.ItemUID = 0;
            this.Type = RepairType.RepairAll;
        }
        //TODO: overload for RepairItem

        public static byte[] Build(uint blacksmithUID)
        {
            return new byte[] { 0x35,
                (byte)(blacksmithUID & 0xFF), (byte)((blacksmithUID >> 8) & 0xFF), (byte)((blacksmithUID >> 16) & 0xFF), (byte)((blacksmithUID >> 24) & 0xFF),
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0x80
            };
        }
	}

	/// <summary>
	/// GC Packet 0x36 - Hire Mercenary - Hire a mercenary from a town folk.
	/// </summary>
	public class HireMercenary : GCPacket
	{
		public readonly uint SlaverUID;
		public readonly uint MercID;

		public HireMercenary(byte[] data) : base(data)
		{
			this.SlaverUID = BitConverter.ToUInt32(data, 1);
			this.MercID = BitConverter.ToUInt32(data, 5);
		}
        public HireMercenary(uint slaverUID, uint mercID) : base(HireMercenary.Build(slaverUID, mercID))
        {
            this.SlaverUID = slaverUID;
            this.MercID = mercID;
        }

        public static byte[] Build(uint slaverUID, uint mercID)
        {
            return new byte[] { 0x36,
                (byte)(slaverUID & 0xFF), (byte)((slaverUID >> 8) & 0xFF), (byte)((slaverUID >> 16) & 0xFF), (byte)((slaverUID >> 24) & 0xFF),
                (byte)(mercID & 0xFF), (byte)((mercID >> 8) & 0xFF), (byte)((mercID >> 16) & 0xFF), (byte)((mercID >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x37 - Identify Gamble Item - Identify a gambled item after you bought it.
	/// </summary>
	public class IdentifyGambleItem : GCPacket
	{
		public readonly uint UID;

		public IdentifyGambleItem(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
		}
        public IdentifyGambleItem(uint uid) : base(IdentifyGambleItem.Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x37,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF),
            };
        }
	}

	/// <summary>
	/// GC Packet 0x38 - Town Folk Menu Select - Choose a menu item / action type once interacted with the town folk.
	/// </summary>
    public class TownFolkMenuSelect : GCPacket
	{
		public readonly TownFolkMenuItem Selection;
		public readonly uint UID;

		public string Unknown9 { get { return ByteConverter.ToHexString(this.Data, 9, 4); } }

		public TownFolkMenuSelect(byte[] data) : base(data)
		{
			this.Selection = (TownFolkMenuItem) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
		}
        public TownFolkMenuSelect(TownFolkMenuItem selection, uint uid, uint unknown9)
            : base(Build(selection, uid, unknown9))
        {
            this.Selection = selection;
            this.UID = uid;
        }

        public static byte[] Build(TownFolkMenuItem selection, uint uid, uint unknown9)
        {
            return new byte[] { 0x38,
                (byte)((uint)selection & 0xFF), (byte)(((uint)selection >> 8) & 0xFF), (byte)(((uint)selection >> 16) & 0xFF), (byte)(((uint)selection >> 24) & 0xFF),
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)((uid >> 24) & 0xFF),
                (byte)(unknown9 & 0xFF), (byte)((unknown9 >> 8) & 0xFF), (byte)((unknown9 >> 16) & 0xFF), (byte)((unknown9 >> 24) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x3A - Increment Attribute - Raise an attribute by one point.
	/// </summary>
	public class IncrementAttribute : GCPacket
	{
		public readonly StatType Attribute;

		public IncrementAttribute(byte[] data) : base(data)
		{
			this.Attribute = (StatType) BitConverter.ToUInt16(data, 1);
		}
        public IncrementAttribute(StatType attribute) : base(IncrementAttribute.Build(attribute))
        {
            this.Attribute = attribute;
        }

        public static byte[] Build(StatType attribute)
        {
            return new byte[] { 0x3A, (byte)((ushort)attribute & 0xFF), (byte)(((ushort)attribute >> 8) & 0xFF) };
        }
	}

	/// <summary>
	/// GC Packet 0x3B - Increment Skill - Raise a skill by one point.
	/// </summary>
	public class IncrementSkill : GCPacket
	{
		public readonly SkillType Skill;

		public IncrementSkill(byte[] data) : base(data)
		{
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 1);
		}
        public IncrementSkill(SkillType skill) : base(IncrementSkill.Build(skill))
        {
            this.Skill = skill;
        }

        public static byte[] Build(SkillType skill)
        {
            return new byte[] { 0x3B, (byte)((ushort)skill & 0xFF), (byte)(((ushort)skill >> 8) & 0xFF) };
        }
	}

	/// <summary>
	/// GC Packet 0x3C - SelectSkill - Sets a given skill as the active one for specified hand.
	/// </summary>
	public class SelectSkill : GCPacket
	{
		public static readonly uint NULL_UInt32 = 0;

		public readonly SkillType Skill;
		public readonly SkillHand Hand = SkillHand.Right;
		public readonly uint ChargedItemUID;

		public SelectSkill(byte[] data) : base(data)
		{
			this.Skill = (SkillType) BitConverter.ToUInt16(data, 1);
			if (data[4] == 0x80) // A bit weird... whats data[3] and why not use same way GS packet for hand?
				this.Hand = SkillHand.Left;
			this.ChargedItemUID = BitConverter.ToUInt32(data, 5);
			if (this.ChargedItemUID == 0xFFFFFFFF)
				this.ChargedItemUID = 0;
        }
        public SelectSkill(SkillType skill, SkillHand hand) : base(SelectSkill.Build(skill, hand))
        {
            this.Skill = skill;
            this.Hand = hand;
            this.ChargedItemUID = 0;
        }
        public SelectSkill(SkillType skill, SkillHand hand, uint chargedItemUID)
            : base(SelectSkill.Build(skill, hand, chargedItemUID))
        {
            this.Skill = skill;
            this.Hand = hand;
            this.ChargedItemUID = chargedItemUID;
        }

        public static byte[] Build(SkillType skill, SkillHand hand)
        {
            return SelectSkill.Build(skill, hand, 0xFFFFFFFF);
        }
        public static byte[] Build(SkillType skill, SkillHand hand, uint chargedItemUID)
        {
            return new byte[] { 0x3C, 
                (byte)((ushort)skill & 0xFF), (byte)(((ushort)skill >> 8) & 0xFF),
                0, (byte)(hand == SkillHand.Left ? 0x80 : 0),
                (byte)(chargedItemUID & 0xFF), (byte)((chargedItemUID >> 8) & 0xFF), (byte)((chargedItemUID >> 16) & 0xFF), (byte)((chargedItemUID >> 24) & 0xFF),
            };
        }
	}

    /// <summary>
    /// GC Packet 0x3D
    /// </summary>
    /// 
    public class HoverUnit : GCPacket
    {
        public readonly uint UID;

        public HoverUnit(byte[] data)
            : base(data)
        {
            this.UID = BitConverter.ToUInt32(data, 1);
        }


        public HoverUnit(uint uid)
            : base(Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x3d, ((byte)uid), ((byte)(uid >> 8)), ((byte)(uid >> 0x10)), ((byte)(uid >> 0x18)) };
        }
    }

	/// <summary>
	/// GC Packet 0x3F - Send Character Speech
	/// </summary>
	public class SendCharacterSpeech : GCPacket
	{
		public readonly GameSound Speech;

		public SendCharacterSpeech(byte[] data) : base(data)
		{
			this.Speech = (GameSound) BitConverter.ToUInt16(data, 1);
		}
        public SendCharacterSpeech(GameSound speech) : base(SendCharacterSpeech.Build(speech))
        {
            this.Speech = speech;
        }

        public static byte[] Build(GameSound speech)
        {
            return new byte[] { 0x3F, (byte)((ushort)speech & 0xFF), (byte)(((ushort)speech >> 8) & 0xFF) };
        }
	}

	/// <summary>
	/// GC Packet 0x40 - Request Quest Log - Request the character's quest log for the current act.
	/// </summary>
	public class RequestQuestLog : GCPacket
	{
		public RequestQuestLog(byte[] data) : base(data)
		{
		}
        public RequestQuestLog() : base(RequestQuestLog.Build())
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x40 };
        }
	}

    /// <summary>
    /// GC Packet 0x41 - Respawn
    /// </summary>
    public class Respawn : GCPacket
    {
        public Respawn()
            : base(Build())
        {
        }

        public Respawn(byte[] data)
            : base(data)
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x41 };
        }
    }


	/// <summary>
	/// GC Packet 0x49 - Waypoint Interact
	/// </summary>
	public class WaypointInteract : GCPacket
	{
		public readonly uint UID;
		public readonly WaypointDestination Destination;

		public WaypointInteract(byte[] data) : base (data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
			this.Destination = (WaypointDestination) data[5];
		}
        public WaypointInteract(uint uid, WaypointDestination destination)
            : base(WaypointInteract.Build(uid, destination))
        {
            this.UID = uid;
            this.Destination = destination;
        }

        public static byte[] Build(uint uid, WaypointDestination destination)
        {
            return new byte[] { 0x49, 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24), 
                (byte)destination, 0, 0, 0
            };
        }
	}

	/// <summary>
    /// GC Packet 0x4B - Request Reassign - Tells server to bring merc along when teleporting or changing area (WP, portal, etc.)
	/// Its important to send this packet whenever you change area (if you have a live merc... this only counts for merc and not summoned pets.)
	/// </summary>
    public class RequestReassign : GCPacket
	{
		public readonly UnitType UnitType;
		public readonly uint MyUID;

		public RequestReassign(byte[] data) :base(data)
		{
			this.UnitType = (UnitType) data[1];
			this.MyUID = BitConverter.ToUInt32(data, 5);
		}
        public RequestReassign(UnitType unitType, uint myUID) : base(Build(unitType, myUID))
        {
            this.UnitType = unitType;
            this.MyUID = myUID;
        }

        public static byte[] Build(UnitType unitType, uint myUID)
        {
            return new byte[] { 0x4B,
                (byte)unitType, 0, 0, 0,
                (byte)(myUID & 0xFF), (byte)((myUID >> 8) & 0xFF), (byte)((myUID >> 16) & 0xFF), (byte)(myUID >> 24)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x4F - Click Button - Click on a UI button.
	/// </summary>
	public class ClickButton : GCPacket
	{
		public readonly GameButton Button;
		public readonly ushort Complement;

		public ClickButton(byte[] data) :base(data)
		{
			this.Button = (GameButton) BitConverter.ToUInt32(data, 1);
			this.Complement = BitConverter.ToUInt16(data, 5);
		}
        public ClickButton(GameButton button, ushort complement) : base(ClickButton.Build(button, complement))
        {
            this.Button = button;
            this.Complement = complement;
        }

        public static byte[] Build(GameButton button, ushort complement)
        {
            return new byte[] { 0x4F, 
                (byte)((uint)button & 0xFF),(byte)(((uint)button >> 8) & 0xFF), (byte)(((uint)button >> 16) & 0xFF), (byte)((uint)button >> 24), 
                (byte)(complement & 0xFF), (byte)((complement >> 8) & 0xFF)
            };
        }
	}
    
	/// <summary>
	/// GC Packet 0x50 - Drop Gold - Remove gold from inventory and drop it on the ground.
	/// </summary>
	public class DropGold : GCPacket
	{
		public readonly uint Amount;
		public readonly uint MyUID;

		public DropGold(byte[] data) : base(data)
		{
			this.MyUID = BitConverter.ToUInt32(data, 1);
			this.Amount = BitConverter.ToUInt32(data, 5);
		}
        public DropGold(uint amount, uint myUID) : base(DropGold.Build(amount, myUID))
        {
            this.Amount = amount;
            this.MyUID = myUID;
        }

        public static byte[] Build(uint amount, uint myUID)
        {
            return new byte[] { 0x50, 
                (byte)(myUID & 0xFF), (byte)((myUID >> 8) & 0xFF), (byte)((myUID >> 16) & 0xFF), (byte)(myUID >> 24),
                (byte)(amount & 0xFF), (byte)((amount >> 8) & 0xFF), (byte)((amount >> 16) & 0xFF), (byte)(amount >> 24)
            };
        }
	}

    public class SetSkillHotkey : GCPacket
    {

        public readonly  uint ChargedItemUID;
        public readonly uint NULL_UInt32;
        public readonly SkillType Skill;
        public readonly ushort Slot;

        public SetSkillHotkey(byte[] data)
            : base(data)
        {
            this.Skill = (SkillType)BitConverter.ToUInt16(data, 1);
            this.Slot = BitConverter.ToUInt16(data, 3);
            this.ChargedItemUID = BitConverter.ToUInt32(data, 5);
        }


        public SetSkillHotkey(ushort slot, SkillType skill)
            : this(slot, skill, uint.MaxValue)
        {
        }

        public SetSkillHotkey(ushort slot, SkillType skill, uint itemUID)
            : base(Build(slot, skill, itemUID))
        {
            this.Slot = slot;
            this.Skill = skill;
            this.ChargedItemUID = itemUID;
        }

        public static byte[] Build(ushort slot, SkillType skill)
        {
            return Build(slot, skill, uint.MaxValue);
        }

        public static byte[] Build(ushort slot, SkillType skill, uint itemUID)
        {
            return new byte[] { 0x51, ((byte)skill), ((byte)(((ushort)skill) >> 8)), ((byte)slot), ((byte)(slot >> 8)), ((byte)itemUID), ((byte)(itemUID >> 8)), ((byte)(itemUID >> 0x10)), ((byte)(itemUID >> 0x18)) };
        }

 

 

    }


    public class CloseQuest : GCPacket
    {
       public readonly  QuestType Quest;


        public CloseQuest(byte[] data)
            : base(data)
        {
            this.Quest = (QuestType)BitConverter.ToUInt16(data, 1);
        }

        public CloseQuest(QuestType quest)
            : base(Build(quest))
        {
            this.Quest = quest;
        }


        public static byte[] Build(QuestType quest)
        {
            return new byte[] { 0x58, ((byte)quest), ((byte)(((ushort)quest) >> 8)) };
        }

    }

	/// <summary>
	/// GC Packet 0x59 - Go To Town Folk - Notify the server of intention to interact with a town folk.
	/// First pack sent when clicking on a town folk, before running / walking toward it before actual interaction.
	/// </summary>
	public class GoToTownFolk : GCPacket
	{
		public readonly UnitType UnitType;
		public readonly uint UID;
		public readonly uint X;
		public readonly uint Y;

		public GoToTownFolk(byte[] data) : base(data)
		{
			this.UnitType = (UnitType) BitConverter.ToUInt32(data, 1);
			this.UID = BitConverter.ToUInt32(data, 5);
			this.X = BitConverter.ToUInt32(data, 9);
			this.Y = BitConverter.ToUInt32(data, 13);
		}
        public GoToTownFolk(UnitType unitType, uint uid, uint x, uint y)
            : base(GoToTownFolk.Build(unitType, uid, x, y))
        {
            this.UnitType = unitType;
            this.UID = uid;
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(UnitType unitType, uint uid, uint x, uint y)
        {
            return new byte[] { 0x59, (byte)unitType, 0, 0, 0,
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24),
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF), (byte)((x >> 16) & 0xFF), (byte)(x >> 24),
                (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF), (byte)((y >> 16) & 0xFF), (byte)(y >> 24)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x5D - Set Player Relation
	/// </summary>
	public class SetPlayerRelation : GCPacket
    {
        public readonly uint UID;
        public readonly PlayerRelationType Relation;
        public readonly bool Value;

		public SetPlayerRelation(byte[] data) : base(data)
        {
            this.Relation = (PlayerRelationType)data[1];
            this.Value = BitConverter.ToBoolean(data, 2);
            this.UID = BitConverter.ToUInt32(data, 3);
		}
        public SetPlayerRelation(uint uid, PlayerRelationType relation, bool value)
            : base(SetPlayerRelation.Build(uid, relation, value))
        {
            this.UID = uid;
            this.Relation = relation;
            this.Value = value;
        }

        public static byte[] Build(uint uid, PlayerRelationType relation, bool value)
        {
            return new byte[] { 0x5D, (byte)relation, (byte)(value ? 1 : 0), 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24)
            };
        }
	}
	
	/// <summary>
	/// GC Packet 0x5E - Party resquest
	/// </summary>
	public class PartyRequest : GCPacket
	{
		public readonly PartyAction Action;
		public readonly uint PlayerUID;

		public PartyRequest(byte[] data) : base(data)
		{
			this.Action = (PartyAction) data[1];
			this.PlayerUID = BitConverter.ToUInt32(data,2);
		}
        public PartyRequest(PartyAction action, uint playerUID) : base(PartyRequest.Build(action, playerUID))
        {
            this.Action = action;
            this.PlayerUID = playerUID;
        }

        public static byte[] Build(PartyAction action, uint playerUID)
        {
            return new byte[] { 0x5E, (byte)action, 
                (byte)(playerUID & 0xFF), (byte)((playerUID >> 8) & 0xFF), (byte)((playerUID >> 16) & 0xFF), (byte)(playerUID >> 24)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x5F - Update Position
	/// </summary>
	public class UpdatePosition : GCPacket
	{
		public readonly int X;
		public readonly int Y;

		public UpdatePosition(byte[] data) : base(data)
		{
			this.X = BitConverter.ToUInt16(data, 1);
			this.Y = BitConverter.ToUInt16(data, 3);
		}
        public UpdatePosition(int x, int y) : base(UpdatePosition.Build(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Build(int x, int y)
        {
            return new byte[] { 0x5F, 
                (byte)(x & 0xFF), (byte)((x >> 8) & 0xFF), (byte)(y & 0xFF), (byte)((y >> 8) & 0xFF)
            };
        }
	}
	
	/// <summary>
	/// GC Packet 0x60 - Switch Weapons - Toogle the active weapon tab.
	/// </summary>
	public class SwitchWeapons : GCPacket
	{
		public SwitchWeapons(byte[] data) : base(data)
		{
		}
        public SwitchWeapons() : base(SwitchWeapons.Build())
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x60 };
        }
	}
	
	/// <summary>
	/// GC Packet 0x61 - Change Merc Equipment - Equip or unequip merc item.
	/// </summary>
	public class ChangeMercEquipment : GCPacket
	{
		public readonly EquipmentLocation Location;
		public readonly bool Unequip = false;

		public ChangeMercEquipment(byte[] data) : base(data)
		{
			this.Location = (EquipmentLocation) BitConverter.ToUInt16(data, 1);
			if (this.Location != EquipmentLocation.NotApplicable)
				this.Unequip = true;
		}
        public ChangeMercEquipment(EquipmentLocation location)
            : base(ChangeMercEquipment.Build(location))
        {
            this.Location = location;
            if (location != EquipmentLocation.NotApplicable)
                this.Unequip = true;
        }

        public static byte[] Build(EquipmentLocation location)
        {
            return new byte[] { 0x61, 
                (byte)((ushort)location & 0xFF), (byte)(((ushort)location >> 8) & 0xFF)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x62 - Resurrect Merc - Resurrect your current mercenary at a slaver town folk.
	/// </summary>
	public class ResurrectMerc : GCPacket
	{
		public readonly uint SlaverUID;

		public ResurrectMerc(byte[] data) : base(data)
		{
			this.SlaverUID = BitConverter.ToUInt32(data, 1);
		}
        public ResurrectMerc(uint slaverUID) : base(ResurrectMerc.Build(slaverUID))
        {
            this.SlaverUID = slaverUID;
        }

        public static byte[] Build(uint slaverUID)
        {
            return new byte[] { 0x62, 
                (byte)(slaverUID & 0xFF), (byte)((slaverUID >> 8) & 0xFF), (byte)((slaverUID >> 16) & 0xFF), (byte)(slaverUID >> 24)
            };
        }
	}

	/// <summary>
	/// GC Packet 0x63 - Inventory Item To Belt - Move an item (potion or scroll) directly from inventory to belt.
	/// </summary>
	public class InventoryItemToBelt : GCPacket
	{
		public readonly uint UID;

		public InventoryItemToBelt(byte[] data) : base(data)
		{
			this.UID = BitConverter.ToUInt32(data, 1);
        }
        public InventoryItemToBelt(uint uid) : base(InventoryItemToBelt.Build(uid))
        {
            this.UID = uid;
        }

        public static byte[] Build(uint uid)
        {
            return new byte[] { 0x63, 
                (byte)(uid & 0xFF), (byte)((uid >> 8) & 0xFF), (byte)((uid >> 16) & 0xFF), (byte)(uid >> 24)
            };
        }
	}
    
	/// <summary>
    /// GC Packet 0x66 - Warden Response - Sent in reply to GS 0xAE; Warden check's response.
	/// </summary>
	public class WardenResponse : GCPacket
	{
		public readonly int Length;

        public string Unknown3 { get { return ByteConverter.ToHexString(this.Data, 3); } }

		public WardenResponse(byte[] data) : base(data)
		{
            this.Length = BitConverter.ToUInt16(data, 1);
        }
	}
 
	/// <summary>
	/// GC Packet 0x68 - Game Logon Request - This is the first packet sent to D2GS, using info from RS (0x04 - JoinGame)
    ///	This is the main packet for the D2GS logon where you need to send your char infomation to the game server.
    ///	This packet must be 37 bytes long, so if your char name is not 15 letters in lengh, you will need to add the correct number of bytes after your Char name.
    ///	Your char name should be inbedded to the start of the following hex strand:
    ///	Code: 00000000B5D6779A81B36F4B00000000
    ///	The strand must be 16 bytes long and contain your character name with a null terminator.
    ///	The server will not answer this packet if it is not 37 bytes long, doesnt contain the D2GS hash or D2GS token or doesnt contain your char name.
	/// </summary>
	public class GameLogonRequest : GCPacket
	{
		public readonly uint Version;
		public readonly CharacterClass Class;
		public readonly string Name;
		public readonly uint D2GShash;
		public readonly ushort D2GSToken;
		
		public string Unknown12 { get { return ByteConverter.ToHexString(this.Data, 12, 4); } }
		public string Unknown16 { get { return ByteConverter.ToHexString(this.Data, 16, 4); } }
		public string Unknown20 { get { return ByteConverter.ToHexString(this.Data, 20, 1); } }

		public GameLogonRequest(byte[] data) : base(data)
		{
			this.D2GShash = BitConverter.ToUInt32(data, 1);
			this.D2GSToken = BitConverter.ToUInt16(data, 5);
			this.Class = (CharacterClass) data[7];
			this.Version = BitConverter.ToUInt32(data, 8);
			//12-15 - uint Unknown1
			//16-19 - uint Unknown2
			//20    - byte Unknown3
			this.Name = ByteConverter.GetNullString(data, 21);
		}
	}

	/// <summary>
	/// GC Packet 0x69 - Exit Game - Notify D2GS that you are leaving the game.
    /// NOTE: Upon receiving this packet, D2 will send a WM_CLOSE message to it's main window.
    /// If the "ExitGame" flag is not set first it will not just the leave the game, but close Diablo II !
    /// Use D2Instance.ExitGame instead...
	/// </summary>
	public class ExitGame : GCPacket
	{
		public ExitGame(byte[] data) : base(data)
		{
		}
        public ExitGame() : base(EnterGame.Build())
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x69 };
        }
	}

	/// <summary>
	/// GC Packet 0x6B - Enter Game - Sent after sucessful logon. Server will then send all the game's information.
	/// </summary>
	public class EnterGame : GCPacket
	{
		public EnterGame(byte[] data) : base(data)
		{
		}
        public EnterGame() : base(EnterGame.Build())
        {
        }

        public static byte[] Build()
        {
            return new byte[] { 0x6B };
        }
	}

	/// <summary>
	/// GC Packet 0x6D - Ping - Should be sent every 5 to 7 seconds to stop from dropping from the game.
	/// </summary>
	public class Ping : GCPacket
	{
		public readonly uint TickCount;

        public string Unknown5 { get { return ByteConverter.ToHexString(this.Data, 5, 8); } }
        /*
        *	The *Unknown should be a null value on joining the game server, and after that, a non-static value is used.
        *	Averying DWORD between 0x30 and 0x50 seems to be ok for a temp solution.
        */

		public Ping(byte[] data) : base(data)
		{
			this.TickCount = BitConverter.ToUInt32(data, 1);
		}
        public Ping(uint tickCount, long unknown5) : base(Ping.Build(tickCount, unknown5))
        {
            this.TickCount = tickCount;
        }

        public static byte[] Build(uint tickCount, long unknown5)
        {
            return new byte[] { 0x6D, 
                (byte)(tickCount & 0xFF), (byte)((tickCount >> 8) & 0xFF), (byte)((tickCount >> 16) & 0xFF), (byte)(tickCount >> 24),
                (byte)(unknown5 & 0xFF), (byte)((unknown5 >> 8) & 0xFF), (byte)((unknown5 >> 16) & 0xFF), (byte)((unknown5 >> 24) & 0xFF), 
                (byte)((unknown5 >> 32) & 0xFF), (byte)((unknown5 >> 40) & 0xFF), (byte)((unknown5 >> 48) & 0xFF), (byte)((unknown5 >> 56) & 0xFF)
            };
        }
	}
}