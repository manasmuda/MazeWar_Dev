using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppTeamData 
{
    public static string teamName;
    public static Dictionary<string, GameObject> playerData=new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> gadgetObjects = new Dictionary<string, GameObject>();

    public static GameObject charPrefab;
    public static Quaternion spwanDirection;
}
