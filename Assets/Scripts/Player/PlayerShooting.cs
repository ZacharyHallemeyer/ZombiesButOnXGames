using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player shooting and grappling
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    // General Variables

    // set up in Awake
    private InputMaster inputMaster;
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
    private int maxGrappleRopeDivider = 15;
    private bool grappleContactMade = false;

    // Gun Variables =======================

    public class GunInformation
    {
        public string name;
        public GameObject gunContainer;
        public ParticleSystem bullet;
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
        public float rightHandPosition;
        public float leftHandPosition;
        public bool isAutomatic;
    }

    public Dictionary<string, GunInformation> allGunInformation { get; private set; } = new Dictionary<string, GunInformation>();

    public float timeSinceLastShoot = 5f;       // Garbage value

    // Animation variables
    public int animationCounter = 0;
    public bool isAnimInProgress;
    public bool isAudioPlaying = false;

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
    private ParticleSystem.ShapeModule shapeModule;

    public LayerMask whatIsShootable;
    public int currentParticleIndex;
    public bool isShooting = false;

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
    private int grenadeThrowForce = 2500;

    // Item variables
    public int CurrentShockWaves { get; set; } = 3;
    public PlayerItems playerItems;

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

    private void Start()
    {
        // Set up items and grenades 
        playerUI.SetGrenadeText(CurrentGrenades);
        playerUI.SetShockWaveText(CurrentShockWaves);

        // Set up guns
        SetGunInformation();

        int index = 0;
        gunNames = new string[allGunInformation.Count];
        foreach (string str in allGunInformation.Keys)
        {
            gunNames[index] = str;
            index++;
        }

        do
        {
            currentGun = allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
            secondaryGun = allGunInformation[gunNames[Random.Range(0, gunNames.Length)]];
        } while (currentGun.name.Equals(secondaryGun.name));

        shapeModule = currentGun.gun.shape;
        currentGun.gunContainer.SetActive(true);
        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        // Set up grapple
        timeLeftToGrapple = maxGrappleTime;
        playerUI.SetMaxGrapple(maxGrappleTime);
        lineRender = GetComponent<LineRenderer>();

        StartCoroutine(SetAudioManager());
    }

    public void OnEnable()
    {
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        inputMaster.Disable();
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
            bullet = pistolMuzzleFlash,
            gun = pistol,
            originalGunRadius = pistol.shape.radius,
            magSize = 6,
            ammoIncrementor = 60,
            reserveAmmo = 60,
            currentAmmo = 6,
            damage = 30,
            fireRate = .7f,
            accuaracyOffset = .001f,
            reloadTime = 1f,
            range = 1000f,
            rightHandPosition = -.3f,
            leftHandPosition = -1.5f,
            isAutomatic = false,
        };

        allGunInformation["GunSMG"] = new GunInformation
        {
            name = "GunSMG",
            gunContainer = gunSMG,
            bullet = smgMuzzleFlash,
            gun = smg,
            originalGunRadius = smg.shape.radius,
            magSize = 30,
            ammoIncrementor = 300,
            reserveAmmo = 300,
            currentAmmo = 30,
            damage = 10,
            fireRate = .1f,
            accuaracyOffset = .025f,
            reloadTime = 1f,
            range = 1000f,
            rightHandPosition = -.3f,
            leftHandPosition = -1.5f,
            isAutomatic = true,
        };

        allGunInformation["GunAR"] = new GunInformation
        {
            name = "GunAR",
            gunContainer = gunAR,
            bullet = arMuzzleFlash,
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
            rightHandPosition = -.3f,
            leftHandPosition = -1.5f,
            isAutomatic = true,
        };

        allGunInformation["GunShotgun"] = new GunInformation
        {
            name = "GunShotgun",
            gunContainer = gunShotgun,
            bullet = shotgunMuzzleFlash,
            gun = shotgun,
            originalGunRadius = shotgun.shape.radius,
            magSize = 8,
            ammoIncrementor = 80,
            reserveAmmo = 80,
            currentAmmo = 8,
            damage = 10,
            fireRate = .7f,
            accuaracyOffset = .1f,
            reloadTime = 1f,
            range = 75f,
            rightHandPosition = -.3f,
            leftHandPosition = -.3f,
            isAutomatic = false,
        };
    }

    private void Update()
    {
        // Handle grapple =========================================
        // Player must have more than 25% of grapple left to start grapple
        if(!IsGrappling)
        {
            if(inputMaster.Player.Grapple.ReadValue<float>() != 0 && releasedGrappleControlSinceLastGrapple)
            {
                if (timeLeftToGrapple > (maxGrappleTime * .25))
                {
                    releasedGrappleControlSinceLastGrapple = false;
                    StartGrapple();
                }
                else
                    playerUI.GrappleOutOfBoundsUI();
            }
        }
        if(!releasedGrappleControlSinceLastGrapple)
        {
            if (inputMaster.Player.Grapple.ReadValue<float>() == 0 
                    || Mathf.Abs((player.position - GrapplePoint).magnitude) < 5f 
                    && IsGrappling)
            {
                releasedGrappleControlSinceLastGrapple = true;
                StopGrapple();
            }
        }

        // Handle gun shooting ====================================

        // Handle items
        if (inputMaster.Player.Special.triggered && CurrentShockWaves > 0)
        {
            CurrentShockWaves--;
            playerUI.SetShockWaveText(CurrentShockWaves);
            playerItems.StartShockWave();
        }

        // Do not shoot if animation is in progress
        if (isAnimInProgress) return;

        // Switching between primary and secondary by using mouse scroll wheel or '1' on keyboard
        if (inputMaster.Player.SwitchWeaponMouseWheel.ReadValue<Vector2>().y != 0
            || inputMaster.Player.SwitchWeaponButton.triggered)
            ChangeCurrentWeapon();
            
        if(!isShooting)
        {
            // Shoot
            if (inputMaster.Player.Shoot.ReadValue<float>() != 0 && currentGun.currentAmmo > 0)
            {
                if (currentGun.isAutomatic)
                {
                    isShooting = true;
                    ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
                    rot.enabled = true;
                    InvokeRepeating("AutomaticShoot", 0f, currentGun.fireRate);
                }
                else
                    SingleFireShoot();
            }
        }
        else if (inputMaster.Player.Shoot.ReadValue<float>() == 0)
        {
            // Stop shooting
            if (currentGun.isAutomatic)
            {
                StopAutomaticShoot();
                isShooting = false;
            }
        }
        // Reload
        if (currentGun.currentAmmo < currentGun.magSize && inputMaster.Player.Reload.triggered 
                    && currentGun.reserveAmmo > 0)
        {
            StopAutomaticShoot();
            Reload();
        }

        // Handle Grenades
        if (inputMaster.Player.Grenade.triggered && CurrentGrenades > 0)
            ThrowGrenade();
    }

    // Called after Update function
    private void LateUpdate()
    {
        if (IsGrappling)
        {
            if (grappleContactMade)
            {
                DrawRope();
                ContinueGrapple();
            }
            else
                DrawIncompleteRope();
        }
    }

    // GUNS =================================================================================

    /// <summary>
    /// fires a raycast from firepoint and damages if ray hits an enemy
    /// </summary>
    private void AutomaticShoot()
    {
        if (!isAudioPlaying)
        {
            audioManager.Play(currentGun.name);
            isAudioPlaying = true;
        }
        currentGun.bullet.Play();
        // Reduce accuracy by a certain value 
        Vector3 reduceAccuracy = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                                Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));

        currentGun.currentAmmo--;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            StopAutomaticShoot();
            if (currentGun.reserveAmmo > 0)
            {
                Reload();
            }
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        Ray ray = new Ray(firePoint.position, reduceAccuracy);
        if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
        {
            // Play shot hit particle
            shotHitParitcleGameObject.transform.position = hit.point;
            shotHitParticle.Play();

            // Damage enemy if hit was an enemy
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
                enemyStats.TakeDamage(currentGun.damage);


        }
    }

    /// <summary>
    /// Stops AutomaticShoot function and stops current gun from rotating (automatic guns rotate while firing)
    /// </summary>
    private void StopAutomaticShoot()
    {
        ParticleSystem.RotationOverLifetimeModule rot = currentGun.gun.rotationOverLifetime;
        rot.enabled = false;
        if (isAudioPlaying)
        {
            InvokeRepeating("SoundFadeOut", 0f, .1f);
        }
        isShooting = false;
        CancelInvoke("AutomaticShoot");
    }

    /// <summary>
    /// fires 10 raycasts from firepoint and damages if ray hits an enemy
    /// </summary>
    private void SingleFireShoot()
    {
        isAnimInProgress = true;
        audioManager.Play(currentGun.name);
        currentGun.gun.Stop();
        currentGun.bullet.Play();
        InvokeRepeating("SingleFireShootAnim", currentGun.fireRate, 0);

        // Reduce accuracy by a certain value 
        Vector3 reduceAccuracy = firePoint.forward + new Vector3(Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset),
                                                                Random.Range(-currentGun.accuaracyOffset, currentGun.accuaracyOffset));

        currentGun.currentAmmo--;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                Reload();
            }
        }

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);

        if (currentGun.name == "GunPistol")
        {
            Ray ray = new Ray(firePoint.position, reduceAccuracy);
            if (Physics.Raycast(ray, out RaycastHit hit, currentGun.range, whatIsShootable))
            {
                // Play shot hit particle
                shotHitParitcleGameObject.transform.position = hit.point;
                shotHitParticle.Play();

                // Damage enemy if hit was an enemy
                EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
                if (enemyStats != null)
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
                    if (enemyStats != null)
                        enemyStats.TakeDamage(currentGun.damage);
                }
            }
        }

        isShooting = false;

        // Reload if current ammo is zero
        if (currentGun.currentAmmo <= 0)
        {
            if (currentGun.reserveAmmo > 0)
            {
                Reload();
            }
        }
    }

    /// <summary>
    /// Decreases the sound of current gun over time. Should be called with stoping automatic weapons
    /// </summary>
    public void SoundFadeOut()
    {
        if (audioManager.FadeOut(currentGun.name))
        {
            CancelInvoke("SoundFadeOut");
            isAudioPlaying = false;
            audioManager.Stop(currentGun.name);
            audioManager.ResetSound(currentGun.name);
        }
    }

    /// <summary>
    /// Plays current gun's bullet particly system. Must be called with invoke repeating with the wait time
    /// as current gun's reload time
    /// </summary>
    private void SingleFireShootAnim()
    {
        currentGun.gun.Play();
        isAnimInProgress = false;
        CancelInvoke("SingleFireShootAnim");
    }

    // Must called with InvokeRepeating
    // This function plays a reload animation and 'reloads' current gun
    private void Reload()
    {
        isAnimInProgress = true;
        audioManager.Play("GunReload");
        InvokeRepeating("ReloadAnimCompress", 0f, currentGun.reloadTime / 6f);
    }

    /// <summary>
    /// This takes 6 iterations. Expands current gun by increasing particle system radius
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
    /// This takes 4 iterations. Compress current gun by decreasing particle system radius
    /// </summary>
    private void ReloadAnimExpand()
    {
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

    /// <summary>
    /// Change current weapon to secondary and vice versa
    /// Dependencies: ChangeCurrentWeaponAnimation
    /// </summary>
    private void ChangeCurrentWeapon()
    {
        // Prevents automatic weapon sounds to continue infinitly
        if (isAudioPlaying)
        {
            isAudioPlaying = false;
            audioManager.Stop(currentGun.name);
        }
        audioManager.Play("GunReload");
        StopAutomaticShoot();
        isAnimInProgress = true;

        GunInformation temp = currentGun;
        currentGun = secondaryGun;
        secondaryGun = temp;

        playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
        InvokeRepeating("ChangeCurrentGunAnimationExapnd", 0, 1f / 10f);
    }

    /// <summary>
    /// Expands current gun by increasing particle system radius
    /// Has be called with invoke repeating and takes 10 iterations to finished. Divide total time by 10 for time repeat 
    /// Dependencies: ChangeCurrentGunAnimationCompress
    /// </summary>
    private void ChangeCurrentGunAnimationExapnd()
    {
        if (animationCounter >= 10)
        {
            animationCounter = 0;
            CancelInvoke("ChangeCurrentGunAnimationExapnd");
            shapeModule.radius = currentGun.originalGunRadius;

            secondaryGun.gunContainer.SetActive(false);
            currentGun.gunContainer.SetActive(true);
            shapeModule = currentGun.gun.shape;
            shapeModule.radius *= 20;

            if (playerMovement.crouching)
            {
                playerMovement.InvokeRepeating("StartCrouchAnimation", 0, .1f / playerMovement.crouchGunDegree);
                secondaryGun.gunContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            InvokeRepeating("ChangeCurrentGunAnimationCompress", 0f, 1f / 10);
            return;
        }
        // else
        secondaryGun.gun.Stop();
        shapeModule.radius *= 2;
        secondaryGun.gun.Play();

        animationCounter++;
    }

    /// <summary>
    /// Compress current gun by decreasing particle system radius
    /// Has be called with invoke repeating and takes 10 iterations to finished. Divide total time by 10 for time repeat 
    /// </summary>
    private void ChangeCurrentGunAnimationCompress()
    {
        if (animationCounter >= 10)
        {
            animationCounter = 0;
            CancelInvoke("ChangeCurrentGunAnimationCompress");

            shapeModule.radius = currentGun.originalGunRadius;
            isAnimInProgress = false;
            return;
        }
        // else
        currentGun.gun.Stop();
        shapeModule.radius /= 2;
        currentGun.gun.Play();

        animationCounter++;
    }

    /// <summary>
    /// Resets current gun and starts pick up weapon animation
    /// Dependencies: PickWeapnAnimationExpand
    /// </summary>
    public void PickUpWeapon(string gunName)
    {
        isAnimInProgress = true;
        StartCoroutine(PickWeapnAnimationExpand(0, gunName));
    }

    /// <summary>
    /// Take 10 iterations and switches current gun with gun cooresponding to gunName (parem)
    /// Dependcies: ChangeCurrentGunAnimationCompress
    /// </summary>
    private IEnumerator PickWeapnAnimationExpand(int counter, string gunName)
    {
        yield return new WaitForSeconds(1f / 10f);
        if (counter >= 10)
        {
            animationCounter = 0;
            shapeModule.radius = currentGun.originalGunRadius;

            currentGun.gunContainer.SetActive(false);
            currentGun = allGunInformation[gunName];

            shapeModule = currentGun.gun.shape;
            shapeModule.radius *= 20;
            currentGun.gunContainer.SetActive(true);

            if (playerMovement.crouching)
            {
                playerMovement.InvokeRepeating("StartCrouchAnimation", 0, .1f / playerMovement.crouchGunDegree);
                secondaryGun.gunContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            playerUI.ChangeGunUIText(currentGun.currentAmmo, currentGun.reserveAmmo);
            InvokeRepeating("ChangeCurrentGunAnimationCompress", 0f, 1f / 10);
            yield break;
        }
        // else
        currentGun.gun.Stop();
        shapeModule.radius *= 2;
        currentGun.gun.Play();

        StartCoroutine(PickWeapnAnimationExpand(counter + 1, gunName));
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

    private void ShockWave()
    {
        CurrentShockWaves--;
        playerUI.SetShockWaveText(CurrentShockWaves);
        playerItems.StartShockWave();
    }

    // END OF GUNS ==========================================================================

    // GRAPPLE ==============================================================================

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

            audioManager.Play("Grapple");

            // reset grapple animation variables
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
        else
            playerUI.GrappleOutOfBoundsUI();
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

    private void DrawIncompleteRope()
    {
        if (!IsGrappling) return;
        currentGrapplePoint = (GrapplePoint - grappleFirePoint.position) / grappleRopeDivider + grappleFirePoint.position;
        grappleRopeDivider--;

        lineRender.SetPosition(0, grappleFirePoint.position);
        lineRender.SetPosition(1, currentGrapplePoint);
        if (grappleRopeDivider <= 1)
            grappleContactMade = true;
    }

    /// <summary>
    /// Erases grapple rope, turns player gravity on, and destroys joint
    /// </summary>
    private void StopGrapple()
    {
        if(!IsGrappleRecoveryInProgress)
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
