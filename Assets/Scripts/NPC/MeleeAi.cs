using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeAI : MonoBehaviour
{
    public GameObject player;
    public Transform target;
    public Rigidbody2D rb;

    [Header("Enemy Config")]
    public float aggroRange;
    public float movementSpeed;
    public float maxSpeed;
    public float attackRange;
    public GameObject meleeAttack;

    float angle;
    bool isAttacking = false;

    void Start()
    {
        player = GameObject.Find("Player");
        target = player.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange) //Check if the player is in Aggro range
        {
            Vector2 aimDirection = player.transform.position - transform.position;
            angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;

            if (Vector3.Distance(transform.position, player.transform.position) < attackRange - 0.1f) //If the player is in attack range, then attack the player
            {
                StartCoroutine(DoAttack());
            }
            else // Or if we aren't close enough, move closer
            {
                float movementStep = movementSpeed / 100;
                rb.AddForce(aimDirection * movementStep * 10);
            }

            // Look at the target
            if (target != null)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); 
            }

            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed)); // Clamp speed
        }
    }

    IEnumerator DoAttack()
    {
        // Lock the coroutine while it's on cooldown
        if (!isAttacking)
        {
            isAttacking = true;
            GameObject currentAttack = Instantiate(meleeAttack, gameObject.transform, false); //Spawn attack
            currentAttack.transform.parent = null;
            yield return new WaitForSeconds(1f);
            isAttacking = false;
        }
    }
}
