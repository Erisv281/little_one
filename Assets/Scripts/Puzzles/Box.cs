using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [SerializeField] private Sprite damagedCrate;
    [SerializeField] private Sprite moreDamagedCrate;
    private SpriteRenderer SR;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        SR = GetComponent<SpriteRenderer>();

    }

    void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If collide with arrow AND arrow is on fire then Destroy, if not on fire then take Damage
        if (other.CompareTag("arrow"))
        {
            if (other.GetComponent<Arrow>().isLit)
            {
                Destroy(gameObject);
            }
            else
            {
                TakeDamage(1);
                Destroy(other.gameObject);
            }

        }
        else if (other.CompareTag("fire"))
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        // Change sprite of the box depending on how much health remains. 
        health -= damage;

        if (health <= 2 && health > 0)
        {
            SR.sprite = moreDamagedCrate;
        }
        else if (health <= 0)
        {
            DestroyBox();
        }
        else
        {
            SR.sprite = damagedCrate;
        }

        // Eg. Start health = 3
        // Health = 2 --> DamagedCrate
        // Health = 1 --> MoreDamaged
        // Health = 0 --> Destroyed
    }

    public void DestroyBox()
    {
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
