using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Collect this to complete the game
/// </summary>
public class Shrine : MonoBehaviour
{
    public bool interacted;
    [SerializeField] private float timeBeforeCompleteScreen;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player") && !interacted)
        {
            interacted = true;
            StartCoroutine(GameComplete());

        }

    }

    public IEnumerator GameComplete()
    {
        // Pause player, reset animation
        GameManager.instance.player.StopMovement();
        GameManager.instance.player.ResetAnimation();
        GameManager.instance.player.pstate.isEnteringCutscene = true;
        GameManager.instance.switchGameState();

        // Place the thing on the player
        transform.parent = GameManager.instance.player.transform;
        transform.position = GameManager.instance.player.transform.position + Vector3.up;

        // Camera zoom in
        GameManager.instance.MainCamera.ZoomIn(timeBeforeCompleteScreen);

        // Animation
        GameManager.instance.player.anim.SetTrigger("Collect");

        // Wait time
        yield return new WaitForSeconds(timeBeforeCompleteScreen);

        // Zoom out and show complete screen. 
        GameManager.instance.MainCamera.ZoomOut(timeBeforeCompleteScreen);
        StartCoroutine(AnimationManager.instance.ActivateCompleteScreen());

        // Exit
        GameManager.instance.player.pstate.isEnteringCutscene = false;
        Destroy(gameObject);
    }
}
