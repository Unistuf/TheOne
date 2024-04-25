using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerAI : MonoBehaviour
{
    [Header("Base Stats")]
    public float health;
    public float fireRate;
    public float aggroRange;
    public float projectileLife;

    [Header("Necro stats")]
    public float summonCoolDown;
    public Transform[] spawnPositions;
    public Vector2 spawnMinMax;

    [Header("Spin Attack Stats")]
    public int spinAttackAfterRegularShots;
    public float spinDuration;
    public float shotsDuringSpin;
    public float spinSpeed;
    
    [Header("Prefabs")]
    public GameObject bullet;
    public GameObject meleeAi;
    public GameObject rangedAi;

    [Header("Runtime")]
    public bool isAggro;
    private GameObject player;
    private int shotsFired;

    [Header("Xp")]
    public int xpGain;

    void Start()
    {
        player = GameObject.Find("Player"); //Get player ref

        StartCoroutine(RegularFireCD());
        StartCoroutine(SpinAttackCD());
        StartCoroutine(DeadSummonCD());
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < aggroRange) //Check for player in range
        {
            isAggro = true;
        }
        else{
            isAggro = false;
        }
    }

    IEnumerator RegularFireCD()
    {
        if (isAggro)//If the player is in range fire a homing bullet
        {
            EnemyProjectileLogic currentProjectile = Instantiate(bullet, transform.position, Quaternion.identity, this.transform).GetComponent<EnemyProjectileLogic>();
            currentProjectile.target = player;
            currentProjectile.isHoming = true;
            currentProjectile.projectileSpeed = 3f;
            currentProjectile.projectileLifespan = projectileLife;

            shotsFired += 1; //Increase shots fired for the spin attack to trigger
        }

        yield return new WaitForSeconds(fireRate); //Wait for the firerate
        StartCoroutine(RegularFireCD());
    }

    IEnumerator SpinAttackCD() //Aoe Attack
    {
        if (isAggro && shotsFired >= spinAttackAfterRegularShots) //If player is in range and enough regular shots have been fired to trigger the spin attack
        {
            for (int i = 0; i < shotsDuringSpin; i++) //Cycle through the amount of shots during spin
            {
                for (int u = 0; u < spawnPositions.Length; u++) //Cycle through the spawn positions
                {
                    //Spawn homing bullets
                    EnemyProjectileLogic currentProjectile = Instantiate(bullet, spawnPositions[u].position, Quaternion.identity, this.transform).GetComponent<EnemyProjectileLogic>();
                    currentProjectile.target = player;
                    currentProjectile.isHoming = true;
                    currentProjectile.projectileSpeed = 3f;
                    currentProjectile.projectileLifespan = projectileLife;
                }

                yield return new WaitForSeconds(spinDuration / shotsDuringSpin); //Fire another shot after the spin duration (split amongst each spin attack)
            }

            shotsFired = 0; //Reset the amount of shots to trigger this move again
        }

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SpinAttackCD());
    }

    IEnumerator DeadSummonCD() //Summon a random amount of random Ai after a set amount of time 
    {
        if (isAggro)
        {
            yield return new WaitForSeconds(summonCoolDown);
            
            for (int i = 0; i < Random.Range(spawnMinMax.x, spawnMinMax.y); i++) //Spawns a random amount of enemies
            {
                int rnd = Random.Range(0, 1);
                int rndSpawn = Random.Range(0, spawnPositions.Length);

                if (rnd == 0)
                {
                    Instantiate(meleeAi, spawnPositions[rndSpawn].position, transform.rotation); //Spawn melee AI
                }else{
                    Instantiate(rangedAi, spawnPositions[rndSpawn].position, transform.rotation); //Spawn Ranged AI
                }
            }
        }   
        else{
            yield return new WaitForSeconds(0.25f);
        }

        StartCoroutine(DeadSummonCD());
    }

    public void DoDamage(float damage) //The damage system for the necromancer
    {
        health -= damage;

        if (health <= 0)
        {
            DoDeath();
        }
    }

    void DoDeath()
    {
        player.GetComponent<XpLevelSystem>().AddPlayerXp(xpGain); //Reward the player with xp when the necromancer dies
        Destroy(this.gameObject);
    }
}
