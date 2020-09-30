using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollowPlayer : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<newPlayer>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.position.x, this.transform.position.y, player.position.z);
    }
}
