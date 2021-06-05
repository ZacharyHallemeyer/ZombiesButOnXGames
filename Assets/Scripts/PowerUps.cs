using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public GameObject nukeExplosion;

    private void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("PowerUp");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        switch (tag)
        {
            case "MaxAmmo":
                ActivateMaxAmmo();
                break;
            case "DoublePoints":
                ActivateDoublePoints();
                break;
            case "Nuke":
                ActivateNuke();
                break;
            default:
                Debug.LogError("Wrong tag :(");
                break;
        }
    }

    /// <summary>
    /// Resets ammo for current and secondary gun and updates UI. Also 5 grenades are added and 3 shockwaves are added
    /// </summary>
    private void ActivateMaxAmmo()
    {
        PlayerShooting playerShooting = FindObjectOfType<PlayerShooting>();
        playerShooting.currentGun.currentAmmo = playerShooting.currentGun.magSize;
        playerShooting.currentGun.reserveAmmo = playerShooting.currentGun.ammoIncrementor;
        playerShooting.secondaryGun.currentAmmo = playerShooting.secondaryGun.magSize;
        playerShooting.secondaryGun.reserveAmmo = playerShooting.secondaryGun.ammoIncrementor;
        playerShooting.playerUI.ChangeGunUIText(playerShooting.currentGun.currentAmmo,
                                                playerShooting.currentGun.reserveAmmo);
        playerShooting.CurrentGrenades += 5;
        playerShooting.CurrentShockWaves += 3;
        playerShooting.playerUI.SetGrenadeText(playerShooting.CurrentGrenades);
        playerShooting.playerUI.SetShockWaveText(playerShooting.CurrentShockWaves);

        Destroy(gameObject);
    }

    /// <summary>
    /// Changes point multiplier in PlayerStats to 2 and Invokes "DeactivateDoublePoints" with a 30 second timer
    /// Dependencies: PlayerStats class
    /// </summary>
    private void ActivateDoublePoints()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        // Restart timer if player picks up another double point power up before the first one stops
        playerStats.CancelInvoke("DeactivateDoublePoints");
        playerStats.PointMultiplier = 2;
        playerStats.InvokeRepeating("DeactivateDoublePoints", 30f, .01f);
        Destroy(gameObject);
    }

    /// <summary>
    /// 'kills' all gameobjects with the tag "Enemy" in the scene
    /// Dependencies: EnemyStats class, PlayerStats class
    /// </summary>
    private void ActivateNuke()
    {
        // Add 500 points to player points
        FindObjectOfType<PlayerStats>().CurrentPoints += 500;
        // disable mesh renderer and box collider because explosion particle system is a child of nuke
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        nukeExplosion.SetActive(true);
        EnemyStats[] enemyStats = FindObjectsOfType<EnemyStats>();
        foreach(EnemyStats enemy in enemyStats)
            enemy.Death();
        InvokeRepeating("DestroyNuke", 2, 0);
    }

    private void DestroyNuke()
    {
        Destroy(gameObject);
    }
}
