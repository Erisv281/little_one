using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBow : Ability
{

    protected override void UnlockAbility()
    {
        GameManager.instance.player.unlocks.hasUnlockedBow = true;
    }
}
