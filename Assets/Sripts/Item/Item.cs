using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EXP,
    Life,
    Bomb,
    Drone
}
public class Item : MonoBehaviour
{
    public ItemType itemType;
}
