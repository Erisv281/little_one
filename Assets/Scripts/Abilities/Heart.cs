using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collect this to increase slightly player health
/// </summary>
public class Heart : Ability
{

    private int healAmount = 1;

    /// <summary>
    /// If player collides with hearts, increase health and update HUD. 
    /// </summary>
    protected override void UnlockAbility()
    {
        base.UnlockAbility();

        // Increase health
        GameManager.instance.player.Heal(healAmount);

        // Destroy object
        Destroy(gameObject);
    }

}
