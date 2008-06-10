using System;

namespace D2Packets
{
	public enum BnetServerPacket : byte
	{
        KeepAlive           = 0x00,
        EnterChatResponse   = 0x0A,
        ChannelList         = 0x0B,
		ChatEvent			= 0x0F,
        AdInfo              = 0x15,
        BnetPing            = 0x25,
        FileTimeInfo        = 0x33,
        BnetLogonResponse   = 0x3A,
        RealmLogonResponse  = 0x3E,
        QueryRealmsResponse = 0x40,
        NewsInfo            = 0x46,
        ExtraWorkInfo       = 0x4A,
        RequiredExtraWorkInfo   = 0x4C,
        BnetConnectionResponse  = 0x50,
        BnetAuthResponse    = 0x51,
		Invalid				= 0xA0	// Probably 0x83, leaving some space for safety...
	}
}
