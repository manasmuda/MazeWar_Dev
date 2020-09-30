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
    public float coolDownTime = 0.5f;
    private float _coolDownTime;
    public float bulletForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
        _coolDownTime = coolDownTime;
        shooterButton = FindObjectOfType<ShooterButton>();
    }

    // Update is called once per frame
    void Update()

    {
        if (shooterButton.pressed)
        {
            //if the player not moving and standing for shoot
            if (coolDownTime <= 0f)
            {
                coolDownTime = _coolDownTime;
                Shoot();             
            }
        }
        
        coolDownTime -= Time.deltaTime;
      
    }

    public void Shoot()
    {
        Debug.Log("Shooting");
        if (shooterButton.pressed)
        {
           GameObject temp = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
            temp.AddComponent<Rigidbody>();
            temp.GetComponent<Rigidbody>().AddForce(directionToGo.forward * bulletForce, ForceMode.Impulse);
            GameObject _muzzleflash = Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint.position, Quaternion.Euler(0f, -90f, 0f));
            Destroy(_muzzleflash, 2f);
            Destroy(temp, 5f);
        }


    }
   
        
}
