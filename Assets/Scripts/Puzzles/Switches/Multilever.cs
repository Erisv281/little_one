using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLever : MultiSwitchBase
{
    [SerializeField] private bool havePulled;


    protected override void ActivateSwitch()
    {
        if (!havePulled)
        {
            PullLever();
            foreach (SwitchTrigger trigger in this.triggers)
            {
                trigger.CallActivate();
            }

        }
        else
        {
            PullLever();
            foreach (SwitchTrigger trigger in this.triggers)
            {
                trigger.CallDeactivate();
            }
        }
    }

    public void PullLever()
    {
        SR.sprite = !havePulled ? activatedSwitch : regularSwitch;
        havePulled = !havePulled;
    }
}
