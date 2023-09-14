using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHeart : Ability
{
    /// <summary>
    /// If player collidsen with big hearts, increase max health and update HUD. 
    /// </summary>
    protected override void UnlockAbility()
    {
        GameManager.instance.player.maxHealth++;
        GameManager.instance.player.health += 1;
        GameManager.instance.hud.UpdateHartsHUD();

    }
}
