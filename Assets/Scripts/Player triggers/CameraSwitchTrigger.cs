using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An adapter for camera and switches. This script move the camera when a switch has been pressed. 
/// </summary>
public class CameraSwitchTrigger : MonoBehaviour
{
    [SerializeField] private SwitchBase switchBase;
    [SerializeField] private CameraTrigger cameraTrigger;

    private bool isActivated;

    private void Awake()
    {
        switchBase = GetComponent<SwitchBase>();
        cameraTrigger = GetComponent<CameraTrigger>();
        switchBase.onSwitchHitCallback += SwitchActivationCallback;
    }

    private void OnDestroy()
    {
        switchBase.onSwitchHitCallback -= SwitchActivationCallback;
    }

    private void SwitchActivationCallback()
    {
        if (!isActivated)
        {
            isActivated = true;
            cameraTrigger.TriggerCamera();
        }
    }


}
