using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum CharatorType
{
    Player,
    Enemy,
    Boss
}
public class Death : MonoBehaviour
{
    public int totalHp;
    public CharatorType type;
    public Sprite[] status;
    public Slider hpBar;
    [HideInInspector]
    public int hp;
    void Awake()
    {
        hp = totalHp;
    }
    public void Hurt()
    {
        hp -= 1;
        if (type == CharatorType.Player)
        {
            if (!gameObject.GetComponent<Player>().isInvincible && hp > 0)
            {
                var sprite = GameObject.Find("Image").GetComponent<SpriteRenderer>(); ;
                sprite.sprite = status[totalHp - hp];
            }
        }
        if (type == CharatorType.Boss)
        {
            hpBar.value = (float)hp / totalHp;
        }
        if (hp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerIsDied = true;
            GameManager.Instance.PlayerDiePosition = gameObject.transform.position;
            GameManager.Instance.AddLife(-1);
            GameManager.Instance.ClearBullet();
            Destroy(this.gameObject);
        }
        if (gameObject.tag == "Enemy")
        {
            var temp = gameObject.GetComponent<Enemy>();
            temp.ClearBarrage();
            if (temp.gameObject.GetComponent<Item>())
            {
                var tempItem = temp.gameObject.GetComponent<Item>();
                GameManager.Instance.EatItem(tempItem.itemType, tempItem.score, tempItem.exp);
            }
            Destroy(this.gameObject);
        }
    }
}
