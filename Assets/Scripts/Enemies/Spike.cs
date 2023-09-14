using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float hitForce;

    private void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("player"))
        {
            Attack();
        }
    }

    public void Attack()
    {
        PlayerMovement p = GameManager.instance.player;
        p.TakeDamage(this.damage, (p.transform.position - transform.position).normalized, hitForce);
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }
}
