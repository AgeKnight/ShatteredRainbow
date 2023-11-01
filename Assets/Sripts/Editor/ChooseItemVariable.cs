using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Item))]
public class ChooseItemVariable : Editor
{
    SerializedObject obj;
    Item item;
    List<string> propertyNames;
    SerializedProperty iterator;
    ItemType itemType;
    Dictionary<string,ItemType> specialPropertys= new Dictionary<string, ItemType>
        {
            { "expType", ItemType.EXP},
            { "Exp", ItemType.EXP},
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
        item = (Item)target;
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out itemType) 
                && itemType != item.itemType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
