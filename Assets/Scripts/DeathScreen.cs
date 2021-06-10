using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public TextMeshProUGUI waveText, killsText, pointsText;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        waveText.text = "WAVE: " + PlayerPrefs.GetInt("LastWaveNumber", 0);
        killsText.text = "KILLS: " + PlayerPrefs.GetInt("LastKillNumber", 0);
        pointsText.text = "POINTS: " + PlayerPrefs.GetInt("LastPointNumber", 0);
    }

    public void PlayAgain()
    {
        if(PlayerPrefs.GetString("LastMode", "WaveMode") == "WaveMode")
            SceneManager.LoadScene("WaveMode");
        else
            SceneManager.LoadScene("SurvivalMode");
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
