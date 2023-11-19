using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bullet),true)]
public class ChooseBullet : Editor
{
    SerializedObject obj;
    Bullet bullet;
    List<string> propertyNames;
    SerializedProperty iterator;
    BulletType bulletType;
    Dictionary<string,BulletType> specialPropertys= new Dictionary<string, BulletType>
        {
            { "canTrackEnemy", BulletType.Player},
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
        bullet = (Bullet)target;
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out bulletType) 
                && bulletType != bullet.bulletType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
