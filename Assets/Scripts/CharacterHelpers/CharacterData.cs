using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterData : MonoBehaviour
{

    public float MaxHealth = 100;
    public float CurrentHealth;

    public Slider healthSlider;

    public Text PlayerName;

    public string id;
    public string team;

    void Start()
    {
        CurrentHealth = MaxHealth; // Assigning the Variables
        healthSlider.value = (CurrentHealth / MaxHealth);
        //PlayerName = FindObjectOfType<Text>();
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("Canvas/PlayerHealth/Slider").GetComponent<Slider>();
        }
    }

    
    void Update()
    {
      
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

    public void NewPlayerState(ClientState state)
    {
        SyncHealth(state.health);
    }

    public void SyncHealth(int health)
    {
        if (health != Convert.ToInt32(CurrentHealth))
        {
            if (health < CurrentHealth)
            {
                //damage animation
            }
            CurrentHealth = health;
            healthSlider.value = CurrentHealth / MaxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Debug.Log("Hit the player");
            //TakeDamage(damageAmount);
            //Destroy(other.gameObject, 0.5f);
        }
    }

}
