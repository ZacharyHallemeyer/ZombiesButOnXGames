using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationEnvironment : MonoBehaviour
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
    public float buildingMinX, buildingMaxX, buildingXSize, buildingXCoord;
    public float buildingMinY, buildingMaxY, buildingYSize, buildingYCoord;
    public float buildingMinZ, buildingMaxZ, buildingZSize, buildingZCoord;
    public int buildingMinCount, buildingMaxCount, buildingCount;

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
    }

    private void SpawnBuildings()
    {
        buildingCount = Random.Range(buildingMinCount, buildingMaxCount);
        StartCoroutine(SpawnBuilding(0));
    }

    private IEnumerator SpawnBuilding(int counter)
    {
        int errorCatchCounter;
        GameObject currentBuilding;
        Vector3 buildingCenter = Vector3.zero, buildingSize = Vector3.zero;

        if (counter >= buildingCount)
            yield break;

        yield return new WaitForEndOfFrame();

        errorCatchCounter = 0;
        do
        {
            errorCatchCounter++;
            if (errorCatchCounter > 1000)
            {
                buildingCount = counter;
                yield break;
            }
            buildingXSize = Random.Range(buildingMinX, buildingMaxX);
            buildingYSize = Random.Range(buildingMinY, buildingMaxY);
            buildingZSize = Random.Range(buildingMinZ, buildingMaxZ);
            buildingSize = new Vector3(buildingXSize, buildingYSize, buildingZSize);

            buildingXCoord = Random.Range(startOfGroundX, endOfGroundX);
            buildingYCoord = buildingYSize / 2 + .7f;
            buildingZCoord = Random.Range(startOfGroundZ, endOfGroundZ);
            buildingCenter = new Vector3(buildingXCoord, buildingYCoord, buildingZCoord);
        } while (CheckIfBuilding(buildingCenter, buildingSize));

        currentBuilding = Instantiate(buildingPrefab, buildingCenter, Quaternion.Euler(0, 0, 0));
        currentBuilding.transform.localScale = buildingSize;

        // Spawn one mystery box
        if(counter == 0)
        {
            Instantiate(mysteryBoxPrefab,
                        new Vector3(buildingXCoord, buildingYSize + 1.5f, buildingZCoord),
                        mysteryBoxPrefab.transform.rotation);
        }

        StartCoroutine(SpawnBuilding(counter + 1));
    }

    private bool CheckIfBuilding(Vector3 center, Vector3 size)
    {
        return Physics.CheckBox(center, size/2, Quaternion.Euler(0, 0, 0));
    }

    private void SpawnLights()
    {
        GameObject light;
        int groundLightCount = Random.Range(groundLightMinCount, groundLightMaxCount);
        int wallLightCount = Random.Range(perWallLightMinCount, perWallLightMaxCount);
        int xCoord, zCoord, yCoord;

        // Ground lights
        for (int i = 0; i < groundLightCount; i++)
        {
            //xCoord += (groundXSize * 2  / groundLightCount) + Random.Range(10,spaceBetweenWallAndLight);
            xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            light = Instantiate(groundLightPrefab, new Vector3(xCoord, yCoord, zCoord), groundLightPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
        }
        zCoord = 0;
        // Wall lights right
        for (int i = 0; i < wallLightCount; i++)
        {
            //xCoord += (groundXSize * 2 / groundLightCount);
            xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightRightPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightRightPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
        }
        // Wall lights left
        for (int i = 0; i < wallLightCount; i++)
        {
            //xCoord += (groundXSize * 2 / groundLightCount);
            xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightLeftPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightLeftPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
        }
        xCoord = 0;
        // Wall lights forward
        for (int i = 0; i < wallLightCount; i++)
        {
            //zCoord += (groundZSize * 2 / groundLightCount);
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightForwardPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightForwardPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
        }
        zCoord = startOfGroundZ + (groundZSize * 2 / groundLightCount);
        // Wall lights backward
        for (int i = 0; i < wallLightCount; i++)
        {
            //zCoord += (groundZSize * 2 / groundLightCount);
            zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
            yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
            light = Instantiate(wallLightBackwardPrefab, new Vector3(xCoord, yCoord, zCoord), wallLightBackwardPrefab.transform.rotation);
            light.GetComponent<Light>().color = RandomColor();
        }
    }

    private void SpawnSuns()
    {
        GameObject currentSun;
        int sunCount = Random.Range(sunCountMin, sunCountMax);
        int sunRadius;
        int xCoord, yCoord, zCoord;

        for(int i = 0; i < sunCount; i++)
        {
            do
            {
                xCoord = Random.Range(startOfGroundX + spaceBetweenWallAndLight, endOfGroundX - spaceBetweenWallAndLight);
                yCoord = Random.Range(outsideWallSize / 2, 3 * outsideWallSize / 4);
                zCoord = Random.Range(startOfGroundZ + spaceBetweenWallAndLight, endOfGroundZ - spaceBetweenWallAndLight);
                sunRadius = Random.Range(sunRadiusMin, sunRadiusMax);
            } while (CheckIfSun(new Vector3(xCoord, yCoord, zCoord), sunRadius)) ;
            currentSun = Instantiate(sunPrefab, new Vector3(xCoord, yCoord, zCoord), transform.rotation);
            currentSun.transform.localScale *= sunRadius;
        }
    }

    private bool CheckIfSun(Vector3 center, float radius)
    {
        return Physics.CheckSphere(center, radius * 5);
    }

    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

} 