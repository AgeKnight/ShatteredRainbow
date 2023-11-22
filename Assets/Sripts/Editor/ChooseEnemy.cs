using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy),true)]
public class ChooseEnemy : Editor
{
    SerializedObject obj;
    Enemy enemy;
    List<string> propertyNames;
    SerializedProperty iterator;
    UseBarrageType useBarrage;
    MoveType moveType;
    Dictionary<string, UseBarrageType> specialPropertys = new Dictionary<string, UseBarrageType>
        {
            { "bullet", UseBarrageType.useBarrage},
            { "allBarragecount", UseBarrageType.useBarrage},
            { "spanCount", UseBarrageType.useBarrage},
            { "countTime", UseBarrageType.useBarrage},
            { "otherAttackType", UseBarrageType.nonUse},
        };
    Dictionary<string, MoveType> specialPropertys2 = new Dictionary<string, MoveType>
        {
             { "allMoveTime", MoveType.SomeTimesMove},
             { "otherAttackType", MoveType.MoveToTarget},
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
        enemy = (Enemy)target;
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out useBarrage)
                && useBarrage != enemy.useBarrage)
                continue;
            if (specialPropertys2.TryGetValue(name, out moveType) 
                && moveType != enemy.moveType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}