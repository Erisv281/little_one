using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailParticles : MonoBehaviour
{
    private ParticleSystem particles;
    private bool isWalking;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.player.pstate.isMoving && GameManager.instance.player.IsGrounded())
        {
            if (!isWalking)
            {
                isWalking = true;
                particles.Play();
            }
        }
        else
        {
            isWalking = false;
            particles.Stop();
        }
    }
}
