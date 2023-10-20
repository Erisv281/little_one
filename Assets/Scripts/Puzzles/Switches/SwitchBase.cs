using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A switch base is the acstract class for items such as switches, levers, ...
/// </summary>
public class SwitchBase : MonoBehaviour
{
    [SerializeField] protected Sprite activatedSwitch;   // The switched switch sprite  
    [SerializeField] protected Sprite regularSwitch;   // The OG sprite  
    protected bool isActivated;
    [SerializeField] protected SpriteRenderer SR;
    [SerializeField] protected List<SwitchTrigger> triggers;    // Has multiple triggers. Note: Assuming correclty set in inspector!


    // Delegates
    public delegate void OnSwitchHit();
    [HideInInspector] public OnSwitchHit onSwitchHitCallback;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.sprite = regularSwitch;
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    protected virtual void PlayerDeath()
    {
        // Reset sprite of switches
        SR.sprite = regularSwitch;
        isActivated = false;
    }

    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    protected virtual void ActivateSwitch()
    {
        foreach (SwitchTrigger trigger in triggers)
        {
            trigger.CallActivate(); // Activate the triggerers
        }

        // Set sprite and audio
        SR.sprite = activatedSwitch;
        isActivated = true;
        AudioManager.instance.Play("Switch");

        // Notify listeners
        SwitchActivationCallback();
    }

    public void CallActivateSwitch()
    {
        ActivateSwitch();
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

    public void SwitchActivationCallback()
    {
        // Notify subscribers that the player has died. 
        if (onSwitchHitCallback != null)
        {
            onSwitchHitCallback.Invoke();
        }
    }
}
