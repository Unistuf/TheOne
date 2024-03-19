using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAi : MonoBehaviour
{
    public GameObject player;
    public GameObject projectilePrefab;

    bool isAttacking;

    [Header("Enemy Config")]
    public float aggroRange;
    public float healthPoints;
    public float movementSpeed;
    public float attackRangeMin;
    public float attackRangeMax;
    public float attackCooldown;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackRangeMax && Vector3.Distance(transform.position, player.transform.position) > attackRangeMin)
            {
                StartCoroutine(DoAttack());
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
                    float movementStep = movementSpeed / 135;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -movementStep);
                }

            }
        }
    }

    IEnumerator DoAttack()
    {
        if (!isAttacking)
        {
            // Lock coroutine until it finishes
            isAttacking = true;

            // Create a projectile and set its target to the player
            GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, this.transform);
            currentProjectile.GetComponent<EnemyProjectileLogic>().target = player;

            // Then wait for the attack cooldown, then unlock the coroutine
            yield return new WaitForSeconds(attackCooldown);
            isAttacking = false;
        }
    }

    IEnumerator DoHomingAttack()
    {
        if (!isAttacking)
        {
            // Lock coroutine until it finishes
            isAttacking = true;

            // Create a projectile and set its target to the player
            EnemyProjectileLogic currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, this.transform).GetComponent<EnemyProjectileLogic>();
            currentProjectile.target = player;
            currentProjectile.isHoming = true;
            currentProjectile.projectileSpeed = 3f;

            // Then wait for the attack cooldown, then unlock the coroutine
            yield return new WaitForSeconds(attackCooldown);
            isAttacking = false;
        }
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
        //Death code
        player.GetComponent<XpLevelSystem>().AddPlayerXp(3);
        Destroy(this.gameObject);
    }
}
