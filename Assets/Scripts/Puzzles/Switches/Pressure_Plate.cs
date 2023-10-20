using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  ´RMOEVD REMOVED REMOVED
/// </summary>
public class Preasure_Plate : MonoBehaviour
{
    private SpriteRenderer SR;
    [SerializeField] private Sprite regularSwitch;    //Original form
    [SerializeField] private Sprite activatedSwitch;   //For swapping sprite. Start set empty.  
    [SerializeField] private SwitchTrigger triggerer;
    [SerializeField] private LayerMask plateLayer;

    private bool isStanding;

    // To fix the issue on register frames correclty we use delays between enter and exit. 
    private float exitTime = 0f;
    private float delay = 0.2f; // Adjust this value as needed



    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        if (triggerer == null)
        {
            triggerer = GetComponent<SwitchTrigger>();
        }
    }

    void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        exitTime = Time.time;

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time - exitTime < delay)
        {
            return;
        }


        if (IsOnPlate() && !isStanding)
        {
            // Do something here when standing on the plate 
            SR.sprite = activatedSwitch;
            triggerer.CallActivate();
            isStanding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Do something here when exiting the plate

        if (isStanding)
        {
            SR.sprite = regularSwitch;
            triggerer.CallDeactivate();
            isStanding = false;

        }
    }

    public bool IsOnPlate()
    {
        PlayerMovement p = GameManager.instance.player;
        if (Physics2D.OverlapBox(p._groundCheckPoint.position, p._groundCheckSize, 0, plateLayer))
        {
            return true;
        }
        return false;
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
    }


}

