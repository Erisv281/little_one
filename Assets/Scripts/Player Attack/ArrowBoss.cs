using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBoss : Arrow
{

    /// <summary>
    /// Only consider player, nothing else
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter2D(Collider2D other)
    {

        if (hasHitSomething)
        {
            return;
        }

        // Don't bother with enemies
        if (other.CompareTag("enemy"))
        {
            return;
        }

        // The arrow is hitting the player
        if (other.CompareTag("player"))
        {
            PlayerMovement p = GameManager.instance.player;
            p.TakeDamage(arrowDamage, (p.transform.position - transform.position).normalized, hitForce);
            hasHitSomething = true;
        }

        // Note: Make sure enemy and this arrow can collide. 

        Destroy(gameObject);

    }

}
