using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    public Sprite[] status;
    void Update() 
    {
        gameObject.GetComponent<SpriteRenderer>().sprite= status[GameManager.Instance.playerLevel];
    }
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
                var tempEnemy = other.gameObject.GetComponent<Enemy>();
                var tempEnemy2 = other.gameObject.GetComponent<Death>();
                if(tempEnemy.useBarrage==AttackType.suicideAttack)
                    tempEnemy2.Die();
                if(!tempPlayer.isInvincible&&tempEnemy.canTouch)
                    tempPlayer.Die();
                break;
        }
    }
}
