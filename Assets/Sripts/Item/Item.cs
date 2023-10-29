using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EnemyDie,
    Life,
    Bottom
}
public class Item : MonoBehaviour
{
    public ItemType itemType;
    public int score;
    public int exp;
}
