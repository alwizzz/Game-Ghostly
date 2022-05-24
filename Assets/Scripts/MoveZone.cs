using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZone : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnMouseDown()
    {
        var xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        //Debug.Log("clicked on move zone");
        player.MoveHere(xPos);

    }

}
