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
    Dictionary<string,CharatorType> specialPropertys= new Dictionary<string, CharatorType>
        {
            { "status", CharatorType.Player},
            { "hpBar", CharatorType.Boss },
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
                && charatorType != death.type)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
