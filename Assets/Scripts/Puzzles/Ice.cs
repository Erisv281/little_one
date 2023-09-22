using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If collide with arrow AND arrow is on fire
        if (other.CompareTag("arrow") && other.GetComponent<Arrow>().isLit)
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("fire"))
        {
            Destroy(gameObject);
        }
    }

}
