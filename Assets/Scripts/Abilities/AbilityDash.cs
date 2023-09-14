using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDash : Ability
{

    protected override void UnlockAbility()
    {
        GameManager.instance.player.unlocks.hasUnlockedDash = true;
    }
}
