using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public PlayerMovement player;
    public GameObject playerPrefab;
    public CameraFollow MainCamera;
    public string transitionedFromScene;

    // Saving these variables
    public Vector2 respawnPoint;
    public Vector2 initialRespawnPoint; // Where player initially starts
    public List<string> interactedSpawnPoints;
    public List<string> collectedAbilities;
    public int playerTempMaxHealth; // Max health of the player when restarting
    public int playerTempHealth;    // Health of player when quitting
    public int rewardsAmount;       // Amount of rewards collected. 
    public int rewardsTotal;        // Total amount of rewards in the game. 

    // Also save copies of player unlockables, to fix the player clone deletion bug
    public bool hasUnlockedBow;
    public bool hasUnlockedMelee;
    public bool hasUnlockedWallJump;
    public bool hasUnlockedDoubleJump;
    public bool hasUnlockedDash;


    // Canvases
    public HUD hud;
    public FadeUI pauseMenu;
    public float fadeTime;
    public AbilityInstruction abilityInstruction;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // Destroy if another GM exists
            Destroy(MainCamera.gameObject);
            Destroy(player.gameObject);
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
        respawnPoint = initialRespawnPoint;
    }

    private void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    // Death screen methods
    public void respawnPlayer()
    {
        instance.player.transform.position = respawnPoint;
        StartCoroutine(AnimationManager.instance.DeactivateDeathScreen());
        switchGameState();
        instance.player.Respawned();
    }

    /// <summary>
    /// Call this function when player is respawning. 
    /// </summary>
    public void SetPlayerHealth()
    {
        if (playerTempMaxHealth != 0)
        {
            player.maxHealth = playerTempMaxHealth;     // When respawning, sets health
            if (playerTempHealth != 0)
            {
                player.health = playerTempHealth;
            }
            else
            {
                player.health = 1; // Fixing bug so we don't respawn with 0 health
            }

        }
        else
        {
            player.health = player.maxHealth;
        }

    }


    /// <summary>
    /// Save the temp variables to current player health. 
    /// </summary>
    public void SetTempHealth()
    {
        playerTempMaxHealth = player.maxHealth;
        playerTempHealth = player.health;
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name != "Start Scene")
        {
            Input.ResetInputAxes();     // Reset the input buffer
            StartCoroutine(AnimationManager.instance.DeactivateDeathScreen());
            switchGameState();
            SceneManager.LoadScene("Start_Scene");
        }
    }

    // Complete screen methods
    public void QuitGameComplete()
    {
        if (SceneManager.GetActiveScene().name != "Start Scene")
        {
            Input.ResetInputAxes();     // Reset the input buffer
            StartCoroutine(AnimationManager.instance.DeactivateCompleteScreen());
            switchGameState();
            ResetGame();
            SceneManager.LoadScene("Start_Scene");
        }
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
            instance.abilityInstruction.gameObject.SetActive(false);
            if (instance.player == null) // If have been destroyed in earlier scene mistakenly, instantiate new player. 
            {
                CreatePlayerClone();
            }
            instance.player.gameObject.SetActive(false);

            // Start coroutine fadeout
        }
        else
        {
            instance.player.gameObject.SetActive(true);
            instance.hud.gameObject.SetActive(true);
            instance.abilityInstruction.gameObject.SetActive(true);

            SetCollectedData();
            SetTotalRewards();
            instance.hud.UpdateHartsHUD();

            instance.player.transform.position = respawnPoint;

            // Start coroutine fadeout

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

    /// <summary>
    /// Only do this first time entering other scenes than Start scene. Assuming all 5 abilities are present. 
    /// </summary>
    private void SetTotalRewards()
    {
        if (rewardsTotal != 0)
        {
            return;
        }

        GameObject abilityHolder = GameObject.FindWithTag("AbilityHolder");   // Set this in inspector!
        foreach (Transform abilityTransform in abilityHolder.transform)
        {
            rewardsTotal += 1;
        }
        rewardsTotal -= 5; // Remove awll collected abilities, thus 5 abilities.
    }


    /// <summary>
    /// This method resets all things collected in the game. 
    /// Call this method when the game is finished. 
    /// </summary>
    public void ResetGame()
    {
        // Reset spawnpoints and reset back to the initial one
        respawnPoint = initialRespawnPoint;
        interactedSpawnPoints.Clear();

        // Reset collected abilities
        collectedAbilities.Clear();
        hasUnlockedBow = false;
        hasUnlockedMelee = false;
        hasUnlockedWallJump = false;
        hasUnlockedDoubleJump = false;
        hasUnlockedDash = false;

        // Reset player health
        playerTempMaxHealth = 0;
        playerTempHealth = 0;

        // Reset rewards
        rewardsAmount = 0;       // Amount of rewards collected. 
        rewardsTotal = 0;        // Total amount of rewards in the game. 

        // Reset player data aswell
        player.ResetGame();

    }

    /// <summary>
    /// Solves the Bug where the player is a child object of a parent ex. moving platforms. 
    /// </summary>
    private void CreatePlayerClone()
    {
        GameObject playerObj = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        instance.player = playerObj.GetComponent<PlayerMovement>();

        // Set the unlocks here
        player.unlocks.hasUnlockedBow = hasUnlockedBow;
        player.unlocks.hasUnlockedDash = hasUnlockedDash;
        player.unlocks.hasUnlockedDoubleJump = hasUnlockedDoubleJump;
        player.unlocks.hasUnlockedMelee = hasUnlockedMelee;
        player.unlocks.hasUnlockedWallJump = hasUnlockedWallJump;

        // Update HUD
        hud.UpdatePlayerCallback();
    }


    /// <summary>
    /// Saving the temporary player unlockables by setting these variables to whether player has unlocked 
    /// them or not. 
    /// </summary>
    public void SavePlayerUnlocks()
    {
        hasUnlockedBow = player.unlocks.hasUnlockedBow;
        hasUnlockedDash = player.unlocks.hasUnlockedDash;
        hasUnlockedDoubleJump = player.unlocks.hasUnlockedDoubleJump;
        hasUnlockedMelee = player.unlocks.hasUnlockedMelee;
        hasUnlockedWallJump = player.unlocks.hasUnlockedWallJump;
    }


    // For ability collection
    public bool ContainAbility(string name)
    {
        return collectedAbilities.Contains(name);

    }
    public void AddAbility(string name)
    {
        collectedAbilities.Add(name);
    }

    // For respawnpoint collection
    public bool ContainSpawnpoint(string name)
    {
        return interactedSpawnPoints.Contains(name);

    }
    public void AddSpawnpoint(string name)
    {
        interactedSpawnPoints.Add(name);
    }

    // For showing/hiding Ability instruction
    public void ShowInstruction(string whatToConfigure)
    {
        abilityInstruction.ShowInstructions(whatToConfigure);
    }

    public void HideInstruction()
    {
        abilityInstruction.HideInstructions();
    }

    // For HUD
    public void UpdateHartsHUD()
    {
        hud.UpdateHartsHUD();
    }


    // GameState
    private void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }


    public void switchGameState()
    {
        //Freeze gp
        GameState currGameState = GameStateManager.instance.currentGameState;
        GameState newGameState = currGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;
        GameStateManager.instance.SetState(newGameState);
        GameStateManager.instance.currentGameState = newGameState;  //Were not included in unfreeze?
    }
}
