using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static string path = Application.persistentDataPath + "/PlayerData.jazz";

    /// <summary>
    /// Save player data
    /// </summary>
    public static void SavePlayerData(int highScore, int enemiesKilled, int highestPoints)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream;
        PlayerData playerData = new PlayerData(0, 0, 0);

        Debug.Log(path);

        if (File.Exists(path))
        {
            fileStream = new FileStream(path, FileMode.Open);

            playerData = formatter.Deserialize(fileStream) as PlayerData;
            playerData.highScoreWave = highScore;
            playerData.totalEnemiesKilled += enemiesKilled;
            playerData.highestPoints = highestPoints;
            Debug.Log("old file");
        }
        else
        {
            fileStream = new FileStream(path, FileMode.Create);
            playerData = new PlayerData(highScore, enemiesKilled, highestPoints);
            Debug.Log("new file");
        }


        formatter.Serialize(fileStream, playerData);
        fileStream.Close();
    }

    /// <summary>
    /// Returns player data in save file if file exists. If it does not exists null is return
    /// </summary>
    public static PlayerData LoadPlayerData()
    {
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            PlayerData playerData = formatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();

            return playerData;
        }
        else
        {
            return null;
        }
    }
}
