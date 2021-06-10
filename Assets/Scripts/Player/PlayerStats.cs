using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    // Components
    public Transform camPosition;

    // Health
    private float maxHealth = 30;
    private float health;
    private float timeSinceLastHit = 0f, timeToHeal = 3f, healingIncrement = .1f, maxTimeInvincible = 1f;
    private bool healingFunctionActive = false;

    // Player stats and Power up stats
    public int CurrentLives { get; set; } = 2;
    public int CurrentPoints { get; set; } = 0;
    public int TotalEnemiesKilled { get; set; } = 0;
    public int PointMultiplier { get; set; } = 1;
    private int killsToNextPowerUp = 20;


    public Vector3 spawnPosition;
    // Environment generator script uses this vector as to where to spawn shop room
    public Vector3 shopSpawnPosition = new Vector3(0, -150, 0);

    // Scripts
    public PlayerShooting playerShooting;
    public GameMenu gameMenu;
    private EnemyStats enemyStats;
    private WaveSpawner waveSpawner;
    private GameManager gameManager;
    private InputMaster inputMaster;

    // UI
    public PlayerUIScript playerUI;

    private Coroutine healOverTime;

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
        SetControls setControls = gameObject.AddComponent<SetControls>();
        inputMaster = new InputMaster();
        inputMaster = setControls.SetPlayerControls(inputMaster);
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (waveSpawner == null)
            waveSpawner = FindObjectOfType<WaveSpawner>();
        spawnPosition = transform.position;
        health = maxHealth;
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

    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;
        // Start healing player after a specified time after last time damage was taken
        // healingFunctionActive prevent mmultiple coroutines from occuring all at once
        if (health < maxHealth && timeSinceLastHit > timeToHeal && !healingFunctionActive)
            StartCoroutine(HealOverTime());

        if (TotalEnemiesKilled > killsToNextPowerUp)
        {
            gameManager.SpawnPowerUp();
            killsToNextPowerUp += killsToNextPowerUp / 2;
        }

        if (CheckForInteractableObject())
        {
            shouldInteract = inputMaster.Player.Interact.triggered;
            HandleInteractable();
        }
        if (inputMaster.Player.Escape.triggered)
            gameMenu.PauseGame();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Take damage if collision was an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(collision.gameObject.GetComponent<EnemyStats>().BaseDamage);
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
        // Player can not be damaged until maxTimeInvicible amount of seconds
        if (timeSinceLastHit < maxTimeInvincible) return;

        health -= damage;
        playerUI.ChangeHealthUI(health, maxHealth);
        timeSinceLastHit = 0f;

        // Stop healing coroutine if one is active
        if (healingFunctionActive)
        {
            StopCoroutine(healOverTime);
            healingFunctionActive = false;
        }

        if (health <= 0)
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
                playerUI.ChangeDeathUI(CurrentLives);
            }
        }
    }

    /// <summary>
    /// Reloads current scene and updates high score if necessary
    /// </summary>
    private void Death()
    {
        FindObjectOfType<AudioManager>().StopAllSoundsBesideMusic();
        SaveScore();

        // These are used for death screen
        PlayerPrefs.SetInt("LastWaveNumber", waveSpawner.waveNumber - 1);
        PlayerPrefs.SetInt("LastKillNumber", TotalEnemiesKilled);
        PlayerPrefs.SetInt("LastPointNumber", CurrentPoints);

        // Reload current scene
        SceneManager.LoadScene("DeathScreenWave");
    }

    public void SaveScore()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        if (playerData != null)
        {
            // minus 1 from current wave number because we want completed waves 
            switch (PlayerPrefs.GetString("Difficulty"))
            {
                case "Easy":
                    if (waveSpawner.waveNumber - 1 > playerData.easyHighScoreWave)
                        playerData.easyHighScoreWave = waveSpawner.waveNumber - 1;
                    if (CurrentPoints > playerData.easyHighestPoints)
                        playerData.easyHighestPoints = CurrentPoints;
                    playerData.easyTotalEnemiesKilled += TotalEnemiesKilled;

                    break;
                case "Hard":
                    if (waveSpawner.waveNumber - 1 > playerData.hardHighScoreWave)
                        playerData.hardHighScoreWave = waveSpawner.waveNumber - 1;
                    if (CurrentPoints > playerData.hardHighestPoints)
                        playerData.hardHighestPoints = CurrentPoints;
                    playerData.hardTotalEnemiesKilled += TotalEnemiesKilled;

                    break;
                case "Insane":
                    if (waveSpawner.waveNumber - 1 > playerData.insaneHighScoreWave)
                        playerData.insaneHighScoreWave = waveSpawner.waveNumber - 1;
                    if (CurrentPoints > playerData.easyHighestPoints)
                        playerData.insaneHighestPoints = CurrentPoints;
                    playerData.insaneTotalEnemiesKilled += TotalEnemiesKilled;

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
                    playerData.easyHighScoreWave = waveSpawner.waveNumber - 1;
                    playerData.easyHighestPoints = CurrentPoints;
                    playerData.easyTotalEnemiesKilled = TotalEnemiesKilled;

                    break;
                case "Hard":
                    playerData.hardHighScoreWave = waveSpawner.waveNumber - 1;
                    playerData.hardHighestPoints = CurrentPoints;
                    playerData.hardTotalEnemiesKilled = TotalEnemiesKilled;

                    break;
                case "Insane":
                    playerData.insaneHighScoreWave = waveSpawner.waveNumber - 1;
                    playerData.insaneHighestPoints = CurrentPoints;
                    playerData.insaneTotalEnemiesKilled = TotalEnemiesKilled;

                    break;
                default:
                    Debug.LogError("Difficulty not found");
                    break;
            }

        }
        SaveSystem.SavePlayerData(playerData);
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
        if (health >= maxHealth)
            healingFunctionActive = false;
        else
        {
            health += healingIncrement;
            playerUI.ChangeHealthUI(health, maxHealth);
            healOverTime = StartCoroutine(HealOverTime());
        }
    }

    /// <summary>
    /// Changes the point value UI
    /// Dependencies: PlayerUIScript class
    /// </summary>
    public void ChangeInPointValue()
    {
        playerUI.SetPointText(CurrentPoints);
    }

    /// <summary>
    /// Sends an ray out 10 units forward relative to player camera and return true if a game object with layer 
    /// 'Interactable' is hit by ray
    /// </summary>
    /// <returns></returns>
    public bool CheckForInteractableObject()
    {
        Ray ray = new Ray(transform.position, camPosition.forward);
        if (Physics.Raycast(ray, out interactableHit, 10f, whatIsInteractable))
            return true;
        return false;
    }

    /// <summary>
    /// If CheckForInteractableObject returns true then this function should be called.
    /// It handles both calling the UI functions as well as handling calling the correct functions if player 
    /// desires to interact with said object. For instance, if player wishes to use mystery box, then this function will
    /// call the correct functions to spawn a new 'random' weapon
    /// Dependencies: InteractableObjects class
    /// </summary>
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

                if (CurrentPoints >= interactableObjects.refillAmmo.price)
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

    /// <summary>
    /// Activated from PowerUps class
    /// </summary>
    private void DeactivateDoublePoints()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.PointMultiplier = 1;
        CancelInvoke("DeactivateDoublePoints");
    }
}
