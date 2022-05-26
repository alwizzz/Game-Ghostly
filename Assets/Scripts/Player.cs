using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //TODO: if Human is being chased and they move to offscreen, it still being chased resulting in Player went offscreen

    [Header("Configs")]
    public float moveSpeed = 10f;
    public int prayerAoECount = 0;
    public float defaultYvalue;
    public float devourYOffset = 0.1f;

    [Header("States")]
    public bool isMoving = false;
    public bool isInterruptable = true;
    public bool isHiding = true;
    public bool isChasing = false;
    public bool isInPrayerAoE = false;
    [Header("Animator Params")]
    public bool isDevouring = false; 

    //[Header("Caches")]
    int defaultSortingLayer;
    float moveDestination;
    SpriteRenderer spriteRenderer;
    BoxCollider2D presenceCollider;
    Animator animator;
    Human chasedHuman;
    HealthBar healthBar;

    protected virtual void Awake()
    {
        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        presenceCollider = transform.Find("Presence").GetComponent<BoxCollider2D>();
        healthBar = FindObjectOfType<HealthBar>();
    }

    private void Start()
    {

        defaultSortingLayer = spriteRenderer.sortingOrder;
        moveDestination = transform.position.x;
        defaultYvalue = transform.position.y;

        UpdateAnimatorParam();
        UpdatePresenceCollider();
    }


    // Update is called once per frame
    void Update()
    {
        spriteRenderer.enabled = (isHiding) ? false : true;

        if (isChasing)
        {
            UpdateChasedHumanPosition();
        }

        if (isMoving)
        {
            Move();
        }

        
    }

    void UpdateAnimatorParam()
    {
        animator.SetBool("isDevouring", isDevouring);
    }

    // Move Mechanic
    void Move()
    {
        if (isHiding) { isHiding = false; } // if currently hiding then make it false
        UpdatePresenceCollider();

        var distance = moveDestination - transform.position.x;
        //Debug.Log(distance);
        if (Mathf.Abs(distance) >= 0.1) // distance less than 0.05 = arrived
        {
            if(distance >= 0) // moving to right
            {
                FacingRight();
                transform.Translate(Time.deltaTime * moveSpeed * Vector2.right);
            } else // moving to left
            {
                FacingLeft();
                transform.Translate(Time.deltaTime * moveSpeed * Vector2.left);
            }
        } else // arrived
        {
            isMoving = false;
            if (isChasing) // if arrived while also chasing means player ready to devour
            {
                Devour();
            }
        }
    }

    // default facing is right
    void FacingRight() { spriteRenderer.flipX = false; }
    void FacingLeft() { spriteRenderer.flipX = true; }

    void MoveToPoint(float x)
    {
        isMoving = true;
        moveDestination = x;
    }

    public void MoveHere(float worldXPos) //called by MoveZone
    {
        if (!isInterruptable) { return; }

        if (isChasing) { StopChasing(); }
        MoveToPoint(worldXPos);
    }


    // Hide Mechanic
    public void Hide() //called by HidingSpot
    {
        if (!isInterruptable) { return; }

        if (isChasing) { StopChasing(); }

        MoveToPoint(0);
        StartCoroutine(Hiding());
    }

    IEnumerator Hiding()
    {
        while (isMoving)
        {
            yield return null;
        }
        isHiding = true;
        UpdatePresenceCollider();
    }


    // Chase and Devour Mechanic
    public void Chase(Human target) //called by Human
    {
        if (!isInterruptable) { return; }

        isChasing = true;
        chasedHuman = target;
        MoveToPoint(target.transform.position.x);
    }

    void StopChasing()
    {
        isChasing = false;
        chasedHuman.isBeingChased = false;
        chasedHuman = null;
    }

    void Devour()
    {
        isInterruptable = false; // this action is uninterruptable

        isDevouring = true;
        UpdateAnimatorParam();
        chasedHuman.BeingDevoured();
        MoveToDevouredHumanYValue(chasedHuman);

        spriteRenderer.sortingOrder = 10; // so it will appear on top of human
    }

    public void DevourFinished() // called by Ghost Devour animation event
    {
        BackToDefaultYValue();
        isDevouring = false;
        UpdateAnimatorParam();

        chasedHuman.Die();
        healthBar.AddHealth(chasedHuman.grantedHealth);
        chasedHuman = null; // delete chache

        isChasing = false;
        spriteRenderer.sortingOrder = defaultSortingLayer; // back to default 

        isInterruptable = true;
    }

    void MoveToDevouredHumanYValue(Human chasedHuman)
    {
        transform.position = new Vector2(transform.position.x, chasedHuman.transform.position.y - devourYOffset);
    }
    
    void BackToDefaultYValue()
    {
        transform.position = new Vector2(transform.position.x, defaultYvalue);
    }

    void UpdatePresenceCollider()
    {
        presenceCollider.enabled = (isHiding) ? false : true; 
    }

    void UpdateChasedHumanPosition()
    {
        moveDestination = chasedHuman.transform.position.x;
    }

    public void EnteredPrayerAoE(float drainSpeed)
    {
        prayerAoECount++;
        if (!isInPrayerAoE) //currently not in aoe
        {
            isInPrayerAoE = true;
            healthBar.StartDrainHealthFromPrayer(drainSpeed);
        } else //already in aoe
        {
            healthBar.ModifyPrayerDrainSpeed(+drainSpeed);
        }
    }

    public void ExitedPrayerAoE(float drainSpeed)
    {
        prayerAoECount--;
        if(prayerAoECount <= 0)
        {
            isInPrayerAoE = false;
            healthBar.StopDrainHealthFromPrayer();
        } else
        {
            healthBar.ModifyPrayerDrainSpeed(-drainSpeed);
        }
    }
}
