using System;
using D2Data;
using D2Packets;
using ETUtils;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BnetServer
{
	/// <summary>
	/// Base class for Battle.net Server Packets
	/// </summary>
	public class BSPacket : D2Packet
	{
		public readonly BnetServerPacket PacketID;

		public BSPacket(byte[] data) : base(data, Origin.BattleNetServer)
		{
			this.PacketID = (BnetServerPacket) data[0];
		}
	}

    /// <summary>
    /// Bs 0x00
    /// </summary>
    public class KeepAlive : BSPacket
    {
        public KeepAlive(byte[] data)
            : base(data)
        {
        }
    }

    /// <summary>
    /// BS 0x0A
    /// </summary>
    public class EnterChatResponse : BSPacket
    {
        public readonly string account;
        public readonly int characterAct;
        public readonly CharacterFlags characterFlags;
        public readonly int characterLevel;
        public readonly CharacterTitle characterTitle;
        public readonly D2Data.BattleNetCharacter characterType;
        public readonly BattleNetClient client;
        public readonly int clientVersion;
        public readonly string name;
        public readonly string realm;
        public readonly string username;

        public EnterChatResponse(byte[] data)
            : base(data)
        {
            this.clientVersion = -1;
            this.characterType = D2Data.BattleNetCharacter.Unknown;
            this.characterLevel = -1;
            this.characterAct = -1;
            this.characterTitle = CharacterTitle.None;
            this.username = ByteConverter.GetNullString(data, 3);
            int startIndex = 4 + this.username.Length;
            this.client = (BattleNetClient)BitConverter.ToUInt32(data, startIndex);
            if (data[startIndex += 4] == 0)
            {
                this.account = ByteConverter.GetNullString(data, startIndex + 1);
            }
            else
            {
                this.realm = ByteConverter.GetString(data, startIndex, -1, 0x2c);
                startIndex += 1 + this.realm.Length;
                this.name = ByteConverter.GetString(data, startIndex, -1, 0x2c);
                startIndex += 1 + this.name.Length;
                int num2 = ByteConverter.GetByteOffset(data, 0, startIndex);
                this.account = ByteConverter.GetNullString(data, (startIndex + num2) + 1);
                if (this.client == BattleNetClient.Diablo2LoD)
                {
                    this.characterFlags |= CharacterFlags.Expansion;
                }
                StatString.ParseD2StatString(data, startIndex, ref this.clientVersion, ref this.characterType, ref this.characterLevel, ref this.characterFlags, ref this.characterAct, ref this.characterTitle);
            }
        }
    }

    /// <summary>
    /// BS 0x0B
    /// </summary>
    public class ChannelList : BSPacket
    {
        public readonly List<string> channels;

        public ChannelList(byte[] data)
            : base(data)
        {
            this.channels = new List<string>();
            int offset = 3;
            for (int i = 0; offset < (data.Length - 1); i++)
            {
                this.channels.Add(ByteConverter.GetNullString(data, offset));
                offset += this.channels[i].Length + 1;
            }
        }
    }


	[Flags]
	public enum ChannelFlags
	{
		PublicChannel = 0x01,
		Moderated = 0x02,
		Restricted = 0x04,
		Silent = 0x08,
		System = 0x10,
		ProductSpecific = 0x20,
		GloballyAccessible = 0x1000,
	}

	[Flags]
	public enum BattleNetFlags : uint
	{
		BlizzardRepresentative = 0x01,
		ChannelOperator = 0x02,
		Speaker = 0x04,
		BattleNetAdministrator = 0x08,	
		NoUDPSupport = 0x10, 	
		Squelched = 0x20, 	
		SpecialGuest = 0x40,
		PGLOfficial = 0x400,			// (Defunct) - probably wrong... 0x4000000 ?
		WCGOfficial = 0x1000, 	
		KBKSingles = 0x2000,
		KBKPlayer = 0x8000,
		KBKBeginner = 0x10000,
		WhiteKBK = 0x20000,				// 1 orange bar icon
		GameRoom = 0x40000,
		GFOfficial = 0x100000, 	
		GFPlayer = 0x200000, 	
		PGLPlayer = 0x2000000, 	
	}

	public enum BattleNetClient
	{
		DiabloShareware = 0,
		Diablo = 1,
		Diablo2 = 2,
		Diablo2LoD = 3,
		Warcraft2 = 4,
		Warcraft3,
		Warcraft3FrozenThrone,
		StarcraftShareware,
		Starcraft,
		StarcraftBroodWar,
		ChatBot,
		Unknown,
	}

	public enum BattleNetCharacter
	{
		// Diablo 2
		Amazon,
		Sorceress,
		Necromancer,
		Paladin,
		Barbarian,
		// Diablo 2 LoD
		Druid,
		Assassin,
		
		OpenCharacter,

		// Diablo 1
		Warrior,
		Rogue,
		Sorcerer,

		Unknown1,
		Unknown2Grey,
		Unknown3Grey,

		StarcraftMarine,
		BroodWarMedic,
		WarcraftIIGrunt,
		BlizzardRep,
		Moderator,
		Sysop,
		Referee,
		Chat,
		Speaker,
		Unknown,
	}

	public enum ChatEventType : uint
	{
		ChannelUser			= 1,	// Received for every user when you join a channel. Also sent when logged on using D2XP/D2DV and a user requires an update to their statstring e.g. by logging a different character onto a realm !?
		ChannelJoin			= 2,	// Received when someone joins the channel you're currently in
		ChannelLeave		= 3,	// Received when someone leaves the channel you're currently in
		ReceiveWhisper		= 4,	// Receive whisper message
		ChannelMessage		= 5,	// Received when someone talks in the channel you're currently in
		ServerBroadcast		= 6,	// Server broadcast
		ChannelInfo			= 7,	// Received when you join a channel (channel's name and flags)
		UserFlags			= 9,	// Update a user's flags.
		WhisperSent			= 0x0a,	// Sent whisper message receipt
		ChannelFull			= 0x0d,
		ChannelDoesNotExist	= 0x0e,
		ChannelRestricted	= 0x0f,
		Broadcast			= 0x12,	// Account is the one you are logged on. Used to be the BattleNetAdministrator who sent the message.
		Error				= 0x13,
		Emote				= 0x14,
		//EID_UNIQUENAME      2010
	}

	/// <summary>
	/// BS Packet 0x0F - Chat Event - Various chat related events (chat join, message, whisper, etc)
	/// </summary>
    public class ChatEvent : BSPacket
    {
        public readonly string account;
        public readonly int characterAct;
        public readonly CharacterFlags characterFlags;
        public readonly int characterLevel;
        public readonly CharacterTitle characterTitle;
        public readonly D2Data.BattleNetCharacter characterType;
        public readonly BattleNetClient client;
        public readonly int clientVersion;
        public readonly ChatEventType eventType;
        public readonly uint flags;
        public readonly string message;
        public readonly string name;
        public readonly int NULL_Int32;
        public readonly int NULL_UInt32;
        public readonly uint ping;
        public readonly string realm;

        
        public ChatEvent(byte[] data) : base(data)
        {
            this.clientVersion = -1;
            this.characterType = D2Data.BattleNetCharacter.Unknown;
            this.characterLevel = -1;
            this.characterAct = -1;
            this.characterTitle = CharacterTitle.None;
            this.eventType = (ChatEventType) BitConverter.ToUInt32(data, 3);
            this.flags = BitConverter.ToUInt32(data, 7);
            this.ping = BitConverter.ToUInt32(data, 11);
            int length = ByteConverter.GetByteOffset(data, 0, 0x1b);
            int num2 = ByteConverter.GetByteOffset(data, 0x2a, 0x1b, length);
            if (num2 > 0)
            {
                this.name = ByteConverter.GetString(data, 0x1b, num2);
                length -= num2 + 1;
                num2 += 0x1c;
            }
            else if (num2 == 0)
            {
                num2 = 0x1c;
                length--;
                this.characterType = D2Data.BattleNetCharacter.OpenCharacter;
            }
            else
            {
                num2 = 0x1b;
            }
            this.account = ByteConverter.GetString(data, num2, length);
            length += num2 + 1;
            if (this.eventType != ChatEventType.ChannelLeave)
            {
                if ((this.eventType == ChatEventType.ChannelJoin) || (this.eventType == ChatEventType.ChannelUser))
                {
                    if ((data.Length - length) > 3)
                    {
                        this.client = (BattleNetClient) BitConverter.ToUInt32(data, length);
                        length += 4;
                    }
                    if ((((this.client != BattleNetClient.StarcraftShareware) && (this.client != BattleNetClient.Starcraft)) && (this.client != BattleNetClient.StarcraftBroodWar)) && ((this.client == BattleNetClient.Diablo2) || (this.client == BattleNetClient.Diablo2LoD)))
                    {
                        if (this.client == BattleNetClient.Diablo2LoD)
                        {
                            this.characterFlags |= CharacterFlags.Expansion;
                        }
                        if ((data.Length - length) >= 4)
                        {
                            this.realm = ByteConverter.GetString(data, length, -1, 0x2c);
                            length += this.realm.Length + 1;
                            if (data.Length >= length)
                            {
                                length += ByteConverter.GetByteOffset(data, 0x2c, length) + 1;
                                if (((length != -1) && (data.Length > length)) && ((data.Length - length) >= 0x21))
                                {
                                    StatString.ParseD2StatString(data, length, ref this.clientVersion, ref this.characterType, ref this.characterLevel, ref this.characterFlags, ref this.characterAct, ref this.characterTitle);
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.message = ByteConverter.GetNullString(data, length);
                }
            }
        }
    }

    /// <summary>
    /// BS 0x15
    /// </summary>
    public class AdInfo : BSPacket
    {
        public readonly string extension;
        public readonly string filename;
        public readonly uint id;
        public readonly DateTime timestamp;
        public readonly string url;

        public AdInfo(byte[] data)
            : base(data)
        {
            this.id = BitConverter.ToUInt32(data, 3);
            this.extension = ByteConverter.GetString(data, 7, 4);
            this.timestamp = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 11));
            this.filename = ByteConverter.GetNullString(data, 0x13);
            this.url = ByteConverter.GetNullString(data, 20 + this.filename.Length);
        }
    }

    /// <summary>
    /// BS 0x25
    /// </summary>
    public class BnetPing : BSPacket
    {
        public readonly uint timestamp;

        public BnetPing(byte[] data)
            : base(data)
        {
            this.timestamp = BitConverter.ToUInt32(data, 3);
        }
    }

    /// <summary>
    /// Bs 0x33
    /// </summary>
    public class FileTimeInfo : BSPacket
    {
        public readonly string filename;
        public readonly DateTime filetime;
        public readonly uint requestID;
        public readonly uint unknown;

        public FileTimeInfo(byte[] data)
            : base(data)
        {
            this.requestID = BitConverter.ToUInt32(data, 3);
            this.unknown = BitConverter.ToUInt32(data, 7);
            this.filetime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 11));
            this.filename = ByteConverter.GetNullString(data, 0x13);
        }


    }

    public enum BnetLogonResult
    {
        Success,
        AccountDoesNotExist,
        PasswordIncorrect
    }

    /// <summary>
    /// BS 0x3A
    /// </summary>
    public class BnetLogonResponse : BSPacket
    {
        public readonly string reason;
        public readonly BnetLogonResult result;

        public BnetLogonResponse(byte[] data)
            : base(data)
        {
            this.result = (BnetLogonResult)BitConverter.ToUInt32(data, 3);
            if (data.Length > 7)
            {
                this.reason = ByteConverter.GetNullString(data, 7);
            }
        }
    }


    public enum RealmLogonResult : uint
    {
        LogonFailed = 0x80000002,
        RealmUnavailable = 0x80000001,
        Success = 0
    }

	/// <summary>
	/// BS Packet 0x3E - Realm Logon Response - Supplies the data neccessary to connect to an MCP (Realm) server.
	/// </summary>
    public class RealmLogonResponse : BSPacket
    {
        // Fields
        protected uint cookie;
        public static readonly int NULL_Int32 = -1;
        protected System.Net.IPAddress realmServerIP;
        protected int realmServerPort;
        protected RealmLogonResult result;
        protected ushort unknown;
        protected string username;

        // Methods
        public RealmLogonResponse(byte[] data)
            : base(data)
        {
            this.realmServerPort = -1;
            this.cookie = BitConverter.ToUInt32(data, 3);
            if (base.Data.Length < 0x4a)
            {
                this.result = (RealmLogonResult)BitConverter.ToUInt32(data, 7);
            }
            else
            {
                this.result = RealmLogonResult.Success;
                this.realmServerIP = new System.Net.IPAddress((long)BitConverter.ToUInt32(data, 0x13));
                this.realmServerPort = BEBitConverter.ToUInt16(data, 0x17);
                this.username = ByteConverter.GetNullString(data, 0x4b);
                this.unknown = BitConverter.ToUInt16(data, 0x4c + this.username.Length);
            }
        }

        public bool CompareStartupData(RealmClient.RealmStartupRequest realmStartup)
        {
            return realmStartup.CompareStartupData(this);
        }

        public bool CompareStartupData(byte[] bytes)
        {
            return this.CompareStartupData(bytes, 0);
        }

        public bool CompareStartupData(byte[] bytes, int offset)
        {
            for (int i = 0; i < 0x10; i++)
            {
                if (base.Data[i + 3] != bytes[i + offset])
                {
                    return false;
                }
            }
            offset += 0x10;
            for (int j = 0; j < 0x30; j++)
            {
                if (base.Data[j + 0x1b] != bytes[j + offset])
                {
                    return false;
                }
            }
            return true;
        }

        // Properties
        public uint Cookie
        {
            get
            {
                return this.cookie;
            }
        }

        public System.Net.IPAddress RealmServerIP
        {
            get
            {
                return this.realmServerIP;
            }
        }

        public int RealmServerPort
        {
            get
            {
                return this.realmServerPort;
            }
        }

        public RealmLogonResult Result
        {
            get
            {
                return this.result;
            }
        }

        public byte[] StartupData
        {
            get
            {
                if (this.result != RealmLogonResult.Success)
                {
                    return null;
                }
                byte[] destinationArray = new byte[0x40];
                Array.Copy(base.Data, 3, destinationArray, 0, 0x10);
                Array.Copy(base.Data, 0x1b, destinationArray, 0x10, 0x30);
                return destinationArray;
            }
        }

        public ushort Unknown
        {
            get
            {
                return this.unknown;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
        }
    }

    /// <summary>
    /// BS 0x40
    /// </summary>
    public class QueryRealmsResponse : BSPacket
    {
        // Fields
        protected uint count;
        protected RealmInfo[] realms;
        protected uint unknown;

        // Methods
        public QueryRealmsResponse(byte[] data)
            : base(data)
        {
            this.unknown = BitConverter.ToUInt32(data, 3);
            this.count = BitConverter.ToUInt32(data, 7);
            this.realms = new RealmInfo[this.count];
            int offset = 11;
            for (int i = 0; i < this.count; i++)
            {
                this.realms[i] = new RealmInfo(data, offset);
                offset += (6 + this.realms[i].Name.Length) + this.realms[i].Description.Length;
            }
        }

        // Properties
        public uint Count
        {
            get
            {
                return this.count;
            }
        }

        public RealmInfo[] Realms
        {
            get
            {
                return this.realms;
            }
        }

        public uint Unknown
        {
            get
            {
                return this.unknown;
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NewsEntry
    {
        private DateTime timestamp;
        private string content;
        public NewsEntry(byte[] data, int offset)
        {
            this.timestamp = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, offset));
            this.content = ByteConverter.GetNullString(data, offset + 4);
        }

        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }
        public string Content
        {
            get
            {
                return this.content;
            }
        }
        public override string ToString()
        {
            return string.Format("Timestamp: {0}, Content: {1}", this.Timestamp, this.Content);
        }
    }

    /// <summary>
    /// BS 0x46
    /// </summary>
    public class NewsInfo : BSPacket
    {
        public readonly int count;
        public readonly NewsEntry[] entries;
        public readonly DateTime lastLogon;
        public readonly DateTime newestEntry;
        public readonly DateTime oldestEntry;

        public NewsInfo(byte[] data)
            : base(data)
        {
            this.count = data[3];
            this.lastLogon = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 4));
            this.oldestEntry = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 8));
            this.newestEntry = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 12));
            this.entries = new NewsEntry[this.count];
            int offset = 0x10;
            for (int i = 0; i < this.entries.Length; i++)
            {
                this.entries[i] = new NewsEntry(data, offset);
                offset += 5 + this.entries[i].Content.Length;
            }
        }
    }

    /// <summary>
    /// Bs Packet 0x4A
    /// </summary>
    public class ExtraWorkInfo : BSPacket
    {
        public readonly string filename;

        public ExtraWorkInfo(byte[] data)
            : base(data)
        {
            this.filename = ByteConverter.GetNullString(data, 3);
        }
    }

    /// <summary>
    /// BS 0x4C
    /// </summary>
    public class RequiredExtraWorkInfo : BSPacket
    {
        // Fields
        protected string filename;

        // Methods
        public RequiredExtraWorkInfo(byte[] data)
            : base(data)
        {
            this.filename = ByteConverter.GetNullString(data, 3);
        }

        // Properties
        public string Filename
        {
            get
            {
                return this.filename;
            }
        }
    }



    /// <summary>
    /// BS 0x50
    /// </summary>
    public class BnetConnectionResponse : BSPacket
    {
        public readonly uint logonType;
        public readonly uint serverToken;
        public readonly uint udpValue;
        public readonly string versionFileName;
        public readonly DateTime versionFileTime;
        public readonly string versionFormulae;

        public BnetConnectionResponse(byte[] data)
            : base(data)
        {
            this.logonType = BitConverter.ToUInt32(data, 3);
            this.serverToken = BitConverter.ToUInt32(data, 7);
            this.udpValue = BitConverter.ToUInt32(data, 11);
            this.versionFileTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 15));
            this.versionFileName = ByteConverter.GetNullString(data, 0x17);
            this.versionFormulae = ByteConverter.GetNullString(data, 0x18 + this.versionFileName.Length);
        }
    }


    public enum BnetAuthResult
    {
        BannedCDKey = 0x202,
        BuggedVersion = 0x102,
        CDKeyInUse = 0x201,
        InvalidCDKey = 0x200,
        InvalidVersion = 0x101,
        OldVersion = 0x100,
        Success = 0,
        WrongProduct = 0x203
    }

    /// <summary>
    /// BS 0x51
    /// </summary>
    public class BnetAuthResponse : BSPacket
    {
        public readonly string info;
        public readonly BnetAuthResult result;

        public BnetAuthResponse(byte[] data)
            : base(data)
        {
            this.result = (BnetAuthResult)BitConverter.ToUInt32(data, 3);
            if (data.Length > 8)
            {
                this.info = ByteConverter.GetNullString(data, 7);
            }
        }
    }



}
