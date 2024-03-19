using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIspawner : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] bool canSpawn = false;
    [SerializeField] bool isSpawning = false;
    
    [SerializeField] GameObject player;

    [Header("HOSTILE PREFABS")]
    [SerializeField] GameObject[] enemyPrefabs;


    public float inRange = 5f;

    void Start()
    {
        player = GameObject.Find("Player");

        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < inRange)
        {
            canSpawn = true;
            StartCoroutine(Spawner());
        }
        else
        {
            canSpawn = false;
        }
    }

    private IEnumerator Spawner()
    {
        if (canSpawn && !isSpawning)
        {
            isSpawning = true;

            int rand = Random.Range(0, enemyPrefabs.Length);                                // Gets listed Prefabs of Hostiles
            GameObject enemyToSpawn = enemyPrefabs[rand];                                   // Picks randomly what AI to spawn (not random according to Mini max but mini max can stfu)
            Instantiate(enemyToSpawn, transform.position, Quaternion.Euler(0, 0, 0));       // Spawns Hostile within Scene

            yield return new WaitForSeconds(timeBetweenSpawns);
            isSpawning = false;
        }
    }
}
