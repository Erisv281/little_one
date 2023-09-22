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
        base.UnlockAbility();
        GameManager.instance.player.maxHealth += 2;
        GameManager.instance.player.health += 2;
        GameManager.instance.hud.UpdateHartsHUD();
        Destroy(gameObject);
    }

}
