using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool interacted;
    public GameObject enemyPrefab;        // The enemy prefab to spawn.
    public Vector3[] spawnPoints;       // Array of spawn points for the enemies.
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("player") && !interacted)
        {
            interacted = true;
            foreach (Vector3 vec in spawnPoints)
            {
                Instantiate(enemyPrefab, vec, Quaternion.identity);
            }
        }

    }
}
