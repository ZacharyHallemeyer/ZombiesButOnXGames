using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Easy mode
    public int easyHighScoreWave;
    public int easyTotalEnemiesKilled;
    public int easyHighestPoints;

    // Hard mode
    public int hardHighScoreWave;
    public int hardTotalEnemiesKilled;
    public int hardHighestPoints;

    // Insane mode
    public int insaneHighScoreWave;
    public int insaneTotalEnemiesKilled;
    public int insaneHighestPoints;

    public PlayerData(string difficulty, int highScoreWave, int enemiesKilled, int highestPoints)
    {
        switch (difficulty)
        {
            case "Easy":
                easyHighScoreWave = highScoreWave;
                easyTotalEnemiesKilled = enemiesKilled;
                easyHighestPoints = highestPoints;
                
                break;
            case "Hard":
                hardHighScoreWave = highScoreWave;
                hardTotalEnemiesKilled = enemiesKilled;
                hardHighestPoints = highestPoints;
                
                break;
            case "Insane":
                insaneHighScoreWave = highScoreWave;
                insaneTotalEnemiesKilled = enemiesKilled;
                insaneHighestPoints = highestPoints;

                break;
            default:
                Debug.LogError("Difficulty not found");
                break;
        }
    }
}
