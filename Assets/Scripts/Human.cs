//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [Header("Configs")]
    public float walkSpeed = 0.5f;
    public float runSpeed = 1.5f;
    public float walkDurationMin = 4f;
    public float walkDurationMax = 11f;
    public float idleDurationMin = 2f;
    public float idleDurationMax = 5f;
    public float panicDuration = 1.5f;
    public int changeFacingDirectionProbability = 1;

    [Header("Health Values")]
    public float grantedHealth = 5f;

    [Header("States")]
    public bool isBeingChased = false;
    public bool isBeingDevoured = false;
    public bool isPanic = false;
    public bool isFacingRight = false;
    [Header("Animator Params")]
    public bool isWalking = false; 
    public bool isRunning = false; 
    public bool isIdling = false;
    //public bool isForcedIdling = false;

    //[Header("Caches")]
    protected Player player;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected SpriteRenderer exclamationMarkRenderer;
    protected HumanVision thisHumanVision;

    //[Header("Configs")]
    //[Header("States")]
    //[Header("Animator Params")]
    //[Header("Caches")]


    protected virtual void Awake()
    {
        SetupHumanCache();
    }

    protected void Start()
    {
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

    protected void SetupHumanCache()
    {
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

        // will run to the reversed direction while alerted
        isFacingRight = !isFacingRight; //flip the value
        UpdateFaceDirection();
        SetToRunning();
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
    protected void SetToPanic() { SetActiveMovementState("isPanic"); }
    protected void SetToRunning() { SetActiveMovementState("isRunning"); }


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
        Destroy(gameObject);
    }

    public void BeingDevoured()
    {
        isBeingChased = false;
        isBeingDevoured = true;
        animator.SetTrigger("forcedIdle"); // forced play idle animation
    }
}
