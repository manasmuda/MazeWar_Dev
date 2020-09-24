using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterData : MonoBehaviour
{

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float damageAmount =20;

  public Slider healthSlider;

    void Start()
    {
        
        CurrentHealth = MaxHealth; // Assigning the Variables
        healthSlider.value = (CurrentHealth / MaxHealth);
        
    }

    
    void Update()
    {
        healthSlider.transform.LookAt(Camera.main.transform);
    }


    public void TakeDamage(float D_Amount)// Damaging the Player and can be used for Enemy as well
    {
        CurrentHealth -= D_Amount;
        healthSlider.value = (CurrentHealth / MaxHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
       
    public void Addhealth(int H_Amount)// Adding Health to Player
    {
        CurrentHealth += H_Amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public void UpgradeHealth(int H_Upgrade)// Upgrading Player Health
    {
        MaxHealth = H_Upgrade;
        CurrentHealth = MaxHealth;
    }

    public void Die()// Killing Player
    {
       Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)// You can change this to Trigger if you want
    {
        if(collision.gameObject.tag == "Medic")// Medium Health kit for Player
        {
            Addhealth(50);
        }

        if (collision.gameObject.tag == "Medikit")// Large Health kit for Player
        {
            Addhealth(100);
        }

        if (collision.gameObject.tag == "Enemy")// Damage for Player by 
        {
            TakeDamage(10);
        }

        if(collision.gameObject.tag == "Upgrade")// Upgrade for Player
        {
            UpgradeHealth(200);
        }

        if(collision.gameObject.tag =="Player")// Damage for Enemy by Player
        {
            TakeDamage(20);
        }
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {

            Debug.Log("Damaging the player");
            TakeDamage(damageAmount);
            //Destroy(other.gameObject, 0.5f);
           
        }
    }

}
