using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour
{
    [Header("Game Status")]
    public bool isPlaying = true;
    public bool isIntense = false;
    public int currentLevel = 2;
    public float timer;
    public float currentHealth;
    public int currentScore;
    public int currentPanickedHumanCount;

    [Header("Game Constants")]
    public float timerMax = 30f;
    public float maxHealth = 100f;
    public int intenseMinimumCount = 10;

    [Header("Level Config")]
    [SerializeField] LevelConfig currentLevelConfig;
    [SerializeField] LevelConfig[] levelConfigs;

    [Header("Granted Healths")]
    public float humanGrantedHealth = 5f;
    public float priestGrantedHealth = 5f;
    public float innocentGrantedHealth = -5f;



    [Header("Ghost Constants")]
    public float playerMoveSpeed = 5f;
    public float devourYOffset = 0.16f;

    [Header("Human Constants")]
    public float humanWalkSpeed = 0.5f;
    public float humanRunSpeed = 1.5f;
    public float humanWalkDurationMin = 4f;
    public float humanWalkDurationMax = 11f;
    public float humanIdleDurationMin = 2f;
    public float humanIdleDurationMax = 5f;
    public float humanPanicDuration = 1.5f;
    public int humanChangeFacingDirectionProbability = 1;
    public float humanChaoticRunningDurationMin = 2f;
    public float humanChaoticRunningDurationMax = 4f;

    [Header("Priest Constants")]
    public float priestPrayDurationMin = 2f;
    public float priestPrayDurationMax = 5f;
    public float priestPrayerDrainSpeed = 2f;
    public int priestPrayingProbability = 70;

    // CACHE
    SceneLoader sceneLoader;
    MusicManager musicManager;

    private void Awake()
    {
        Singleton();

        UpdateLevelConfig();
        sceneLoader = SceneLoader.GetThisSingletonScript();
        musicManager = MusicManager.GetThisSingletonScript();

        currentHealth = maxHealth;
        currentScore = 0;
        ResetTimer(); 
    }

    private void Start()
    {
        if (sceneLoader.IsInTheGameScene()) { StartGame(); } //only automatically start when in TheGame scene
        ResetIntensity();
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
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public static LevelMaster GetThisSingletonScript()
    {
        var list = FindObjectsOfType<LevelMaster>();
        return list[list.Length - 1];
    }

    public LevelConfig GetLevelConfig() => currentLevelConfig;

    void UpdateLevelConfig()
    {
        int maxLevelConfig = levelConfigs.Length;
        if(currentLevel > maxLevelConfig)
        {
            currentLevelConfig = levelConfigs[maxLevelConfig - 1];
        } else
        {
            currentLevelConfig = levelConfigs[currentLevel - 1];
        }
    }

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
    public void UpdateCurrentScore(int newScore) { currentScore = newScore; }

    public void Lose()
    {
        isPlaying = false;
        sceneLoader.LoadGameOverScene();
    }

    public void StartGame()
    {
        isPlaying = true;
        UpdateLevelConfig();
        ResetTimer();
    }

    public void GameOver()
    {
        isPlaying = false;
        sceneLoader.LoadGameOverScene();
    }

    public void LevelUp()
    {
        currentLevel++;
    }

    public void RestartGame()
    {
        currentHealth = maxHealth;
        currentScore = 0;
        ResetTimer();

        currentLevel = 1;
    }

    public void IncrementPanickedHumanCount() 
    { 
        currentPanickedHumanCount++; 
        if(currentPanickedHumanCount >= intenseMinimumCount && !isIntense)
        {
            IntenseMode();
        }
    }

    void IntenseMode()
    {
        isIntense = true;
        musicManager.PlayPlayingIntenseTrack();
    }

    public void ResetIntensity()
    {
        currentPanickedHumanCount = 0;
        isIntense = false;
    }

}
