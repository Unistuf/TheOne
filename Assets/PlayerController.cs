using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    float speedX, speedY;
    public Animator animator;

    Vector2 movement;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // grabs rigidbody component on player
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // rb.velocity = new Vector2(speedX, speedY);

        // Animation
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Called upon overlapping colliders
    void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
