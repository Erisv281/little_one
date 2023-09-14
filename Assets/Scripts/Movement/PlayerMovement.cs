/*
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Thanks so much for checking this out and I hope you find it helpful! 
	If you have any further queries, questions or feedback feel free to reach out on my twitter or leave a comment on youtube :D

	Feel free to use this in your own games, and I'd love to see anything you make!
 */

using System.Collections;
using UnityEngine;

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

    //Jump
    private bool _isJumpCut;    // If having released jump key
    private bool _isJumpFalling;
    public float LastPressedJumpTime { get; private set; }

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;


    [Header("Double jumping")]
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    [Space(5)]
    [Header("Dashing")]
    [SerializeField] private float dashSpeed;
    private bool canDash;
    [SerializeField] private float dashTime;    // How long dashing lasts
    [SerializeField] private float dashCooldown;    // Time between dashes
    [SerializeField] private float dashButtonCooldown;  // Cooldown between next button tap
    private int dashButtonCount = 0;                    // Amount of button taps
    private float lastTapTime = -1f;
    private float latestButtonDirection = 0;                // Whether dashing RIGHT or LEFTs

    [Space(5)]
    [Header("Attacking")]
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject melee;
    private string faceVerticalDir;

    [Space(5)]
    [Header("Recoil")]
    [SerializeField] private Vector2 recoilLength; // Determine how far player recoils.
    [SerializeField] private Vector2 recoilSpeed;    // Determine speed of recoil
    private Vector2 stepsRecoil;

    [Space(5)]
    [Header("Health settings")]
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public float invinsibleCooldown;   // Amount of time player is invinsible
    [SerializeField] private FlashAnimation flashAnimation;


    [Space(5)]
    [Header("Checks")]
    // Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Others")]
    [SerializeField] private LayerMask _groundLayer;
    public SpriteRenderer SR { get; private set; }
    private Animator anim;
    public PlayerStateList pstate;
    public PlayerUnlocks unlocks;
    public Rigidbody2D RB { get; private set; }
    public PlayerData Data;
    [SerializeField] private float timeBeforeDeathScreen;   // Time before death screen pops up

    // HUD delegates
    public delegate void OnHealthChanged();
    [HideInInspector] public OnHealthChanged onHealthChangedCallback;




    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        GameStateManager.onGameStateChanged += onGameStateChanged;
    }

    private void Start()
    {
        Respawned();
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
        if (unlocks.hasUnlockedDash)
        {
            StartDash();
        }


    }

    private void FixedUpdate()
    {
        if (!pstate.isAlive || pstate.isEnteringCutscene || pstate.isDashing)
        {
            return;
        }

        // Handle recoil 
        CheckRecoil();

        //Handle Run
        if (pstate.isWallJumping)
        {
            Run(Data.wallJumpRunLerp);
        }
        else
        {
            Run(1);
        }


        // Handle Slide
        if (pstate.isSliding)
        {
            Slide();
        }

    }


    void TimerHandler()
    {
        // Decrease the timer variables 
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
    }

    void InputHandler()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0); // Turn player on horizontal direction
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }
        SetFirepointAngle();    // Set the firing angle on vertical direction

    }

    /// <summary>
    /// Rotate the Bow and Melee firepoint direction depending on which inputs are pressed. 
    /// Facing directions: 'U' = UP, '-' = Horizontal, 'D' = DOWN
    /// </summary>
    void SetFirepointAngle()
    {
        if (_moveInput.y == 0 && faceVerticalDir == "U")
        {
            bow.transform.Rotate(0f, 0f, -90);
            melee.transform.Rotate(0f, 0f, -90);
            faceVerticalDir = "-";
        }

        else if (_moveInput.y == 0 && faceVerticalDir == "D")
        {
            bow.transform.Rotate(0f, 0f, 90);
            melee.transform.Rotate(0f, 0f, 90);
            faceVerticalDir = "-";
        }
        else if (_moveInput.y < 0 && faceVerticalDir == "U")
        {
            bow.transform.Rotate(0f, 0f, -180);
            melee.transform.Rotate(0f, 0f, -180);
            faceVerticalDir = "D";
        }
        else if (_moveInput.y < 0 && faceVerticalDir == "-")
        {
            bow.transform.Rotate(0f, 0f, -90);
            melee.transform.Rotate(0f, 0f, -90);
            faceVerticalDir = "D";
        }
        else if (_moveInput.y > 0 && faceVerticalDir == "D")
        {
            bow.transform.Rotate(0f, 0f, 180);
            melee.transform.Rotate(0f, 0f, 180);
            faceVerticalDir = "U";
        }
        else if (_moveInput.y > 0 && faceVerticalDir == "-")
        {
            bow.transform.Rotate(0f, 0f, 90);
            melee.transform.Rotate(0f, 0f, 90);
            faceVerticalDir = "U";
        }

    }

    void CheckCollision()
    {
        if (!pstate.isJumping)
        {
            // Ground Check
            if (IsGrounded())
            {
                LastOnGroundTime = Data.coyoteTime; // Last on ground time to coyote time
                airJumpCounter = 0; // Reset the double jump counter. 
            }

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

    void CheckJump()
    {
        if (pstate.isDashing)
        {
            return;
        }

        // If jumping and moving downwards, we're falling if not touching walls. 
        if (pstate.isJumping && RB.velocity.y <= 0)
        {
            pstate.isJumping = false;
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

        // If in the air and neither jump nor walljump, cannot jumpcut
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
            SetJumpSettings();
            Jump();
        }
        // Wall jump
        else if (unlocks.hasUnlockedWallJump && CanWallJump() && LastPressedJumpTime > 0)
        {
            pstate.isWallJumping = true;
            pstate.isJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
            WallJump(_lastWallJumpDir);
        }
        // Double jump
        else if (unlocks.hasUnlockedDoubleJump && !IsGrounded() && CanDoubleJump() && Input.GetButtonDown("Jump") && !CanWallJump())
        {
            SetJumpSettings();
            airJumpCounter++;
            Jump();
        }



        anim.SetBool("Jumping", pstate.isJumping && RB.velocity.y > 0);
    }

    void CheckSlide()
    {
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            pstate.isSliding = true;
        }
        else
        {
            pstate.isSliding = false;
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
    /// Dashing coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator Dash()
    {
        canDash = false;
        pstate.isDashing = true;
        anim.SetTrigger("Dashing");
        RB.gravityScale = 0;    // Player dashing without falling
        RB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        RB.gravityScale = Data.gravityScale;
        pstate.isDashing = false;
        yield return new WaitForSeconds(dashCooldown);  // Player can dash again after cooldown 
        canDash = true;
    }

    void StartDash()
    {
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
    /// Return T if pressing the same direction twice within the interval [dashButtonCooldown]
    /// </summary>
    /// <returns></returns>
    bool IsTappingDash()
    {
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
    /// Handle player knockback
    /// </summary>
    public void KnockBack(Vector2 hitDirection, float hitForce)
    {
        RB.velocity = hitDirection * hitForce;   // Recoil set
    }


    /// <summary>
    /// Handle Recoiling player
    /// </summary>
    void CheckRecoil()
    {
        // Recoil Horizontally
        if (pstate.isRecoilingX)
        {
            if (pstate.isFacingRight)
            {
                // KB player to the left if facing right
                RB.velocity = new Vector2(-recoilSpeed.x, 0);
            }
            else
            {
                // Otherwise KB player to the right
                RB.velocity = new Vector2(recoilSpeed.x, 0);
            }
        }
        // Recoil vertically 
        if (pstate.isRecoilingY)
        {
            RB.gravityScale = 0;
            if (_moveInput.y < 0)
            {
                // If holding DOWN gets knockbacked Up, otherwise knockbacked down. 
                RB.velocity = new Vector2(RB.velocity.x, recoilSpeed.y);
            }
            else
            {
                RB.velocity = new Vector2(RB.velocity.x, -recoilSpeed.y);
            }
            airJumpCounter = 0;
        }
        else
        {
            HandleGravity();
        }

        // Stop recoil when recoiled enough length
        if (pstate.isRecoilingX && stepsRecoil.x < recoilLength.x)
        {
            stepsRecoil.x++;
        }
        else
        {
            stopRecoilX();
        }

        if (pstate.isRecoilingY && stepsRecoil.y < recoilLength.y)
        {
            stepsRecoil.y++;
        }
        else
        {
            stopRecoilY();
        }

        // Stop recoiling Y-axis when grounded. 
        if (IsGrounded())
        {
            stopRecoilY();
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

    void stopRecoilX()
    {
        stepsRecoil.x = 0;
        pstate.isRecoilingX = false;
    }
    void stopRecoilY()
    {
        stepsRecoil.y = 0;
        pstate.isRecoilingY = false;
    }

    /// <summary>
    /// Lastpressed set to jumpinput buffer time. 
    /// </summary>
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    /// <summary>
    /// Set _isJumpCut to true if player can jumpcut and can walljumpcut. 
    /// </summary>
    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
        {
            _isJumpCut = true;
        }
    }


    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }


    /// <summary>
    /// Lerping movementspeed in some direction, also handles movement acceleration. 
    /// </summary>
    /// <param name="lerpAmount"></param>
    private void Run(float lerpAmount)
    {
        // Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        // We can reduce control using Lerp() this smooths changes to direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        // Gets an acceleration value based on if we are accelerating (includes turning) 
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }

        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }

        // Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((pstate.isJumping || pstate.isWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }


        // We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
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
        anim.SetBool("Walking", Mathf.Abs(RB.velocity.x) >= 0.1 && IsGrounded());
    }

    private void Slide()
    {
        // Works the same as the Run but only in the y-axis
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        // So, we clamp the movement here to prevent any over corrections
        // The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        RB.AddForce(movement * Vector2.down);
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
    /// Jumping by adding Force
    /// </summary>
    private void Jump()
    {
        // Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        // We increase the force applied if we are falling
        // This means we'll always feel like we jump the same amount 
        float force = Data.jumpForce;
        if (RB.velocity.y < 0)
        {
            force -= RB.velocity.y;
        }

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Force is applied for the given direction. 
    /// </summary>
    /// <param name="dir"></param>
    private void WallJump(int dir)
    {
        // Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;


        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; // apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) // checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

        // Unlike in the run we want to use the Impulse mode.
        // The default mode will apply are force instantly ignoring mass
        RB.AddForce(force, ForceMode2D.Impulse);
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
    /// True, if not jumping, LastOnGroundTime > 0 and LastPressedJumpTime > 0
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && LastPressedJumpTime > 0 && !pstate.isJumping;
    }

    /// <summary>
    /// Setting jumping = T, walljumping = F, jumpcut = F, jumpfalling = F. 
    /// </summary>
    private void SetJumpSettings()
    {
        pstate.isJumping = true;
        pstate.isWallJumping = false;
        _isJumpCut = false;
        _isJumpFalling = false;
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
    /// If jumping and are moving upwards
    /// </summary>
    /// <returns></returns>
    private bool CanJumpCut()
    {
        return pstate.isJumping && RB.velocity.y > 0;
    }

    /// <summary>
    /// If already walljumping and is moving upwards
    /// </summary>
    /// <returns></returns>
    private bool CanWallJumpCut()
    {
        return pstate.isWallJumping && RB.velocity.y > 0;
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
    /// Cap max fall speed. 
    /// </summary>
    /// <param name="speed"></param>
    public void SetMaxFallSpeed(float speed)
    {
        RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -speed));
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }


    /// <summary>
    /// Checks if set box overlaps with ground
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
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
        if (!pstate.isAlive || damage < 0)
        {
            return;
        }

        health -= damage;
        changeHUD();

        // Player is dead
        if (health <= 0)
        {
            health = 0;
            StartCoroutine(Death());
        }

        // Player is not already invinsible
        if (!pstate.isInvinsible)
        {
            KnockBack(hitDirection, hitForce);
            StartCoroutine(StartInvinsibleAnimation());
        }
    }

    IEnumerator Death()
    {
        pstate.isAlive = false;
        GameManager.instance.switchGameState(); // Pause the gameplay
        anim.SetTrigger("Death");            // Player death animation
        yield return new WaitForSeconds(timeBeforeDeathScreen);
        StartCoroutine(AnimationManager.instance.activateDeathScreen()); // Show death screen

    }

    public void Respawned()
    {
        pstate.isAlive = true;
        Input.ResetInputAxes();
        health = maxHealth;     // Change to the health in the save system!
        faceVerticalDir = "-";
        SetGravityScale(Data.gravityScale);
        canDash = true;
        changeHUD();
        anim.Play("player_idle");

    }

    IEnumerator StartInvinsibleAnimation()
    {
        pstate.isInvinsible = true;
        anim.SetTrigger("TakeDamage");
        flashAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(invinsibleCooldown);
        flashAnimation.destroyFlash();
        flashAnimation.gameObject.SetActive(false);
        pstate.isInvinsible = false;
        RB.velocity = Vector2.zero;    // Stop knockback

        // Reset attack timers
        if (unlocks.hasUnlockedMelee)
        {
            melee.GetComponent<Melee>().resetAttackTimer();
        }

        if (unlocks.hasUnlockedBow)
        {
            bow.GetComponent<Bow>().resetAttackTimer();
        }

    }



    public IEnumerator WalkIntoNewScene(Vector2 exitDir, float delay)
    {
        yield return new WaitForSeconds(delay);
        pstate.isEnteringCutscene = false;

    }

    public void changeHUD()
    {
        // Notify HUD
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
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

}


