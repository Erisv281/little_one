using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected int arrowDamage;
    [SerializeField] protected Rigidbody2D RB;
    [SerializeField] protected float hitForce;   // The knockback force
    public bool isLit;
    [SerializeField] private Sprite fireArrowSprite;
    protected SpriteRenderer SR;

    protected Vector2 RBPausedVelocity;

    protected bool hasHitSomething;   // Fail safe, since arrow detect collisions twice appearently

    // Start is called before the first frame update
    protected virtual void Start()
    {
        RB.velocity = transform.right * speed;
        SR = GetComponent<SpriteRenderer>();
    }

    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Fixing the bug where entities can take damage twice. 
        if (hasHitSomething)
        {
            return;
        }

        // Return if the arrow is hitting the player,...
        if (other.CompareTag("player"))
        {
            if (!isLit)
            {
                return;
            }
            //.., however firearrows hits the player!
            else
            {
                PlayerMovement p = GameManager.instance.player;
                p.TakeDamage(arrowDamage, (p.transform.position - transform.position).normalized, hitForce);
                hasHitSomething = true;
            }

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

    // GP

    protected void OnGameStateChanged(GameState gameState)
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

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }

    /// <summary>
    /// Stops the movement of the arrow
    /// </summary>
    protected void StopMovement()
    {
        RB.velocity = Vector2.zero;
        RB.Sleep();
    }

}
