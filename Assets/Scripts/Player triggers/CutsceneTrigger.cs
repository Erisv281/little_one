using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject target;
    private bool interacted;
    [SerializeField] private bool shallPlayOnce; // Set this to T if you want to only play animation once. 
    private bool playedOnce;    // This variable checks if the cutscene has been played once. 

    private void Start()
    {
        timeline = GetComponent<PlayableDirector>();

        // Pause the timeline at the start
        timeline.Pause();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!interacted && other.CompareTag("player"))
        {
            if (shallPlayOnce && playedOnce)
            {
                return;
            }

            interacted = true;
            if (!shallPlayOnce)
            {
                timeline.Play();
            }

            if (shallPlayOnce && !playedOnce)
            {
                timeline.Play();
                playedOnce = true;
            }


        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interacted && other.CompareTag("player"))
        {
            interacted = false;
            timeline.Stop();
            target.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
