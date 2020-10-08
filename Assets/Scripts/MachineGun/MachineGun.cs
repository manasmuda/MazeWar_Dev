using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [Header("Turning Speed of the turret")]
    public float turnSpeed;
    [Header("Collider Radius")]
    public float radius;

    [Header("Target to attack")]
     public List<Transform> targets = new List<Transform>();
     public  Transform currentTarget;

    [Header("Firing")]
    public bool canShoot = true;
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

        
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {

                direction = (new Vector3(targets[i].position.x, this.transform.position.y, targets[i].position.z) - this.transform.position).normalized;
                Debug.DrawRay(this.transform.position, direction *10f, Color.white);
                if (Physics.Raycast(this.transform.position, direction, out hit, 10f)) 
                {
                    if (hit.collider != null)
                    {
                        Debug.Log(hit.collider.name + "  collider");
                        if (hit.collider.CompareTag("wall")|| hit.collider.CompareTag("Maze") )
                        {
                            continue;
                        }
                        else
                        {
                            if (targets[i] != null)

                            {
                                currentTarget = targets[i];
                                break;
                            }
                            else
                            {
                                currentTarget = null;
                            }
                        }


                    }
                }

            }
            if (currentTarget!= null)
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
        Debug.Log(other.name);
        if (other.CompareTag("wall") == false || other.CompareTag("Maze") == false)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (other.transform == targets[i])
                {
                    return;
                }
            }
                targets.Add(other.transform);
        }
       

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("wall") == false || other.CompareTag("Maze") == false)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == other.gameObject.transform)
                {
                    targets.Remove(other.gameObject.transform);

                }


            }
        }
        
    }

  
}
