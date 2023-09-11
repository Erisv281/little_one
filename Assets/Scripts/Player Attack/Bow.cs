using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

    public Transform firePoint;
    public GameObject arrow;
    [SerializeField] private float attackTime = 0.5f;   // Time to next attack
    private float nextAttackTime = 0f;

    void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time - nextAttackTime >= attackTime)
        {
            if (Input.GetButtonDown("Bow"))   // left click
            {
                Shoot();
                nextAttackTime = Time.time;
            }
        }

    }

    void Shoot()
    {
        // Bow shooting
        Instantiate(arrow, firePoint.position, firePoint.rotation);
    }

    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }

}



// Holding Horizontal + mouse: Fire
// Holding UP + mouse: Fire up
// Holding DOWN + mouse: Firw down
// Holding UP+Right etc.: Fire that direction. 
