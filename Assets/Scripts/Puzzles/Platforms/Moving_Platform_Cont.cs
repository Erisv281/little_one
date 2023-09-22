using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Press a switch for this platform to move between two points, when switch is of, it stops. 
/// </summary>
public class Moving_Platform_Cont : SwitchTrigger
{
    [SerializeField] private Vector3 endPoint;  // Where the endpoint is located
    [SerializeField] private float movingSpeed;
    private Vector3 startPoint;
    private Vector3 targetPoint;    // Which point we shall move to. 
    private bool isMoving;

    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        transform.position = startPoint;
        targetPoint = endPoint;
        isMoving = false;
    }
    protected override void Start()
    {
        base.Start();
        startPoint = transform.position;
        targetPoint = endPoint;
    }

    protected override void Update()
    {
        // Rotate to next position
        if (isMoving)
        {
            // Rotate towards endpoint and check if within area. 
            if (Vector3.Distance(transform.position, targetPoint) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * movingSpeed);
            }
            else
            {
                // Set new targetpoint
                targetPoint = targetPoint == endPoint ? startPoint : endPoint;
            }

        }

    }

    protected override void Activate()
    {
        // Move platform if having reached destination
        if (!isMoving)
        {
            base.Activate();
            isMoving = true;
        }

    }

    protected override void Deactivate()
    {
        // Move platform back if having reached destination
        if (isMoving)
        {
            base.Deactivate();
            isMoving = false;
        }
    }

}
