using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShooterButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public bool pressed = false;
    public int hitTimes = -1; // bullet is shot for every time hitTime is 0


  
  
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {





        if (pressed)
        {
          
            hitTimes = (hitTimes + 1) % 25;

        }

        else
        {
            hitTimes = -1;    
        }
          
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
     
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
  
}