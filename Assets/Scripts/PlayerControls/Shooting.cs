using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos,directionToGo;
    public ShooterButton shooterButton;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashSpawnPoint;

    [Header("Bullet Spawning Time")]
    public float coolDownTime = 0f;
    private float _coolDownTime = 0.5f;
    public float bulletForce = 300f;

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
      
        if (shooterButton.pressed)
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
                state.position = new float[3] { transform.position.x, transform.position.y, transform.position.z };
                state.angle = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z };
                //state.tick = clientScript.tick;
                state.playerId = MyData.playerId;
                state.team = MyData.team;
                SendShootMessage1(state);
                GameObject temp = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
                //temp.GetComponent<BulletScript>().SetClientState(state);
                temp.GetComponent<Rigidbody>().velocity = directionToGo.forward * bulletForce;
                GameObject _muzzleflash = Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint.position, Quaternion.Euler(0f, -90f, 0f));
                Destroy(_muzzleflash, 2f);
                //Destroy(temp, 50f);
            }
        }
        else
        {
            //Show no bullets or something
        }

        IEnumerator SendShootMessage(ClientState state)
        {
            float t = 0f;
            while (t < 0.07f)
            {
                if (state.bulletHit)
                {
                    clientScript.ShootAction(state);
                    break;
                }
                yield return new WaitForSeconds(0.02f);
                t = t + 0.02f;
                
            }
            if (t > 0.07f)
            {
                clientScript.ShootAction(state);
            }   
        }

        void SendShootMessage1(ClientState state)
        {
            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = bulletSpawnPos.position;
            ray.direction = directionToGo.forward;
            if(Physics.Raycast(ray,out hit))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Raycast Hit Player");
                    state.bulletHit = true;
                    state.bulletHitId = hit.collider.gameObject.GetComponent<CharacterData>().id;
                    state.bulletHitPosition= new float[3] { hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y, hit.collider.gameObject.transform.position.z };
                }
            }
            //clientScript.ShootAction(state);
        }
    }
}
