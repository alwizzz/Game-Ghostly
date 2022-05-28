using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanDespawner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D human)
    {
        human.gameObject.GetComponent<Human>().GoingToDespawn();
        Destroy(human.gameObject);
    }
}
