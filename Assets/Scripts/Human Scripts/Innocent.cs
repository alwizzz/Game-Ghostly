using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Innocent : Human
{

    protected override void Start()
    {
        SetupHumanConstants();
        SetupInnocentConstants();

        UpdateAnimatorParam();
        UpdateFaceDirection();
        StartCoroutine(WalkAndIdle());
    }

    void SetupInnocentConstants()
    {
        grantedHealth = levelMaster.innocentGrantedHealth;
    }
}
