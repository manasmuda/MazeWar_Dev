using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

public class newPlayer : MonoBehaviour
{
    public enum playerCameraMode { Fps, thirdPerson}
    public playerCameraMode Mode;

  
 

    public Transform fpCameraTransform;
    public Transform cameraPole;
    public Transform tpCamTransform;

    public LayerMask cameraObstacleLayer;
   public  float maxDistanceDistance;  

   
    public float cameraSensitivity;
    public float moveSpeed;
    public float moveInputDeadZone;


    int leftFingerId;
       int rightFingerId;
    float halfScreenWidth;

 
    Vector2 lookInput;
    float cameraPitch;

    public float maxClamp;
    public float minClamp;

   
    Vector2 moveTouchStartPosition;
    Vector2 moveInput;

    public Touch touch;

    public static newPlayer playerController_instance;

    private void Awake()
    {
        if (playerController_instance == null)
        {
            playerController_instance = this;

        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      
        leftFingerId = -1;
        rightFingerId = -1;

    
        halfScreenWidth = Screen.width / 2;

       
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);

        if ( Mode == playerCameraMode.thirdPerson)
        {
            cameraPitch = cameraPole.localRotation.eulerAngles.x;
            maxDistanceDistance = tpCamTransform.localPosition.z;
        }

    }

    // Update is called once per frame
    void Update()
    {
     
        GetTouchInput();


        if (rightFingerId != -1)
        {
           
            
            LookAround();
        }

        if (leftFingerId != -1)
        {
            
            
        }
            Move();
       
    }
    private void FixedUpdate()
    {
        if (Mode == playerCameraMode.thirdPerson)
        {
            MoveCamera();
        
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

                    if (touch.position.x < halfScreenWidth && leftFingerId == -1)
                    {

                        leftFingerId = touch.fingerId;    
                        
                            moveTouchStartPosition = touch.position;
                           

                    }
                    else if (touch.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                       
                        rightFingerId = touch.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (touch.fingerId == leftFingerId)
                    {
                     
                        leftFingerId = -1;
                       // Debug.Log("Stopped tracking left finger");
                    }
                    else if (touch.fingerId == rightFingerId)
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
                    else if (touch.fingerId == leftFingerId)
                    {

                      

                        moveInput = touch.position - moveTouchStartPosition;
                    }

                        Debug.Log(touch.deltaPosition);
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
        switch (Mode)
        {
            case playerCameraMode.Fps:
                cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, minClamp, maxClamp);
                fpCameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

                break;
            case playerCameraMode.thirdPerson:
                cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, minClamp, maxClamp);
                cameraPole.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
                break;
            
        }
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {


        if (Mode == playerCameraMode.Fps)
        {
            if (moveInput.sqrMagnitude <= moveInputDeadZone) return;
        }

        moveInput.x = CnInputManager.GetAxis("MoveHorizontal");
        moveInput.y = CnInputManager.GetAxis("MoveVertical");
      



        Vector2 movementDirection = moveInput * moveSpeed * Time.deltaTime;
        Debug.Log(movementDirection);
        GetComponent<Rigidbody>().velocity = transform.right * movementDirection.x + transform.forward * movementDirection.y;
        //characterController.Move(transform.right * movementDirection.x  + transform.forward * movementDirection.y );
    }


    void MoveCamera()
    {
        Vector3 rayDir = tpCamTransform.position - cameraPole.position;

        Debug.DrawRay(cameraPole.position, rayDir, Color.red);

        if (Physics.Raycast(cameraPole.position, rayDir, out RaycastHit hit, Mathf.Abs(maxDistanceDistance-1), cameraObstacleLayer))
        {
            tpCamTransform.position = hit.point;
        }
        else {
            tpCamTransform.localPosition = new Vector3(0,0, maxDistanceDistance);
        }
    }
}