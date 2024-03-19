using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    public PlayerControls controls;
    public PlayerHealth playerHealth;

    [SerializeField] float attackCooldown = 0.5f;

    Vector2 rStickPos;
    [SerializeField] float zoneWidth = 0.3f;
    [SerializeField] float zoneHeight = 0.8f;
    string currentZone;
    bool insideZone;

    // The list of valid attacks and combos that we should check against
    // To add more moves, increase the first value of comboList and add the new move's name and corresponding combo in the format below
    string[,] comboList = new string[8, 2]
        {
    {"basicThrust", "A"},
    {"basicRightSwing", "B"},
    {"basicParry", "C"},
    {"basicLeftSwing", "D"},
    {"doubleThrust", "AA" },
    {"secondRightSwing", "BB" },
    {"doubleParry", "CC"},
    {"secondLeftSwing", "DD" },
};
    [SerializeField] string comboString = string.Empty;

    public GameObject ThrustHitbox;
    public GameObject SwingHitbox;
    public GameObject ParryHitbox;

    public void OnSwordSwing(InputValue value)
    {
        // Collect the input value of our right stick
        rStickPos = value.Get<Vector2>();

        // And then check if it is inside the bounds of any of these "zones" at the top, right, bottom, and left edges of our analogue stick
        // Each zone is labeled ABCD respectively, and N is the "neutral" zone representing the centre of the stick
        // The exact bounds of these boxes can be tuned with zoneHeight and zoneWidth
        if (rStickPos.y >= zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth)
        {
            StartCoroutine(SwordAttacks("A"));
        }
        if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x >= zoneHeight)
        {
            StartCoroutine(SwordAttacks("B"));
        }
        if (rStickPos.y <= -zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth)
        {
            StartCoroutine(SwordAttacks("C"));
        }
        if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x <= -zoneHeight)
        {
            StartCoroutine(SwordAttacks("D"));
        }
        if (rStickPos.y <= zoneHeight && rStickPos.y >= -zoneHeight && rStickPos.x <= zoneHeight && rStickPos.x >= -zoneHeight)
        {
            StartCoroutine(SwordAttacks("N"));
        }
    }

    public IEnumerator SwordAttacks(string inZone)
    {
        if (insideZone == false)
        {
            // If our stick has entered a new zone, lock this function until we leave it
            insideZone = true;
            currentZone = inZone;

            // And append the zone's ID to the combo string, unless we are in the null zone in the centre
            if (inZone != "N")
            {
                comboString += inZone;
                bool foundOnFirstCheck = false;

                // Then, check current the combo against the combo list, and perform any attack or combo that we "hit" at each stage
                // E.g. if we perform a combo AAC, this means we will perform 3 attacks, A, AA, and AAC
                for (int i = 0; i < comboList.GetLength(0); i++)
                {
                    if (comboString == comboList[i, 1])
                    {
                        foundOnFirstCheck = true;
                        Debug.Log(comboList[i, 0]);
                        StartCoroutine(AttackWithSword(i));
                    }
                }

                // And if we do not hit a valid combo, just perform the attack of the most recent letter (e.g. if DDC is not a valid combo, just execute C)
                if (!foundOnFirstCheck)
                {
                    string lastAttack = Char.ToString(comboString[comboString.Length - 1]);

                    for (int i = 0; i < comboList.GetLength(0); i++)
                    {
                        if (lastAttack == comboList[i, 1])
                        {
                            Debug.Log(comboList[i, 0]);
                            StartCoroutine(AttackWithSword(i));
                        }
                    }
                }
            }

            // Cap our combo length to 2
            if (comboString.Length >= 2)
            {
                comboString = string.Empty;
            }
        }

        // If the stick is not inside a zone, allow it to select a new zone
        if (inZone != currentZone)
        {
            insideZone = false;
        }

        yield return null;
    }

    // Initiate actual attacks
    public IEnumerator AttackWithSword(int attackID)
    {
        if (!playerHealth.isImmortal) // Player cannot attack while immortal
        {
            // Attack IDs
            if (attackID == 0)
            {
                StartCoroutine(ThrustAttack());
            }
            if (attackID == 1)
            {
                StartCoroutine(SwingAttack(true));
            }
            if (attackID == 3)
            {
                StartCoroutine(SwingAttack(false));
            }
            if (attackID == 4)
            {
                StartCoroutine(ThrustAttack());
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(ThrustAttack());
            }
        }

        yield return null;
    }

    // Thrust attack logic
    public IEnumerator ThrustAttack()
    {
        GameObject currentThrustAttack = Instantiate(ThrustHitbox, transform);
        currentThrustAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;

        // Move the hitbox forwards relative to the player
        for (int i = 0; i < 100; i++)
        {
            currentThrustAttack.transform.localPosition = new Vector3(0, i * 0.025f, 0);
            currentThrustAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;
            yield return new WaitForSeconds(0.00005f);
        }

        // And destroy the game object after the attack is complete
        Destroy(currentThrustAttack);
        yield return null;
    }

    // Swing attack logic
    public IEnumerator SwingAttack(bool isRight)
    {
        GameObject currentSwingAttack = Instantiate(SwingHitbox, transform);
        currentSwingAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;
        float multiplier = 2f;
        
        // If we are swinging to the right, invert the rotation direction by making multiplier negative
        if (isRight)
        {
            multiplier = -multiplier;
        }
        //asdhahs
        for (int i = 0; i <= 45; i++)
        {
            currentSwingAttack.transform.localRotation = Quaternion.Euler(0, 0, i * multiplier);
            yield return new WaitForSeconds(0.005f);
        }

        Destroy(currentSwingAttack.gameObject); // beeng
        yield return null;
    }

}
