using UnityEngine;
using TMPro;

public class MenuStats : MonoBehaviour
{
    public TextMeshProUGUI highestWaveText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI highestPointsText;

    private void Awake()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        if(playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.highScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.totalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.highestPoints;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
        }
    }
}
