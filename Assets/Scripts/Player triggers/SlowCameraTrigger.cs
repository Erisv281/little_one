using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Slow cameras are cameras with various transform targets corresponding with speeds. 
/// </summary>
public class SlowCameraTrigger : MonoBehaviour
{
    private bool interacted;
    [SerializeField] private List<Vector3> targets; // Note: The first item in this list corresponds to the last position that shall be visited
    [SerializeField] private List<float> speeds;    // Note: Correspondant with the target at the same position

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When triggered by player: Switch camera and set player mode to cutscene. 
        if (!interacted && other.CompareTag("player"))
        {
            TriggerCamera();
        }

    }

    public void TriggerCamera()
    {
        interacted = true;
        GameManager.instance.MainCamera.StartSlowCameraCutscene(targets, speeds);

    }
}
