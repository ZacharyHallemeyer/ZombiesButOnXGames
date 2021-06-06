using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentalPlayerShooting : MonoBehaviour
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

    public class GunInformation
    {
        public string name;
        public GameObject gunContainer;
        public ParticleSystem muzzleFlash;
        public ParticleSystem gun;
        public float originalGunRadius;
        public int magSize;
        public int ammoIncrementor;
        public int reserveAmmo;
        public int currentAmmo;
        public float damage;
        public float fireRate;
        public float accuaracyOffset;
        public float reloadTime;
        public float range;
        public bool isAutomatic;
    }

    public Dictionary<string, GunInformation> allGunInformation { get; private set; } = new Dictionary<string, GunInformation>();

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

    // Guns
    // Gun models (GameObject) and muzzleflashes (ParticleSystem)
    public GameObject gunPistol;
    public GameObject gunSMG;
    public GameObject gunAR;
    public GameObject gunShotgun;
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem smgMuzzleFlash;
    public ParticleSystem arMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;
    public ParticleSystem pistol;
    public ParticleSystem smg;
    public ParticleSystem ar;
    public ParticleSystem shotgun;


    public GunInformation currentGun;
    public GunInformation secondaryGun;
    private ParticleSystem.ShapeModule  shapeModule;

    public LayerMask whatIsShootable;
    public int currentParticleIndex;

    // Components 
    public Transform gunContainer;
    public Transform gunPosition;
    public Transform firePoint;

    // UI
    public PlayerUIScript playerUI;

    // Grenade
    public int CurrentGrenades { get; set; } = 5;
    public Transform grenadeFirePoint;
    public GameObject grenadePrefab;
    public int grenadeThrowForce;

    // Item variables
    public int CurrentShockWaves { get; set; } = 3;
    public PlayerItems playerItems;

    // TESTING VARIABLES
    public GameObject particleSytemObject;
    public ParticleSystem particleSytem;
    
    private void Start()
    {
        // Set up items and grenades 
        playerUI.SetGrenadeText(CurrentGrenades);
        playerUI.SetShockWaveText(CurrentShockWaves);

        // Set up guns
        SetGunInformation();

        int index = 0;
        gunNames = new string[allGunInformation.Count];
        foreach(string str in allGunInformation.Keys)
        {
            gunNames[index] = str;
            index++;
        }

        /*do
        {
            currentGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
            secondaryGun = playerStats.allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
        } while ( currentGun.name.Equals(secondaryGun.name) );*/

        currentGun = allGunInformation[gunNames[1]];
        currentGun.gunContainer.SetActive(true);
        shapeModule = currentGun.gun.shape;
        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

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

    /// <summary>
    /// Fills the dictionary allGunInformation with hardcoded informaiton of each gun
    /// </summary>
    public void SetGunInformation()
    {
        allGunInformation["GunPistol"] = new GunInformation
        {
            name = "GunPistol",
            gunContainer = gunPistol,
            muzzleFlash = pistolMuzzleFlash,
            gun = pistol,
            originalGunRadius = pistol.shape.radius,
            magSize = 6,
            ammoIncrementor = 60,
            reserveAmmo = 60,
            currentAmmo = 6,
            damage = 30,
            fireRate = .25f,
            accuaracyOffset = .001f,
            reloadTime = 1f,
            range = 1000f,
            isAutomatic = false,
        };

        allGunInformation["GunSMG"] = new GunInformation
        {
            name = "GunSMG",
            gunContainer = gunSMG,
            muzzleFlash = smgMuzzleFlash,
            gun = smg,
            originalGunRadius = smg.shape.radius,
            magSize = 30,
            ammoIncrementor = 300,
            reserveAmmo = 300,
            currentAmmo = 30,
            damage = 10,
            fireRate = .1f,
            accuaracyOffset = .025f,
            reloadTime = .1f,
            range = 1000f,
            isAutomatic = true,
        };

        allGunInformation["GunAR"] = new GunInformation
        {
            name = "GunAR",
            gunContainer = gunAR,
            muzzleFlash = arMuzzleFlash,
            gun = ar,
            originalGunRadius = ar.shape.radius,
            magSize = 20,
            ammoIncrementor = 260,
            reserveAmmo = 260,
            currentAmmo = 20,
            damage = 20,
            fireRate = .2f,
            accuaracyOffset = .02f,
            reloadTime = 1f,
            range = 1000f,
            isAutomatic = true,
        };

        allGunInformation["GunShotgun"] = new GunInformation
        {
            name = "GunShotgun",
            gunContainer = gunShotgun,
            muzzleFlash = shotgunMuzzleFlash,
            gun = shotgun,
            originalGunRadius = shotgun.shape.radius,
            magSize = 8,
            ammoIncrementor = 80,
            reserveAmmo = 80,
            currentAmmo = 8,
            damage = 10,
            fireRate = .5f,
            accuaracyOffset = .1f,
            reloadTime = 1f,
            range = 75f,
            isAutomatic = false,
        };
    }

    private void Update()
    {
        // Handle grapple =========================================
        // Player must have more than 25% of grapple left to start grapple
        if (Input.GetMouseButtonDown(2) && timeLeftToGrapple > (maxGrappleTime * .25))
            StartGrapple();
        else if ( (Input.GetMouseButtonUp(2) || Mathf.Abs( (player.position - GrapplePoint).magnitude ) < 5f )
                 && IsGrappling)
            StopGrapple();

        // Handle gun shooting ====================================
        if (!isAnimInProgress)
        {
            // Switching between primary and secondary by using mouse scroll wheel or '1' on keyboard
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeCurrentWeapon();
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
                {
                    ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
                    rot.enabled = true;
                    InvokeRepeating("AutomaticShoot", 0f, currentGun.fireRate);
                }
                else
                    SingleFireShoot();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if(currentGun.isAutomatic)
                {
                    ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
                    rot.enabled = false;
                    CancelInvoke("AutomaticShoot");
                }
            }
            // Reload
            else if (currentGun.currentAmmo < currentGun.magSize && Input.GetKeyDown(KeyCode.R) && currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                isAnimInProgress = true;
                Reload();
            }

            // Handle Grenades
            if (Input.GetKeyDown(KeyCode.G) && CurrentGrenades > 0)
                ThrowGrenade();
        }
        // Handle items
        if (Input.GetKeyDown(KeyCode.Q) && CurrentShockWaves > 0)
        {
            CurrentShockWaves--;
            playerUI.SetShockWaveText(CurrentShockWaves);
            playerItems.StartShockWave();
        }
    }

    // Called after Update function
    private void LateUpdate()
    {
        DrawRope();
        if(IsGrappling)
            ContinueGrapple();
    }



    /// <summary>
    /// fires a raycast from firepoint and damages if ray hits an enemy
    /// </summary>
    private void AutomaticShoot()
    {
        audioManager.Play(currentGun.name);
        currentGun.muzzleFlash.Play();
        // Reduce accuracy by a certain value 
        Vector3 reduceAccuracy = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                                Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));

        currentGun.currentAmmo--;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                Reload();
                CancelInvoke("AutomaticShoot");
            }
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        Ray ray = new Ray(firePoint.position, reduceAccuracy);
        if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
        {
            // Play particle representing where ray hit
            shotHitParitcleGameObject.transform.position = hit.point;
            shotHitParticle.Play();

            // Damage enemy if hit was an enemy
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats == null) return;

            enemyStats.TakeDamage(currentGun.damage);
        }

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                isAnimInProgress = true;
                Reload();
            }
            if (currentGun.isAutomatic)
                CancelInvoke("AutomaticShoot");
        }
    }

    /// <summary>
    /// fires 10 raycasts from firepoint and damages if ray hits an enemy
    /// </summary>
    private void SingleFireShoot()
    {
        isAnimInProgress = true;
        audioManager.Play(currentGun.name);
        currentGun.gun.Stop();
        currentGun.muzzleFlash.Play();
        InvokeRepeating("SingleFireShootAnim", currentGun.reloadTime, 0);

        // Reduce accuracy by a certain value 
        Vector3 reduceAccuracy = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                                Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));

        currentGun.currentAmmo--;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                
            }
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        if (currentGun.name == "GunPistol")
        {
            Ray ray = new Ray(firePoint.position, reduceAccuracy);
            if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
            {
                // Play particle representing where ray hit
                shotHitParitcleGameObject.transform.position = hit.point;
                shotHitParticle.Play();

                // Damage enemy if hit was an enemy
                EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
                if (enemyStats == null) return;

                enemyStats.TakeDamage(currentGun.damage);
            }
            
        }
        // shotgun
        else
        {
        Vector3 trajectory;
            for (int i = 0; i < 10; i++)
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
                    if (!(enemyStats == null))
                        enemyStats.TakeDamage(currentGun.damage);
                }
            }
        }

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                audioManager.Play("GunReload");
                isAnimInProgress = true;
                Reload();
            }
            if (currentGun.isAutomatic)
                CancelInvoke("AutomaticShoot");
        }
    }

    private void SingleFireShootAnim()
    {
        currentGun.gun.Play();
        isAnimInProgress = false;
        CancelInvoke("ShotgunShootAnim");
    }

    // Must called with InvokeRepeating
    // This function plays a reload animation and 'reloads' current gun
    private void Reload()
    {
        isAnimInProgress = true;
        InvokeRepeating("ReloadAnimCompress", 0f, currentGun.reloadTime / 6f);
    }

    /// <summary>
    /// This takes 6 iterations
    /// </summary>
    private void ReloadAnimCompress()
    {
        if (shapeModule.radius > currentGun.originalGunRadius * 10)
        {
            InvokeRepeating("ReloadAnimExpand", 0f, currentGun.reloadTime / 4);
            CancelInvoke("ReloadAnimCompress");
            return;
        }
        currentGun.gun.Stop();
        shapeModule.radius *= 1.5f;
        currentGun.gun.Play();
    }

    /// <summary>
    /// This takes 4 iterations
    /// </summary>
    private void ReloadAnimExpand()
    {
        Debug.Log("EXPAND");
        if (shapeModule.radius < currentGun.originalGunRadius)
        {
            shapeModule.radius = currentGun.originalGunRadius;
            isAnimInProgress = false;
            CancelInvoke("ReloadAnimExpand");

            // Reload gun
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
            // Reset ammo UI
            playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
            return;
        }
        currentGun.gun.Stop();
        shapeModule.radius *= .5f;
        currentGun.gun.Play();
    }

    // Change current weapon to secondary and vice versa
    // Dependencies: ChangeCurrentWeaponAnimation
    private void ChangeCurrentWeapon()
    {
    }

    /// <summary>
    /// Rotates current gun 90 degrees on x-axis, switches to secondary, and moves secondary (new current) down 90 degrees
    /// </summary>
    private void ChangeCurrentGunAnimation()
    {

    }

    /// <summary>
    /// Resets current gun and starts pick up weapon animation
    /// </summary>
    /// <param name="gunName"></param>
    public void PickUpWeapon(string gunName)
    {
    }

    
    /// <summary>
    /// plays animation where primary gun is lifted over head and seconday gun comes down 
    /// counter: current count of how many times function has been called
    /// gunName: name of gun to switch to 
    /// </summary>
    private IEnumerator PickWeapnAnimation(int counter, string gunName)
    {
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Instantiates grenade at GrenadeFirePoint and adds a forward force
    /// </summary>
    private void ThrowGrenade()
    {
        CurrentGrenades--;
        playerUI.SetGrenadeText(CurrentGrenades);
        Rigidbody grenade = Instantiate(grenadePrefab, grenadeFirePoint.position, transform.rotation).GetComponent<Rigidbody>();
        grenade.AddForce(grenadeFirePoint.forward * grenadeThrowForce * Time.deltaTime, ForceMode.Impulse);
    }
    // GRAPPLE ==============================================================================

    /// <summary>
    /// Turns off gravity, creates joints for grapple
    /// Call whenever player inputs for grapple
    /// Dependencies: DrawRope
    /// </summary>
    private void StartGrapple()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, whatIsGrapple ))
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
            StopGrapple();
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

    // END OF GRAPPLE ================================================================
}
