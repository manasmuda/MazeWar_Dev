using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [Header("Turning Speed of the turret")]
    public float turnSpeed;

    [Header("overlap Circle")]
    public bool isEntered = false;
    public float radius;
    public LayerMask playerLayer;
    public Vector3 Offset;

    [Header("Target to attack")]
    public Transform targetToAttack;

    [Header("Firing")]
    public bool canShoot = true;
    public float fireRate = 0.5f;
    public float bulletForce = 100f;
    public Transform bulletSpawnPosition;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        isEntered = Physics.CheckSphere(transform.position +Offset, radius, playerLayer);
        if (isEntered)
        {
            Debug.Log("Player entered the radius");
            Vector3 direction = (new Vector3(targetToAttack.position.x,this.transform.position.y,targetToAttack.position.z) - this.transform.position).normalized;
            Debug.DrawRay(this.transform.position, new Vector3(direction.x, 0f, direction.z), Color.white);
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            if (canShoot)
            {
                if (Vector3.Angle(direction, transform.forward) < 5f)
                    Fire();
                    canShoot = false;
                    Invoke("ChangeFire", fireRate);
            }
           


        }
        else {

            Debug.Log("player Not entered and  looking for the player");
        }
        

    }


    void ChangeFire()
    {
        canShoot = true;
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
        bullet.AddComponent<Rigidbody>();
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPosition.forward * bulletForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Offset, radius);

    }   

}
