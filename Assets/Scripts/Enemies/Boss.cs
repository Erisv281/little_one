using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The final boss of the game. He can shoot arrows and teleport between locations. 
/// </summary>
public class Boss : Enemy
{
    // General
    [SerializeField] private List<Vector3> teleportCoordinates; // List of random teleport coordinates
    private bool isInvisible;   // The boss is invisible when recoiled or doing rage animation. 


    // Attack
    public Transform firePoint;
    public GameObject arrow;    // Enemy arrow


    // Idle
    [SerializeField] private float shootCooldown;   // Cooldown between shooting arrows. 
    private float timer;


    // Surprise
    private float surpriseTimer;
    [SerializeField] private float surpriseDuration = 4.0f; // Duration of showing the surprise animation

    // Enragerd
    private bool isEnraged;
    private bool isMoreEnraged;
    [SerializeField] private float thresholdHealth; // Threshold for enraged

    [SerializeField] private float thresholdHealth2;    // The second threshold for more enraged
    [SerializeField] private float enragedShootCooldown;
    [SerializeField] private float teleportCooldown = 8.0f;
    private float teleportTimer;

    // Particle effect
    [SerializeField] private GameObject blackParticles;
    [SerializeField] private float particleDuration = 1.0f;

    // Callbacks
    public delegate void OnEnraged();
    [HideInInspector] public OnEnraged onEnragedCallback;

    public delegate void OnMoreEnraged();
    [HideInInspector] public OnMoreEnraged onMoreEnragedCallback;

    public delegate void OnBossDeath();
    [HideInInspector] public OnBossDeath onBossDeathCallback;





    protected override void PlayerDeath()
    {
        // Player has died and is respawning. 
        base.PlayerDeath();
        isEnraged = false;
        isMoreEnraged = false;
        isInvisible = false;
        timer = 0f;
        surpriseTimer = 0f;
    }

    protected override void Update()
    {
        if (isActive)
        {
            base.Update();
        }
    }

    protected override void TakeDamage(int damage, Vector2 hitDirection, float hitForce)
    {
        // Do not take damage is isinvible i.e. recoiled. 
        if (isInvisible)
        {
            return;
        }

        // Enable enraged mode when the health is less than the threshold.
        // Otherwise let the boss take damage and get recoiled.  
        if (health <= thresholdHealth && !isEnraged)
        {
            // Now the boss is in enraged Mode
            isEnraged = true;
            // Take damage without recoil
            health -= damage;
            if (health <= 0)
            {
                ChangeState(EnemyStates.Death);
                return;
            }

            // Switch to surprise state
            isInvisible = true;
            ChangeState(EnemyStates.Surprise);

            // Animation
            anim.SetTrigger("Rage");

            // Notify listeners
            EnragedCallback();
        }

        else if (health <= thresholdHealth2 && !isMoreEnraged)
        {
            isMoreEnraged = true;
            MoreEnragedCallback();
        }
        else
        {
            // Animation
            anim.SetTrigger("Recoil");

            // Reset teleport timer
            teleportTimer = 0f;

            // Take damage
            base.TakeDamage(damage, hitDirection, hitForce);
        }
    }


    protected override void EnemyIdle()
    {
        if (Time.time - timer >= shootCooldown)
        {
            Instantiate(arrow, firePoint.position, firePoint.rotation);
            timer = Time.time;
        }

        CheckTurn();
    }

    protected override void EnemyRecoil()
    {
        if (Time.time - recoilTimer >= recoilDuration)
        {
            // Recoil done, change state to idle or charge
            recoilTimer = Time.time;
            SetBossState();
            RB.velocity = Vector2.zero;

            // Also teleport away to some random position. 
            Teleport();
        }
    }

    protected override IEnumerator DeathAnim()
    {
        // Notify listeners
        BossDeathCallback();

        // Animation and sound
        anim.SetTrigger("Death");
        AudioManager.instance.Play("Collect");

        // Play death animation for 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        Instantiate(hart, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BossDeathCallback();    // Notify listeners that this object has been destroyed. 
    }


