using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public class Wave
    {
        public GameObject enemyPrefab;
        public int health;
        public int count;
        public int speedForce;
        public int maxSpeed;
        public float rate;
    }


    // Prefabs
    public GameObject[] enemyPrefabs;
    public Vector3[] enemySpawnPoints;

    // Wave numerical variables
    public int waveNumber { get; private set; } = 0;
    public float timeBetweenWaves = 5f;
    public float searchCountdown = 1f;

    // Wave boolean variables
    public bool enemyAlive = false;
    public bool spawningWave = false;

    // UI
    public WaveUIScript waveUI;

    private void Start()
    {
        if (waveUI == null)
            waveUI = FindObjectOfType<WaveUIScript>();
        InvokeRepeating("LifeCycle", 2f, 1f);
    }   

    private void FillSpawnPoints()
    {
        int xCoord, yCoord, zCoord;
        ProceduralGenerationEnvironment environment = FindObjectOfType<ProceduralGenerationEnvironment>();
        enemySpawnPoints = new Vector3[4];
        yCoord = environment.outsideWallSize - 5;
        for(int i = 0; i < 4; i++)
        {
            xCoord = Random.Range(environment.startOfGroundX + 10, environment.endOfGroundX - 10);
            zCoord = Random.Range(environment.startOfGroundZ + 10, environment.endOfGroundZ - 10);
            enemySpawnPoints[i] = new Vector3(xCoord, yCoord, zCoord);
        }
    }

    private void LifeCycle()
    {
        if (!IsEnemyAlive() && !spawningWave)
        {
            waveNumber++;
            spawningWave = true;
            StartCoroutine(StartWave());
        }
    }

    /// <summary>
    /// Returns true if game object with tag "Enemy" is in the scene
    /// </summary>
    private bool IsEnemyAlive()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Starts Wave UI and starts spawning enemy after timeBetweenWave(float) seconds
    /// </summary>
    private IEnumerator StartWave()
    {
        //Debug.Log("Start wave is called");
        StartCoroutine(waveUI.NewWaveUI(waveNumber));
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnWave());
    }

    /// <summary>
    /// Spawn every enemy in current wave
    /// Dependencies: CreateWave()
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        //Debug.Log("Spawn Wave is called");
        Wave wave = CreateWave();

        if (enemySpawnPoints.Length == 0)
            FillSpawnPoints();

        for (int i = 0; i < wave.count; i++)
        {
            int spawnIndex = Random.Range(0, enemySpawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                                           enemySpawnPoints[spawnIndex],
                                           Quaternion.Euler(0, 0, 0));
            enemy.GetComponent<EnemyMovement>().MoveSpeed = wave.speedForce;
            enemy.GetComponent<EnemyMovement>().maxSpeed = wave.maxSpeed;
            enemy.GetComponent<EnemyStats>().MaxHealth = wave.health;
            yield return new WaitForSeconds(wave.rate);
        }

        // Spawning is completed
        spawningWave = false;
        yield break;
    }

    /// <summary>
    /// creates and returns new wave which will become progressivly harder as waveNumber increases
    /// </summary>
    /// <returns> Wave </returns>
    private Wave CreateWave()
    {
        Wave wave;
        //Debug.Log("Create Wave is called");
        // Easy Diff.
        if(PlayerPrefs.GetString("Difficulty").Equals("Easy"))
        {
            wave = new Wave
            {
                health = 40 + waveNumber * 2,
                count = 3 +  waveNumber * 2,
                speedForce = 1500 + waveNumber * 100,
                maxSpeed = 10 + waveNumber,
                rate = 1.5f - waveNumber / 100,
            };
        }
        // Hard Diff.
        else if(PlayerPrefs.GetString("Difficulty").Equals("Hard"))
        {
            wave = new Wave
            {
                health = 50 + waveNumber * 2,
                count = 5 + waveNumber * 2,
                speedForce = 2000 + waveNumber * 100,
                maxSpeed = 20 + waveNumber,
                rate = 1f - waveNumber / 100,
            };
        }
        // Insane Diff.
        else
        {
            wave = new Wave
            {
                health = 70 + waveNumber * 2,
                count = 7 + waveNumber * 2,
                speedForce = 3000 + waveNumber * 100,
                maxSpeed = 20 + waveNumber,
                rate = 1f - waveNumber / 100,
            };
        }

        return wave;
    }
}
