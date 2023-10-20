using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is for Abilities, things that the player can pickup such as waljump, hearts, ..
/// </summary>
public class Ability : MonoBehaviour
{
    public bool hasCollected;   // If the ability has been picked up. 
    [SerializeField] protected SpriteRenderer SR;

    protected virtual void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("player") && !hasCollected)
        {
            UnlockAbility();
        }
    }

    /// <summary>
    /// Set unlocked for the player
    /// </summary>
    protected virtual void UnlockAbility()
    {
        hasCollected = true;

        // Put in collection of unlocks. 
        if (!GameManager.instance.ContainAbility(gameObject.name))
        {
            GameManager.instance.AddAbility(gameObject.name);
        }
    }

    public void HasUnlocked()
    {
        Destroy(gameObject);
    }


}
