using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //TODO: if Human is being chased and they move to offscreen, it still being chased resulting in Player went offscreen

    [Header("Configs")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float defaultYValue;
    [SerializeField] float devourYOffset = 0.1f;

    [Header("States")]
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isInterruptable = true;
    [SerializeField] bool isHiding = true;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool isInPrayerAoE = false;
    [Header("Animator Params")]
    [SerializeField] bool isDevouring = false;

    [Header("SFX")]
    [SerializeField] AudioClip devourSFX;

    [Header("Others")]
    [SerializeField] int prayerAOECount = 0;

    //[Header("Caches")]
    int defaultSortingLayer;
    float moveDestination;
    SpriteRenderer spriteRenderer;
    BoxCollider2D presenceCollider;
    Animator animator;
    Human chasedHuman;
    HealthBar healthBar;
    ScoreDisplay scoreDisplay;
    LevelMaster levelMaster;

    protected virtual void Awake()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        presenceCollider = transform.Find("Presence").GetComponent<BoxCollider2D>();
        healthBar = FindObjectOfType<HealthBar>();
        scoreDisplay = FindObjectOfType<ScoreDisplay>();
    }

    private void Start()
    {
        SetupConstants();
        defaultSortingLayer = spriteRenderer.sortingOrder;
        moveDestination = transform.position.x;
        defaultYValue = transform.position.y;

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

    void SetupConstants()
    {
        moveSpeed = levelMaster.playerMoveSpeed;
        devourYOffset = levelMaster.devourYOffset;
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

    public bool IsHiding() => isHiding;
    public bool IsDevouring() => isDevouring;


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
        Debug.Log("chasing " + target);
        
        MoveToPoint(target.transform.position.x);
    }

    public void StopChasing()
    {
        isChasing = false;
        if (chasedHuman != null) { chasedHuman.SetIsBeingChased(false); }
        chasedHuman = null;
    }

    void Devour()
    {
        isInterruptable = false; // this action is uninterruptable

        isDevouring = true;
        UpdateAnimatorParam();
        chasedHuman.BeingDevoured();
        MoveToDevouredHumanYValue(chasedHuman);
        AudioSource.PlayClipAtPoint(devourSFX, Camera.main.transform.position, 0.01f);

        spriteRenderer.sortingOrder = 10; // so it will appear on top of human
    }

    public void DevourFinished() // called by Ghost Devour animation event
    {
        BackToDefaultYValue();
        isDevouring = false;
        UpdateAnimatorParam();

        healthBar.AddHealth(chasedHuman.GetGrantedHealth());
        chasedHuman.Die();
        chasedHuman = null; // delete chache

        isChasing = false;
        spriteRenderer.sortingOrder = defaultSortingLayer; // back to default 

        isInterruptable = true;
        scoreDisplay.IncrementScore();
    }

    void MoveToDevouredHumanYValue(Human chasedHuman)
    {
        transform.position = new Vector2(transform.position.x, chasedHuman.transform.position.y - devourYOffset);
    }
    
    void BackToDefaultYValue()
    {
        transform.position = new Vector2(transform.position.x, defaultYValue);
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
        prayerAOECount++;
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
        prayerAOECount--;
        if(prayerAOECount <= 0)
        {
            isInPrayerAoE = false;
            healthBar.StopDrainHealthFromPrayer();
        } else
        {
            healthBar.ModifyPrayerDrainSpeed(-drainSpeed);
        }
    }
}
