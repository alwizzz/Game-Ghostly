//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] protected float walkSpeed = 0.5f;
    [SerializeField] protected float runSpeed = 1.5f;
    [SerializeField] protected float walkDurationMin = 4f;
    [SerializeField] protected float walkDurationMax = 11f;
    [SerializeField] protected float idleDurationMin = 2f;
    [SerializeField] protected float idleDurationMax = 5f;
    [SerializeField] protected float panicDuration = 1.5f;
    [SerializeField] protected int changeFacingDirectionProbability = 1;
    [SerializeField] protected float chaoticRunningDurationMin = 2f;
    [SerializeField] protected float chaoticRunningDurationMax = 5f;


    [Header("Health Values")]
    [SerializeField] protected float grantedHealth = 5f;

    [Header("States")]
    [SerializeField] protected bool isBeingChased = false;
    [SerializeField] protected bool isBeingDevoured = false;
    [SerializeField] protected bool isPanic = false;
    [SerializeField] protected bool isFacingRight = false;
    [Header("Animator Params")]
    [SerializeField] protected bool isWalking = false; 
    [SerializeField] protected bool isRunning = false; 
    [SerializeField] protected bool isIdling = false;
    //public bool isForcedIdling = false;

    //[Header("Caches")]
    protected Player player;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected SpriteRenderer exclamationMarkRenderer;
    protected HumanVision thisHumanVision;
    protected LevelMaster levelMaster;

    //[Header("Configs")]
    //[Header("States")]
    //[Header("Animator Params")]
    //[Header("Caches")]


    protected virtual void Awake()
    {
        SetupHumanCache();
    }

    protected virtual void Start()
    {
        SetupHumanConstants();

        UpdateAnimatorParam();
        UpdateFaceDirection();
        StartCoroutine(WalkAndIdle());
    }

    protected void Update()
    {
        if (!isBeingDevoured)
        {
            if (isWalking) { Walk(); }
            else if(isRunning) { Run(); }
        }
    }

    protected void SetupHumanConstants()
    {
        walkSpeed = levelMaster.humanWalkSpeed;
        runSpeed = levelMaster.humanRunSpeed;
        walkDurationMin = levelMaster.humanWalkDurationMin;
        walkDurationMax = levelMaster.humanWalkDurationMax;
        idleDurationMin = levelMaster.humanIdleDurationMin;
        idleDurationMax = levelMaster.humanIdleDurationMax;
        panicDuration = levelMaster.humanPanicDuration;
        changeFacingDirectionProbability = levelMaster.humanChangeFacingDirectionProbability;
        grantedHealth = levelMaster.humanGrantedHealth;
        chaoticRunningDurationMin = levelMaster.humanChaoticRunningDurationMin;
        chaoticRunningDurationMax = levelMaster.humanChaoticRunningDurationMax;
    }

    protected void SetupHumanCache()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        thisHumanVision = transform.Find("Vision").GetComponent<HumanVision>();
        exclamationMarkRenderer = transform.Find("Exclamation Mark").GetComponent<SpriteRenderer>();
    }

    protected virtual IEnumerator WalkAndIdle()
    {
        while (true) // only stop when StopCoroutine called from outside
        {
            var walkDuration = Random.Range(walkDurationMin, walkDurationMax);
            var idleDuration = Random.Range(idleDurationMin, idleDurationMax);

            SetToWalking();
            yield return new WaitForSeconds(walkDuration);

            SetToIdling();
            yield return new WaitForSeconds(idleDuration);

            SetNewFaceDirection();
        }
    }

    public virtual void Alerted()
    {
        //Debug.Log("alerted in Human Script");
        StopAllCoroutines();
        StartCoroutine(PanicAndRun());
    }

    protected IEnumerator PanicAndRun()
    {
        SetToPanic();
        exclamationMarkRenderer.enabled = true;
        yield return new WaitForSeconds(panicDuration);
        exclamationMarkRenderer.enabled = false;



        SetToRunning();
        StartCoroutine(ChaoticRunning());
    }

    IEnumerator ChaoticRunning()
    {
        // will first run to the reversed direction
        isFacingRight = !isFacingRight; //flip the value
        UpdateFaceDirection();

        var firstChaos = Random.Range(chaoticRunningDurationMin, chaoticRunningDurationMax);
        yield return new WaitForSeconds(firstChaos);

        // then facing back to the previous value
        isFacingRight = !isFacingRight; //flip the value
        UpdateFaceDirection();

        var secondChaos = Random.Range(chaoticRunningDurationMin, chaoticRunningDurationMax);
        yield return new WaitForSeconds(secondChaos);

        // finally run to the first time running direction
        isFacingRight = !isFacingRight; //flip the value
        UpdateFaceDirection();

        var thirdChaos = Random.Range(chaoticRunningDurationMin, chaoticRunningDurationMax);
        yield return new WaitForSeconds(thirdChaos);
    }

    protected void Walk()
    {
        transform.Translate(Time.deltaTime * walkSpeed * DirectionVector());
    }

    protected void Run()
    {
        transform.Translate(Time.deltaTime * runSpeed * DirectionVector());
    }

    protected void SetToWalking() { SetActiveMovementState("isWalking"); }
    protected void SetToIdling() { SetActiveMovementState("isIdling"); }
    protected void SetToPanic() 
    { 
        SetActiveMovementState("isPanic");
        levelMaster.IncrementPanickedHumanCount();
    }
    protected void SetToRunning() { SetActiveMovementState("isRunning"); }

    public bool IsPanic() => isPanic;
    public bool IsRunning() => isRunning;
    public bool IsFacingRight() => isFacingRight;
    public bool IsBeingChased() => isBeingChased;
    public bool IsBeingDevoured() => isBeingDevoured;


    public float GetGrantedHealth() => grantedHealth;
    public void SetIsBeingChased(bool input) { isBeingChased = input; }



    public void SetNewFaceDirection(bool newDirection) { isFacingRight = newDirection; }

    protected void SetNewFaceDirection()
    {
        // new face direction is leaning to previous value 
        int rand = Random.Range(0, 99);
        if (isFacingRight) // currently facing right
        {
            isFacingRight = (rand < changeFacingDirectionProbability) ? false : true;
        } else // currently facing left
        {
            isFacingRight = (rand < changeFacingDirectionProbability) ? true : false;
        }

        UpdateFaceDirection();
    }

    protected void UpdateFaceDirection()
    {        
        if (!isFacingRight)
        {
            spriteRenderer.flipX = false;
            thisHumanVision.FlipVisionCollider(false);
        } else
        {
            spriteRenderer.flipX = true;
            thisHumanVision.FlipVisionCollider(true);
        }
    }

    Vector2 DirectionVector()
    {
        return (isFacingRight) ? Vector2.right : Vector2.left;
    }

    protected virtual void SetActiveMovementState(string varName)
    {
        isWalking = false;
        isRunning = false;
        isIdling = false;
        isPanic = false;

        if (varName == "isWalking") { isWalking = true; }
        else if (varName == "isRunning") { isRunning = true; }
        else if (varName == "isIdling") { isIdling = true; }
        else if (varName == "isPanic") { isPanic = true; }

        UpdateAnimatorParam();
    }

    protected virtual void UpdateAnimatorParam()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isIdling", isIdling);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isPanic", isPanic);
    }

    protected void OnMouseDown()
    {
        //Debug.Log("clicked on Human");
        isBeingChased = true;
        player.Chase(this);
    }

    public void Die() // called by Player
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 2f);
    }

    public void BeingDevoured()
    {
        isBeingChased = false;
        isBeingDevoured = true;
        animator.SetTrigger("forcedIdle"); // forced play idle animation
    }

    public void GoingToDespawn()
    {
        //Debug.Log("player being told to stop chasing");
        player.StopChasing();
    }

    public bool IsInNormalState()
    {
        return (!isPanic && !isRunning && !isBeingDevoured) ? true : false;
    }

    public void DetectedGhostAround()
    {
        Alerted();
    }

    
}
