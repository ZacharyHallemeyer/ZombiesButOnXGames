using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableObjects : MonoBehaviour
{
    public class InteractableObject
    {
        public string name;
        public bool isInteractable;
        public int price;
        public int priceIncrement;
    }

    public InteractableObject mysteryBox = new InteractableObject
    {
        name = "MysteryBox",
        isInteractable = true,
        price = 1000,
    };

    public InteractableObject upgradeGrapple = new InteractableObject
    {
        name = "UpgradeGrapple",
        isInteractable = true,
        price = 1000,
        priceIncrement = 200,
    };

    public InteractableObject upgradeDamage = new InteractableObject
    {
        name = "UpgradeDamage",
        isInteractable = true,
        price = 1000,
        priceIncrement = 100,
    };

    public InteractableObject shockWaveIncrease = new InteractableObject
    {
        name = "ShockWaveIncrease",
        isInteractable = true,
        price = 100,
        priceIncrement = 100,
    };

    public InteractableObject extraLife = new InteractableObject
    {
        name = "ExtraLife",
        isInteractable = true,
        price = 2500,
    };    

    public InteractableObject refillAmmo = new InteractableObject
    {
        name = "RefillAmmo",
        isInteractable = true,
        price = 2500,
        priceIncrement = 100,
    };

    public InteractableObject grenadeIncrease = new InteractableObject
    {
        name = "GrenadeIncrease",
        isInteractable = true,
        price = 100,
    };

    public InteractableObject exitShop = new InteractableObject
    {
        name = "ExitShop",
        isInteractable = true,
        price = 0,
    };


    // General variables
    public PlayerShooting playerShooting;
    public PlayerStats playerStats;
    public TextMeshProUGUI text;
    public PlayerUIScript playerUI;
    private GameManager gameManager;

    private InteractableObject currentObject;

    // Mystery Box variables
    // This array must be the same order as PlayerShooting.gunNames
    public GameObject[] gunPrefabArray;
    private GameObject spawnedGun;
    private Transform mysteryBoxTransform;
    private Coroutine selfDestructCoroutine;
    public string gunName;
    public bool IsWeaponSpawned { get; private set; } = false;

    /// <summary>
    /// get a reference to both UI text and playerShooting
    /// </summary>
    private void Start()
    {
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (text == null)
            text = GameObject.Find("InteractableUI").GetComponent<TextMeshProUGUI>();

    }

    /// <summary>
    /// Changes to text of the iteractable UI to reflect what interactable object player is currently looking at
    /// </summary>
    public void ShowUI(string name)
    {
        switch (name)
        {
            case "MysteryBox":
                currentObject = mysteryBox;
                if (IsWeaponSpawned)
                    text.text = "Interact To Pick Up Weapon";
                else
                    text.text = "Interact To Purchase Weapon For " + mysteryBox.price + " Points";
                break;
            case "UpgradeGrapple":
                currentObject = upgradeGrapple;
                text.text = "Interact to Purchase " + 1f + " More Seconds Of Grapple Time For " + upgradeGrapple.price + " Points";
                break;
            case "UpgradeDamage":
                currentObject = upgradeDamage;
                text.text = "Interact to Purchase " + 1f + " More Damage For Current Gun For " + upgradeDamage.price + " Points";
                break;
            case "ShockWaveIncrease":
                currentObject = shockWaveIncrease;
                text.text = "Interact to Purchase A Shock Wave Item For " + shockWaveIncrease.price + " Points";
                break;
            case "ExtraLife":
                currentObject = extraLife;
                text.text = "Interact to Purchase An Extra Life For " + extraLife.price + " Points";
                break;
            case "RefillAmmo":
                currentObject = refillAmmo;
                text.text = "Interact To Refill Ammo For " + refillAmmo.price + " Points";
                break;
            case "GrenadeIncrease":
                currentObject = grenadeIncrease;
                text.text = "Interact To Purchase A Grenade For " + grenadeIncrease.price + " Points";
                break;
            case "ExitShop":
                text.text = "Interact To Exit Shop";
                break;
            default:
                return;
        }

        // Reset HideUI
        CancelInvoke("HideUI");
        InvokeRepeating("HideUI", .5f, .5f);
    }

    /// <summary>
    /// Resets UI text if player has not looked at an interactable object for more than .5 seconds
    /// or the object player is currently looking at is not currently interactable
    /// </summary>
    private void HideUI()
    {
        if (!playerStats.CheckForInteractableObject() || !currentObject.isInteractable)
        {
            text.text = "";
            CancelInvoke("HideUI");
        }
    }

    /// <summary>
    /// Spawns a gun that the player does not currently have
    /// Dependencies: GunAscendingAnimation
    /// </summary>
    public void SpawnRandomWeapon()
    {
        if(mysteryBoxTransform == null)
            mysteryBoxTransform = GameObject.FindGameObjectWithTag("MysteryBox").GetComponent<Transform>();
        mysteryBox.price += mysteryBox.priceIncrement;
        int index;
        mysteryBox.isInteractable = false;
        do
        {
            index = Random.Range(0, gunPrefabArray.Length);
            gunName = playerShooting.gunNames[index];

        } while (gunName.Equals(playerShooting.currentGun.name) || gunName.Equals(playerShooting.secondaryGun.name));
        
        spawnedGun = Instantiate(gunPrefabArray[index], mysteryBoxTransform.position, gunPrefabArray[index].transform.rotation);
        IsWeaponSpawned = true;
        StartCoroutine(GunAscendingAnimation(0));
    }

    /// <summary>
    /// Destroys spawned gun after 10 second
    /// Dependencies: DestroyGun
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelfDestructGun()
    {
        yield return new WaitForSeconds(10f);
        DestroyGun();
    }

    /// <summary>
    /// Destroys spawned gun and resets UI
    /// This function also returns the name of the gun
    /// </summary>
    public string DestroyGun()
    {
        text.text = "";
        if(selfDestructCoroutine != null)
            StopCoroutine(selfDestructCoroutine);
        Destroy(spawnedGun);
        IsWeaponSpawned = false;
        return gunName;
    }

    /// <summary>
    /// Raises the gun up by a 1.5 units on the y axis.
    /// This is accomplished by recursively calling itself and incrementing the counter by 1 each call
    /// </summary>
    /// <param name="counter"> counter to be incremented by one each iteration </param>
    private IEnumerator GunAscendingAnimation(int counter)
    {
        yield return new WaitForSeconds(.01f);
        if (counter < 150)
        {
            spawnedGun.transform.position += Vector3.up * .01f;
            StartCoroutine(GunAscendingAnimation(counter + 1));
        }
        else
        {
            mysteryBox.isInteractable = true;
            StartCoroutine(GunDecendingAnimation(0));
            selfDestructCoroutine = StartCoroutine(SelfDestructGun());
        }
    }

    /// <summary>
    /// Raises the gun down by a 1.5 units on the y axis.
    /// This is accomplished by recursively calling itself and incrementing the counter by 1 each call
    /// </summary>
    /// <param name="counter"> counter to be incremented by one each iteration </param>
    private IEnumerator GunDecendingAnimation(int counter)
    {
        yield return new WaitForSeconds(10f / 100f);
        if( counter < 150)
        {
            if (spawnedGun == null)
                yield break;
            spawnedGun.transform.position -= Vector3.up * .01f;
            StartCoroutine(GunDecendingAnimation(counter + 1));
        }
    }

    // END OF MYSTERY BOX ==============================================

    // UPGRADE DAMAGE ==================================================

    /// <summary>
    /// Increases damage of current gun by 1/2 of its current damage
    /// </summary>
    public void UpgradeDamage()
    {
        upgradeDamage.price += upgradeDamage.priceIncrement;
        currentObject.isInteractable = false;
        foreach (string key in playerShooting.allGunInformation.Keys)
        {
            if(playerShooting.currentGun.name == key)
            {
                playerShooting.allGunInformation[key].damage += playerShooting.allGunInformation[key].damage / 2;
            }
        }
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF UPGRADE DAMAGE ===========================================

    // UPGRADE GRAPPLE =================================================

    /// <summary>
    /// Increases max grapple time by .25 seconds
    /// </summary>
    public void UpgradeGrapple()
    {
        upgradeGrapple.price += upgradeGrapple.priceIncrement;
        currentObject.isInteractable = false;
        playerShooting.maxGrappleTime += .25f;
        // Adjust grapple slider and start grapple recovery
        playerUI.grappleSlider.maxValue = playerShooting.maxGrappleTime;
        if(!playerShooting.IsGrappleRecoveryInProgress)
        {
            playerShooting.IsGrappleRecoveryInProgress = true;
            playerShooting.InvokeRepeating("GrappleRecovery", 0, .1f);
        }
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF UPGRADE GRAPPLE ==========================================

    // REFILL AMMO =====================================================

    /// <summary>
    /// Refills the ammo of player's current weapon
    /// </summary>
    public void RefillAmmo()
    {
        refillAmmo.price += refillAmmo.priceIncrement;
        currentObject.isInteractable = false;
        playerShooting.currentGun.reserveAmmo = playerShooting.currentGun.ammoIncrementor;
        playerShooting.currentGun.currentAmmo = playerShooting.currentGun.magSize;
        playerUI.ChangeGunUIText(playerShooting.currentGun.currentAmmo, playerShooting.currentGun.reserveAmmo);
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF REFILL AMMO ==============================================

    // GRENADE INCREASE ================================================

    public void GrenadeIncrease()
    {
        currentObject.isInteractable = false;
        playerShooting.CurrentGrenades++;
        playerUI.SetGrenadeText(playerShooting.CurrentGrenades);
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF GRENADE INCREASE =========================================

    // SHOCK WAVE SHOP =================================================

    /// <summary>
    /// Increases the amount of shock wave items has by one
    /// </summary>
    public void IncreaseShockWaveItem()
    {
        shockWaveIncrease.price += shockWaveIncrease.priceIncrement;
        currentObject.isInteractable = false;
        playerShooting.CurrentShockWaves++;
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF SHOCK WAVE SHOP ==========================================

    // EXTRA LIFE ======================================================

    /// <summary>
    /// Increases teh amount of lives that player has by one
    /// </summary>
    public void IncreaseAmountOfLives()
    {
        currentObject.isInteractable = false;
        playerStats.CurrentLives++;
        StartCoroutine(TurnOnInteractable(currentObject));
    }

    // END OF EXTRA LIFE ===============================================

    // EXIT SHOP =======================================================

    /// <summary>
    /// Teleports to main room
    /// Dependencies: GameManager.ChangeToMainRoom();
    /// </summary>
    public void ExitShop()
    {
        gameManager.ChangeToMainRoom();
    }

    // END OF EXIT SHOP ================================================


    /// <summary>
    /// Sets 'isInteractable' to true after 1 seconds of the object given in parems
    /// </summary>
    private IEnumerator TurnOnInteractable(InteractableObject currentObject)
    {
        yield return new WaitForSeconds(1f);
        currentObject.isInteractable = true;
    }
}
