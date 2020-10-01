using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MapObject
{
    public Image icon { get; set; }
    public GameObject owner { get; set; }
}



public class MiniMapController : MonoBehaviour
{
    public Transform player;
    public Camera minimapCam;
    public Canvas canvas;
    public static List<MapObject> mapObjects = new List<MapObject>();


    public static void RegisterMapObject(GameObject gameobject, Image image)
    {
        Image i = Instantiate(image);
        mapObjects.Add(new MapObject() { owner = gameobject, icon = i });
    }
    public static void RemoveMapobject(GameObject gameObject)
    {
        List<MapObject> newList = new List<MapObject>();
        for (int i = 0; i < mapObjects.Count; i++)
        {
            if (mapObjects[i].owner == gameObject)
            {
                Destroy(mapObjects[i].icon);
                continue;
            }
            else
                newList.Add(mapObjects[i]);
        }
        mapObjects.RemoveRange(0, mapObjects.Count);
        mapObjects.AddRange(newList);
    }

    public List<GameObject> otherplayer = new List<GameObject>();
    public List<GameObject> pointer = new List<GameObject>();
    public GameObject[] otherPlayerPrefab;

    void DrawMapIcons()
    {
        foreach (MapObject MO in mapObjects)
        {
            Vector2 mop = new Vector2(MO.owner.transform.position.x, MO.owner.transform.position.y);
            Vector2 playerPos = new Vector2(player.position.x, player.position.y);

            if (Vector2.Distance(mop, playerPos) > 200f)
            {
                MO.icon.enabled = false;
                continue;
            }
            else
                MO.icon.enabled = true;


            Vector3 screenPos = minimapCam.WorldToViewportPoint(MO.owner.transform.position);
            MO.icon.transform.SetParent(this.transform);

            RectTransform rt = this.GetComponent<RectTransform>();
            Debug.Log(rt);
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            screenPos.x = Mathf.Clamp(screenPos.x *  rt.rect.width   + corners[0].x, corners[0].x, corners[2].x);
            screenPos.y = Mathf.Clamp(screenPos.y * rt.rect.height + corners[0].y, corners[0].y, corners[1].y);

            screenPos.z = 0;
            MO.icon.transform.position = screenPos;

        }
    }


    private void Start()
    {
       
      
        for (int i = 0; i < otherPlayerPrefab.Length; i++)
        {

            otherplayer.Add(otherPlayerPrefab[i]);
            
        }

    }
    private void Update()
    {
        DrawMapIcons();
    }

}
