using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int BaseDamage { private set; get; } = 10;

    public MeshRenderer meshRenderer;
    private readonly float showDamageDuration = .2f;
    private bool showDamageActive = false;

    public float MaxHealth { set; get; } = 50f;
    public float health;

    // 0D00FF 8400FF 8D0043 00FF98
    public Color[] colors;
    public Color baseColor, damagedColor;

    public Vector3 originalScale;

    // Scripts
    public EnemyMovement enemyMovement;
    private PlayerStats playerStats;
    public AudioSource audioSource;

    // Components
    public BoxCollider boxCollider;
    public Rigidbody rb;

    private void Awake()
    {
        health = MaxHealth;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        audioSource.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);

        // Change size
        originalScale = new Vector3(transform.localScale.x,
                                           transform.localScale.y * Random.Range(.5f, 1.5f),
                                           transform.localScale.z);
        transform.localScale = originalScale;

        // Change colors
        baseColor = colors[Random.Range(0, colors.Length)];
        meshRenderer.material.color = baseColor;
    }

    /// subtracts damage from health and destorys enemy object if enemy
    /// health is less than or equal to 0
    /// Dependencies: ShowDamage, Death
    public void TakeDamage(float damage)
    {
        playerStats.CurrentPoints += (int) damage * playerStats.PointMultiplier;
        playerStats.ChangeInPointValue();
        health -= damage;

        if (!showDamageActive)
            StartCoroutine(ShowDamage());

        if (health <= 0)
        {
            Death();
        }
    }

    /// Changes the material of enemy to a different material than switches back
    /// to original material after showDamageDuration (float) amount of seconds
    private IEnumerator ShowDamage()
    {
        showDamageActive = true;
        meshRenderer.material.color = damagedColor;
        yield return new WaitForSeconds(showDamageDuration);
        meshRenderer.material.color = baseColor;
        showDamageActive = false;
    }

    public void Death()
    {
        playerStats.TotalEnemiesKilled++;
        enemyMovement.enabled = false;
        boxCollider.enabled = false;
        InvokeRepeating("DeathAnimation", 0f, .01f);
    }

    private void DeathAnimation()
    {
        transform.localRotation *= Quaternion.Euler(0f, 0f, 1.5f);
        if(transform.localEulerAngles.z > 350)
        {
            CancelInvoke("DeathAnimation");
            Destroy(gameObject);
        }
    }
}
