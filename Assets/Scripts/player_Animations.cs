using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_Animations : MonoBehaviour
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
    void Update()
    {
      
      
      

      
        char_anim.SetBool("isCrouching", crouch_Button.instance.isCrouched);

        if (crouch_Button.instance.isCrouched)
        {
            Crouching(newPlayer.playerController_instance.moveInput.x, newPlayer.playerController_instance.moveInput.y);
        }
        else if(!crouch_Button.instance.isCrouched) {
             Walking(newPlayer.playerController_instance.moveInput.x, newPlayer.playerController_instance.moveInput.y);

          
        }


        Shooting();






    }


    void Shooting()
    {

        if (_shooterButton.pressed && newPlayer.playerController_instance.moveInput.x == 0f && newPlayer.playerController_instance.moveInput.y == 0f)
        {
           
            char_anim.SetBool("isShooting", true);

        }
        else
        {
           
            char_anim.SetBool("isShooting", false);
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
