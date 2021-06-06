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
    public int easyHighestTimeSurvived;

    // Hard mode
    public int hardHighScoreWave;
    public int hardTotalEnemiesKilled;
    public int hardHighestPoints;
    public int hardHighestTimeSurvived;

    // Insane mode
    public int insaneHighScoreWave;
    public int insaneTotalEnemiesKilled;
    public int insaneHighestPoints;
    public int insaneHighestTimeSurvived;
}
