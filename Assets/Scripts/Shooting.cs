﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos,directionToGo;
    public ShooterButton shooterButton;
    public ParticleSystem muzzleFlash;

  
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
            muzzleFlash.Play();
            Destroy(temp, 5f);
        }


    }
   
        
}
