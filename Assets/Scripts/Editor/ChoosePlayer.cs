using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player), true)]
public class ChoosePlayer : Editor
{
    SerializedObject obj;
    Player player;
    List<string> propertyNames;
    SerializedProperty iterator;
    PlayerType playerType;
    Dictionary<string, PlayerType> specialPropertys = new Dictionary<string, PlayerType>
        {
            { "vivyBarrageTrans", PlayerType.vyles},
            { "BumbNums", PlayerType.vyles},
            { "LilyGatherTime", PlayerType.Lily},
            {"MaxGatherTime", PlayerType.Lily},
            {"maxLazerTime", PlayerType.Lily},
            {"around_speed", PlayerType.Lily},
            {"DroneRLevil", PlayerType.Lil_Void},
            {"DroneLLevil", PlayerType.Lil_Void},
        };
    Dictionary<string, PlayerType> specialPropertys2 = new Dictionary<string, PlayerType>
        {
            { "bulletTransform", PlayerType.vyles},
        };
    Dictionary<string, PlayerType> specialPropertys3 = new Dictionary<string, PlayerType>
        {
            {"bulletTransform", PlayerType.Lil_Void},
        };
    void OnEnable()
    {
        obj = new SerializedObject(target);
        iterator = obj.GetIterator();
        iterator.NextVisible(true);
        propertyNames = new List<string>();
        do
        {
            propertyNames.Add(iterator.name);
        } while (iterator.NextVisible(false));
        player = (Player)target;
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out playerType)
                && playerType != player.playerType)
                continue;
            if (specialPropertys2.TryGetValue(name, out playerType)
                && playerType == player.playerType)
                continue;
            if (specialPropertys3.TryGetValue(name, out playerType)
                && playerType == player.playerType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
