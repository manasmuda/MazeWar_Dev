using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos,directionToGo;
    public ShooterButton shooterButton;

    public float coolDownTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()

    {
        if (shooterButton.pressed)
        {
            if (coolDownTime <= 0f)
            {
                coolDownTime = 0.5f;
                Shoot();
            }
           
           
        }
        coolDownTime -= Time.deltaTime;
    }

    private void Shoot()
    {
        Debug.Log("Shooting");

        GameObject temp = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);

        temp.AddComponent<Rigidbody>();

        temp.GetComponent<Rigidbody>().AddForce(directionToGo.forward * 100f, ForceMode.Impulse);

        Destroy(temp, 5f);
    }
}
