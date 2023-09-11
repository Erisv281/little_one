using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{

    [SerializeField] private float chaseDistance;
    private float distanceToPlayer;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        changeState(EnemyStates.Idle);
        distanceToPlayer = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);

    }

    // Update is called once per frame
    protected override void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);
        base.Update();

    }

    protected override void enemyIdle()
    {
        if (distanceToPlayer <= chaseDistance)
        {
            changeState(EnemyStates.Chase);
        }

    }

    protected override void enemyChase()
    {
        RB.MovePosition(Vector2.MoveTowards(transform.position,
         GameManager.instance.player.transform.position, Time.deltaTime * speed));

        Turn();
    }

    protected override void enemyRecoil()
    {
        if (Time.time - recoilTimer >= recoilLength)
        {
            recoilTimer = Time.time;
            changeState(EnemyStates.Idle);
            RB.velocity = Vector2.zero;

        }
    }

    protected override void enemyDeath()
    {
        RB.gravityScale = 12;
        base.enemyDeath();

    }


    protected override void Turn()
    {
        sr.flipX = GameManager.instance.player.transform.position.x < transform.position.x ? true : false;
    }

    protected override void changeAnimation()
    {
        anim.SetBool("Idle", isState(EnemyStates.Idle));            // Note: Need to match
        anim.SetBool("Chase", isState(EnemyStates.Chase));
        anim.SetBool("Recoil", isState(EnemyStates.Recoil));
        if (isState(EnemyStates.Death))
        {
            anim.SetTrigger("Death");

        }
    }





}
