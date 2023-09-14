using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool hasCollected;   // The ability has been picked up. 

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // If player has collect this ability, Destroy it. 
        if (hasCollected)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("player") && !hasCollected)
        {
            hasCollected = true;
            UnlockAbility();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set unlocked for the player
    /// </summary>
    protected virtual void UnlockAbility()
    {
        // Placeholder
    }


}
