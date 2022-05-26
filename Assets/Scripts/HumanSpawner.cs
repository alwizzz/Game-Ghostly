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
    public Human humanPrefab;
    public Human priestPrefab;
    public LevelConfig levelConfig;

    //COLLECTION
    float[] lanes = new float[] { 1f, 0.8f, 0.6f };

    void Awake()
    {
        
    }

    IEnumerator Start()
    {
        levelConfig = FindObjectOfType<LevelMaster>().GetLevelConfig();
        Setup();

        var firstSpawnDelay = Random.Range(firstSpawnDelayMin, firstSpawnDelayMax);
        yield return new WaitForSeconds(firstSpawnDelay);

        StartCoroutine(Spawning());
    }

    void Setup()
    {
        humanCount = levelConfig.humanCount;
        firstSpawnDelayMin = levelConfig.firstSpawnDelayMin;
        firstSpawnDelayMax = levelConfig.firstSpawnDelayMax;
        spawnGroupIntervalMin = levelConfig.spawnGroupIntervalMin;
        spawnGroupIntervalMax = levelConfig.spawnGroupIntervalMax;
        spawnGroupMin = levelConfig.spawnGroupMin;
        spawnGroupMax = levelConfig.spawnGroupMax;
        spawnGroupIntervalMin = levelConfig.spawnGroupIntervalMin;
        spawnGroupIntervalMax = levelConfig.spawnGroupIntervalMax;
        spawnInGroupProbability = levelConfig.spawnInGroupProbability;
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

    IEnumerator SpawnGroup(Human prefab, int count)
    {
        //Debug.Log("spawned in group from" + gameObject.name);
        for(int i=0; i<count; i++)
        {
            Spawn(prefab);
            float interval = Random.Range(spawnGroupIntervalMin, spawnGroupIntervalMax);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnSingle(Human prefab)
    { 
        //Debug.Log("spawned single from" + gameObject.name);
        Spawn(prefab);
    }

    void Spawn(Human prefabToBeSpawned)
    {
        int laneIndex = GetRandomLaneIndex();
        Vector3 spawnPosition = new Vector3(transform.position.x, lanes[laneIndex], 0);

        var spawned = Instantiate(
            prefabToBeSpawned.gameObject,
            spawnPosition,
            Quaternion.identity
        );
        spawned.GetComponent<Human>().isFacingRight = this.isFacingRight;
        spawned.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = laneIndex;
        spawned.transform.parent = transform;

        humanCount--;
    }

    int GetRandomLaneIndex()
    {
        var rand = Random.value;
        if(rand > 0 && rand <= 0.33f)
        {
            return 0;
        } 
        else if(rand > 0.33f && rand <= 0.66f)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

}
