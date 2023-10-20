using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For these abilities, show player collect animation, zoom in and play sound. These
/// are for collecting abilities you can only collect once such as double jump, walljump, ...
/// </summary>
public class Ability_Once : Ability
{
    [SerializeField] protected float collectAnimationDuration = 1f;

    protected override void UnlockAbility()
    {
        base.UnlockAbility();

        // Zoom camera
        GameManager.instance.MainCamera.ZoomIn(collectAnimationDuration);

        // Place abilitity top of player
        transform.parent = GameManager.instance.player.transform;
        transform.position = GameManager.instance.player.transform.position + Vector3.up;

        // Animation and sound
        GameManager.instance.player.anim.SetTrigger("Collect");
        AudioManager.instance.Play("Collect");

        // Start exit coroutine
        StartCoroutine(DestroyAfterAnimation(collectAnimationDuration));
    }

    protected IEnumerator DestroyAfterAnimation(float duration)
    {
        // Pause player, handle animation
        GameManager.instance.player.pstate.isEnteringCutscene = true;
        GameManager.instance.player.StopMovement();
        GameManager.instance.player.ResetAnimation();

        // Wait some exit time
        yield return new WaitForSeconds(duration);

        // Zoom out, let player walk again
        GameManager.instance.MainCamera.ZoomOut(collectAnimationDuration);
        GameManager.instance.player.pstate.isEnteringCutscene = false;

        // Destroy the GameObject holding the ability sprite
        Destroy(gameObject);
    }

}
