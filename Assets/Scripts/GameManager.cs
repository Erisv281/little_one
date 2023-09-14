using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public PlayerMovement player;
    public string transitionedFromScene;

    // Saving these variables
    public Vector2 respawnPoint;

    // List [] savepoint VectorPos. If have interacted then sprite is green, otherwise gray. 
    // Vector2 latest_Savepoint_interacted that will set respawnpoint

    // Player health and maxhealth
    // 



    // Enemies defeated? No, they shall return. 

    // Canvases
    public HUD hud;
    public FadeUI pauseMenu;
    public float fadeTime;


    void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(player.gameObject);
            Destroy(pauseMenu.gameObject);
            Destroy(hud.gameObject);
            Destroy(gameObject);
            return;
        }
        instance = this;
        GameStateManager.instance = new GameStateManager(); // Can give problems in respawn
        GameStateManager.onGameStateChanged += onGameStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    // Death screen methods
    public void respawnPlayer()
    {
        instance.player.transform.position = respawnPoint;
        StartCoroutine(AnimationManager.instance.deactivateDeathScreen());
        switchGameState();
        instance.player.Respawned();
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name != "Start Scene")
        {
            Input.ResetInputAxes();     // Reset the input buffer
            StartCoroutine(AnimationManager.instance.deactivateDeathScreen());
            switchGameState();
            SceneManager.LoadScene("Start_Scene");
        }

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
        AnimationManager.instance.sceneFader.fadeOutImage.enabled = false;

        if (SceneManager.GetActiveScene().name.Equals("Start_Scene"))
        {
            instance.hud.gameObject.SetActive(false);
            instance.player.gameObject.SetActive(false);
        }
        else
        {
            instance.player.gameObject.SetActive(true);
            instance.hud.gameObject.SetActive(true);
            instance.hud.UpdateHartsHUD();
            instance.player.transform.position = respawnPoint;
        }

    }
}
