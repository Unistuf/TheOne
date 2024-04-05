using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health is changed during runtime, only modify maxHealth in editor")]
    public float health;
    [SerializeField] float maxHealth = 100f;

    [Header("Set to false if the player should not be deactivated on death")]
    [SerializeField] bool setInactiveOnDeath = true;

    [Header("Safezone Immortal Flag")]
    public bool isImmortal = false;

    [Header("Items")]
    public int hpPotion;
    public int armour;
    public int maxArmour;
    public TextMeshProUGUI hpPotionText;

    // Start is called before the first frame update
    void Start()
    {
        // Set health to max on start
        health = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown("q")) //TEMP KEY BINDING, IM NOT TOUCHING THAT INPUT SYSTEM, PLS CHANGE
        {
            if (health < maxHealth)
            {
                hpPotion -= 1;
                ApplyHealing(50);
            }
        }

        hpPotionText.text = " x " + hpPotion;
    }

    // Apply damage to the player with this function
    public void ApplyDamage(float damage)
    {
        if (!isImmortal) //check if the player isnt immortal
        {
            if (armour <= 0)//Check if the player has armour
            {
                // Take the damage
                health -= damage;

                // Then check if we are dead
                if (health <= 0)
                {   
                    // Cap our health at 0
                    health = 0;

                    // Then set ourselves to inactive if we should
                    if (setInactiveOnDeath)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                armour -= 1; //Remove armour if the player has it
            }
        }
    }

    // And apply healing to them with this function
    public void ApplyHealing(float healing)
    {
        // Apply the healing
        health += healing;

        // And cap our health so it can't go above maximum
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    IEnumerator SafeZoneHealing()
    {
        if (isImmortal)
        {
            ApplyHealing(maxHealth / 10); //Heal for 10% of the players hp
        }

        yield return new WaitForSeconds(0.2f); //Time between heals
        StartCoroutine(SafeZoneHealing());
    }


    public void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.tag == "safeZone")
        {
            isImmortal = true;
        }
        else if (col.gameObject.tag == "HpPotion")
        {
            hpPotion += 1;
            Destroy(col.gameObject);
        }
        else if (col.gameObject.tag == "Armour")
        {
            if (armour < maxArmour)
            {
                armour += 1;
                Destroy(col.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "safeZone")
        {
            isImmortal = false;
        }
    }
}
