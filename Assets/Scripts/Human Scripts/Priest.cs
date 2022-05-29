using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Human
{
    [Header("Priest Settings")]
    [SerializeField] float prayDurationMin = 2f;
    [SerializeField] float prayDurationMax = 5f;
    [SerializeField] float prayerDrainSpeed = 2f;
    [SerializeField] int prayingProbability = 70;

    [SerializeField] bool isPraying = false;

    //CACHE
    PriestPrayer priestPrayer;

    protected override void Awake()
    {
        SetupHumanCache();
        isDespawnable = false;

        // Priest cache
        priestPrayer = transform.Find("Prayer AoE").gameObject.GetComponent<PriestPrayer>();
    }

    protected override void Start()
    {
        SetupHumanConstants();
        SetupPriestConstants();

        UpdateAnimatorParam();
        UpdateFaceDirection();
        StartCoroutine(WalkAndIdle());
    }

    void SetupPriestConstants()
    {
        prayDurationMin = levelMaster.priestPrayDurationMin;
        prayDurationMax = levelMaster.priestPrayDurationMax;
        prayingProbability = levelMaster.priestPrayingProbability;

        grantedHealth = levelMaster.priestGrantedHealth;
        prayerDrainSpeed = levelConfig.priestPrayerDrainSpeed;
        
    }   
    
    protected override void SetActiveMovementState(string varName)
    {
        isWalking = false;
        isRunning = false;
        isIdling = false;
        isPanic = false;
        isPraying = false;

        if (varName == "isWalking") { isWalking = true; }
        else if (varName == "isRunning") { isRunning = true; }
        else if (varName == "isIdling") { isIdling = true; }
        else if (varName == "isPanic") { isPanic = true; }
        else if (varName == "isPraying") { isPraying = true; }

        UpdateAnimatorParam();
        UpdatePriestPrayer();
    }

    void UpdatePriestPrayer()
    {
        if (isPraying || isRunning) { priestPrayer.ActivatePrayerAoE(); }
        else { priestPrayer.DeactivatePrayerAoE(); }
    }

    protected override void UpdateAnimatorParam()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isIdling", isIdling);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isPanic", isPanic);
        animator.SetBool("isPraying", isPraying);
    }

    void SetToPraying() { SetActiveMovementState("isPraying"); }

    protected override IEnumerator WalkAndIdle() // now theres chance to pray instead of idle
    {
        while (true) // only stop when StopCoroutine called from outside
        {
            var walkDuration = Random.Range(walkDurationMin, walkDurationMax);
            var idleDuration = Random.Range(idleDurationMin, idleDurationMax);
            var prayDuration = Random.Range(prayDurationMin, prayDurationMax);

            SetToWalking();
            yield return new WaitForSeconds(walkDuration);

            var rand = Random.Range(0, 99);
            if(rand < prayingProbability)
            {
                SetToPraying();
                yield return new WaitForSeconds(prayDuration);
            }
            else
            {
                SetToIdling();
                yield return new WaitForSeconds(idleDuration);
            }


            SetNewFaceDirection();
        }
    }

    public float GetPrayerDrainSpeed() => prayerDrainSpeed;

}
