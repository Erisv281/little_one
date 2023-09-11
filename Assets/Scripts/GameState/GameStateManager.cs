using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is referenced here: https://www.youtube.com/watch?v=KPaEnLpu57s&t=40s
//It is used for pausing and repausing the game properly combines with the UI
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameStateManager();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (GameStateManager.instance != null)
        {
            return;
        }
        else
        {
            instance = this;    // Was new..
        }

    }

    public GameState currentGameState;

    public delegate void GameStateChangeHandler(GameState gameState);
    public static event GameStateChangeHandler onGameStateChanged;



    public void SetState(GameState gameState)
    {
        if (gameState == currentGameState)
        {
            return;
        }
        currentGameState = gameState;
        if (onGameStateChanged != null)
        {
            onGameStateChanged(currentGameState);
        }
    }



    //Add this to classes that are affected by pausing. I.e Enemies, Collidable etc. 
    //Note: Some of this code simply doesn't work and need some modifications. 
    /**
    private void onGameStateChanged(GameState gameState){
        enabled = gameState == GameState.Gameplay;
    }

    private void Awake(){
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    void OnDestroy(){
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }
*/

    //Get the current gamestate: Add it to pausecontrollers, deathcontrollers etc. 
    /**
        GameState currGameState = GameStateManager.Instance.currentGameState;
        GameState newGameState = currGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;
        GameStateManager.Instance.SetState(newGameState);
        */

}
