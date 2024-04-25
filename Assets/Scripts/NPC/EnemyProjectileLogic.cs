using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileLogic : MonoBehaviour
{
    // This projectile will aim and fly at the target on spawn

    [Header("Projectile configs")]
    public float damage = 1;
    public float projectileSpeed = 2;
    public float projectileLifespan;

    [Header("Whether the projectile should follow the player or not")]
    public bool isHoming;
    [Header("Buggy, adjust at own risk")]
    public float homingStrength = 0.1f;
    [Header("Whether the projectile is destroyed or not after hitting a collider")]
    public bool destroySelfOnHit;

    [Header("Projectile Lifetime (0 = infinite)")]
    public float lifeTime;

    public Rigidbody2D rb;
    public GameObject parentEnemy;
    public GameObject target;
    float angle;

    Vector2 aimDirection;
    bool alreadyHoming;

    // Start is called before the first frame update
    void Start()
    {
        // Find our rb and the enemy that fired us
        rb = GetComponent<Rigidbody2D>();
        parentEnemy = rb.gameObject.transform.parent.gameObject;
        
        // Find the direction to the target and set our velocity toward it
        aimDirection = target.transform.position - transform.position;
        rb.velocity = aimDirection * projectileSpeed;
        float angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Unparent the projectile so it doesn't continuously inherit momentum
        transform.parent = null;

        // Destroy the projectile after its life expires
        Destroy(gameObject, projectileLifespan);
    }

    // Update is called once per frame
    void Update()
    {
        // If we are a homing projectile, start homing
        if (isHoming)
        {
            StartCoroutine(ProjectileHoming());

            Vector2 aimDirection = target.transform.position - transform.position;
            angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;

            if (target != null)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        StartCoroutine(DestroyOverTime());
    }

    IEnumerator ProjectileHoming()
    {
        if (alreadyHoming == false)
        {
            // Lock the coroutine so only one can run at once
            alreadyHoming = true;

            // Update our direction to the target and nudge our velocity slightly to begin chasing the target
            aimDirection = target.transform.position - transform.position;
            rb.velocity = aimDirection * projectileSpeed;

            // Cap our speed in both axes
            if (rb.velocity.x > projectileSpeed)
            {
                rb.velocity = new Vector2(projectileSpeed, rb.velocity.y);
            }
            if (rb.velocity.y > projectileSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, projectileSpeed);
            }

            // Then wait for the cooldown before nudging again, and unlock the coroutine
            yield return new WaitForSeconds(homingStrength);
            alreadyHoming = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // If we hit our target, damage it
        if (otherCollider.gameObject == target)
        {
            target.GetComponent<PlayerHealth>().ApplyDamage(damage);
        }
        
        // And destroy ourselves if we should upon touching any collider that isn't our parent Enemy     //Do nothing if the collision is with another bullet
        if (otherCollider.gameObject != parentEnemy && destroySelfOnHit && otherCollider.gameObject.tag != "bullet")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(lifeTime);

        if (lifeTime > 0)
        {
            Destroy(gameObject);
        }
    }
}
