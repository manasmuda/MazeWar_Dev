using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameState 
{
    public int tick;
    public int stateType;

    public List<ClientState> blueTeamState = new List<ClientState>();
    public List<ClientState> redTeamState = new List<ClientState>();

}
