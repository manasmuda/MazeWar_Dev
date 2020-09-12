using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class function : MonoBehaviour
{
    public Transform lastpos;
    public float minspeed = 1;
    public float maxspeed = 5;
   
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Custom",1f);
       
    }

    // Update is called once per frame
    void Update()
    {
      
      
    }



    IEnumerator Customfunction(GameObject moveableObject, Vector3 currentPos, Quaternion rot, Vector3 targetPose, float speed,float maxSpeed)
    {
        //yield return new WaitForSeconds(2f);
        GameObject Moveobj = moveableObject;
        Vector3 target = targetPose;
        while (Vector3.Distance(moveableObject.transform.position, target) > 0.5f)
        {
           
            currentPos = Moveobj.transform.position;
           

            Vector3 dir = target - moveableObject.transform.position;
            rot = Quaternion.LookRotation(dir);
            Debug.Log("funtion");

            if (speed < maxSpeed)
            {
                speed += speed * Time.deltaTime;
            }
            else
            {
                speed = maxSpeed;
            }
            Moveobj.transform.position = Vector3.MoveTowards(currentPos, Vector3.Lerp(currentPos, target, Time.deltaTime), speed);
            Moveobj.transform.rotation = Quaternion.Lerp(Moveobj.transform.rotation, rot, speed);
            yield return new WaitForEndOfFrame();
        }
        speed = 0;
        Custom();
    }


    void Custom()
    {
        StartCoroutine((Customfunction(gameObject, gameObject.transform.position, gameObject.transform.rotation, lastpos.position, minspeed, maxspeed)));
    }
}
