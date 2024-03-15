using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpLevelSystem : MonoBehaviour
{
    public int currentPlayerLevel; // x
    public float currentPlayerXp;

    [Header("UI Elements")]
    public Slider ProgressBar;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI xpAmountText;

    [Header("Config")]
    public float xpStartOffset = 9; // c
    public float xpCostScalingMultiplyer = 1.5f; // b

    [Header("Runtime")]
    public float xpRequired; // y


    void Update()
    {
        xpRequired = (currentPlayerLevel * xpCostScalingMultiplyer) * currentPlayerLevel + xpStartOffset; //y = (x * b) * x + c

        if (currentPlayerXp >= xpRequired)
        {
            LevelUp();
        }

        UpdateUi();
    }

    void UpdateUi()
    {
        ProgressBar.maxValue = xpRequired;
        ProgressBar.value = currentPlayerXp;

        currentLevelText.text = "" + currentPlayerLevel;
        nextLevelText.text = "" + (currentPlayerLevel + 1);
        xpAmountText.text = "Level " + currentPlayerLevel + ": " + Mathf.FloorToInt(currentPlayerXp) + "/" + xpRequired + " xp";
    }

    void LevelUp()
    {
        currentPlayerXp -= xpRequired;
        currentPlayerLevel += 1;
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
