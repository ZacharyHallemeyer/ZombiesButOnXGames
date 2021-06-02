using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // General Variables
    public Transform cam, player;
    public PlayerStats playerStats;
    public PlayerMovement playerMovement;
    private AudioManager audioManager;

    // Grappling Variables ==================
    // Components
    private LineRenderer lineRender;
    private SpringJoint joint;
    public Transform grappleFirePoint;
    public Rigidbody playerRB;

    public LayerMask whatIsGrapple;
    private Coroutine grappleRecovery;

    // Numerical variables
    public float maxGrappleDistance = 200f, maxGrappleTime = 3f, grappleRecoveryIncrement = .01f;
    public float timeLeftToGrapple;

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    // Gun Variables =======================
    public float timeSinceLastShoot = 5f;
    public int scrollingCounter = 0;

    // Animation variables
    public int animationCounter = 0;
    public int singleFireRecoilDegree = 10;
    public bool isAnimInProgress;
    public bool isReloading;
    public bool isChangingCurrentWeapon;
    public bool isPickingUpWeapon;
    public bool isSingleFireRecoil;

    // Arrays
    public string[] gunNames;

    // Game objects and particle systems and misc
    public GameObject shotHitParitcleGameObject;
    public ParticleSystem shotHitParticle;
    public GameObject[] shotgunHitParticleGameObjects;
    public ParticleSystem[] shotgunHitParticles;

    public LayerMask whatIsShootable;
    public int currentParticleIndex;

    // Components 
    public Transform gunContainer;
    public Transform gunPosition;
    public Transform firePoint;

    // GunInformation variables
    public PlayerStats.GunInformation currentGun;
    public PlayerStats.GunInformation secondaryGun;

    // UI
    public PlayerUIScript playerUI;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        // Set up guns
        playerStats.SetGunInformation();

        int index = 0;
        gunNames = new string[playerStats.allGunInformation.Count];
        foreach(string str in playerStats.allGunInformation.Keys)
        {
            gunNames[index] = str;
            index++;
        }

        do
        {
            currentGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
            secondaryGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
        } while ( currentGun.name.Equals(secondaryGun.name) );

        currentGun.gunContainer.SetActive(true);
        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        // Set up grapple
        timeLeftToGrapple = maxGrappleTime;
        playerUI.SetMaxGrapple(maxGrappleTime);
        lineRender = GetComponent<LineRenderer>();
    }
    
    private void Update()
    {
        // Handle grapple
        // Player must have more than 25% of grapple left to start grapple
        if (Input.GetMouseButtonDown(2) && timeLeftToGrapple > (maxGrappleTime * .25))
        {
            StartGrapple();
        }
        else if ( (Input.GetMouseButtonUp(2) || Mathf.Abs( (player.position - GrapplePoint).magnitude ) < 5f )
                 && IsGrappling)
        {
            StopGrapple();
        }

        // Handle gun shooting
        if (!isAnimInProgress)
        {
            // Switching between primary and secondary by using mouse scroll wheel or '1' on keyboard
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeCurrentWeapon();
            }
            else if (Input.mouseScrollDelta.y != 0)
            {
                scrollingCounter++;
                if (scrollingCounter > 1)
                {
                    scrollingCounter = 0;
                    ChangeCurrentWeapon();
                }
            }


            // Shoot
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentGun.currentAmmo > 0)
            {
                if (currentGun.isAutomatic)
                    InvokeRepeating("BaseShoot", 0f, currentGun.fireRate);
                else
                {
                    if (currentGun.name.Equals("GunShotgun"))
                        ShotgunShoot();
                    else
                        BaseShoot();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (currentGun.isAutomatic)
                    CancelInvoke("BaseShoot");
                CorrectGunPosition();
            }
            // Reload
            else if (currentGun.currentAmmo < currentGun.magSize && Input.GetKeyDown(KeyCode.R) && currentGun.reserveAmmo > 0)
            {
                isAnimInProgress = true;
                audioManager.Play("GunReload");
                InvokeRepeating("Reload", 0, currentGun.reloadTime / 360f);
            }
        }
    }

    // Called after Update function
    private void LateUpdate()
    {
        DrawRope();
        if(IsGrappling)
        {
            ContinueGrapple();
        }
    }


    /// <summary>
    /// Turns off gravity, creates joints for grapple
    /// Call whenever player inputs for grapple
    /// Dependencies: DrawRope
    /// </summary>
    private void StartGrapple()
    {
        //Debug.Log("Start Grapple");

        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, whatIsGrapple ))
        //if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance) && !hit.collider.CompareTag("Player"))
        {
            playerMovement.StopCrouch();
            // Turn off grapple recovery
            if (grappleRecovery != null)
                StopCoroutine(grappleRecovery);

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
        {
            StopGrapple();
        }
    }


    /// <summary>
    /// Draw line with linerender from player position to grapple point (grapple point
    /// is found in StartGrapple)
    /// </summary>
    private void DrawRope()
    {
        if(!IsGrappling) return;

        lineRender.SetPosition(0, grappleFirePoint.position);
        lineRender.SetPosition(1, GrapplePoint);
    }

    /// <summary>
    /// Erases grapple rope, turns player gravity on, and destroys joint
    /// </summary>
    private void StopGrapple()
    {
        StartCoroutine(GrappleRecovery());
        playerRB.useGravity = true;
        lineRender.positionCount = 0;
        IsGrappling = false;
        Destroy(joint);
    }


    /// <summary>
    /// adds time to player's amount of grapple left by calling itself recursively 
    /// </summary>
    /// <returns>waits for .1 seconds</returns>
    public IEnumerator GrappleRecovery()
    {
        yield return new WaitForSeconds(.1f);
        if(timeLeftToGrapple <= maxGrappleTime)
        {
            timeLeftToGrapple += grappleRecoveryIncrement;
            playerUI.SetGrapple(timeLeftToGrapple);
            grappleRecovery = StartCoroutine(GrappleRecovery());
        }
    }

    /// <summary>
    /// fires a raycast from firepoint and damages if ray hits an enemy
    /// </summary>
    private void BaseShoot()
    {
        audioManager.Play(currentGun.name);
        SimulateRecoil();
        currentGun.muzzleFlash.Play();

        // Reduce accuracy by a certain value 
        Vector3 reduceAccuracy = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset), 
                                                                Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));

        currentGun.currentAmmo--;
        
        // Reload if current ammo is zero
        if(currentGun.currentAmmo <= 0)
        {
            if(currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                InvokeRepeating("Reload", 0, currentGun.reloadTime / 360f);
            }
            CorrectGunPosition();
            if(currentGun.isAutomatic)
                CancelInvoke("BaseShoot");
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        Ray ray = new Ray(firePoint.position, reduceAccuracy);
        if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
        {
            // Play particle representing where ray hit
            shotHitParitcleGameObject.transform.position = hit.point;
            shotHitParticle.Play();

            //Instantiate(collisionObject, hit.point, hit.collider.transform.rotation);

            // Damage enemy if hit was an enemy
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats == null) return;

            enemyStats.TakeDamage(currentGun.damage);
        }
    }

    private void ShotgunShoot()
    {
        audioManager.Play(currentGun.name);
        Vector3 trajectory;
        SimulateRecoil();
        currentGun.muzzleFlash.Play();

        currentGun.currentAmmo--;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                InvokeRepeating("Reload", 0, currentGun.reloadTime / 360f);
            }
            CorrectGunPosition();
            if (currentGun.isAutomatic)
                CancelInvoke("BaseShoot");
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        for(int i = 0; i < 10; i++)
        {
            trajectory = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                         Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                         Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));
            Ray ray = new Ray(firePoint.position, trajectory);
            if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
            {
                // Play particle representing where ray hit
                shotgunHitParticleGameObjects[i].transform.position = hit.point;
                shotgunHitParticles[i].Play();


                // Damage enemy if hit was an enemy
                EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
                if ( !(enemyStats == null) )
                    enemyStats.TakeDamage(currentGun.damage);
            }
        }

    }

    private void Reload()
    {
        CancelInvoke("BaseShoot");

        // Rotate gun on x axis
        gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
        animationCounter++;

        // if gun hasnt rotated 360 degress then rotate one more degree
        if(animationCounter >= 360)
        {
            // Reset reload variables
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            isAnimInProgress = false;
            animationCounter = 0;
            if (currentGun.reserveAmmo > currentGun.magSize)
            {
                currentGun.reserveAmmo += -currentGun.magSize + currentGun.currentAmmo;
                currentGun.currentAmmo = currentGun.magSize;
            }
            else
            {
                if (currentGun.magSize - currentGun.currentAmmo <= currentGun.reserveAmmo)
                {
                    currentGun.reserveAmmo -= currentGun.magSize - currentGun.currentAmmo;
                    currentGun.currentAmmo = currentGun.magSize;
                }
                else
                {
                    currentGun.currentAmmo += currentGun.reserveAmmo;
                    currentGun.reserveAmmo = 0;
                }

            }
            playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
            CancelInvoke("Reload");
        }
    }

    /// <summary>
    /// Change current weapon to secondary and vice versa
    /// Dependencies: ChangeCurrentWeaponAnimation
    /// </summary>
    private void ChangeCurrentWeapon()
    {
        CancelInvoke("BaseShoot");
        isAnimInProgress = true;
        PlayerStats.GunInformation temp = currentGun;
        currentGun = secondaryGun;
        secondaryGun = temp;
        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
        InvokeRepeating("ChangeCurrentGunAnimation", 0, 1f / 180f);
    }

    private void ChangeCurrentGunAnimation()
    {
        animationCounter++;
        if (animationCounter < 180)
        {
            // disable primary gun and enable new primary gun
            if (animationCounter == 90)
            {
                secondaryGun.gunContainer.SetActive(false);
                currentGun.gunContainer.SetActive(true);
                // fix gun rotation if player is crouching to prevent secondary gun having a crouch angled rotation while
                // player is not crouching
                if(playerMovement.crouching)
                {
                    playerMovement.InvokeRepeating("StartCrouchAnimation", 0, .1f / playerMovement.crouchGunDegree);
                    secondaryGun.gunContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            // Rotate gun upwards if counter is less han 90
            if (animationCounter < 90)
            {
                gunContainer.localRotation = Quaternion.Euler(-1f, 0, 0) * gunContainer.localRotation;
            }
            // Rotate gun downwards otherwise
            else
            {
                gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
            }
        }
        else
        {
            // animation finished so reset variables
            isAnimInProgress = false;
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            animationCounter = 0;
            CancelInvoke("ChangeCurrentGunAnimation");
        }
    }

    public void PickUpWeapon(string gunName)
    {
        isAnimInProgress = true;
        // Reset reserve ammo for current weapon
        currentGun.reserveAmmo = currentGun.ammoIncrementor;
        StartCoroutine(PickWeapnAnimation(0, gunName));
    }

    /// <summary>
    /// plays animation where primary gun is lifted over head and seconday gun comes down 
    /// </summary>
    /// <param name="counter">current count of how many times function has been called</param>
    private IEnumerator PickWeapnAnimation(int counter, string gunName)
    {
        yield return new WaitForSeconds(1f / 180f);
        if (counter < 180)
        {
            // disable primary gun and enable new primary gun
            if (counter == 90)
            {
                currentGun.gunContainer.SetActive(false);
                currentGun = playerStats.allGunInformation[gunName];
                currentGun.gunContainer.SetActive(true);
            }   

            // Rotate gun upwards if counter is less han 90
            if (counter < 90)
            {
                gunContainer.localRotation = Quaternion.Euler(-1f, 0, 0) * gunContainer.localRotation;
            }
            // Rotate gun downwards otherwise
            else
            {
                gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
            }
            // Start over
            StartCoroutine(PickWeapnAnimation(counter + 1, gunName));
        }
        else
        {
            // animation finished so reset variables
            isAnimInProgress = false;
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
        }
    }

    /// <summary>
    /// either makes gun jump around slightly in x/y directions or makes gun flip upwards depending on whether 
    /// the current gun is automatic
    /// Dependencies: SingleFireRecoil
    /// </summary>
    private void SimulateRecoil()
    {
        if (currentGun.isAutomatic)
            gunPosition.localPosition = new Vector3(Random.Range(-.025f, .025f), Random.Range(-.025f, .025f), 0f);
        else
        {
            isAnimInProgress = true;
            InvokeRepeating("SingleFireRecoil", 0f, currentGun.fireRate / (singleFireRecoilDegree * 2));
            //StartCoroutine(SingleFireRecoil(0, 10));
        }
    }

    private void SingleFireRecoil()
    {
        // Prevents reload animation and recoil animation from happening at the same time
        if (currentGun.currentAmmo <= 0)
        {
            CancelInvoke("SingleFireRecoil");
        }
        animationCounter++;
        if (animationCounter < (singleFireRecoilDegree * 2))
        {
            // Rotate upward if count is less than 20
            if (animationCounter <= singleFireRecoilDegree / 2)
            {
                gunContainer.localRotation = Quaternion.Euler(-2f, 0, 0) * gunContainer.localRotation;
            }
            // otherwise rotate downward 
            else
            {
                if (!(gunContainer.localRotation.eulerAngles.x < .1f))
                    gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
            }
        }
        else
        {
            // Animation finished so reset variables
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            isAnimInProgress = false;
            animationCounter = 0;
            CancelInvoke("SingleFireRecoil");
        }
    }    
    
    /// <summary>
    /// Makes gun local position to be equal to (0, 0, 0)
    /// </summary>
    private void CorrectGunPosition()
    {
        gunPosition.localPosition = Vector3.zero;
    }
}
