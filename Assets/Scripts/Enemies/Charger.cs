using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Walker
{
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float chargeDuration;
    float chargeTimer;

    protected override void enemySurprise()
    {
        // Do something here, then charge
        //RB.velocity = new Vector2(0, jumpForce);
        changeState(EnemyStates.Charge);

    }

    protected override void enemyIdle()
    {
        base.enemyIdle();
        // Cast a raycast in front of the player, charge when closed enough
        updateLedgeCheck();

        RaycastHit2D hit = Physics2D.Raycast(transform.position + ledgeCheckStart, wallCheckDir, ledgeCheck.x * 10);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("player"))
            {
                changeState(EnemyStates.Surprise);
            }
        }
    }

    protected override void enemyCharge()
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
                //RB.velocity = new Vector2(0, RB.velocity.y);
                RB.velocity = Vector2.zero;
            }
        }
        else
        {
            // Charging done
            chargeTimer = 0;
            changeState(EnemyStates.Idle);

        }
    }

    protected override void changeAnimation()
    {
        if (isState(EnemyStates.Idle))
        {
            anim.speed = 1;
        }

        if (isState(EnemyStates.Charge))
        {
            anim.speed = chargeSpeedMultiplier;
        }

    }
}

//Buggar: 

// Does not jump vertically solemly

// When jumping: Gets stuck at close corner walls. 
