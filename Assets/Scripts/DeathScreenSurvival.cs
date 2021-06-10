using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreenSurvival : DeathScreenWave
{
    public TextMeshProUGUI timeText;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timeText.text = "TIME: " + PlayerPrefs.GetInt("LastTimeNumber", 0);
    }

    public override void PlayAgain()
    {
        SceneManager.LoadScene("SurvivalMode");
    }
}
