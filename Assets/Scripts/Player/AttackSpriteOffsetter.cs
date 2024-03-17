using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpriteOffsetter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.transform.position;
    }
}
