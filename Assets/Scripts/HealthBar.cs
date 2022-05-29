using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    //CONFIGS
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] float healthDrainSpeed = 10f;
    [SerializeField] float prayerHealthDrainSpeed = 0f;

    //STATES
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isDrainingFromPrayer = false;

    //CACHES
    RectTransform healthRectTransform;
    public LevelMaster levelMaster;
    public LevelConfig levelConfig;


    private void Awake()
    {
        healthRectTransform = transform.Find("Health").GetComponent<RectTransform>();
    }

    void Start()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        levelConfig = levelMaster.GetLevelConfig();
        maxHealth = levelMaster.maxHealth;
        currentHealth = levelMaster.currentHealth;
        healthDrainSpeed = levelConfig.healthDrainSpeed;

        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (levelMaster != null) { levelMaster = LevelMaster.GetThisSingletonScript(); }
        if (isAlive) 
        { 
            DrainHealthOverTime();
            if (isDrainingFromPrayer)
            {
                DrainHealthFromPrayer();
            }
        }
        UpdateHealthBar();
    }

    void DrainHealthOverTime()
    {
        currentHealth -= Time.deltaTime * healthDrainSpeed;
        CheckIfDead();
    }

    void CheckIfDead()
    {
        levelMaster.UpdateCurrentHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isAlive = false;
            levelMaster.GameOver();
        } 
    }

    void UpdateHealthBar()
    {
        var ratio = (currentHealth / maxHealth);
        healthRectTransform.localScale = new Vector3(ratio, 1, 0);
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        CheckIfDead();
    }

    public void StartDrainHealthFromPrayer(float drainSpeed)
    {
        prayerHealthDrainSpeed = drainSpeed;
        isDrainingFromPrayer = true;
    }

    public void StopDrainHealthFromPrayer()
    {
        prayerHealthDrainSpeed = 0;
        isDrainingFromPrayer = false;
    }

    public void ModifyPrayerDrainSpeed(float amount)
    {
        prayerHealthDrainSpeed += amount;
    }

    void DrainHealthFromPrayer()
    {
        currentHealth -= Time.deltaTime * prayerHealthDrainSpeed;
        CheckIfDead();
    }
}
