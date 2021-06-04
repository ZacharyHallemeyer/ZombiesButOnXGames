using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int highScoreWave;
    public int totalEnemiesKilled;
    public int highestPoints;

    public PlayerData(int highScoreWave, int enemiesKilled, int highestPoints)
    {
        this.highScoreWave = highScoreWave;
        this.totalEnemiesKilled = enemiesKilled;
        this.highestPoints = highestPoints;
    }
}
