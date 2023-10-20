using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These abilities increase player max health. 
/// </summary>
public class BigHeart : Ability
{
    // Particle system effects
    [SerializeField] private float particleDuration;
    [SerializeField] private GameObject HeartParticles;
    [SerializeField] private float animationDuration = 0.5f;
    private int healAmount = 2;


    /// <summary>
    /// If player collides with big hearts, increase max health and update HUD. 
    /// </summary>
    protected override void UnlockAbility()
    {
        base.UnlockAbility();

        // Increase found rewards
        GameManager.instance.rewardsAmount += 1;

        // Increase health
        GameManager.instance.player.maxHealth += healAmount;
        GameManager.instance.player.Heal(healAmount);

        // Create particles
        GameObject particles = Instantiate(HeartParticles, transform.position, Quaternion.identity);
        Destroy(particles, particleDuration);

        // Scale hearts UI
        GameManager.instance.hud.ScaleHearts(animationDuration);

        // Destroy object
        Destroy(gameObject);
    }

}
