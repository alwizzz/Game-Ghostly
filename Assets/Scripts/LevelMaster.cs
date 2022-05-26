using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour
{
    public bool isPlaying = true;

    public LevelConfig[] levelConfigs;
    LevelConfig currentLevelConfig;
    public int currentLevel = 2;

    public float timerMax = 30f;
    public float timer;

    public float currentHealth;
    public float maxHealth = 100f;

    SceneLoader sceneLoader;

    private void Awake()
    {
        Singleton();

        currentLevelConfig = levelConfigs[currentLevel - 1];
        sceneLoader = FindObjectOfType<SceneLoader>();

        if(currentHealth == 0f) { currentHealth = maxHealth; } // float will be 0f as default value, not null
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (isPlaying) { CountdownTimer(); }                                        
    }

    void Singleton()
    {
        var thisScriptCount = FindObjectsOfType<LevelMaster>().Length;
        if(thisScriptCount > 1)
        {
            Destroy(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public LevelConfig GetLevelConfig() => currentLevelConfig;

    void ResetTimer() { timer = timerMax; }
    void CountdownTimer() { 
        timer -= Time.deltaTime; 

        if(timer <= 0) { TimeUp(); }
    }

    void TimeUp()
    {
        isPlaying = false;
        ResetTimer();
        sceneLoader.LoadTransitionScene();
    }

    public void UpdateCurrentHealth(float newHealth) { currentHealth = newHealth; }

    public void Lose()
    {
        isPlaying = false;
        sceneLoader.LoadGameOverScene();
    }

    public void StartGame()
    {
        isPlaying = true;
        currentLevelConfig = levelConfigs[currentLevel - 1];
        ResetTimer();
    }

    public void LevelUp()
    {
        currentLevel++;
        var max = levelConfigs.Length;
        if(currentLevel > max)
        {
            currentLevel = max;
        }
    }

}
