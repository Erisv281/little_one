using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// This script handles the camera movement between 
/// 1. Follow the player
/// 2. Slowly move between points
/// 3. Show a cutscene target
/// </summary>
public class CameraFollow : MonoBehaviour
{

    // Camera states
    public enum CameraStates
    {
        None,
        InCutscene, // Change camera to targetposition, then change quickly back
        SlowFollow, // Change camera to target, slowly change back
    }

    private CameraStates currentState;

    // Camera Following
    [SerializeField] private float followSpeed;
    [SerializeField] private Vector3 offset;    // Offset from the target position 

    // Camera Zooming
    private Camera mainCamera;
    private float initialOrthographicSize;
    [SerializeField] private float zoomFactor = 2.0f; // Adjust the zoom factor as needed

    // Cutscenes
    private Vector3 cutsceneTarget;   // Some point that the camera shall follow
    private bool isInCutscene;
    float cutSceneTimer = 0f;
    private float cutsceneDuration;
    [SerializeField] private float lerpSpeed = 2.0f;    // Speed which camera changes to point

    // Slow cutscenes
    private Stack<Vector3> targets;
    private Vector3 slowCurrentTarget;
    private Stack<float> targetSpeeds;
    private float slowCurrentSpeed;



    public void Start()
    {
        mainCamera = Camera.main;
        initialOrthographicSize = mainCamera.orthographicSize;

        // Slow cutscenes
        targets = new Stack<Vector3>();
        targetSpeeds = new Stack<float>();

        GameManager.instance.player.onPlayerDeathCallback += PlayerDeath;
    }

    public void Awake()
    {
        GameStateManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= OnGameStateChanged;
        GameManager.instance.player.onPlayerDeathCallback -= PlayerDeath;
    }

    private void PlayerDeath()
    {
        // Player has died and is respawning. 
        ChangeState(CameraStates.None);
        cutSceneTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        // Do nothing if in start scene
        if (SceneManager.GetActiveScene().name.Equals("Start_Scene"))
        {
            return;
        }

        switch (currentState)
        {
            case CameraStates.None:
                CameraDefault();
                break;
            case CameraStates.InCutscene:
                CameraInCutscene();
                break;
            case CameraStates.SlowFollow:
                CameraSlowFollow();
                break;
        }

    }

    /// <summary>
    /// Do this when camera follows the player + offset
    /// </summary>
    public void CameraDefault()
    {
        Vector3 playerPos = GameManager.instance.player.gameObject.transform.position;
        Vector3 desiredPosition = playerPos + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Do this when camera is in a cutscene. Lerps between position and the target. 
    /// </summary>
    public void CameraInCutscene()
    {
        transform.position = Vector3.Lerp(transform.position, cutsceneTarget + offset, lerpSpeed * Time.deltaTime);
        cutSceneTimer += Time.deltaTime;
        if (cutSceneTimer >= cutsceneDuration)
        {
            ChangeState(CameraStates.None);
            StartMovement();
        }

    }


    /// <summary>
    /// Do this when camera is slowly following a target
    /// </summary>
    public void CameraSlowFollow()
    {
        // Continue lerp if not close enough to target
        if (!(Vector3.Distance(transform.position, slowCurrentTarget + offset) < 1.0f))
        {
            transform.position = Vector3.Lerp(transform.position, slowCurrentTarget + offset, slowCurrentSpeed * Time.deltaTime);
        }
        else
        {
            // If stacks are empty, done. 
            if (targets.Count == 0 || targetSpeeds.Count == 0)
            {
                // Done, so change back to normal camera mode
                ChangeState(CameraStates.None);
                StartMovement();
                return;
            }
            else
            {
                slowCurrentTarget = targets.Pop();
                slowCurrentSpeed = targetSpeeds.Pop();
            }


        }

    }



    // Cutscene mode:
    public void StartCameraCutscene(Vector3 cutsceneTarget, float cutsceneDuration)
    {
        ChangeState(CameraStates.InCutscene);
        cutSceneTimer = 0f;
        this.cutsceneDuration = cutsceneDuration;
        this.cutsceneTarget = cutsceneTarget;

        StopMovement();
    }


    /// <summary>
    /// Slow cutscene mode;; Note: The first vector of the list is the final point
    /// </summary>
    /// <param name="_targets"></param>
    /// <param name="_speeds"></param>
    public void StartSlowCameraCutscene(List<Vector3> _targets, List<float> _speeds)
    {
        // Do nothing if the lists are not correspondant speed and target. Or empty lists
        if (_targets.Count != _speeds.Count || _targets.Count == 0 || _speeds.Count == 0)
        {
            return;
        }

        // Push targets
        foreach (Vector3 target in _targets)
        {
            targets.Push(target);
        }

        // Push correspondant speeds
        foreach (float speed in _speeds)
        {
            targetSpeeds.Push(speed);
        }

        // Set initial target and speed
        slowCurrentTarget = targets.Pop();
        slowCurrentSpeed = targetSpeeds.Pop();


        ChangeState(CameraStates.SlowFollow);
        StopMovement();
    }

    public void StopMovement()
    {
        // Stop player movement and animation
        GameManager.instance.player.pstate.isEnteringCutscene = true;
        GameManager.instance.player.StopMovement();
        GameManager.instance.player.ResetAnimation();

        // Pause the gameplay
        //GameManager.instance.switchGameState();
    }

    public void StartMovement()
    {
        // Let player move again
        GameManager.instance.player.pstate.isEnteringCutscene = false;

        // Unpause the gameplay
        //GameManager.instance.switchGameState();

    }




    // Zomming

    public void ZoomIn(float zoomDuration)
    {
        float targetOrthographicSize = initialOrthographicSize / zoomFactor;
        StartCoroutine(ZoomCoroutine(targetOrthographicSize, zoomDuration));
    }
    public void ZoomOut(float zoomDuration)
    {
        float targetOrthographicSize = initialOrthographicSize;
        StartCoroutine(ZoomCoroutine(targetOrthographicSize, zoomDuration));
    }

    public void ZoomOutBySize(float zoomDuration, float size)
    {
        StartCoroutine(ZoomCoroutine(size, zoomDuration));
    }

    public void ZoomInBySize(float zoomDuration, float size)
    {
        StartCoroutine(ZoomCoroutine(size, zoomDuration));
    }

    private IEnumerator ZoomCoroutine(float targetSize, float zoomDuration)
    {
        float currentSize = mainCamera.orthographicSize;
        float timer = 0.0f;

        // During zoomduration, lerp between the cameras orthographic size and the targetsize. 
        while (timer < zoomDuration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(currentSize, targetSize, timer / zoomDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        mainCamera.orthographicSize = targetSize;
    }





    // Camera states
    public void ChangeState(CameraStates newState)
    {
        currentState = newState;
    }

    public bool IsState(CameraStates newState)
    {
        return currentState == newState;
    }

    //GP
    private void OnGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }


}
