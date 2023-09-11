using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed;

    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Awake()
    {
        GameStateManager.onGameStateChanged += onGameStateChanged;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = GameManager.instance.player.gameObject.transform.position;   //Testing othger script
        transform.position = Vector3.Lerp(transform.position, playerPos + offset, followSpeed);

    }

    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }


}
