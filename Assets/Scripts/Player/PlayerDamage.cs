using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<EnemyHealth>().DoDamage(attackDamage);

            Vector2 aimDirection = transform.parent.transform.position - collision.transform.position;
            float angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
            collision.GetComponent<Rigidbody2D>().AddForce(-aimDirection.normalized * 100);
        }
    }
}
