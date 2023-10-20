using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool isPaued = false;
    public GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        // Pressed 'Esc' in not the start_scene and player is not in some cutscene
        if (Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name != "Start_Scene" && !GameManager.instance.player.pstate.isEnteringCutscene)
        {
            ChangePause();
        }

    }


    /// <summary>
    /// Toggles the pause canvas
    /// </summary>
    public void ChangePause()
    {
        Input.ResetInputAxes();

        // Toggle pausing mode
        GameManager.instance.switchGameState();
        isPaued = !isPaued;
        pauseMenu.SetActive(isPaued);

        if (isPaued)
        {
            GameManager.instance.pauseMenu.FadeUIIn(GameManager.instance.fadeTime);
            GameManager.instance.player.StopMovement();
        }
    }

    public void showControls()
    {
        // Show an image with the controls. 
    }

    public void QuitLevel()
    {
        if (SceneManager.GetActiveScene().name != "Start Scene")
        {
            Input.ResetInputAxes();     // Reset the input buffer
            GameManager.instance.pauseMenu.FadeUIOut(GameManager.instance.fadeTime);    // Problem: Cannot do this
            ChangePause();
            SceneManager.LoadScene("Start_Scene");
        }
    }



    // GameState
    private void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;          // Pause handling
    }

    void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }



}
