using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementsScript : MonoBehaviour
{
    public CanvasGroup joystick;
    public CanvasGroup shootingButton;
    public CanvasGroup jumpButton;
    public CanvasGroup crouchButton;
    public CanvasGroup GadgetsGroup;
    public CanvasGroup aim;
    public CanvasGroup miniShootingButton;

    public static UIElementsScript instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
