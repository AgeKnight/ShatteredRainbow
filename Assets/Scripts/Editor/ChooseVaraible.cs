using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Death))]
public class ChooseVaraible : Editor
{
    SerializedObject obj;
    Death death;
    List<string> propertyNames;
    SerializedProperty iterator;
    CharatorType charatorType;
    EnemyType enemyType;
    Dictionary<string,CharatorType> specialPropertys= new Dictionary<string, CharatorType>
        {
            { "Status", CharatorType.Player},
            { "enemyType", CharatorType.Enemy},
            { "score", CharatorType.Enemy},
            { "Exps", CharatorType.Enemy},
            { "countTime", CharatorType.Enemy},
            { "minExp", CharatorType.Enemy},
            { "maxExp", CharatorType.Enemy},
            { "expObject", CharatorType.Enemy},
            { "ultimateAttack", CharatorType.Enemy},
            { "bonusScore", CharatorType.Enemy},
            { "Bonus", CharatorType.Enemy},
            { "probability", CharatorType.Enemy},
            
        };
    Dictionary<string,EnemyType> specialPropertys2= new Dictionary<string, EnemyType>
        {
            { "bonusScore", EnemyType.Boss},
            { "ultimateAttack", EnemyType.Boss},
            { "Bonus", EnemyType.Trash},
            
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
        death = (Death)target;
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out charatorType) 
                && charatorType != death.charatorType)
                continue;
            if (specialPropertys2.TryGetValue(name, out enemyType) 
                && enemyType != death.enemyType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
