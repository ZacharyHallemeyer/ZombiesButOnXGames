using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static string path = Application.persistentDataPath + "/PlayerData.jazz";

    /// <summary>
    /// Save player data
    /// </summary>
    public static void SavePlayerData(PlayerData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream;

        fileStream = new FileStream(path, FileMode.Create);

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
