using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootBotSyncScript : MonoBehaviour
{
    public Queue<IEnumerator> movers = new Queue<IEnumerator>();
    public int count;

    public bool isMoving = false;

    public Vector3 lastPos;
    public Vector3 Direction;
    public float lastY;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddNewMove(Vector3 end, Vector3 fang)
    {
        float dist = Vector3.Distance(lastPos, end);
        float dist1 = Vector3.Distance(lastPos, transform.position);
        if ((dist > 0.1 && dist < 5) || Math.Abs(lastY - fang.y) > 10)
        {
            Debug.Log("New Move Added");
            IEnumerator newMover = MoveOverSpeed(end, fang);
            movers.Enqueue(newMover);
            if (movers.Count > 10)
            {
                Transport(end, fang);
                return;
            }
            lastPos = new Vector3(end.x, end.y, end.z);
            lastY = fang.y;
            if (!isMoving && movers.Count > 0)
            {
                //Debug.Log("Movement Started freshly");
                StartCoroutine(movers.Dequeue());
                isMoving = true;
            }
            //Debug.Log("Last Pos Set");
        }
        else if (dist >= 5)
        {
            Transport(end, fang);
        }
        if (dist1 > 5)
        {
            Transport(end, fang);
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, Vector3 fang)
    {
        // speed should be 1 unit per second
        //Debug.Log("Move Over Speed Started");
        float time = 0.25f;
        speed = 30f;

        Direction = (end - transform.position).normalized;

        //Debug.Log("w/c animation set");

        Quaternion q = new Quaternion();
        q.eulerAngles = fang;
        transform.rotation = q;

        //Debug.Log("Angle Set");
        int lc = 0;
        //Debug.Log("end:" + end);
        while (transform.position != end)
        {
            //Debug.Log("t:"+transform.position.z+",e:"+end.z);
            transform.position = Vector3.MoveTowards(transform.position, end, speed * 0.02f);
            //Debug.Log("lc:" + lc+", transformed");
            lc++;
            yield return new WaitForSeconds(0.02f);
            time = time - 0.02f;
            //Debug.Log("lc:" + lc);
            if (lc > 10)
            {

                transform.position = end;
                break;
            }
        }
        //Debug.Log("Transformation fixed");
        if (movers.Count > 0)
        {
            Debug.Log("Next Move Enqued");
            StartCoroutine(movers.Dequeue());
        }
        else if (time > 0)
        {
            yield return new WaitForSeconds(time);
            time = 0;
            if (movers.Count > 0)
            {
                Debug.Log("Next Move Enqued");
                StartCoroutine(movers.Dequeue());
            }
            else
            {
                Debug.Log("stopped");
                isMoving = false;
                movers.Clear();
            }
        }
        else
        {
            Debug.Log("stopped");
            isMoving = false;
            movers.Clear();
        }

    }

    public void Transport(Vector3 end, Vector3 fang)
    {
        Debug.Log("Transported");
        movers.Clear();
        transform.position = end;
        Quaternion q = new Quaternion();
        q.eulerAngles = fang;
        transform.rotation = q;
        isMoving = false;
        lastPos = end;
    }
}
