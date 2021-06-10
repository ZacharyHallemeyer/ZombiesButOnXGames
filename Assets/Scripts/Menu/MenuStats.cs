using UnityEngine;
using TMPro;

public class MenuStats : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI highestWaveText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI highestPointsText;
    public TextMeshProUGUI highestTimeText;

    /// <summary>
    /// Sets easy stats to easy high scores
    /// </summary>
    public void UpdateEasyStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "EASY MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.easyHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.easyTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.easyHighestPoints;
            highestTimeText.text = "HIGHEST TIME: " + playerData.easyHighestTimeSurvived;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
            highestTimeText.text = "HIGHEST TIME: 0";
        }
    }

    /// <summary>
    /// Sets hard stats to hard high scores
    /// </summary>
    public void UpdateHardStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "HARD MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.hardHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.hardTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.hardHighestPoints;
            highestTimeText.text = "HIGHEST TIME: " + playerData.hardHighestTimeSurvived;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
            highestTimeText.text = "HIGHEST TIME: 0";
        }
    }

    /// <summary>
    /// Sets insane stats to insane high scores
    /// </summary>
    public void UpdateInsaneStatsBoard()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        title.text = "INSANE MODE STATS";
        if (playerData != null)
        {
            highestWaveText.text = "HIGHEST WAVE: " + playerData.insaneHighScoreWave;
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: " + playerData.insaneTotalEnemiesKilled;
            highestPointsText.text = "HIGHEST POINTS: " + playerData.insaneHighestPoints;
            highestTimeText.text = "HIGHEST TIME: " + playerData.insaneHighestTimeSurvived;
        }
        else
        {
            highestWaveText.text = "HIGHEST WAVE: 0";
            enemiesKilledText.text = "TOTAL ENEMIES KILLED: 0";
            highestPointsText.text = "HIGHEST POINTS: 0";
            highestTimeText.text = "HIGHEST TIME: 0";
        }
    }
}
