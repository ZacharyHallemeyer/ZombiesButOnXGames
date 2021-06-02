using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Containers
    public GameObject gameEnvironment;      // gameEnvironment holds instatiated game environment
    public GameObject shopRoom;             // shopRoom holds instiated shop room

    // Scripts
    public PlayerStats playerStats;
    public LoadingScreenScript loadingScreen;

    public bool playerShopedSpawnedThisWave = false;

    // Set instances 
    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if (loadingScreen == null)
            loadingScreen = FindObjectOfType<LoadingScreenScript>();
    }

    /// <summary>
    /// Teleport player to main room
    /// </summary>
    public void ChangeToMainRoom()
    {
        loadingScreen.TurnOnLoadingScreen();
        gameEnvironment.SetActive(true);
        shopRoom.SetActive(false);
        playerStats.transform.position = playerStats.spawnPosition;
        loadingScreen.InvokeRepeating("TurnOffLoadingScreen", .5f, .01f);
    }

    /// <summary>
    /// Teleport player to shop room
    /// </summary>
    public void ChangeToShopRoom()
    {
        loadingScreen.TurnOnLoadingScreen();
        playerShopedSpawnedThisWave = true;
        gameEnvironment.SetActive(false);
        shopRoom.SetActive(true);
        playerStats.transform.position = playerStats.shopSpawnPosition;
        loadingScreen.InvokeRepeating("TurnOffLoadingScreen", .5f, .01f);
    }
}
