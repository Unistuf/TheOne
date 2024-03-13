using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    public PlayerControls controls;

    [SerializeField] float attackCooldown = 0.5f;
    public bool isAttacking;

    Vector2 rightStickLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Right: " + rightStickLocation);
    }

    public IEnumerator OnSwordSwing(InputValue value)
    {
        rightStickLocation = value.Get<Vector2>();

        if (rightStickLocation.y >= 0.8f && rightStickLocation.x <= 0.3f && isAttacking == false)
        {
            isAttacking = true;

            Debug.Log("Thrust");

            yield return new WaitForSeconds(attackCooldown);
            isAttacking = false;
        }

        yield return null;
    }
}
