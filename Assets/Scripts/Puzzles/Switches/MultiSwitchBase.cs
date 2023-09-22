using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSwitchBase : MonoBehaviour
{
    [SerializeField] protected Sprite activatedSwitch;   // The switched switch sprite  
    [SerializeField] protected Sprite regularSwitch;   // The OG sprite  
    [SerializeField] protected SpriteRenderer SR;
    [SerializeField] protected List<SwitchTrigger> triggers;    // Has multiple triggers


    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.sprite = regularSwitch;
        // Assuming our list of switchtriggers are correct


    }

    protected virtual void ActivateSwitch()
    {
        foreach (SwitchTrigger trigger in triggers)
        {
            trigger.CallActivate(); // Activate the triggerers
        }

        SR.sprite = activatedSwitch;
    }

    public void CallActivateSwitch()
    {
        ActivateSwitch();
    }
}
