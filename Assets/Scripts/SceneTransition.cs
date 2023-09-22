using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo; // Save the scene we're going to transition to. 
    [SerializeField] private Transform startPoint; // Where to place the player after he has transitioned. 
    [SerializeField] private Vector2 exitDirection;  // Dir for player is exiting, Not using!
    [SerializeField] private float exitTime;

    private void Start()
    {
        if (transitionTo == GameManager.instance.transitionedFromScene)
        {
            GameManager.instance.player.gameObject.transform.position = startPoint.position;
            StartCoroutine(GameManager.instance.player.WalkIntoNewScene(exitDirection, exitTime));
        }

        StartCoroutine(AnimationManager.instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            print("Scenetransition");
            GameManager.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            GameManager.instance.player.pstate.isEnteringCutscene = true;
            StartCoroutine(AnimationManager.instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));

        }
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
