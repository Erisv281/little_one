using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that switches between foreground and background 
/// </summary>
public class Static_Platform : SwitchTrigger
{
    [SerializeField] private GameObject platform;
    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        platform.SetActive(true);
    }

    protected override void Activate()
    {
        base.Activate();
        platform.SetActive(false);

    }
    protected override void Deactivate()
    {
        base.Deactivate();
        platform.SetActive(true);

        // Note: Can get the player stucwk. 

    }
}
