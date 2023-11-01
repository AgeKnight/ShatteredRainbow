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
public enum ExpType
{
    Small,
    Middle,
    Big,
}
public class Item : MonoBehaviour
{
    public ItemType itemType;
    public ExpType expType;
    public int Exp;
}
