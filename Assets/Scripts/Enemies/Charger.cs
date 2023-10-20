using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Walker
{
    [SerializeField] private float chargeSpeedMultiplier;    // How x faster enemy becomes
    [SerializeField] private float chargeDuration;
    [SerializeField] private float surpriseDuration; // How long to be surprised
    [SerializeField] private GameObject surpriseAnimPrefab;
    [SerializeField] private GameObject chargeAnimPrefab;
    [SerializeField] private float watchLength;    // How far away the charger should see the player
    float chargeTimer;
    float surpriseTimer;
    private bool isCharging;

    protected override void EnemySurprise()
    {
        StartCoroutine(StartSurprise());
    }

    public IEnumerator StartSurprise()
    {
        ChangeState(EnemyStates.Charge);
        surpriseAnimPrefab.SetActive(true);
        yield return new WaitForSeconds(surpriseDuration);
        surpriseAnimPrefab.SetActive(false);
    }

    protected override void EnemyIdle()
    {
        base.EnemyIdle();
        // Cast a raycast in front of the player, charge when closed enough
        UpdateLedgeCheck();

        RaycastHit2D hit = Physics2D.Raycast(transform.position + ledgeCheckStart, wallCheckDir, watchLength);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("player"))
            {
                ChangeState(EnemyStates.Surprise);
            }
        }
    }

    protected override void EnemyCharge()
    {
        // Start animation once. 
        if (!isCharging)
        {
            StartCoroutine(StartCharge());
        }

        // Check if on ground, shall continue moving
        if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheck.y, groundLayer).collider != null)
        {
            float directionX = transform.localScale.x >= 0 ? chargeSpeedMultiplier : -chargeSpeedMultiplier;
            RB.velocity = new Vector2(directionX, RB.velocity.y);
        }

        else
        {
            RB.velocity = Vector2.zero;
        }

        // Check if colliding with wall
        UpdateLedgeCheck();
        if (!Physics2D.Raycast(transform.position + ledgeCheckStart, Vector2.down, ledgeCheck.y, groundLayer)   // Ground check
           || Physics2D.Raycast(transform.position, wallCheckDir, ledgeCheck.x, groundLayer)) // Wall check
        {
            Turn();
        }
    }

    public IEnumerator StartCharge()
    {
        isCharging = true;
        chargeAnimPrefab.SetActive(true);
        yield return new WaitForSeconds(chargeDuration);

        ChangeState(EnemyStates.Idle);  // When done, change to idle. 
        isCharging = false;
        chargeAnimPrefab.SetActive(false);


    }


    protected override void ChangeAnimation()
    {
        anim.SetBool("Charger_idle", IsState(EnemyStates.Idle));            // Note: Need to match
    }

    // For all enemies --> Stop movement like we did with the arrows!
}

