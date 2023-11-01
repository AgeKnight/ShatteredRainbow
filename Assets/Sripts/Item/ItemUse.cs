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
            GameManager.Instance.EatItem(temp.itemType);
            Die(temp.gameObject);
        }
    }
    void Die(GameObject temp)
    {
        Destroy(temp.gameObject);
    }
}
