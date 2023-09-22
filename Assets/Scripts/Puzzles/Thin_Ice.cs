using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thin_Ice : MonoBehaviour
{
    [SerializeField] private float meltDuration = 2f;
    private float timer = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time - timer >= meltDuration)
        {
            // Destroy ice
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        // Start to break the ice animation. A switch can make it grow back?  
        if (other.CompareTag("player"))
        {
            timer = Time.time;
        }
    }
}
