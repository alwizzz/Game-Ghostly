using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] Color ghostOutsideColor;
    [SerializeField] Color ghostInsideColor;

    SpriteRenderer spriteRenderer;
    Player player;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //ghostEye = transform.Find("GhostEye").gameObject;\
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsHiding())
        {
            spriteRenderer.color = ghostInsideColor;
        } else
        {
            spriteRenderer.color = ghostOutsideColor;
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
