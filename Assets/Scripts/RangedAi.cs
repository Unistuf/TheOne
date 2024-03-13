using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAi : MonoBehaviour
{
    public GameObject player;

    [Header("Enemy Config")]
    public float aggroRange;
    public float healthPoints;
    public float movementSpeed;
    public float attackRangeMin;
    public float attackRangeMax;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackRangeMax &&Vector3.Distance(transform.position, player.transform.position) > attackRangeMin)
            {
                DoAttack();
            }
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) > attackRangeMax)
                {
                    float movementStep = movementSpeed / 100;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementStep);
                }

                else if (Vector3.Distance(transform.position, player.transform.position) < attackRangeMin)
                {
                    float movementStep = movementSpeed / 80;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -movementStep);                    
                }

            }
        }
    }

    void DoAttack()
    {
        //Attack code goes in here
        Debug.Log("SHOOT");
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
