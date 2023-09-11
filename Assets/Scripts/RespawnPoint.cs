using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public bool interacted;

    private void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player") && Input.GetButtonDown("Interact"))  // Press f to save
        {
            GameManager.instance.respawnPoint = transform.position;
            interacted = true;
            print("Respawn set");
        }
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
