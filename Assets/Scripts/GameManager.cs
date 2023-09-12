using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public PlayerMovement player;
    public string transitionedFromScene;
    public Vector2 respawnPoint;            // Need to be set to something initial, if we havent received checkpoint yet. 

    // Canvases
    public HUD hud;

    public FadeUI pauseMenu;
    public float fadeTime;


    void Awake()
    {
        GameStateManager.instance = new GameStateManager(); // Can give problems in respawn
        GameStateManager.onGameStateChanged += onGameStateChanged;

        if (GameManager.instance != null)
        {
            Destroy(player.gameObject);
            Destroy(pauseMenu.gameObject);
            Destroy(hud.gameObject);
            Destroy(gameObject);
            return;
        }
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }
    public void respawnPlayer()
    {
        instance.player.transform.position = respawnPoint;
        StartCoroutine(AnimationManager.instance.deactivateDeathScreen());
        instance.player.Respawned();
    }

    //GameState
    private void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    private void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void switchGameState()
    {
        //Freeze gp
        GameState currGameState = GameStateManager.instance.currentGameState;
        GameState newGameState = currGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;
        GameStateManager.instance.SetState(newGameState);
        GameStateManager.instance.currentGameState = newGameState;  //Were not included in unfreeze?

    }

    // When a Scene has loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        instance.pauseMenu.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name.Equals("Start_Scene"))
        {
            instance.hud.gameObject.SetActive(false);
        }
        else
        {
            instance.hud.gameObject.SetActive(true);
            instance.hud.UpdateHartsHUD();
            instance.player.transform.position = respawnPoint;
        }

    }
}
