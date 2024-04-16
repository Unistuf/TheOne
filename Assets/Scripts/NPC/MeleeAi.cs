using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeAI : MonoBehaviour
{
    public GameObject player;
    public Transform target;
    public GameObject hpBottleDrop;
    public float hpBottleDropChance;

    [Header("Enemy Config")]
    public float aggroRange;
    public float healthPoints;
    public float movementSpeed;
    public float attackRange;

    [Header("Xp")]
    public int xpGain;

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

                Vector2 aimDirection = player.transform.position - transform.position;
                transform.rotation = Quaternion.Euler(aimDirection);
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

    public void DoDamage(float damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            DoDeath();
        }
    }

    void DoDeath()
    {
        if (hpBottleDropChance > Random.Range(0,100))
        {
            Instantiate(hpBottleDrop, transform.position, transform.rotation);
        }
        //Death code

        player.GetComponent<XpLevelSystem>().AddPlayerXp(xpGain);
        Destroy(this.gameObject);
    }
}
