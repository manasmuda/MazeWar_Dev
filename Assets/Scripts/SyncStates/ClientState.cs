﻿using System;
using System.Collections.Generic;

[Serializable]
public class ClientState
{
    public int tick = 0;

    public string playerId;
    public string team;

    public float[] position = new float[3];
    public float[] angle = new float[3];

    public bool crouch;

    public String name;
    public int health;
    public int coinsHolding;
    public int tracersRemaining;
    public int bulletsLeft;

    public bool bulletHit;
    public string bulletHitId;
    public float[] bulletHitPosition;

    public int reloadStartTick;

    public int stateType;
    public bool movementPressed;
}

