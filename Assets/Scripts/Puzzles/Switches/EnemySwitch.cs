using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwitch : MonoBehaviour
{

    [SerializeField] protected List<SwitchTrigger> triggers;    // Triggers

    [SerializeField] private List<Enemy> enemies;   // The enemies. 
    public bool roomCleared;    // If the room is safe from enemies. 

    public void Update()
    {
        // Activate trigger when all enemies from the list are dead. 
        if (areEnemiesDead())
        {
            ClearRoom();
        }
    }

    public bool areEnemiesDead()
    {
        foreach (Enemy e in enemies)
        {
            if (e != null)
            {
                return false;
            }
        }
        return true;

    }

    public void ClearRoom()
    {
        roomCleared = true;
        foreach (SwitchTrigger trigger in triggers)
        {
            trigger.CallActivate(); // Activate the triggerers
        }
    }
}
