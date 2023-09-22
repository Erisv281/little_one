using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a object hindering the player from moving forward, derived objects can be like static platforms, 
/// moving platforms, rotate pltforms etc. 
/// </summary>
public class SwitchTrigger : MonoBehaviour
{

    [SerializeField] protected SpriteRenderer SR; // SR of child platform
    [SerializeField] protected Sprite activatedSprite;
    protected Sprite regularSprite;
    protected bool isActivated;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        regularSprite = SR.sprite;
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    protected virtual void PlayerDeath()
    {
        // Reset position of platforms
        SR.sprite = regularSprite;
        isActivated = false;
    }

    protected virtual void Update()
    {

    }

    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    protected virtual void Activate()
    {
        // Do something here like move platform to position the object
        isActivated = true;
    }
    protected virtual void Deactivate()
    {
        // Do something here like move platform back
        isActivated = false;
    }

    public void CallActivate()
    {
        Activate();
    }
    public void CallDeactivate()
    {
        Deactivate();
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }


}
