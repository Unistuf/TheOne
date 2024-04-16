using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeAI : MonoBehaviour
{
    public GameObject player;
    public Transform target;

    [Header("Enemy Config")]
    public float aggroRange;
    public float movementSpeed;
    public float attackRange;

    float angle;

    void Start()
    {
        player = GameObject.Find("Player");
        target = player.transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackRange - 0.1f)
            {
                DoAttack();
            }
            else
            {
                float movementStep = movementSpeed / 100;
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementStep);
            }

            if (target != null)
            {
                Vector2 aimDirection = player.transform.position - transform.position;
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
