using System;
using System.Collections.Generic;

[Serializable]
public class ClientState
{
    public int tick = 0;

    public string playerId;

    public float[] position = new float[3];
    public float[] angle = new float[3];

    public String name;
    public int health;
    public int coinsHolding;
    public int tracersRemaining;
    public int bulletsLeft;

    public int reloadStartTick;

    public int stateType;
    public bool movementPressed;
}

