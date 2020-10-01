using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    public Transform player;
    public Transform minimapCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        this.transform.localEulerAngles = new Vector3(90f,0f, -player.transform.localEulerAngles.y);
        this.transform.position = new Vector3(player.position.x,8f,player.position.z);
    }
}
