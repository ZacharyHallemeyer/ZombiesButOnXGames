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
    public TextMeshProUGUI grappleOutOfBoundsText;

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

    // Power Up UI
    public TextMeshProUGUI powerUpText;

    // Crouch UI
    public Image crouchImage;

    // Death UI
    public TextMeshProUGUI deathText;

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

    public void GrappleOutOfBoundsUI()
    {
        grappleOutOfBoundsText.text = "X";
        InvokeRepeating("HideGrappleOutOfBoundsUI", 1f, 0f);
    }

    private void HideGrappleOutOfBoundsUI()
    {
        grappleOutOfBoundsText.text = "";
        CancelInvoke("HideGrappleOutOfBoundsUI");
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

    // Power Up  ===========================

    public void ChangePowerUpUI(string powerUpName)
    {
        switch (powerUpName)
        {
            case "DoublePoints":
                powerUpText.text = "DOUBLE POINTS";
                break;

            case "MaxAmmo":
                powerUpText.text = "MAX AMMO";
                break;

            case "Nuke":
                powerUpText.text = "NUKE";
                break;
                
            default:
                Debug.LogError("Power up name not found");
                break;
        }

        InvokeRepeating("HidePowerUpUI", 3f, 0f);
    }

    public void HidePowerUpUI()
    {
        powerUpText.text = "";
        CancelInvoke("HidePowerUpUI");
    }

    // End of Power Up  ====================

    // Crouch ==============================

    public void ChangeToCrouch()
    {
        CancelInvoke("ChangeToStandAnim");
        InvokeRepeating("ChangeToCrouchAnim", 0f, .01f);
    }

    /// <summary>
    /// Compress square that represents player state. Must be called with Invoke repeating
    /// </summary>
    public void ChangeToCrouchAnim()
    {
        if (crouchImage.rectTransform.sizeDelta.y > 25)
        {
            crouchImage.rectTransform.anchoredPosition -= new Vector2(0f, .25f);
            crouchImage.rectTransform.sizeDelta -= Vector2.up;
        }
        else
        {
            crouchImage.rectTransform.sizeDelta = new Vector2(50, 25);
            CancelInvoke("ChangeToCrouchAnim");
        }
    }

    public void ChangeToStand()
    {
        CancelInvoke("ChangeToCrouchAnim");
        InvokeRepeating("ChangeToStandAnim", 0f, .01f);
    }

    /// <summary>
    /// Expands square that represents player state. Must be called with Invoke repeating
    /// </summary>
    public void ChangeToStandAnim()
    {
        if(crouchImage.rectTransform.sizeDelta.y < 50)
        {
            crouchImage.rectTransform.localPosition += new Vector3(0f, .25f, 0f);
            crouchImage.rectTransform.sizeDelta += Vector2.up;
        }
        else
        {
            crouchImage.rectTransform.sizeDelta = new Vector2(50, 50);
            CancelInvoke("ChangeToStandAnim");
        }
    }

    // End of Crouch =======================

    // Death ===============================

    public void ChangeDeathUI(int currentAmountOfLives)
    {
        if(currentAmountOfLives == 1)
            deathText.text = currentAmountOfLives.ToString() + " MORE CHANCE!";
        else
            deathText.text = currentAmountOfLives.ToString() + " MORE CHANCES!";
        InvokeRepeating("HideDeathUI", 3f, 0f);
    }

    public void HideDeathUI()
    {
        deathText.text = "";
        CancelInvoke("HideDeathUI");
    }

    // End of Death ========================
}
