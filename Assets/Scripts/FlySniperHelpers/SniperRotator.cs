using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRotator : MonoBehaviour
{

    private Touch touch;

    int rightFingerId;
    float halfScreenWidth;

    public float cameraSensitivity;

    Vector2 lookInput;

    Vector3 prefabOrigin;

    // Start is called before the first frame update
    void Start()
    {
        prefabOrigin = transform.parent.position;

        Client.clientInstance.character.transform.position = new Vector3(prefabOrigin.x, 1f, prefabOrigin.z - 1.5f);
        rightFingerId = -1;

        cameraSensitivity = 8;

        halfScreenWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        GetTouchInput();
        if (rightFingerId != -1)
        {
            LookAround();
        }
    }

    void GetTouchInput()
    {

        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);

            // Check each touch's phase
            switch (touch.phase)
            {
                case TouchPhase.Began:


                    if (touch.position.x > halfScreenWidth && rightFingerId == -1)
                    {

                        rightFingerId = touch.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:


                    if (touch.fingerId == rightFingerId)
                    {

                        rightFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }

                    break;
                case TouchPhase.Moved:


                    if (touch.fingerId == rightFingerId)
                    {

                        lookInput = touch.deltaPosition * cameraSensitivity * Time.deltaTime;

                    }


                    break;
                case TouchPhase.Stationary:

                    if (touch.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {
        float z = Mathf.Clamp(transform.rotation.eulerAngles.z - lookInput.y, 0.1f, 32f);
        float y = transform.rotation.eulerAngles.y + lookInput.x;
        Quaternion quaternion = Quaternion.identity;
        quaternion.eulerAngles = new Vector3(0f, y, z);
        transform.rotation = quaternion;
        /*transform.Rotate(0f,lookInput.x,-lookInput.y);
        Quaternion quaternion = transform.rotation;
        Vector3 ea = quaternion.eulerAngles;
        ea.x = 0;
        quaternion.eulerAngles = ea;
        transform.rotation = quaternion;*/
    }

}
