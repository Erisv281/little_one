using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomtrigger : MonoBehaviour
{

    private bool interacted;
    [SerializeField] private float cameraZoomSize = 10f;
    [SerializeField] private float zoomDuration = 2.0f;

    private void OnDestroy()
    {
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }

    private void Start()
    {
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    private void PlayerDeath()
    {
        if (interacted)
        {
            ZoomInitial();
        }
        interacted = false;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        // When triggered by player: Zoom out camera
        if (!interacted && other.CompareTag("player"))
        {
            ZoomOut();
            print("Zoom out");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interacted && other.CompareTag("player"))
        {
            ZoomInitial();
            print("Zoom in");
        }
    }

    public void ZoomOut()
    {
        interacted = true;
        GameManager.instance.MainCamera.ZoomOutBySize(zoomDuration, cameraZoomSize);
    }

    public void ZoomInitial()
    {
        interacted = false;
        GameManager.instance.MainCamera.ZoomOut(zoomDuration);
    }
}
