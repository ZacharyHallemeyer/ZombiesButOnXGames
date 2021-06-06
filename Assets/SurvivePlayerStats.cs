using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivePlayerStats : MonoBehaviour
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


    // Player stats and Power up stats
    public int CurrentPoints { get; set; } = 0;
    public int TotalEnemiesKilled { get; set; } = 0;


    public Vector3 spawnPosition;
    // Environment generator script uses this vector as to where to spawn shop room

    // Scripts
    public EnemyStats enemyStats;
    public PlayerShooting playerShooting;
    public GameMenu gameMenu;
    public WaveSpawner waveSpawner;
    public GameManager gameManager;

    // UI
    public PlayerUIScript playerUI;

    // Gun models (GameObject) and muzzleflashes (ParticleSystem)
    public GameObject gunPistol;
    public GameObject gunSMG;
    public GameObject gunAR;
    public GameObject gunShotgun;
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem smgMuzzleFlash;
    public ParticleSystem arMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;

    /// <summary>
    /// Starts lifeCycle (slower update function) and sets health
    /// </summary>
    private void Awake()
    {
        CurrentPoints = 0;
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (waveSpawner == null)
            waveSpawner = FindObjectOfType<WaveSpawner>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
        CurrentPoints = (int) Time.time;
        playerUI.SetPointText(CurrentPoints);
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

    private void OnCollisionEnter(Collision collision)
    {
        // Take damage if collision was an enemy
        if (collision.gameObject.CompareTag("Enemy"))
            Death();
    }

    /// <summary>
    /// Reloads current scene and updates high score if necessary
    /// </summary>
    private void Death()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        if (playerData != null)
        {
            // minus 1 from current wave number because we want completed waves 
            switch (PlayerPrefs.GetString("Difficulty"))
            {
                case "Easy":
                    if (CurrentPoints > playerData.easyHighestTimeSurvived)
                        playerData.easyHighestTimeSurvived = CurrentPoints;

                    break;
                case "Hard":
                    if (CurrentPoints > playerData.hardHighestTimeSurvived)
                        playerData.hardHighestTimeSurvived = CurrentPoints;

                    break;
                case "Insane":
                    if (CurrentPoints > playerData.insaneHighestTimeSurvived)
                        playerData.insaneHighestTimeSurvived = CurrentPoints;

                    break;
                default:
                    Debug.LogError("Difficulty not found");
                    return;
            }
        }
        // if no current data
        else
        {
            // minus 1 from current wave number because we want completed waves 
            switch (PlayerPrefs.GetString("Difficulty"))
            {
                case "Easy":
                    playerData.easyHighestTimeSurvived = CurrentPoints;
                    break;
                case "Hard":
                    playerData.hardHighestTimeSurvived = CurrentPoints;
                    break;
                case "Insane":
                    playerData.insaneHighestTimeSurvived = CurrentPoints;
                    break;
                default:
                    Debug.LogError("Difficulty not found");
                    break;
            }

        }

        // Save new data
        SaveSystem.SavePlayerData(playerData);
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
