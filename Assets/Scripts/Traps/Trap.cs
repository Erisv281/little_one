using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float hitForce;

    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

        // When traps collide with player, then attack them
        if (IsCollidingWithPlayer(other))
        {
            Attack();
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {

        // When traps stays to collide with player, then attack them
        if (IsCollidingWithPlayer(other))
        {
            Attack();
        }
        // Also attack enemimies
        else if (other.CompareTag("enemy"))
        {
            other.GetComponent<Enemy>().EnemyHit(this.damage, (other.transform.position - transform.position).normalized, hitForce);
        }
    }

    /// <summary>
    /// Return T if colliding with player and the player is not invinvible or is alive. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsCollidingWithPlayer(Collider2D other)
    {
        return other.CompareTag("player") && !GameManager.instance.player.pstate.isInvinsible
         && GameManager.instance.player.pstate.isAlive;
    }

    /// <summary>
    /// Deal damage to the player and push in opposite direction
    /// </summary>
    protected virtual void Attack()
    {
        PlayerMovement p = GameManager.instance.player;
        p.TakeDamage(this.damage, (p.transform.position - transform.position).normalized, hitForce);
    }

    protected virtual void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected virtual void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }
}
