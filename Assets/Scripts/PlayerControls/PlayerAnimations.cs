using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public Animator char_anim;
    public ShooterButton _shooterButton;
   
    // Start is called before the first frame update
    void Start()
    {
        char_anim = GetComponent<Animator>();
        _shooterButton = FindObjectOfType<ShooterButton>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
      
        char_anim.SetBool("isCrouching", CrouchButton.instance.isCrouched);

        if (CrouchButton.instance.isCrouched)
        {
            Crouching(NewPlayer.playerController_instance.moveInput.x, NewPlayer.playerController_instance.moveInput.y);
        }
        else if(!CrouchButton.instance.isCrouched) {
             Walking(NewPlayer.playerController_instance.moveInput.x, NewPlayer.playerController_instance.moveInput.y);

          
        }

        Shooting();
    }


    void Shooting()
    {

        if (_shooterButton.pressed&& !CrouchButton.instance.isCrouched)
        {
           
            char_anim.SetBool("isShooting", true);
        }
        else
        {
            
            char_anim.SetBool("isShooting", false);
        }
        if (_shooterButton.pressed && CrouchButton.instance.isCrouched)
        {
            char_anim.SetBool("isCrouchShoot", true);
        }
        else {
            char_anim.SetBool("isCrouchShoot", false);
        }

    }

    void Walking(float X, float Y)
    { 
        char_anim.SetFloat("MoveX", X);
        char_anim.SetFloat("MoveY", Y);
    }

    void Crouching(float X, float Y)
    { 
        char_anim.SetFloat("CrouchX", X);
        char_anim.SetFloat("CrouchY", Y);  
    }
}
