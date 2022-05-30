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
    [SerializeField] Color defaultColor;
    [SerializeField] Color damagedColor;
    [SerializeField] Color insidePrayerColor;
    Color currentColor;


    [Header("States")]
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isInterruptable = true;
    [SerializeField] bool isHiding = true;
    [SerializeField] bool isGoingToHide = false;
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
    float grantedHealthCache;
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

        currentColor = defaultColor;
        UpdateColor();
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
            else if (isGoingToHide)
            {
                Hiding();
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
        if (isGoingToHide) { isGoingToHide = false; }

        MoveToPoint(worldXPos);
    }


    // Hide Mechanic
    public void Hide() //called by HidingSpot
    {
        if (!isInterruptable) { return; }
        if (isChasing) { StopChasing(); }

        MoveToPoint(0);
        isGoingToHide = true;
    }

    void Hiding()
    {
        isGoingToHide = false;
        isHiding = true;
        UpdatePresenceCollider();
    }


    // Chase and Devour Mechanic
    public void Chase(Human target) //called by Human
    {
        if (!isInterruptable) { return; }
        if (isGoingToHide) { isGoingToHide = false; }

        isChasing = true;
        chasedHuman = target;
        // Debug.Log("chasing " + chasedHuman);
        grantedHealthCache = chasedHuman.GetGrantedHealth();

        MoveToPoint(target.transform.position.x);
    }

    public void StopChasing()
    {
        if (!isInterruptable) { return; }

        isChasing = false;
        if (chasedHuman != null) { chasedHuman.SetIsBeingChased(false); }
        chasedHuman = null;
    }

    void Devour()
    {
        // Debug.Log("devouring " + chasedHuman);
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
        // Debug.Log("finished devouring " + chasedHuman);

        BackToDefaultYValue();
        isDevouring = false;
        UpdateAnimatorParam();

        healthBar.AddHealth(grantedHealthCache);
        if(grantedHealthCache < 0) { EatenInnocent(); }

        if (chasedHuman != null) { chasedHuman.Die(); } // to avoid bug: Human reference missing midway, bug still not fixed
        else { Debug.Log("THAT bug occured"); }

        
        chasedHuman = null; // delete chache

        //isChasing = false;
        //spriteRenderer.sortingOrder = defaultSortingLayer; // back to default 

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
        if(chasedHuman == null) { return; }
        moveDestination = chasedHuman.transform.position.x;
    }

    public void EnteredPrayerAoE(float drainSpeed)
    {
        prayerAOECount++;
        if (!isInPrayerAoE) //currently not in aoe
        {
            isInPrayerAoE = true;
            currentColor = insidePrayerColor;
            UpdateColor();
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
            currentColor = defaultColor;
            UpdateColor();
            healthBar.StopDrainHealthFromPrayer();
        } else
        {
            healthBar.ModifyPrayerDrainSpeed(-drainSpeed);
        }
    }

    void EatenInnocent()
    {
        StartCoroutine(DamagedFlare());
    }

    IEnumerator DamagedFlare()
    {
        spriteRenderer.color = damagedColor;
        for (float t = 0.01f; t < 1f; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, currentColor, Mathf.Min(1, t / 0.5f));
            yield return null;
        }
    }

    void UpdateColor()
    {
        spriteRenderer.color = currentColor;
    }
}
