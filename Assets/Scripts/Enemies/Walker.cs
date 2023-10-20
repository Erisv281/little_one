using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{

    protected float flipTimer;
    [SerializeField] protected float flipWaitTime;
    [SerializeField] protected Vector2 ledgeCheck;
    [SerializeField] protected float flipCooldown;
    protected bool isFlipping;
    protected Vector3 ledgeCheckStart;  // Vector for the ledge start pos
    protected Vector2 wallCheckDir;     // Vector for wall checking direction

    [SerializeField] protected Transform _frontWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);


    protected override void EnemyIdle()
    {
        float directionX = transform.localScale.x >= 0 ? speed : -speed;
        RB.velocity = new Vector2(directionX, RB.velocity.y);
    }

    protected override void EnemyFlip()
    {
        // Walk, then when seeing edge. Flip
        flipTimer += Time.deltaTime;
        if (flipTimer > flipWaitTime)
        {
            flipTimer = 0;
            Turn();
            ChangeState(EnemyStates.Idle);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag == "ground")
        {
            foreach (ContactPoint2D contactPoint in other.contacts)
            {
                Vector2 contactDir = contactPoint.normal;   // The direction (y-value increases downwards, x-values increases left)

                if (contactDir.y > 0)
                {
                    // Ground collision
                }

                // If recoiling and Recoiled by player, facing the player and gets recoiled to a wall.
                if (IsState(EnemyStates.Recoil))
                {
                    if ((isFacingRight && contactDir.x > 0) || (!isFacingRight && contactDir.x < 0))
                    {
                        StartCoroutine(FlipCooldown());
                        return;
                    }
                }
                else if (IsState(EnemyStates.Idle))
                {
                    // If hitting the walls, start flipping
                    UpdateLedgeCheck();
                    if (!Physics2D.Raycast(transform.position + ledgeCheckStart, Vector2.down, ledgeCheck.y, groundLayer)   // Ground check
                    || Physics2D.Raycast(transform.position, wallCheckDir, ledgeCheck.x, groundLayer)) // Wall check
                    {
                        ChangeState(EnemyStates.Flip);
                        StartCoroutine(FlipCooldown());
                        return;
                    }
                }

                // Roof collision AND we're not flipping
                if (contactDir.y <= 0 && !isFlipping)
                {
                    ChangeState(EnemyStates.Flip);
                    StartCoroutine(FlipCooldown());
                    return;
                }
            }
        }
    }

    /// <summary>
    /// // If the enemy is stuck within another enemy, traps and is not the player then turn. 
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("player"))
        {
            return;
        }

        // Only turn once we collide with some attackable
        UpdateLedgeCheck();
        if (!isFlipping && Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, attackableLayer))
        {
            Turn();
        }
    }

    // Charger collide with himself sp overlapbox.collider == charger. 

    private IEnumerator FlipCooldown()
    {
        isFlipping = true;
        yield return new WaitForSeconds(flipCooldown);
        isFlipping = false;
    }

    protected override void EnemyRecoil()
    {
        if (Time.time - recoilTimer >= recoilDuration)
        {
            // Recoil done, change state to idle
            recoilTimer = Time.time;
            ChangeState(EnemyStates.Idle);
            RB.velocity = Vector2.zero;
        }
    }

    protected virtual void UpdateLedgeCheck()
    {
        // CHeck for wall and check for the ledge
        ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheck.x, 0) : new Vector3(-ledgeCheck.x, 0);
        wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
    }

    protected override void ChangeAnimation()
    {
        anim.SetBool("Walker_idle", IsState(EnemyStates.Idle));
    }

    protected void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, Vector2.down * ledgeCheck.y);

    }

}
