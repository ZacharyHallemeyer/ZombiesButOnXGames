using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private float maxHealth = 30;
    private float health;
    private float timeSinceLastHit = 0f, timeToHeal = 3f, healingIncrement = .1f;
    private bool healingFunctionActive = false;

    public int CurrentPoints { get; set; } = 0;

    // Scripts
    public HealthUIScript healthUI;
    public PointUIScript pointUI;
    public EnemyStats enemyStats;
    public PlayerShooting playerShooting;

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

    /// <summary>
    /// Starts lifeCycle (slower update function) and sets health
    /// </summary>
    private void Awake()
    {
        health = maxHealth;
        StartCoroutine(LifeCycle());
    }

    /// <summary>
    /// Slower update function
    /// This function calls itself infinitely after every second
    /// </summary>
    /// <returns></returns>
    public IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(1f);

        timeSinceLastHit += 1f;
        // Start healing player after a specified time after last time damage was taken
        // healingFunctionActive prevent mmultiple coroutines from occuring all at once
        if (health < maxHealth && timeSinceLastHit > timeToHeal && !healingFunctionActive)
            StartCoroutine(HealOverTime());
        StartCoroutine(LifeCycle());
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
    /// </summary>
    /// <param name="damage">variable to be subtracted from player healht</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthUI.ChangeHealthUI(health, maxHealth);
        timeSinceLastHit = 0f;

        // Stop healing coroutine if one is active
        if (healingFunctionActive)
        {
            StopCoroutine(healOverTime);
            healingFunctionActive = false;
        }

        if(health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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
            healthUI.ChangeHealthUI(health, maxHealth);
            healOverTime = StartCoroutine(HealOverTime());
        }
    }

    public void ChangeInPointValue()
    {
        pointUI.SetPointText(CurrentPoints);
    }
}
