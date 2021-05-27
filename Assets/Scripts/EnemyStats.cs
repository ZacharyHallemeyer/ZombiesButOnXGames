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

    // Scripts
    private PlayerStats playerStats;

    private void Awake()
    {
        health = MaxHealth;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    /// <summary>
    /// subtracts damage from health and destorys enemy object if enemy
    /// health is less than or equal to 0
    /// Dependencies: ShowDamage
    /// </summary>
    /// <param name="damage">variable to subtract from emeny health</param>
    public void TakeDamage(float damage)
    {
        playerStats.CurrentPoints += (int) damage;
        playerStats.ChangeInPointValue();
        health -= damage;

        if(!showDamageActive)
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
        meshRenderer.material = showDamageMaterial;
        yield return new WaitForSeconds(showDamageDuration);
        meshRenderer.material = normalMaterial;
        showDamageActive = false;
    }
}
