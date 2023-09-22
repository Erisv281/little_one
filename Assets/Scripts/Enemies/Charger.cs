using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Walker
{
    [SerializeField] private float chargeSpeedMultiplier;    // How x faster enemy becomes
    [SerializeField] private float chargeDuration;
    [SerializeField] private float surpriseDuration; // How long to be surprised
    [SerializeField] private GameObject surpriseAnimPrefab;
    float chargeTimer;
    float surpriseTimer;

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position + ledgeCheckStart, wallCheckDir, ledgeCheck.x * 10);
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
        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeDuration)
        {

            if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheck.y, groundLayer))
            {
                float directionX = transform.localScale.x >= 0 ? chargeSpeedMultiplier : -chargeSpeedMultiplier;
                RB.velocity = new Vector2(directionX, RB.velocity.y);
            }
            else
            {
                RB.velocity = Vector2.zero;
            }
        }
        else
        {
            // Charging done
            chargeTimer = 0;
            ChangeState(EnemyStates.Idle);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }

    protected override void ChangeAnimation()
    {
        anim.SetBool("Charger_idle", IsState(EnemyStates.Idle));            // Note: Need to match
        anim.SetBool("Charger_charge", IsState(EnemyStates.Charge));

        if (IsState(EnemyStates.Idle))
        {
            anim.speed = 1;
        }

        if (IsState(EnemyStates.Charge))
        {
            anim.speed = chargeSpeedMultiplier;
        }

    }
}

