using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos,directionToGo;
    public ShooterButton shooterButton;
    public ParticleSystem muzzleFlash;

  
    public float coolDownTime = 0f;
    private float _coolDownTime = 0.5f;
    public float bulletForce = 100f;
    public bool shooting = false;
    public int loadedBullets = 10;

    private Client clientScript;

    // Start is called before the first frame update
    void Start()
    {
        shooterButton = FindObjectOfType<ShooterButton>();
        clientScript = GameObject.Find("Client").GetComponent<Client>();
    }

    // Update is called once per frame
    void Update()

    {
        if (shooterButton.pressed && newPlayer.playerController_instance.moveInput.x != 0 && newPlayer.playerController_instance.moveInput.y != 0|| crouch_Button.instance.isCrouched)
        {
            //if the player not moving and standing for shoot
            if (!shooting)
            {
                shooting = true;
                coolDownTime = _coolDownTime;
                Shoot();             
            }
        }

        if (shooting)
        {
            coolDownTime -= Time.deltaTime;
            if (coolDownTime <= 0f)
            {
                shooting = false;
            }
        }
    }

    public void Shoot()
    {
        Debug.Log("Shooting");
        if (loadedBullets > 0)
        {
            loadedBullets--;
            if (loadedBullets < 10)
            {
                //enable reload button
            } 
            if (shooterButton.pressed)
            {
                ClientState state = new ClientState();
                state.bulletsLeft = loadedBullets;
                state.position = new float[3]{transform.position.x,transform.position.y,transform.position.z};
                state.angle = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z };
                state.tick = clientScript.tick;
                state.playerId = MyData.playerId;
                state.team = MyData.team;
                clientScript.ShootAction(state);
                GameObject temp = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
                temp.AddComponent<Rigidbody>();
                temp.GetComponent<Rigidbody>().AddForce(directionToGo.forward * bulletForce, ForceMode.Impulse);
                muzzleFlash.Play();
                Destroy(temp, 5f);
            }
        }
        else
        {
            //Show no bullets or something
        }

    }
        
}
