using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles player movement
/// </summary>
public class PlayerMovement : Movement
{
    //Assingables
    public Transform playerCamPosition;
    public Camera playerCam;
    public Transform orientation;
    public PlayerShooting playerShooting;
    public PlayerUIScript playerUIScript;
    private InputMaster inputMaster;
    public Rigidbody rb;

    // Ground Check
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    //Rotation and look
    private float xRotation;
    private float desiredX;
    private float normalFOV = 60;
    private float adsFOV = 40;
    public float sensitivity = 2000f;
    private float normalSensitivity = 2000f;
    private float adsSensitivity = 500f;
    // Default value for sens multipliers are 1 
    public float sensMultiplier { get; set; } = 1f;
    public float adsSensMultiplier { get; set; } = 1f;

    //Movement
    private readonly int moveSpeed = 4500;
    private readonly int crouchMoveSpeed = 3000;
    private readonly int maxBaseSpeed = 20;
    private readonly int maxCrouchSpeed = 10;
    private readonly float counterMovement = 0.175f;
    private readonly float threshold = 0.01f;
    private bool shouldSlide;
    public LayerMask whatIsGround;
    public bool grounded;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 1, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    public float crouchGunDegree = 50;

    public bool isSliding = false;

    //Jumping
    public float jumpForce = 550f;
    private readonly int maxJumps = 2;
    private int jumpsAvaliable = 2;

    //Input
    private float x, y;
    public bool jumping = false, crouching = false;

    //Wallrunning
    public LayerMask whatIsWall;
    public Transform wallCheck;
    private readonly float wallDistance = 1f;
    public float wallrunForce, maxWallrunTime;
    public float maxWallRunCameraTilt, wallRunCameraTilt;
    private float currentHandPosition = .15f;
    private float wallRunSpeed = 100f, maxWallRunSpeed = 40f;
    public float timeOnWall, maxtimeOnWall = 3f;
    private bool isWallRight, isWallLeft, isWallForward, isWallBackward;
    public Transform gunPosition;

    public bool IsOnWall { get; private set; }

