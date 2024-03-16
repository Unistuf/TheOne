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
    public string currentZone;

    string comboString;

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



        if (rStickPos.y >= zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth)
        {
            StartCoroutine(SwordAttacks("A"));
        }
        if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x >= zoneHeight)
        {
            StartCoroutine(SwordAttacks("B"));
        }
        if (rStickPos.y <= -zoneHeight && rStickPos.x <= zoneWidth && rStickPos.x >= -zoneWidth)
        {
            StartCoroutine(SwordAttacks("C"));
        }
        if (rStickPos.y <= zoneWidth && rStickPos.y >= -zoneWidth && rStickPos.x <= -zoneHeight)
        {
            StartCoroutine(SwordAttacks("D"));
        }
        if (rStickPos.y <= zoneHeight && rStickPos.y >= -zoneHeight && rStickPos.x <= zoneHeight && rStickPos.x >= -zoneHeight)
        {
            StartCoroutine(SwordAttacks("N"));
        }
    }

    public IEnumerator SwordAttacks(string inZone)
    {
        if (insideZone == false)
        {
            insideZone = true;
            currentZone = inZone;

            if (inZone != "N")
            {
                comboString += inZone;
            }
        }

        if (inZone != currentZone)
        {
            insideZone = false;
        }

        Debug.Log(comboString);

        yield return null;
    }


}
