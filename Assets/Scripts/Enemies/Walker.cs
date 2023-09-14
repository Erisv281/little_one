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

    private void OnCollisionStay2D(Collision2D other)
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
                    // If hitting the wall, start flipping
                    UpdateLedgeCheck();
                    if (!Physics2D.Raycast(transform.position + ledgeCheckStart, Vector2.down, ledgeCheck.y, groundLayer)   // Ground check
                    || Physics2D.Raycast(transform.position, wallCheckDir, ledgeCheck.x, groundLayer))       // Wall check
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
        else if (other.transform.tag == "enemy")
        {
            Turn(); // If the enemy is stuck within an enemy, then turn
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        // If hitting an enemy, turn
        if (other.gameObject.CompareTag("enemy"))
        {
            Turn();
        }
    }

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

}
