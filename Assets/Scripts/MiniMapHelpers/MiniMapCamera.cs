using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
    public class ListOfOtherPlayers {

        public GameObject Pointer;
        public GameObject otherPlayer;
    }

public class MiniMapCamera : MonoBehaviour
{

    public GameObject pointer;

    public Transform player;

    public List<ListOfOtherPlayers> _listOfPlayers = new List<ListOfOtherPlayers>();

    public float miniMapSize;
 
    void Start()
    {
        /*for (int i = 0; i < count; i++)
        {

            _listOfPlayers.Add(new ListOfOtherPlayers());
            GameObject temp = Instantiate(pointer);
            _listOfPlayers[i].Pointer = temp;
            // _listOfPlayers[i].otherPlayer = GameObject.FindObjectOfType<CharacterSyncScript>().gameObject;
            //_listOfPlayers[i].Pointer.transform.position = _listOfPlayers[i].otherPlayer.transform.position;
        }*/
    }

    public void AddPlayer(GameObject player)
    {
        GameObject temp = Instantiate(pointer);
        ListOfOtherPlayers otherPlayer = new ListOfOtherPlayers();
        otherPlayer.otherPlayer = player;
        otherPlayer.Pointer = temp;
        _listOfPlayers.Add(otherPlayer);
    }

    // Update is called once per frame
    void Update()
    {


        foreach (ListOfOtherPlayers list in _listOfPlayers)
        {
            Vector3 pointerPos = new Vector3(list.otherPlayer.transform.position.x, 10f, list.otherPlayer.transform.position.z);

           

            list.Pointer.transform.position = 

            new Vector3(Mathf.Clamp(pointerPos.x, this.transform.position.x - miniMapSize, miniMapSize + this.transform.position.x),
            pointerPos.y,
            Mathf.Clamp(pointerPos.z, this.transform.position.z - miniMapSize, miniMapSize + this.transform.position.z));
            
        }

    }



}
