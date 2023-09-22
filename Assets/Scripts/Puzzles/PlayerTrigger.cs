using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Player Collides and triggers som triggerres
/// </summary>
public class PlayerTrigger : MonoBehaviour
{

    public bool interacted;
    [SerializeField] protected List<SwitchTrigger> triggers;    // List of Triggers
    private void Start()
    {
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player") && !interacted)
        {
            interacted = true;
            foreach (SwitchTrigger trigger in triggers)
            {
                trigger.CallActivate(); // Activate the triggerers
            }
        }

    }

    public void PlayerDeath()
    {
        // When player dies, reset that we can interact with this objectr again.'
        interacted = false;
    }

    public void OnDestroy()
    {
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }
}
