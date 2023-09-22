using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For these abilities, show player collect animation
/// </summary>
public class Ability_Once : Ability
{

    [SerializeField] private float collectAnimationDuration = 1f;

    protected override void UnlockAbility()
    {
        base.UnlockAbility();
        transform.position = GameManager.instance.player.transform.position += Vector3.up;
        GameManager.instance.player.anim.SetTrigger("Collect");
        StartCoroutine(DestroyAfterAnimation(collectAnimationDuration));
    }

    protected IEnumerator DestroyAfterAnimation(float duration)
    {
        GameManager.instance.player.pstate.isEnteringCutscene = true;
        GameManager.instance.player.RB.velocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        GameManager.instance.player.pstate.isEnteringCutscene = false;

        // Destroy the GameObject holding the ability sprite
        Destroy(gameObject);
    }

}
