using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Healthbuff")]
public class HealthBuff : PowerUpEffect
{ 
    [Header("Health Increase Amount")]
    public float amount;

    public override void Apply(GameObject target)
    {
       
        // Add Health Ref to player here
        //                       \/
        // target.GetComponent<Health>()>health.value += amount;
    }
}
