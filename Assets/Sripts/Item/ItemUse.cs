using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="Item")
        {
            var temp = other.gameObject.GetComponent<Item>();
            int ScoreValue = temp.score;
            int ExpValue = temp.exp;
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
