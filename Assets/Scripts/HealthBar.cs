using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    //CONFIGS
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthDrainSpeed = 10f;
    public float prayerHealthDrainSpeed = 0f;

    //STATES
    public bool isAlive = true;
    public bool isDrainingFromPrayer = false;

    //CACHES
    RectTransform healthRectTransform;
    LevelMaster levelMaster;

    private void Awake()
    {
        healthRectTransform = transform.Find("Health").GetComponent<RectTransform>();
        levelMaster = FindObjectOfType<LevelMaster>();
    }

    void Start()
    {
        maxHealth = levelMaster.maxHealth;
        currentHealth = levelMaster.currentHealth;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
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

    void DrainHealthFromPrayer()
    {
        currentHealth -= Time.deltaTime * prayerHealthDrainSpeed;
        CheckIfDead();
    }
}
