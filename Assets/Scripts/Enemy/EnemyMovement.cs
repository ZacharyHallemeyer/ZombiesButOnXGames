using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Components
    public Rigidbody rb;
    public Transform player;
    public Transform groundCheck;
    public Transform obstacleCheck;

    // Scripts
    public EnemyStats enemyStats;

    // Movement
    public float maxSpeed = 40f;
    
    public float MoveSpeed { get; set; } = 2000;

    private float groundedDistance = 2f;
    private float timeMotionless = 0f;
    private float maxTimeMotionless = 2f;
    private float range = 25f;
    private float threshold = .75f;
    private float jumpMultiplier = 1.5f;
    private float minJumpVelocity = 7f;
    public bool grounded, jumpActive;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public RaycastHit hit;

    // Knockback variables
    private int knockbackForce = 1500;
    private float shockWaveForce = 5000;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        InvokeRepeating("RandomMovement", Random.Range(.1f, 10f), 2.5f);
    }

    void Update()
    {
        Look();
        CheckStatus();

        // Prevents enemy from being stuck
        if (rb.velocity.magnitude < 2)
            timeMotionless += Time.deltaTime;
        else
            timeMotionless = 0;
        if (timeMotionless > maxTimeMotionless)
        {
            timeMotionless = 0;
            Knockback();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                Knockback(knockbackForce, collision.transform);
                break;
            case "Player":
                Knockback(knockbackForce, collision.transform);
                break;
            case "ShockWave":
                Knockback(shockWaveForce, collision.transform);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Check if enemy is grounded or not
    /// </summary>
    private void CheckStatus()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundedDistance, whatIsGround);
    }

    /// <summary>
    /// Faces towards player
    /// </summary>
    private void Look()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, desiredRotation.eulerAngles.y, 0f ), Time.deltaTime * 10f);
    }
    

    private void Movement()
    {
        Vector3 direction;
        // if there is an obstacle between enemy and player then jump over it
        if (IsObstacle())
        {
            if (!jumpActive && IsPlayerInCurrentDirection(0f) && rb.velocity.y < 10f)
                Jump( jumpMultiplier * CaclulateJumpVelocity( FindRelativeHeight(hit.transform)));
        }   
        //direction = (player.position - transform.position);
        direction = (new Vector3(player.position.x, 0, player.position.z) 
                     - new Vector3(transform.position.x, 0, transform.position.z));

        // Player is above enemy so jump up to them
        if (player.position.y - transform.position.y > 1f && !jumpActive
                && InRange() && IsPlayerInCurrentDirection(threshold)
                && rb.velocity.y < 1f)
            StartCoroutine(WaitAndJump(jumpMultiplier, .2f, player));

        // Check if enemy is going faster than max speed return so enemy does not recieve more force
        if ( new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude  > maxSpeed )
            return;

        // Add force in direction of player
        rb.AddForce(direction.normalized * MoveSpeed * Time.deltaTime);
    }

    
    private void RandomMovement()
    {
        if (!InRange())
            return;
        if(Random.Range(0, 5) == 0)
        {
            if (grounded && !jumpActive)
                // Twice as high as current player position
                StartCoroutine(WaitAndJump(2, .2f, player));
        }
    }

    /// <summary>
    /// Returns true is this gameobject is traveling toward player
    /// </summary>
    /// <param name="threshold">the closer to 1 threshold becomes the more accurate the function becomes</param>
    private bool IsPlayerInCurrentDirection(float threshold)
    {
        if (rb.velocity.magnitude < .1)
            return true;
        if(Vector3.Dot(transform.forward.normalized, rb.velocity.normalized) > threshold)
            return true;
        return false;
    }

    /// <summary>
    /// Returns true if player is within 'range'(float) distance of player with no regard to y position
    /// </summary>
    private bool InRange()
    {
        Vector3 playerPosition = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        if (Vector3.Distance(playerPosition, currentPosition) < range)
            return true;
        return false;
    }


    /// <summary>
    /// Returns true if there is a obstacle between enemy and player
    /// Unless the enemy is above the player in which case function will 
    /// return false
    /// </summary>
    private bool IsObstacle()
    {
        // minus around 5 from max distance to prevent viewing wall behind player as an obstacle
        if (Physics.Raycast(obstacleCheck.position, player.position - obstacleCheck.position, out hit,
            Vector3.Distance(obstacleCheck.position, player.position) - 5f, whatIsObstacle))
        {
            if (player.transform.position.y - transform.position.y < -1f)
                if (hit.collider.transform.position.y * hit.collider.transform.localScale.y
                    < transform.position.y * transform.localScale.y)
                    return false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Applies force backwards
    /// </summary>
    /// <param name="knockbackForce"></param>
    private void Knockback(float knockbackForce, Transform collider)
    {
        if (collider.GetComponent<Rigidbody>() != null)
            rb.velocity = collider.GetComponent<Rigidbody>().velocity;
        else
            rb.velocity = Vector3.zero;

        rb.AddForce(-(collider.position - transform.position).normalized 
                     * knockbackForce * Time.deltaTime, ForceMode.Impulse);
    }

    /// <summary>
    /// Applies force backwards. This should only be called for if this gameobject is motionless for an extended amount of time
    /// </summary>
    private void Knockback()
    {
        rb.AddForce(-transform.forward * MoveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    // JUMP ===================================================================

    /// <summary>
    /// applies a y velocity to enemy
    /// Dependencies: TurnOffJumpActiveFlag
    /// </summary>
    /// <param name="jumpVelocity">y velocity</param>
    private void Jump(float jumpVelocity)
    {
        jumpActive = true;
        if (jumpVelocity < minJumpVelocity)
            jumpVelocity = minJumpVelocity;
        Vector3 vel = rb.velocity;
        rb.velocity = new Vector3(vel.x, jumpVelocity, vel.z);
        StartCoroutine(TurnOffJumpActiveFlag());
    }

    /// <summary>
    /// Returns the jump velocity required to jump to a certain y position
    /// </summary>
    /// <param name="relativeJumpHeight"> gameobject.transform.position.y - transfrom.position.y </param>
    private float FindRelativeHeight(Transform target)
    {
        return target.position.y + target.localScale.y / 2 - transform.position.y + transform.localScale.y / 2;
    }

    private float CaclulateJumpVelocity(float relativeJumpHeight)
    {
        // Prevents error in sqrt() a negative number 
        // This can occur because enemy waits a specified time before jumping
        // (player can be below enemy at that point)
        if (relativeJumpHeight < 0) return 0f;

        return Mathf.Sqrt(-1f * Physics.gravity.y * relativeJumpHeight);
    }

    /// <summary>
    /// call jump function after 'timeToWait' amount of scaled seconds. Also calls compress and expand jump animations 
    /// </summary>
    /// <param name="jumpMultiplier">value to multiply with jumpVelocity</param>
    /// <param name="timeToWait">time (scaled time seconds) before jump function is called</param>
    /// <param name="target">The Transform of the position to jump to</param>
    /// <returns></returns>
    private IEnumerator WaitAndJump(float jumpMultiplier, float timeToWait, Transform target)
    {
        // compresses game object to half of its height around 3/4 of timeToWait
        InvokeRepeating("CompressJumpAnimation", 0f, timeToWait / 10f);
        yield return new WaitForSeconds(timeToWait);
        // expands game object to its original height around 3/4 of timeToWait
        InvokeRepeating("ExpandJumpAnimation", 0f, timeToWait / 10f);
        if(!jumpActive)
            Jump(jumpMultiplier * CaclulateJumpVelocity(FindRelativeHeight(target)));

    }

    /// <summary>
    /// Recursively calls itself until enemy is grounded in which case 
    /// function sets jumpActive to be false
    /// </summary>
    /// <returns> waits .1 seconds </returns>
    private IEnumerator TurnOffJumpActiveFlag()
    {
        yield return new WaitForSeconds(.1f);
        if (grounded)
            jumpActive = false;
        else
            StartCoroutine(TurnOffJumpActiveFlag());
    }

    /// <summary>
    /// Compresses gameobject to half of its height in 7 interations
    /// </summary>
    private void CompressJumpAnimation()
    {
        transform.localScale =  new Vector3(transform.localScale.x, transform.localScale.y * .9f, transform.localScale.z) ;
        //transform.position = new Vector3(transform.position.x, transform.position.y * .9f, transform.position.z);
        if (transform.localScale.y < enemyStats.originalScale.y/2)
            CancelInvoke("CompressJumpAnimation");
    }

    /// <summary>
    /// expands gameobject to its original height in 7 interations
    /// </summary>
    private void ExpandJumpAnimation()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 1.01f, transform.localScale.z);
        if (transform.localScale.y >= enemyStats.originalScale.y)
            CancelInvoke("ExpandJumpAnimation");
    }

    // END OF JUMP ======================================================
}
