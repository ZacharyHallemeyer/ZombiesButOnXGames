using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Rigidbody rb;

    public GameObject explosionParticle;
    public GameObject explosionColliderPrefab;
    public GameObject explosionCollider;

    public int explosionColliderMaxRadius;
    public int grenadeDamage = 50;

    void Start()
    {
        StartCoroutine(WaitAndExplode());
    }

    private IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(3f);

        // Play explosion particle system
        Instantiate(explosionParticle, transform.position, transform.rotation);

        // Expand blast wave
        explosionCollider = Instantiate(explosionColliderPrefab, transform.position, transform.rotation);
        InvokeRepeating("StartExplosion", 0f, .01f);
    }

    private void StartExplosion()
    {
        explosionCollider.transform.localScale *= 1.1f;
        if(explosionCollider.transform.localScale.x > explosionColliderMaxRadius)
        {
            CancelInvoke("StartExplosion");
            StartCoroutine(SelfDestruct());
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(explosionCollider);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Player"))
            rb.velocity = Vector3.zero;
    }
}
