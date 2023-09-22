using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public bool interacted;
    [SerializeField] private Sprite graySprite;
    [SerializeField] private Sprite greenSprite;
    private SpriteRenderer SR;
    private Animator anim;

    private void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        SR.sprite = graySprite;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player"))  // Save by colliding with this
        {
            GameManager.instance.respawnPoint = transform.position;
            SetInteracted(true);
            print("Respawn set");
        }
    }

    public void SetInteracted(bool interactable)
    {
        interacted = interactable;
        // Add spawnpoint to collctible list
        if (!GameManager.instance.interactedSpawnPoints.Contains(gameObject.name))
        {
            GameManager.instance.interactedSpawnPoints.Add(gameObject.name);
            SR.sprite = greenSprite;
            anim.SetTrigger("Interact");  // Show anim here
            print("Animation plays");
        }
        else
        {
            SR.sprite = graySprite;
        }

        // Save player health
        GameManager.instance.playerTempMaxHealth = GameManager.instance.player.maxHealth;
        GameManager.instance.playerTempHealth = GameManager.instance.player.health;


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
