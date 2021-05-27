using UnityEngine;
using UnityEngine.UI;

public class HealthUIScript : MonoBehaviour
{
    public Image image;

    public void ChangeHealthUI(float currentHealth, float maxHealth)
    {
        float ratio = currentHealth / maxHealth;

        image.color = new Color (image.color.r, image.color.g, image.color.b, (1f - ratio)/2);
    }
}
