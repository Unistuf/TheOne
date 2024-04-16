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
        Quaternion playerAim = Quaternion.LookRotation(Vector3.forward, movement);

        transform.rotation = playerAim;

        // Animation

        if (movement == new Vector2(0, 0))
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
        }

        //animator.SetFloat("Horizontal", movement.x);
        //animator.SetFloat("Vertical", movement.y);
        //animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    // Use our Move action from the new input system to get our input
    public void OnMove(InputAction.CallbackContext value)
    {
        // Grab the value of the left stick, and apply it as velocity to our RB
        movement = value.ReadValue<Vector2>();
        rb.velocity = movement * moveSpeed;
    }

    public void OnSwordSwing(InputAction.CallbackContext value)
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
                    StartCoroutine(AttackWithSword(attackID));
                }

                // And if we do not hit a valid combo, just perform the attack of the most recent letter (e.g. if DDC is not a valid combo, just execute C)
                if (!foundOnFirstCheck)
                {
                    string lastAttack = Char.ToString(comboString[comboString.Length - 1]);

                    if (comboList.ContainsKey(lastAttack))
                    {
                        comboList.TryGetValue(comboString, out int attackID);

                        StartCoroutine(AttackWithSword(attackID));
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
