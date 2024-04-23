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

    float angle;

    void Start()
    {
        player = GameObject.Find("Player");
        target = player.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            Vector2 aimDirection = player.transform.position - transform.position;
            angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;

            if (Vector3.Distance(transform.position, player.transform.position) < attackRange - 0.1f)
            {
                DoAttack();
            }
            else
            {
                float movementStep = movementSpeed / 100;
                rb.AddForce(aimDirection * movementStep * 10);
            }

            if (target != null)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }

            if (rb.velocity.x >= maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            if (rb.velocity.y >= maxSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxSpeed);
            }
            if (rb.velocity.x <= -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
            if (rb.velocity.y <= -maxSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxSpeed);
            }
        }

        
    }

    void DoAttack()
    {
        //Attack code goes in here
        Debug.Log("MeleeAttack");
    }
}
