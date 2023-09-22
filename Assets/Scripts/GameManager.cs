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
    public List<string> interactedSpawnPoints;
    public List<string> collectedAbilities;
    public int playerTempMaxHealth; // Max health of the player when restarting
    public int playerTempHealth;    // Health of player when quitting

    // Canvases
    public HUD hud;
    public FadeUI pauseMenu;
    public float fadeTime;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            // Destroy if another GM exists
            Destroy(gameObject);
            return;
        }
        // Do this once
        instance = this;
        DontDestroyOnLoad(gameObject);

        GameStateManager.instance = new GameStateManager(); // Can give problems in respawn
        GameStateManager.onGameStateChanged += onGameStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        collectedAbilities = new List<string>();
        interactedSpawnPoints = new List<string>();


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
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void switchGameState()
    {
        //Freeze gp
        GameState currGameState = GameStateManager.instance.currentGameState;
        GameState newGameState = currGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;
        GameStateManager.instance.SetState(newGameState);
        GameStateManager.instance.currentGameState = newGameState;  //Were not included in unfreeze?

    }

    public void OnSceneUnloaded(Scene s)
    {
        // When one scene unloads

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

            SetCollectedData();
            instance.hud.UpdateHartsHUD();

            instance.player.transform.position = respawnPoint;
        }

    }

    public void SetCollectedData()
    {
        //For each respawnpoints
        GameObject spawnPointHolder = GameObject.FindWithTag("SpawnPointHolder");   // Set this in inspector!
        foreach (Transform spawnpointTransform in spawnPointHolder.transform)
        {
            GameObject rp = spawnpointTransform.gameObject;
            if (interactedSpawnPoints.Contains(rp.name))                           // Names need to be different
            {
                rp.GetComponent<RespawnPoint>().SetInteracted(true);
                print("true!");
            }
        }

        // For each Ability
        GameObject abilityHolder = GameObject.FindWithTag("AbilityHolder");   // Set this in inspector!
        foreach (Transform abilityTransform in abilityHolder.transform)
        {
            GameObject ab = abilityTransform.gameObject;
            if (collectedAbilities.Contains(ab.name))                           // Names need to be different
            {
                ab.GetComponent<Ability>().HasUnlocked();
            }
        }

        // Set player health if have interacted with some spawnpoint
        if (playerTempMaxHealth != 0)
        {
            player.maxHealth = playerTempMaxHealth;
            player.health = playerTempHealth;
        }


    }
}
