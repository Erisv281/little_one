using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int arrowDamage;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private float hitForce;   // The knockback force
    public bool isLit;
    [SerializeField] private Sprite fireArrowSprite;
    private SpriteRenderer SR;

    private bool hasHitSomething;   // Fail safe, since arrow detect collisions twice appearently

    // Start is called before the first frame update
    void Start()
    {
        RB.velocity = transform.right * speed;
        SR = GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitSomething)
        {
            return;
        }

        // Return if the arrow is hitting the player
        if (other.CompareTag("player"))
        {
            return;
        }

        // Enemies or Chargers will take damage
        else if (other.CompareTag("enemy") || (other.CompareTag("charger") && isLit))
        {
            Enemy e = other.GetComponent<Enemy>();
            e.EnemyHit(arrowDamage, (e.transform.position - transform.position).normalized, hitForce);
            hasHitSomething = true;
        }
        // Mirrors reflects arrow to which direction the mirror is facing
        else if (other.CompareTag("mirror"))
        {
            transform.rotation = other.transform.rotation;
            RB.velocity = transform.right * speed;
            return;
        }
        // Fire will lit the arrow on fire
        else if (other.CompareTag("fire"))
        {
            isLit = true;
            SR.sprite = fireArrowSprite;
            return;
        }
        // If hitting bow_switch, Levers or Switches then activate it
        else if (other.CompareTag("switch"))
        {
            other.GetComponent<SwitchBase>().CallActivateSwitch();
            hasHitSomething = true;
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
