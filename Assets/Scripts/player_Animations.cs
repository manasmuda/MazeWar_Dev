using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_Animations : MonoBehaviour
{

    public Animator char_anim;
  
    // Start is called before the first frame update
    void Start()
    {
        char_anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {


        //if ()
        //{
        //}
        char_anim.SetBool("isCrouching", crouch_Button.instance.isCrouched);
        if (crouch_Button.instance.isCrouched)
        {
            Crouching(newPlayer.playerController_instance.moveInput.x, newPlayer.playerController_instance.moveInput.y);
        }
        else if(!crouch_Button.instance.isCrouched) {
             Walking(newPlayer.playerController_instance.moveInput.x, newPlayer.playerController_instance.moveInput.y);

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
