using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float healthPoints;
    public GameObject hpBottleDrop;
    public float hpBottleDropChance;
    public GameObject player;

    [Header("Xp")]
    public int xpGain;

    void Start()
    {
        player = GameObject.Find("Player");
    }
public void DoDamage(float damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            DoDeath();
        }
    }

    void DoDeath()
    {
        if (hpBottleDropChance > Random.Range(0, 100))
        {
            Instantiate(hpBottleDrop, transform.position, transform.rotation);
        }
        //Death code

        player.GetComponent<XpLevelSystem>().AddPlayerXp(xpGain);
        Destroy(this.gameObject);
    }
}
