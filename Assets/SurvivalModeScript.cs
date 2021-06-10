using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalModeScript : MonoBehaviour
{
    public GameObject survivalEnemyPrefab;

    public int maxNumOfEnemies;
    public int rate;
    public int maxSpeed;
    public int speedForce;

    private float timeOfLastSpawn;
    private Vector3 spawnPosition = new Vector3(0, 110, 0);

    // Start is called before the first frame update
    void Start()
    {
        // Make sure to spawn enemy at start
        timeOfLastSpawn = -11;
        switch (PlayerPrefs.GetString("Difficulty"))
        {
            case "Easy":
                maxNumOfEnemies = 5;
                rate = 30;
                maxSpeed = 30;
                speedForce = 1500;

                break;
            case "Hard":
                maxNumOfEnemies = 15;
                rate = 25;
                maxSpeed = 40;
                speedForce = 2000;

                break;
            case "Insane":
                maxNumOfEnemies = 30;
                rate = 10;
                maxSpeed = 50;
                speedForce = 3000;

                break;
            default:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeSinceLevelLoad - timeOfLastSpawn > rate)
            SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        timeOfLastSpawn = Time.timeSinceLevelLoad;
        GameObject enemy = Instantiate(survivalEnemyPrefab, spawnPosition, Quaternion.Euler(0, 0, 0));
        enemy.GetComponent<SurviveEnemyMovement>().MoveSpeed = speedForce;
        enemy.GetComponent<SurviveEnemyMovement>().maxSpeed = maxSpeed;
        // Stop update function once all enemies are spawned
        if (FindObjectsOfType<SurviveEnemyMovement>().Length >= maxNumOfEnemies)
            enabled = false;
    }
}
