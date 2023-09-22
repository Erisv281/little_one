using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : SwitchTrigger
{
    [SerializeField] private Vector3 endPoint;  // Where the endpoint is located
    [SerializeField] private float movingSpeed;
    private Vector3 startPoint;
    private bool isMoving;
    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        transform.position = startPoint;
        isMoving = false;
    }

    protected override void Start()
    {
        base.Start();
        startPoint = transform.position;
    }

    protected override void Update()
    {
        // Rotate to next position
        if (isMoving)
        {
            // Rotate towards endpoint and check if within area. 
            if (Vector3.Distance(transform.position, endPoint) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPoint, Time.deltaTime * movingSpeed);
            }
        }
        else
        {
            // Rotate back to the startRotation
            if (Vector3.Distance(transform.position, startPoint) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPoint, Time.deltaTime * movingSpeed);
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
