using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerStats : MonoBehaviour
{
    public class GunInformation
    {
        public string name;
        public GameObject gunContainer;
        public ParticleSystem muzzleFlash;
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

    // Components
    public Transform camPosition;

    // Number variables
    private float maxHealth = 30;
    private float health;
    private float timeSinceLastHit = 0f, timeToHeal = 3f, healingIncrement = .1f;
    private bool healingFunctionActive = false;

    public int CurrentLives { get; set; } = 1;
    public int CurrentPoints { get; set; } = 0;
    public int TotalEnemiesKilled { get; set; } = 0;

    public Vector3 spawnPosition;
    // Environment generator uses this vector as to where to spawn shop room
    public Vector3 shopSpawnPosition = new Vector3(0, -150, 0);

    // Scripts
    public EnemyStats enemyStats;
    public PlayerShooting playerShooting;
    public GameMenu gameMenu;
    public WaveSpawner waveSpawner;

    // UI
    public PlayerUIScript playerUI;

    private Coroutine healOverTime;

    // Gun models (GameObject) and muzzleflashes (ParticleSystem)
    public GameObject gunPistol;
    public GameObject gunSMG;
    public GameObject gunAR;
    public GameObject gunShotgun;
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem smgMuzzleFlash;
    public ParticleSystem arMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;

    // Interactable
    private InteractableObjects interactableObjects;
    public LayerMask whatIsInteractable;
    public bool shouldInteract;
    public RaycastHit interactableHit;

    /// <summary>
    /// Starts lifeCycle (slower update function) and sets health
    /// </summary>
    private void Awake()
    {
        if (waveSpawner == null)
            waveSpawner = FindObjectOfType<WaveSpawner>();
        spawnPosition = transform.position;
        InvokeRepeating("LifeCycle", 1f, 1f);
        health = maxHealth;

        // TESTING
        //CurrentPoints = 50000;
        //pointUI.SetPointText(CurrentPoints);
    }

    private void Update()
    {
        if (CheckForInteractableObject())
        {
            shouldInteract = Input.GetKeyDown(KeyCode.E);
            HandleInteractable();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            gameMenu.PauseGame();
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

    private void LifeCycle()
    {
        timeSinceLastHit += 1f;
        // Start healing player after a specified time after last time damage was taken
        // healingFunctionActive prevent mmultiple coroutines from occuring all at once
        if (health < maxHealth && timeSinceLastHit > timeToHeal && !healingFunctionActive)
            StartCoroutine(HealOverTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Take damage if collision was an enemy
        if(collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(enemyStats.BaseDamage);
        }
    }

    /// <summary>
    /// subtract damage from player health, update health UI, stop healing function
    /// and restarts scene if player health is less than or equal to zero
    /// Dependencies: Death
    /// </summary>
    /// <param name="damage">variable to be subtracted from player healht</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        playerUI.ChangeHealthUI(health, maxHealth);
        timeSinceLastHit = 0f;

        // Stop healing coroutine if one is active
        if (healingFunctionActive)
        {
            StopCoroutine(healOverTime);
            healingFunctionActive = false;
        }

        if(health <= 0)
        {
            CurrentLives--;
            if (CurrentLives <= 0)
                Death();
            else
            {
                // Reset player health and teleport to spawn location
                playerUI.SetLivesText(CurrentLives);
                health = maxHealth;
                playerUI.ChangeHealthUI(health, maxHealth);
                transform.position = spawnPosition;
            }
        }
    }

    /// <summary>
    /// Reloads current scene and updates high score if necessary
    /// </summary>
    private void Death()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        if(playerData != null)
        {
            // minus 1 from current wave number because we want completed waves 
            if(waveSpawner.waveNumber - 1 > playerData.highScoreWave)
                playerData.highScoreWave = waveSpawner.waveNumber - 1;
            if (CurrentPoints > playerData.highestPoints)
                playerData.highestPoints = CurrentPoints;
            Debug.Log("Save from old file");
            SaveSystem.SavePlayerData(playerData.highScoreWave, TotalEnemiesKilled, playerData.highestPoints);
        }
        // if no current data
        else
            SaveSystem.SavePlayerData(waveSpawner.waveNumber, TotalEnemiesKilled, CurrentPoints);

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Heals player over time. It can be stopped in TakeDamage if the function is called while HealOverTime is active
    /// rate: 10 health every one second
    /// </summary>
    /// <returns>waits for .1 seconds then recursively calls itself</returns>
    public IEnumerator HealOverTime()
    {
        healingFunctionActive = true;
        yield return new WaitForSeconds(.1f);
        if(health >= maxHealth)
            healingFunctionActive = false;
        else
        {
            health += healingIncrement;
            playerUI.ChangeHealthUI(health, maxHealth);
            healOverTime = StartCoroutine(HealOverTime());
        }
    }

    public void ChangeInPointValue()
    {
        playerUI.SetPointText(CurrentPoints);
    }

    public bool CheckForInteractableObject()
    {
        Ray ray = new Ray(transform.position, camPosition.forward);
        if (Physics.Raycast(ray, out interactableHit, 10f, whatIsInteractable))
            return true;
        return false;
    }

    private void HandleInteractable()
    {
        if (interactableObjects == null)
            interactableObjects = FindObjectOfType<InteractableObjects>();

        switch (interactableHit.collider.tag)
        {
            case "MysteryBox":
                if (!(interactableObjects.mysteryBox.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.mysteryBox.name);
                
                if (!shouldInteract) return;
                
                if (interactableObjects.IsWeaponSpawned)
                {
                    playerShooting.PickUpWeapon(interactableObjects.DestroyGun());
                }
                else if (CurrentPoints >= interactableObjects.mysteryBox.price)
                {
                    CurrentPoints -= interactableObjects.mysteryBox.price;
                    ChangeInPointValue();
                    interactableObjects.SpawnRandomWeapon();
                }
                break;

            case "UpgradeGrapple":
                if (!(interactableObjects.upgradeGrapple.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.upgradeGrapple.name);
                
                if (!shouldInteract) break;
                
                if (CurrentPoints >= interactableObjects.upgradeGrapple.price)
                {
                    CurrentPoints -= interactableObjects.upgradeGrapple.price;
                    ChangeInPointValue();
                    interactableObjects.UpgradeGrapple();
                }

                break;

            case "UpgradeDamage":
                if (!(interactableObjects.upgradeDamage.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.upgradeDamage.name);
                
                if (!shouldInteract) break;

                if (CurrentPoints >= interactableObjects.upgradeDamage.price)
                {
                    CurrentPoints -= interactableObjects.upgradeDamage.price;
                    ChangeInPointValue();
                    interactableObjects.UpgradeDamage();
                }
                break;

            case "ShockWaveIncrease":
                if (!(interactableObjects.shockWaveIncrease.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.shockWaveIncrease.name);
                
                if (!shouldInteract) break;

                if (CurrentPoints >= interactableObjects.shockWaveIncrease.price)
                {
                    CurrentPoints -= interactableObjects.shockWaveIncrease.price;
                    ChangeInPointValue();
                    interactableObjects.IncreaseShockWaveItem();
                    playerUI.SetShockWaveText(playerShooting.CurrentShockWaves);
                }

                break;

            case "ExtraLife":
                if (!(interactableObjects.extraLife.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.extraLife.name);
                
                if (!shouldInteract) break;

                if (CurrentPoints >= interactableObjects.extraLife.price)
                {
                    CurrentPoints -= interactableObjects.extraLife.price;
                    ChangeInPointValue();
                    interactableObjects.IncreaseAmountOfLives();
                    playerUI.SetLivesText(CurrentLives);
                }

                break;

            case "RefillAmmo":
                if (!(interactableObjects.refillAmmo.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.refillAmmo.name);
                
                if (!shouldInteract) break;

                if(CurrentPoints >= interactableObjects.refillAmmo.price)
                {
                    CurrentPoints -= interactableObjects.refillAmmo.price;
                    ChangeInPointValue();
                    interactableObjects.RefillAmmo();
                }

                break;

            case "GrenadeIncrease":
                if (!(interactableObjects.grenadeIncrease.isInteractable)) break;
                interactableObjects.ShowUI(interactableObjects.grenadeIncrease.name);

                if (!shouldInteract) break;

                if (CurrentPoints >= interactableObjects.grenadeIncrease.price)
                {
                    CurrentPoints -= interactableObjects.grenadeIncrease.price;
                    ChangeInPointValue();
                    interactableObjects.GrenadeIncrease();
                }

                break;

            case "ExitShop":
                interactableObjects.ShowUI(interactableObjects.exitShop.name);

                if (!shouldInteract) break;

                interactableObjects.ExitShop();

                break;

            default:
                break;
        }
    }
}
