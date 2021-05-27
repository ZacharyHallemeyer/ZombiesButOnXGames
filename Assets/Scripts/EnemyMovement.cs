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

    // Movement
    public float maxSpeed = 40f;
    
    public float MoveSpeed { get; set; } = 2000;

    public float groundedDistance = .75f;
    public float timeMotionless = 0f;
    public float maxTimeMotionless = 2f;
    private float maxJumpVelocity= 50f, minJumpVelocity = 5f;
    public float range = 20f;
    public bool grounded, jumpActive;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public RaycastHit hit;

    // Knockback variables
    private float knockbackForce = 1500f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        Look();
        CheckStatus();
    }

    private void FixedUpdate()
    {
        Movement();
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
        // if there is an obstacle between enemy and player
        // then jump over it
        if (IsObstacle())
        {
            if (Physics.Raycast(transform.position, transform.forward, 5f))
                Knockback(0);
            if (grounded && !jumpActive)
                Jump( 1.3f * CaclulateJumpVelocity(hit.collider.transform.position.y * hit.collider.transform.localScale.y
                                                  - transform.position.y * transform.localScale.y));
            return;
        }   
        direction = player.position - transform.position;


        // Player is above enemy so jump up to them
        if (player.position.y - transform.position.y > 1f && !jumpActive && InRange())
            StartCoroutine(WaitAndJump());

        // Check if enemy is going faster than max speed return so enemy does not recieve more force
        if ( new Vector2(rb.velocity.x, rb.velocity.y).magnitude  > maxSpeed )
            return;

        // Add force in direction of player
        rb.AddForce(direction.normalized * MoveSpeed * Time.deltaTime);

        // Random movement on random chance to add less predictable patterns to enemy movement
        if (Random.Range(0,50) == 0)
        {
            if (grounded && !jumpActive)
                Jump( Random.Range(10, 20) );
        }
        if(Random.Range(0,50) == 0)
        {
            rb.AddForce(new Vector3(Random.Range(-10f , 10f), 0, 0) * MoveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Applies force backwards
    /// </summary>
    /// <param name="knockbackForce"></param>
    private void Knockback(float knockbackForce)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
        {
            rb.velocity = Vector3.zero;
            Vector3 direction = -(hit.point - transform.position).normalized;
            rb.AddForce(direction * knockbackForce);
        }
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
        //if (player.transform.position.y - transform.position.y < -1f)
        //    return false;

        if (Physics.Raycast(obstacleCheck.position, player.position - obstacleCheck.position, out hit,
            Vector3.Distance(transform.position, obstacleCheck.position), whatIsObstacle))
        {
            if (player.transform.position.y - transform.position.y < -1f)
                if (hit.collider.transform.position.y * hit.collider.transform.localScale.y
                    < transform.position.y * transform.localScale.y)
                    return false;
            //Debug.Log("Obstacle: " + hit.collider.name + ", Height: " + hit.collider.transform.position.y * hit.collider.transform.localScale.y);
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Applys knockback with enemy hits any object
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Knockback(knockbackForce);
        }
    }
    
    /// <summary>
    /// Returns the jump velocity required to jump to a certain y position
    /// </summary>
    /// <param name="relativeJumpHeight"> gameobject.transform.position.y - transfrom.position.y </param>
    private float CaclulateJumpVelocity(float relativeJumpHeight)
    {
        // Prevents error in sqrt() a negative number 
        // This can occur because enemy waits a specified time before jumping
        // (player can be below enemy at that point)
        if (relativeJumpHeight < 0) return 0f;

        return Mathf.Sqrt( -2f * Physics.gravity.y * relativeJumpHeight);
    }

    /// <summary>
    /// applies a y velocity to enemy
    /// Dependencies: TurnOffJumpActiveFlag
    /// </summary>
    /// <param name="jumpVelocity">y velocity</param>
    private void Jump(float jumpVelocity)
    {
        //Debug.Log("Jump is called with the jump velociy of " + jumpVelocity);
        if (jumpVelocity > maxJumpVelocity)
            jumpVelocity = maxJumpVelocity;
        else if (jumpVelocity < minJumpVelocity)
            jumpVelocity = minJumpVelocity;
        //Debug.Log("Jump velocity: " + jumpVelocity);
        jumpActive = true;
        Vector3 vel = rb.velocity;
        rb.velocity = new Vector3(vel.x, jumpVelocity, vel.z);
        StartCoroutine(TurnOffJumpActiveFlag());
    }

    /// <summary>
    /// waits a specified time then calls Jump() function 
    /// This is useful because this allows enemy to jump to
    /// player's apex of jump rather than y position that is not the apex
    /// Dependencies: Jump()
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitAndJump()
    {
        yield return new WaitForSeconds(.2f);
        Jump(CaclulateJumpVelocity(player.position.y * player.localScale.y * 1.2f - transform.position.y * transform.localScale.y));
    }

    /// <summary>
    /// Recursively calls itself until enemy is grounded in which case 
    /// function sets jumpActive to be false
    /// </summary>
    /// <returns> waits .1 seconds </returns>
    private IEnumerator TurnOffJumpActiveFlag()
    {
        yield return new WaitForSeconds(.1f);
        if (grounded )
            jumpActive = false;
        else
            StartCoroutine(TurnOffJumpActiveFlag());
    }
}
