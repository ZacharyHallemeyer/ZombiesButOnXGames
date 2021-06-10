using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles every kind of power up
/// </summary>
public class PowerUps : MonoBehaviour
{
    // Nuke variables
    public GameObject pickUpExplosion;
    public ParticleSystem powerUpParticles;

    private void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("PowerUpSpawn");
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
        FindObjectOfType<AudioManager>().Play("PowerUpPickUp");

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

        // disable mesh renderer and box collider because explosion particle system is a child of power up
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        powerUpParticles.Stop();
        pickUpExplosion.SetActive(true);

        InvokeRepeating("DestroyObject", 2, 0);
        FindObjectOfType<PlayerUIScript>().ChangePowerUpUI("MaxAmmo");
    }

    /// <summary>
    /// Changes point multiplier in PlayerStats to 2 and Invokes "DeactivateDoublePoints" with a 30 second timer
    /// Dependencies: PlayerStats class
    /// </summary>
    private void ActivateDoublePoints()
    {
        FindObjectOfType<AudioManager>().Play("PowerUpPickUp");

        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        // Restart timer if player picks up another double point power up before the first one stops
        playerStats.CancelInvoke("DeactivateDoublePoints");
        playerStats.PointMultiplier = 2;
        playerStats.InvokeRepeating("DeactivateDoublePoints", 30f, .01f);

        // disable mesh renderer and box collider because explosion particle system is a child of power up
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        powerUpParticles.Stop();
        pickUpExplosion.SetActive(true);

        InvokeRepeating("DestroyObject", 2, 0);
        FindObjectOfType<PlayerUIScript>().ChangePowerUpUI("DoublePoints");
    }

    /// <summary>
    /// 'kills' all gameobjects with the tag "Enemy" in the scene
    /// Dependencies: EnemyStats class, PlayerStats class
    /// </summary>
    private void ActivateNuke()
    {
        FindObjectOfType<AudioManager>().Play("PowerUpPickUp");

        // Add 500 points to player points
        FindObjectOfType<PlayerStats>().CurrentPoints += 500;
        FindObjectOfType<PlayerStats>().ChangeInPointValue();

        // disable mesh renderer and box collider because explosion particle system is a child of power up
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        powerUpParticles.Stop();
        pickUpExplosion.SetActive(true);

        // 'Kill' all enemies in scene
        EnemyStats[] enemyStats = FindObjectsOfType<EnemyStats>();
        foreach(EnemyStats enemy in enemyStats)
            enemy.Death();

        InvokeRepeating("DestroyObject", 2, 0);
        FindObjectOfType<PlayerUIScript>().ChangePowerUpUI("Nuke");
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
