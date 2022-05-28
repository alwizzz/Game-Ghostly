using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D humanCollision)
    {
        var human = humanCollision.gameObject.GetComponent<Human>();
        if (human.IsBeingChased())
        {
            humanCollision.gameObject.GetComponent<Human>().GoingToDespawn();
            //Debug.Log("OutOfPlayZone called");
        }
    }
}
