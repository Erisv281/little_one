using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{


    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;  // Dir for player is exiting
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
        {     // Check if player has this tag
            GameManager.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            GameManager.instance.player.pstate.isEnteringCutscene = true;
            //SceneManager.LoadScene(transitionTo);
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
