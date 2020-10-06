using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private ClientState state;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClientState(ClientState state)
    {
        this.state = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Bullect Hit Player");
            //state.bulletHit = true;
            //state.bulletHitId = other.gameObject.GetComponent<CharacterData>().id;
            //state.bulletHitPosition = new float[3] { other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z };
        }
    }
}
