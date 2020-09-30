using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    public Transform player;
    public Camera minimapCam;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<newPlayer>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        this.transform.localEulerAngles = new Vector3(0f,0f, -player.transform.localEulerAngles.y);
    }
}
