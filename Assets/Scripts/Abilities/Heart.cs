using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Ability
{

    /// <summary>
    /// If player collidsen with hearts, increase health and update HUD. 
    /// </summary>
    protected override void UnlockAbility()
    {
        if (GameManager.instance.player.health < GameManager.instance.player.maxHealth)
        {
            GameManager.instance.player.health += 1;
            GameManager.instance.hud.UpdateHartsHUD();
        }
    }

}
