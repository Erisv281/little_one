using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float attackTime = 0.5f;   // Time to next attack
    private float timeSinceAttack = 0f;
    [SerializeField] private float hitForce = 20;   // The knockback force

    private void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    public void resetAttackTimer()
    {
        timeSinceAttack = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.player.unlocks.hasUnlockedMelee)
        {
            return;
        }
        if (Time.time - timeSinceAttack >= attackTime)
        {
            if (Input.GetButton("Melee") && CanAttack())     // right click
            {
                MeleeAttack();
                timeSinceAttack = Time.time;
            }
            else
            {
                GameManager.instance.player.pstate.isAttackingMelee = false;
            }
        }

    }


    /// <summary>
    /// Return T if, player is not attacked or player is not using its bow
    /// </summary>
    public bool CanAttack()
    {
        PlayerMovement p = GameManager.instance.player;
        return !p.pstate.isInvinsible && !p.pstate.isAttackingBow;
    }

    void MeleeAttack()
    {
        // Animation
        anim.SetTrigger("Attack");
        GameManager.instance.player.pstate.isAttackingMelee = true;

        //Detect enemies within some circle radius
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D e in enemies)
        {
            Enemy enemy = e.GetComponent<Enemy>();
            enemy.EnemyHit(meleeDamage, (e.transform.position - transform.position).normalized, hitForce);
        }

        // Recoil the player when melee hit enemies
        if (enemies.Length > 0)
        {
            GameManager.instance.player.SetRecoilingDirection();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