    void Awake()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);
        sensMultiplier = PlayerPrefs.GetFloat("Sens", 1f);
        adsSensMultiplier = PlayerPrefs.GetFloat("ADSSens", 1f);
    }

    public void RebindContols()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);
    }

    public void OnEnable()
    {
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        inputMaster.Disable();
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        MyInput();
        CheckForWall();
        Look();
    }

    /// <summary>
    /// Find user input
    /// </summary>
    private void MyInput()
    {
        x = inputMaster.Player.Movement.ReadValue<Vector2>().x;
        y = inputMaster.Player.Movement.ReadValue<Vector2>().y;

        // increase timer if player is motionless on wall. Otherwise reset timer
        if (IsOnWall)
            timeOnWall += Time.deltaTime;
        else
            timeOnWall = 0;

        // Jump if jump is pressed and num of jumps are more than 0
        if (inputMaster.Player.Jump.triggered && jumpsAvaliable > 0)
        {
            if (IsOnWall)
                // Send in false because player is not motionless
                WallJump(false);
            else
                Jump();
        }

        // Crouching
        // crouch if not on wall and input from left shift or mouse 4
        if (!crouching)
            if (inputMaster.Player.Crouch.ReadValue<float>() > 0 && !IsOnWall! && !playerShooting.IsGrappling)
                StartCrouch();
        if (crouching)
            if (inputMaster.Player.Crouch.ReadValue<float>() == 0 && !IsOnWall && !playerShooting.IsGrappling)
                StopCrouch();

        // AIM DOWN SIGHTS
        if (inputMaster.Player.ADS.ReadValue<float>() != 0)
        {
            if (playerShooting.IsGrappling)
                sensitivity = normalSensitivity;
            else if (sensitivity != adsSensitivity)
                sensitivity = adsSensitivity;
        }
        else if (sensitivity != normalSensitivity)
            sensitivity = normalSensitivity;
    }

    /// <summary>
    /// Shrinks player and conditionally decides if player should slide
    /// </summary>
    private void StartCrouch()
    {
        crouching = true;
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
            shouldSlide = true;

        // Prevents guns and other such objects from shrinking
        foreach(Transform child in gameObject.transform)
            child.localScale = playerScale;
        playerUIScript.ChangeToCrouch();
    }

    /// <summary>
    /// enlarges players to original size
    /// </summary>
    public void StopCrouch()
    {
        crouching = false;
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        // Prevents guns and other such objects from expanding larger than intended
        foreach (Transform child in gameObject.transform)
            child.localScale = crouchScale;
        playerUIScript.ChangeToStand();
    }

    /// <summary>
    /// Player movement (called by FixedUpdate)
    /// </summary>
    private void Movement()
    {
        if (jumping) return;
        // Reset jumps if player is on ground
        if (grounded) jumpsAvaliable = maxJumps;

        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook(orientation, rb);
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        if(!IsOnWall)
            CounterMovement(x, y, mag);

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (crouching)
        {
            if (x > 0 && xMag > maxCrouchSpeed) x = 0;
            if (x < 0 && xMag < -maxCrouchSpeed) x = 0;
            if (y > 0 && yMag > maxCrouchSpeed) y = 0;
            if (y < 0 && yMag < -maxCrouchSpeed) y = 0;
        }
        else if (IsOnWall)
        {
            if (x > 0 && xMag > maxWallRunSpeed) x = 0;
            if (x < 0 && xMag < -maxWallRunSpeed) x = 0;
            if (y > 0 && yMag > maxWallRunSpeed) y = 0;
            if (y < 0 && yMag < -maxWallRunSpeed) y = 0;
        }
        else
        {
            if (x > 0 && xMag > maxBaseSpeed) x = 0;
            if (x < 0 && xMag < -maxBaseSpeed) x = 0;
            if (y > 0 && yMag > maxBaseSpeed) y = 0;
            if (y < 0 && yMag < -maxBaseSpeed) y = 0;
        }

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;

            // Allows for bunny hopping
            if (rb.velocity.magnitude > 0.5f && crouching)
                shouldSlide = true;
        }

        // Slide as soon as player lands 
        if (grounded && shouldSlide)
        {
            Slide();
            return;
        }

        // Prevents player from moving while sliding
        if (grounded && crouching) multiplierV = 0f;

        // Apply forces to move player
        // crouch walking
        if (crouching && !isSliding)
        {
            rb.AddForce(orientation.transform.forward * y * crouchMoveSpeed * Time.deltaTime);
            rb.AddForce(orientation.transform.right * x * crouchMoveSpeed * Time.deltaTime );
        }
        // Wall Running
        else if (IsOnWall)
        {
            rb.AddForce(orientation.transform.right * x * wallRunSpeed * Time.deltaTime, ForceMode.Impulse);
            rb.AddForce(orientation.transform.forward * y * wallRunSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        // Normal movement on ground
        else if (!playerShooting.IsGrappling)
        {
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }
    }

    /// <summary>
    /// Player jumps from a ground object
    /// </summary>
    private void Jump()
    {
        if (jumpsAvaliable < 0) return;
        if (IsOnWall)
        {
            WallJump(false);
            return;
        }

        jumpsAvaliable -= 2;

        //Add jump forces
        rb.AddForce(Vector3.up * jumpForce * 1.5f);
        rb.AddForce(Vector3.up * jumpForce * 0.5f);

        //If jumping while falling, reset y velocity.
        Vector3 vel = rb.velocity;
        if (rb.velocity.y < 0.5f)
            rb.velocity = new Vector3(vel.x, 0, vel.z);
        else if (rb.velocity.y > 0)
            rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
    }

    /// <summary>
    /// Adds forces for player to slide and modifies booleans relating to sliding
    /// Dependencies: TurnOffisSliding
    /// </summary>
    private void Slide()
    {
        shouldSlide = false;
        isSliding = true;
        rb.AddForce(orientation.transform.forward * slideForce);
        StartCoroutine(TurnOffIsSliding());
    }

    /// <summary>
    /// recursively calls itself until player velocity is less than .1 in which case
    /// isSliding is set equal to false
    /// </summary>
    private IEnumerator TurnOffIsSliding()
    {
        yield return new WaitForSeconds(.1f);
        if(Mathf.Abs(rb.velocity.magnitude) > .1f)
            StartCoroutine(TurnOffIsSliding());
        else
            isSliding = false;
    }

    /// <summary>
    /// Rotates player with mouse movements and rotates player on z axis when wall running
    /// </summary>
    private void Look()
    {
        float mouseX = Camera.main.ScreenToViewportPoint(inputMaster.Player.MouseLook.ReadValue<Vector2>()).x 
                       * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Camera.main.ScreenToViewportPoint(inputMaster.Player.MouseLook.ReadValue<Vector2>()).y 
                       * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCamPosition.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        // Perform the rotations
        playerCamPosition.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCameraTilt);
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);

        // ADS zoom in and out
        if (sensitivity == adsSensitivity && playerCam.fieldOfView > adsFOV)
            playerCam.fieldOfView -= 1f;
        if (sensitivity == normalSensitivity && playerCam.fieldOfView < normalFOV)
            playerCam.fieldOfView += 1f;

        // Tilts camera in .5 second
        // Prevents camera from spinning on the y-axis at a very fast angular velocity
        // maxWallRunCameraTilt - wallRunCameraTilt < -.1f, wallRunCameraTilt < maxWallRunCameraTilt
        if (wallRunCameraTilt < maxWallRunCameraTilt && IsOnWall && isWallRight)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        if (wallRunCameraTilt > -maxWallRunCameraTilt && IsOnWall && isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;

        // Tilts camera back again
        if(wallRunCameraTilt > .2f && !isWallRight && !isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        else if (wallRunCameraTilt < -.2f && !isWallRight && !isWallLeft)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;

    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxBaseSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxBaseSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    // Wall Running
    // ================================================================================= //

    /// <summary>
    /// turns off gravity and adds forces to have player stick to wall
    /// </summary>
    private void Wallrun()
    {
        // Cancel out y velocity
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if(rb.useGravity)
                rb.useGravity = false;
        }

        // prevents player to be crouched on wall
        if (crouching)
            StopCrouch();

        // get player off wall if they go over max time
        if (timeOnWall > maxtimeOnWall)
        {
            // if player is in a corner really send them flying
            if(isWallLeft && isWallRight)
            {
                if (isWallForward)
                    rb.AddForce(-orientation.forward * jumpForce * Time.deltaTime, ForceMode.Impulse);
                else if (isWallBackward)
                    rb.AddForce(orientation.forward * jumpForce * Time.deltaTime, ForceMode.Impulse);
            }
            // otherwise a subtle nudge to get them off a wall
            else 
                WallJump(true);
        }

        // Stick player to wall
        /*
        if (isWallRight)
            rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
        else if(isWallLeft)
            rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
        else if(isWallForward)
            rb.AddForce(orientation.forward * wallrunForce / 5 * Time.deltaTime);
        else if (isWallBackward)
            rb.AddForce(-orientation.forward * wallrunForce / 5 * Time.deltaTime);
        */
    }

    /// <summary>
    /// Turns player gravity back on 
    /// </summary>
    private void StopWallRun()
    {
        rb.useGravity = true;
    }


    /// <summary>
    /// Checks if player is on wall and if so which side of the player is on the wall
    /// </summary> 
    private void CheckForWall() //make sure to call in void Update
    {
        IsOnWall = Physics.CheckSphere(wallCheck.position + Vector3.up, wallDistance, whatIsWall);
        if (IsOnWall) Wallrun();
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);
        isWallForward = Physics.Raycast(transform.position, orientation.forward, 1f, whatIsWall);
        isWallBackward = Physics.Raycast(transform.position, -orientation.forward, 1f, whatIsWall);

        //leave wall run
        if (!IsOnWall && !playerShooting.IsGrappling) StopWallRun();
        //reset double jump (if you have one :D)
        if (isWallLeft || isWallRight ) jumpsAvaliable = maxJumps;

        // Switch hand gun is in (Prevents gun from phasing into wall)
        // use form playerShooting.currentGun.rightHandPosition - gunPosition.localPosition.x < .1f instead of
        // form gunPosition.localPosition.x > playerShooting.currentGun.leftHandPosition
        // to prevent player weapon from glitching back and forth when player is close but not on a wall
        if (isWallRight && isWallLeft) return;
        if (!IsOnWall && playerShooting.currentGun.rightHandPosition - gunPosition.localPosition.x > .1f )
        {
            currentHandPosition = Time.deltaTime * playerShooting.currentGun.rightHandPosition * 14;
            gunPosition.localPosition -= Vector3.right * currentHandPosition;
        }
        else if (isWallLeft && playerShooting.currentGun.rightHandPosition - gunPosition.localPosition.x > .1f)
        {
            currentHandPosition = Time.deltaTime * playerShooting.currentGun.rightHandPosition * 14;
            gunPosition.localPosition -= Vector3.right * currentHandPosition;

        }
        // gunPosition.localPosition.x > playerShooting.currentGun.leftHandPosition
        else if (isWallRight && playerShooting.currentGun.rightHandPosition - gunPosition.localPosition.x < .1f)
        {
            currentHandPosition = Time.deltaTime * playerShooting.currentGun.leftHandPosition * 4;
            gunPosition.localPosition += Vector3.right * currentHandPosition;
        }
    }


    /// <summary>
    /// Player jumps from a wall
    /// </summary>
    /// <param name="motionless"> boolean: Tells function whether the player is motionless of not </param>
    private void WallJump(bool motionless)
    {
        float forceMultiplier = 0.05f;
        RaycastHit hit;
        Vector3 direction;

        jumpsAvaliable -= 2;

        if(!motionless)
        {
            // Sets multipler for non-motionless wall jump
            forceMultiplier = 1.5f;
        }

        // Jumps off wall (opposite direction relative to wall)
        // If wall to left
        if (Physics.Raycast(transform.position, -orientation.right, out hit, 1f))
        {
            direction = -(hit.point - transform.position).normalized;
            rb.AddForce(direction * jumpForce * forceMultiplier);
        }
        // If wall to right
        else if (Physics.Raycast(transform.position, orientation.right, out hit, 1f))
        {
            direction = -(hit.point - transform.position).normalized;
            rb.AddForce(direction * jumpForce * forceMultiplier);
        }
        // If wall in front
        else if (Physics.Raycast(transform.position, orientation.forward, out hit, 1f))
        {
            direction = -(hit.point - transform.position).normalized;
            rb.AddForce(direction * jumpForce * forceMultiplier);
        }
        // If wall behind
        else if (Physics.Raycast(transform.position, -orientation.forward, out hit, 1f))
        {
            direction = -(hit.point - transform.position).normalized;
            rb.AddForce(direction * jumpForce * forceMultiplier);
        }

        if (!motionless)
            //Add vertical jump forces
            StartCoroutine(WallJumpHelper());

    }

    /// <summary>
    /// Adds upward velocity after .1 seconds of scaled time. This is to allow for player to get horizontal distance from wall
    /// before applying an upward velocity (cleaner feel)
    /// </summary>
    private IEnumerator WallJumpHelper()
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForce(Vector3.up * jumpForce * 1.5f);
    }

    // End of wall running methods
    // ================================================================================= //
}