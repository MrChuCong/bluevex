using System;

namespace D2Packets
{
	public enum BnetClientPacket : byte
	{
        KeepAlive               = 0x00,
        LeaveChat               = 0x10,
        AdInfoRequest           = 0x15,
        StartGame               = 0x1C,
		LeaveGame			    = 0x1F,
        DisplayAd               = 0x21,
        NotifyJoin              = 0x22,
        BnetPong                = 0x25,
        FileTimeRequest         = 0x33,
        BnetLogonRequest        = 0x3A,
        RealmLogonRequest       = 0x3E,
        QueryRealms             = 0x40,
        NewsInfoRequest         = 0x46,
        ExtraWorkResponse       = 0x4B,
        BnetConnectionRequest   = 0x50,
        BnetAuthRequest         = 0x51,
		Invalid				    = 0xA0,	// Probably 0x83, leaving some space for safety...
        EnterChatRequest        = 0x0A,
        ChannelListRequest      = 0x0B,
        JoinChannel             = 0x0C,
        ChatCommand             = 0x0E
    
    }
}
