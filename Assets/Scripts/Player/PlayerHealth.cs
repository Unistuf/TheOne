using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    [Header(("SaveData"))]
    public XpLevelSystem xpSystem;
    public int levelsCleared;

    [Header(("UI"))]
    public UnityEngine.UI.Image healthBarImage;
    public UnityEngine.UI.Image healthSlider;
    public UnityEngine.UI.Image deathScreen;
    public Sprite[] hpSprites;

    // Start is called before the first frame update
    void Start()
    {       
        // Set health to max on start
        health = maxHealth;

        StartCoroutine(SafeZoneHealing()); //Start the Safe zone healing loop
        LoadSave();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll(); //Delete level persistant data
    }

    public void LoadSave()
    {
        maxHealth = PlayerPrefs.GetFloat("maxHealth", 100);
        health = PlayerPrefs.GetFloat("health", 100);

        hpPotion = PlayerPrefs.GetInt("hpPotion", 0);
        armour = PlayerPrefs.GetInt("armour", 0);

        xpSystem.currentPlayerXp = PlayerPrefs.GetFloat("currentXp", 0);
        xpSystem.currentPlayerLevel = PlayerPrefs.GetInt("currentLevel", 0);

        levelsCleared = PlayerPrefs.GetInt("levelsCleared", 0);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("maxHealth", maxHealth);
        PlayerPrefs.SetFloat("health", health);

        PlayerPrefs.SetInt("hpPotion", hpPotion);
        PlayerPrefs.SetInt("armour", armour);

        PlayerPrefs.SetFloat("currentXp", xpSystem.currentPlayerXp);
        PlayerPrefs.SetInt("currentLevel", xpSystem.currentPlayerLevel);

        PlayerPrefs.SetInt("levelsCleared", levelsCleared);
    }

    void Update()
    {
        // Match the sprite of the HP bar to the fraction of health remaining
        hpPotionText.text = " x " + hpPotion;
        
        if (health <= maxHealth * 0.2f)
        {
            healthBarImage.sprite = hpSprites[4];
        }
        else if (health <= maxHealth * 0.4f)
        {
            healthBarImage.sprite = hpSprites[3];
        }
        else if (health <= maxHealth * 0.6f)
        {
            healthBarImage.sprite = hpSprites[2];
        }
        else if (health <= maxHealth * 0.8f)
        {
            healthBarImage.sprite = hpSprites[1];
        }
        else if (health <= maxHealth)
        {
            healthBarImage.sprite = hpSprites[0];
        }

        healthSlider.fillAmount = health / maxHealth;
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
                // healthSlider.value = health * 0.01f;

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

                    PlayerPrefs.DeleteAll(); //Delete level persistant data

                    Time.timeScale = 0;

                    deathScreen.gameObject.SetActive(true);
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

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
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
        // If we touch a campfire's radius, make the player immortal
        if (col.gameObject.tag == "CampfireRadius")
        {
            isImmortal = true;
        }
        // Or if we touch a potion, add one to our count and destroy it
        else if (col.gameObject.tag == "HpPotion")
        {
            hpPotion += 1;
            Destroy(col.gameObject);
        }
        // And ditto for armour if we can pick it up
        else if (col.gameObject.tag == "Armour")
        {
            if (armour < maxArmour)
            {
                armour += 1;
                Destroy(col.gameObject);
            }
        }
    }

    // Disable immortality on leaving campfire
    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "CampfireRadius")
        {
            isImmortal = false;
        }
    }

    // Heal ourselves when we use a potion and decrease our count
    public void OnUsePotion(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (health < maxHealth)
            {
                hpPotion -= 1;
                ApplyHealing(25);
            }
        }
    }
}
