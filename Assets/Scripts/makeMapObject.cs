using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeMapObject : MonoBehaviour
{
    public Image Image;

    // Start is called before the first frame update
    void Start()
    {
        MiniMapController.RegisterMapObject(this.gameObject, Image);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        MiniMapController.RemoveMapobject(this.gameObject);
    }
}
