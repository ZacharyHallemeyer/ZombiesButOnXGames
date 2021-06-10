using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveEnemyStats : MonoBehaviour
{
    public Vector3 originalScale;

    // Scripts
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);

        // Change size
        originalScale = new Vector3(transform.localScale.x,
                                           transform.localScale.y * Random.Range(.5f, 1.5f),
                                           transform.localScale.z);
        transform.localScale = originalScale;
        InvokeRepeating("TurnOnRB", 5f, 0.1f );
    }

    private void TurnOnRB()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        CancelInvoke("TurnOnRB");
    }
}
