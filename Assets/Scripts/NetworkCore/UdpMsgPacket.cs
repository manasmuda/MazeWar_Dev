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
    ClientState,
    GameState,
    Shoot
};

[Serializable]
public class UdpMsgPacket 
{
   public UdpMsgPacket(PacketType type,string message,string playerId,string team)
    {
        this.type = type;
        this.message = message;
        this.time=DateTime.Now.Millisecond;
        this.playerId = playerId;
        this.team = team;
    }

    public PacketType type { get; set; }
    public string message { get; set; }
    public string playerId { get; set; }
    public string team { get; set; }
    public int time { get; set; }
    public ClientState clientState { get; set; }
    public GameState gameState { get; set; }
}
