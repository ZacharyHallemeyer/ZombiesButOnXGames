using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivePlayerStats : MonoBehaviour
{
    // Components
    public Transform camPosition;


    // Player stats and Power up stats
    public int CurrentPoints { get; set; } = 0;
    public int TotalEnemiesKilled { get; set; } = 0;


    public Vector3 spawnPosition;
    // Environment generator script uses this vector as to where to spawn shop room

    // Scripts
    private EnemyStats enemyStats;
    public SurvivalPlayerShoot playerShooting;
    public SurvivalGameMenu gameMenu;

    // UI
    public PlayerUIScript playerUI;

    /// <summary>
    /// Starts lifeCycle (slower update function) and sets health
    /// </summary>
    private void Awake()
    {
        CurrentPoints = 0;
        spawnPosition = transform.position;
    }

    private void Update()
    {
        CurrentPoints = (int) Time.timeSinceLevelLoad;
        playerUI.SetPointText(CurrentPoints);
        if (Input.GetKeyDown(KeyCode.Escape))
            gameMenu.PauseGame();
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
