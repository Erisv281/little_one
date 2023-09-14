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
        if (Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name != "Start_Scene")
        {   // Pressed Escape
            ChangePause();
        }

    }


    public void ChangePause()
    {
        Input.ResetInputAxes();     // Reset the input buffer
        GameManager.instance.switchGameState(); // Note: Changes timescale.
        isPaued = !isPaued;
        pauseMenu.SetActive(isPaued);

        if (isPaued)
        {
            GameManager.instance.pauseMenu.FadeUIIn(GameManager.instance.fadeTime);
            GameManager.instance.player.RB.velocity = Vector2.zero; // Reset player movement
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
            ChangePause();
            GameManager.instance.pauseMenu.FadeUIOut(GameManager.instance.fadeTime);
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
