using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HidingSpot : MonoBehaviour
{
    [SerializeField] Color ghostOutsideColor;
    [SerializeField] Color ghostInsideColor;

    Image thisImage;
    Player player;
    void Start()
    {
        thisImage = GetComponent<Image>();
        //ghostEye = transform.Find("GhostEye").gameObject;\
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsHiding())
        {
            thisImage.color = ghostInsideColor;
        } else
        {
            thisImage.color = ghostOutsideColor;
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
