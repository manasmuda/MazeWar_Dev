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
        player = FindObjectOfType<NewPlayer>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        this.transform.localEulerAngles = new Vector3(90f,0f, -player.transform.localEulerAngles.y);
        this.transform.position = player.transform.position;
    }
}
