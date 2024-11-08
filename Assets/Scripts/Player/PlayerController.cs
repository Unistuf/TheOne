using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public PlayerHealth playerHealth;
    public Rigidbody2D rb;

    Vector2 movement;
    [SerializeField] Vector2 rStickPos;

    [SerializeField] int maxComboLength;
    [SerializeField] float zoneWidth = 0.3f;
    [SerializeField] float zoneHeight = 0.8f;
    string currentZone;
    bool insideZone;

    [SerializeField] string comboString = string.Empty;
    bool attackEnabled = true;
    [SerializeField] InputActionAsset inputActionAsset;

    public GameObject ThrustHitbox;
    public GameObject SwingHitbox;
    public GameObject ParryHitbox;

    public GameObject pauseMenuPanel;
    public GameObject comboDisplay;
    public Image comboArrowImage;

    // The list of valid attacks and combos that we should check against
    // To add more moves, increase the first value of comboList and add the new move's name and corresponding combo in the format below
    Dictionary<string, int> comboList = new Dictionary<string, int>()
    {
        {"A", 0},
        {"B", 1},
        {"C", 2},
        {"D", 3},
        {"AA", 4},
        {"AAA", 8},
        {"DDB", 9},
        {"BBD", 10}
     };


    // Start is called before the first frame update
    void Start()
    {
        // grabs rigidbody component on player
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        comboDisplay = GameObject.Find("ComboDisplay");
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

    public void ExitToMainMenu()
    {  
        SceneManager.LoadScene(1);
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

            if (inZone != "N")
            {
                // Do not clear our combo display until we start another combo
                if (inZone != string.Empty && comboString == string.Empty)
                {
                    foreach (Transform arrowImage in comboDisplay.transform)
                    {
                        Destroy(arrowImage.gameObject);
                    }
                }

                // And append the zone's ID to the combo string, unless we are in the null zone in the centre
                comboString += inZone;
                bool foundOnFirstCheck = false;


                // Add the current input to the combo display
                Image currentArrow = Instantiate(comboArrowImage, comboDisplay.transform);

                // And rotate the image of the arrow to match the input direction
                switch (inZone)
                {
                    case "A":
                        currentArrow.transform.rotation = Quaternion.identity;
                        break;                    
                    case "B":
                        currentArrow.transform.rotation = Quaternion.Euler(0, 0, 270);
                        break;                    
                    case "C":
                        currentArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
                        break;                   
                    case "D":
                        currentArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                        break;
                }

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

                rStickPos = Vector2.zero;
            }

            // Cap our combo length to 3
            if (comboString.Length >= maxComboLength)
            {
                comboString = string.Empty;
            }
        }

        // If the stick is not inside a zone, allow it to select a new zone
        if (inZone != currentZone)
        {
            insideZone = false;
        }

        inZone = string.Empty;

        yield return null;
    }

    // Initiate actual attacks
    public IEnumerator CheckSwordAttack(int attackID)
    {
        if (!playerHealth.isImmortal) // Player cannot attack while immortal
        {
            // Switch to select attack to initiate
            switch (attackID)
            {
                case 0:
                    StartCoroutine(ThrustAttack());
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.15f);
                    attackEnabled = true;
                    break;
                case 1:
                    StartCoroutine(SwingAttack(true, 22.5f, 45f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
                case 2:
                    StartCoroutine(ParryAttack());
                    attackEnabled = false;
                    yield return new WaitForSeconds(1f);
                    attackEnabled = true;
                    break;
                case 3:
                    StartCoroutine(SwingAttack(false, 22.5f, 45f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
                case 4:
                    StartCoroutine(SwingAttack(true, -22.5f, 45f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine(ThrustAttack());
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
                case 8:
                    StartCoroutine(SwingAttack(false, -22.5f, 45f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(SwingAttack(true, -22.5f, 45f));
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(SwingAttack(false, -22.5f, 45f));
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(ThrustAttack());
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
                case 9:
                    StartCoroutine(SwingAttack(true, 45f, 360f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
                case 10:
                    StartCoroutine(SwingAttack(false, -45f, 360f));
                    attackEnabled = false;
                    yield return new WaitForSeconds(0.25f);
                    attackEnabled = true;
                    break;
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
    public IEnumerator SwingAttack(bool isRight, float startPos, float swingWidth)
    {
        GameObject currentSwingAttack = Instantiate(SwingHitbox, transform);
        currentSwingAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;
        float multiplier = 2f;
        float maxSwing = swingWidth;

        // If we are swinging to the right, invert the rotation direction by making multiplier negative
        if (isRight)
        {
            multiplier = -multiplier;
        }
        for (float i = startPos; i <= startPos + maxSwing; i += 2)
        {
            currentSwingAttack.transform.localRotation = Quaternion.Euler(0, 0, i * multiplier);
            yield return new WaitForSeconds(0.005f);
        }

        Destroy(currentSwingAttack.gameObject);
        yield return null;
    }
    
    public IEnumerator ParryAttack()
    {
        GameObject currentParryAttack = Instantiate(ParryHitbox, transform);
        currentParryAttack.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;

        Destroy(currentParryAttack, 0.3f);
        yield return null;
    }

    public void OnTogglePause(InputAction.CallbackContext context)
    {
        bool isPanelActive = pauseMenuPanel.activeSelf;

        // Open the panel
        if (context.performed)
        {
            pauseMenuPanel.SetActive(!isPanelActive);
        }

        // Freeze/unfreeze game when panel is open
        if (isPanelActive)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    /*    
    public void SelectNextButton(GameObject gameobject)
    {
        EventSystem.current.SetSelectedGameObject(gameobject);
    }*/
}
