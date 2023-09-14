using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_DoubleJump : Ability
{
    protected override void UnlockAbility()
    {
        GameManager.instance.player.unlocks.hasUnlockedDoubleJump = true;
    }
}
