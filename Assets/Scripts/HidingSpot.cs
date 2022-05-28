using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    GameObject ghostEye;
    Player player;
    void Start()
    {
        ghostEye = transform.Find("GhostEye").gameObject;
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsHiding())
        {
            ghostEye.SetActive(true);
        } else
        {
            ghostEye.SetActive(false);
        }

        if (Input.GetMouseButton(1)) // right click
        {
            if (!player.IsHiding())
            {
                player.Hide();
            }
        }
    }
}
