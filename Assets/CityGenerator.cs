using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject buildingPrefab;
    public GameObject groundPrefab;
    public GameObject roofPrefab;
    public GameObject groundLightPrefab;
    public GameObject mysteryBoxPrefab;
    public GameObject wallLightRightPrefab;
    public GameObject wallLightLeftPrefab;
    public GameObject wallLightForwardPrefab;
    public GameObject wallLightBackwardPrefab;
    public GameObject sunPrefab;

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
    public int width, height, scale;
    public int perlinOffsetMin, perlinOffsetMax, xOffset, yOffset;
    public int index = 0;

    // Testing variables
    private GameObject[] buildings = new GameObject[1000];

    public void Start()
    {
        xOffset = Random.Range(perlinOffsetMin, perlinOffsetMax);
        yOffset = Random.Range(perlinOffsetMin, perlinOffsetMax);
        SpawnContainer();
        buildingCount = Random.Range(buildingMinCount, buildingMaxCount);
        StartCoroutine(SpawnBuilding(0));
        //SpawnPerlinNoise();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            scale++;
            foreach(GameObject building in buildings)
            {
                Destroy(building);
            }
            buildings = new GameObject[1000];
            SpawnPerlinNoise();
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            scale++;
            foreach (GameObject building in buildings)
                Destroy(building);
            buildings = new GameObject[1000];
            index = 0;
            StartCoroutine(SpawnBuilding(0));
        }    
    }


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

        // Spawn outside walls (4)
        outsideWallSize = Random.Range(outsideWallsMinYSize, outsideWallsMaxYSize);
        GameObject outsideWall = Instantiate(buildingPrefab, new Vector3(startOfGroundX, outsideWallSize / 2, 0), transform.rotation);
        outsideWall.transform.localScale = new Vector3(1, outsideWallSize, groundZSize);

        outsideWall = Instantiate(buildingPrefab, new Vector3(endOfGroundX, outsideWallSize / 2, 0), transform.rotation);
        outsideWall.transform.localScale = new Vector3(1, outsideWallSize, groundZSize);

        outsideWall = Instantiate(buildingPrefab, new Vector3(0, outsideWallSize / 2, startOfGroundZ), transform.rotation);
        outsideWall.transform.localScale = new Vector3(groundXSize, outsideWallSize, 1);

        outsideWall = Instantiate(buildingPrefab, new Vector3(0, outsideWallSize / 2, endOfGroundZ), transform.rotation);
        outsideWall.transform.localScale = new Vector3(groundXSize, outsideWallSize, 1);


        // Spawn roof
        GameObject roof = Instantiate(roofPrefab, new Vector3(0, outsideWallSize, 0), transform.rotation);
        roof.transform.localScale = new Vector3(groundXSize, 1, groundZSize);
        width = groundXSize;
        height = groundZSize;
    }


    private IEnumerator SpawnBuilding(int counter)
    {
        Debug.Log("SpawnBuilding is called");
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
            errorCatchCounter++;
            if (errorCatchCounter > 1000)
            {
                Debug.Log("Error caught");
                buildingCount = counter;
                yield break;
            }
            xSize = Random.Range(buildingMinX, buildingMaxX);
            //ySize = Random.Range(buildingMinY, buildingMaxY);
            zSize = Random.Range(buildingMinZ, buildingMaxZ);
            xCoord = Random.Range(startOfGroundX, endOfGroundX);
            zCoord = Random.Range(startOfGroundZ, endOfGroundZ);

            ySize = GeneratePerlinNoise(xCoord, zCoord) * 100;
            yCoord = ySize / 2 + .7f;

            buildingCenter = new Vector3(xCoord, yCoord, zCoord);
        } while (CheckIfBuilding(buildingCenter, new Vector3(spaceBetweenBuildingsMultiplier * xSize, yCoord, spaceBetweenBuildingsMultiplier * zSize)));

        currentBuilding = Instantiate(buildingPrefab, buildingCenter, Quaternion.Euler(0, 0, 0));
        currentBuilding.transform.localScale = new Vector3(xSize, ySize, zSize);
        buildings[index] = currentBuilding;
        index++;

        StartCoroutine(SpawnBuilding(counter + 1));
    }

    private bool CheckIfBuilding(Vector3 center, Vector3 size)
    {
        return Physics.CheckBox(center, size / 2, Quaternion.Euler(0, 0, 0));
    }

    private void SpawnPerlinNoise()
    {
        GameObject currentBuilding;
        float ySize;
        int index = 0;
        for (int x = 0; x < width; x += 10)
        {
            for(int z = 0; z < height; z += 10)
            {
                ySize = GeneratePerlinNoise(x, z) * 100;
                currentBuilding = Instantiate(buildingPrefab, new Vector3(x, ySize / 2, z), transform.rotation);
                currentBuilding.transform.localScale = new Vector3(5, ySize, 5);
                buildings[index] = currentBuilding;
                index++;
            }
        }
    }


    private float GeneratePerlinNoise(float x, float y)
    {
        float xCoord = (float) x / width * scale + xOffset;
        float yCoord = (float) y / height * scale + yOffset;

        return Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord), 0, 1);
    }

}
