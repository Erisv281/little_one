using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public bool interacted;
    public GameObject enemy;        // The enemy prefab to enable the SR. 

    private void Start()
    {
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    private void PlayerDeath()
    {
        if (interacted)
        {
            enemy.GetComponent<Enemy>().SwitchSR();
        }
        interacted = false;
    }

    private void OnDestroy()
    {
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player") && !interacted)
        {
            interacted = true;
            enemy.GetComponent<Enemy>().SwitchSR();
        }

    }
}
