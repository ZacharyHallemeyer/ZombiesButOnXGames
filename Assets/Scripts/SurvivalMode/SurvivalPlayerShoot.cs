using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalPlayerShoot : MonoBehaviour
{
    // General Variables
    public Transform cam, player;
    public SurvivePlayerStats playerStats;
    public SurvivalPlayerMovement playerMovement;
    private AudioManager audioManager;
    private InputMaster inputMaster;

    // Grappling Variables ==================

    // Components
    private LineRenderer lineRender;
    private SpringJoint joint;
    public Transform grappleFirePoint;
    public Rigidbody playerRB;

    public LayerMask whatIsGrapple;

    // Numerical variables
    private float maxGrappleDistance = 200f, minGrappleDistance = 10f;
    public float maxGrappleTime = 3f, grappleRecoveryIncrement = .01f;
    public float timeLeftToGrapple;

    private bool releasedGrappleControlSinceLastGrapple = true;
    public bool IsGrappleRecoveryInProgress { get; set; } = false;
    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    // Grapple animation
    private Vector3 currentGrapplePoint;
    private int grappleRopeDivider;
    private int maxGrappleRopeDivider = 30;
    private bool grappleContactMade = false;

    // UI
    public PlayerUIScript playerUI;

    private void Awake()
    {
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);
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


    private void Start()
    {
        // Set up grapple
        timeLeftToGrapple = maxGrappleTime;
        playerUI.SetMaxGrapple(maxGrappleTime);
        lineRender = GetComponent<LineRenderer>();

        StartCoroutine(SetAudioManager());
    }

    /// <summary>
    /// Call to set audio manager. This prevents player from referencing a null instance of audio manager
    /// when changing scenes (AudioManager is set to not destroy on load)
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetAudioManager()
    {
        yield return new WaitForEndOfFrame();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (!IsGrappling)
        {
            if (inputMaster.Player.Grapple.ReadValue<float>() != 0
                    && timeLeftToGrapple > (maxGrappleTime * .25)
                    && releasedGrappleControlSinceLastGrapple)
            {
                releasedGrappleControlSinceLastGrapple = false;
                StartGrapple();
            }
        }
        if (!releasedGrappleControlSinceLastGrapple)
        {
            if (inputMaster.Player.Grapple.ReadValue<float>() == 0
                    || Mathf.Abs((player.position - GrapplePoint).magnitude) < 5f
                    && IsGrappling)
            {
                releasedGrappleControlSinceLastGrapple = true;
                StopGrapple();
            }
        }
    }

    // Called after Update function
    private void LateUpdate()
    {
        if (IsGrappling)
        {
            if(grappleContactMade)
            {
                DrawRope();
                ContinueGrapple();
            }
            else
                DrawIncompleteRope();
        }
    }

    /// <summary>
    /// Turns off gravity, creates joints for grapple
    /// Call whenever player inputs for grapple
    /// Dependencies: DrawRope
    /// </summary>
    private void StartGrapple()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, whatIsGrapple))
        {
            if (Vector3.Distance(transform.position, hit.point) < minGrappleDistance)
                return;
            grappleRopeDivider = maxGrappleRopeDivider;
            grappleContactMade = false;
            if (IsGrappleRecoveryInProgress)
            {
                IsGrappleRecoveryInProgress = false;
                CancelInvoke("GrappleRecovery");
            }

            playerMovement.StopCrouch();
            // Turn off grapple recovery

            // Turn player gravity off
            playerRB.useGravity = false;
            IsGrappling = true;

            // Create joint ("Grapple rope") and anchor to player and grapple point
            GrapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = GrapplePoint;

            joint.spring = 0f;
            joint.damper = 0f;
            joint.massScale = 0f;

            lineRender.positionCount = 2;

            // Lift player off ground
            playerRB.AddForce(Vector2.up * 150);
        }
    }

    /// <summary>
    /// Call every frame while the player is grappling
    /// </summary>
    private void ContinueGrapple()
    {
        // Reduce time left of grapple
        timeLeftToGrapple -= Time.deltaTime;
        playerUI.SetGrapple(timeLeftToGrapple);
        if (timeLeftToGrapple < 0)
            StopGrapple();

        // Pull player to grapple point
        Vector3 direction = (GrapplePoint - player.position).normalized;
        playerRB.AddForce(direction * 100 * Time.deltaTime, ForceMode.Impulse);

        // Prevent grapple from phasing through/into objectsz
        // (Game objects such as buildings must have a rotation for this section to work)
        if (Physics.Raycast(GrapplePoint, (player.position - GrapplePoint), Vector3.Distance(GrapplePoint, player.position) - 5, whatIsGrapple))
            StopGrapple();
    }


    private void DrawIncompleteRope()
    {
        if (!IsGrappling) return;
        currentGrapplePoint = (GrapplePoint - grappleFirePoint.position) / grappleRopeDivider 
                               + grappleFirePoint.position;
        grappleRopeDivider--;

        lineRender.SetPosition(0, grappleFirePoint.position);
        lineRender.SetPosition(1, currentGrapplePoint);
        if (grappleRopeDivider <= 1)
            grappleContactMade = true;
    }

    /// <summary>
    /// Draw line with linerender from player position to grapple point (grapple point
    /// is found in StartGrapple)
    /// </summary>
    private void DrawRope()
    {
        if (!IsGrappling) return;

        lineRender.SetPosition(0, grappleFirePoint.position);
        lineRender.SetPosition(1, GrapplePoint);
    }

    /// <summary>
    /// Erases grapple rope, turns player gravity on, and destroys joint
    /// </summary>
    private void StopGrapple()
    {
        if (!IsGrappleRecoveryInProgress)
        {
            IsGrappleRecoveryInProgress = true;
            InvokeRepeating("GrappleRecovery", 0f, .1f);
        }
        playerRB.useGravity = true;
        lineRender.positionCount = 0;
        IsGrappling = false;
        Destroy(joint);
    }


    /// <summary>
    /// adds time to player's amount of grapple left. Must be called through invoke repeating with
    /// a repeat time of .1 seconds of scaled time
    /// </summary>
    public void GrappleRecovery()
    {
        if (timeLeftToGrapple <= maxGrappleTime)
        {
            timeLeftToGrapple += grappleRecoveryIncrement;
            playerUI.SetGrapple(timeLeftToGrapple);
        }
        else
            CancelInvoke("GrappleRecovery");
    }
    // END OF GRAPPLE ================================================================
}
