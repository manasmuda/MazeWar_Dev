﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public enum PacketType
{
    UDPConnect,
    ServerTick,
    Spawn,
};

[Serializable]
public class UdpMsgPacket 
{
   public UdpMsgPacket(PacketType type,string message)
    {
        this.type = type;
        this.message = message;
        this.time=DateTime.UtcNow.Millisecond;
    }

    public PacketType type { get; set; }
    public string message { get; set; }
    public int time { get; set; }
}
