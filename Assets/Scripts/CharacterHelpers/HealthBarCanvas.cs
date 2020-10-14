using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OtherPlayerHealthBar
{
    public string playerName;
    public GameObject otherPlayer;
    public Slider healthBar;
}

public class HealthBarCanvas : MonoBehaviour
{
    public Camera PlayerCamera;
    public Text PlayerName;

  
    // Start is called before the first frame update
    void Start()
    {
        PlayerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        transform.LookAt(transform.position + PlayerCamera.transform.rotation * Vector3.forward, PlayerCamera.transform.rotation * Vector3.up);
    }

    void AddOtherPlayerHealth()
    {

    }

}
