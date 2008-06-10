using System;
using D2Data;
using D2Packets;
using ETUtils;

namespace BnetClient
{
    /// <summary>
	/// Base class for Battle.net Client Packets
	/// </summary>
	public class BCPacket : D2Packet
	{
		public readonly BnetClientPacket PacketID;

		public BCPacket(byte[] data) : base(data, Origin.BattleNetClient)
		{
			this.PacketID = (BnetClientPacket) data[1];
		}
	}

    /// <summary>
    /// BC 0x00
    /// </summary>
    public class KeepAlive : BCPacket
    {
        public KeepAlive(byte[] data)
            : base(data)
        {
        }
    }

    public class LeaveChat : BCPacket
    {
        public LeaveChat(byte[] data)
            : base(data)
        {

        }
    }


    /// <summary>
    /// BC Packet 0x15
    /// </summary>
    public class AdInfoRequest : BCPacket
    {
        public readonly  BattleNetClient Client;
        public readonly uint Id;
        public readonly BattleNetPlatform Platform;
        public readonly DateTime Timestamp;

        public AdInfoRequest(byte[] data)
            : base(data)
        {
            this.Platform = (BattleNetPlatform)BitConverter.ToUInt32(data, 3);
            this.Client = (BattleNetClient)BitConverter.ToUInt32(data, 7);
            this.Id = BitConverter.ToUInt32(data, 11);
            this.Timestamp = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 15));
        }

    }

    [Flags]
    public enum StartGameFlags
    {
        Public,
        Private
    }

 /// <summary>
 /// BC 0x1C
 /// </summary>
    public class StartGame : BCPacket
    {
        public readonly StartGameFlags flags;
        public readonly string name;
        public readonly string password;
        public readonly string statString;

        public StartGame(byte[] data)
            : base(data)
        {
            this.flags = (StartGameFlags)BitConverter.ToUInt32(data, 3);
            this.name = ByteConverter.GetNullString(data, 0x17);
            if ((this.flags & StartGameFlags.Private) == StartGameFlags.Private)
            {
                this.password = ByteConverter.GetNullString(data, 0x18 + this.name.Length);
            }
            int offset = (0x19 + this.name.Length) + ((this.password == null) ? 0 : this.password.Length);
            if (data.Length > (offset + 1))
            {
                this.statString = ByteConverter.GetNullString(data, offset);
            }
        }
    }


	/// <summary>
	/// BC Packet 0x1F - Leave Game - Notifies Battle.net user has left a game.
	/// </summary>
	public class LeaveGame : BCPacket
	{
		public LeaveGame(byte[] data) : base (data)
		{
		}
	}

    /// <summary>
    /// BC 0x21
    /// </summary>
    public class DisplayAd : BCPacket
    {
        public readonly BattleNetClient client;
        public readonly string filename;
        public readonly uint id;
        public readonly BattleNetPlatform platform;
        public readonly  string url;

        public DisplayAd(byte[] data)
            : base(data)
        {
            this.platform = (BattleNetPlatform)BitConverter.ToUInt32(data, 3);
            this.client = (BattleNetClient)BitConverter.ToUInt32(data, 7);
            this.id = BitConverter.ToUInt32(data, 11);
            if (data[15] != 0)
            {
                this.filename = ByteConverter.GetNullString(data, 15);
            }
            int index = 0x10 + ((this.filename == null) ? 0 : this.filename.Length);
            if (data[index] != 0)
            {
                this.url = ByteConverter.GetNullString(data, index);
            }
        }

    }

    /// <summary>
    /// BC 0x22
    /// </summary>
    public class NotifyJoin : BCPacket
    {
        public readonly BattleNetClient client;
        public readonly string name;
        public readonly string password;
        public readonly uint version;

        public NotifyJoin(byte[] data)
            : base(data)
        {
            this.client = (BattleNetClient)BitConverter.ToUInt32(data, 3);
            this.version = BitConverter.ToUInt32(data, 7);
            this.name = ByteConverter.GetNullString(data, 11);
            this.password = ByteConverter.GetNullString(data, 12 + this.name.Length);
        }
    }

    /// <summary>
    /// BC 0x25
    /// </summary>
    public class BnetPong : BCPacket
    {
        public readonly  uint timestamp;

        public BnetPong(byte[] data)
            : base(data)
        {
            this.timestamp = BitConverter.ToUInt32(data, 3);
        }
    }

    /// <summary>
    /// BC 0x33
    /// </summary>
    public class FileTimeRequest : BCPacket
    {
        public readonly string filename;
        public readonly uint requestID;
        public readonly uint unknown;


        public FileTimeRequest(byte[] data)
            : base(data)
        {
            this.requestID = BitConverter.ToUInt32(data, 3);
            this.unknown = BitConverter.ToUInt32(data, 7);
            this.filename = ByteConverter.GetNullString(data, 11);
        }
    }

    /// <summary>
    /// BC Packet 0x3A - BnetLogonRequest
    /// </summary>
    public class BnetLogonRequest : BCPacket
    {
        public readonly uint clientToken;
        public readonly uint serverToken;
        public readonly string username;

        public BnetLogonRequest(byte[] data)
            : base(data)
        {
            this.clientToken = BitConverter.ToUInt32(data, 3);
            this.serverToken = BitConverter.ToUInt32(data, 7);
            this.username = ByteConverter.GetNullString(data, 0x1f);
        }


    }

	/// <summary>
	/// BC Packet 0x3E - Realm Logon Request - Request Realm server information to establish a connection.
	/// </summary>
	public class RealmLogonRequest : BCPacket
	{
		public readonly string Realm;
		public readonly uint Cookie;

		public RealmLogonRequest(byte[] data) : base (data)
		{
			this.Cookie = BitConverter.ToUInt32(data, 3);
			// 5 * DWORD; hashed password ("password")
			this.Realm = ByteConverter.GetNullString(data, 27);
		}
	}

    /// <summary>
    /// BC 0x40
    /// </summary>
    public class QueryRealms : BCPacket
    {
        public QueryRealms(byte[] data)
            : base(data)
        {
        }
    }

    /// <summary>
    /// BC 0x46
    /// </summary>
    public class NewsInfoRequest : BCPacket
    {
        public readonly DateTime since;

        public NewsInfoRequest(byte[] data)
            : base(data)
        {
            this.since = TimeUtils.ParseUnixTimeUtc(BitConverter.ToUInt32(data, 3));
        }
    }

    /// <summary>
    /// BC 0x4B
    /// </summary>
    public class ExtraWorkResponse : BCPacket
    {
        public readonly  int client;
        public readonly int resultLength;

        public byte[] ResultData
        {
            get
            {
                byte[] destinationArray = new byte[this.resultLength];
                Array.Copy(base.Data, 7, destinationArray, 0, this.resultLength);
                return destinationArray;
            }
        }

        public ExtraWorkResponse(byte[] data)
            : base(data)
        {
            this.client = BitConverter.ToUInt16(data, 3);
            this.resultLength = BitConverter.ToUInt16(data, 5);
        }
    }

    /// <summary>
    /// BC Packet 0x50 - Bnet Connection Request - First packet sent to bnet to establish a connection.
    /// </summary>
    public class BnetConnectionRequest : BCPacket
    {
        public readonly BattleNetClient client;
        public readonly string countryAbbreviation;
        public readonly string countryName;
        public readonly static uint CurrentD2LoDVersion;
        public readonly static uint CurrentD2Version;
        public readonly uint language;
        public readonly uint languageID;
        public readonly uint localeID;
        public readonly System.Net.IPAddress localIP;
        public readonly BattleNetPlatform platform;
        public readonly uint protocol;
        public readonly uint timeZoneBias;
        public readonly uint version;

        public BnetConnectionRequest(byte[] data)
            : base(data)
        {
            this.protocol = BitConverter.ToUInt32(data, 3);
            this.platform = (BattleNetPlatform)BitConverter.ToUInt32(data, 7);
            this.client = (BattleNetClient)BitConverter.ToUInt32(data, 11);
            this.version = BitConverter.ToUInt32(data, 15);
            this.language = BitConverter.ToUInt32(data, 0x13);
            this.localIP = new System.Net.IPAddress((long)BitConverter.ToUInt32(data, 0x17));
            this.timeZoneBias = BitConverter.ToUInt32(data, 0x1b);
            this.localeID = BitConverter.ToUInt32(data, 0x1f);
            this.languageID = BitConverter.ToUInt32(data, 0x23);
            this.countryAbbreviation = ByteConverter.GetNullString(data, 0x27);
            this.countryName = ByteConverter.GetNullString(data, 40 + this.countryAbbreviation.Length);
        }
    }

    public struct CDKeyInfo
    {
        uint Length;
        uint ProductValue;
        uint PublicValue;
        uint Unknown;       // 0
        byte[] Hash;

        public CDKeyInfo(byte[] data, int offset)
        {
            this.Length = BitConverter.ToUInt32(data, offset);
            this.ProductValue = BitConverter.ToUInt32(data, offset + 4);
            this.PublicValue = BitConverter.ToUInt32(data, offset + 8);
            this.Unknown = BitConverter.ToUInt32(data, offset + 12);
            this.Hash = new byte[20];
            Array.Copy(data, offset + 16, this.Hash, 0, 20);
        }
    }

    /// <summary>
    /// BC Packet 0x51 - Bnet Auth Request - Second packet sent to bnet containing CDKey and other auth info.
    /// </summary>
    public class BnetAuthRequest : BCPacket
    {
        public readonly uint ClientToken;
        public readonly uint GameVersion;
        public readonly uint GameHash;
        public readonly uint KeyCount;      // 1 for classic, 2 for LOD...
        public readonly bool UseSpawn;      // Always false for Diablo...
        public readonly CDKeyInfo[] Keys;
        public readonly string GameInfo;
        public readonly string OwnerName;

        public BnetAuthRequest(byte[] data) : base(data)
        {
            this.ClientToken = BitConverter.ToUInt32(data, 3);
            this.GameVersion = BitConverter.ToUInt32(data, 7);
            this.GameHash = BitConverter.ToUInt32(data, 11);
            this.KeyCount = BitConverter.ToUInt32(data, 15);
            this.UseSpawn = BitConverter.ToUInt32(data, 19) == 1 ? true : false;
            this.Keys = new CDKeyInfo[this.KeyCount];
            int offset = 23;
            for (int i = 0; i < this.KeyCount; i++)
            {
                this.Keys[i] = new CDKeyInfo(data, offset);
                offset += 36;
            }
            this.GameInfo = ByteConverter.GetNullString(data, offset);
            this.OwnerName = ByteConverter.GetNullString(data, offset + this.GameInfo.Length + 1);
        }
    }

    /// <summary>
    /// BC 0x0a
    /// </summary>
    public class EnterChatRequest : BCPacket
    {
        public readonly string name;
        public readonly string realm;

        public EnterChatRequest(byte[] data)
            : base(data)
        {
            this.name = ByteConverter.GetNullString(data, 3);
            this.realm = ByteConverter.GetString(data, 4 + this.name.Length, -1, 0x2c);
        }
    }
    
    /// <summary>
    /// BC 0x0B
    /// </summary>
    public class ChannelListRequest : BCPacket
    {
        public readonly  BattleNetClient client;

        public ChannelListRequest(byte[] data)
            : base(data)
        {
            this.client = (BattleNetClient)BitConverter.ToUInt32(data, 3);
        }

    }

    public enum JoinChannelFlags
    {
        AutoJoin = 5,
        Create = 2,
        NormalJoin = 0
    }

    /// <summary>
    /// BC 0x0C
    /// </summary>
    public class JoinChannel : BCPacket
    {
        public readonly JoinChannelFlags flags;
        public readonly string name;

        public JoinChannel(byte[] data)
            : base(data)
        {
            this.flags = (JoinChannelFlags)BitConverter.ToUInt32(data, 3);
            this.name = ByteConverter.GetNullString(data, 7);
        }
    }

    /// <summary>
    /// BC 0x0E
    /// </summary>
    public class ChatCommand : BCPacket
    {
        public readonly string message;

        public ChatCommand(byte[] data)
            : base(data)
        {
            this.message = ByteConverter.GetNullString(data, 3);
        }

 


    }

}

