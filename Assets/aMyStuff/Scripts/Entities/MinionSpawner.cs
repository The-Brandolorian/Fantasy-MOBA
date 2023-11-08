using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject minionPrefab;
    public GameObject enemyBase;

    public bool performSpawn = true;
    public float spawnInterval;
    public float nextSpawnTime = 0;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        // Attempt to find opposing base if not allocated.
        if (enemyBase == null)
        {
            if (gameObject.tag == "Enemy") enemyBase = GameObject.Find("Base (Player)");
            else enemyBase = GameObject.Find("Base (Enemy)");
        }
    }

    void Update()
    {
        // Do spawn each interval.
        if (performSpawn && Time.time > nextSpawnTime)
        {
            performSpawn = false;
            Spawn();
        }
    }

    public void Spawn()
    {
        // Randomly spawn a minion at 1 of 2 spawn points, set their tag and layer and then make them walk to the enemy tower.
        Vector3 spawnPos = Vector3.zero;
        float randValue = Random.value;
        if (randValue < 0.5f) spawnPos = spawn1.transform.position;
        else spawnPos = spawn2.transform.position;

        GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
        minion.tag = gameObject.tag;
        minion.layer = gameObject.layer;

        AI_Movement movement = minion.GetComponent<AI_Movement>();
        movement.enemyBase = enemyBase;
        movement.MoveToTarget(enemyBase);

        nextSpawnTime = Time.time + spawnInterval;
        performSpawn = true;
    }

    private void OnDestroy()
    {
        Application.Quit();
    }
}
