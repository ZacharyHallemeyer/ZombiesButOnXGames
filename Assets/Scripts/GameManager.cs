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
    public GenerateWaveEnvironment environment;

    public bool playerShopedSpawnedThisWave = false;

    // Prefabs
    public GameObject[] powerUps;

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

    /// <summary>
    /// Spawn random power up on a rooftop
    /// Dependencies: GenerateWaveEnvironment class
    /// </summary>
    public void SpawnPowerUp()
    {
        int index = Random.Range(0, powerUps.Length);
        GameObject powerUp = Instantiate(powerUps[index],
                             environment.powerUpSpawnPositions[Random.Range(0, environment.powerUpSpawnPositions.Length)],
                             powerUps[index].transform.rotation);
        StartCoroutine(SelfDestruct(powerUp));
    }


    /// <summary>
    /// Wait one minute of scaled time then destroyed game object passed in parems
    /// </summary>
    private IEnumerator SelfDestruct(GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(60);
        Destroy(objectToDestroy);
    }
}
