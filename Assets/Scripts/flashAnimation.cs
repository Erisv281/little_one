using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAnimation : MonoBehaviour
{
    [SerializeField] private float flashSpeed;
    private float playerInvinsibleCooldown;


    // Start is called before the first frame update
    void Start()
    {
        playerInvinsibleCooldown = GameManager.instance.player.invinsibleCooldown;
    }

    void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        // Pingpong between transparent and red color of the player. 
        GameManager.instance.player.SR.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * flashSpeed, playerInvinsibleCooldown));
    }

    /// <summary>
    ///  Restores the flash back to normal color
    /// </summary>
    public void destroyFlash()
    {
        GameManager.instance.player.SR.material.color = Color.white;
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
