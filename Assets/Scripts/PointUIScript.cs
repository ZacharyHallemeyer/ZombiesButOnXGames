using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointUIScript : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI disappearingText;
    private Coroutine hideText;

    private int oldPoint;

    public void SetPointText(int points)
    {
        mainText.text = points.ToString();
        ChangeDisappearingText(points);
    }

    public void ChangeDisappearingText(int points)
    {
        if(points > 0)
            disappearingText.text = "+" + (points - oldPoint);
        else
            disappearingText.text = (points - oldPoint).ToString();
        oldPoint = points;

        if(hideText != null)
        {
            StopCoroutine(hideText);
        }
        hideText = StartCoroutine(HideText());
    }

    public IEnumerator HideText()
    {
        yield return new WaitForSeconds(1f);
        disappearingText.text = "";
    }
}
