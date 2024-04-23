using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.UIElements;

public class XpLevelSystem : MonoBehaviour
{
    public int currentPlayerLevel; // x
    public float currentPlayerXp;

    [Header("UI Elements")]
    public Image ProgressBar;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI xpAmountText;

    [Header("Config")]
    public float xpStartOffset = 9; // c
    public float xpCostScalingMultiplyer = 1.5f; // b

    [Header("Runtime")]
    public float xpRequired; // y
    public PlayerHealth playerHealth;


    void Update()
    {
        xpRequired = (currentPlayerLevel * xpCostScalingMultiplyer) * currentPlayerLevel + xpStartOffset; //y = (x * b) * x + c

        if (currentPlayerXp >= xpRequired)
        {
            LevelUp();
        }

        UpdateUi();

        ProgressBar.fillAmount = currentPlayerXp /xpRequired;
    }

    void UpdateUi()
    {
        //ProgressBar.maxValue = xpRequired;
        //ProgressBar.value = currentPlayerXp;

        currentLevelText.text = "" + currentPlayerLevel;
        nextLevelText.text = "" + (currentPlayerLevel + 1);
        xpAmountText.text = "Level " + currentPlayerLevel + ": " + Mathf.FloorToInt(currentPlayerXp) + "/" + xpRequired + " xp";
    }

    void LevelUp()
    {
        currentPlayerXp -= xpRequired;
        currentPlayerLevel += 1;

        
        playerHealth.IncreaseMaxHealth(10);
        playerHealth.ApplyHealing(10);
        playerHealth.hpPotion += 2;
    }


//                                                  Public Functions
//--------------------------------------------------------------------------------------------------------------------------------

    public void AddPlayerXp(int amount)
    {
        currentPlayerXp += amount;
    }

    public void RemovePlayerXp(int amount)
    {
        currentPlayerXp -= amount;
    }

    public void SetPlayerXp(int amount)
    {
        currentPlayerXp = amount;
    }

    public void AddPlayerLevel(int amount)
    {
        currentPlayerLevel += amount;
    }

    public void RemovePlayerLevel(int amount)
    {
        currentPlayerLevel -= amount;
    }

    public void SetPlayerLevel(int amount)
    {
        currentPlayerLevel = amount;
    }
}
