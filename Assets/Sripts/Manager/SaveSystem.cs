using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SaveSystem : MonoBehaviour
{
    const string GAME_DATA = "Assets/game_SaveData/Game.game";
    public static void SaveGame(object data)
    {
        if (!Directory.Exists("Assets/game_SaveData"))
        {
            Directory.CreateDirectory("Assets/game_SaveData");
        }
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(GAME_DATA);
        File.WriteAllText(path, json);
    }
    public static T LoadGame<T>()
    {
        var path = Path.Combine(GAME_DATA);
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
