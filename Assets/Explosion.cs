using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Grenade
{

    private int explosiveForce = 25;
    public AudioSource audioSource;
    public LayerMask whatIsObstacle;

    private void Start()
    {
           audioSource.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (IsObstacle(other.transform)) return;
            other.attachedRigidbody.AddExplosionForce(explosiveForce, transform.position, 
                                                      explosionColliderMaxRadius, .1f, ForceMode.Impulse);

            EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(grenadeDamage);
            Physics.IgnoreCollision(other.GetComponent<BoxCollider>(), gameObject.GetComponent<SphereCollider>());
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            if (IsObstacle(other.transform)) return;
            other.attachedRigidbody.AddExplosionForce(explosiveForce, transform.position,
                                          explosionColliderMaxRadius, 3f, ForceMode.Impulse);
            PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
            playerStats.TakeDamage(grenadeDamage/4);
            Physics.IgnoreCollision(other.GetComponent<CapsuleCollider>(), gameObject.GetComponent<SphereCollider>());
        }
    }

    /// <summary>
    /// Returns true if there is a obstacle between enemy and player
    /// Unless the enemy is above the player in which case function will 
    /// return false
    /// </summary>
    private bool IsObstacle(Transform target)
    {
        // minus around 5 from max distance to prevent viewing wall behind player as an obstacle
        if (Physics.Raycast(transform.position, target.position - transform.position,
            Vector3.Distance(transform.position, target.position), whatIsObstacle))
            return true;

        return false;
    }
}
