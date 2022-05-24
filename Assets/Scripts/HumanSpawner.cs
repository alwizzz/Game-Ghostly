using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    //CONFIG
    public int humanCount = 10;
    public float firstSpawnDelayMin = 3f;
    public float firstSpawnDelayMax = 5f;
    public float spawnIntervalMin = 5f;
    public float spawnIntervalMax = 10f;
    // Spawning can happen in single instance or group of instances with their own interval
    public int spawnGroupMin = 2;
    public int spawnGroupMax = 2;
    public float spawnGroupIntervalMin = 2f;
    public float spawnGroupIntervalMax = 2f;
    public int spawnInGroupProbability = 40;

    //STATES
    public bool isFacingRight = false;

    //CACHE
    public GameObject humanPrefab;

    IEnumerator Start()
    {
        var firstSpawnDelay = Random.Range(firstSpawnDelayMin, firstSpawnDelayMax);
        yield return new WaitForSeconds(firstSpawnDelay);

        StartCoroutine(Spawning());
    }

    IEnumerator Spawning()
    {
        while(humanCount > 0)
        {
            int randSpawnGroup = Random.Range(0, 99);
            if(randSpawnGroup < spawnInGroupProbability)
            {
                int toBeSpawnedInGroupCount = Random.Range(spawnGroupMin, spawnGroupMax);
                StartCoroutine(SpawnGroup(humanPrefab, toBeSpawnedInGroupCount));
            } else
            {
                SpawnSingle(humanPrefab);
            }

            float interval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnGroup(GameObject prefab, int count)
    {
        //Debug.Log("spawned in group from" + gameObject.name);
        for(int i=0; i<count; i++)
        {
            Spawn(prefab);
            float interval = Random.Range(spawnGroupIntervalMin, spawnGroupIntervalMax);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnSingle(GameObject prefab)
    { 
        //Debug.Log("spawned single from" + gameObject.name);
        Spawn(prefab);
    }

    void Spawn(GameObject prefabToBeSpawned)
    {
        var spawned = Instantiate(
            prefabToBeSpawned,
            transform.position,
            Quaternion.identity
        );
        spawned.GetComponent<Human>().isFacingRight = this.isFacingRight;
        spawned.transform.parent = transform;

        humanCount--;
    }

}
