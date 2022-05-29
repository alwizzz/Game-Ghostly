using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZone : MonoBehaviour
{
    //private void OnTriggerExit2D(Collider2D humanCollision)
    //{
    //    human.GoingToDespawn();
    //}

    private void OnTriggerEnter2D(Collider2D humanCollision)
    {
        var human = humanCollision.gameObject.GetComponent<Human>();
        human.EnteredPlayZone();
        //Debug.Log(human.gameObject.name);
    }
}
