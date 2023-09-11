using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage;

    private void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
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
        GameManager.instance.player.takeDamage(this.damage);
    }

    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }
}
