using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenWave : MonoBehaviour
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

    public virtual void PlayAgain()
    {
        SceneManager.LoadScene("WaveMode");
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
