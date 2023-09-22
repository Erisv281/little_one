using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMelee : Ability_Once
{
    protected override void UnlockAbility()
    {
        base.UnlockAbility();
        GameManager.instance.player.unlocks.hasUnlockedMelee = true;
    }

}
