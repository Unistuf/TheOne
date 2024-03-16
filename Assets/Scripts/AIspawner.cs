using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIspawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 1f;
    [SerializeField] bool canSpawn = false;

    [SerializeField] GameObject player;

    [Header("HOSTILE PREFABS")]
    [SerializeField] GameObject[] enemyPrefabs;


    public float inRange = 5f;

    void Start()
    {
        player = GameObject.Find("Player");

        StartCoroutine(Spawner());
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < inRange)
        {
            canSpawn = true;
        }
        else
        {
            canSpawn = false;
        }
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn)
        {
            yield return wait;

            int rand = Random.Range(0, enemyPrefabs.Length);                                // Gets listed Prefabs of Hostiles
            GameObject enemyToSpawn = enemyPrefabs[rand];                                   // Picks randomly what AI to spawn (not random according to Mini max but mini max can stfu)
            Instantiate(enemyToSpawn, transform.position, Quaternion.Euler(0, 0, 0));       // Spawns Hostile within Scene
        }
    }
}
