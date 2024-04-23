using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RangedAI : MonoBehaviour
{
    public GameObject player;
    public Transform target;
    public Rigidbody2D rb;
    public GameObject projectilePrefab;

    public SpriteRenderer spriteRenderer;
    public Sprite chargingSprite;
    public Sprite attackingSprite;

    bool isAttacking;

    [Header("Enemy Config")]
    public float aggroRange;
    public float movementSpeed;
    public float maxSpeed;
    public float attackRangeMin;
    public float attackRangeMax;
    public float attackCooldown;

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

            if (Vector3.Distance(transform.position, player.transform.position) < attackRangeMax && Vector3.Distance(transform.position, player.transform.position) > attackRangeMin)
            {
                StartCoroutine(DoAttack());
            }
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) > attackRangeMax)
                {
                    float movementStep = movementSpeed / 100;
                    rb.AddForce(aimDirection * movementStep * 10);
                }

                else if (Vector3.Distance(transform.position, player.transform.position) < attackRangeMin)
                {
                    float movementStep = movementSpeed / 135;
                    rb.AddForce(aimDirection * movementStep * -10);
                }

                if (target != null)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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

            // Set the sprite to the attacking sprite
            spriteRenderer.sprite = attackingSprite;
            yield return new WaitForSeconds(attackCooldown / 5f);

            // Create a projectile and set its target to the player
            GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, this.transform);
            currentProjectile.GetComponent<EnemyProjectileLogic>().target = player;

            // Then wait for the attack cooldown, then unlock the coroutine
            spriteRenderer.sprite = chargingSprite;
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
}
