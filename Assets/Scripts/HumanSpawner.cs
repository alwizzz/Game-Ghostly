using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    //CONFIG
    [SerializeField] int humanCount = 10;
    [SerializeField] float humanSpawnProbability;
    [SerializeField] int priestCount = 10;
    [SerializeField] float priestSpawnProbability;
    [SerializeField] int innocentCount = 10;
    [SerializeField] float innocentSpawnProbability;

    [SerializeField] float firstSpawnDelayMin = 3f;
    [SerializeField] float firstSpawnDelayMax = 5f;
    [SerializeField] float spawnIntervalMin = 5f;
    [SerializeField] float spawnIntervalMax = 10f;
    // Spawning can happen in single instance or group of instances with their own interval
    [SerializeField] int spawnGroupMin = 2;
    [SerializeField] int spawnGroupMax = 2;
    [SerializeField] float spawnGroupIntervalMin = 2f;
    [SerializeField] float spawnGroupIntervalMax = 2f;
    [SerializeField] int spawnInGroupProbability = 40;

    //STATES
    [SerializeField] bool isFacingRight = false;
    bool humanAvailable;
    bool priestAvailable;
    bool innocentAvailable;

    //CACHE 
    [SerializeField] Human humanPrefab;
    [SerializeField] Human priestPrefab;
    [SerializeField] Human innocentPrefab;
    [SerializeField] LevelConfig levelConfig;
    [SerializeField] LevelMaster levelMaster;

    //COLLECTION
    float[] lanes = new float[] { 1f, 0.8f, 0.6f };

    IEnumerator Start()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        levelConfig = levelMaster.GetLevelConfig();
        Setup();
        UpdateAvailability();

        var firstSpawnDelay = Random.Range(firstSpawnDelayMin, firstSpawnDelayMax);
        yield return new WaitForSeconds(firstSpawnDelay);

        StartCoroutine(Spawning());
    }

    void Setup()
    {
        humanCount = levelConfig.humanCount;
        humanSpawnProbability = levelConfig.humanSpawnProbability;
        priestCount = levelConfig.priestCount;
        priestSpawnProbability = levelConfig.priestSpawnProbability;
        innocentCount = levelConfig.innocentCount;
        innocentSpawnProbability = levelConfig.innocentSpawnProbability;


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
        while(humanAvailable || priestAvailable || innocentAvailable)
        {
            int randSpawnGroup = Random.Range(0, 99);
            if(randSpawnGroup < spawnInGroupProbability)
            {
                int toBeSpawnedInGroupCount = Random.Range(spawnGroupMin, spawnGroupMax);
                StartCoroutine(SpawnGroup(toBeSpawnedInGroupCount));
            } else
            {
                SpawnSingle();
            }

            float interval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnGroup(int count)
    {
        //Debug.Log("spawned in group from" + gameObject.name);
        for(int i=0; i<count; i++)
        {
            Spawn();
            float interval = Random.Range(spawnGroupIntervalMin, spawnGroupIntervalMax);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnSingle()
    { 
        //Debug.Log("spawned single from" + gameObject.name);
        Spawn();
    }

    void Spawn()
    {
        int laneIndex = GetRandomLaneIndex();
        Vector3 spawnPosition = new Vector3(transform.position.x, lanes[laneIndex], 0);

        var prefabToBeSpawned = GetRandomPrefab();

        var spawned = Instantiate(
            prefabToBeSpawned.gameObject,
            spawnPosition,
            Quaternion.identity
        );
        spawned.GetComponent<Human>().SetNewFaceDirection(this.isFacingRight);
        spawned.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = laneIndex;
        spawned.transform.parent = transform;

        DecrementCount(prefabToBeSpawned);
    }

    void DecrementCount(Human prefab)
    {
        if(prefab == humanPrefab) { humanCount--; }
        else if (prefab == priestPrefab) { priestCount--; }
        else if (prefab == innocentPrefab) { innocentCount--; }

        UpdateAvailability();
    }

    void UpdateAvailability()
    {
        humanAvailable = (humanCount > 0) ? true : false;
        priestAvailable = (priestCount > 0) ? true : false;
        innocentAvailable = (innocentCount > 0) ? true : false;
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

    Human GetRandomPrefab()
    {
        var rand = Random.Range(0,99);

        if(humanAvailable && priestAvailable && innocentAvailable) //condition 1: all human type available
        {
            if (rand > 0 && rand <= humanSpawnProbability) { return humanPrefab; }
            else if (rand > humanSpawnProbability && rand <= humanSpawnProbability+priestSpawnProbability) { return priestPrefab; }
            else /*(rand > humanSpawnProbability + priestSpawnProbability && rand <= 1f) */{ return innocentPrefab; }
        } 
        else if(!humanAvailable && priestAvailable && innocentAvailable) //condition 2: human type not available
        {
            var newPriestSpawnProbability = priestSpawnProbability + (humanSpawnProbability/2);
            var newInnocentSpawnProbability = innocentSpawnProbability + (humanSpawnProbability / 2);
            if (rand > newPriestSpawnProbability && rand <= newPriestSpawnProbability + newInnocentSpawnProbability) { return priestPrefab; }
            else /* (rand > newPriestSpawnProbability + newInnocentSpawnProbability && rand <= 1f)*/ { return innocentPrefab; }
        }
        else if (humanAvailable && !priestAvailable && innocentAvailable) //condition 3: priest type not available
        {
            var newHumanSpawnProbability = humanSpawnProbability + (priestSpawnProbability / 2);
            var newInnocentSpawnProbability = innocentSpawnProbability + (priestSpawnProbability / 2);
            if (rand > newHumanSpawnProbability && rand <= newHumanSpawnProbability + newInnocentSpawnProbability) { return humanPrefab; }
            else /* (rand > newHumanSpawnProbability + newInnocentSpawnProbability && rand <= 1f)*/ { return innocentPrefab; }
        }
        else if (humanAvailable && priestAvailable && !innocentAvailable) //condition 4: innocent type not available
        {
            var newHumanSpawnProbability = humanSpawnProbability + (innocentSpawnProbability / 2);
            var newPriestSpawnProbability = priestSpawnProbability + (innocentSpawnProbability / 2);
            if (rand > newHumanSpawnProbability && rand <= newHumanSpawnProbability + newPriestSpawnProbability) { return humanPrefab; }
            else /* (rand > newHumanSpawnProbability + newPriestSpawnProbability && rand <= 1f) */ { return priestPrefab; }
        }
        else if (humanAvailable && !priestAvailable && !innocentAvailable) //condition 5: only human type available
        {
            return humanPrefab;
        }
        else if (!humanAvailable && priestAvailable && !innocentAvailable) //condition 6: only priest type available
        {
            return priestPrefab;
        }
        else // (!humanAvailable && !priestAvailable && innocentAvailable) //condition 7: only innocent type available
        {
            return innocentPrefab;
        }

    }

}
