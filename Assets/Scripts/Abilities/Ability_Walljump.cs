using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Walljump : Ability
{

    protected override void UnlockAbility()
    {
        GameManager.instance.player.unlocks.hasUnlockedWallJump = true;
    }
}
