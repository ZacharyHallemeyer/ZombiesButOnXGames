using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MysteryBox : MonoBehaviour
{
    public bool IsInteractable { get; private set; } = true;
    public bool IsWeaponSpawned { get; private set; } = false;
    public int MysteryBoxPrice { get; private set; } = 3000;

    public PlayerShooting playerShooting;
    public TextMeshProUGUI text;

    // This array must be the same order as PlayerShooting.gunNames
    public GameObject[] gunPrefabArray;

    private GameObject spawnedGun;
    private Coroutine selfDestructCoroutine;
    private Coroutine hideUICoroutine;
    public string gunName;

    /// <summary>
    /// get a reference to both UI text and playerShooting
    /// </summary>
    private void Start()
    {
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();
        if (text == null)
            text = GameObject.Find("InteractableUI").GetComponent<TextMeshProUGUI>();

    }

    /// <summary>
    /// Changes to text of the mysterbox UI to reflect its current state
    /// </summary>
    public void ShowUI()
    {
        if(IsWeaponSpawned)
        {
            text.text = "Press E To Pick Up Weapon";
        }
        else
        {
            text.text = "Press E TO Purchase Weapon For " + MysteryBoxPrice + " Points";
        }

        if(hideUICoroutine == null)
        {
            hideUICoroutine = StartCoroutine(HideUI());
        }
    }

    /// <summary>
    /// Resets UI text if player has not looked by mysterybox for more than .5 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideUI()
    {
        yield return new WaitForSeconds(.5f);
        if (!playerShooting.CheckIfMysterBoxInFront() || !IsInteractable)
        {
            hideUICoroutine = null;
            text.text = "";
        }
        else
            hideUICoroutine = StartCoroutine(HideUI());
    }

    /// <summary>
    /// Spawns a gun that the player does not currently have
    /// Dependencies: GunAscendingAnimation
    /// </summary>
    public void SpawnRandomWeapon()
    {
        int index;
        IsInteractable = false;
        do
        {
            index = Random.Range(0, gunPrefabArray.Length);
            gunName = playerShooting.gunNames[index];

        } while (gunName.Equals(playerShooting.currentGun.name) || gunName.Equals(playerShooting.secondaryGun.name));
        
        spawnedGun = Instantiate(gunPrefabArray[index], transform.position, gunPrefabArray[index].transform.rotation);
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
        {
            StopCoroutine(selfDestructCoroutine);
        }
        Destroy(spawnedGun);
        IsWeaponSpawned = false;
        return gunName;
    }

    /// <summary>
    /// Raises the gun up by a 1.5 units on the y axis.
    /// This is accomplished by recursively calling itself and incrementing the counter by 1 each call
    /// </summary>
    /// <param name="counter">  </param>
    /// <returns></returns>
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
            IsInteractable = true;
            selfDestructCoroutine = StartCoroutine(SelfDestructGun());
        }
    }

}
