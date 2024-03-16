using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerAI : MonoBehaviour
{
    [Header("Base Stats")]
    public int health;
    public float fireRate;
    public float aggroRange;

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

    void Start()
    {
        player = GameObject.Find("Player");

        StartCoroutine(RegularFireCD());
        StartCoroutine(SpinAttackCD());
        StartCoroutine(DeadSummonCD());
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < aggroRange)
        {
            isAggro = true;
        }
        else{
            isAggro = false;
        }
    }

    IEnumerator RegularFireCD()
    {
        if (isAggro)
        {
            //Shoot Bullet
            Debug.Log("Shot");
        }

        yield return new WaitForSeconds(fireRate);
        StartCoroutine(RegularFireCD());
    }

    IEnumerator SpinAttackCD()
    {
        if (isAggro && shotsFired >= spinAttackAfterRegularShots)
        {
            for (int i = 0; i < shotsDuringSpin; i++)
            {
                yield return new WaitForSeconds(spinDuration / shotsDuringSpin);

                //Shoot Bullet
                Debug.Log("SpinAttackShot");
            }

            shotsFired = 0;
        }

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SpinAttackCD());
    }

    IEnumerator DeadSummonCD()
    {
        if (isAggro)
        {
            yield return new WaitForSeconds(summonCoolDown);
            
            for (int i = 0; i < Random.Range(spawnMinMax.x, spawnMinMax.y); i++)
            {
                int rnd = Random.Range(0, 1);
                int rndSpawn = Random.Range(0, spawnPositions.Length);

                if (rnd == 0)
                {
                    Instantiate(meleeAi, spawnPositions[rndSpawn].position, transform.rotation);
                }else{
                    Instantiate(rangedAi, spawnPositions[rndSpawn].position, transform.rotation);
                }
            }
        }   
        else{
            yield return new WaitForSeconds(0.25f);
        }

        StartCoroutine(DeadSummonCD());
    }
}
