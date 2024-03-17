using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health is changed during runtime, only modify maxHealth in editor")]
    [SerializeField] float health;
    [SerializeField] float maxHealth = 100f;

    [Header("Set to false if the player should not be deactivated on death")]
    [SerializeField] bool setInactiveOnDeath = true;

    [Header("Safezone Immortal Flag")]
    public bool isImmortal = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set health to max on start
        health = maxHealth;
    }

    // Apply damage to the player with this function
    public void ApplyDamage(float damage)
    {
        if (!isImmortal) //check if the player isnt immortal
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
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "safeZone")
        {
            isImmortal = false;
        }
    }
}
