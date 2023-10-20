using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy the thin ice after interacted with it delayed some cooldown. 
/// </summary>
public class Thin_Ice : MonoBehaviour
{
    [SerializeField] private float meltDuration = 2f;
    private bool interacted;

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
        // Player has died and is respawning. 
        this.gameObject.SetActive(true);
        interacted = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        // Start to break the ice animation.
        if (other.CompareTag("player") && !interacted)
        {
            StartCoroutine(Melt());
        }
    }

    private IEnumerator Melt()
    {
        interacted = true;

        // Wait some time before dstroyed
        yield return new WaitForSeconds(meltDuration);

        this.gameObject.SetActive(false);   // Note script and ice two different. 

    }
}
