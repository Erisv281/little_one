using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// These mines drop from some random positions and when hitting the ground they splinter into 5 boss arrows. 
/// </summary>
public class Boss_mine : MonoBehaviour
{
    [SerializeField] private List<Vector3> spawnPositions;
    [SerializeField] private GameObject bossArrow;
    [SerializeField] private Transform firepoint;
    private Rigidbody2D RB;
    private Vector2 RBPausedVelocity;

    // Ground check
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private LayerMask _groundLayer;

    private void Update()
    {
        if (IsGrounded())
        {
            // Instantiate 5 arrows: (up, right, left, up-right and up-left)
            SpawnArrows();
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
        SpawnPosition();
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }


    /// <summary>
    /// Spawn at one random position. 
    /// </summary>
    private void SpawnPosition()
    {
        // Check if teleport position is valid
        Vector3 spawnPoint;
        do
        {
            int randomIndex = Random.Range(0, spawnPositions.Count - 1);
            spawnPoint = spawnPositions[randomIndex];
        }
        while (!isTeleportAvailable(spawnPoint));

        // Now place boss on this spawnPosition
        transform.position = spawnPoint;
    }

    /// <summary>
    /// Return T if the teleportPos is not within range of the player. 
    /// </summary>
    /// <param name="teleportPos"></param>
    /// <returns></returns>
    private bool isTeleportAvailable(Vector3 teleportPos)
    {
        Vector3 playerPos = GameManager.instance.player.transform.position;
        if (Vector3.Distance(playerPos, teleportPos) < 3)
        {
            return false;
        }
        return true;
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {
            return true;
        }
        return false;
    }

    private void SpawnArrows()
    {
        Instantiate(bossArrow, firepoint.position, Quaternion.identity);
        Instantiate(bossArrow, firepoint.position, Quaternion.Euler(0, 0, 180));
        Instantiate(bossArrow, firepoint.position, Quaternion.Euler(0, 0, 90));
        Instantiate(bossArrow, firepoint.position, Quaternion.Euler(0, 0, 45));
        Instantiate(bossArrow, firepoint.position, Quaternion.Euler(0, 0, 125));

        // Note: These does not instantiates. 
    }

    // GP
    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
        if (!enabled)
        {
            RBPausedVelocity = RB.velocity;
            StopMovement();
        }
        else
        {
            RB.velocity = RBPausedVelocity;
        }
    }

    /// <summary>
    /// Stops the movement of the arrow
    /// </summary>
    protected void StopMovement()
    {
        RB.velocity = Vector2.zero;
        RB.Sleep();
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }


}
