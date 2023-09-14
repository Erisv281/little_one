using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMelee : Ability
{
    protected override void UnlockAbility()
    {
        GameManager.instance.player.unlocks.hasUnlockedMelee = true;
    }
}
