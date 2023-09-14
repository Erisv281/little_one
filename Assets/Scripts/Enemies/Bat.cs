using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{

    [SerializeField] private float chaseRadius;   // The radius where Bat will spot player
    private float distanceToPlayer;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Idle);
        distanceToPlayer = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);

    }

    // Update is called once per frame
    protected override void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);
        base.Update();
    }

    protected override void EnemyIdle()
    {
        // If player is within chase radius, start chase player. 
        if (distanceToPlayer <= chaseRadius)
        {
            ChangeState(EnemyStates.Chase);
        }

    }

    protected override void EnemyChase()
    {
        // Move towards the player
        RB.MovePosition(Vector2.MoveTowards(transform.position,
         GameManager.instance.player.transform.position, Time.deltaTime * speed));

        Turn();
    }

    protected override void EnemyRecoil()
    {
        if (Time.time - recoilTimer >= recoilDuration)
        {
            recoilTimer = Time.time;
            ChangeState(EnemyStates.Idle);
            RB.velocity = Vector2.zero;
        }
    }

    protected override void EnemyDeath()
    {
        RB.gravityScale = 12;
        base.EnemyDeath();
    }


    protected override void Turn()
    {
        sr.flipX = GameManager.instance.player.transform.position.x < transform.position.x ? true : false;
    }

    protected override void ChangeAnimation()
    {
        anim.SetBool("Bat_idle", IsState(EnemyStates.Idle));            // Note: Need to match
        anim.SetBool("Chase", IsState(EnemyStates.Chase));
        anim.SetBool("Recoil", IsState(EnemyStates.Recoil));
        if (IsState(EnemyStates.Death))
        {
            anim.SetTrigger("Death");

        }
    }





}
