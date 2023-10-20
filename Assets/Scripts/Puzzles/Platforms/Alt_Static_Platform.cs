using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alt_Static_Platform : SwitchTrigger
{
    [SerializeField] private GameObject platform;
    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        platform.GetComponent<SpriteRenderer>().enabled = false;
        platform.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        platform.SetActive(false);
    }

    protected override void Activate()
    {
        base.Activate();
        platform.SetActive(true);
        platform.GetComponent<SpriteRenderer>().enabled = true;

    }
    protected override void Deactivate()
    {
        base.Deactivate();
        platform.SetActive(false);
        platform.GetComponent<SpriteRenderer>().enabled = false;

        // Note: Can get the player stucwk. 

    }
}
