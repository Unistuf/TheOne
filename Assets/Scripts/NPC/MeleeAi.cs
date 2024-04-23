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

            if (Vector3.Distance(transform.position, player.transform.position) < attackRange - 0.1f)
            {
                DoAttack();
            }
            else
            {
                float movementStep = movementSpeed / 100;
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementStep);
                Debug.Log("guh");
                //rb.AddForce(aimDirection * movementStep);
            }

            if (target != null)
            {
                angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        
    }

    void DoAttack()
    {
        //Attack code goes in here
        Debug.Log("MeleeAttack");
    }
}
