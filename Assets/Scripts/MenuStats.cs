using UnityEngine;
using TMPro;

public class MenuStats : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI highestWaveText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI highestPointsText;

    private void Awake()
    {
        /*PlayerData playerData = SaveSystem.LoadPlayerData();
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
        }*/
    }

    public void UpdateEasyStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "EASY MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.easyHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.easyTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.easyHighestPoints;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
        }
    }

    public void UpdateHardStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "HARD MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.hardHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.hardTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.hardHighestPoints;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
        }
    }

    public void UpdateInsaneStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "INSANE MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.insaneHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.insaneTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.insaneHighestPoints;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
        }
    }
}
