using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When colliding with player, move the camera to given position. 
/// </summary>
public class CameraTrigger : MonoBehaviour
{
    private bool interacted;
    [SerializeField] private bool shallNotInteract; // Set this to true if you don't want to interact with this item. 
    [SerializeField] private float cutsceneDuration = 2.0f;
    [SerializeField] private Vector3 target; // Which position the camera shall focus on
                                             // Note: The targetTransform:s position need to be set outside given gameobject container. 


    private void OnTriggerEnter2D(Collider2D other)
    {
        // When triggered by player: Switch camera and set player mode to cutscene. 
        if (!interacted && other.CompareTag("player") && !shallNotInteract)
        {
            TriggerCamera();
        }

    }

    public void TriggerCamera()
    {
        interacted = true;
        GameManager.instance.MainCamera.StartCameraCutscene(target, cutsceneDuration);

    }
}
