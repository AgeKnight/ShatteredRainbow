using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Item":
                var temp = other.gameObject.GetComponent<Item>();
                GameManager.Instance.EatItem(temp);
                temp.Die();
                break;
            case "Enemy":
                var tempPlayer = this.transform.parent.gameObject.GetComponent<Death>();
                if(other.gameObject.GetComponent<TrashAttack1>())
                {
                    var tempEnemy = other.gameObject.GetComponent<Death>();
                    tempEnemy.Die();
                }
                tempPlayer.Die();
                break;
        }
    }
}
