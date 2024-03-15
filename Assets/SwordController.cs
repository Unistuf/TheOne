using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    public PlayerControls controls;

    [SerializeField] float attackCooldown = 0.5f;
    public bool insideZone;

    string comboString;
    bool extendingString;

    Vector2 rStickPos;
    [SerializeField] float zoneWidth = 0.3f;
    [SerializeField] float zoneHeight = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Right: " + rightStickLocation);
    }

    public void OnSwordSwing(InputValue value)
    {
        rStickPos = value.Get<Vector2>();
        Debug.Log("Input called");


        if (insideZone == false)
        {
            if (rStickPos.y >= zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth && insideZone == false)
            {
                Debug.Log("A");
                insideZone = true;

                StartCoroutine(SwordAttacks("A"));
            }
            else if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x >= zoneHeight && insideZone == false)
            {
                Debug.Log("B");
                insideZone = true;
            }
            else if (rStickPos.y <= -zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth && insideZone == false)
            {
                Debug.Log("C");
                insideZone = true;
            }
            else if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x <= -zoneHeight && insideZone == false)
            {
                Debug.Log("D");
                insideZone = true;
            }
            else
            {
                insideZone = false;
                Debug.Log("false");
            }
        }
    }

    public IEnumerator SwordAttacks(string inZone)
    {
        if (insideZone == false)
        {
            extendingString = true;

            comboString += inZone;

            extendingString = false;
        }

        Debug.Log(comboString);

        yield return null;
    }


}
