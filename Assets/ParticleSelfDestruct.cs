using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
