using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

    public Transform firePoint;
    public GameObject arrow;
    [SerializeField] private float attackTime = 0.5f;   // Time to next attack
    private float nextAttackTime = 0f;
    [SerializeField] private Animator anim;

    void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.player.unlocks.hasUnlockedBow)
        {
            return;
        }

        if (Time.time - nextAttackTime >= attackTime)
        {
            // Can only shoot when not attacked nor using melee
            if (Input.GetButtonDown("Bow") && CanAttack())   // left click
            {
                Shoot();
                nextAttackTime = Time.time;
            }
            else
            {
                GameManager.instance.player.pstate.isAttackingBow = false;
            }
        }


    }

    public void resetAttackTimer()
    {
        nextAttackTime = 0f;
    }

    public bool CanAttack()
    {
        PlayerMovement p = GameManager.instance.player;
        return !p.pstate.isInvinsible && !p.pstate.isAttackingMelee && p.pstate.isAlive && !p.pstate.isEnteringCutscene;
    }

    void Shoot()
    {
        // Animation and sounds
        anim.SetTrigger("BowAttack");
        AudioManager.instance.Play("Bow");

        // Bow shooting
        GameManager.instance.player.pstate.isAttackingBow = true;
        Instantiate(arrow, firePoint.position, firePoint.rotation);
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



// Holding Horizontal + mouse: Fire
// Holding UP + mouse: Fire up
// Holding DOWN + mouse: Firw down
// Holding UP+Right etc.: Fire that direction. 
