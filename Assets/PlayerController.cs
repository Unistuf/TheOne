using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public PlayerControls controls;

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
        // Animation
        //animator.SetFloat("Horizontal", movement.x);
        //animator.SetFloat("Vertical", movement.y);
        //animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    // Use our Move action from the new input system to get our input
    public void OnMove(InputValue value)
    {
        // Grab the value of the left stick, and apply it as velocity to our RB
        movement = value.Get<Vector2>();
        rb.velocity = movement * moveSpeed;
    }
}
