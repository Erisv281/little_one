using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating_Platform_Cont : SwitchTrigger
{

    private Quaternion endPoint;
    [SerializeField] private float rotationAmount;  // How much shall we rotate (90 degrees)
    [SerializeField] private float rotationSpeed;  // How fast rotation
    private Vector3 rotationAxis;
    private Quaternion startRotation;
    private bool isRotating;

    protected override void PlayerDeath()
    {
        base.PlayerDeath();
        transform.rotation = startRotation;
        isRotating = false;
    }

    protected override void Start()
    {
        base.Start();
        startRotation = transform.rotation;
        rotationAxis = new Vector3(0, 0, rotationAmount);
    }

    protected override void Update()
    {
        // Rotate to next position
        if (isRotating)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
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
