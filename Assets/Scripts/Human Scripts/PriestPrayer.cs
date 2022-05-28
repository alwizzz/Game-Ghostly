using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestPrayer : MonoBehaviour
{

    Priest priest;
    Player player;
    SpriteRenderer prayerRenderer;
    CircleCollider2D prayerCollider;

    private void Awake()
    {
        priest = transform.parent.GetComponent<Priest>();
        player = FindObjectOfType<Player>();
        prayerRenderer = GetComponent<SpriteRenderer>();
        prayerCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D()
    {
        //Debug.Log("ghost enter aoe of " + priest.gameObject.name);
        player.EnteredPrayerAoE(priest.GetPrayerDrainSpeed());
    }

    private void OnTriggerExit2D()
    {
        //Debug.Log("ghost exit aoe of " + priest.gameObject.name);
        player.ExitedPrayerAoE(priest.GetPrayerDrainSpeed());
    }

    public void ActivatePrayerAoE()
    {
        prayerRenderer.enabled = true;
        prayerCollider.enabled = true;
    }
    public void DeactivatePrayerAoE()
    {
        prayerRenderer.enabled = false;
        prayerCollider.enabled = false;
    }


}
