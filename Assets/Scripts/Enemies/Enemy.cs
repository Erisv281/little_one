using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected enum EnemyStates
    {
        Idle,
        Flip,
        Recoil,
        Chase,
        Death,
        Surprise,
        Charge,
    }

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float recoilDuration;    // How long the recoil lasts
    [SerializeField] protected float recoilFactor;  // How endurance the enemy is from knockback
    [SerializeField] protected float hitForce;   // How strong the enemy knockback player
    [SerializeField] protected bool isFacingRight;
    protected float recoilTimer;
    [SerializeField] protected int damage;
    //protected GameObject deathEffect;

    [SerializeField] protected float speed;
    protected Animator anim;
    protected Rigidbody2D RB;
    protected SpriteRenderer sr;
    [SerializeField] protected LayerMask groundLayer;
    protected EnemyStates currentState;
    [SerializeField] private FlashAnimation flashAnimation;


    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        RB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        ChangeState(EnemyStates.Idle);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!GameManager.instance.player.pstate.isAlive)
        {
            ChangeState(EnemyStates.Idle);
        }
        UpdateEnemyStates();
        ChangeAnimation();
    }

    protected virtual void TakeDamage(int damage, Vector2 hitDirection, float hitForce)
    {
        health -= damage;

        if (health <= 0)
        {
            ChangeState(EnemyStates.Death);
            return;
        }

        // If not recoiling then push the enemy back 
        if (!IsState(EnemyStates.Recoil))
        {
            // Note: Spawn hit effect here
            RB.velocity = hitDirection * hitForce * recoilFactor;
            recoilTimer = Time.time;
            ChangeState(EnemyStates.Recoil);
            StartCoroutine(StartInvinsibleAnimation());
        }

    }

    protected virtual IEnumerator StartInvinsibleAnimation()
    {
        //Set enemy invisibility = true ?
        //anim.SetTrigger("TakeDamage");
        flashAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(recoilDuration);
        flashAnimation.destroyFlash();
        flashAnimation.gameObject.SetActive(false);
        //Set enemy invisibility = true ?

    }

    /// <summary>
    /// Calls the TakeDamage function. 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitDirection"></param>
    /// <param name="hitForce"></param>
    public void EnemyHit(int damage, Vector2 hitDirection, float hitForce)
    {
        if (damage >= 0 && hitForce >= 0)
        {
            TakeDamage(damage, hitDirection, hitForce);
        }

    }

    protected virtual void Attack()
    {
        PlayerMovement p = GameManager.instance.player;
        p.TakeDamage(this.damage, (p.transform.position - transform.position).normalized, hitForce);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (IsState(EnemyStates.Death))
        {
            return;
        }

        if (other.CompareTag("player") && !GameManager.instance.player.pstate.isInvinsible && GameManager.instance.player.pstate.isAlive)
        {
            Attack();
        }
    }

    protected virtual void UpdateEnemyStates()
    {
        // Placeholder for updating the enemy state
        switch (currentState)
        {
            case EnemyStates.Idle:
                EnemyIdle();
                break;

            case EnemyStates.Flip:
                EnemyFlip();
                break;

            case EnemyStates.Chase:
                EnemyChase();
                break;

            case EnemyStates.Recoil:
                EnemyRecoil();
                break;

            case EnemyStates.Death:
                EnemyDeath();
                break;

            case EnemyStates.Surprise:
                EnemySurprise();
                break;

            case EnemyStates.Charge:
                EnemyCharge();
                break;
        }
    }

    protected virtual void EnemyIdle()
    {
        // Placeholder for enemy is idle
    }

    protected virtual void EnemyFlip()
    {
        // Placeholder for enemy is flipping
    }

    protected virtual void EnemyChase()
    {
        // Placeholder for enemy is chasing player
    }

    protected virtual void EnemyRecoil()
    {
        // Placeholder for enemy is recoiling
    }

    protected virtual void EnemyDeath()
    {
        // Note: Spawn death effect here
        Destroy(gameObject);
    }

    protected virtual void EnemySurprise()
    {
        // Placeholder for when the enemy is surprised
    }

    protected virtual void EnemyCharge()
    {
        // Placeholder for enemy charging
    }

    protected virtual void ChangeState(EnemyStates newState)
    {
        currentState = newState;
    }

    protected virtual bool IsState(EnemyStates newState)
    {
        return currentState == newState;
    }

    protected virtual void Turn()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

    protected virtual void ChangeAnimation()
    {
        // Placeholder for handling all animations

    }

    protected void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }


    //Note: Rigidbody2d collision set to continous --> Performance heavy
}
