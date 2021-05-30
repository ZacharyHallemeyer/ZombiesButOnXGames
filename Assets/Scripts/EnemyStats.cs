using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int BaseDamage { private set; get; } = 10;

    public MeshRenderer meshRenderer;
    public Material normalMaterial;
    public Material showDamageMaterial;
    private readonly float showDamageDuration = .2f;
    private bool showDamageActive = false;

    public float MaxHealth { set; get; } = 50f;
    public float health;

    // 0D00FF 8400FF 8D0043 00FF98
    public Color[] colors;
    public Color baseColor, damagedColor;

    // Scripts
    private PlayerStats playerStats;

    private void Awake()
    {
        health = MaxHealth;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        // Change size
        transform.localScale = new Vector3(transform.localScale.x,
                                           transform.localScale.y * Random.Range(.5f, 1.5f),
                                           transform.localScale.z);

        // Change colors
        baseColor = colors[Random.Range(0, colors.Length)];
        meshRenderer.material.color = baseColor;
    }

    /// <summary>
    /// subtracts damage from health and destorys enemy object if enemy
    /// health is less than or equal to 0
    /// Dependencies: ShowDamage
    /// </summary>
    /// <param name="damage">variable to subtract from emeny health</param>
    public void TakeDamage(float damage)
    {
        playerStats.CurrentPoints += (int)damage;
        playerStats.ChangeInPointValue();
        health -= damage;

        if (!showDamageActive)
            StartCoroutine(ShowDamage());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Changes the material of enemy to a different material than switches back
    /// to original material after showDamageDuration (float) amount of seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowDamage()
    {
        showDamageActive = true;
        meshRenderer.material.color = damagedColor;
        yield return new WaitForSeconds(showDamageDuration);
        meshRenderer.material.color = baseColor;
        showDamageActive = false;
    }
}
