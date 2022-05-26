using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanVision : MonoBehaviour
{

    // CONFIG
    public float alertnessMax = 100f;
    [Range(0, 100)] // slider for better visualization in inspector
    public float alertnessLevel = 0; //if maximum then this human is going to isPanic state 
    public float alertRiseUpSpeed = 10f;
    public float alertRiseDownSpeed = 10f;
    Quaternion defaultRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    Quaternion flippedRotation = Quaternion.Euler(new Vector3(0, 180, 0));

    // STATES
    public bool ghostInVision;

    // CACHE
    public Human thisHuman;

    // Start is called before the first frame update
    void Start()
    {
        thisHuman = transform.parent.GetComponent<Human>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!thisHuman.isPanic && !thisHuman.isRunning) // as long the human is not in isPanic state, below codes will work
        {
            if (ghostInVision)
            {
                RiseUpAlertness();
            } else
            {
                RiseDownAlertness();
            }
        }
            
    }

    private void OnTriggerEnter2D()
    {
        ghostInVision = true;
    }

    private void OnTriggerExit2D()
    {
        ghostInVision = false;
    }

    void RiseUpAlertness()
    {
        alertnessLevel += Time.deltaTime * alertRiseUpSpeed;
        if(alertnessLevel >= alertnessMax)
        {
            alertnessLevel = 100f;
            thisHuman.Alerted();
            //Debug.Log("panik lur");
        }
    }

    void RiseDownAlertness()
    {
        if(alertnessLevel > 0)
        {
            alertnessLevel -= Time.deltaTime * alertRiseDownSpeed;
        }
    }

    public void FlipVisionCollider(bool isFlipped) // called by Human
    {
        transform.rotation = (isFlipped) ? flippedRotation : defaultRotation;
    }
}
