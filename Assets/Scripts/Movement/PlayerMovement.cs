/*
	Inspiration by @DawnosaurDev at youtube.com/c/DawnosaurStudios
 */

using System.Collections;
using UnityEngine;


/// <summary>
/// This class handles all movement of the player aswell as the player data. 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //HOW TO: to add the scriptable object, right-click in the project window -> create -> Player Data
    //Next, drag it into the slot in playerMovement on your player

    private Vector2 _moveInput;

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    // Jump
    private bool _isJumpCut;    // If having released jump key
    public bool _isJumpFalling;
    public float LastPressedJumpTime { get; private set; }

    // Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;


    [Header("Double jumping")]
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    [Space(5)]
    [Header("Dashing")]
    [SerializeField] private float dashSpeed;
    private bool canDash;
    [SerializeField] private float dashDuration;    // How long dashing lasts
    [SerializeField] private float dashCooldown;    // Cooldown between dashes
    [SerializeField] private float dashButtonCooldown;  // Cooldown between next button tap
    private int dashButtonCount = 0;                    // Amount of button taps
    private float lastTapTime = -1f;
    private float latestButtonDirection = 0;                // Whether dashing RIGHT or LEFTs

    [Space(5)]
    [Header("Attacking")]
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject melee;
    private string faceVerticalDir; // Which direction the player faces, U, D or -

    [Space(5)]
    [Header("Recoil")]
    [SerializeField] private Vector2 recoilLength; // Determine how far player recoils.
    [SerializeField] private Vector2 recoilSpeed;    // Determine speed of recoil
    private Vector2 stepsRecoil;
    private float KBTimer;
    private Vector2 knockbackDirection; // The direction player is hit from

    [Space(5)]
    [Header("Health settings")]
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] private int initialMaxHealth;  // Used for resetting the game
    [SerializeField] public float invinsibleCooldown;   // Amount of time player is invinsible
    [SerializeField] private FlashAnimation flashAnimation;


    [Space(5)]
    [Header("Checks")]
    // Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    public Transform _groundCheckPoint;
    public Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Space(5)]
    [Header("Particles")]
    [SerializeField] private GameObject DBLParticles;
    [SerializeField] private GameObject DashParticles;
    [SerializeField] private float particleDuration = 0.5f;

    [Space(5)]
    [Header("Others")]
    public Transform originalParentTransform;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask switchLayer;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask deathLayer;
    public SpriteRenderer SR { get; private set; }
    public Animator anim;
    public PlayerStateList pstate;
    public PlayerUnlocks unlocks;
    public Rigidbody2D RB { get; private set; }
    public PlayerData Data;
    [SerializeField] private float timeBeforeDeathScreen;   // Time before death screen pops up

    // HUD delegates
    public delegate void OnHealthChanged();
    [HideInInspector] public OnHealthChanged onHealthChangedCallback;

    public delegate void OnPlayerDeath();
    [HideInInspector] public OnPlayerDeath onPlayerDeathCallback;


    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalParentTransform = transform.parent;
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    private void Start()
    {
        pstate.isAlive = true;
        Input.ResetInputAxes();
        health = maxHealth;
        faceVerticalDir = "-";
        CheckDirectionToFace(pstate.isFacingRight);
        SetGravityScale(Data.gravityScale);
        canDash = true;
        changeHUD();
        anim.Play("player_idle");
    }

    private void Update()
    {
        if (!pstate.isAlive || pstate.isEnteringCutscene)
        {
            return;
        }
        TimerHandler();
        InputHandler();
        CheckCollision();
        CheckJump();
        CheckSlide();
        HandleGravity();
        KnockBackHandler();
        CheckDash();

        pstate.isMoving = RB.velocity.magnitude > 0.1f; // Player is moving if above this threshold
    }

    private void FixedUpdate()
    {
        if (!pstate.isAlive || pstate.isEnteringCutscene || pstate.isDashing)
        {
            return;
        }

        CheckRecoil();
        RunHandler();
        Slide();

    }

    private void OnCollisionStay2D(Collision2D other)
    {

        if (other.collider.CompareTag("platform"))
        {
            if (!pstate.isMoving)
            {
                // Stick player onto platform
                transform.parent = other.gameObject.transform.parent;
            }
            else
            {
                transform.parent = null;
            }

        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("platform"))
        {
            transform.parent = null;
        }

    }


    private void TimerHandler()
    {
        // Decrease the timer variables 
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
    }

    private void InputHandler()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        // Handle horizontal and vertical movement
        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }

        // Set the firing angle on vertical direction
        SetFirepointAngle();

        // Animation
        anim.SetFloat("Horizontal", _moveInput.x);
        anim.SetFloat("Vertical", _moveInput.y);

    }

    /// <summary>
    /// Rotate the Bow and Melee firepoint direction depending on which inputs are pressed. 
    /// Facing directions: 'U' = UP, '-' = Horizontal, 'D' = DOWN
    /// </summary>
    private void SetFirepointAngle()
    {
        if (_moveInput.y == 0 && faceVerticalDir == "U")
        {
            bow.transform.Rotate(0f, 0f, -90);
            faceVerticalDir = "-";
        }
        else if (_moveInput.y == 0 && faceVerticalDir == "D")
        {
            bow.transform.Rotate(0f, 0f, 90);
            faceVerticalDir = "-";
        }
        else if (_moveInput.y < 0 && faceVerticalDir == "U")
        {
            bow.transform.Rotate(0f, 0f, -180);
            faceVerticalDir = "D";
        }
        else if (_moveInput.y < 0 && faceVerticalDir == "-")
        {
            bow.transform.Rotate(0f, 0f, -90);
            faceVerticalDir = "D";
        }
        else if (_moveInput.y > 0 && faceVerticalDir == "D")
        {
            bow.transform.Rotate(0f, 0f, 180);
            faceVerticalDir = "U";
        }
        else if (_moveInput.y > 0 && faceVerticalDir == "-")
        {
            bow.transform.Rotate(0f, 0f, 90);
            faceVerticalDir = "U";
        }

    }

    private void CheckCollision()
    {
        if (!pstate.isJumping)
        {
            // Ground collision Check
            if (IsGrounded())
            {
                LastOnGroundTime = Data.coyoteTime; // Last on ground time to coyote time
                airJumpCounter = 0; // Reset the double jump counter. 
            }

            // Wall collision check
            if (!pstate.isWallJumping)
            {
                // Right Wall Check
                if (IsCollidingWallRight())
                {
                    LastOnWallRightTime = Data.coyoteTime;  // Set coyote time
                }

                // Left Wall Check
                if (IsCollidingWallLeft())
                {
                    LastOnWallLeftTime = Data.coyoteTime;   // Set coyote time
                }
            }

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
    }

    private void CheckJump()
    {
        if (pstate.isDashing)
        {
            return;
        }


        // If jumping and moving downwards, we're falling if not touching walls. 
        // Threshold of 0.01, since sometimes the velocity.y is registering very small positive numbers. 
        if (pstate.isJumping && RB.velocity.y <= 0.01)
        {
            pstate.isJumping = false;
            // If player is not walljumping, he is then falling
            if (!pstate.isWallJumping)
            {
                _isJumpFalling = true;
            }
        }

        // If walljumping and walljumptime has exceeded, we're not walljumping anymore
        if (pstate.isWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            pstate.isWallJumping = false;
        }

        // If in the air and neither jump nor walljump, cannot jumpcut (release space)
        if (LastOnGroundTime > 0 && !pstate.isJumping && !pstate.isWallJumping)
        {
            _isJumpCut = false;

            if (!pstate.isJumping)
            {
                _isJumpFalling = false;
            }
        }

        // Jump
        if (CanJump())
        {
            Jump();
        }

        // Wall jump
        else if (unlocks.hasUnlockedWallJump && CanWallJump() && LastPressedJumpTime > 0)
        {
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
            WallJump(_lastWallJumpDir);
        }

        // Double jump
        else if (unlocks.hasUnlockedDoubleJump && !IsGrounded() && CanDoubleJump() && Input.GetButtonDown("Jump"))
        {
            DoubleJump();
        }

        // Animations for jumping and falling
        anim.SetBool("Jumping", (pstate.isJumping || pstate.isWallJumping) && RB.velocity.y > 0.01);


        if (!pstate.isSliding)
        {
            anim.SetBool("Falling", RB.velocity.y < -0.1);
        }
        else
        {
            anim.SetBool("Falling", false);
        }

    }

    /// <summary>
    /// True, if not jumping, LastOnGroundTime > 0 and LastPressedJumpTime > 0
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && LastPressedJumpTime > 0 && !pstate.isJumping;
    }

    /// <summary>
    /// True if has more jumps left. 
    /// </summary>
    /// <returns></returns>
    private bool CanDoubleJump()
    {
        return airJumpCounter < maxAirJumps;
    }


    /// <summary>
    /// Return T if can walljump...
    /// </summary>
    /// <returns></returns>
    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!pstate.isWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }


    /// <summary>
    /// Jumping by adding Force
    /// </summary>
    private void Jump()
    {
        // Set jumping flags
        pstate.isJumping = true;
        pstate.isWallJumping = false;
        _isJumpCut = false;
        _isJumpFalling = false;

        // Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        // We increase the force applied if we are falling
        // This means we'll always feel like we jump the same amount 
        // Also for double jumping we decrease the applied force
        float force = Data.jumpForce;
        if (RB.velocity.y != 0)
        {
            force -= RB.velocity.y;
        }

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // Audio 
        AudioManager.instance.Play("Jump");
    }



    /// <summary>
    /// Execute player double jump and create double jump particles
    /// </summary>
    private void DoubleJump()
    {
        // This check fixes the bug regarding we have DBL jump ability but not wall ability. 
        if (CanWallJump() && !unlocks.hasUnlockedWallJump)
        {
            return;
        }
        else if (!CanWallJump())
        {
            airJumpCounter++;
            Jump();

            // Double jump particles
            GameObject particles = Instantiate(DBLParticles, transform.position, Quaternion.identity);
            Destroy(particles, particleDuration);
        }
    }


    /// <summary>
    /// Force is applied for the given direction. 
    /// </summary>
    /// <param name="dir"></param>
    private void WallJump(int dir)
    {
        // Setting boolean values
        pstate.isWallJumping = true;
        pstate.isJumping = false;
        _isJumpCut = false;
        _isJumpFalling = false;

        // Ensures we can't call Wall Jump multiple times from one press
        _wallJumpStartTime = Time.time;
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        // Apply force in opposite direction of wall
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= RB.velocity.x;
        }


        // Subtract the velocity.y. This ensures the player always reaches our desired jump force or greater
        force.y -= RB.velocity.y;

        // Unlike in the run we want to use the Impulse mode.
        // The default mode will apply are force instantly ignoring mass
        RB.AddForce(force, ForceMode2D.Impulse);

        // Audio
        AudioManager.instance.Play("Jump");
    }

    private void CheckSlide()
    {
        // Sliding if: 
        // On either left/right wall and is holding down key for left/right
        // Player is not jumping nor walljumping
        // LastWalltime > 0
        // LastOngroundtime <= 0
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            pstate.isSliding = true;
        }
        else
        {
            pstate.isSliding = false;
        }

        // Animation
        anim.SetBool("Sliding", pstate.isSliding);
    }

    /// <summary>
    /// Return T if not jump/wall jump and LastOnWallTime > 0, and LastOnGroundTime <= 0
    /// </summary>
    /// <returns></returns>
    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !pstate.isJumping && !pstate.isWallJumping && LastOnGroundTime <= 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }


    /// <summary>
    /// Handle gravity by some notices:
    /// 1. When falling
    /// 2. When holding DOWN
    /// 3. When jump button released
    /// 4. Jumping under some threshold
    /// 5. Falling
    /// 6. Default
    /// </summary>
    void HandleGravity()
    {
        // Higher gravity if we've released the jump input or are falling
        if (pstate.isSliding)
        {
            SetGravityScale(0);
        }
        // Much higher gravity if holding down DOWN
        else if (RB.velocity.y <= 0 && _moveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
            SetMaxFallSpeed(Data.maxFastFallSpeed);
        }
        // Higher gravity if jump button released
        else if (_isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            SetMaxFallSpeed(Data.maxFallSpeed);
        }
        else if ((pstate.isJumping || pstate.isWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        // Higher gravity if falling
        else if (RB.velocity.y <= 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            SetMaxFallSpeed(Data.maxFallSpeed);
        }
        else
        {
            // Default gravity if standing on a platform or moving upwards
            SetGravityScale(Data.gravityScale);
        }
    }

    /// <summary>
    /// Gradually reduce the knockback from which direction the knockback came from. 
    /// </summary>
    private void KnockBackHandler()
    {
        if (pstate.isInvinsible)
        {
            if (Time.time - KBTimer < invinsibleCooldown)
            {
                RB.AddForce(-knockbackDirection * Time.deltaTime);
            }
        }
    }



    /// <summary>
    /// Dash checker. Allow dashing if tapping dash buttons and available to dash if not already dashing.  
    /// </summary>
    void CheckDash()
    {
        if (!unlocks.hasUnlockedDash)
        {
            return;
        }


        // Pressing dash button and if we can dash
        if (IsTappingDash() && canDash && !pstate.isDashing)
        {
            StartCoroutine(Dash());
            pstate.isDashing = true;
        }
        if (IsGrounded())
        {
            pstate.isDashing = false;
        }

    }

    /// <summary>
    /// Dashing coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator Dash()
    {
        // Animation and sound
        anim.SetTrigger("Dashing");
        AudioManager.instance.Play("Dash");

        // Set flags
        canDash = false;
        pstate.isDashing = true;

        // Set gravity and velocity
        RB.gravityScale = 0;    // Player dashing without falling
        RB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * transform.localScale.x * dashSpeed, 0);

        // Create Dash particles
        GameObject particles = Instantiate(DashParticles, transform.position + Vector3.down, Quaternion.identity);
        Destroy(particles, particleDuration);

        // Wait some time
        yield return new WaitForSeconds(dashDuration);

        // Dashing done
        RB.gravityScale = Data.gravityScale;
        pstate.isDashing = false;

        // Wait Cooldown before player can dash again
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    /// <summary>
    /// Return T if pressing the same direction twice within the interval [dashButtonCooldown]
    /// </summary>
    /// <returns></returns>
    bool IsTappingDash()
    {
        // Get direction
        float thisButtonDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Horizontal"))
        {
            if (Time.time - lastTapTime <= dashButtonCooldown)
            {
                dashButtonCount++;
                lastTapTime = Time.time;

                if (dashButtonCount >= 2 && thisButtonDirection == latestButtonDirection)
                {
                    // Press dashbutton twice in the same direction
                    return true;
                }
                else
                {
                    latestButtonDirection = thisButtonDirection;
                }
            }
            else
            {
                dashButtonCount = 1;
                lastTapTime = Time.time;
                latestButtonDirection = thisButtonDirection;
            }
        }
        return false;
    }



    /// <summary>
    /// Handle Recoiling player that is when player is using weapons, he get recoiled. 
    /// </summary>
    void CheckRecoil()
    {
        float recoilDirection;

        // Recoil Horizontally
        if (pstate.isRecoilingX)
        {
            recoilDirection = pstate.isFacingRight ? -recoilSpeed.x : recoilSpeed.x;
            RB.velocity = new Vector2(recoilDirection, 0);
        }

        // Recoil vertically 
        if (pstate.isRecoilingY)
        {
            // If holding DOWN gets knockbacked Up, otherwise knockbacked down. 
            RB.gravityScale = 0;
            recoilDirection = _moveInput.y < 0 ? recoilSpeed.y : -recoilSpeed.y;
            RB.velocity = new Vector2(RB.velocity.x, recoilDirection);

            // Reset double jump
            airJumpCounter = 0;
        }
        else
        {
            // Otherwise check gravity
            HandleGravity();
        }

        // Step recoiling in X,Y axis
        StepRecoilX();
        StepRecoilY();

        // Stop recoiling Y-axis when grounded. 
        if (IsGrounded())
        {
            StopRecoilY();
        }


    }

    public void SetRecoilingDirection()
    {
        // When hitting down/up: RecoilingY with yspeed. Hitting Horizontal recoilX with xspeed. 

        if (_moveInput.x != 0)
        {
            pstate.isRecoilingX = true;
        }
        else if (_moveInput.y != 0)
        {
            pstate.isRecoilingY = true;
        }
    }

    private void StepRecoilY()
    {
        // Recoil within some length
        if (pstate.isRecoilingY && stepsRecoil.y < recoilLength.y)
        {
            stepsRecoil.y++;
        }
        // Stop recoiling Y
        else
        {
            StopRecoilY();
        }
    }

    private void StepRecoilX()
    {
        // Recoil within some length
        if (pstate.isRecoilingX && stepsRecoil.x < recoilLength.x)
        {
            stepsRecoil.x++;
        }
        // Stop recoiling X
        else
        {
            StopRecoilX();
        }
    }

    private void StopRecoilX()
    {
        stepsRecoil.x = 0;
        pstate.isRecoilingX = false;
    }
    private void StopRecoilY()
    {
        stepsRecoil.y = 0;
        pstate.isRecoilingY = false;
    }


    /// <summary>
    /// Different running lerp speed depending on walljumping or not. 
    /// </summary>
    private void RunHandler()
    {
        float lerpAmount = pstate.isWallJumping ? Data.wallJumpRunLerp : 1;
        Run(lerpAmount);

    }


    /// <summary>
    /// Lerping movementspeed in some direction, also handles movement acceleration. 
    /// </summary>
    /// <param name="lerpAmount"></param>
    private void Run(float lerpAmount)
    {
        // Speed we want to move
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;

        // Reduce control using Lerp() which smooths changes to direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        // Handle acceleration. As well as applying a multiplier if we're air borne.
        float accelRate;
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }

        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }

        // Handle jump to apex
        // Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((pstate.isJumping || pstate.isWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        // Don't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            // Prevent any deceleration from happening, or in other words conserve are current momentum
            // You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }

        // Difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;

        // Force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        // Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        // Animation
        anim.SetBool("Walking", Mathf.Abs(RB.velocity.x) >= 0.1 && IsGrounded());
    }


    private void Slide()
    {
        if (!pstate.isSliding)
        {
            return;
        }

        // Calculate the speeddifference between the target slidespeed and the velocity of RB
        // Dot product is used to make sure the player slides the correct direction.
        float speedDif = Data.slideSpeed - Vector2.Dot(RB.velocity, Vector2.down);
        float movement = speedDif * Data.slideAccel;

        // So, we clamp the movement here to prevent any over corrections
        // The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        RB.AddForce(movement * Vector2.down);
    }


    /// <summary>
    /// Lastpressed set to jumpinput buffer time. 
    /// </summary>
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    /// <summary>
    /// Set _isJumpCut to true if player can jumpcut or can walljumpcut. Meaning we can release space
    /// </summary>
    public void OnJumpUpInput()
    {
        if (CanJumpCut(pstate.isJumping) || CanJumpCut(pstate.isWallJumping))
        {
            _isJumpCut = true;
        }
    }

    private bool CanJumpCut(bool state)
    {
        return state && RB.velocity.y > 0;
    }


    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    /// <summary>
    /// Cap max fall speed. 
    /// </summary>
    /// <param name="speed"></param>
    public void SetMaxFallSpeed(float speed)
    {
        RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -speed));
    }


    /// <summary>
    /// If isMovingRIght is facing left then the player turns. 
    /// </summary>
    /// <param name="isMovingRight"></param>
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != pstate.isFacingRight)
        {
            Turn();
        }
    }

    /// <summary>
    /// Turn player 180 degrees in the other direction
    /// </summary>
    private void Turn()
    {
        transform.Rotate(0f, 180, 0f);
        pstate.isFacingRight = !pstate.isFacingRight;
    }



    /// <summary>
    /// Checks if set box overlaps with ground, switches, items or deaths
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)
        || Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, switchLayer)
        || Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, itemLayer)
        || Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, deathLayer))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Return true if colliding with right wall and facing it. Or backplayer hits it facing left. 
    /// </summary>
    /// <returns></returns>
    public bool IsCollidingWallRight()
    {
        if ((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && pstate.isFacingRight)
            || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !pstate.isFacingRight))
        {
            return true;
        }
        return false;

    }
    /// <summary>
    /// Return true if colliding with left wall and facing it. Or backplayer hits it facing right. 
    /// </summary>
    /// <returns></returns>
    public bool IsCollidingWallLeft()
    {
        if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !pstate.isFacingRight)
            || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && pstate.isFacingRight)))
        {
            return true;
        }
        return false;

    }


    /// <summary>
    /// The player decreases health, killing him if sufficient damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, Vector2 hitDirection, float hitForce)
    {
        if (!pstate.isAlive || damage < 0 || pstate.isInvinsible)
        {
            return;
        }

        health -= damage;
        changeHUD();

        // Player is dead
        if (health <= 0)
        {
            StartCoroutine(Death());
        }

        // Player is not already invinsible
        if (!pstate.isInvinsible && pstate.isAlive)
        {
            KnockBack(hitDirection, hitForce);
            StartCoroutine(StartInvinsibleAnimation());
        }
    }




    IEnumerator Death()
    {
        health = 0;
        pstate.isAlive = false;

        // Set player parent if on some moving platform
        transform.parent = originalParentTransform;

        // Pasue playermovement and gameplay
        StopMovement();
        GameManager.instance.switchGameState();

        // Animation and sounds
        anim.SetTrigger("Death");
        AudioManager.instance.Play("Player_Death");

        // Wait some time before death screen
        yield return new WaitForSeconds(timeBeforeDeathScreen);

        // Show death screen
        StartCoroutine(AnimationManager.instance.ActivateDeathScreen());
    }

    /// <summary>
    /// Call this method, ONLY when player shall respawn. 
    /// </summary>
    public void Respawned()
    {
        Input.ResetInputAxes();
        GameManager.instance.SetPlayerHealth();

        SetFirepointAngle();
        CheckDirectionToFace(pstate.isFacingRight);
        SetGravityScale(Data.gravityScale);
        canDash = true;
        changeHUD();
        anim.Play("player_idle");
        pstate.isAlive = true;

        // Notifiy listeners that player has died and is respawning
        PlayerDeathCallback();

        // Place player on respawn point
        transform.position = GameManager.instance.respawnPoint;
    }

    /// <summary>
    /// Handle player knockback
    /// </summary>
    public void KnockBack(Vector2 hitDirection, float hitForce)
    {
        knockbackDirection = hitDirection;
        RB.velocity = hitDirection * hitForce;   // Setting KB here
    }

    IEnumerator StartInvinsibleAnimation()
    {
        // Animation and sounds
        anim.SetTrigger("TakeDamage");
        AudioManager.instance.Play("Player_Hurt");

        // Activate flash object and wait
        pstate.isInvinsible = true;
        KBTimer = Time.time;
        flashAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(invinsibleCooldown);

        // Flash is done, reset
        flashAnimation.destroyFlash();
        flashAnimation.gameObject.SetActive(false);
        pstate.isInvinsible = false;
        KBTimer = 0f;

        // Reset attack timers
        if (unlocks.hasUnlockedMelee && melee.GetComponent<Melee>() != null)
        {
            melee.GetComponent<Melee>().resetAttackTimer();
        }

        if (unlocks.hasUnlockedBow && melee.GetComponent<Melee>() != null)
        {
            bow.GetComponent<Bow>().resetAttackTimer();
        }

    }

    public void Heal(int healAmount)
    {
        if (health == maxHealth || healAmount <= 0)
        {
            return;
        }

        int diff = maxHealth - health;
        // If healing more than our diff, then set health to maxhealth. Otherwise pump the player health up. 

        if (healAmount >= diff)
        {
            health = maxHealth;
        }
        else
        {
            health += healAmount;
        }
        changeHUD();
    }


    /// <summary>
    /// Call this when player changes scene
    /// </summary>
    /// <param name="exitDir"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator WalkIntoNewScene(Vector2 exitDir, float delay)
    {
        yield return new WaitForSeconds(delay);
        pstate.isEnteringCutscene = false;

    }


    /// <summary>
    /// Stops the movement of the player
    /// </summary>
    public void StopMovement()
    {
        RB.velocity = Vector2.zero;
        RB.Sleep();
    }


    /// <summary>
    /// ONLY call this method when collecting stuff like shrine and abilities. Resets all player states. 
    /// </summary>
    public void ResetAnimation()
    {
        anim.SetBool("Jumping", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Dashing", false);
        anim.SetBool("Falling", false);
        anim.SetBool("Sliding", false);
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("Vertical", 0);
    }

    /// <summary>
    /// Reset player collect data
    /// </summary>
    public void ResetGame()
    {
        unlocks.hasUnlockedBow = false;
        unlocks.hasUnlockedMelee = false;
        unlocks.hasUnlockedWallJump = false;
        unlocks.hasUnlockedDoubleJump = false;
        unlocks.hasUnlockedDash = false;

        maxHealth = initialMaxHealth;

    }

    // Callbacks

    public void changeHUD()
    {
        // Notify HUD
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
        }
    }

    public void PlayerDeathCallback()
    {
        // Notify subscribers that the player has died. 
        if (onPlayerDeathCallback != null)
        {
            onPlayerDeathCallback.Invoke();
        }
    }

    // GameState
    protected void onGameStateChanged(GameState gameState)
    {
        enabled = gameState == GameState.Gameplay;
    }

    protected void OnDestroy()
    {
        GameStateManager.onGameStateChanged -= onGameStateChanged;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }


}


