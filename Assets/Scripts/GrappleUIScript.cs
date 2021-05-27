using UnityEngine;
using UnityEngine.UI;

public class GrappleUIScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    public void SetMaxGrapple(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetGrapple(float grappleTime)
    {
        slider.value = grappleTime;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
