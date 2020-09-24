using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private bl_Joystick joystick;
    [SerializeField]
    private Button shootButton;

    public float vertical;
    public float horizontal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vertical = joystick.Vertical;
        horizontal = joystick.Horizontal;
    }
}
