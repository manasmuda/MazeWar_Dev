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
    
    [Header("Target to attack")]

  
   public Transform targetToAttack = null;
  //  public Transform currenttarget = null;

    [Header("Firing")]
    public bool canShoot = true;
    private bool inSight = false;
   
    public float fireRate = 0.5f;
    public float bulletForce = 100f;
    public Transform  bulletSpawnPosition;
    public GameObject bulletPrefab;
  
    [SerializeField]
    RaycastHit hit;
    private Vector3 direction;

  
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
       this.transform.GetComponent<SphereCollider>().radius = radius;
    }

    // Update is called once per frame
    void Update()
    {


        //direction = (new Vector3(targetToAttack.position.x, this.transform.position.y, targetToAttack.position.z) - this.transform.position);

        
        if (targetToAttack != null)
        {
           
            if (inSight =Physics.Raycast(this.transform.position, direction, out hit) )
            Debug.DrawRay(this.transform.position, direction, Color.white);
            {
                direction = (new Vector3(targetToAttack.position.x, this.transform.position.y, targetToAttack.position.z) - this.transform.position).normalized;
                if (hit.collider != null)
                {
                    if (hit.collider.tag == "wall" || hit.collider.tag == "Maze")
                    {
                        if ( targetToAttack !=null)
                        {
                            targetToAttack = null;
                        }

                        
                    }
                    else
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

                        if (canShoot)
                        {

                            if (Vector3.Angle(direction, this.transform.forward) < 5f)
                            {
                                Fire();
                            }

                            canShoot = false;
                            Invoke("ChangeFire", fireRate);
                        }

                    }


                }
            }

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

    

    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag("Maze") == false && other.CompareTag("wall")== false && other.CompareTag("Bullet") )
        {
         Debug.Log(other.name + " collider");
           if (targetToAttack == null)
            {
               
             targetToAttack = other.gameObject.transform;
               
            
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (targetToAttack == other.gameObject.transform)
        {
            targetToAttack = null;
           
        }
    }

  
}
