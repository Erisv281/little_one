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
        SR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SetSprite();
        if (SR.sprite == greenSprite)
        {
            anim.SetTrigger("Green");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player"))  // Save by colliding with this
        {
            GameManager.instance.respawnPoint = transform.position;
            SetInteracted(true);
        }
    }

    public void SetInteracted(bool interactable)
    {
        interacted = interactable;
        // Add spawnpoint to collctible list
        if (!GameManager.instance.ContainSpawnpoint(gameObject.name)) ;
        {
            GameManager.instance.AddSpawnpoint(gameObject.name);

            // Animation
            anim.SetTrigger("Interact");
        }

        // Change sprite to grey
        SetSprite();

        // Save player health
        GameManager.instance.SetTempHealth();

        // Save player unlocks
        GameManager.instance.SavePlayerUnlocks();
    }

    public void SetSprite()
    {
        // If interacted, set green. Otherwise grey
        SR.sprite = interacted ? greenSprite : graySprite;

    }


    // GP
    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }
}
