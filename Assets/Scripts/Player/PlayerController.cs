using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public PlayerHealth playerHealth;
    public Rigidbody2D rb;

    Vector2 movement;
    Vector2 rStickPos;

    [SerializeField] float zoneWidth = 0.3f;
    [SerializeField] float zoneHeight = 0.8f;
    string currentZone;
    bool insideZone;

    [SerializeField] string comboString = string.Empty;
    [SerializeField] float attackCooldown = 0.5f;
    bool attackEnabled = true;

    public GameObject ThrustHitbox;
    public GameObject SwingHitbox;
    public GameObject ParryHitbox;

    // The list of valid attacks and combos that we should check against
    // To add more moves, increase the first value of comboList and add the new move's name and corresponding combo in the format below
    Dictionary<string, int> comboList = new Dictionary<string, int>()
    {
        {"A", 0},
        {"B", 1},
        {"C", 2},
        {"D", 3},
        {"AA", 4}
     };
    

    // Start is called before the first frame update
    void Start()
    {
        // grabs rigidbody component on player
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        if (movement == new Vector2(0, 0))
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
        }
    }

    // Use our Move action from the new input system to get our input
    public void OnMove(InputAction.CallbackContext value)
    {
        // Grab the value of the left stick, and apply it as velocity to our RB
        movement = value.ReadValue<Vector2>();
        rb.velocity = movement * moveSpeed;

        // And do the same for our rotation
        if (value.ReadValue<Vector2>() != new Vector2(0, 0))
        {
            Quaternion playerAim = Quaternion.LookRotation(Vector3.forward, movement);

            transform.rotation = playerAim;
        }
        
    }

    public void OnSwordSwing(InputAction.CallbackContext value)
    {
        if (attackEnabled)
        {
            // Collect the input value of our right stick
            rStickPos = value.ReadValue<Vector2>();

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
                if (comboList.ContainsKey(comboString))
                {
                    comboList.TryGetValue(comboString, out int attackID);

                    foundOnFirstCheck = true;
                    StartCoroutine(CheckSwordAttack(attackID));
                }

                // And if we do not hit a valid combo, just perform the attack of the most recent letter (e.g. if DDC is not a valid combo, just execute C)
                if (!foundOnFirstCheck)
                {
                    string lastAttack = Char.ToString(comboString[comboString.Length - 1]);

                    if (comboList.ContainsKey(lastAttack))
                    {
                        comboList.TryGetValue(lastAttack, out int attackID);

                        StartCoroutine(CheckSwordAttack(attackID));
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
    public IEnumerator CheckSwordAttack(int attackID)
    {
        if (!playerHealth.isImmortal) // Player cannot attack while immortal
        {
            // Attack IDs
            if (attackID == 0)
            {
                StartCoroutine(ThrustAttack());
                attackEnabled = false;
                yield return new WaitForSeconds(0.15f);
                attackEnabled = true;
            }
            if (attackID == 1)
            {
                StartCoroutine(SwingAttack(true, 45));
                attackEnabled = false;
                yield return new WaitForSeconds(0.25f);
                attackEnabled = true;
            }
            if (attackID == 3)
            {
                StartCoroutine(SwingAttack(false, 45));
                attackEnabled = false;
                yield return new WaitForSeconds(0.25f);
                attackEnabled = true;
            }
            if (attackID == 4)
            {
                StartCoroutine(ThrustAttack());
                attackEnabled = false;
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(ThrustAttack());
                yield return new WaitForSeconds(0.25f);
                attackEnabled = true;
            }
        }

        yield return null;
    }

    // Thrust attack logic
    public IEnumerator ThrustAttack()
    {
        GameObject currentThrustAttack = Instantiate(ThrustHitbox, transform);
        currentThrustAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;

        // And destroy the game object after the attack is complete
        Destroy(currentThrustAttack, 0.15f);
        yield return null;
    }

    // Swing attack logic
    public IEnumerator SwingAttack(bool isRight, float startPos)
    {
        GameObject currentSwingAttack = Instantiate(SwingHitbox, transform);
        currentSwingAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;
        float multiplier = 2f;

        // If we are swinging to the right, invert the rotation direction by making multiplier negative
        if (isRight)
        {
            multiplier = -multiplier;
        }
        for (float i = startPos; i <= i + 45; i++)
        {
            currentSwingAttack.transform.localRotation = Quaternion.Euler(0, 0, i * multiplier);
            yield return new WaitForSeconds(0.75f * Time.deltaTime);
        }

        Destroy(currentSwingAttack.gameObject);
        yield return null;
    }

}
