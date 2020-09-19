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

    [HideInInspector]
   public  float maxDistanceDistance;  

   [Header("Camera Rotation speed")]
   [Tooltip("Set the rotation speed for camera")]
    public float cameraSensitivity;

    
    public float currentSpeed;

    [Header("Walking speed")]
   [Tooltip("Set the speed for walk")]
    public float walkSpeed = 170f;
    [Header("Run Speed")]
    [Tooltip("Set the speed for run")]
    public float runningSpeed = 300f;

    
    public float moveInputDeadZone;
  
       int rightFingerId;
    float halfScreenWidth;


    Vector2 lookInput;


           float cameraPitch;
    public float maxClamp;
    public float minClamp;



    float maxCamY = 1.682f;
    float minCamY =1.45f;


    Vector2 moveTouchStartPosition;
    [HideInInspector]

   public Vector2 moveInput;

   Touch touch;
   
    public Rigidbody rb;


  

    public static newPlayer playerController_instance;

    private void Awake()
    {
        if (playerController_instance == null)
        {
            playerController_instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      
       
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
            Move();
        

    }
    private void FixedUpdate()
    {

     
        if (Mode == playerCameraMode.thirdPerson)
        {
            //checking whether the player is crouched or not
            if (crouch_Button.instance.isCrouched)
            {
                //aligning camera if crouched
                cameraPole.localPosition = new Vector3(cameraPole.localPosition.x, Mathf.Lerp(cameraPole.localPosition.y, minCamY, 0.2f), cameraPole.localPosition.z);
            }
            else
            {
                //aligning camera if  not crouched
                cameraPole.localPosition = new Vector3(cameraPole.localPosition.x, Mathf.Lerp(cameraPole.localPosition.y, maxCamY, 0.2f), cameraPole.localPosition.z);
            }
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
        transform.Rotate(Vector3.up, lookInput.x);
    }

    void Move()
    {
        

        if (Mode == playerCameraMode.Fps)
        {
            if (moveInput.sqrMagnitude <= moveInputDeadZone) return;
        }

        //Setting Joystick axis 
        moveInput.x = CnInputManager.GetAxis("MoveHorizontal");
        moveInput.y = CnInputManager.GetAxis("MoveVertical");


        //Controlling speed through the axis
        if (moveInput.y > 0.75f && moveInput.y <= 1)
        {
            currentSpeed = Mathf.Lerp(walkSpeed, runningSpeed, 1f);
        }
        else if (moveInput.y >= 0.1f)
        {

            currentSpeed = walkSpeed;
        }
        else if (moveInput.y == 0f)
        {

            currentSpeed = 0f;
        }
        else if (moveInput.y>-1&& moveInput.y<0)
        {
            currentSpeed = walkSpeed;
        }

     //moving along direction
        Vector2 movementDirection = moveInput * currentSpeed * Time.deltaTime;
        rb.velocity = transform.right * movementDirection.x + transform.forward * movementDirection.y;
       
    }


    void MoveCamera()
    {
        // camera collision
        Vector3 rayDir = tpCamTransform.position - cameraPole.position;

        Debug.DrawRay(cameraPole.position, rayDir, Color.red);

        if (Physics.Raycast(cameraPole.position, rayDir, out RaycastHit hit, Mathf.Abs(maxDistanceDistance), cameraObstacleLayer))
        {
            tpCamTransform.position = hit.point;
        }
        else {

            tpCamTransform.localPosition = new Vector3(0,0, maxDistanceDistance);
        }
    }


  
}