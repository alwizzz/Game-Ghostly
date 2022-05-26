using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Human
{
    [Header("Priest Settings")]
    public float prayDurationMin = 2f;
    public float prayDurationMax = 5f;
    public float prayerDrainSpeed = 2f;

    public bool isPraying = false;
    public int prayingProbability = 70;

    //CACHE
    PriestPrayer priestPrayer;

    protected override void Awake()
    {
        SetupHumanCache();
        priestPrayer = transform.Find("Prayer AoE").gameObject.GetComponent<PriestPrayer>();
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
        if (isPraying) { priestPrayer.ActivatePrayerAoE(); }
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

}
