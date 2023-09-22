using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Walljump : Ability_Once
{

    protected override void UnlockAbility()
    {
        base.UnlockAbility();
        GameManager.instance.player.unlocks.hasUnlockedWallJump = true;
    }
}
