using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool hasCollected;   // The ability has been picked up. 
    [SerializeField] protected SpriteRenderer SR;

    protected virtual void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
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
        if (!GameManager.instance.collectedAbilities.Contains(gameObject.name))
        {
            GameManager.instance.collectedAbilities.Add(gameObject.name);
        }
    }

    public void HasUnlocked()
    {
        Destroy(gameObject);
    }


}
