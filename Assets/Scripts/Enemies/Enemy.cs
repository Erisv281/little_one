using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float recoilLength;    // How long the recoil lasts
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isFacingRight;
    protected float recoilTimer;
    protected Rigidbody2D RB;
    [SerializeField] protected int damage;
    protected GameObject deathEffect;
    [SerializeField] protected float speed;

    protected Animator anim;
    protected SpriteRenderer sr;

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

    protected EnemyStates currentState;

    [SerializeField] protected LayerMask groundLayer;

    protected virtual void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        isFacingRight = true;
        RB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        changeState(EnemyStates.Idle);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //print(currentState);
        if (!GameManager.instance.player.pstate.isAlive)
        {
            changeState(EnemyStates.Idle);
        }
        updateEnemyStates();

        //if (!isRecoiling)
        //{
        //  Vector2 targetPos = new Vector2(GameManager.instance.player.gameObject.transform.position.x, transform.position.y);
        //transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //}

    }

    protected virtual void takeDamage(int damage, Vector2 hitDirection, float hitForce)
    {
        health -= damage;

        if (health <= 0)
        {
            changeState(EnemyStates.Death);
            return;
            //enemyDeath();
        }
        // If not recoiling then push the enemy back 
        if (!isState(EnemyStates.Recoil))
        {
            // Note: Spawn hit effect here
            RB.velocity = hitDirection * hitForce * recoilFactor;
            recoilTimer = Time.time;
            changeState(EnemyStates.Recoil);
        }

    }

    public void enemyHit(int damage, Vector2 hitDirection, float hitForce)
    {
        takeDamage(damage, hitDirection, hitForce);
    }

    protected virtual void Attack()
    {
        GameManager.instance.player.TakeDamage(this.damage);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (isState(EnemyStates.Death))
        {
            return;
        }

        if (other.CompareTag("player") && !GameManager.instance.player.pstate.isInvinsible)
        {
            Attack();
        }
    }

    protected virtual void updateEnemyStates()
    {
        // Placeholder for updating the enemy state
        switch (currentState)
        {
            case EnemyStates.Idle:
                enemyIdle();
                break;

            case EnemyStates.Flip:
                enemyFlip();
                break;

            case EnemyStates.Chase:
                enemyChase();
                break;

            case EnemyStates.Recoil:
                enemyRecoil();
                break;

            case EnemyStates.Death:
                enemyDeath();
                break;

            case EnemyStates.Surprise:
                enemySurprise();
                break;

            case EnemyStates.Charge:
                enemyCharge();
                break;

        }
    }

    protected virtual void enemyIdle()
    {
        // Placeholder for enemy is idle
    }

    protected virtual void enemyFlip()
    {
        // Placeholder for enemy is flipping
    }

    protected virtual void enemyChase()
    {
        // Placeholder for enemy is chasing player
    }

    protected virtual void enemyRecoil()
    {
        // Placeholder for enemy is recoiling
    }

    protected virtual void enemyDeath()
    {
        // Note: Spawn death effect here
        Destroy(gameObject);
    }

    protected virtual void enemySurprise()
    {
        // Placeholder for when the enemy is surprised
    }

    protected virtual void enemyCharge()
    {
        // Placeholder for enemy charging
    }

    protected virtual void changeState(EnemyStates newState)
    {
        currentState = newState;
    }

    protected virtual bool isState(EnemyStates newState)
    {
        return currentState == newState;
    }

    protected virtual void Turn()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

    protected virtual void changeAnimation()
    {

    }

    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }


    //Note: Rigidbody2d collision set to continous --> Performance heavy
}