    /// <summary>
    /// Charging for boss is that his cooldown decreases for shooting arrows. 
    /// </summary>
    protected override void EnemyCharge()
    {
        if (Time.time - timer >= enragedShootCooldown)
        {
            Instantiate(arrow, firePoint.position, firePoint.rotation);
            timer = Time.time;
        }

        if (Time.time - teleportTimer >= teleportCooldown)
        {
            teleportTimer = Time.time;
            Teleport();
        }

        CheckTurn();
    }


    /// <summary>
    /// Show the enraged animation for some time, then turn to charge. .
    /// </summary>
    protected override void EnemySurprise()
    {
        if (Time.time - surpriseTimer >= surpriseDuration)
        {
            isInvisible = false;
            surpriseTimer = Time.time;
            ChangeState(EnemyStates.Charge);
        }

    }

    protected override void ChangeAnimation()
    {
        anim.SetBool("Boss_idle", (IsState(EnemyStates.Idle) || IsState(EnemyStates.Charge)));            // Note: Need to matchd
    }

    protected override IEnumerator StartInvinsibleAnimation()
    {
        isInvisible = true;
        yield return base.StartInvinsibleAnimation();
        isInvisible = false;
    }

    private void CheckTurn()
    {
        // Turn checker
        bool isPlayerLeftOfBoss = GameManager.instance.player.transform.position.x < transform.position.x ? true : false;

        // Turn left
        if (isPlayerLeftOfBoss && isFacingRight)
        {
            firePoint.transform.Rotate(0f, 0f, -180);   // The same as 180??
            Turn();
        }
        else if (!isPlayerLeftOfBoss && !isFacingRight)
        {
            firePoint.transform.Rotate(0f, 0f, 180);
            Turn();

        }
    }


    /// <summary>
    /// Set the boss state depending if the enraged mode active. 
    /// </summary>
    private void SetBossState()
    {
        if (isEnraged)
        {
            ChangeState(EnemyStates.Charge);
        }
        else
        {
            ChangeState(EnemyStates.Idle);
        }

    }

    /// <summary>
    /// Teleport to any of the set locations. 
    /// </summary>
    public void Teleport()
    {
        // Add some effects here. 
        GameObject particles = Instantiate(blackParticles, transform.position + Vector3.down, Quaternion.identity);
        Destroy(particles, particleDuration);

        // Check if teleport position is valid
        Vector3 teleportVector;
        do
        {
            int randomIndex = Random.Range(0, teleportCoordinates.Count - 1);
            teleportVector = teleportCoordinates[randomIndex];
        }
        while (!isTeleportAvailable(teleportVector));

        // Now place boss on this teleportcooridnate. 
        transform.position = teleportVector;

    }


    /// <summary>
    /// Return T if the teleportPos is not within range of the player. 
    /// </summary>
    /// <param name="teleportPos"></param>
    /// <returns></returns>
    public bool isTeleportAvailable(Vector3 teleportPos)
    {
        Vector3 playerPos = GameManager.instance.player.transform.position;
        if (Vector3.Distance(playerPos, teleportPos) < 3)
        {
            return false;
        }
        return true;
    }


    public void EnragedCallback()
    {
        // Notify subscribers that boss is enraged
        if (onEnragedCallback != null)
        {
            onEnragedCallback.Invoke();
        }
    }

    public void MoreEnragedCallback()
    {
        // Notify subscribers that boss is enraged
        if (onMoreEnragedCallback != null)
        {
            onMoreEnragedCallback.Invoke();
        }
    }


    public void BossDeathCallback()
    {
        // Notify subscribers that boss is enraged
        if (onBossDeathCallback != null)
        {
            onBossDeathCallback.Invoke();
        }
    }



}
