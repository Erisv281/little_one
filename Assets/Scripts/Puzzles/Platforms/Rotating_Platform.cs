using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating_Platform : SwitchTrigger
{
    private Quaternion endPoint;
    [SerializeField] private float rotationAmount;  // How much shall we rotate (90 degrees)
    [SerializeField] private float rotationSpeed;  // How fast rotation
    private Quaternion startRotation;
    private bool isRotating;

    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        transform.rotation = startRotation;
        endPoint = Quaternion.Euler(0f, 0f, rotationAmount) * startRotation;
        isRotating = false;
    }

    protected override void Start()
    {
        base.Start();
        startRotation = transform.rotation;
        endPoint = Quaternion.Euler(0f, 0f, rotationAmount) * startRotation;
    }

    protected override void Update()
    {
        // Rotate to next position
        if (isRotating)
        {
            // Rotate towards endpoint and check if within area. 
            if (Quaternion.Angle(transform.rotation, endPoint) >= 0.01f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, endPoint, rotationAmount * Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // Rotate back to the startRotation
            if (Quaternion.Angle(transform.rotation, startRotation) >= 0.01f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, startRotation, rotationAmount * Time.deltaTime * rotationSpeed);
            }
        }
    }

    protected override void Activate()
    {
        //Rotate platform if having reached destination
        if (!isRotating)
        {
            base.Activate();
            isRotating = true;
        }

    }

    protected override void Deactivate()
    {
        // Rotate platform back if having reached destination
        if (isRotating)
        {
            base.Deactivate();
            isRotating = false;
        }
    }


}
