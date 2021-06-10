using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSurvivalEnvironment : MonoBehaviour
{
    // Prefabs
    public GameObject buildingPrefab;
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject roofPrefab;
    public GameObject groundLightPrefab;
    public GameObject wallLightRightPrefab;
    public GameObject wallLightLeftPrefab;
    public GameObject wallLightForwardPrefab;
    public GameObject wallLightBackwardPrefab;
    public GameObject sunPrefab;
    public GameObject environmentContainer;

    // Materials 
    public Material[] sunMaterials;

    // Container (ground, outside walls, roof)
    public int groundMinXSize, groundMaxXSize, groundMinZSize, groundMaxZSize;
    public int groundXSize, groundZSize, startOfGroundX, endOfGroundX, startOfGroundZ, endOfGroundZ;
    public int outsideWallsMinYSize, outsideWallsMaxYSize, outsideWallSize;

    // Buidlings
    public float buildingMinX, buildingMaxX;
    public float buildingMinY, buildingMaxY;
    public float buildingMinZ, buildingMaxZ;
    public float spaceBetweenBuildingsMultiplier = 1.2f;
    public int buildingMinCount, buildingMaxCount, buildingCount;

    // Perlin Noise
    public int perlinOffsetMin, perlinOffsetMax, xOffset, yOffset, scale;

    // Lights
    public int groundLightMinCount, groundLightMaxCount;
    public int perWallLightMinCount, perWallLightMaxCount;
    public int spaceBetweenWallAndLight;
    public int sunCountMin, sunCountMax, sunRadiusMin, sunRadiusMax;

    // Start is called before the first frame update
    void Start()
    {
        SpawnContainer();
        SpawnBuildings();
        SpawnLights();
        SpawnSuns();
    }

    /// <summary>
    /// Spawn box according to the dimensions given from public variables
    /// </summary>
    private void SpawnContainer()
    {
        // Spawn ground
        GameObject ground = Instantiate(groundPrefab, Vector3.zero, transform.rotation);
        groundXSize = Random.Range(groundMinXSize, groundMaxXSize);
        groundZSize = Random.Range(groundMinZSize, groundMaxZSize);

        startOfGroundX = -groundXSize / 2;
        endOfGroundX = groundXSize / 2;
        startOfGroundZ = -groundZSize / 2;
        endOfGroundZ = groundZSize / 2;

        ground.transform.localScale = new Vector3(groundXSize, 1, groundZSize);
        ground.transform.parent = environmentContainer.transform;
        // Spawn outside walls (4)
        outsideWallSize = Random.Range(outsideWallsMinYSize, outsideWallsMaxYSize);
        GameObject outsideWall = Instantiate(wallPrefab, new Vector3(startOfGroundX, outsideWallSize / 2, 0), transform.rotation);
        outsideWall.transform.localScale = new Vector3(1, outsideWallSize, groundZSize);
        outsideWall.transform.parent = environmentContainer.transform;

        outsideWall = Instantiate(wallPrefab, new Vector3(endOfGroundX, outsideWallSize / 2, 0), transform.rotation);
        outsideWall.transform.localScale = new Vector3(1, outsideWallSize, groundZSize);
        outsideWall.transform.parent = environmentContainer.transform;

        outsideWall = Instantiate(wallPrefab, new Vector3(0, outsideWallSize / 2, startOfGroundZ), transform.rotation);
        outsideWall.transform.localScale = new Vector3(groundXSize, outsideWallSize, 1);
        outsideWall.transform.parent = environmentContainer.transform;

        outsideWall = Instantiate(wallPrefab, new Vector3(0, outsideWallSize / 2, endOfGroundZ), transform.rotation);
        outsideWall.transform.localScale = new Vector3(groundXSize, outsideWallSize, 1);
        outsideWall.transform.parent = environmentContainer.transform;

        // Spawn roof
        GameObject roof = Instantiate(roofPrefab, new Vector3(0, outsideWallSize, 0), transform.rotation);
        roof.transform.localScale = new Vector3(groundXSize, 1, groundZSize);
        roof.transform.parent = environmentContainer.transform;
    }

    /// <summary>
    /// Sets up and starts a coroutine for spawning buildings
    /// </summary>
    private void SpawnBuildings()
    {
        xOffset = Random.Range(perlinOffsetMin, perlinOffsetMax);
        yOffset = Random.Range(perlinOffsetMin, perlinOffsetMax);
        buildingCount = Random.Range(buildingMinCount, buildingMaxCount);
        StartCoroutine(SpawnBuilding(0));
    }

    /// <summary>
    /// Spawn a building according to the dimensions given from public variables.
    /// Recursivly calls itself until it has spawned a set amount of buildings (set in SpawnBuildings)
    /// </summary>
    /// <param name="counter"></param>
    /// <returns></returns>
    private IEnumerator SpawnBuilding(int counter)
    {
        int errorCatchCounter;
        float xCoord, yCoord, zCoord, xSize, ySize, zSize;
        GameObject currentBuilding;
        Vector3 buildingCenter = Vector3.zero;

        if (counter >= buildingCount)
            yield break;

        yield return new WaitForEndOfFrame();

        errorCatchCounter = 0;
        do
        {
            // Stop if while loop has gone for more than 1000 iterations in which case break out of recursive coroutine 
            // because there is likely no more room for buildings
            errorCatchCounter++;
            if (errorCatchCounter > 1000)
            {
                buildingCount = counter;
                yield break;
            }
            // Find size and coords
            xSize = Random.Range(buildingMinX, buildingMaxX);
            zSize = Random.Range(buildingMinZ, buildingMaxZ);
            xCoord = Random.Range(startOfGroundX, endOfGroundX);
            zCoord = Random.Range(startOfGroundZ, endOfGroundZ);

            ySize = GeneratePerlinNoise(xCoord, zCoord) * buildingMaxY;
            yCoord = ySize / 2 + .7f;
            buildingCenter = new Vector3(xCoord, yCoord, zCoord);
        // Check if building is going to morph into another buidling
        } while (CheckIfBuilding(buildingCenter, new Vector3(spaceBetweenBuildingsMultiplier * xSize, yCoord, spaceBetweenBuildingsMultiplier * zSize)));

        currentBuilding = Instantiate(buildingPrefab, buildingCenter, Quaternion.Euler(0, 0, 0));
        currentBuilding.transform.localScale = new Vector3(xSize, ySize, zSize);
        // Parent object to container
        currentBuilding.transform.parent = environmentContainer.transform;

        StartCoroutine(SpawnBuilding(counter + 1));
    }

    /// <summary>
    /// Returns true if there is a collider in the dimensions/coords given
    /// </summary>
    private bool CheckIfBuilding(Vector3 center, Vector3 size)
    {
        return Physics.CheckBox(center, size / 2, Quaternion.Euler(0, 0, 0));
    }

    /// <summary>
    /// Returns a the perlin noise value for the cooresponding x and y values
    /// </summary>
    private float GeneratePerlinNoise(float x, float y)
    {
        float xCoord = (float)x / groundXSize * scale + xOffset;
        float yCoord = (float)y / groundZSize * scale + yOffset;

        return Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord), 0, 1);
    }

    /// <summary>
    /// Spawns ground and wall lights based on the public variables
    /// </summary>
    private void SpawnLights()
    {
        GameObject light;
        int groundLightCount = Random.Range(groundLightMinCount, groundLightMaxCount);
        int wallLightCount = Random.Range(perWallLightMinCount, perWallLightMaxCount);
        int xCoord, zCoord, yCoord;

        xCoord = startOfGroundX;
        // Ground lights
        for (int i = 0; i < groundLightCount; i++)
        {
            xCoord += (groundXSize / groundLightCount);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            light = Instantiate(groundLightPrefab, new Vector3(xCoord, yCoord, zCoord), groundLightPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
            light.GetComponent<Light>().range = yCoord + 100;
            light.transform.parent = environmentContainer.transform;
        }
        zCoord = 0;
        // Wall lights right
        for (int i = 0; i < wallLightCount; i++)
        {
            xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightRightPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightRightPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
            light.transform.parent = environmentContainer.transform;
        }
        // Wall lights left
        for (int i = 0; i < wallLightCount; i++)
        {
            xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightLeftPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightLeftPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
            light.transform.parent = environmentContainer.transform;
        }
        xCoord = 0;
        // Wall lights forward
        for (int i = 0; i < wallLightCount; i++)
        {
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightForwardPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightForwardPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
            light.transform.parent = environmentContainer.transform;
        }
        zCoord = startOfGroundZ + (groundZSize * 2 / groundLightCount);
        // Wall lights backward
        for (int i = 0; i < wallLightCount; i++)
        {
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightBackwardPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightBackwardPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
            light.transform.parent = environmentContainer.transform;
        }
    }

    /// <summary>
    /// Spawns 'suns' based on the given public variables
    /// </summary>
    private void SpawnSuns()
    {
        GameObject currentSun;
        int sunCount = Random.Range(sunCountMin, sunCountMax);
        int sunRadius;
        int xCoord, yCoord, zCoord;

        for (int i = 0; i < sunCount; i++)
        {
            do
            {
                xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
                yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
                zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
                sunRadius = Random.Range(sunRadiusMin, sunRadiusMax);
            } while (CheckIfSun(new Vector3(xCoord, yCoord, zCoord), sunRadius));
            currentSun = Instantiate(sunPrefab, new Vector3(xCoord, yCoord, zCoord), transform.rotation);
            currentSun.transform.localScale *= sunRadius;
            currentSun.GetComponent<Renderer>().material = sunMaterials[Random.Range(0, sunMaterials.Length)];
            currentSun.transform.parent = environmentContainer.transform;
        }
    }

    /// <summary>
    /// Returns true if there is a collider in the dimensions/coords given
    /// </summary>
    private bool CheckIfSun(Vector3 center, float radius)
    {
        return Physics.CheckSphere(center, radius * 5);
    }

    /// <summary>
    /// Returns a Random Color in the value type Color
    /// </summary>
    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

}
