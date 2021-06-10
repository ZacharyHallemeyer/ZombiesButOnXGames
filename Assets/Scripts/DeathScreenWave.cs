using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenWave : MonoBehaviour
{
    public TextMeshProUGUI waveText, killsText, pointsText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        waveText.text = "WAVE: " + PlayerPrefs.GetInt("LastWaveNumber", 0);
        killsText.text = "KILLS: " + PlayerPrefs.GetInt("LastKillNumber", 0);
        pointsText.text = "POINTS: " + PlayerPrefs.GetInt("LastPointNumber", 0);
    }

    /// <summary>
    /// Reloads WaveMode scene
    /// </summary>
    public virtual void PlayAgain()
    {
        SceneManager.LoadScene("WaveMode");
    }

    /// <summary>
    /// Loads MainMenu scene
    /// </summary>
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
