using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class crouch_Button : MonoBehaviour, IPointerClickHandler
{
    public bool isCrouched;
    public int hitTimes = 0;
    public static crouch_Button instance;

    // Start is called before the first frame update
    
      
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        isCrouched = false;
    }
   

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isCrouched&& hitTimes ==0)
        {
            isCrouched = true;
            hitTimes++;
        }
        else if (isCrouched&& hitTimes>0)
        {
            isCrouched = false;
            hitTimes = 0;
        }
        
    }
}
