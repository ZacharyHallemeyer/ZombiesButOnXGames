using System.Collections;
using UnityEngine;
using TMPro;

public class WaveUIScript : MonoBehaviour
{
    //public TextMeshPro text;

    public TextMeshProUGUI text;
    public IEnumerator NewWaveUI(int waveNumber)
    {
        text.text = "Wave " + waveNumber;
        yield return new WaitForSeconds(3f);
        text.text = "";
    }
}
