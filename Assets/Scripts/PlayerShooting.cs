using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // General Variables
    public Transform cam, player;
    public PlayerStats playerStats;
    private AudioManager audioManager;

    // Grappling Variables ==================
    // Components
    private LineRenderer lineRender;
    private SpringJoint joint;
    public Transform grappleFirePoint;
    public Rigidbody playerRB;

    public LayerMask whatIsGrapple;
    private Coroutine grappleRecovery;
    
    // Scripts
    public GrappleUIScript grappleUI;

    // Numerical variables
    private float maxGrappleDistance = 200f, maxGrappleTime = 3f, grappleRecoveryIncrement = .01f;
    private float timeLeftToGrapple;
    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    // Gun Variables =======================
    public float timeSinceLastShoot = 5f;
    public int reloadAnimationCounter = 0;
    public bool isAnimInProgress;
    public int scrollingCounter = 0;

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

    // Gun UI
    public AmmoUIScript ammoUI;

    // MysteryBox
    private MysteryBox mysteryBox;
    public LayerMask whatIsInteractable;
    public bool shouldInteract;

    // Testing Variables
    public GameObject collisionObject;
    public float timer;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        mysteryBox = FindObjectOfType<MysteryBox>();

        // Set up guns
        playerStats.SetGunInformation();

        int index = 0;
        gunNames = new string[playerStats.allGunInformation.Count];
        foreach(string str in playerStats.allGunInformation.Keys)
        {
            gunNames[index] = str;
            index++;
        }

        //currentGun = playerStats.allGunInformation["GunAR"];
        //secondaryGun = playerStats.allGunInformation["GunSMG"];        
        do
        {
            currentGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
            secondaryGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
        } while ( currentGun.name.Equals(secondaryGun.name) );

        currentGun.gunContainer.SetActive(true);
        ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        // Set up grapple
        timeLeftToGrapple = maxGrappleTime;
        grappleUI.SetMaxGrapple(maxGrappleTime);
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
        if(!isAnimInProgress)
        {
            // Switching between primary and secondary by using mouse scroll wheel or '1' on keyboard
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeCurrentWeapon();
            }    
            else if(Input.mouseScrollDelta.y != 0)
            {
                scrollingCounter++;
                if(scrollingCounter > 1)
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
                if(currentGun.isAutomatic)
                    CancelInvoke("BaseShoot");
                CorrectGunPosition();
            }
            // Reload
            else if(currentGun.currentAmmo < currentGun.magSize && Input.GetKeyDown(KeyCode.R) && currentGun.reserveAmmo > 0)
            {
                isAnimInProgress = true;
                audioManager.Play("GunReload");
                StartCoroutine(Reload());
            }

            if (CheckIfMysterBoxInFront())
            {
                shouldInteract = Input.GetKeyDown(KeyCode.E);
                HandleMysteryBox();
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
        grappleUI.SetGrapple(timeLeftToGrapple);
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
    private IEnumerator GrappleRecovery()
    {
        yield return new WaitForSeconds(.1f);
        if(timeLeftToGrapple <= maxGrappleTime)
        {
            timeLeftToGrapple += grappleRecoveryIncrement;
            grappleUI.SetGrapple(timeLeftToGrapple);
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
                StartCoroutine(Reload());
            }
            CorrectGunPosition();
            if(currentGun.isAutomatic)
                CancelInvoke("BaseShoot");
        }

        ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

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
                StartCoroutine(Reload());
            }
            CorrectGunPosition();
            if (currentGun.isAutomatic)
                CancelInvoke("BaseShoot");
        }

        ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

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

    /// <summary>
    /// Reload current gun and plays a reload animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload()
    {
        CancelInvoke("BaseShoot");
        yield return new WaitForSeconds(currentGun.reloadTime / 360f);

        // Rotate gun on x axis
        gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
        reloadAnimationCounter++;

        // if gun hasnt rotated 360 degress then rotate one more degree
        if ( reloadAnimationCounter < 360)
            StartCoroutine(Reload());
        // else reset variables and 'reload' gun
        else
        {
            // Reset reload variables
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            isAnimInProgress = false;
            reloadAnimationCounter = 0;
            if (currentGun.reserveAmmo > currentGun.magSize)
            {
                currentGun.reserveAmmo += -currentGun.magSize + currentGun.currentAmmo;
                currentGun.currentAmmo = currentGun.magSize;
            }
            else
            {
                if(currentGun.magSize - currentGun.currentAmmo <= currentGun.reserveAmmo)
                {
                    currentGun.currentAmmo = currentGun.magSize;
                    currentGun.reserveAmmo = currentGun.magSize - currentGun.currentAmmo;
                }
                else
                {
                    currentGun.currentAmmo += currentGun.reserveAmmo;
                    currentGun.reserveAmmo = 0;
                }
            }
            ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
        }
    }

    /// <summary>
    /// Change current weapon to secondary and vice versa
    /// Dependencies: ChangeCurrentWeaponAnimation
    /// </summary>
    private void ChangeCurrentWeapon()
    {
        isAnimInProgress = true;
        PlayerStats.GunInformation temp = currentGun;
        currentGun = secondaryGun;
        secondaryGun = temp;
        ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
        StartCoroutine(ChangeCurrentWeaponAnimation(0));
    }

    /// <summary>
    /// plays animation where primary gun is lifted over head and seconday gun comes down 
    /// </summary>
    /// <param name="counter">current count of how many times function has been called</param>
    private IEnumerator ChangeCurrentWeaponAnimation(int counter)
    {
        yield return new WaitForSeconds(1f / 180f);
        if (counter < 180)
        {
            // disable primary gun and enable new primary gun
            if(counter == 90)
            {
                secondaryGun.gunContainer.SetActive(false);
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
            StartCoroutine(ChangeCurrentWeaponAnimation(counter + 1));
        }
        else
        {
            // animation finished so reset variables
            isAnimInProgress = false;
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }

    private void PickUpWeapon(string gunName)
    {
        isAnimInProgress = true;
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
            ammoUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
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
            StartCoroutine(SingleFireRecoil(0, 10));
        }
    }

    /// <summary>
    /// Flips gun upwards to a 20 degree angle then brings it back to 0 degrees
    /// </summary>
    /// <param name="counter">current count of how many times function has been called</param>
    /// <param name="degree">The x degree gun should be rotated to</param>
    private IEnumerator SingleFireRecoil(int counter, int degree)
    {
        yield return new WaitForSeconds(currentGun.fireRate / ( degree * 2 ));
        timer += currentGun.fireRate / (degree * 2);
        // Prevents reload animation and recoil animation from happening at the same time
        if (currentGun.currentAmmo <= 0 )
        {
            yield break;
        }
        if (counter < (degree * 2))
        {
            // Rotate upward if count is less than 20
            if (counter <= degree / 2)
            {
                gunContainer.localRotation = Quaternion.Euler(-2f, 0, 0) * gunContainer.localRotation;
            }
            // otherwise rotate downward 
            else
            {
                if( !(gunContainer.localRotation.eulerAngles.x  < .1f) )
                    gunContainer.localRotation = Quaternion.Euler(1f, 0, 0) * gunContainer.localRotation;
            }
            // Start over
            StartCoroutine(SingleFireRecoil(counter + 1, degree));
        }
        else
        {
            // Animation finished so reset variables
            gunContainer.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            isAnimInProgress = false;
        }
    }
    
    /// <summary>
    /// Makes gun local position to be equal to (0, 0, 0)
    /// </summary>
    private void CorrectGunPosition()
    {
        gunPosition.localPosition = Vector3.zero;
    }

    public bool CheckIfMysterBoxInFront()
    {
        //Debug.Log("Check mysterbox called");
        if (Physics.Raycast(transform.position, cam.forward, 10f, whatIsInteractable))
            return true;
        return false;
    }

    private void HandleMysteryBox()
    {
        //Debug.Log("Handle Mystery Box is called");
        if (mysteryBox == null)
            mysteryBox = FindObjectOfType<MysteryBox>();

        if(mysteryBox.IsInteractable)
        {
            mysteryBox.ShowUI();
            if (shouldInteract)
                if(mysteryBox.IsWeaponSpawned)
                {
                    PickUpWeapon(mysteryBox.DestroyGun()) ; 
                }
                else if (playerStats.CurrentPoints > mysteryBox.MysteryBoxPrice)
                {
                    playerStats.CurrentPoints -= mysteryBox.MysteryBoxPrice;
                    playerStats.ChangeInPointValue();
                    mysteryBox.SpawnRandomWeapon();
                }
        }   
    }
}
