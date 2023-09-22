using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_DoubleJump : Ability_Once
{
    protected override void UnlockAbility()
    {
        base.UnlockAbility();
        GameManager.instance.player.unlocks.hasUnlockedDoubleJump = true;
    }
}
