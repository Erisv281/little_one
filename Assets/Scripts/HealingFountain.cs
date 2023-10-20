using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For healing the player when staying within its collider. 
/// </summary>
public class HealingFountain : MonoBehaviour
{
    [SerializeField] private int healingAmount = 1;
    [SerializeField] private float healCooldown = 1.0f; // Cooldown between each heal
    private bool isHealing;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    private void OnDestroy()
    {
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }

    private void PlayerDeath()
    {
        // Reset cooldown and anim
        isHealing = false;
        anim.SetBool("Interact", false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            // Set animation
            anim.SetBool("Interact", true);

            // If not already healing player, start coroutine. 
            if (!isHealing)
            {
                StartCoroutine(HealPlayer());
                // Do HUD scale hearts here. 
            }

        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("player"))
        {
            anim.SetBool("Interact", false);
        }
    }


    /// <summary>
    /// Heal player between some cooldown. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator HealPlayer()
    {
        isHealing = true;
        GameManager.instance.player.Heal(healingAmount);
        yield return new WaitForSeconds(healCooldown);
        isHealing = false;
    }

}
