using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When allowed, spawn mines within some time interval. 
/// </summary>
public class MineSpawner : MonoBehaviour
{

    private bool allowed;
    [SerializeField] private float slowInterval = 4.0f;
    [SerializeField] private float fasterInterval = 2.0f;
    private float mineSpawnInterval;
    private float timer;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject boss;

    private void Start()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
        if (boss.GetComponent<Boss>() != null)
        {
            boss.GetComponent<Boss>().onEnragedCallback += BossEnrage;
            boss.GetComponent<Boss>().onMoreEnragedCallback += BossMoreEnrage;
            boss.GetComponent<Boss>().onBossDeathCallback += BossDeath;
        }

    }


    private void Update()
    {
        // Spawn Mines if boss is enraged. 
        if (allowed)
        {
            SpawnMines();
        }
    }

    public void SpawnMines()
    {
        if (Time.time - timer >= mineSpawnInterval)
        {
            Instantiate(minePrefab);
            timer = Time.time;
        }

    }


    private void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
        if (boss != null)
        {
            boss.GetComponent<Boss>().onEnragedCallback -= BossEnrage;
            boss.GetComponent<Boss>().onMoreEnragedCallback -= BossMoreEnrage;
            boss.GetComponent<Boss>().onBossDeathCallback -= BossDeath;
        }


    }

    private void PlayerDeath()
    {
        allowed = false;
        timer = Time.time;
    }

    private void BossEnrage()
    {
        allowed = true;
        mineSpawnInterval = slowInterval;
        timer = Time.time;
    }

    private void BossMoreEnrage()
    {
        mineSpawnInterval = fasterInterval;
    }


    /// <summary>
    /// When the boss dies, stop spawning more mines. 
    /// </summary>
    private void BossDeath()
    {
        // Cleanup references
        boss.GetComponent<Boss>().onEnragedCallback -= BossEnrage;
        boss.GetComponent<Boss>().onMoreEnragedCallback -= BossMoreEnrage;
        boss.GetComponent<Boss>().onBossDeathCallback -= BossDeath;

        // Destory this object
        Destroy(this.gameObject);
    }


    private void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

}
