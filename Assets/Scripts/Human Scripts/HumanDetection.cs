using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanDetection : MonoBehaviour
{
    Human thisHuman;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        thisHuman = transform.parent.GetComponent<Human>();
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerStay2D()
    {
        if (thisHuman.IsInNormalState() && player.IsDevouring())
        {
            thisHuman.DetectedGhostAround();
        }
    }
}
