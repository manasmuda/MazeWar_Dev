using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{

    public Transform player;
    public GameObject PointerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<newPlayer>().transform;
        PointerPrefab = GameObject.FindWithTag("PlayerPointer");


       // if (blueColor)
       // check which team the player belong to and change the color of the sprite.
      //  {
             PointerPrefab.GetComponent<SpriteRenderer>().color = Color.blue;
     //   }
        /* else
         {
           PointerPrefab.GetComponent<SpriteRenderer>().color = Color.red;
        } */


    }


    private void LateUpdate()
    {
       // To place the pointer above the player
        Vector3 _pointerPosition = player.position;
        _pointerPosition.y = player.position.y + 6f;
        PointerPrefab.transform.position = _pointerPosition;
      

        // pointer moving along the player
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

    }
}
