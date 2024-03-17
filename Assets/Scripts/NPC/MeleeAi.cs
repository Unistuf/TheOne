using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAi : MonoBehaviour
{
    public GameObject player;
    public Transform target;

    [Header("Enemy Config")]
    public float aggroRange;
    public float healthPoints;
    public float movementSpeed;
    public float attackRange;

    void Start()
    {
        player = GameObject.Find("Player");
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

            // Needs Fixing because i cant figure out why it wont rotate on the fucking Z AXIS!!!!!!!!!

            //if (target != null)
            //{
            //    // transform.forward(target);
            //}
        }
    }

    void DoAttack()
    {
        //Attack code goes in here
        Debug.Log("MeleeAttack");
    }

    void DoDamage(float damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            DoDeath();
        }
    }

    void DoDeath()
    {
        //Death code
        Destroy(this.gameObject);
    }
}
