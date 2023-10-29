using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    int ScoreValue=0;
    int ExpValue=0;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="Item")
        {
            var temp = other.gameObject.GetComponent<Item>();
            if(temp.itemType==ItemType.EnemyDie)
            {
                ScoreValue = temp.score;
                ExpValue = temp.exp;
            }
            GameManager.Instance.EatItem(temp.itemType,ScoreValue,ExpValue);
            if(temp.itemType!=ItemType.EnemyDie)
            {
                Die(other.gameObject);
            }
        }
    }
    void Die(GameObject tempgGmeObject)
    {
        Destroy(tempgGmeObject);
    }
}
