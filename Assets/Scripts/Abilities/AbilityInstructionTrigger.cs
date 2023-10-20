using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Call the instructionUI when trigger with this object. 
/// </summary>
public class AbilityInstructionTrigger : MonoBehaviour
{
    private bool interacted;
    [SerializeField] private string whatToConfigure;    // Eg. JUMP, DBL, MOVE, MELEE, BOW, DASH, WALL


    /// <summary>
    /// Show instructions when entering area
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player") && !interacted)
        {
            interacted = true;
            GameManager.instance.ShowInstruction(whatToConfigure);
        }

    }

    /// <summary>
    /// Hide the instructions when leaving the trigger area
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player") && interacted)
        {
            interacted = false;
            GameManager.instance.HideInstruction();
        }
    }


}
