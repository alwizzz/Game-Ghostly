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
    public float panicDuration = 2f;
    public int changeFacingDirectionProbability = 1;

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

    [Header("Caches")]
    Player player;
    Animator animator;
    SpriteRenderer spriteRenderer;
    SpriteRenderer exclamationMarkRenderer;
    HumanVision thisHumanVision;

    //[Header("Configs")]
    //[Header("States")]
    //[Header("Animator Params")]
    //[Header("Caches")]


    private void Awake()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        thisHumanVision = transform.Find("Vision").GetComponent<HumanVision>();
        exclamationMarkRenderer = transform.Find("Exclamation Mark").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateAnimatorParam();
        UpdateFaceDirection();
        StartCoroutine(WalkAndIdle());
    }

    private void Update()
    {
        if (!isBeingDevoured)
        {
            if (isWalking) { Walk(); }
            else if(isRunning) { Run(); }
        }
    }

    IEnumerator WalkAndIdle()
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

    public void Alerted()
    {
        StopAllCoroutines();
        StartCoroutine(PanicAndRun());
    }

    IEnumerator PanicAndRun()
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

    void Walk()
    {
        transform.Translate(Time.deltaTime * walkSpeed * DirectionVector());
    }

    void Run()
    {
        transform.Translate(Time.deltaTime * runSpeed * DirectionVector());
    }

    void SetToWalking() { SetActiveMovementState("isWalking"); }
    void SetToIdling() { SetActiveMovementState("isIdling"); }
    void SetToPanic() { SetActiveMovementState("isPanic"); }
    void SetToRunning() { SetActiveMovementState("isRunning"); }


    void SetNewFaceDirection()
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

    void UpdateFaceDirection()
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

    void SetActiveMovementState(string varName)
    {
        if(varName == "isWalking")
        {
            isWalking = true;
            isRunning = false;
            isIdling = false;
            isPanic = false;
        } else if(varName == "isRunning")
        {
            isWalking = false;
            isRunning = true;
            isIdling = false;
            isPanic = false;
        } else if (varName == "isIdling")
        {
            isWalking = false;
            isRunning = false;
            isIdling = true;
            isPanic = false;
        } else if (varName == "isPanic")
        {
            isWalking = false;
            isRunning = false;
            isIdling = false;
            isPanic = true;
        } 
        UpdateAnimatorParam();
    }

    void UpdateAnimatorParam()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isIdling", isIdling);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isPanic", isPanic);
    }

    private void OnMouseDown()
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
