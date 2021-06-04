using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public CapsuleCollider playerCollider;
    
    // Shock Wave
    public GameObject shockWavePrefab;
    private GameObject shockWave;
    public int sphereMaxRadius;

    public ParticleSystem shockWaveParticle;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        // Spawn on player
        shockWave = Instantiate(shockWavePrefab, transform.position, transform.rotation);
        Physics.IgnoreCollision(playerCollider, shockWave.GetComponent<SphereCollider>() );
    }

    /// <summary>
    /// plays sound and particle system and starts shock wave
    /// Dependiencies: ExpandShockWave
    /// </summary>
    public void StartShockWave()
    {
        // Move to player 
        audioManager.Play("GunShockWave");
        shockWaveParticle.Play();
        shockWave.transform.position = transform.position;
        shockWave.transform.localScale = Vector3.one;
        InvokeRepeating("ExpandShockWave", 0f, .01f);
    }

    /// <summary>
    /// Expands shockwave(sphere w/ only a sphere collider) scale
    /// </summary>
    private void ExpandShockWave()
    {
        shockWave.transform.localScale *= 1.1f;
        if (shockWave.transform.localScale.x > sphereMaxRadius)
        {
            shockWave.transform.localScale = Vector3.zero;
            CancelInvoke("ExpandShockWave");
        }
    }
}
