using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int arrowDamage;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private float hitForce;   // The knockback force

    // Start is called before the first frame update
    void Start()
    {
        RB.velocity = transform.right * speed;
    }

    void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Return if the arrow is hitting the player
        if (other.GetComponent<PlayerMovement>() != null)
        {
            return;
        }

        Enemy e = other.GetComponent<Enemy>();
        if (e != null)
        {
            e.EnemyHit(arrowDamage, (e.transform.position - transform.position).normalized, hitForce);
        }
        Destroy(gameObject);
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }

}
