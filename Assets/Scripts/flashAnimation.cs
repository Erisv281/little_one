using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Make entities flashing between original color and red. 
/// </summary>
public class FlashAnimation : MonoBehaviour
{
    [SerializeField] private float flashSpeed;
    [SerializeField] private float flashCooldown;   // Eg. invisible or recoilduration
    [SerializeField] private SpriteRenderer SR; // SR of the object that shall flash


    void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        // Pingpong between transparent and red color of the player. 
        SR.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * flashSpeed, flashCooldown));
    }

    /// <summary>
    ///  Restores the flash back to normal color
    /// </summary>
    public void destroyFlash()
    {
        SR.material.color = Color.white;
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
