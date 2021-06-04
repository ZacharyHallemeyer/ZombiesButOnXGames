using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIScript : MonoBehaviour
{
    // Grapple UI
    public Slider grappleSlider;
    public Gradient grappleGradient;
    public Image grappleFill;

    // Health UI
    public Image healthImage;

    // Ammo UI
    public TextMeshProUGUI ammoText;

    // Wave UI
    public TextMeshProUGUI waveText;

    // Points UI
    public TextMeshProUGUI pointMainText;
    public TextMeshProUGUI pointDisappearingText;
    private Coroutine pointHideText;
    private int oldPoint;
    private int pointAccumulator;

    // Lives UI
    public TextMeshProUGUI livesText;

    // ShockWave UI
    public TextMeshProUGUI shockWaveText;

    // Grenade UI
    public TextMeshProUGUI grenadeText;

    // Grapple =============================
    public void SetMaxGrapple(float maxValue)
    {
        grappleSlider.maxValue = maxValue;
        grappleSlider.value = maxValue;

        grappleFill.color = grappleGradient.Evaluate(1f);
    }

    public void SetGrapple(float grappleTime)
    {
        grappleSlider.value = grappleTime;
        grappleFill.color = grappleGradient.Evaluate(grappleSlider.normalizedValue);
    }

    // End of Grapple ======================

    // Health ==============================

    public void ChangeHealthUI(float currentHealth, float maxHealth)
    {
        float ratio = currentHealth / maxHealth;

        healthImage.color = new Color(healthImage.color.r, 
                                      healthImage.color.g, healthImage.color.b, (1f - ratio) / 2);
    }

    // End of Health =======================

    // Ammo ================================

    public void ChangeGunUIText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    // End of Ammo =========================

    // Wave ================================

    public IEnumerator NewWaveUI(int waveNumber)
    {
        waveText.text = "Wave " + waveNumber;
        yield return new WaitForSeconds(3f);
        waveText.text = "";
    }

    // End of Wave =========================

    // Point ===============================

    public void SetPointText(int points)
    {
        pointMainText.text = points.ToString();
        ChangeDisappearingText(points);
    }

    public void ChangeDisappearingText(int points)
    {
        if (points - oldPoint > 0)
        {
            pointAccumulator += points - oldPoint;
            pointDisappearingText.text = "+" + pointAccumulator;
        }
        else
            pointDisappearingText.text = (points - oldPoint).ToString();
        oldPoint = points;

        if (pointHideText != null)
            StopCoroutine(pointHideText);

        pointHideText = StartCoroutine(HideText());
    }

    public IEnumerator HideText()
    {
        yield return new WaitForSeconds(1f);
        pointAccumulator = 0;
        pointDisappearingText.text = "";
    }

    // End of Point ========================

    // Lives ===============================

    public void SetLivesText(int lives)
    {
        livesText.text = "LIVES: " + lives;
    }

    // End of Lives ========================

    // Shock Wave ==========================

    public void SetShockWaveText(int amount)
    {
        shockWaveText.text = "SHOCKWAVE: " + amount;
    }

    // End of Shock Wave ===================

    // Grenade =============================

    public void SetGrenadeText(int amount)
    {
        grenadeText.text = "GRENADES: " + amount;
    }

    // End of Grenade
}
